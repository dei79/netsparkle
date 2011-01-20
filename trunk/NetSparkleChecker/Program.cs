using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using AppLimit.NetSparkle;

namespace NetSparkleChecker
{
    static class Program
    {
        private static Form _frmWait;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // init app
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // create wait form
            _frmWait = new NetSparkleCheckerWaitUI();
            _frmWait.Show();

            // get the commandline args
            String[] args = Environment.GetCommandLineArgs();
            if (args.Length != 3)
            {
                MessageBox.Show("The NetSparkle Update Checker requires the following 2 commandline parameters:\n\n1: path to the application executable\n2: url of the appcast", 
                                "NetSparkle Update Checker - Syntax", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                Sparkle _sparkle;

                // get the parameter
                String referenceAssembly = args[1];
                String appCast = args[2];

                // check parameter
                if (!File.Exists(referenceAssembly))
                {
                    MessageBox.Show("Couldn't find the application executable (" + Path.GetFullPath(referenceAssembly) + ")",
                                    "NetSparkle Update Checker - Missing File",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // build sparkle
                _sparkle = new Sparkle(appCast, referenceAssembly);

                // get the config
                NetSparkleConfiguration config = _sparkle.GetApplicationConfig();

                // check for updats
                NetSparkleAppCastItem latestVersion = null;
                Boolean bUpdateRequired = _sparkle.IsUpdateRequired(config, out latestVersion);
                
                // close the check form
                _frmWait.Close();
                
                // show update dialg
                if (bUpdateRequired)
                {
                    _sparkle.ShowUpdateNeededUI(latestVersion);
                }                                    
            }                        
        }                   
    }
}
