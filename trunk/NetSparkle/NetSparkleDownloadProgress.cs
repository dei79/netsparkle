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
        private Sparkle _sparkle;
        private Boolean _unattend;

        public NetSparkleDownloadProgress(Sparkle sparkle, NetSparkleAppCastItem item, String referencedAssembly, Image appIcon, Icon windowIcon, Boolean Unattend)
        {
            InitializeComponent();

            if (appIcon != null)
                imgAppIcon.Image = appIcon;

            if (windowIcon != null)
                Icon = windowIcon;

            // store the item
            _sparkle = sparkle;
            _item = item;
            _referencedAssembly = referencedAssembly;
            _unattend = Unattend;

            // init ui
            btnInstallAndReLaunch.Visible = false;
            lblHeader.Text = lblHeader.Text.Replace("APP", item.AppName + " " + item.Version);
            progressDownload.Maximum = 100;
            progressDownload.Minimum = 0;
            progressDownload.Step = 1;

            // show the right 
            Size = new Size(Size.Width, 107);
            lblSecurityHint.Visible = false;                
            
            // get the filename of the download link
            String[] segments = item.DownloadLink.Split('/');
            String fileName = segments[segments.Length - 1];

            //trim url parameters
            if(fileName.LastIndexOf('?') > 0) {
                fileName = fileName.Substring(0, fileName.LastIndexOf('?'));
            }

            //sanitize filename
            fileName = MakeValidFileName(fileName);

            //if no extension present make msi the default extension
            if (Path.GetExtension(fileName).Length == 0)
            {
                fileName += ".msi";
            }

            // get temp path
            _tempName = Environment.ExpandEnvironmentVariables("%temp%\\" + fileName);

            //check if file already exists and add counter
            while(File.Exists(_tempName))
            {
                _tempName = Path.GetDirectoryName(_tempName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(_tempName) + DateTime.Now.ToFileTimeUtc() + Path.GetExtension(_tempName);
            }

            // start async download
            WebClient Client = new WebClient();
            Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);

            Uri url = new Uri(item.DownloadLink);

            Client.DownloadFileAsync(url, _tempName);
        }

        //from http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidReStr, "_");
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            progressDownload.Visible = false;
            btnInstallAndReLaunch.Visible = true;            

            // report message            
            _sparkle.ReportDiagnosticMessage("Finished downloading file to: " + _tempName);

            if (!NetSparkleCheckAndInstall.CheckDSA(_sparkle, _item, _tempName))
            {
                Size = new Size(Size.Width, 137);
                lblSecurityHint.Visible = true;
                BackColor = Color.Tomato;
            }
               
            // Check the unattended mode
            if (_unattend)
                btnInstallAndReLaunch_Click(null, null);
        }
               
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressDownload.Value = e.ProgressPercentage;            
        }

        private void btnInstallAndReLaunch_Click(object sender, EventArgs e)
        {
            NetSparkleCheckAndInstall.Install(_sparkle, _tempName);
        }
    }
}
