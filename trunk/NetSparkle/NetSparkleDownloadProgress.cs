﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace AppLimit.NetSparkle
{
    public partial class NetSparkleDownloadProgress : Form
    {
        private String _tempName;

        public NetSparkleDownloadProgress(NetSparkleAppCastItem item)
        {
            InitializeComponent();

            // init ui
            btnInstallAndReLaunch.Visible = false;
            lblHeader.Text = "Downloading " + item.AppName + " " + item.Version + "...";
            progressDownload.Maximum = 100;
            progressDownload.Minimum = 0;
            progressDownload.Step = 1;
            
            // get the filename of the download lin
            String[] segments = item.DownloadLink.Split('/');
            String fileName = segments[segments.Length - 1];

            // get temp path
            _tempName = Environment.ExpandEnvironmentVariables("%temp%\\" + fileName);

            // start async download
            WebClient Client = new WebClient();
            Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);

            Uri url = new Uri(item.DownloadLink);

            Client.DownloadFileAsync(url, _tempName);
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            progressDownload.Visible = false;
            btnInstallAndReLaunch.Visible = true;            
        }
               
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressDownload.Value = e.ProgressPercentage;            
        }

        private void btnInstallAndReLaunch_Click(object sender, EventArgs e)
        {
            // get the commandline 
            String cmdLine = Environment.CommandLine;
            String workingDir = Environment.CurrentDirectory;

            // generate the batch file path
            String cmd = Environment.ExpandEnvironmentVariables("%temp%\\" + Guid.NewGuid() + ".cmd");

            // generate the batch file
            StreamWriter write = new StreamWriter(cmd);
            write.WriteLine("msiexec /i " + _tempName);
            write.WriteLine("cd " + workingDir);
            write.WriteLine(cmdLine);
            write.Close();

            // start the installer helper
            Process process = new Process();
            process.StartInfo.FileName = cmd;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            

            // quit the app
            Environment.Exit(0);
        }
    }
}
