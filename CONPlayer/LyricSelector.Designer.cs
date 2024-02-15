using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace cPlayer
{
    partial class LyricSelector
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
            this.btnSave = new System.Windows.Forms.Button();
            this.grpStyle = new System.Windows.Forms.GroupBox();
            this.radioNone = new System.Windows.Forms.RadioButton();
            this.radioKaraoke = new System.Windows.Forms.RadioButton();
            this.radioScrolling = new System.Windows.Forms.RadioButton();
            this.radioStatic = new System.Windows.Forms.RadioButton();
            this.grpDisplay = new System.Windows.Forms.GroupBox();
            this.radioHarmonies = new System.Windows.Forms.RadioButton();
            this.radioVocals = new System.Windows.Forms.RadioButton();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.radioGame = new System.Windows.Forms.RadioButton();
            this.radioWhole = new System.Windows.Forms.RadioButton();
            this.btnDefault = new System.Windows.Forms.Button();
            this.grpStyle.SuspendLayout();
            this.grpDisplay.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Location = new System.Drawing.Point(218, 180);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpStyle
            // 
            this.grpStyle.Controls.Add(this.radioNone);
            this.grpStyle.Controls.Add(this.radioKaraoke);
            this.grpStyle.Controls.Add(this.radioScrolling);
            this.grpStyle.Controls.Add(this.radioStatic);
            this.grpStyle.Location = new System.Drawing.Point(12, 12);
            this.grpStyle.Name = "grpStyle";
            this.grpStyle.Size = new System.Drawing.Size(266, 50);
            this.grpStyle.TabIndex = 10;
            this.grpStyle.TabStop = false;
            this.grpStyle.Text = "Display style:";
            // 
            // radioNone
            // 
            this.radioNone.AutoSize = true;
            this.radioNone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioNone.Location = new System.Drawing.Point(210, 20);
            this.radioNone.Name = "radioNone";
            this.radioNone.Size = new System.Drawing.Size(51, 17);
            this.radioNone.TabIndex = 3;
            this.radioNone.Text = "None";
            this.radioNone.UseVisualStyleBackColor = true;
            this.radioNone.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // radioKaraoke
            // 
            this.radioKaraoke.AutoSize = true;
            this.radioKaraoke.Checked = true;
            this.radioKaraoke.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioKaraoke.Location = new System.Drawing.Point(68, 20);
            this.radioKaraoke.Name = "radioKaraoke";
            this.radioKaraoke.Size = new System.Drawing.Size(65, 17);
            this.radioKaraoke.TabIndex = 2;
            this.radioKaraoke.TabStop = true;
            this.radioKaraoke.Text = "Karaoke";
            this.radioKaraoke.UseVisualStyleBackColor = true;
            this.radioKaraoke.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // radioScrolling
            // 
            this.radioScrolling.AutoSize = true;
            this.radioScrolling.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioScrolling.Location = new System.Drawing.Point(139, 20);
            this.radioScrolling.Name = "radioScrolling";
            this.radioScrolling.Size = new System.Drawing.Size(65, 17);
            this.radioScrolling.TabIndex = 1;
            this.radioScrolling.Text = "Scrolling";
            this.radioScrolling.UseVisualStyleBackColor = true;
            this.radioScrolling.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // radioStatic
            // 
            this.radioStatic.AutoSize = true;
            this.radioStatic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioStatic.Location = new System.Drawing.Point(10, 20);
            this.radioStatic.Name = "radioStatic";
            this.radioStatic.Size = new System.Drawing.Size(52, 17);
            this.radioStatic.TabIndex = 0;
            this.radioStatic.Text = "Static";
            this.radioStatic.UseVisualStyleBackColor = true;
            this.radioStatic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // grpDisplay
            // 
            this.grpDisplay.Controls.Add(this.radioHarmonies);
            this.grpDisplay.Controls.Add(this.radioVocals);
            this.grpDisplay.Location = new System.Drawing.Point(12, 68);
            this.grpDisplay.Name = "grpDisplay";
            this.grpDisplay.Size = new System.Drawing.Size(266, 50);
            this.grpDisplay.TabIndex = 11;
            this.grpDisplay.TabStop = false;
            this.grpDisplay.Text = "Lyrics to display:";
            // 
            // radioHarmonies
            // 
            this.radioHarmonies.AutoSize = true;
            this.radioHarmonies.Checked = true;
            this.radioHarmonies.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioHarmonies.Location = new System.Drawing.Point(158, 20);
            this.radioHarmonies.Name = "radioHarmonies";
            this.radioHarmonies.Size = new System.Drawing.Size(75, 17);
            this.radioHarmonies.TabIndex = 2;
            this.radioHarmonies.TabStop = true;
            this.radioHarmonies.Text = "Harmonies";
            this.radioHarmonies.UseVisualStyleBackColor = true;
            this.radioHarmonies.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // radioVocals
            // 
            this.radioVocals.AutoSize = true;
            this.radioVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioVocals.Location = new System.Drawing.Point(34, 20);
            this.radioVocals.Name = "radioVocals";
            this.radioVocals.Size = new System.Drawing.Size(99, 17);
            this.radioVocals.TabIndex = 1;
            this.radioVocals.Text = "PART VOCALS";
            this.radioVocals.UseVisualStyleBackColor = true;
            this.radioVocals.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.radioGame);
            this.grpOptions.Controls.Add(this.radioWhole);
            this.grpOptions.Location = new System.Drawing.Point(12, 124);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(266, 50);
            this.grpOptions.TabIndex = 12;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Word options:";
            // 
            // radioGame
            // 
            this.radioGame.AutoSize = true;
            this.radioGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioGame.Location = new System.Drawing.Point(139, 20);
            this.radioGame.Name = "radioGame";
            this.radioGame.Size = new System.Drawing.Size(107, 17);
            this.radioGame.TabIndex = 2;
            this.radioGame.Text = "Game syl- la- bles";
            this.radioGame.UseVisualStyleBackColor = true;
            this.radioGame.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // radioWhole
            // 
            this.radioWhole.AutoSize = true;
            this.radioWhole.Checked = true;
            this.radioWhole.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioWhole.Location = new System.Drawing.Point(34, 20);
            this.radioWhole.Name = "radioWhole";
            this.radioWhole.Size = new System.Drawing.Size(87, 17);
            this.radioWhole.TabIndex = 1;
            this.radioWhole.TabStop = true;
            this.radioWhole.Text = "Whole words";
            this.radioWhole.UseVisualStyleBackColor = true;
            this.radioWhole.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioStatic_MouseUp);
            // 
            // btnDefault
            // 
            this.btnDefault.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDefault.Location = new System.Drawing.Point(12, 180);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(60, 23);
            this.btnDefault.TabIndex = 13;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // LyricSelector
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(289, 211);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpDisplay);
            this.Controls.Add(this.grpStyle);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LyricSelector";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lyrics settings:";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LyricSelector_FormClosing);
            this.Shown += new System.EventHandler(this.LyricSelector_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LyricSelector_KeyUp);
            this.grpStyle.ResumeLayout(false);
            this.grpStyle.PerformLayout();
            this.grpDisplay.ResumeLayout(false);
            this.grpDisplay.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpStyle;
        private System.Windows.Forms.RadioButton radioNone;
        private System.Windows.Forms.RadioButton radioKaraoke;
        private System.Windows.Forms.RadioButton radioScrolling;
        private System.Windows.Forms.RadioButton radioStatic;
        private System.Windows.Forms.GroupBox grpDisplay;
        private System.Windows.Forms.RadioButton radioHarmonies;
        private System.Windows.Forms.RadioButton radioVocals;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.RadioButton radioGame;
        private System.Windows.Forms.RadioButton radioWhole;
        private System.Windows.Forms.Button btnDefault;
    }
}