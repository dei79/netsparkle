using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppLimit.NetSparkle
{
    public partial class NetSparkleForm : Form
    {
        NetSparkleAppCastItem _currentItem;
        
        public NetSparkleForm(NetSparkleAppCastItem item, Image appIcon, Icon windowIcon)
        {            
            InitializeComponent();
            
            _currentItem = item;

            lblHeader.Text = "A new version of " + item.AppName + " is available!";
            lblInfoText.Text = item.AppName + " " + item.Version + " is now available (you have " + item.AppVersionInstalled + "). Would you like to download it now?";

            if (item.ReleaseNotesLink != null && item.ReleaseNotesLink.Length > 0)
                NetSparkleBrowser.Navigate(item.ReleaseNotesLink);
            else
            {
                NetSparkleBrowser.Navigate("about:blank");
                HtmlDocument doc = NetSparkleBrowser.Document;
                doc.Write(string.Empty);
                NetSparkleBrowser.DocumentText = "<b>Currently no release notes available!</b>";
            }

            if (appIcon != null)
                imgAppIcon.Image = appIcon;

            if (windowIcon != null)
                Icon = windowIcon;
        }

        private void skipButton_Click(object sender, EventArgs e)
        {
            // set the dialog result to no
            this.DialogResult = DialogResult.No;

            // close the windows
            Close();
        }

        private void buttonRemind_Click(object sender, EventArgs e)
        {
            // set the dialog result ot retry
            this.DialogResult = DialogResult.Retry;

            // close the window
            Close();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            // set the result to yes
            DialogResult = DialogResult.Yes;

            // close the dialog
            Close();            
        }
    }
}
