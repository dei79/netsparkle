using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AppLimit.NetSparkle;

namespace NetSparkleChecker
{
    public partial class NetSparkleCheckerWaitUI : Form
    {
        private Sparkle _sparkle;

        public Boolean SprakleRequestedUpdate = false;
        public NetSparkleAppCastItem LatesVersion = null;

        public NetSparkleCheckerWaitUI(Sparkle sparkle, Image appIcon, Icon windowIcon)
        {
            InitializeComponent();

            if (appIcon != null)
                imgAppIcon.Image = appIcon;

            if (windowIcon != null)
                Icon = windowIcon;

            _sparkle = sparkle;

            bckWorker.RunWorkerAsync();
        }

        private void bckWorker_DoWork(object sender, DoWorkEventArgs e)
        {            
            // get the config
            NetSparkleConfiguration config = _sparkle.GetApplicationConfig();

            // check for updats
            NetSparkleAppCastItem latestVersion = null;
            Boolean bUpdateRequired = _sparkle.IsUpdateRequired(config, out latestVersion);
                                
            // save the result
            SprakleRequestedUpdate = bUpdateRequired;
            LatesVersion = latestVersion;
        }

        private void bckWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // close the form
            Close();
        }
    }
}
