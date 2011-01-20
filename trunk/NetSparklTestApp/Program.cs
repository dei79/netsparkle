using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSparklTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hallo, this is version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
}
