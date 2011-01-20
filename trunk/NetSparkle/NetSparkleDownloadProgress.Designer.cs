namespace AppLimit.NetSparkle
{
    partial class NetSparkleDownloadProgress
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetSparkleDownloadProgress));
            this.lblHeader = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressDownload = new System.Windows.Forms.ProgressBar();
            this.btnInstallAndReLaunch = new System.Windows.Forms.Button();
            this.lblSecurityHint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(66, 12);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(301, 22);
            this.lblHeader.TabIndex = 8;
            this.lblHeader.Text = "Downloading APP X.X.X.X ...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AppLimit.NetSparkle.Properties.Resources.software_update_available1;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // progressDownload
            // 
            this.progressDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressDownload.Location = new System.Drawing.Point(69, 37);
            this.progressDownload.Name = "progressDownload";
            this.progressDownload.Size = new System.Drawing.Size(287, 23);
            this.progressDownload.TabIndex = 9;
            // 
            // btnInstallAndReLaunch
            // 
            this.btnInstallAndReLaunch.Location = new System.Drawing.Point(148, 37);
            this.btnInstallAndReLaunch.Name = "btnInstallAndReLaunch";
            this.btnInstallAndReLaunch.Size = new System.Drawing.Size(127, 23);
            this.btnInstallAndReLaunch.TabIndex = 10;
            this.btnInstallAndReLaunch.Text = "Install and Relaunch";
            this.btnInstallAndReLaunch.UseVisualStyleBackColor = true;
            this.btnInstallAndReLaunch.Click += new System.EventHandler(this.btnInstallAndReLaunch_Click);
            // 
            // lblSecurityHint
            // 
            this.lblSecurityHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecurityHint.Location = new System.Drawing.Point(66, 63);
            this.lblSecurityHint.Name = "lblSecurityHint";
            this.lblSecurityHint.Size = new System.Drawing.Size(290, 34);
            this.lblSecurityHint.TabIndex = 11;
            this.lblSecurityHint.Text = "The update was affected by not verified changes, it could be unsafe to install it" +
                "!";
            this.lblSecurityHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NetSparkleDownloadProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(368, 99);
            this.Controls.Add(this.lblSecurityHint);
            this.Controls.Add(this.btnInstallAndReLaunch);
            this.Controls.Add(this.progressDownload);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NetSparkleDownloadProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Software Download";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar progressDownload;
        private System.Windows.Forms.Button btnInstallAndReLaunch;
        private System.Windows.Forms.Label lblSecurityHint;


    }
}