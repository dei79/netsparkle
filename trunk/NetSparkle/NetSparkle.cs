using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;

namespace AppLimit.NetSparkle
{
    public delegate void LoopStartedOperation(Object sender);
    public delegate void LoopFinishedOperation(Object sender, Boolean UpdateRequired);
    
    public class Sparkle : IDisposable
    {
        private BackgroundWorker _worker = new BackgroundWorker();

        private String          _AppCastUrl;
        private String          _AppReferenceAssembly;

        private Boolean         _DoInitialCheck;
        private Boolean         _ForceInitialCheck;

        private EventWaitHandle _exitHandle;        

        private NetSparkleMainWindows _DiagnosticWindow;

        /// <summary>
        /// Enables system profiling against a profile server
        /// </summary>
        public Boolean EnableSystemProfiling = false;
        
        /// <summary>
        /// Contains the profile url for System profiling
        /// </summary>
        public Uri SystemProfileUrl;

        /// <summary>
        /// This event will be raised when a check loop will be started
        /// </summary>
        public event LoopStartedOperation checkLoopStarted;

        /// <summary>
        /// This event will be raised when a check loop is finished
        /// </summary>
        public event LoopFinishedOperation checkLoopFinished;

        /// <summary>
        /// This property holds an optional application icon
        /// which will be displayed in the software update dialog. The icon has
        /// to be 48x48 pixels.
        /// </summary>
        public Image ApplicationIcon { get; set; }

        /// <summary>
        /// This property returns an optional application icon 
        /// which will displayed in the windows as self
        /// </summary>
        public Icon  ApplicationWindowIcon { get; set; }

        /// <summary>
        /// This property enables a diagnostic window for debug reasons
        /// </summary>
        public Boolean ShowDiagnosticWindow { get; set; }

        /// <summary>
        /// ctor which needs the appcast url
        /// </summary>
        /// <param name="appcastUrl"></param>
        public Sparkle(String appcastUrl)
            : this(appcastUrl, null)
        { }

        /// <summary>
        /// ctor which needs the appcast url and a referenceassembly
        /// </summary>
        public Sparkle(String appcastUrl, String referenceAssembly)
            : this(appcastUrl, referenceAssembly, false)
        {}

        /// <summary>
        /// ctor which needs the appcast url and a referenceassembly
        /// </summary>        
        public Sparkle(String appcastUrl, String referenceAssembly, Boolean ShowDiagnostic )            
        {
            // enable visual style to ensure that we have XP style or higher
            // also in WPF applications
            System.Windows.Forms.Application.EnableVisualStyles();
            
            // reset vars
            ApplicationIcon = null;
            _AppReferenceAssembly = null;

            // set var
            ShowDiagnosticWindow = ShowDiagnostic;

            // create the diagnotic window
            _DiagnosticWindow = new NetSparkleMainWindows();

            // show if needed
            ShowDiagnosticWindowIfNeeded();
                       
            // adjust the delegates
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);

            // build the wait handle
            _exitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);            

            // set the url
            _AppCastUrl = appcastUrl;
            _DiagnosticWindow.Report("Using the following url: " + _AppCastUrl);

