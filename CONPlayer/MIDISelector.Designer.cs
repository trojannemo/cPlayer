using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace cPlayer
{
    partial class MIDISelector
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
            this.btnAll = new System.Windows.Forms.Button();
            this.btnNone = new System.Windows.Forms.Button();
            this.chkKeys = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkHarms = new System.Windows.Forms.CheckBox();
            this.chkProKeys = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkHarmColorOnVocals = new System.Windows.Forms.CheckBox();
            this.chkBWKeys = new System.Windows.Forms.CheckBox();
            this.chkHighlightSolos = new System.Windows.Forms.CheckBox();
            this.chkNameProKeys = new System.Windows.Forms.CheckBox();
            this.chkNameVocals = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboWindow = new System.Windows.Forms.ComboBox();
            this.cboSizing = new System.Windows.Forms.ComboBox();
            this.chkNameTracks = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDrums
            // 
            this.chkDrums.AutoSize = true;
            this.chkDrums.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDrums.Location = new System.Drawing.Point(12, 23);
            this.chkDrums.Name = "chkDrums";
            this.chkDrums.Size = new System.Drawing.Size(56, 17);
            this.chkDrums.TabIndex = 0;
            this.chkDrums.Text = "Drums";
            this.chkDrums.UseVisualStyleBackColor = true;
            this.chkDrums.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkBass
            // 
            this.chkBass.AutoSize = true;
            this.chkBass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBass.Location = new System.Drawing.Point(109, 23);
            this.chkBass.Name = "chkBass";
            this.chkBass.Size = new System.Drawing.Size(49, 17);
            this.chkBass.TabIndex = 1;
            this.chkBass.Text = "Bass";
            this.chkBass.UseVisualStyleBackColor = true;
            this.chkBass.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkGuitar
            // 
            this.chkGuitar.AutoSize = true;
            this.chkGuitar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkGuitar.Location = new System.Drawing.Point(198, 23);
            this.chkGuitar.Name = "chkGuitar";
            this.chkGuitar.Size = new System.Drawing.Size(54, 17);
            this.chkGuitar.TabIndex = 2;
            this.chkGuitar.Text = "Guitar";
            this.chkGuitar.UseVisualStyleBackColor = true;
            this.chkGuitar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkVocals
            // 
            this.chkVocals.AutoSize = true;
            this.chkVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkVocals.Location = new System.Drawing.Point(194, 46);
            this.chkVocals.Name = "chkVocals";
            this.chkVocals.Size = new System.Drawing.Size(58, 17);
            this.chkVocals.TabIndex = 3;
            this.chkVocals.Text = "Vocals";
            this.chkVocals.UseVisualStyleBackColor = true;
            this.chkVocals.CheckedChanged += new System.EventHandler(this.chkVocals_CheckedChanged);
            this.chkVocals.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // btnAll
            // 
            this.btnAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAll.Location = new System.Drawing.Point(15, 79);
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
            this.btnNone.Location = new System.Drawing.Point(101, 79);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(60, 23);
            this.btnNone.TabIndex = 8;
            this.btnNone.Text = "None";
            this.btnNone.UseVisualStyleBackColor = true;
            this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
            // 
            // chkKeys
            // 
            this.chkKeys.AutoSize = true;
            this.chkKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkKeys.Location = new System.Drawing.Point(83, 46);
            this.chkKeys.Name = "chkKeys";
            this.chkKeys.Size = new System.Drawing.Size(49, 17);
            this.chkKeys.TabIndex = 4;
            this.chkKeys.Text = "Keys";
            this.chkKeys.UseVisualStyleBackColor = true;
            this.chkKeys.CheckedChanged += new System.EventHandler(this.chkKeys_CheckedChanged);
            this.chkKeys.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.chkHarms);
            this.groupBox1.Controls.Add(this.chkProKeys);
            this.groupBox1.Controls.Add(this.chkKeys);
            this.groupBox1.Controls.Add(this.chkDrums);
            this.groupBox1.Controls.Add(this.btnNone);
            this.groupBox1.Controls.Add(this.chkBass);
            this.groupBox1.Controls.Add(this.btnAll);
            this.groupBox1.Controls.Add(this.chkGuitar);
            this.groupBox1.Controls.Add(this.chkVocals);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 116);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MIDI charts to display:";
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Location = new System.Drawing.Point(183, 79);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkHarms
            // 
            this.chkHarms.AutoSize = true;
            this.chkHarms.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkHarms.Location = new System.Drawing.Point(135, 46);
            this.chkHarms.Name = "chkHarms";
            this.chkHarms.Size = new System.Drawing.Size(56, 17);
            this.chkHarms.TabIndex = 10;
            this.chkHarms.Text = "Harms";
            this.chkHarms.UseVisualStyleBackColor = true;
            this.chkHarms.CheckedChanged += new System.EventHandler(this.chkHarms_CheckedChanged);
            this.chkHarms.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // chkProKeys
            // 
            this.chkProKeys.AutoSize = true;
            this.chkProKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkProKeys.Location = new System.Drawing.Point(12, 46);
            this.chkProKeys.Name = "chkProKeys";
            this.chkProKeys.Size = new System.Drawing.Size(68, 17);
            this.chkProKeys.TabIndex = 9;
            this.chkProKeys.Text = "Pro-Keys";
            this.chkProKeys.UseVisualStyleBackColor = true;
            this.chkProKeys.CheckedChanged += new System.EventHandler(this.chkProKeys_CheckedChanged);
            this.chkProKeys.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkDrums_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkHarmColorOnVocals);
            this.groupBox2.Controls.Add(this.chkBWKeys);
            this.groupBox2.Controls.Add(this.chkHighlightSolos);
            this.groupBox2.Controls.Add(this.chkNameProKeys);
            this.groupBox2.Controls.Add(this.chkNameVocals);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cboWindow);
            this.groupBox2.Controls.Add(this.cboSizing);
            this.groupBox2.Controls.Add(this.chkNameTracks);
            this.groupBox2.Location = new System.Drawing.Point(12, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(258, 220);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Misc options:";
            // 
            // chkHarmColorOnVocals
            // 
            this.chkHarmColorOnVocals.AutoSize = true;
            this.chkHarmColorOnVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkHarmColorOnVocals.Location = new System.Drawing.Point(15, 111);
            this.chkHarmColorOnVocals.Name = "chkHarmColorOnVocals";
            this.chkHarmColorOnVocals.Size = new System.Drawing.Size(197, 17);
            this.chkHarmColorOnVocals.TabIndex = 8;
            this.chkHarmColorOnVocals.Text = "Use Harm1 color for PART VOCALS";
            this.chkHarmColorOnVocals.UseVisualStyleBackColor = true;
            this.chkHarmColorOnVocals.CheckedChanged += new System.EventHandler(this.chkHarmColorOnVocals_CheckedChanged);
            // 
            // chkBWKeys
            // 
            this.chkBWKeys.AutoSize = true;
            this.chkBWKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBWKeys.Location = new System.Drawing.Point(15, 65);
            this.chkBWKeys.Name = "chkBWKeys";
            this.chkBWKeys.Size = new System.Drawing.Size(147, 17);
            this.chkBWKeys.TabIndex = 7;
            this.chkBWKeys.Text = "Black and white Pro-Keys";
            this.chkBWKeys.UseVisualStyleBackColor = true;
            this.chkBWKeys.CheckedChanged += new System.EventHandler(this.chkBWKeys_CheckedChanged);
            // 
            // chkHighlightSolos
            // 
            this.chkHighlightSolos.AutoSize = true;
            this.chkHighlightSolos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkHighlightSolos.Location = new System.Drawing.Point(15, 42);
            this.chkHighlightSolos.Name = "chkHighlightSolos";
            this.chkHighlightSolos.Size = new System.Drawing.Size(158, 17);
            this.chkHighlightSolos.TabIndex = 6;
            this.chkHighlightSolos.Text = "Highlight tracks during solos";
            this.chkHighlightSolos.UseVisualStyleBackColor = true;
            this.chkHighlightSolos.CheckedChanged += new System.EventHandler(this.chkHighlightSolos_CheckedChanged);
            // 
            // chkNameProKeys
            // 
            this.chkNameProKeys.AutoSize = true;
            this.chkNameProKeys.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkNameProKeys.Location = new System.Drawing.Point(15, 88);
            this.chkNameProKeys.Name = "chkNameProKeys";
            this.chkNameProKeys.Size = new System.Drawing.Size(171, 17);
            this.chkNameProKeys.TabIndex = 5;
            this.chkNameProKeys.Text = "Show note names for Pro-Keys";
            this.chkNameProKeys.UseVisualStyleBackColor = true;
            this.chkNameProKeys.CheckedChanged += new System.EventHandler(this.chkNameProKeys_CheckedChanged);
            // 
            // chkNameVocals
            // 
            this.chkNameVocals.AutoSize = true;
            this.chkNameVocals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkNameVocals.Location = new System.Drawing.Point(15, 134);
            this.chkNameVocals.Name = "chkNameVocals";
            this.chkNameVocals.Size = new System.Drawing.Size(216, 17);
            this.chkNameVocals.TabIndex = 4;
            this.chkNameVocals.Text = "Show note names for Vocals/Harmonies";
            this.chkNameVocals.UseVisualStyleBackColor = true;
            this.chkNameVocals.CheckedChanged += new System.EventHandler(this.chkNameVocals_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "MIDI chart window:";
            // 
            // cboWindow
            // 
            this.cboWindow.FormattingEnabled = true;
            this.cboWindow.Items.AddRange(new object[] {
            "1 second",
            "2 seconds",
            "3 seconds (default)",
            "4 seconds",
            "5 seconds",
            "6 seconds",
            "7 seconds",
            "8 seconds",
            "9 seconds",
            "10 seconds"});
            this.cboWindow.Location = new System.Drawing.Point(122, 184);
            this.cboWindow.Name = "cboWindow";
            this.cboWindow.Size = new System.Drawing.Size(121, 21);
            this.cboWindow.TabIndex = 2;
            this.cboWindow.SelectedIndexChanged += new System.EventHandler(this.cboWindow_SelectedIndexChanged);
            // 
            // cboSizing
            // 
            this.cboSizing.FormattingEnabled = true;
            this.cboSizing.Items.AddRange(new object[] {
            "Size notes using mixed mode (default)",
            "Size notes by charted note range",
            "Size notes by total valid note range"});
            this.cboSizing.Location = new System.Drawing.Point(15, 157);
            this.cboSizing.Name = "cboSizing";
            this.cboSizing.Size = new System.Drawing.Size(228, 21);
            this.cboSizing.TabIndex = 1;
            this.cboSizing.SelectedIndexChanged += new System.EventHandler(this.cboSizing_SelectedIndexChanged);
            // 
            // chkNameTracks
            // 
            this.chkNameTracks.AutoSize = true;
            this.chkNameTracks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkNameTracks.Location = new System.Drawing.Point(15, 19);
            this.chkNameTracks.Name = "chkNameTracks";
            this.chkNameTracks.Size = new System.Drawing.Size(132, 17);
            this.chkNameTracks.TabIndex = 0;
            this.chkNameTracks.Text = "Label each MIDI track";
            this.chkNameTracks.UseVisualStyleBackColor = true;
            this.chkNameTracks.CheckedChanged += new System.EventHandler(this.chkNameTracks_CheckedChanged);
            // 
            // MIDISelector
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(281, 371);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MIDISelector";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MIDI settings";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.MIDISelector_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MIDISelector_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

}

        #endregion

        private System.Windows.Forms.CheckBox chkDrums;
        private System.Windows.Forms.CheckBox chkBass;
        private System.Windows.Forms.CheckBox chkGuitar;
        private System.Windows.Forms.CheckBox chkVocals;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnNone;
        private System.Windows.Forms.CheckBox chkKeys;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkNameTracks;
        private System.Windows.Forms.ComboBox cboSizing;
        private System.Windows.Forms.ComboBox cboWindow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkNameProKeys;
        private System.Windows.Forms.CheckBox chkNameVocals;
        private System.Windows.Forms.CheckBox chkHighlightSolos;
        private System.Windows.Forms.CheckBox chkHarms;
        private System.Windows.Forms.CheckBox chkProKeys;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkBWKeys;
        private System.Windows.Forms.CheckBox chkHarmColorOnVocals;
    }
}