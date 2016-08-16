using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClientApplication.Properties;

namespace ClientApplication
{
    public partial class SplashScreen_Form : Form
    {

        public SplashScreen_Form()
        {
            InitializeComponent();
            this.Splash_ProgressBar.Style = ProgressBarStyle.Marquee;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //this.Text = String.Empty;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //PictureBox spashPictureBox = new PictureBox();
            //spashPictureBox.Image = Resources.telerik_logo_RGB_photoshop;
            //spashPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            //spashPictureBox.Dock = DockStyle.Fill;
            //this.Controls.Add(spashPictureBox);
            //this.StartPosition = FormStartPosition.CenterScreen;
        }


    }
}