            // set the reference assembly
            if (referenceAssembly != null)
            {
                _AppReferenceAssembly = referenceAssembly;
                _DiagnosticWindow.Report("Checking the following file: " + _AppReferenceAssembly);
            }
        }
               
        /// <summary>
        /// The method starts a NetSparkle background loop
        /// If NetSparkle is configured to check for updates on startup, proceeds to perform 
        /// the check. You should only call this function when your app is initialized and 
        /// shows its main window.        
        /// </summary>        
        /// <param name="doInitialCheck"></param>
        public void StartLoop(Boolean doInitialCheck)
        {            
            StartLoop(doInitialCheck, false);
        }

        /// <summary>
        /// The method starts a NetSparkle background loop
        /// If NetSparkle is configured to check for updates on startup, proceeds to perform 
        /// the check. You should only call this function when your app is initialized and 
        /// shows its main window.
        /// </summary>
        /// <param name="doInitialCheck"></param>
        /// <param name="forceInitialCheck"></param>
        public void StartLoop(Boolean doInitialCheck, Boolean forceInitialCheck)
        {
            // Start the helper thread as a background worker to 
            // get well ui interaction                        

            // show if needed
            ShowDiagnosticWindowIfNeeded();

            // store infos
            _DoInitialCheck     = doInitialCheck;
            _ForceInitialCheck  = forceInitialCheck;

            // create and configure the worker
            _DiagnosticWindow.Report("Starting background worker");            
            
            // start the work
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// This method will stop the sparkle background loop and is called
        /// through the disposable interface automatically
        /// </summary>
        public void StopLoop()
        {
            // ensure the work will finished
            _exitHandle.Set();
        }

        /// <summary>
        /// Is called in the using context and will stop all background activities
        /// </summary>
        public void Dispose()
        {
            StopLoop();
        }

        /// <summary>
        /// This method updates the profile information which can be sended to the server if enabled    
        /// </summary>
        /// <param name="config"></param>
        public void UpdateSystemProfileInformation(NetSparkleConfiguration config)
        {
            // check if profile data is enabled
            if (!EnableSystemProfiling)
                return;

            // check if we need an update
            if (DateTime.Now  - config.LastProfileUpdate < new TimeSpan(7, 0, 0, 0))
                return;
                
            // touch the profile update time
            config.TouchProfileTime();

            // start the profile thread
            Thread t = new Thread(ProfileDataThreadStart);
            t.Start(config);            
        }

        /// <summary>
        /// Profile data thread
        /// </summary>
        /// <param name="obj"></param>
        private void ProfileDataThreadStart(object obj)
        {
            // get the config
            NetSparkleConfiguration config = obj as NetSparkleConfiguration;

            // build the webrequest url
            String requestUrl = SystemProfileUrl.ToString() + "?";

            // collect data
            NetSparkleDeviceInventory inv = new NetSparkleDeviceInventory(config);
            inv.CollectInventory();

            // build url
            requestUrl = inv.BuildRequestUrl(requestUrl);
                                                                                       
            // perform the webrequest
            HttpWebRequest request = HttpWebRequest.Create(requestUrl) as HttpWebRequest;
            using (WebResponse response = request.GetResponse())
            {
                // close the response 
                response.Close();
            }
        }

        /// <summary>
        /// This method checks if an update is required. During this process the appcast
        /// will be downloaded and checked against the reference assembly. Ensure that
        /// the calling process has access to the internet and read access to the 
        /// reference assembly. This method is also called from the background loops.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public Boolean IsUpdateRequired(NetSparkleConfiguration config, out NetSparkleAppCastItem latestVersion)
        {
            // report
            ReportDiagnosticMessage("Downloading and checking appcast");

            // init the appcast
            NetSparkleAppCast cast = new NetSparkleAppCast(_AppCastUrl, config);

            // check if any updates are available
            latestVersion = cast.GetLatestVersion();
            if (latestVersion == null)
            {
                ReportDiagnosticMessage("No version information in app cast found");
                return false;
            }

            // set the last check time
            ReportDiagnosticMessage("Touch the last check timestamp");
            config.TouchCheckTime();            
                
            // check if the available update has to be skipped
            if (latestVersion.Version.Equals(config.SkipThisVersion))
            {
                ReportDiagnosticMessage("Latest update has to be skipped (user decided to skip version "+ config.SkipThisVersion +")");
                return false;
            }

            // check if the version will be the same then the installed version
            Version v1 = new Version(config.InstalledVersion);
            Version v2 = new Version(latestVersion.Version);

            if (v2 <= v1)
            {
                ReportDiagnosticMessage("Installed version is valid, no update needed (" + config.InstalledVersion + ")");
                return false;
            }

            // ok we need an update
            return true;
        }

        /// <summary>
        /// This method reads the local sparkle configuration for the given
        /// reference assembly
        /// </summary>
        /// <param name="AppReferenceAssembly"></param>
        /// <returns></returns>
        public NetSparkleConfiguration GetApplicationConfig()
        {
            NetSparkleConfiguration config;
            config = new NetSparkleConfiguration(_AppReferenceAssembly);
            return config;
        }

        /// <summary>
        /// This method shows the update ui and allows to perform the 
        /// update process
        /// </summary>
        /// <param name="currentItem"></param>
        public void ShowUpdateNeededUI(NetSparkleAppCastItem currentItem)
        {
            NetSparkleForm frm = new NetSparkleForm(currentItem, ApplicationIcon, ApplicationWindowIcon);
            frm.TopMost = true;
            DialogResult dlgResult = frm.ShowDialog();

            if (dlgResult == DialogResult.No)
            {
                // skip this version
                NetSparkleConfiguration config = new NetSparkleConfiguration(_AppReferenceAssembly);
                config.SetVersionToSkip(currentItem.Version);
            }
            else if (dlgResult == DialogResult.Yes)
            {
                // download the binaries
                InitDownloadAndInstallProcess(currentItem);
            }
        }

        /// <summary>
        /// This method reports a message in the diagnostic window
        /// </summary>
        /// <param name="message"></param>
        public void ReportDiagnosticMessage(String message)
        {
            if (_DiagnosticWindow.InvokeRequired)
            {
                _worker.ReportProgress(0, message);
            }
            else
            {
                _DiagnosticWindow.Report(message);
            }
        }

        /// <summary>
        /// This method will be executed as worker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // build a 24 houres timespan
            TimeSpan tsp = new TimeSpan(24,0,0);            

            // store the did run once feature
            Boolean goIntoLoop = true;
            Boolean checkTSP = true;
            Boolean doInitialCheck = _DoInitialCheck;
            Boolean isInitialCheck = true;

            // start our lifecycles
            do
            {
                // set state
                Boolean bUpdateRequired = false;

                // notify
                if (checkLoopStarted != null)
                    checkLoopStarted(this);

                // report status
                if (doInitialCheck == false)
                {
                    ReportDiagnosticMessage("Initial check prohibited, going to wait");
                    doInitialCheck = true;
                    goto WaitSection;
                }

                // report status
                ReportDiagnosticMessage("Starting update loop...");

                // read the config
                ReportDiagnosticMessage("Reading config...");
                NetSparkleConfiguration config = GetApplicationConfig();

                // calc CheckTasp
                Boolean checkTSPInternal = checkTSP;

                if (isInitialCheck && checkTSPInternal)
                    checkTSPInternal = !_ForceInitialCheck;

                // check if it's ok the recheck to software state
                if (checkTSPInternal)
                {                              
                    TimeSpan csp = DateTime.Now - config.LastCheckTime;
                    if (csp < tsp )
                    {
                        ReportDiagnosticMessage("Update check performed within the last 24 houres!");
                        goto WaitSection;
                    }
                }
                else
                    checkTSP = true;

                // when sparkle will be deactivated wait an other cycle
                if (config.CheckForUpdate == false)
                {
                    ReportDiagnosticMessage("Check for updates disabled");
                    goto WaitSection;
                }

                // update the runonce feature
                goIntoLoop = !config.DidRunOnce;
                
                // update profile information is needed
                UpdateSystemProfileInformation(config);
                
                // check if update is required
                NetSparkleAppCastItem latestVersion = null;
                bUpdateRequired = IsUpdateRequired(config, out latestVersion);
                if (!bUpdateRequired)
                    goto WaitSection;

                // show the update windows     
                ReportDiagnosticMessage("Update needed from version " + config.InstalledVersion + " to version " + latestVersion.Version);
                _worker.ReportProgress(1, latestVersion);

            WaitSection:
                // reset initialcheck
                isInitialCheck = false;

                // notify
                if (checkLoopFinished != null)
                    checkLoopFinished(this, bUpdateRequired);

                // report wait statement
                ReportDiagnosticMessage("Sleeping for an other 24 houres, exit event or force update check event");

                // wait for
                if (!goIntoLoop)
                    break;
                else
                {
                    // build the event array
                    WaitHandle[] handles = new WaitHandle[1];
                    handles[0] = _exitHandle;
                                        
                    // wait for any
                    int i = WaitHandle.WaitAny(handles, tsp);
                    if (WaitHandle.WaitTimeout == i)
                    {
                        ReportDiagnosticMessage("24 houres are over");
                        continue;
                    }

                    // check the exit hadnle
                    if (i == 0)
                    {
                        ReportDiagnosticMessage("Got exit signal");
                        break;
                    }

                    // check an other check needed
                    if (i == 1)
                    {
                        ReportDiagnosticMessage("Got force update check signal");
                        checkTSP = false;
                        continue;
                    }
                }
            } while (goIntoLoop);
        }        

        /// <summary>
        /// This method will be notified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                case 1:
                    {
                        // get the current item
                        NetSparkleAppCastItem currentItem = e.UserState as NetSparkleAppCastItem;

                        // show the updaze ui
                        ShowUpdateNeededUI(currentItem);

                        break;
                    }
                case 0:
                    {
                        ReportDiagnosticMessage(e.UserState.ToString());
                        break;
                    }
            }                        
        }
        
        private void InitDownloadAndInstallProcess(NetSparkleAppCastItem item)
        {
            NetSparkleDownloadProgress dlProgress = new NetSparkleDownloadProgress(this, item, _AppReferenceAssembly, ApplicationIcon, ApplicationWindowIcon);
            dlProgress.ShowDialog();
        }        

        private void ShowDiagnosticWindowIfNeeded()
        {
            // check the diagnotic value
            NetSparkleConfiguration config = new NetSparkleConfiguration(_AppReferenceAssembly);
            if (config.ShowDiagnosticWindow || ShowDiagnosticWindow)
            {
                Point newLocation = new Point();

                newLocation.X = Screen.PrimaryScreen.Bounds.Width - _DiagnosticWindow.Width;
                newLocation.Y = 0;

                _DiagnosticWindow.Location = newLocation;

                _DiagnosticWindow.Show();
            }
        }        
    }
}
