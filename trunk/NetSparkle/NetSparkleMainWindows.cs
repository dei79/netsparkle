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
    public partial class NetSparkleMainWindows : Form
    {
        public NetSparkleMainWindows()
        {
            InitializeComponent();
        }

        public void Report(String message)
        {
            DateTime c = DateTime.Now;

            lstActions.Items.Add("[" + c.ToLongTimeString() +"." + c.Millisecond + "] " + message);
        }
    }
}
