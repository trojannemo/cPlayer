using NAudio.Wave;
using System;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class MicControl : Form
    {                
        private frmMain MainForm;
        private readonly float VolumeLevel;

        public MicControl(frmMain mainForm)
        {
            InitializeComponent();
            InitializeMicrophones();
            VolumeLevel = mainForm.volumeProvider == null ? 0f : mainForm.volumeProvider.Volume;
            MainForm = mainForm;
            tbVolume.Value = (int)(VolumeLevel * 100f);
            if (mainForm.microphoneIndex < 0 || mainForm.microphoneIndex > lstMicrophones.Items.Count - 1) return;
            lstMicrophones.SelectedIndex = mainForm.microphoneIndex;
        }

        private void InitializeMicrophones()
        {
            // List all available microphones
            lstMicrophones.Items.Clear();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var deviceInfo = WaveIn.GetCapabilities(i);
                lstMicrophones.Items.Add($"{i}: {deviceInfo.ProductName}");
            }

            // Disable controls if no microphones are available
            if (lstMicrophones.Items.Count == 0)
            {
                lstMicrophones.Items.Add("No microphones found");
                lstMicrophones.Enabled = false;
            }
        }

        private void btnDeselect_Click(object sender, EventArgs e)
        {
            lstMicrophones.ClearSelected();
            MainForm.StopPassthrough();
        }                       
       
        private void lstMicrophones_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.StopPassthrough();
            MainForm.microphoneIndex = lstMicrophones.SelectedIndex;

            if (lstMicrophones.SelectedIndex < 0) return;

            var selectedDeviceIndex = lstMicrophones.SelectedIndex;
            MainForm.StartPassthrough(selectedDeviceIndex, tbVolume.Value);
        }

        private void tbVolume_ValueChanged(object sender, EventArgs e)
        {
            if (MainForm.volumeProvider != null)
            {
                MainForm.volumeProvider.Volume = tbVolume.Value / 100f;
                lblVolume.Text = $"Volume: {tbVolume.Value}%";
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var message = "This feature only works with microphones that use Windows' default audio driver\n\nIf you're using a USB Logitech Rock Band or Guitar Hero microphone, it might be using the libusbk drivers instead\n\nFollow these steps to fix this:\n- open Device Manager\n- find your microphone (might be under the libusbk heading)\n- right-click on your microphone\n- click on Properties\n- click on Driver\n- click on Update Driver\n- click on Browse my computer for drivers\n- click on Let me pick from a list...\n- then select the default audio driver\n\nRestart cPlayer and it should work";
            MessageBox.Show(message, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
