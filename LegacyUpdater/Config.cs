namespace LegacyUpdater
{
    /// <summary>
    /// ============================================================
    ///  ARQUIVO DE CONFIGURAÇÃO CENTRAL — edite apenas este arquivo
    ///  para personalizar o updater com seus dados.
    /// ============================================================
    /// </summary>
    public static class Config
    {
        // ============================================================
        // JOGO
        // ============================================================

        /// <summary>Nome do jogo — aparece no título da janela e em diálogos.</summary>
        public const string GAME_NAME = "Rookgaard Legacy";

        /// <summary>Nome do executável do jogo dentro da pasta de instalação.</summary>
        public const string GAME_EXECUTABLE = "game.exe";

        /// <summary>Versão do próprio updater (exibida na barra de título).</summary>
        public const string UPDATER_VERSION = "1.0.0";

        // ============================================================
        // CAMINHOS LOCAIS
        // ============================================================

        /// <summary>
        /// Pasta onde o jogo será instalado.
        /// Exemplo: @"C:\Rookgaard Legacy"
        /// Use Environment.GetFolderPath para pastas especiais:
        ///   Environment.SpecialFolder.ProgramFiles  → C:\Program Files
        ///   Environment.SpecialFolder.LocalApplicationData → AppData\Local
        /// O caminho abaixo é um const; para usar variáveis de ambiente,
        /// altere para uma propriedade estática em Updater.cs.
        /// </summary>
        public const string INSTALL_DIR = @"C:\Rookgaard Legacy";

        /// <summary>Nome do arquivo que armazena a versão instalada localmente.</summary>
        public const string VERSION_FILE = "version.txt";

        /// <summary>Nome temporário do arquivo base durante o download.</summary>
        public const string BASE_ZIP_FILENAME = "base.zip";

        /// <summary>Nome temporário do arquivo de update durante o download.</summary>
        public const string UPDATE_ZIP_FILENAME = "update.zip";

        // ============================================================
        // REDE — URLs
        // ============================================================

        /// <summary>
        /// URL da API REST que retorna as informações da versão atual.
        /// Resposta JSON esperada:
        /// {
        ///   "version": "1.2.3",
        ///   "date":    "2026-03-28",
        ///   "url":     "https://example.com/update.zip"
        /// }
        /// </summary>
        public const string VERSION_API_URL = "https://example.com/api/version";

        /// <summary>
        /// URL do arquivo base.zip — instalação completa do zero.
        /// Baixado apenas quando não há instalação local ou ao forçar update.
        /// </summary>
        public const string BASE_ZIP_URL = "https://example.com/base.zip";

        // ============================================================
        // JANELA / DIMENSÕES
        // ============================================================

        /// <summary>Largura da janela em pixels.</summary>
        public const int WINDOW_WIDTH = 800;

        /// <summary>Altura da janela em pixels.</summary>
        public const int WINDOW_HEIGHT = 500;

        /// <summary>
        /// Altura do painel de overlay (controles na parte inferior).
        /// O painel começa em WINDOW_HEIGHT - OVERLAY_HEIGHT.
        /// </summary>
        public const int OVERLAY_HEIGHT = 180;

        // ============================================================
        // TEXTOS DOS BOTÕES
        // ============================================================

        /// <summary>Texto do botão principal para iniciar o jogo.</summary>
        public const string BTN_PLAY_TEXT = "JOGAR";

        /// <summary>Texto do botão de reinstalação completa.</summary>
        public const string BTN_FORCE_UPDATE_TEXT = "Forçar Update";

        // ============================================================
        // MENSAGENS DE STATUS (exibidas na UI durante as operações)
        // ============================================================

        /// <summary>Verificando versão na API.</summary>
        public const string STATUS_CHECKING = "Verificando atualizações...";

        /// <summary>Jogo já está na versão mais recente.</summary>
        public const string STATUS_UP_TO_DATE = "Jogo atualizado! Pronto para jogar.";

        /// <summary>Baixando base.zip. Use {0} para o percentual.</summary>
        public const string STATUS_DOWNLOADING_BASE = "Baixando arquivos base... {0}%";

        /// <summary>Extraindo base.zip.</summary>
        public const string STATUS_EXTRACTING_BASE = "Extraindo arquivos base...";

        /// <summary>Baixando update.zip. Use {0} para o percentual.</summary>
        public const string STATUS_DOWNLOADING_UPDATE = "Baixando atualização... {0}%";

        /// <summary>Extraindo update.zip.</summary>
        public const string STATUS_EXTRACTING_UPDATE = "Instalando atualização...";

        /// <summary>Update concluído com sucesso.</summary>
        public const string STATUS_DONE = "Atualização concluída! Pronto para jogar.";

        /// <summary>Mensagem de erro genérica. Use {0} para a descrição do erro.</summary>
        public const string STATUS_ERROR = "Erro: {0}";

        /// <summary>Início da operação de forçar update.</summary>
        public const string STATUS_FORCE_UPDATE = "Forçando reinstalação completa...";

        // ============================================================
        // TIMEOUT DE REDE
        // ============================================================

        /// <summary>Timeout total das operações de download (em minutos).</summary>
        public const int DOWNLOAD_TIMEOUT_MINUTES = 60;
    }
}
