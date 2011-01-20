using System;
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
using System.Reflection;

namespace AppLimit.NetSparkle
{
    public partial class NetSparkleDownloadProgress : Form
    {
        private String _tempName;
        private NetSparkleAppCastItem _item;
        private String _referencedAssembly;

        public NetSparkleDownloadProgress(NetSparkleAppCastItem item, String referencedAssembly, Image appIcon, Icon windowIcon)
        {
            InitializeComponent();

            if (appIcon != null)
                imgAppIcon.Image = appIcon;

            if (windowIcon != null)
                Icon = windowIcon;

            // store the item
            _item = item;
            _referencedAssembly = referencedAssembly;

            // init ui
            btnInstallAndReLaunch.Visible = false;
            lblHeader.Text = "Downloading " + item.AppName + " " + item.Version + "...";
            progressDownload.Maximum = 100;
            progressDownload.Minimum = 0;
            progressDownload.Step = 1;

            // show the right 
            Size = new Size(Size.Width, 107);
            lblSecurityHint.Visible = false;                
            
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

            // check if we have a dsa signature in appcast
            if (_item.DSASignature == null || _item.DSASignature.Length == 0)
                return;

            Boolean bDSAOk = false;

            // get the assembly
            if (File.Exists(_referencedAssembly))
            {
                String absolutePath = Path.GetFullPath(_referencedAssembly);
                if (!File.Exists(absolutePath))
                    throw new FileNotFoundException();

                Assembly refassembly = Assembly.LoadFile(absolutePath);
                if (refassembly != null)
                {
                    if (NetSparkleDSAVerificator.ExistsPublicKey(refassembly.GetName().Name + ".NetSparkle_DSA.pub"))
                    {
                        // check the DSA Code and modify the back color            
                        NetSparkleDSAVerificator dsaVerifier = new NetSparkleDSAVerificator(refassembly.GetName().Name + ".NetSparkle_DSA.pub");
                        bDSAOk = dsaVerifier.VerifyDSASignature(_item.DSASignature, _tempName);
                    }
                }
            }

            if ( !bDSAOk )
            {
                Size = new Size(Size.Width, 137);
                lblSecurityHint.Visible = true;
                BackColor = Color.Tomato;
            }                                   
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
            String cmd = "";

            // get the file type
            if (Path.GetExtension(_tempName).ToLower().Equals(".exe"))
            {
                cmd = _tempName;
            }
            else if (Path.GetExtension(_tempName).ToLower().Equals(".msi"))
            {
                // build up the command line
                cmd = Environment.ExpandEnvironmentVariables("%temp%\\" + Guid.NewGuid() + ".cmd");

                // generate the batch file
                StreamWriter write = new StreamWriter(cmd);
                write.WriteLine("msiexec /i " + _tempName);
                write.WriteLine("cd " + workingDir);
                write.WriteLine(cmdLine);
                write.Close();
            }
            else
            {
                MessageBox.Show("Updater not supported, please execute " + _tempName + " manually", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
                return;
            }

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
