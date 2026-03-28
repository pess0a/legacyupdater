using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LegacyUpdater
{
    /// <summary>Informações retornadas pela API de versão.</summary>
    public class VersionInfo
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>URL do update.zip retornada pela API.</summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>Responsável por download, extração e verificação de versão.</summary>
    public class Updater
    {
        private static readonly HttpClient _http;

        static Updater()
        {
            _http = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(Config.DOWNLOAD_TIMEOUT_MINUTES)
            };
            _http.DefaultRequestHeaders.Add(
                "User-Agent",
                $"{Config.GAME_NAME} Updater/{Config.UPDATER_VERSION}");
        }

        // ----------------------------------------------------------------
        // Versão
        // ----------------------------------------------------------------

        /// <summary>
        /// Consulta a API REST e retorna as informações da versão mais recente.
        /// Espera resposta JSON: { "version": "x.y.z", "date": "yyyy-mm-dd", "url": "https://..." }
        /// </summary>
        public async Task<VersionInfo> GetRemoteVersionInfoAsync(CancellationToken ct = default)
        {
            var json = await _http.GetStringAsync(Config.VERSION_API_URL);
            var info = JsonConvert.DeserializeObject<VersionInfo>(json);

            if (info == null || string.IsNullOrWhiteSpace(info.Version))
                throw new InvalidOperationException("A API retornou uma resposta inválida.");

            return info;
        }

        /// <summary>
        /// Consulta a API de base e retorna as informações do arquivo base.zip.
        /// Espera resposta JSON: { "version": "x.y.z", "date": "yyyy-mm-dd", "url": "https://..." }
        /// </summary>
        public async Task<VersionInfo> GetBaseInfoAsync(CancellationToken ct = default)
        {
            var json = await _http.GetStringAsync(Config.BASE_ZIP_URL);
            var info = JsonConvert.DeserializeObject<VersionInfo>(json);

            if (info == null || string.IsNullOrWhiteSpace(info.Url))
                throw new InvalidOperationException("A API de base retornou uma resposta inválida.");

            return info;
        }

        /// <summary>Retorna a versão instalada localmente, ou null se não houver.</summary>
        public string GetLocalVersion()
        {
            var path = Path.Combine(Config.INSTALL_DIR, Config.VERSION_FILE);
            if (!File.Exists(path)) return null;
            return File.ReadAllText(path).Trim();
        }

        /// <summary>Salva a versão instalada no arquivo local.</summary>
        public void SaveLocalVersion(string version)
        {
            Directory.CreateDirectory(Config.INSTALL_DIR);
            File.WriteAllText(
                Path.Combine(Config.INSTALL_DIR, Config.VERSION_FILE),
                version);
        }

        /// <summary>Remove o arquivo de versão local, forçando re-download na próxima execução.</summary>
        public void ClearLocalVersion()
        {
            var path = Path.Combine(Config.INSTALL_DIR, Config.VERSION_FILE);
            if (File.Exists(path)) File.Delete(path);
        }

        // ----------------------------------------------------------------
        // Download
        // ----------------------------------------------------------------

        /// <summary>
        /// Baixa um arquivo de <paramref name="url"/> para <paramref name="destPath"/>,
        /// reportando o progresso (0–100) via <paramref name="progress"/>.
        /// </summary>
        public async Task DownloadFileAsync(
            string url,
            string destPath,
            IProgress<int> progress,
            CancellationToken ct = default)
        {
            using (var response = await _http.GetAsync(
                url, HttpCompletionOption.ResponseHeadersRead, ct))
            {
                response.EnsureSuccessStatusCode();

                var total = response.Content.Headers.ContentLength ?? -1L;
                var dir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var file = new FileStream(
                    destPath, FileMode.Create, FileAccess.Write,
                    FileShare.None, 81920, useAsync: true))
                {
                    var buffer = new byte[81920];
                    long downloaded = 0;
                    int read;

                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                    {
                        await file.WriteAsync(buffer, 0, read, ct);
                        downloaded += read;

                        if (total > 0)
                            progress?.Report((int)(downloaded * 100L / total));
                    }

                    // Detecta download truncado quando o servidor informou o tamanho
                    if (total > 0 && downloaded != total)
                    {
                        TryDelete(destPath);
                        throw new IOException(
                            $"Download incompleto de {Path.GetFileName(destPath)}: " +
                            $"esperado {total} bytes, recebido {downloaded} bytes. " +
                            "Verifique sua conexão e tente novamente.");
                    }
                }
            }

            // Valida a estrutura completa do ZIP antes de tentar extrair.
            // Detecta tanto respostas de erro do servidor (HTML) quanto downloads truncados.
            ValidateZipFile(destPath);

            progress?.Report(100);
        }

        /// <summary>
        /// Abre o ZIP e valida sua estrutura completa (incluindo o diretório central).
        /// Detecta arquivos truncados que passariam em uma verificação simples de magic bytes.
        /// </summary>
        private static void ValidateZipFile(string path)
        {
            try
            {
                using (ZipFile.OpenRead(path)) { }
            }
            catch (InvalidDataException ex)
            {
                TryDelete(path);
                throw new InvalidDataException(
                    $"O arquivo baixado está corrompido ou incompleto: {Path.GetFileName(path)}. " +
                    "O download pode ter sido interrompido ou o servidor enviou um arquivo inválido. " +
                    "Tente novamente.", ex);
            }
        }

        // ----------------------------------------------------------------
        // Extração
        // ----------------------------------------------------------------

        /// <summary>
        /// Extrai <paramref name="zipPath"/> para <paramref name="destDir"/>,
        /// sobrescrevendo arquivos existentes.
        /// Reporta progresso (0–100) via <paramref name="progress"/>.
        /// </summary>
        public Task ExtractZipAsync(
            string zipPath,
            string destDir,
            IProgress<int> progress,
            CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                ZipArchive archive;
                try
                {
                    archive = ZipFile.OpenRead(zipPath);
                }
                catch (InvalidDataException ex)
                {
                    TryDelete(zipPath);
                    throw new InvalidDataException(
                        $"O arquivo ZIP está corrompido ou incompleto: {Path.GetFileName(zipPath)}. " +
                        "O arquivo foi removido — tente novamente para refazer o download.", ex);
                }

                using (archive)
                {
                    // Cria o diretório de destino apenas após confirmar que o ZIP é válido
                    Directory.CreateDirectory(destDir);

                    // Normaliza o diretório destino com separador no final para comparação segura
                    var fullDestDir = Path.GetFullPath(destDir);
                    if (!fullDestDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        fullDestDir += Path.DirectorySeparatorChar;

                    int total = archive.Entries.Count;
                    int done = 0;

                    foreach (var entry in archive.Entries)
                    {
                        ct.ThrowIfCancellationRequested();

                        // Proteção contra path traversal (zip slip)
                        var destPath = Path.GetFullPath(Path.Combine(destDir, entry.FullName));
                        if (!destPath.StartsWith(fullDestDir, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException(
                                $"Entrada ZIP inválida detectada (possível path traversal): {entry.FullName}");

                        // Entrada de diretório
                        if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(destPath);
                        }
                        else
                        {
                            var parentDir = Path.GetDirectoryName(destPath);
                            if (!string.IsNullOrEmpty(parentDir))
                                Directory.CreateDirectory(parentDir);

                            entry.ExtractToFile(destPath, overwrite: true);
                        }

                        done++;
                        if (total > 0)
                            progress?.Report(done * 100 / total);
                    }
                }
            }, ct);
        }

        // ----------------------------------------------------------------
        // Fluxo completo de atualização
        // ----------------------------------------------------------------

        /// <summary>
        /// Executa o fluxo completo:
        ///   1. Download base.zip → extrai
        ///   2. Download update.zip (URL da API) → extrai por cima
        ///   3. Salva versão local
        ///
        /// Progresso reportado como (percentual 0–100, texto de status).
        /// </summary>
        public async Task RunFullUpdateAsync(
            VersionInfo versionInfo,
            IProgress<(int Percent, string Status)> progress,
            CancellationToken ct = default)
        {
            var tempDir = Path.Combine(
                Path.GetTempPath(),
                "RookgaardLegacyUpdater");

            Directory.CreateDirectory(tempDir);

            var baseZipPath   = Path.Combine(tempDir, Config.BASE_ZIP_FILENAME);
            var updateZipPath = Path.Combine(tempDir, Config.UPDATE_ZIP_FILENAME);

            try
            {
                // ── 1. Download base.zip (0 → 40%) ──────────────────────
                progress?.Report((0, string.Format(Config.STATUS_DOWNLOADING_BASE, 0)));

                var baseInfo = await GetBaseInfoAsync(ct);

                await DownloadFileAsync(
                    baseInfo.Url,
                    baseZipPath,
                    new Progress<int>(p =>
                        progress?.Report((
                            p * 40 / 100,
                            string.Format(Config.STATUS_DOWNLOADING_BASE, p)))),
                    ct);

                // ── 2. Extração base.zip (40 → 60%) ─────────────────────
                progress?.Report((40, Config.STATUS_EXTRACTING_BASE));

                await ExtractZipAsync(
                    baseZipPath,
                    Config.INSTALL_DIR,
                    new Progress<int>(p =>
                        progress?.Report((
                            40 + p * 20 / 100,
                            Config.STATUS_EXTRACTING_BASE))),
                    ct);

                // ── 3. Download update.zip (60 → 85%) ───────────────────
                progress?.Report((60, string.Format(Config.STATUS_DOWNLOADING_UPDATE, 0)));

                await DownloadFileAsync(
                    versionInfo.Url,
                    updateZipPath,
                    new Progress<int>(p =>
                        progress?.Report((
                            60 + p * 25 / 100,
                            string.Format(Config.STATUS_DOWNLOADING_UPDATE, p)))),
                    ct);

                // ── 4. Extração update.zip (85 → 99%) ───────────────────
                progress?.Report((85, Config.STATUS_EXTRACTING_UPDATE));

                await ExtractZipAsync(
                    updateZipPath,
                    Config.INSTALL_DIR,
                    new Progress<int>(p =>
                        progress?.Report((
                            85 + p * 14 / 100,
                            Config.STATUS_EXTRACTING_UPDATE))),
                    ct);

                // ── 5. Salvar versão local ───────────────────────────────
                SaveLocalVersion(versionInfo.Version);

                progress?.Report((100, Config.STATUS_DONE));
            }
            finally
            {
                // Limpar arquivos temporários
                TryDelete(baseZipPath);
                TryDelete(updateZipPath);
            }
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch { /* ignora erros ao limpar temp */ }
        }
    }
}
