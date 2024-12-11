using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class gifOverlay : Form
    {
        private frmMain mainForm;
        private Image spinningGif;
        private Timer animationTimer;

        public gifOverlay(frmMain MainForm)
        {
            InitializeComponent();
            // Load the GIF
            var gifPATH = Application.StartupPath + "\\res\\working.gif";
            if (File.Exists(gifPATH) )
            {
                spinningGif = Image.FromFile(gifPATH);
            }
            else
            {
                MessageBox.Show("Can't find that GIF file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
                return;
            }
            if (spinningGif == null)
            {
                MessageBox.Show("Failed to load GIF!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
                return;
            }

            // Enable animation
            ImageAnimator.Animate(spinningGif, null);

            // Set up double buffering for smooth rendering
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            // Configure the form
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            // Set up a timer to update the animation
            animationTimer = new Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            
            mainForm = MainForm;
        }

        // Start animation
        public void Start()
        {
            if (IsDisposed) return;
            this.Visible = true;
            animationTimer.Start();
        }

        public void Stop()
        {
            if (IsDisposed) return;
            this.Visible = false;
            animationTimer.Stop();
        }

        // Timer tick updates the animation and redraws the form
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            ImageAnimator.UpdateFrames(spinningGif);
            this.Invalidate(); // Triggers the OnPaint event
        }

        // Draw the GIF
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (spinningGif != null)
            {
                // Define the target rectangle to scale the image
                Rectangle targetRect = new Rectangle(0, 0, this.Width, this.Height);

                // Draw the GIF scaled to the size of the form
                e.Graphics.DrawImage(spinningGif, targetRect);
            }
        }

        private void gifOverlay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var result = MessageBox.Show("Cancel current process?", "cPlayer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mainForm.CancelWorkers = true;
                    Dispose();
                }
            }
        }
    }
}
