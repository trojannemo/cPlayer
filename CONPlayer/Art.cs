using System;
using System.Drawing;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class Art : Form
    {
        private readonly NemoTools Tools;
        private readonly string AlbumArt;
        private readonly Point StartLocation;

        public Art(Point start, string art)
        {
            InitializeComponent();
            Tools = new NemoTools();
            AlbumArt = art;
            StartLocation = start;
        }
        
        private void Art_Shown(object sender, EventArgs e)
        {
            var width = 256;
            var height = 256;
            try
            {
                var img = Image.FromFile(AlbumArt);
                width = img.Width;
                height = img.Height;
                img.Dispose();
            }
            catch (Exception)
            {}
            Width = width;
            Height = height;
            picArt.Width = width;
            picArt.Height = height;
            picArt.Image = Tools.NemoLoadImage(AlbumArt);
            Location = StartLocation;
        }

        private void picArt_MouseClick(object sender, MouseEventArgs e)
        {
            Dispose();
        }

        private void Art_KeyUp(object sender, KeyEventArgs e)
        {
            Dispose();
        }

        private void Art_Deactivate(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
