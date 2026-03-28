using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace LegacyUpdater
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.picBackground  = new PictureBox();
            this.panelOverlay   = new TransparentPanel();
            this.lblGameTitle   = new Label();
            this.lblVersion     = new Label();
            this.lblStatus      = new Label();
            this.progressBar    = new ProgressBar();
            this.lblPercent     = new Label();
            this.btnPlay        = new Button();
            this.btnForceUpdate = new Button();

            ((System.ComponentModel.ISupportInitialize)this.picBackground).BeginInit();
            this.panelOverlay.SuspendLayout();
            this.SuspendLayout();

            // ── picBackground ────────────────────────────────────────────
            this.picBackground.Dock     = DockStyle.Fill;
            this.picBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picBackground.TabStop  = false;
            this.picBackground.BackColor = Color.FromArgb(18, 18, 28); // fallback
            LoadBackgroundImage();

            // ── panelOverlay ─────────────────────────────────────────────
            int overlayTop = Config.WINDOW_HEIGHT - Config.OVERLAY_HEIGHT;
            this.panelOverlay.Location = new Point(0, overlayTop);
            this.panelOverlay.Size     = new Size(Config.WINDOW_WIDTH, Config.OVERLAY_HEIGHT);
            this.panelOverlay.Controls.AddRange(new Control[]
            {
                this.lblGameTitle,
                this.lblVersion,
                this.lblStatus,
                this.progressBar,
                this.lblPercent,
                this.btnPlay,
                this.btnForceUpdate
            });

            // ── lblGameTitle ─────────────────────────────────────────────
            this.lblGameTitle.AutoSize  = false;
            this.lblGameTitle.Location  = new Point(20, 12);
            this.lblGameTitle.Size      = new Size(480, 40);
            this.lblGameTitle.Text      = Config.GAME_NAME;
            this.lblGameTitle.Font      = new Font("Segoe UI", 22f, FontStyle.Bold, GraphicsUnit.Point);
            this.lblGameTitle.ForeColor = Color.White;

            // ── lblVersion ───────────────────────────────────────────────
            this.lblVersion.AutoSize   = false;
            this.lblVersion.Location   = new Point(510, 20);
            this.lblVersion.Size       = new Size(270, 24);
            this.lblVersion.Text       = "Carregando...";
            this.lblVersion.Font       = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point);
            this.lblVersion.ForeColor  = Color.FromArgb(200, 200, 200);
            this.lblVersion.TextAlign  = ContentAlignment.MiddleRight;

            // ── lblStatus ────────────────────────────────────────────────
            this.lblStatus.AutoSize   = false;
            this.lblStatus.Location   = new Point(20, 58);
            this.lblStatus.Size       = new Size(760, 22);
            this.lblStatus.Text       = Config.STATUS_CHECKING;
            this.lblStatus.Font       = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            this.lblStatus.ForeColor  = Color.FromArgb(255, 240, 180);

            // ── progressBar ──────────────────────────────────────────────
            this.progressBar.Location = new Point(20, 86);
            this.progressBar.Size     = new Size(700, 20);
            this.progressBar.Minimum  = 0;
            this.progressBar.Maximum  = 100;
            this.progressBar.Value    = 0;
            this.progressBar.Style    = ProgressBarStyle.Continuous;

            // ── lblPercent ───────────────────────────────────────────────
            this.lblPercent.AutoSize  = false;
            this.lblPercent.Location  = new Point(726, 86);
            this.lblPercent.Size      = new Size(54, 20);
            this.lblPercent.Text      = "0%";
            this.lblPercent.Font      = new Font("Segoe UI", 8.5f, FontStyle.Regular, GraphicsUnit.Point);
            this.lblPercent.ForeColor = Color.White;
            this.lblPercent.TextAlign = ContentAlignment.MiddleRight;

            // ── btnPlay ──────────────────────────────────────────────────
            this.btnPlay.Location    = new Point(20, 120);
            this.btnPlay.Size        = new Size(170, 46);
            this.btnPlay.Text        = Config.BTN_PLAY_TEXT;
            this.btnPlay.Font        = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Point);
            this.btnPlay.BackColor   = Color.FromArgb(30, 160, 70);
            this.btnPlay.ForeColor   = Color.White;
            this.btnPlay.FlatStyle   = FlatStyle.Flat;
            this.btnPlay.FlatAppearance.BorderSize  = 0;
            this.btnPlay.FlatAppearance.MouseOverBackColor  = Color.FromArgb(40, 180, 85);
            this.btnPlay.FlatAppearance.MouseDownBackColor  = Color.FromArgb(20, 130, 55);
            this.btnPlay.Enabled     = false;
            this.btnPlay.Cursor      = Cursors.Hand;
            this.btnPlay.Click      += new System.EventHandler(this.btnPlay_Click);

            // ── btnForceUpdate ───────────────────────────────────────────
            this.btnForceUpdate.Location  = new Point(202, 128);
            this.btnForceUpdate.Size      = new Size(160, 32);
            this.btnForceUpdate.Text      = Config.BTN_FORCE_UPDATE_TEXT;
            this.btnForceUpdate.Font      = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            this.btnForceUpdate.BackColor = Color.FromArgb(70, 70, 80);
            this.btnForceUpdate.ForeColor = Color.FromArgb(220, 220, 220);
            this.btnForceUpdate.FlatStyle = FlatStyle.Flat;
            this.btnForceUpdate.FlatAppearance.BorderSize = 0;
            this.btnForceUpdate.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 90, 100);
            this.btnForceUpdate.Cursor   = Cursors.Hand;
            this.btnForceUpdate.Click   += new System.EventHandler(this.btnForceUpdate_Click);

            // ── MainForm ─────────────────────────────────────────────────
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode       = AutoScaleMode.Dpi;
            this.ClientSize          = new Size(Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT);
            this.Controls.Add(this.panelOverlay);   // overlay na frente
            this.Controls.Add(this.picBackground);  // fundo atrás
            this.FormBorderStyle     = FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.StartPosition       = FormStartPosition.CenterScreen;
            this.Text                = Config.GAME_NAME + " Updater";
            this.BackColor           = Color.FromArgb(18, 18, 28);
            this.Load               += new System.EventHandler(this.MainForm_Load);

            ((System.ComponentModel.ISupportInitialize)this.picBackground).EndInit();
            this.panelOverlay.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Tenta carregar a imagem de fundo de Resources/background.png
        /// ao lado do executável. Se não encontrar, usa a cor de fundo.
        /// </summary>
        private void LoadBackgroundImage()
        {
            try
            {
                var exeDir = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) ?? ".";
                var imgPath = Path.Combine(exeDir, "Resources", "background.png");

                if (File.Exists(imgPath))
                    this.picBackground.Image = Image.FromFile(imgPath);
            }
            catch
            {
                // Se não carregar, fica com a cor de fundo definida
            }
        }

        #endregion

        // ── Declarações dos controles ────────────────────────────────────
        private PictureBox       picBackground;
        private TransparentPanel panelOverlay;
        private Label            lblGameTitle;
        private Label            lblVersion;
        private Label            lblStatus;
        private ProgressBar      progressBar;
        private Label            lblPercent;
        private Button           btnPlay;
        private Button           btnForceUpdate;
    }

    // ────────────────────────────────────────────────────────────────────
    // Panel com fundo semi-transparente pintado manualmente
    // ────────────────────────────────────────────────────────────────────
    internal class TransparentPanel : Panel
    {
        public TransparentPanel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Fundo escuro semi-transparente
            using (var brush = new SolidBrush(Color.FromArgb(185, 15, 15, 25)))
                e.Graphics.FillRectangle(brush, ClientRectangle);
        }

    }
}
