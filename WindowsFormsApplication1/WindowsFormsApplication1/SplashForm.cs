using System;
using System.ComponentModel;
using System.Windows.Forms;
using WindowsFormsApplication1.Properties;

namespace WindowsFormsApplication1
{
    public partial class SplashForm : Form
    {
        private ProgressBar progressBar1;

        public SplashForm()
        {
            InitializeComponent();
            PictureBox spashPictureBox = new PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            spashPictureBox.Image = Resources.telerik_logo_RGB_photoshop;
            spashPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            spashPictureBox.Dock = DockStyle.Fill;
            this.Controls.Add(progressBar1);
            this.Controls.Add(spashPictureBox);
            this.StartPosition = FormStartPosition.CenterScreen;
        }


    }
}
