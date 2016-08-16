namespace ClientApplication
{
    partial class SplashScreen_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Splash_ProgressBar = new System.Windows.Forms.ProgressBar();
            this.Logo_PictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Splash_ProgressBar
            // 
            this.Splash_ProgressBar.Location = new System.Drawing.Point(13, 91);
            this.Splash_ProgressBar.Name = "Splash_ProgressBar";
            this.Splash_ProgressBar.Size = new System.Drawing.Size(412, 23);
            this.Splash_ProgressBar.TabIndex = 0;
            // 
            // Logo_PictureBox
            // 
            this.Logo_PictureBox.Image = global::WindowsFormsApplication1.Properties.Resources.header_logo_alpha;
            this.Logo_PictureBox.Location = new System.Drawing.Point(140, 5);
            this.Logo_PictureBox.Name = "Logo_PictureBox";
            this.Logo_PictureBox.Size = new System.Drawing.Size(144, 84);
            this.Logo_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Logo_PictureBox.TabIndex = 2;
            this.Logo_PictureBox.TabStop = false;
            // 
            // SplashScreen_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 126);
            this.Controls.Add(this.Logo_PictureBox);
            this.Controls.Add(this.Splash_ProgressBar);
            this.Name = "SplashScreen_Form";
            this.Text = "Database Evaluator";
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar Splash_ProgressBar;
        private System.Windows.Forms.PictureBox Logo_PictureBox;
    }
}