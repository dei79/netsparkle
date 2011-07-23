using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using Microsoft.Win32;
namespace NetSparkleTestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // remove the netsparkle key from registry 
            Registry.CurrentUser.DeleteSubKeyTree("Software\\Microsoft\\NetSparkleTestApp");

            // set the lang of your choice
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-TW");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
