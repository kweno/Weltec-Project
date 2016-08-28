using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClientApplication.Properties;
using System.Runtime.InteropServices;

namespace ClientApplication
{
    public partial class SplashScreen_Form : Form
    {
        // http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void SplashScreen_Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public SplashScreen_Form()
        {
            InitializeComponent();
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SplashScreen_Form_MouseDown);
            this.Logo_PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SplashScreen_Form_MouseDown);
            this.Splash_ProgressBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SplashScreen_Form_MouseDown);
            this.Splash_ProgressBar.Style = ProgressBarStyle.Marquee;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Text = String.Empty;
            //PictureBox spashPictureBox = new PictureBox();
            //spashPictureBox.Image = Resources.telerik_logo_RGB_photoshop;
            //spashPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            //spashPictureBox.Dock = DockStyle.Fill;
            //this.Controls.Add(spashPictureBox);
            //this.StartPosition = FormStartPosition.CenterScreen;
        }


    }
}
