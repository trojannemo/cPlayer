using System;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class AudioSelector : Form
    {
        private readonly frmMain MainForm;
        public AudioSelector(frmMain xParent)
        {
            InitializeComponent();
            MainForm = xParent;
            ControlBox = false;
        }

        private void AudioSelector_Shown(object sender, EventArgs e)
        {
            chkDrums.Checked = MainForm.doAudioDrums;
            chkBass.Checked = MainForm.doAudioBass;
            chkGuitar.Checked = MainForm.doAudioGuitar;
            chkVocals.Checked = MainForm.doAudioVocals;
            chkKeys.Checked = MainForm.doAudioKeys;
            chkBacking.Checked = MainForm.doAudioBacking;
            chkCrowd.Checked = MainForm.doAudioCrowd;
        }

        private void CheckAll(bool enabled)
        {
            chkDrums.Checked = enabled;
            chkBass.Checked = enabled;
            chkGuitar.Checked = enabled;
            chkVocals.Checked = enabled;
            chkKeys.Checked = enabled;
            chkBacking.Checked = enabled;
            chkCrowd.Checked = enabled;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            CheckAll(true);
            UpdateAudioPlayback();
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            CheckAll(false);
            UpdateAudioPlayback();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateAudioPlayback()
        {
            MainForm.doAudioDrums = chkDrums.Checked;
            MainForm.doAudioBass = chkBass.Checked;
            MainForm.doAudioGuitar = chkGuitar.Checked;
            MainForm.doAudioVocals = chkVocals.Checked;
            MainForm.doAudioKeys = chkKeys.Checked;
            MainForm.doAudioBacking = chkBacking.Checked;
            MainForm.doAudioCrowd = chkCrowd.Checked;
            MainForm.UpdatePlayback(false);
        }

        private void chkDrums_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateAudioPlayback();
        }

        private void AudioSelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Dispose();
            }
        }

        private void AudioSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.isChoosingStems = false;
        }
    }
}
