using System;
using System.IO;

namespace LegacyUpdater
{
    internal static class ShortcutHelper
    {
        /// <summary>
        /// Cria (ou substitui) um atalho no Desktop do usuário apontando para o executável do jogo.
        /// </summary>
        internal static void CreateDesktopShortcut()
        {
            try
            {
                var desktopPath  = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var shortcutPath = Path.Combine(desktopPath, Config.GAME_NAME + ".lnk");
                var targetPath   = Path.Combine(Config.INSTALL_DIR, Config.GAME_EXECUTABLE);

                Type   shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell    = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath       = targetPath;
                shortcut.WorkingDirectory = Config.INSTALL_DIR;
                shortcut.Description      = Config.GAME_NAME;
                shortcut.IconLocation     = targetPath + ",0";
                shortcut.Save();
            }
            catch
            {
                // Falha silenciosa — o atalho não é essencial para o funcionamento do updater
            }
        }
    }
}
