using System;
using System.IO;
using System.Reflection;

namespace LegacyUpdater
{
    internal static class ShortcutHelper
    {
        /// <summary>
        /// Cria (ou substitui) um atalho no Desktop do usuário apontando para o launcher (updater),
        /// garantindo que o usuário sempre abra o jogo pelo launcher e receba atualizações.
        /// </summary>
        internal static void CreateDesktopShortcut()
        {
            try
            {
                // Usa o caminho do próprio executável em execução como alvo do atalho,
                // de forma que o atalho sempre aponte para o launcher correto.
                var launcherPath = Assembly.GetExecutingAssembly().Location;

                // Fallback: se por algum motivo não for possível obter o caminho via reflection,
                // usa o caminho esperado dentro do diretório de instalação.
                if (string.IsNullOrEmpty(launcherPath) || !File.Exists(launcherPath))
                    launcherPath = Path.Combine(Config.INSTALL_DIR, Config.LAUNCHER_EXECUTABLE);

                var launcherDir  = Path.GetDirectoryName(launcherPath);
                var desktopPath  = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var shortcutPath = Path.Combine(desktopPath, Config.GAME_NAME + ".lnk");

                Type   shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell    = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath       = launcherPath;
                shortcut.WorkingDirectory = launcherDir;
                shortcut.Description      = Config.GAME_NAME;
                shortcut.IconLocation     = launcherPath + ",0";
                shortcut.Save();
            }
            catch
            {
                // Falha silenciosa — o atalho não é essencial para o funcionamento do updater
            }
        }
    }
}
