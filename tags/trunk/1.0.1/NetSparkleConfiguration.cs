using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace AppLimit.NetSparkle
{
    /// <summary>
    /// This class handles all registry values which are used from sparkle to handle 
    /// update intervalls. All values are stored in HKCU\Software\Vendor\AppName which 
    /// will be read ot from the assembly information. All values are of the REG_SZ 
    /// type, no matter what their "logical" type is. The following options are
    /// available:
    /// 
    /// CheckForUpdate  - Boolean    - Whether NetSparkle should check for updates
    /// LastCheckTime   - time_t     - Time of last check
    /// SkipThisVersion - String     - If the user skipped an update, then the version to ignore is stored here (e.g. "1.4.3")
    /// DidRunOnce      - Boolean    - Check only one time when the app launched
    /// </summary>    
    public class NetSparkleConfiguration
    {
        public String   ApplicationName     { get; private set; }
        public String   InstalledVersion    { get; private set; }
        public Boolean  CheckForUpdate      { get; private set; }
        public DateTime LastCheckTime       { get; private set; }
        public String   SkipThisVersion     { get; private set; }
        public Boolean  DidRunOnce           { get; private set; }        

        /// <summary>
        /// The constructor reads out all configured values
        /// </summary>        
        public NetSparkleConfiguration()
        {
            try
            {
                // set some value from the binary
                NetSparkleAssemblyAccessor accessor = new NetSparkleAssemblyAccessor();
                ApplicationName     = accessor.AssemblyProduct;
                InstalledVersion    = accessor.AssemblyVersion;

                // set default values
                InitWithDefaultValues();

                // build the reg path
                String regPath = BuildRegistryPath();

                // load the values
                LoadValuesFromPath(regPath);                                
            }
            catch (Exception)
            {
                // disable update checks when exception was called 
                CheckForUpdate = false;
            }
        }

        /// <summary>
        /// Touches the check time to now, should be used after a check directly
        /// </summary>
        public void TouchCheckTime()
        {
            // set the check tiem
            LastCheckTime = DateTime.Now;

            // build path
            String path = BuildRegistryPath();

            // save the values
            SaveValuesToPath(path);
        }

        /// <summary>
        /// This method allows to skip a specific version
        /// </summary>
        /// <param name="version"></param>
        public void SetVersionToSkip(String version)
        {
            // set the check tiem
            SkipThisVersion = version;

            // build path
            String path = BuildRegistryPath();

            // save the values
            SaveValuesToPath(path);
        }

        /// <summary>
        /// This function build a valid registry path in dependecy to the 
        /// assembly information
        /// </summary>
        /// <returns></returns>
        private String BuildRegistryPath()
        {
            NetSparkleAssemblyAccessor accessor = new NetSparkleAssemblyAccessor();
            return "Software\\" + accessor.AssemblyCompany + "\\" + accessor.AssemblyProduct;
        }

        /// <summary>
        /// This method set's default values for the config
        /// </summary>
        private void InitWithDefaultValues()
        {
            CheckForUpdate = true;
            LastCheckTime = new DateTime(0);
            SkipThisVersion = String.Empty;
            DidRunOnce = false;
        }

        /// <summary>
        /// This method loads the values from registry
        /// </summary>
        /// <param name="regPath"></param>
        /// <returns></returns>
        private Boolean LoadValuesFromPath(String regPath)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath);
            if (key == null)
                return false;
            else
            {
                // read out
                String strCheckForUpdate = key.GetValue("CheckForUpdate", "1") as String;
                String strLastCheckTime = key.GetValue("LastCheckTime", new DateTime(0).ToString()) as String;
                String strSkipThisVersion = key.GetValue("SkipThisVersion", "") as String;
                String strDidRunOnc = key.GetValue("DidRunOnce", "0") as String;

                // convert th right datatypes
                CheckForUpdate = Convert.ToBoolean(strCheckForUpdate);
                LastCheckTime = Convert.ToDateTime(strLastCheckTime);
                SkipThisVersion = strSkipThisVersion;
                DidRunOnce = Convert.ToBoolean(strDidRunOnc);

                return true;
            }
        }

        /// <summary>
        /// This method store the information into registry
        /// </summary>
        /// <param name="regPath"></param>
        /// <returns></returns>
        private Boolean SaveValuesToPath(String regPath)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath);
            if (key == null)
                return false;
            else
            {
                // convert to regsz
                String strCheckForUpdate    = CheckForUpdate.ToString();
                String strLastCheckTime     = LastCheckTime.ToString();
                String strSkipThisVersion   = SkipThisVersion.ToString();
                String strDidRunOnc         = DidRunOnce.ToString();

                // set the values
                key.SetValue("CheckForUpdate", strCheckForUpdate, RegistryValueKind.String);
                key.SetValue("LastCheckTime", strLastCheckTime, RegistryValueKind.String);
                key.SetValue("SkipThisVersion", strSkipThisVersion, RegistryValueKind.String);
                key.SetValue("DidRunOnce", strDidRunOnc, RegistryValueKind.String);

                return true;
            }
        }

    }
}
