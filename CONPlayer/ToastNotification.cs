using System.Drawing;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class ToastNotification : Form
    {
        private Label messageLabel;

        public ToastNotification(string message)
        {
            InitializeComponent();

            messageLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = message,
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            this.Controls.Add(messageLabel);
        }

        public void ShowToast(int durationMilliseconds)
        {            
            this.Show();

            Timer timer = new Timer { Interval = durationMilliseconds };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                this.Close();
            };
            timer.Start();
        }
    }
}
