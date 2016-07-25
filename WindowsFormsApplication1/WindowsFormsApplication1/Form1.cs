using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Data.Sql;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            populateDropdown();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var rk = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server");
            var instances = (String[])rk.GetValue("InstalledInstances");            

            if(instances == null)
            {
                rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server");
                instances = (String[])rk.GetValue("InstalledInstances");
            }

            foreach (string s in instances)
            {
                MessageBox.Show(s);
            }

            //RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            //RegistryKey key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");

            //foreach (string s in key.GetValueNames())
            //{
            //    MessageBox.Show(s);
            //}

            //key.Close();
            //baseKey.Close();

            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;
            //connectionString = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password";
            // references http://stackoverflow.com/questions/9718057/how-to-create-a-single-setup-exe-with-installshield-limited-edition
            connectionString = "Server= "+ this.serverName + "\\"+ instances.FirstOrDefault() + "; Database= test; Integrated Security = SSPI; ";
            //connectionString = "Server= " + this.serverName + "\\SQLEXPRESS; Database= test; Integrated Security = SSPI; ";
            //connectionString = "Data Source=DESKTOP-FVFO8GL\SQLEXPRESS;Initial Catalog=test;Integrated Security=SSPI;Connection Timeout=10;" //NT Authentication
            sql = "SELECT * FROM table1";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1));
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }

            MessageBox.Show("Current Working Directory: " + Directory.GetCurrentDirectory());

            using (XmlWriter writer = XmlWriter.Create("SQLServer.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Instance");


                writer.WriteStartElement("Database");

                writer.WriteElementString("Name", "test");

                writer.WriteEndElement();


                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            XmlTextReader reader = new XmlTextReader("SQLServer.xml"); //Combines the location of App_Data and the file name

            // http://www.codeproject.com/Articles/686994/Create-Read-Advance-PDF-Report-using-iTextSharp-in
            FileStream fs = new FileStream("SQLServer.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter pdfwriter = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        MessageBox.Show("<" + reader.Name + ">");
                        doc.Add(new Paragraph("<" + reader.Name + ">"));
                        break;
                    case XmlNodeType.Text:
                        MessageBox.Show(reader.Value);
                        doc.Add(new Paragraph(reader.Value));
                        break;
                    case XmlNodeType.EndElement:
                        MessageBox.Show("");
                        doc.Add(new Paragraph(""));
                        break;
                }
            }

            doc.Close();

        }


        private void populateDropdown()
        {
            // https://msdn.microsoft.com/en-us/library/a6t1z9x2%28v=vs.110%29.aspx
            // Retrieve the enumerator instance and then the data.
            var instance = SqlDataSourceEnumerator.Instance;
            var serverTable = instance.GetDataSources();
            var listOfServers = from DataRow dr in serverTable.Rows select dr["ServerName"];
            var bindingSource1 = new BindingSource();
            bindingSource1.DataSource = listOfServers;
            this.comboBox1.DataSource = bindingSource1;
            this.ResumeLayout(false);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;            
            this.serverName = (string)comboBox.SelectedItem;
        }


        private string serverName = "";


    }
}
