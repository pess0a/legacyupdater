using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegacyUpdater
{
    public partial class MainForm : Form
    {
        private readonly Updater _updater = new Updater();
        private CancellationTokenSource _cts;
        private VersionInfo _remoteVersion;
        private VersionInfo _baseVersion;

        public MainForm()
        {
            InitializeComponent();
            Text = $"{Config.GAME_NAME} — Updater v{Config.UPDATER_VERSION}";
        }

        // ----------------------------------------------------------------
        // Inicialização
        // ----------------------------------------------------------------

        private async void MainForm_Load(object sender, EventArgs e)
        {
            SetBusy(true);
            SetStatus(Config.STATUS_CHECKING, 0);

            try
            {
                var updateTask = _updater.GetRemoteVersionInfoAsync();
                var baseTask   = _updater.GetBaseInfoAsync();
                await Task.WhenAll(updateTask, baseTask);

                _remoteVersion = updateTask.Result;
                _baseVersion   = baseTask.Result;

                lblVersion.Text = $"Base: v{_baseVersion.Version}   Update: v{_remoteVersion.Version}   ({_remoteVersion.Date})";

                var localVersion  = _updater.GetLocalVersion();
                var exePath       = Path.Combine(Config.INSTALL_DIR, Config.GAME_EXECUTABLE);
                var gameInstalled = File.Exists(exePath);

                if (gameInstalled && localVersion == _remoteVersion.Version)
                {
                    // Já atualizado e instalado
                    SetStatus(Config.STATUS_UP_TO_DATE, 100);
                    SetBusy(false);
                }
                else
                {
                    // Precisa instalar ou atualizar
                    await RunUpdateAsync();
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(Config.STATUS_ERROR, ex.Message), 0);
                SetBusy(false);
            }
        }

        // ----------------------------------------------------------------
        // Botão: Forçar Update
        // ----------------------------------------------------------------

        private async void btnForceUpdate_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Isso irá baixar e reinstalar o jogo completamente.\n\nDeseja continuar?",
                Config.GAME_NAME,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes) return;

            SetBusy(true);
            SetStatus(Config.STATUS_FORCE_UPDATE, 0);

            try
            {
                _updater.ClearLocalVersion();

                if (_remoteVersion == null)
                    _remoteVersion = await _updater.GetRemoteVersionInfoAsync();

                await RunUpdateAsync();
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(Config.STATUS_ERROR, ex.Message), 0);
                SetBusy(false);
            }
        }

        // ----------------------------------------------------------------
        // Botão: Jogar
        // ----------------------------------------------------------------

        private void btnPlay_Click(object sender, EventArgs e)
        {
            var exePath = Path.Combine(Config.INSTALL_DIR, Config.GAME_EXECUTABLE);

            if (!File.Exists(exePath))
            {
                MessageBox.Show(
                    $"Executável não encontrado:\n{exePath}\n\nTente usar \"Forçar Update\" para reinstalar o jogo.",
                    Config.GAME_NAME,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName         = exePath,
                    WorkingDirectory = Config.INSTALL_DIR,
                    UseShellExecute  = true
                });

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao iniciar o jogo:\n{ex.Message}",
                    Config.GAME_NAME,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ----------------------------------------------------------------
        // Lógica interna
        // ----------------------------------------------------------------

        private async Task RunUpdateAsync()
        {
            _cts = new CancellationTokenSource();

            var progress = new Progress<(int Percent, string Status)>(report =>
            {
                SetStatus(report.Status, report.Percent);
            });

            try
            {
                await _updater.RunFullUpdateAsync(_remoteVersion, progress, _cts.Token);
                ShortcutHelper.CreateDesktopShortcut();
                SetBusy(false);
            }
            catch (OperationCanceledException)
            {
                SetStatus("Operação cancelada.", 0);
                SetBusy(false);
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(Config.STATUS_ERROR, ex.Message), 0);
                SetBusy(false);
            }
        }

        /// <summary>Habilita ou desabilita os botões conforme o estado de ocupado.</summary>
        private void SetBusy(bool busy)
        {
            if (InvokeRequired) { Invoke(new Action(() => SetBusy(busy))); return; }

            btnPlay.Enabled        = !busy;
            btnForceUpdate.Enabled = !busy;
        }

        /// <summary>Atualiza a label de status e a barra de progresso (thread-safe).</summary>
        private void SetStatus(string text, int percent)
        {
            if (InvokeRequired) { Invoke(new Action(() => SetStatus(text, percent))); return; }

            lblStatus.Text    = text;
            progressBar.Value = Math.Max(0, Math.Min(100, percent));
            lblPercent.Text   = $"{Math.Max(0, Math.Min(100, percent))}%";
        }
    }
}
