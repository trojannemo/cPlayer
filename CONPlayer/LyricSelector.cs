using System;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class LyricSelector : Form
    {
        private readonly frmMain MainForm;
        public LyricSelector(frmMain xParent)
        {
            InitializeComponent();
            MainForm = xParent;
            ControlBox = false;
        }

        private void LyricSelector_Shown(object sender, EventArgs e)
        {
            radioStatic.Checked = MainForm.doStaticLyrics;
            radioScrolling.Checked = MainForm.doScrollingLyrics;
            radioKaraoke.Checked = MainForm.doKaraokeLyrics;
            radioWhole.Checked = MainForm.doWholeWordsLyrics;
            radioNone.Checked = !radioStatic.Checked && !radioScrolling.Checked && !radioKaraoke.Checked;
            radioGame.Checked = !radioWhole.Checked;
            radioHarmonies.Checked = MainForm.doHarmonyLyrics;
            radioVocals.Checked = !radioHarmonies.Checked;
            UpdatePanels();
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void UpdateLyrics()
        {
            MainForm.doStaticLyrics = radioStatic.Checked && !radioNone.Checked;
            MainForm.doScrollingLyrics = radioScrolling.Checked && !radioNone.Checked;
            MainForm.doKaraokeLyrics = radioKaraoke.Checked && !radioNone.Checked;
            MainForm.doWholeWordsLyrics = radioWhole.Checked;
            MainForm.doHarmonyLyrics = radioHarmonies.Checked && !radioNone.Checked;
            MainForm.UpdateDisplay(false);
        }
        
        private void btnDefault_Click(object sender, EventArgs e)
        {
            radioStatic.Checked = true;
            radioWhole.Checked = true;
            radioVocals.Checked = true;
            UpdatePanels();
            UpdateLyrics();
        }
        
        private void LyricSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateLyrics();
        }

        private void UpdatePanels()
        {
            grpDisplay.Enabled = !radioNone.Checked;
            grpOptions.Enabled = !radioNone.Checked && !radioScrolling.Checked;
        }

        private void radioStatic_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePanels();
            UpdateLyrics();
        }

        private void LyricSelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Dispose();
            }
        }
    }
}
