using System;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class MIDISelector : Form
    {
        private readonly frmMain MainForm;
        public MIDISelector(frmMain xParent)
        {
            InitializeComponent();
            MainForm = xParent;
            ControlBox = false;
        }

        private void MIDISelector_Shown(object sender, EventArgs e)
        {
            chkDrums.Checked = MainForm.doMIDIDrums;
            chkBass.Checked = MainForm.doMIDIBass;
            chkGuitar.Checked = MainForm.doMIDIGuitar;
            chkVocals.Checked = MainForm.doMIDIVocals;
            chkHarms.Checked = MainForm.doMIDIHarmonies;
            chkKeys.Checked = MainForm.doMIDIKeys;
            chkProKeys.Checked = MainForm.doMIDIProKeys;
            cboSizing.SelectedIndex = MainForm.NoteSizingType;
            chkNameTracks.Checked = MainForm.doMIDINameTracks;
            chkNameVocals.Checked = MainForm.doMIDINameVocals;
            chkNameProKeys.Checked = MainForm.doMIDINameProKeys;
            cboWindow.SelectedIndex = (int)MainForm.PlaybackWindow - 1;
            chkHighlightSolos.Checked = MainForm.doMIDIHighlightSolos;
            chkBWKeys.Checked = MainForm.doMIDIBWKeys;
            chkHarmColorOnVocals.Checked = MainForm.doMIDIHarm1onVocals;
        }

        private void CheckAll(bool enabled)
        {
            chkDrums.Checked = enabled;
            chkBass.Checked = enabled;
            chkGuitar.Checked = enabled;
            chkHarms.Checked = enabled;
            chkProKeys.Checked = enabled;
            if (enabled) return;
            chkKeys.Checked = false;
            chkVocals.Checked = false;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            CheckAll(true);
            UpdateMIDITracks();
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            CheckAll(false);
            UpdateMIDITracks();
        }
        
        private void UpdateMIDITracks()
        {
            MainForm.ClearVisuals();
            MainForm.doMIDIDrums = chkDrums.Checked;
            MainForm.doMIDIBass = chkBass.Checked;
            MainForm.doMIDIGuitar = chkGuitar.Checked;
            MainForm.doMIDIVocals = chkVocals.Checked;
            MainForm.doMIDIHarmonies = chkHarms.Checked;
            MainForm.doMIDIKeys = chkKeys.Checked;
            MainForm.doMIDIProKeys = chkProKeys.Checked;
        }

        private void chkDrums_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateMIDITracks();
        }

        private void chkNameTracks_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.doMIDINameTracks = chkNameTracks.Checked;
        }

        private void cboSizing_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.NoteSizingType = cboSizing.SelectedIndex >= 0 ? cboSizing.SelectedIndex : 0;
        }

        private void cboWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboWindow.SelectedIndex == -1)
            {
                MainForm.PlaybackWindow = 3.0;//default
            }
            else
            {
                MainForm.PlaybackWindow = cboWindow.SelectedIndex + 1.0;
            }
        }

        private void chkNameVocals_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.doMIDINameVocals = chkNameVocals.Checked;
        }

        private void chkNameProKeys_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.doMIDINameProKeys = chkNameProKeys.Checked;
        }

        private void chkHighlightSolos_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.doMIDIHighlightSolos = chkHighlightSolos.Checked;
        }

        private void MIDISelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Dispose();
            }
        }

        private void chkProKeys_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkProKeys.Checked) return;
            chkKeys.Checked = false;
        }

        private void chkKeys_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkKeys.Checked) return;
            chkProKeys.Checked = false;
        }

        private void chkHarms_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkHarms.Checked) return;
            chkVocals.Checked = false;
        }

        private void chkVocals_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkVocals.Checked) return;
            chkHarms.Checked = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void chkBWKeys_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.doMIDIBWKeys = chkBWKeys.Checked;
            MainForm.ClearNoteColors(false, true);
        }

        private void chkHarmColorOnVocals_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.doMIDIHarm1onVocals = chkHarmColorOnVocals.Checked;
            MainForm.ClearNoteColors(true);
        }
    }
}
