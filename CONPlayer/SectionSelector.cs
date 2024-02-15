using System;
using System.Drawing;
using System.Windows.Forms;

namespace cPlayer
{
    public partial class SectionSelector : Form
    {
        private readonly frmMain xParent;
        private readonly Point StartLocation;
        private bool isLoading;

        public SectionSelector(frmMain parent, Point start)
        {
            InitializeComponent();
            xParent = parent;
            StartLocation = start;
        }

        private void SectionSelector_Shown(object sender, EventArgs e)
        {
            isLoading = true;
            Location = StartLocation;
            GetPracticeSessions();
            isLoading = false;
            timer1.Enabled = true;
        }

        private void GetPracticeSessions()
        {
            lstSections.Items.Clear();
            foreach (var section in xParent.PracticeSessions)
            {
                lstSections.Items.Add(section.SectionName);
            }
            var index = 0;
            for (var i = 0; i < xParent.PracticeSessions.Count; i++)
            {
                if (xParent.PracticeSessions[i].SectionStart <= xParent.PlaybackSeconds)
                {
                    index = i;
                    continue;
                }
                break;
            }
            lstSections.SelectedIndex = index;
        }

        private void lstSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            xParent.SetPlayLocation(xParent.PracticeSessions[lstSections.SelectedIndex].SectionStart);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            isLoading = true;
            GetPracticeSessions();
            isLoading = false;
        }
        
        private void SectionSelector_Deactivate(object sender, EventArgs e)
        {
            Dispose();
        }

        private void SectionSelector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Dispose();
            }
        }
    }
}
