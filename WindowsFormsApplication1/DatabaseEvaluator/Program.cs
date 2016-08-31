using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseEvaluator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // http://stackoverflow.com/questions/1207105/restrict-multiple-instances-of-an-application
            Process[] result = Process.GetProcessesByName("DatabaseEvaluator");
            if (result.Length > 1)
            {
                MessageBox.Show("There is already an instance running.", "Information");
                System.Environment.Exit(0);
                //Close();
                Application.Exit();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DatabaseEvaluatorMain_Form());
        }
    }
}
