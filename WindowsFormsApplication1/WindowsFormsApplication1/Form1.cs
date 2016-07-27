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
            populateServerDropdown();
            populateInstanceDropdown();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;

            // http://stackoverflow.com/questions/9718057/how-to-create-a-single-setup-exe-with-installshield-limited-edition
            // https://www.connectionstrings.com/sql-server-2012/
            //connectionString = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password";            
            //connectionString = "Server= "+ this.serverName + "\\"+ instances.FirstOrDefault() + "; Database= test; Integrated Security = SSPI; ";
            //connectionString = "Data Source=" + this.serverName + "\\" + instances.FirstOrDefault() + "; Initial Catalog= test; Integrated Security = SSPI; Connection Timeout=10;";
            //connectionString = "Server= " + this.serverName + "\\SQLEXPRESS; Database= test; Integrated Security = SSPI; ";
            //connectionString = "Data Source=DESKTOP-FVFO8GL\SQLEXPRESS;Initial Catalog=test;Integrated Security=SSPI;Connection Timeout=10;" //NT Authentication

            // http://stackoverflow.com/questions/15631602/how-to-set-sql-server-connection-string
            connectionString =
            "Data Source=" + this.serverName +
            ";Initial Catalog=test;" +
            "User id=test;" +
            "Password=test;";
            connectionString =
            "Data Source=" + this.serverName +
            ";Initial Catalog=test;" +
            "Integrated Security=SSPI;";
            //connectionString =
            //"Server=" + this.serverName + "\\" + this.instanceName + 
            //";Initial Catalog=test;" +
            //"User id=test;" +
            //"Password=test;";

            sql = "SELECT * FROM master.sys.all_parameters";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1));
                    return;
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
                MessageBox.Show(ex.ToString());
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
                        //MessageBox.Show(reader.Value);
                        doc.Add(new Paragraph(reader.Value));
                        break;
                    case XmlNodeType.EndElement:
                        //MessageBox.Show("");
                        doc.Add(new Paragraph(""));
                        break;
                }
            }

            doc.Close();

        }


        private void populateServerDropdown()
        {
            // https://msdn.microsoft.com/en-us/library/a6t1z9x2%28v=vs.110%29.aspx
            // Retrieve the enumerator instance and then the data.
            //var instance = SqlDataSourceEnumerator.Instance;
            //var serverTable = instance.GetDataSources();
            //var listOfServers = (from DataRow dr in serverTable.Rows select dr["ServerName"].ToString()).ToList();
            //var bindingSource1 = new BindingSource();
            //bindingSource1.DataSource = listOfServers;
            //this.comboBox1.DataSource = bindingSource1;

            

            while (this.comboBox1.Items.Count <= 0)
            {
                // http://stackoverflow.com/questions/10781334/how-to-get-list-of-available-sql-servers-using-c-sharp-code
                string myServer = Environment.MachineName;
                DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
                for (int i = 0; i < servers.Rows.Count; i++)
                {
                    if (myServer == servers.Rows[i]["ServerName"].ToString()) ///// used to get the servers in the local machine////
                    {
                        if ((servers.Rows[i]["InstanceName"] as string) != null)
                            this.comboBox1.Items.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                        else
                            this.comboBox1.Items.Add(servers.Rows[i]["ServerName"]);
                    }
                }
            }


        }

        private void populateInstanceDropdown()
        {
            //var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            //var rk = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server");
            //var instances = (String[])rk.GetValue("InstalledInstances");

            //if (instances == null)
            //{
            //    rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server");
            //    instances = (String[])rk.GetValue("InstalledInstances");
            //}
            //var bindingSource2 = new BindingSource();
            //bindingSource2.DataSource = instances;
            //this.comboBox2.DataSource = bindingSource2;
            //localMachine.Close();
            //rk.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;            
            this.serverName = (string)comboBox.SelectedItem;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            this.instanceName = (string)comboBox.SelectedItem;
        }

        private string serverName = "";
        private string instanceName = "";


    }
}
