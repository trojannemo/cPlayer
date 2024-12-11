using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace cPlayer
{
    partial class AudioSelector
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
            this.chkDrums = new System.Windows.Forms.CheckBox();
            this.chkBass = new System.Windows.Forms.CheckBox();
            this.chkGuitar = new System.Windows.Forms.CheckBox();
            this.chkVocals = new System.Windows.Forms.CheckBox();
            this.chkKeys = new System.Windows.Forms.CheckBox();
            this.chkBacking = new System.Windows.Forms.CheckBox();
            this.chkCrowd = new System.Windows.Forms.CheckBox();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnNone = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkDrums
            // 
            this.chkDrums.AutoSize = true;
            this.chkDrums.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDrums.Location = new System.Drawing.Point(12, 11);
            this.chkDrums.Name = "chkDrums";
            this.chkDrums.Size = new System.Drawing.Size(56, 17);
            this.chkDrums.TabIndex = 0;
            this.chkDrums.TabStop = false;
            this.chkDrums.Text = "Drums";
            this.chkDrums.UseVisualStyleBackColor = true;
            this.chkDrums.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkBass
            // 
            this.chkBass.AutoSize = true;
            this.chkBass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBass.Location = new System.Drawing.Point(74, 11);
            this.chkBass.Name = "chkBass";
            this.chkBass.Size = new System.Drawing.Size(49, 17);
            this.chkBass.TabIndex = 1;
            this.chkBass.TabStop = false;
            this.chkBass.Text = "Bass";
            this.chkBass.UseVisualStyleBackColor = true;
            this.chkBass.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkGuitar
            // 
            this.chkGuitar.AutoSize = true;
            this.chkGuitar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkGuitar.Location = new System.Drawing.Point(129, 11);
            this.chkGuitar.Name = "chkGuitar";
            this.chkGuitar.Size = new System.Drawing.Size(54, 17);
            this.chkGuitar.TabIndex = 2;
            this.chkGuitar.TabStop = false;
            this.chkGuitar.Text = "Guitar";
            this.chkGuitar.UseVisualStyleBackColor = true;
            this.chkGuitar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkVocals
            // 
            this.chkVocals.AutoSize = true;
            this.chkVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkVocals.Location = new System.Drawing.Point(189, 11);
            this.chkVocals.Name = "chkVocals";
            this.chkVocals.Size = new System.Drawing.Size(58, 17);
            this.chkVocals.TabIndex = 3;
            this.chkVocals.TabStop = false;
            this.chkVocals.Text = "Vocals";
            this.chkVocals.UseVisualStyleBackColor = true;
            this.chkVocals.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkKeys
            // 
            this.chkKeys.AutoSize = true;
            this.chkKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkKeys.Location = new System.Drawing.Point(12, 34);
            this.chkKeys.Name = "chkKeys";
            this.chkKeys.Size = new System.Drawing.Size(49, 17);
            this.chkKeys.TabIndex = 4;
            this.chkKeys.TabStop = false;
            this.chkKeys.Text = "Keys";
            this.chkKeys.UseVisualStyleBackColor = true;
            this.chkKeys.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkBacking
            // 
            this.chkBacking.AutoSize = true;
            this.chkBacking.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBacking.Location = new System.Drawing.Point(92, 34);
            this.chkBacking.Name = "chkBacking";
            this.chkBacking.Size = new System.Drawing.Size(65, 17);
            this.chkBacking.TabIndex = 5;
            this.chkBacking.TabStop = false;
            this.chkBacking.Text = "Backing";
            this.chkBacking.UseVisualStyleBackColor = true;
            this.chkBacking.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkCrowd
            // 
            this.chkCrowd.AutoSize = true;
            this.chkCrowd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCrowd.Location = new System.Drawing.Point(189, 34);
            this.chkCrowd.Name = "chkCrowd";
            this.chkCrowd.Size = new System.Drawing.Size(56, 17);
            this.chkCrowd.TabIndex = 6;
            this.chkCrowd.TabStop = false;
            this.chkCrowd.Text = "Crowd";
            this.chkCrowd.UseVisualStyleBackColor = true;
            this.chkCrowd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // btnAll
            // 
            this.btnAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAll.Location = new System.Drawing.Point(11, 67);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(60, 23);
            this.btnAll.TabIndex = 7;
            this.btnAll.Text = "All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnNone
            // 
            this.btnNone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNone.Location = new System.Drawing.Point(97, 67);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(60, 23);
            this.btnNone.TabIndex = 8;
            this.btnNone.Text = "None";
            this.btnNone.UseVisualStyleBackColor = true;
            this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Location = new System.Drawing.Point(180, 67);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // AudioSelector
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(252, 99);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNone);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.chkCrowd);
            this.Controls.Add(this.chkBacking);
            this.Controls.Add(this.chkKeys);
            this.Controls.Add(this.chkVocals);
            this.Controls.Add(this.chkGuitar);
            this.Controls.Add(this.chkBass);
            this.Controls.Add(this.chkDrums);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AudioSelector";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audio tracks to play:";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AudioSelector_FormClosing);
            this.Shown += new System.EventHandler(this.AudioSelector_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AudioSelector_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDrums;
        private System.Windows.Forms.CheckBox chkBass;
        private System.Windows.Forms.CheckBox chkGuitar;
        private System.Windows.Forms.CheckBox chkVocals;
        private System.Windows.Forms.CheckBox chkKeys;
        private System.Windows.Forms.CheckBox chkBacking;
        private System.Windows.Forms.CheckBox chkCrowd;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnNone;
        private System.Windows.Forms.Button btnSave;
    }
}