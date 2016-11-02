using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Windows.Forms;
using System.Linq;
using System;

namespace ClientApplication
{
    partial class ClientApplicationMain_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientApplicationMain_Form));
            this.Start_Button = new System.Windows.Forms.Button();
            this.Main_Panel = new System.Windows.Forms.Panel();
            this.Global_ProgressBar = new System.Windows.Forms.ProgressBar();
            this.Selection_GroupBox = new System.Windows.Forms.GroupBox();
            this.ConnectStatus_PictureBox = new System.Windows.Forms.PictureBox();
            this.ServerName_Label = new System.Windows.Forms.Label();
            this.DatabaseName_CheckBox = new System.Windows.Forms.CheckBox();
            this.Server_ComboBox = new System.Windows.Forms.ComboBox();
            this.Database_ComboBox = new System.Windows.Forms.ComboBox();
            this.Logo_PictureBox = new System.Windows.Forms.PictureBox();
            this.Progress_GroupBox = new System.Windows.Forms.GroupBox();
            this.Database_TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.DatabaseProgress_Label4 = new System.Windows.Forms.Label();
            this.DatabaseProgress_PictureBox4 = new System.Windows.Forms.PictureBox();
            this.DatabaseProgress_Label3 = new System.Windows.Forms.Label();
            this.DatabaseProgress_PictureBox3 = new System.Windows.Forms.PictureBox();
            this.DatabaseProgress_Label2 = new System.Windows.Forms.Label();
            this.DatabaseProgress_PictureBox2 = new System.Windows.Forms.PictureBox();
            this.DatabaseProgress_Label1 = new System.Windows.Forms.Label();
            this.DatabaseProgress_PictureBox1 = new System.Windows.Forms.PictureBox();
            this.DatabaseMainProgress_Label = new System.Windows.Forms.Label();
            this.DatabaseMainProgress_PictureBox = new System.Windows.Forms.PictureBox();
            this.Instance_TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.InstanceProgress_Label3 = new System.Windows.Forms.Label();
            this.InstanceProgress_PictureBox3 = new System.Windows.Forms.PictureBox();
            this.InstanceProgress_Label2 = new System.Windows.Forms.Label();
            this.InstanceProgress_PictureBox2 = new System.Windows.Forms.PictureBox();
            this.InstanceMainProgress_PictureBox = new System.Windows.Forms.PictureBox();
            this.InstanceMainProgress_Label = new System.Windows.Forms.Label();
            this.InstanceProgress_PictureBox1 = new System.Windows.Forms.PictureBox();
            this.InstanceProgress_Label1 = new System.Windows.Forms.Label();
            this.Close_Button = new System.Windows.Forms.Button();
            this.Main_Panel.SuspendLayout();
            this.Selection_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectStatus_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).BeginInit();
            this.Progress_GroupBox.SuspendLayout();
            this.Database_TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseMainProgress_PictureBox)).BeginInit();
            this.Instance_TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceProgress_PictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceProgress_PictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceMainProgress_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceProgress_PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Start_Button
            // 
            this.Start_Button.Enabled = false;
            this.Start_Button.Location = new System.Drawing.Point(323, 543);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(75, 23);
            this.Start_Button.TabIndex = 0;
            this.Start_Button.Text = "Start";
            this.Start_Button.UseVisualStyleBackColor = true;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // Main_Panel
            // 
            this.Main_Panel.Controls.Add(this.Selection_GroupBox);
            this.Main_Panel.Controls.Add(this.Logo_PictureBox);
            this.Main_Panel.Controls.Add(this.Progress_GroupBox);
            this.Main_Panel.Location = new System.Drawing.Point(13, 13);
            this.Main_Panel.Name = "Main_Panel";
            this.Main_Panel.Size = new System.Drawing.Size(480, 524);
            this.Main_Panel.TabIndex = 1;
            // 
            // Global_ProgressBar
            // 
            this.Global_ProgressBar.Location = new System.Drawing.Point(214, 45);
            this.Global_ProgressBar.Name = "Global_ProgressBar";
            this.Global_ProgressBar.Size = new System.Drawing.Size(202, 5);
            this.Global_ProgressBar.TabIndex = 2;
            // 
            // Selection_GroupBox
            // 
            this.Selection_GroupBox.Controls.Add(this.Global_ProgressBar);
            this.Selection_GroupBox.Controls.Add(this.ConnectStatus_PictureBox);
            this.Selection_GroupBox.Controls.Add(this.ServerName_Label);
            this.Selection_GroupBox.Controls.Add(this.DatabaseName_CheckBox);
            this.Selection_GroupBox.Controls.Add(this.Server_ComboBox);
            this.Selection_GroupBox.Controls.Add(this.Database_ComboBox);
            this.Selection_GroupBox.Location = new System.Drawing.Point(16, 170);
            this.Selection_GroupBox.Name = "Selection_GroupBox";
            this.Selection_GroupBox.Size = new System.Drawing.Size(450, 94);
            this.Selection_GroupBox.TabIndex = 12;
            this.Selection_GroupBox.TabStop = false;
            this.Selection_GroupBox.Text = "Selection";
            // 
            // ConnectStatus_PictureBox
            // 
            this.ConnectStatus_PictureBox.Location = new System.Drawing.Point(422, 21);
            this.ConnectStatus_PictureBox.Name = "ConnectStatus_PictureBox";
            this.ConnectStatus_PictureBox.Size = new System.Drawing.Size(17, 17);
            this.ConnectStatus_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ConnectStatus_PictureBox.TabIndex = 14;
            this.ConnectStatus_PictureBox.TabStop = false;
            // 
            // ServerName_Label
            // 
            this.ServerName_Label.AutoSize = true;
            this.ServerName_Label.Location = new System.Drawing.Point(12, 23);
            this.ServerName_Label.Name = "ServerName_Label";
            this.ServerName_Label.Size = new System.Drawing.Size(72, 13);
            this.ServerName_Label.TabIndex = 12;
            this.ServerName_Label.Text = "Server Name:";
            // 
            // DatabaseName_CheckBox
            // 
            this.DatabaseName_CheckBox.AutoSize = true;
            this.DatabaseName_CheckBox.Enabled = false;
            this.DatabaseName_CheckBox.Location = new System.Drawing.Point(15, 61);
            this.DatabaseName_CheckBox.Name = "DatabaseName_CheckBox";
            this.DatabaseName_CheckBox.Size = new System.Drawing.Size(106, 17);
            this.DatabaseName_CheckBox.TabIndex = 11;
            this.DatabaseName_CheckBox.Text = "Database Name:";
            this.DatabaseName_CheckBox.UseVisualStyleBackColor = true;
            this.DatabaseName_CheckBox.CheckedChanged += new System.EventHandler(this.DatabaseName_CheckedChanged);
            // 
            // Server_ComboBox
            // 
            this.Server_ComboBox.FormattingEnabled = true;
            this.Server_ComboBox.Location = new System.Drawing.Point(214, 20);
            this.Server_ComboBox.Name = "Server_ComboBox";
            this.Server_ComboBox.Size = new System.Drawing.Size(202, 21);
            this.Server_ComboBox.TabIndex = 2;
            this.Server_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Server_ComboBox_SelectedIndexChanged);
            this.Server_ComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Server_ComboBox_KeyPress);
            // 
            // Database_ComboBox
            // 
            this.Database_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Database_ComboBox.Enabled = false;
            this.Database_ComboBox.FormattingEnabled = true;
            this.Database_ComboBox.Location = new System.Drawing.Point(214, 59);
            this.Database_ComboBox.Name = "Database_ComboBox";
            this.Database_ComboBox.Size = new System.Drawing.Size(202, 21);
            this.Database_ComboBox.TabIndex = 9;
            this.Database_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Database_ComboBox_SelectedIndexChanged);
            // 
            // Logo_PictureBox
            // 
            this.Logo_PictureBox.Image = global::ClientApplication.Properties.Resources.header_logo_alpha1;
            this.Logo_PictureBox.Location = new System.Drawing.Point(120, 12);
            this.Logo_PictureBox.Name = "Logo_PictureBox";
            this.Logo_PictureBox.Size = new System.Drawing.Size(235, 140);
            this.Logo_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Logo_PictureBox.TabIndex = 0;
            this.Logo_PictureBox.TabStop = false;
            // 
            // Progress_GroupBox
            // 
            this.Progress_GroupBox.Controls.Add(this.Database_TableLayoutPanel);
            this.Progress_GroupBox.Controls.Add(this.Instance_TableLayoutPanel);
            this.Progress_GroupBox.Location = new System.Drawing.Point(16, 270);
            this.Progress_GroupBox.Name = "Progress_GroupBox";
            this.Progress_GroupBox.Size = new System.Drawing.Size(450, 244);
            this.Progress_GroupBox.TabIndex = 7;
            this.Progress_GroupBox.TabStop = false;
            this.Progress_GroupBox.Text = "Progress";
            // 
            // Database_TableLayoutPanel
            // 
            this.Database_TableLayoutPanel.BackColor = System.Drawing.Color.White;
            this.Database_TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.Database_TableLayoutPanel.ColumnCount = 2;
            this.Database_TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.755396F));
            this.Database_TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 94.24461F));
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_Label4, 1, 4);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_PictureBox4, 0, 4);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_Label3, 1, 3);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_PictureBox3, 0, 3);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_Label2, 1, 2);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_PictureBox2, 0, 2);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_Label1, 1, 1);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseProgress_PictureBox1, 0, 1);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseMainProgress_Label, 1, 0);
            this.Database_TableLayoutPanel.Controls.Add(this.DatabaseMainProgress_PictureBox, 0, 0);
            this.Database_TableLayoutPanel.Enabled = false;
            this.Database_TableLayoutPanel.Location = new System.Drawing.Point(15, 119);
            this.Database_TableLayoutPanel.Name = "Database_TableLayoutPanel";
            this.Database_TableLayoutPanel.RowCount = 5;
            this.Database_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Database_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Database_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.Database_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.Database_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.Database_TableLayoutPanel.Size = new System.Drawing.Size(418, 107);
            this.Database_TableLayoutPanel.TabIndex = 0;
            // 
            // DatabaseProgress_Label4
            // 
            this.DatabaseProgress_Label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DatabaseProgress_Label4.AutoSize = true;
            this.DatabaseProgress_Label4.Location = new System.Drawing.Point(28, 89);
            this.DatabaseProgress_Label4.Name = "DatabaseProgress_Label4";
            this.DatabaseProgress_Label4.Size = new System.Drawing.Size(63, 13);
            this.DatabaseProgress_Label4.TabIndex = 19;
            this.DatabaseProgress_Label4.Text = "   • Security";
            this.DatabaseProgress_Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatabaseProgress_PictureBox4
            // 
            this.DatabaseProgress_PictureBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabaseProgress_PictureBox4.Location = new System.Drawing.Point(4, 88);
            this.DatabaseProgress_PictureBox4.Name = "DatabaseProgress_PictureBox4";
            this.DatabaseProgress_PictureBox4.Size = new System.Drawing.Size(17, 15);
            this.DatabaseProgress_PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DatabaseProgress_PictureBox4.TabIndex = 18;
            this.DatabaseProgress_PictureBox4.TabStop = false;
            // 
            // DatabaseProgress_Label3
            // 
            this.DatabaseProgress_Label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DatabaseProgress_Label3.AutoSize = true;
            this.DatabaseProgress_Label3.Location = new System.Drawing.Point(28, 68);
            this.DatabaseProgress_Label3.Name = "DatabaseProgress_Label3";
            this.DatabaseProgress_Label3.Size = new System.Drawing.Size(87, 13);
            this.DatabaseProgress_Label3.TabIndex = 17;
            this.DatabaseProgress_Label3.Text = "   • Maintenance";
            this.DatabaseProgress_Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatabaseProgress_PictureBox3
            // 
            this.DatabaseProgress_PictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabaseProgress_PictureBox3.Location = new System.Drawing.Point(4, 68);
            this.DatabaseProgress_PictureBox3.Name = "DatabaseProgress_PictureBox3";
            this.DatabaseProgress_PictureBox3.Size = new System.Drawing.Size(17, 13);
            this.DatabaseProgress_PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DatabaseProgress_PictureBox3.TabIndex = 16;
            this.DatabaseProgress_PictureBox3.TabStop = false;
            // 
            // DatabaseProgress_Label2
            // 
            this.DatabaseProgress_Label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DatabaseProgress_Label2.AutoSize = true;
            this.DatabaseProgress_Label2.Location = new System.Drawing.Point(28, 47);
            this.DatabaseProgress_Label2.Name = "DatabaseProgress_Label2";
            this.DatabaseProgress_Label2.Size = new System.Drawing.Size(175, 13);
            this.DatabaseProgress_Label2.TabIndex = 15;
            this.DatabaseProgress_Label2.Text = "   • Database Configuration Options";
            this.DatabaseProgress_Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatabaseProgress_PictureBox2
            // 
            this.DatabaseProgress_PictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabaseProgress_PictureBox2.Location = new System.Drawing.Point(4, 46);
            this.DatabaseProgress_PictureBox2.Name = "DatabaseProgress_PictureBox2";
            this.DatabaseProgress_PictureBox2.Size = new System.Drawing.Size(17, 15);
            this.DatabaseProgress_PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DatabaseProgress_PictureBox2.TabIndex = 14;
            this.DatabaseProgress_PictureBox2.TabStop = false;
            // 
            // DatabaseProgress_Label1
            // 
            this.DatabaseProgress_Label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DatabaseProgress_Label1.AutoSize = true;
            this.DatabaseProgress_Label1.Location = new System.Drawing.Point(28, 25);
            this.DatabaseProgress_Label1.Name = "DatabaseProgress_Label1";
            this.DatabaseProgress_Label1.Size = new System.Drawing.Size(157, 13);
            this.DatabaseProgress_Label1.TabIndex = 13;
            this.DatabaseProgress_Label1.Text = "   • Implementation of Database";
            this.DatabaseProgress_Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatabaseProgress_PictureBox1
            // 
            this.DatabaseProgress_PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabaseProgress_PictureBox1.Location = new System.Drawing.Point(4, 25);
            this.DatabaseProgress_PictureBox1.Name = "DatabaseProgress_PictureBox1";
            this.DatabaseProgress_PictureBox1.Size = new System.Drawing.Size(17, 14);
            this.DatabaseProgress_PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DatabaseProgress_PictureBox1.TabIndex = 12;
            this.DatabaseProgress_PictureBox1.TabStop = false;
            // 
            // DatabaseMainProgress_Label
            // 
            this.DatabaseMainProgress_Label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DatabaseMainProgress_Label.AutoSize = true;
            this.DatabaseMainProgress_Label.Location = new System.Drawing.Point(28, 4);
            this.DatabaseMainProgress_Label.Name = "DatabaseMainProgress_Label";
            this.DatabaseMainProgress_Label.Size = new System.Drawing.Size(111, 13);
            this.DatabaseMainProgress_Label.TabIndex = 7;
            this.DatabaseMainProgress_Label.Text = "SQL Server Database";
            this.DatabaseMainProgress_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatabaseMainProgress_PictureBox
            // 
            this.DatabaseMainProgress_PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabaseMainProgress_PictureBox.Location = new System.Drawing.Point(4, 4);
            this.DatabaseMainProgress_PictureBox.Name = "DatabaseMainProgress_PictureBox";
            this.DatabaseMainProgress_PictureBox.Size = new System.Drawing.Size(17, 14);
            this.DatabaseMainProgress_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DatabaseMainProgress_PictureBox.TabIndex = 11;
            this.DatabaseMainProgress_PictureBox.TabStop = false;
            // 
            // Instance_TableLayoutPanel
            // 
            this.Instance_TableLayoutPanel.BackColor = System.Drawing.Color.White;
            this.Instance_TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.Instance_TableLayoutPanel.ColumnCount = 2;
            this.Instance_TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.755396F));
            this.Instance_TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 94.24461F));
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceProgress_Label3, 1, 3);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceProgress_PictureBox3, 0, 3);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceProgress_Label2, 1, 2);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceProgress_PictureBox2, 0, 2);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceMainProgress_PictureBox, 0, 0);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceMainProgress_Label, 1, 0);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceProgress_PictureBox1, 0, 1);
            this.Instance_TableLayoutPanel.Controls.Add(this.InstanceProgress_Label1, 1, 1);
            this.Instance_TableLayoutPanel.Location = new System.Drawing.Point(15, 19);
            this.Instance_TableLayoutPanel.Name = "Instance_TableLayoutPanel";
            this.Instance_TableLayoutPanel.RowCount = 4;
            this.Instance_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.17391F));
            this.Instance_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.82609F));
            this.Instance_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.Instance_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Instance_TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Instance_TableLayoutPanel.Size = new System.Drawing.Size(418, 91);
            this.Instance_TableLayoutPanel.TabIndex = 0;
            // 
            // InstanceProgress_Label3
            // 
            this.InstanceProgress_Label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.InstanceProgress_Label3.AutoSize = true;
            this.InstanceProgress_Label3.Location = new System.Drawing.Point(28, 73);
            this.InstanceProgress_Label3.Name = "InstanceProgress_Label3";
            this.InstanceProgress_Label3.Size = new System.Drawing.Size(63, 13);
            this.InstanceProgress_Label3.TabIndex = 10;
            this.InstanceProgress_Label3.Text = "   • Security";
            this.InstanceProgress_Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InstanceProgress_PictureBox3
            // 
            this.InstanceProgress_PictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstanceProgress_PictureBox3.Location = new System.Drawing.Point(4, 72);
            this.InstanceProgress_PictureBox3.Name = "InstanceProgress_PictureBox3";
            this.InstanceProgress_PictureBox3.Size = new System.Drawing.Size(17, 15);
            this.InstanceProgress_PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InstanceProgress_PictureBox3.TabIndex = 10;
            this.InstanceProgress_PictureBox3.TabStop = false;
            // 
            // InstanceProgress_Label2
            // 
            this.InstanceProgress_Label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.InstanceProgress_Label2.AutoSize = true;
            this.InstanceProgress_Label2.Location = new System.Drawing.Point(28, 51);
            this.InstanceProgress_Label2.Name = "InstanceProgress_Label2";
            this.InstanceProgress_Label2.Size = new System.Drawing.Size(87, 13);
            this.InstanceProgress_Label2.TabIndex = 9;
            this.InstanceProgress_Label2.Text = "   • Configuration";
            this.InstanceProgress_Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InstanceProgress_PictureBox2
            // 
            this.InstanceProgress_PictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstanceProgress_PictureBox2.Location = new System.Drawing.Point(4, 50);
            this.InstanceProgress_PictureBox2.Name = "InstanceProgress_PictureBox2";
            this.InstanceProgress_PictureBox2.Size = new System.Drawing.Size(17, 15);
            this.InstanceProgress_PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InstanceProgress_PictureBox2.TabIndex = 9;
            this.InstanceProgress_PictureBox2.TabStop = false;
            // 
            // InstanceMainProgress_PictureBox
            // 
            this.InstanceMainProgress_PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstanceMainProgress_PictureBox.Location = new System.Drawing.Point(4, 4);
            this.InstanceMainProgress_PictureBox.Name = "InstanceMainProgress_PictureBox";
            this.InstanceMainProgress_PictureBox.Size = new System.Drawing.Size(17, 17);
            this.InstanceMainProgress_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InstanceMainProgress_PictureBox.TabIndex = 0;
            this.InstanceMainProgress_PictureBox.TabStop = false;
            // 
            // InstanceMainProgress_Label
            // 
            this.InstanceMainProgress_Label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.InstanceMainProgress_Label.AutoSize = true;
            this.InstanceMainProgress_Label.Location = new System.Drawing.Point(28, 6);
            this.InstanceMainProgress_Label.Name = "InstanceMainProgress_Label";
            this.InstanceMainProgress_Label.Size = new System.Drawing.Size(72, 13);
            this.InstanceMainProgress_Label.TabIndex = 6;
            this.InstanceMainProgress_Label.Text = "SQL Instance";
            this.InstanceMainProgress_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InstanceProgress_PictureBox1
            // 
            this.InstanceProgress_PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstanceProgress_PictureBox1.Location = new System.Drawing.Point(4, 28);
            this.InstanceProgress_PictureBox1.Name = "InstanceProgress_PictureBox1";
            this.InstanceProgress_PictureBox1.Size = new System.Drawing.Size(17, 15);
            this.InstanceProgress_PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InstanceProgress_PictureBox1.TabIndex = 7;
            this.InstanceProgress_PictureBox1.TabStop = false;
            // 
            // InstanceProgress_Label1
            // 
            this.InstanceProgress_Label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.InstanceProgress_Label1.AutoSize = true;
            this.InstanceProgress_Label1.Location = new System.Drawing.Point(28, 29);
            this.InstanceProgress_Label1.Name = "InstanceProgress_Label1";
            this.InstanceProgress_Label1.Size = new System.Drawing.Size(75, 13);
            this.InstanceProgress_Label1.TabIndex = 8;
            this.InstanceProgress_Label1.Text = "   • Installation";
            this.InstanceProgress_Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Close_Button
            // 
            this.Close_Button.Location = new System.Drawing.Point(404, 543);
            this.Close_Button.Name = "Close_Button";
            this.Close_Button.Size = new System.Drawing.Size(75, 23);
            this.Close_Button.TabIndex = 2;
            this.Close_Button.Text = "Close";
            this.Close_Button.UseVisualStyleBackColor = true;
            this.Close_Button.Click += new System.EventHandler(this.Close_Button_Click);
            // 
            // ClientApplicationMain_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 581);
            this.Controls.Add(this.Close_Button);
            this.Controls.Add(this.Main_Panel);
            this.Controls.Add(this.Start_Button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientApplicationMain_Form";
            this.Text = "Database Evaluator";
            this.Main_Panel.ResumeLayout(false);
            this.Selection_GroupBox.ResumeLayout(false);
            this.Selection_GroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectStatus_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Logo_PictureBox)).EndInit();
            this.Progress_GroupBox.ResumeLayout(false);
            this.Database_TableLayoutPanel.ResumeLayout(false);
            this.Database_TableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseProgress_PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseMainProgress_PictureBox)).EndInit();
            this.Instance_TableLayoutPanel.ResumeLayout(false);
            this.Instance_TableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceProgress_PictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceProgress_PictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceMainProgress_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceProgress_PictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Panel Main_Panel;
        private System.Windows.Forms.ComboBox Server_ComboBox;
        private System.Windows.Forms.PictureBox Logo_PictureBox;
        private GroupBox Progress_GroupBox;
        private PictureBox InstanceMainProgress_PictureBox;
        private Label InstanceMainProgress_Label;
        private PictureBox InstanceProgress_PictureBox1;
        private Label InstanceProgress_Label1;
        private Label InstanceProgress_Label3;
        private PictureBox InstanceProgress_PictureBox3;
        private Label InstanceProgress_Label2;
        private PictureBox InstanceProgress_PictureBox2;
        private ComboBox Database_ComboBox;
        private GroupBox Selection_GroupBox;
        private CheckBox DatabaseName_CheckBox;
        private Label ServerName_Label;
        private TableLayoutPanel Database_TableLayoutPanel;
        private Label DatabaseProgress_Label4;
        private PictureBox DatabaseProgress_PictureBox4;
        private Label DatabaseProgress_Label3;
        private PictureBox DatabaseProgress_PictureBox3;
        private Label DatabaseProgress_Label2;
        private PictureBox DatabaseProgress_PictureBox2;
        private Label DatabaseProgress_Label1;
        private PictureBox DatabaseProgress_PictureBox1;
        private Label DatabaseMainProgress_Label;
        private PictureBox DatabaseMainProgress_PictureBox;
        private TableLayoutPanel Instance_TableLayoutPanel;
        private PictureBox ConnectStatus_PictureBox;
        private ProgressBar Global_ProgressBar;
        private Button Close_Button;
    }
}

