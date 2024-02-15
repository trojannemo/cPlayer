namespace cPlayer
{
    partial class SectionSelector
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
            this.components = new System.ComponentModel.Container();
            this.lstSections = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lstSections
            // 
            this.lstSections.FormattingEnabled = true;
            this.lstSections.Location = new System.Drawing.Point(12, 12);
            this.lstSections.Name = "lstSections";
            this.lstSections.Size = new System.Drawing.Size(211, 225);
            this.lstSections.TabIndex = 0;
            this.lstSections.SelectedIndexChanged += new System.EventHandler(this.lstSections_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SectionSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(235, 252);
            this.Controls.Add(this.lstSections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "SectionSelector";
            this.Text = "SectionSelector";
            this.Deactivate += new System.EventHandler(this.SectionSelector_Deactivate);
            this.Shown += new System.EventHandler(this.SectionSelector_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SectionSelector_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstSections;
        private System.Windows.Forms.Timer timer1;
    }
}