using System;
using System.Threading;
using System.Windows.Forms;

namespace LegacyUpdater
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Garante que apenas uma instância do updater rode por vez
            bool createdNew;
            using (var mutex = new Mutex(true, Config.GAME_NAME + "_Updater_Mutex", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show(
                        "O updater já está em execução.",
                        Config.GAME_NAME,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
