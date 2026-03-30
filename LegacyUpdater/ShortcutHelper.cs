using System;
using System.IO;
using System.Reflection;

namespace LegacyUpdater
{
    internal static class ShortcutHelper
    {
        /// <summary>
        /// Copia o launcher para a pasta de instalação do jogo (se necessário) e cria (ou
        /// substitui) um atalho no Desktop apontando para esse caminho fixo. Assim o atalho
        /// permanece válido mesmo que o usuário mova o arquivo original do launcher.
        /// </summary>
        internal static void CreateDesktopShortcut()
        {
            try
            {
                var launcherDest = Path.Combine(Config.INSTALL_DIR, Config.LAUNCHER_EXECUTABLE);

                // Copia o launcher e suas dependências para a pasta do jogo, sobrescrevendo
                // se já existir, a menos que já seja o mesmo arquivo (usuário já o executa de lá).
                var launcherSource = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(launcherSource) &&
                    File.Exists(launcherSource) &&
                    !string.Equals(launcherSource, launcherDest, StringComparison.OrdinalIgnoreCase))
                {
                    Directory.CreateDirectory(Config.INSTALL_DIR);
                    File.Copy(launcherSource, launcherDest, overwrite: true);

                    // Copia dependências que ficam no mesmo diretório do launcher
                    var sourceDir = Path.GetDirectoryName(launcherSource);
                    foreach (var dll in Config.LAUNCHER_DEPENDENCIES)
                    {
                        var src  = Path.Combine(sourceDir, dll);
                        var dest = Path.Combine(Config.INSTALL_DIR, dll);
                        if (File.Exists(src))
                            File.Copy(src, dest, overwrite: true);
                    }
                }

                var desktopPath  = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var shortcutPath = Path.Combine(desktopPath, Config.GAME_NAME + ".lnk");

                Type   shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell    = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath       = launcherDest;
                shortcut.WorkingDirectory = Config.INSTALL_DIR;
                shortcut.Description      = Config.GAME_NAME;
                shortcut.IconLocation     = launcherDest + ",0";
                shortcut.Save();
            }
            catch
            {
                // Falha silenciosa — o atalho não é essencial para o funcionamento do updater
            }
        }
    }
}
