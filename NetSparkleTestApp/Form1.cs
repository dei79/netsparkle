using System;
using System.Windows.Forms;
using AppLimit.NetSparkle;

namespace NetSparkleTestApp
{
    public partial class Form1 : Form
    {
        private Sparkle _sparkle;

        public Form1()
        {
            InitializeComponent();

            _sparkle = new Sparkle("http://update.applimit.com/netsparkle/versioninfo.xml")
            {
                ShowDiagnosticWindow = true,
                EnableSystemProfiling = true,
                //SystemProfileUrl = new Uri("http://update.applimit.com/netsparkle/stat/profileInfo.php")
            };

            //_sparkle.EnableSilentMode = true;
            //_sparkle.HideReleaseNotes = true;

            _sparkle.StartLoop(true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _sparkle.StopLoop();
        }
    }
}
