using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Windows.Forms;

namespace AppLimit.NetSparkle
{
    public delegate void UpdateCheckOperation(Object sender);
    
    public class Sparkle : IDisposable
    {
        /// <summary>
        /// The background worker
        /// </summary>
        private BackgroundWorker _worker;

        private String          _AppCastUrl;

        private EventWaitHandle _exitHandle;
        private EventWaitHandle _performUpdateHandle;
        
        public event UpdateCheckOperation updateCheckStarted;
        public event UpdateCheckOperation updateCheckFinished;
      
        /// <summary>
        /// The constructore starts NetSparkle
        /// If NetSparkle is configured to check for updates on startup, proceeds to perform 
        /// the check. You should only call this function when your app is initialized and 
        /// shows its main window.        
        /// </summary>
        /// <param name="appcastUrl">URL for the app's appcast.</param>
        /// /// <param name="noInitialCheck">check during start up phase</param>
        public Sparkle(String appcastUrl)
        {   
            // Start the helper thread as a background worker to 
            // get well ui interaction
            
            // set the url
            _AppCastUrl = appcastUrl;

            // create and configure the worker
            _worker = new BackgroundWorker();            
            _worker.WorkerReportsProgress = true;

            // adjust the delegates
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);

            // build the wait handle
            _exitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _performUpdateHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            
            // start the work
            _worker.RunWorkerAsync();
        }        
               
        /// <summary>
        /// Cleans up after NetSparkle.
        /// Should be called by the app when it's shutting down. Cancels any
        /// pending Sparkle operations and shuts down its helper threads.
        /// </summary>
        public void Dispose()
        {
            // ensure the work will finished
            _exitHandle.Set();
        }

        /// <summary>
        /// This method will perform an update check
        /// </summary>
        public void CheckForUpdates()
        {
            // set an update 
            _performUpdateHandle.Set();
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
         
            // start our lifecycles
            do
            {
                // read the config
                NetSparkleConfiguration config;
                config = new NetSparkleConfiguration();

                // check if it's ok the recheck to software state
                if (checkTSP)
                {
                    TimeSpan csp = DateTime.Now - config.LastCheckTime;
                    if (csp < tsp)
                        goto WaitSection;
                }
                else
                    checkTSP = true;

                // when sparkle will be deactivated wait an other cycle
                if (config.CheckForUpdate == false)
                    goto WaitSection; 

                // update the runonce feature
                goIntoLoop = !config.DidRunOnce;
                
                // notify
                if (updateCheckStarted != null)
                    updateCheckStarted(this);

                // init the appcast
                NetSparkleAppCast cast = new NetSparkleAppCast(_AppCastUrl, config);

                // check if any updates are available
                NetSparkleAppCastItem latestVersion = cast.GetLatestVersion();
                if (latestVersion == null)
                    goto WaitSection; 

                // set the last check time
                config.TouchCheckTime();

                // notify
                if (updateCheckFinished != null)
                    updateCheckFinished(this);

                // check if the available update has to be skipped
                if (latestVersion.Version.Equals(config.SkipThisVersion))
                    goto WaitSection; 

                // check if the version will be other then the installed version
                if (latestVersion.Version.Equals(config.InstalledVersion))
                    goto WaitSection; 

                // show the update windows                
                _worker.ReportProgress(1, latestVersion);

            WaitSection:

                // wait for
                if (!goIntoLoop)
                    break;
                else
                {
                    // build the event array
                    WaitHandle[] handles = new WaitHandle[2];
                    handles[0] = _exitHandle;
                    handles[1] = _performUpdateHandle;

                    // reset the update handel
                    _performUpdateHandle.Reset();

                    // wait for any
                    int i = WaitHandle.WaitAny(handles, tsp);
                    if (WaitHandle.WaitTimeout == i)
                        continue;

                    // check the exit hadnle
                    if (i == 0)
                        break;

                    // check an other check needed
                    if (i == 1)
                    {
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
        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {                
                NetSparkleAppCastItem currentItem = e.UserState as NetSparkleAppCastItem;

                NetSparkleForm frm = new NetSparkleForm(currentItem);
                frm.TopMost = true;
                DialogResult dlgResult = frm.ShowDialog();

                if (dlgResult == DialogResult.No)
                {
                    // skip this version
                    NetSparkleConfiguration config = new NetSparkleConfiguration();
                    config.SetVersionToSkip(currentItem.Version);            
                }
                else if (dlgResult == DialogResult.Yes)
                {
                    // download the binaries
                    InitDownloadAndInstallProcess(currentItem);                   
                }
            }            
        }

        void InitDownloadAndInstallProcess(NetSparkleAppCastItem item)
        {
            NetSparkleDownloadProgress dlProgress = new NetSparkleDownloadProgress(item);
            dlProgress.Show();
        }
    }
}
