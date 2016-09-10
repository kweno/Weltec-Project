using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApplication
{
    static class Program
    {
        public static SplashScreen_Form splashForm = null;

        /// <summary>
        /// The main entry point for the application.
        /// http://www.telerik.com/support/kb/winforms/forms-and-dialogs/details/add-splashscreen-to-your-application
        /// </summary>
        [STAThread]
        static void Main()
        {
            // http://stackoverflow.com/questions/1207105/restrict-multiple-instances-of-an-application
            Process[] result = Process.GetProcessesByName("ClientApplication");
            if (result.Length > 1)
            {
                MessageBox.Show("There is already an instance running.", "Information");
                System.Environment.Exit(0);
                //Close();
                Application.Exit();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //show splash
            Thread splashThread = new Thread(new ThreadStart(
                delegate
                {
                    splashForm = new SplashScreen_Form();
                    Application.Run(splashForm);
                }
                ));

            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            //run form - time taking operation
            Form mainForm = new ClientApplicationMain_Form();
            mainForm.Load += new EventHandler(mainForm_Load);
            Application.Run(mainForm);
        }

        static void mainForm_Load(object sender, EventArgs e)
        {
            //close splash
            if (splashForm == null)
            {
                return;
            }

            splashForm.Invoke(new Action(splashForm.Close));
            splashForm.Dispose();
            splashForm = null;
        }




        
    }
}
