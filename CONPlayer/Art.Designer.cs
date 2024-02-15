namespace cPlayer
{
    partial class Art
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picArt = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picArt)).BeginInit();
            this.SuspendLayout();
            // 
            // picArt
            // 
            this.picArt.Location = new System.Drawing.Point(0, 0);
            this.picArt.Name = "picArt";
            this.picArt.Size = new System.Drawing.Size(256, 256);
            this.picArt.TabIndex = 0;
            this.picArt.TabStop = false;
            this.picArt.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picArt_MouseClick);
            // 
            // Art
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.picArt);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Art";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Album Art";
            this.Deactivate += new System.EventHandler(this.Art_Deactivate);
            this.Shown += new System.EventHandler(this.Art_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Art_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.picArt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picArt;
    }
}