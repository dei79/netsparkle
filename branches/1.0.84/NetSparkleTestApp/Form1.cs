﻿using System;
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

            _sparkle = new Sparkle("https://update.applimit.com/netsparkle/versioninfo.xml")
            {
                ShowDiagnosticWindow = true,
                TrustEverySSLConnection = true,
                //EnableSystemProfiling = true,
                //SystemProfileUrl = new Uri("http://update.applimit.com/netsparkle/stat/profileInfo.php")
            };

            _sparkle.updateDetected += new UpdateDetected(_sparkle_updateDetected);
            //_sparkle.EnableSilentMode = true;
            //_sparkle.HideReleaseNotes = true;

            _sparkle.StartLoop(true);
        }

        void _sparkle_updateDetected(object sender, UpdateDetectedEventArgs e)
        {
            DialogResult res = MessageBox.Show("Update detected, perform unattended", "Update", MessageBoxButtons.YesNoCancel);

            if (res == System.Windows.Forms.DialogResult.Yes)
                e.NextAction = nNextUpdateAction.performUpdateUnattended;
            else if (res == System.Windows.Forms.DialogResult.Cancel)
                e.NextAction = nNextUpdateAction.prohibitUpdate;
            else
                e.NextAction = nNextUpdateAction.showStandardUserInterface;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _sparkle.StopLoop();
        }

        private void btnStopLoop_Click(object sender, EventArgs e)
        {
            _sparkle.StopLoop();
        }

        private void btnTestLoop_Click(object sender, EventArgs e)
        {
            if (_sparkle.IsUpdateLoopRunning)
                MessageBox.Show("Loop is running");
            else
                MessageBox.Show("Loop is not running");
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            
        }
    }
}
