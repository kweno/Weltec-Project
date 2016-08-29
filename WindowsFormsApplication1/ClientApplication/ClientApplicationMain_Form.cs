using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Data.Sql;
using System.Threading;
using System.Security.Cryptography;

namespace ClientApplication
{
    public partial class ClientApplicationMain_Form : Form
    {
        private string EVALUATOR_KEY = "AAECAwQFBgcICQoLDA0ODw==";

        // https://support.microsoft.com/en-nz/kb/307010
        // https://dotnetfiddle.net/bFvxp8
        // http://stackoverflow.com/questions/2919228/specified-key-is-not-a-valid-size-for-this-algorithm
        private void EncryptFile(string sInputFilename,
           string sOutputFilename)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                FileStream fsInput = new FileStream(sInputFilename,
                FileMode.Open,
                FileAccess.Read);

                FileStream fsEncrypted = new FileStream(sOutputFilename,
                   FileMode.Create,
                   FileAccess.Write);

                aesAlg.Key = Convert.FromBase64String(EVALUATOR_KEY);
                aesAlg.IV = Convert.FromBase64String(EVALUATOR_KEY);
                // Create a decrytor to perform the stream transform.
                ICryptoTransform desencrypt = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
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
        }

        /// The backgroundworker object on which the time consuming operation shall be executed
        /// http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners
        BackgroundWorker ClientApplication_BackgroundWorker;

