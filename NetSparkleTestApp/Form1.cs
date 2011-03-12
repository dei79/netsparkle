using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

            _sparkle = new Sparkle("http://update.applimit.com/netsparkle/versioninfo.xml") { ShowDiagnosticWindow = true };

            _sparkle.StartLoop(true);    
        }
    }
}
