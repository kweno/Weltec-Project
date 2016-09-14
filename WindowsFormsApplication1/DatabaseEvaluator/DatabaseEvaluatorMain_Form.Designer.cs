using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Windows.Forms;
using System.Linq;

namespace DatabaseEvaluator
{
    partial class DatabaseEvaluatorMain_Form : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseEvaluatorMain_Form));
            this.Start_Button = new System.Windows.Forms.Button();
            this.Main_Panel = new System.Windows.Forms.Panel();
            this.Global_ProgressBar = new System.Windows.Forms.ProgressBar();
            this.FindFile_GroupBox = new System.Windows.Forms.GroupBox();
            this.PathToXML_TextBox = new System.Windows.Forms.TextBox();
            this.Browse_Button = new System.Windows.Forms.Button();
            this.PathToXML_Label = new System.Windows.Forms.Label();
            this.Logo_PictureBox = new System.Windows.Forms.PictureBox();
            this.Main_Panel.SuspendLayout();
            this.FindFile_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Start_Button
            // 
            this.Start_Button.Location = new System.Drawing.Point(404, 312);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(75, 23);
            this.Start_Button.TabIndex = 0;
            this.Start_Button.Text = "Start";
            this.Start_Button.UseVisualStyleBackColor = true;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // Main_Panel
            // 
            this.Main_Panel.Controls.Add(this.Global_ProgressBar);
            this.Main_Panel.Controls.Add(this.FindFile_GroupBox);
            this.Main_Panel.Controls.Add(this.Logo_PictureBox);
            this.Main_Panel.Location = new System.Drawing.Point(13, 13);
            this.Main_Panel.Name = "Main_Panel";
            this.Main_Panel.Size = new System.Drawing.Size(480, 295);
            this.Main_Panel.TabIndex = 1;
            // 
            // Global_ProgressBar
            // 
            this.Global_ProgressBar.Location = new System.Drawing.Point(16, 267);
            this.Global_ProgressBar.Name = "Global_ProgressBar";
            this.Global_ProgressBar.Size = new System.Drawing.Size(450, 21);
            this.Global_ProgressBar.TabIndex = 13;
            // 
            // FindFile_GroupBox
            // 
            this.FindFile_GroupBox.Controls.Add(this.PathToXML_TextBox);
            this.FindFile_GroupBox.Controls.Add(this.Browse_Button);
            this.FindFile_GroupBox.Controls.Add(this.PathToXML_Label);
            this.FindFile_GroupBox.Location = new System.Drawing.Point(16, 170);
            this.FindFile_GroupBox.Name = "FindFile_GroupBox";
            this.FindFile_GroupBox.Size = new System.Drawing.Size(450, 87);
            this.FindFile_GroupBox.TabIndex = 12;
            this.FindFile_GroupBox.TabStop = false;
            this.FindFile_GroupBox.Text = "Find File";
            // 
            // PathToXML_TextBox
            // 
            this.PathToXML_TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PathToXML_TextBox.Location = new System.Drawing.Point(104, 20);
            this.PathToXML_TextBox.Name = "PathToXML_TextBox";
            this.PathToXML_TextBox.ReadOnly = true;
            this.PathToXML_TextBox.Size = new System.Drawing.Size(328, 20);
            this.PathToXML_TextBox.TabIndex = 14;
            // 
            // Browse_Button
            // 
            this.Browse_Button.Location = new System.Drawing.Point(357, 48);
            this.Browse_Button.Name = "Browse_Button";
            this.Browse_Button.Size = new System.Drawing.Size(75, 23);
            this.Browse_Button.TabIndex = 13;
            this.Browse_Button.Text = "Browse";
            this.Browse_Button.UseVisualStyleBackColor = true;
            this.Browse_Button.Click += new System.EventHandler(this.Browse_Button_Click);
            // 
            // PathToXML_Label
            // 
            this.PathToXML_Label.AutoSize = true;
            this.PathToXML_Label.Location = new System.Drawing.Point(12, 23);
            this.PathToXML_Label.Name = "PathToXML_Label";
            this.PathToXML_Label.Size = new System.Drawing.Size(69, 13);
            this.PathToXML_Label.TabIndex = 12;
            this.PathToXML_Label.Text = "Path to XML:";
            // 
            // Logo_PictureBox
            // 
            this.Logo_PictureBox.Image = global::DatabaseEvaluator.Properties.Resources.header_logo_alpha1;
            this.Logo_PictureBox.Location = new System.Drawing.Point(120, 12);
            this.Logo_PictureBox.Name = "Logo_PictureBox";
            this.Logo_PictureBox.Size = new System.Drawing.Size(235, 140);
            this.Logo_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Logo_PictureBox.TabIndex = 0;
            this.Logo_PictureBox.TabStop = false;
            // 
            // DatabaseEvaluatorMain_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 347);
            this.Controls.Add(this.Main_Panel);
            this.Controls.Add(this.Start_Button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DatabaseEvaluatorMain_Form";
            this.Text = "Database Evaluator";
            this.Main_Panel.ResumeLayout(false);
            this.FindFile_GroupBox.ResumeLayout(false);
            this.FindFile_GroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Panel Main_Panel;
        private System.Windows.Forms.PictureBox Logo_PictureBox;
        private GroupBox FindFile_GroupBox;
        private Label PathToXML_Label;
        private Button Browse_Button;
        private TextBox PathToXML_TextBox;
        private ProgressBar Global_ProgressBar;
    }
}

