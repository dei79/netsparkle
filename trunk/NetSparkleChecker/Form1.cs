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
    public partial class Form1 : Form
    {
        private Sparkle _sparkle;

        public Form1()
        {
            InitializeComponent();

            // get the commandline args
            String[] args = Environment.GetCommandLineArgs();
            if (args.Length !=3)
            {
                MessageBox.Show("Invalid count of parameters");
                return;
            }
            else
            {
                // get the parameter
                String referenceAssembly = args[1];
                String appCast = args[2];

                // build sparkle
                _sparkle = new Sparkle();

                // init the callback
                _sparkle.checkLoopFinished += new LoopFinishedOperation(_sparkle_checkLoopFinished);

                // init and start sparkle check
                _sparkle.StartLoop(appCast, referenceAssembly, true);                                                
            }
        }

        void _sparkle_checkLoopFinished(object sender, bool UpdateRequired)
        {
            // stop sparkle
            _sparkle.ShutdownSparkle();   

            // invoke if needed
            if (InvokeRequired)
            {
                Action<object, bool> act = new Action<object, bool>(_sparkle_checkLoopFinished);
                Invoke(act, sender, UpdateRequired);
            }
            else
            {
                // close the form
                Close();
            }
        }
    }
}
