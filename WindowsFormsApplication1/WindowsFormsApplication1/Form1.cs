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
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        // https://support.microsoft.com/en-nz/kb/307010
        //  Call this function to remove the key from memory after use for security
        [System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        // Function to Generate a 64 bits Key.
        private string GenerateKey()
        {
            // Create an instance of Symetric Algorithm. Key and IV is generated automatically.
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();

            // Use the Automatically generated key for Encryption. 
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }

        private void EncryptFile(string sInputFilename,
           string sOutputFilename,
           string sKey)
        {
            FileStream fsInput = new FileStream(sInputFilename,
               FileMode.Open,
               FileAccess.Read);

            FileStream fsEncrypted = new FileStream(sOutputFilename,
               FileMode.Create,
               FileAccess.Write);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
               desencrypt,
               CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();
        }

        private void DecryptFile(string sInputFilename,
           string sOutputFilename,
           string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            //A 64 bit key and IV is required for this provider.
            //Set secret key For DES algorithm.
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //Set initialization vector.
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            //Create a file stream to read the encrypted file back.
            FileStream fsread = new FileStream(sInputFilename,
               FileMode.Open,
               FileAccess.Read);
            //Create a DES decryptor from the DES instance.
            ICryptoTransform desdecrypt = DES.CreateDecryptor();
            //Create crypto stream set to read and do a 
            //DES decryption transform on incoming bytes.
            CryptoStream cryptostreamDecr = new CryptoStream(fsread,
               desdecrypt,
               CryptoStreamMode.Read);
            //Print the contents of the decrypted file.
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
        }

        /// The backgroundworker object on which the time consuming operation shall be executed
        /// http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners
        BackgroundWorker m_oWorker;

        public Form1()
        {
            InitializeComponent();
            populateServerDropdown();

            this.comboBox1.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            //Start the async operation here
            m_oWorker.RunWorkerAsync();

            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;

            // http://stackoverflow.com/questions/15631602/how-to-set-sql-server-connection-string
            connectionString =
            "Data Source=" + this.comboBox1.Text + ";" +
            "Initial Catalog=test;" +
            "User id=test;" +
            "Password=test;";
            connectionString =
            "Data Source=" + this.comboBox1.Text + ";" +
            //"Initial Catalog=test;" +
            "Integrated Security=SSPI;";
            //connectionString =
            //"Server=" + this.serverName + "\\" + this.instanceName + ";" +
            //"Initial Catalog=test;" +
            //"User id=test;" +
            //"Password=test;";

            // http://stackoverflow.com/questions/18654157/how-to-make-sql-query-result-to-xml-file
            //sql = "SELECT * FROM master.sys.all_parameters";
            sql = "SELECT * FROM master.sys.database_files";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    //MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1));
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }

            //MessageBox.Show("Current Working Directory: " + Directory.GetCurrentDirectory());

            //using (XmlWriter writer = XmlWriter.Create("SQLServer.xml"))
            //{
            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("Instance");
            //    writer.WriteStartElement("Database");
            //    writer.WriteElementString("Name", "test");
            //    writer.WriteEndElement();
            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}

            // http://csharp.net-informations.com/xml/xml-from-sql.htm
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();
            adapter = new SqlDataAdapter(sql, connection);
            adapter.Fill(ds);
            connection.Close();
            ds.WriteXml("SQLServer.xml");

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "SQLServer.xml";
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();
            ds.WriteXml(saveFileDialog1.FileName);

            // Must be 64 bits, 8 bytes.
            // Distribute this key to the user who will decrypt this file.
            string sSecretKey;

            // Get the Key for the file to Encrypt.
            sSecretKey = GenerateKey();

            // For additional security Pin the key.
            GCHandle gch = GCHandle.Alloc(sSecretKey, GCHandleType.Pinned);

            // Encrypt the file.        
            EncryptFile(@"SQLServer.xml",
               @"Encrypted_SQLServer.xml",
               sSecretKey);

            // Decrypt the file.
            DecryptFile(@"Encrypted_SQLServer.xml",
               @"Decrypted_SQLServer.xml",
               sSecretKey);

            // Remove the Key from memory. 
            ZeroMemory(gch.AddrOfPinnedObject(), sSecretKey.Length * 2);
            gch.Free();

            XmlTextReader reader = new XmlTextReader("Decrypted_SQLServer.xml"); //Combines the location of App_Data and the file name

            // http://www.codeproject.com/Articles/686994/Create-Read-Advance-PDF-Report-using-iTextSharp-in
            FileStream fs = new FileStream("SQLServer.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter pdfwriter = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            XDocument xml = XDocument.Load("Decrypted_SQLServer.xml");
            var numberOfRows = xml.Descendants("Table").Count();

            // http://www.c-sharpcorner.com/blogs/create-table-in-pdf-using-c-sharp-and-itextsharp          
            PdfPTable table = new PdfPTable(3);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        //MessageBox.Show("<" + reader.Name + ">");
                        //doc.Add(new Paragraph("<" + reader.Name + ">"));
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

            // http://stackoverflow.com/questions/10781334/how-to-get-list-of-available-sql-servers-using-c-sharp-code
            DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
            for (int i = 0; i < servers.Rows.Count; i++)
            {
                if ((servers.Rows[i]["InstanceName"] as string) != null)
                    this.comboBox1.Items.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                else
                    this.comboBox1.Items.Add(servers.Rows[i]["ServerName"]);
            }
        }

        private void populateDatabaseDropdown()
        {
            this.comboBox2.Items.Clear();
            // http://stackoverflow.com/questions/12862604/c-sharp-connect-to-database-and-list-the-databases
            var connectionString = "Data Source=" + this.comboBox1.Text + ";" +
            //"Initial Catalog=test;" +
            "Integrated Security=SSPI;";

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                DataTable databases = con.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    String databaseName = database.Field<String>("database_name");
                    this.comboBox2.Items.Add(databaseName);
                }
            }
        }

        /// On completed do the appropriate task
        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //If it was cancelled midway
            if (e.Cancelled)
            {
                //this.label2.Text = "Task Cancelled.";
            }
            else if (e.Error != null)
            {
                //this.label2.Text = "Error while performing background operation.";
            }
            else
            {
                //this.label2.Text = "Task Completed...";
            }
            this.button1.Enabled = true;
            //btnCancel.Enabled = false;
        }

        /// Notification is performed here to the progress bar
        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Here you play with the main UI thread
            //progressBar1.Value = e.ProgressPercentage;
            if (e.ProgressPercentage == 1)
            {
                this.pictureBox2.Image = global::WindowsFormsApplication1.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 10)
            {
                this.pictureBox3.Image = global::WindowsFormsApplication1.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 14)
            {
                this.pictureBox3.Image = global::WindowsFormsApplication1.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 15)
            {
                this.pictureBox4.Image = global::WindowsFormsApplication1.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 28)
            {
                this.pictureBox4.Image = global::WindowsFormsApplication1.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 29)
            {
                this.pictureBox5.Image = global::WindowsFormsApplication1.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 42)
            {
                this.pictureBox5.Image = global::WindowsFormsApplication1.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 43)
            {
                this.pictureBox2.Image = global::WindowsFormsApplication1.Properties.Resources.success;
            }

            //this.label2.Text = "Processing......";// + progressBar1.Value.ToString() + "%";
        }

        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //NOTE : Never play with the UI thread here...
            //time consuming operation
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(200);
                m_oWorker.ReportProgress(i);
                //If cancel button was pressed while the execution is in progress
                //Change the state from cancellation ---> cancel'ed
                if (m_oWorker.CancellationPending)
                {
                    e.Cancel = true;
                    m_oWorker.ReportProgress(0);
                    return;
                }
            }
            //Report 100% completion on operation completed
            m_oWorker.ReportProgress(100);
        }

        private void btnStartAsyncOperation_Click(object sender, EventArgs e)
        {
            //btnStartAsyncOperation.Enabled = false;
            //btnCancel.Enabled = true;
            //Start the async operation here
            m_oWorker.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (m_oWorker.IsBusy)
            {
                //Stop/Cancel the async operation here
                m_oWorker.CancelAsync();
            }
        }

        

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox2.Enabled)
            {
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
                comboBox2.Enabled = false;
                this.comboBox2.Text = "";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!comboBox2.Enabled)
            {
                this.comboBox2.Enabled = true;
                populateDatabaseDropdown();
                this.tableLayoutPanel2.Enabled = true;
            }
            else
            {
                this.comboBox2.Enabled = false;
                this.comboBox2.Items.Clear();
                this.tableLayoutPanel2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = null;
            SqlConnection connection;
            connectionString =
            "Data Source=" + this.comboBox1.Text + ";" +
            "Integrated Security=SSPI;";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                this.checkBox2.Enabled = true;
                populateDatabaseDropdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }

        }

        private void comboBox1_TextChanged(Object sender, EventArgs e)
        {
            if (checkBox2.Enabled)
            {
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
                comboBox2.Enabled = false;
                this.comboBox2.Text = "";
                this.tableLayoutPanel2.Enabled = false;
            }
        }





    }
}
