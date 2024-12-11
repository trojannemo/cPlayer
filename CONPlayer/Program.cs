using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Un4seen.Bass;

namespace cPlayer
{
    static class Program
    {
        private const string APP_NAME = "cPlayer";
        private const string bKey = "2X14232420202322";
        private const string user = "nemo";
        private const string domain = "keepitfishy";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BassNet.Registration(user + "@" + domain + ".com", bKey);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = (Exception)e.ExceptionObject;
                var vers = Assembly.GetExecutingAssembly().GetName().Version;
                var version = String.Format("{0}.{1}.{2}", vers.Major, vers.Minor, vers.Build);

                Clipboard.SetText(APP_NAME + " crashed! Please see the error log below:" + Environment.NewLine + Environment.NewLine + "[code]" +
                        Environment.NewLine + APP_NAME + " version " + version + Environment.NewLine + "Error Message:" + Environment.NewLine +
                        ex.Message + Environment.NewLine + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine + "[/code]");

                if (MessageBox.Show("Derp, " + APP_NAME + " has crashed! Sorry.\nI copied some helpful information to your clipboard " +
                    "that can help fix this in the future.\nClick OK to visit my website and you can find how to contact me through there.\nClick Cancel to close this error message and " + APP_NAME + " will exit.",
                    "Fatal Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK)
                {
                    Process.Start("https://nemosnautilus.com/cplayer/");
                }
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
