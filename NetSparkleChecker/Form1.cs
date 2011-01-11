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
            if (args.Length == 0)
                return;
            else
            {
                _sparkle = new Sparkle(args[0], false);
                
                NetSparkleConfiguration conf = new NetSparkleConfiguration();

                NetSparkleAppCast cast = new NetSparkleAppCast(args[1], conf);
                cast.GetLatestVersion();

                _sparkle.checkLoopFinished += new LoopFinishedOperation(_sparkle_checkLoopFinished);
            }
        }

        void _sparkle_checkLoopFinished(object sender, bool UpdateRequired)
        {
            
        }
    }
}
