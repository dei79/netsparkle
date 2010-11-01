using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AppLimit.NetSparkle;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        private Sparkle _sparkle; 

        public Form1()
        {
            InitializeComponent();

            label1.Text = Environment.CommandLine;

            _sparkle = new Sparkle("http://update.applimit.com/businessbox/versioninfo.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _sparkle.CheckForUpdates();
        }
    }
}