        public ClientApplicationMain_Form()
        {
            InitializeComponent();
            //populateServerDropdown();

            Server_ComboBox.TextChanged += new System.EventHandler(Server_ComboBox_TextChanged);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            ClientApplication_BackgroundWorker = new BackgroundWorker();
            ClientApplication_BackgroundWorker.DoWork += new DoWorkEventHandler(ClientApplication_BackgroundWorker_DoWork);
            ClientApplication_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ClientApplication_BackgroundWorker_ProgressChanged);
            ClientApplication_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ClientApplication_BackgroundWorker_RunWorkerCompleted);
            ClientApplication_BackgroundWorker.WorkerReportsProgress = true;
            ClientApplication_BackgroundWorker.WorkerSupportsCancellation = true;
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            Start_Button.Enabled = false;
            Server_ComboBox.Enabled = false;
            Connect_Button.Enabled = false;
            DatabaseName_CheckBox.Enabled = false;
            Database_ComboBox.Enabled = false;

            //Start the async operation here
            ClientApplication_BackgroundWorker.RunWorkerAsync();

            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;

            // http://stackoverflow.com/questions/15631602/how-to-set-sql-server-connection-string
            connectionString =
            "Data Source=" + Server_ComboBox.Text + ";" +
            "Initial Catalog=test;" +
            "User id=test;" +
            "Password=test;";
            connectionString =
            "Data Source=" + Server_ComboBox.Text + ";" +
            //"Initial Catalog=test;" +
            "Integrated Security=SSPI;";
            //connectionString =
            //"Server=" + serverName + "\\" + instanceName + ";" +
            //"Initial Catalog=test;" +
            //"User id=test;" +
            //"Password=test;";

            /*
            // http://stackoverflow.com/questions/18654157/how-to-make-sql-query-result-to-xml-file
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
            catch (Exception exception)
            {
                MessageBox.Show("Can not open connection ! ");
            }
            */

            sql =     "DECLARE @ExpressionToSearch VARCHAR(200)" 
                    + "DECLARE  @ExpressionToFind VARCHAR(200)" 
                    + "CREATE TABLE [dbo].[#TmpErrorLog] ([LogDate] DATETIME NULL, [ProcessInfo] VARCHAR(20) NULL, [Text] VARCHAR(MAX) NULL );" 
                    + "CREATE TABLE [dbo].[#TmpResults] ([Text] VARCHAR(MAX) NULL );" 
                    + "INSERT INTO #TmpErrorLog ([LogDate], [ProcessInfo], [Text]) EXEC [master].[dbo].[xp_readerrorlog] 0, 1, N'Server is listening on';" 
                    + "SET @ExpressionToFind = '1433'" 
                    + "SELECT @ExpressionToSearch = [Text] FROM #TmpErrorLog where text like '%any%' and text like '%<ipv4>%' and text like '%1433%' and ProcessInfo = 'Server'" 
                    + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%'" 
                    + "    INSERT INTO #TmpResults VALUES ('Yes, 1433 port is using by SQL Server');" 
                    + "ELSE" 
                    + "    INSERT INTO #TmpResults VALUES ('SQL Server doesn''t use default port');"
                    + "SELECT * FROM #TmpResults;";

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    MessageBox.Show(dataReader.GetValue(0)+"");
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception exception)
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

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "SQLServer.xml";
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();
            ds.WriteXml(saveFileDialog1.FileName);

            // Encrypt the file.        
            EncryptFile(@"SQLServer.xml",
               @"Encrypted_SQLServer.xml");
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
            //comboBox1.DataSource = bindingSource1;

            // http://stackoverflow.com/questions/10781334/how-to-get-list-of-available-sql-servers-using-c-sharp-code
            // https://msdn.microsoft.com/en-us/library/a6t1z9x2%28v=vs.110%29.aspx
            DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
            for (int i = 0; i < servers.Rows.Count; i++)
            {
                if ((servers.Rows[i]["InstanceName"] as string) != null)
                    Server_ComboBox.Items.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                else
                    Server_ComboBox.Items.Add(servers.Rows[i]["ServerName"]);
            }
        }

        private void populateDatabaseDropdown()
        {
            Database_ComboBox.Items.Clear();
            // http://stackoverflow.com/questions/12862604/c-sharp-connect-to-database-and-list-the-databases
            var connectionString = "Data Source=" + Server_ComboBox.Text + ";" +
            //"Initial Catalog=test;" +
            "Integrated Security=SSPI;";

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                DataTable databases = con.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    string databaseName = database.Field<String>("database_name");
                    Database_ComboBox.Items.Add(databaseName);
                }
            }
        }

        /// On completed do the appropriate task
        void ClientApplication_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //If it was cancelled midway
            if (e.Cancelled)
            {
                //label2.Text = "Task Cancelled.";
            }
            else if (e.Error != null)
            {
                //label2.Text = "Error while performing background operation.";
            }
            else
            {
                //label2.Text = "Task Completed...";
            }
            Start_Button.Enabled = true;
            Start_Button.Enabled = true;
            Server_ComboBox.Enabled = true;
            Connect_Button.Enabled = true;
            DatabaseName_CheckBox.Enabled = true;
            Database_ComboBox.Enabled = true;
            //btnCancel.Enabled = false;
        }

        /// Notification is performed here to the progress bar
        void ClientApplication_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Here you play with the main UI thread
            //progressBar1.Value = e.ProgressPercentage;
            if (e.ProgressPercentage == 1)
            {
                InstanceMainProgress_PictureBox.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 10)
            {
                InstanceProgress_PictureBox1.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 14)
            {
                InstanceProgress_PictureBox1.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 15)
            {
                InstanceProgress_PictureBox2.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 28)
            {
                InstanceProgress_PictureBox2.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 29)
            {
                InstanceProgress_PictureBox3.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 42)
            {
                InstanceProgress_PictureBox3.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 43)
            {
                InstanceMainProgress_PictureBox.Image = global::ClientApplication.Properties.Resources.success;
            }

            //label2.Text = "Processing......";// + progressBar1.Value.ToString() + "%";
        }

        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        void ClientApplication_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //NOTE : Never play with the UI thread here...
            //time consuming operation
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(200);
                ClientApplication_BackgroundWorker.ReportProgress(i);
                //If cancel button was pressed while the execution is in progress
                //Change the state from cancellation ---> cancel'ed
                if (ClientApplication_BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    ClientApplication_BackgroundWorker.ReportProgress(0);
                    return;
                }
            }
            //Report 100% completion on operation completed
            ClientApplication_BackgroundWorker.ReportProgress(100);
        }

        private void btnStartAsyncOperation_Click(object sender, EventArgs e)
        {
            //btnStartAsyncOperation.Enabled = false;
            //btnCancel.Enabled = true;
            //Start the async operation here
            ClientApplication_BackgroundWorker.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ClientApplication_BackgroundWorker.IsBusy)
            {
                //Stop/Cancel the async operation here
                ClientApplication_BackgroundWorker.CancelAsync();
            }
        }

        private void Database_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Server_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DatabaseName_CheckBox.Enabled)
            {
                DatabaseName_CheckBox.Checked = false;
                DatabaseName_CheckBox.Enabled = false;
                Database_ComboBox.Enabled = false;
                Database_ComboBox.Text = "";
            }
        }

        private void DatabaseName_CheckedChanged(object sender, EventArgs e)
        {
            if (DatabaseName_CheckBox.Checked)
            {
                Database_ComboBox.Enabled = true;
                populateDatabaseDropdown();
                Database_TableLayoutPanel.Enabled = true;
            }
            else
            {
                Database_ComboBox.Enabled = false;
                Database_ComboBox.Items.Clear();
                Database_TableLayoutPanel.Enabled = false;
            }
        }

        private void Connect_Button_Click(object sender, EventArgs e)
        {
            string connectionString = null;
            SqlConnection connection;
            connectionString =
            "Data Source=" + Server_ComboBox.Text + ";" +
            "Integrated Security=SSPI;";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                DatabaseName_CheckBox.Enabled = true;
                Start_Button.Enabled = true;
                populateDatabaseDropdown();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can not open connection ! ");
            }

        }

        private void Server_ComboBox_TextChanged(Object sender, EventArgs e)
        {
            if (DatabaseName_CheckBox.Enabled)
            {
                Start_Button.Enabled = false;
                DatabaseName_CheckBox.Checked = false;
                DatabaseName_CheckBox.Enabled = false;
                Database_ComboBox.Enabled = false;
                Database_ComboBox.Text = "";
                Database_TableLayoutPanel.Enabled = false;
            }
        }





    }
}
