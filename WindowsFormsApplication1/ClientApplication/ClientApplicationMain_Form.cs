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
        private string SERVER = "";
        private bool SERVER_OK = false;

        /// The backgroundworker object on which the time consuming operation shall be executed
        /// http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners
        BackgroundWorker ClientApplication_BackgroundWorker;
        BackgroundWorker CheckServerConnection_BackgroundWorker;

        public ClientApplicationMain_Form()
        {
            InitializeComponent();
            //populateServerDropdown();

            Server_ComboBox.TextChanged += new System.EventHandler(Server_ComboBox_TextChanged);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

            ClientApplication_BackgroundWorker = new BackgroundWorker();
            ClientApplication_BackgroundWorker.DoWork += new DoWorkEventHandler(ClientApplication_BackgroundWorker_DoWork);
            ClientApplication_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ClientApplication_BackgroundWorker_ProgressChanged);
            ClientApplication_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ClientApplication_BackgroundWorker_RunWorkerCompleted);
            ClientApplication_BackgroundWorker.WorkerReportsProgress = true;
            ClientApplication_BackgroundWorker.WorkerSupportsCancellation = true;

            CheckServerConnection_BackgroundWorker = new BackgroundWorker();
            CheckServerConnection_BackgroundWorker.DoWork += new DoWorkEventHandler(CheckServerConnection_BackgroundWorker_DoWork);
            CheckServerConnection_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CheckServerConnectionn_BackgroundWorker_RunWorkerCompleted);
            CheckServerConnection_BackgroundWorker.WorkerReportsProgress = true;
            CheckServerConnection_BackgroundWorker.WorkerSupportsCancellation = true;
        }

        // https://dotnetfiddle.net/bFvxp8
        private byte[] EncryptStringToBytes_Aes(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Convert.FromBase64String(EVALUATOR_KEY); ;
                aesAlg.IV = Convert.FromBase64String(EVALUATOR_KEY); ;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            var parameterValues = "";
            var encryptedParameterValues = "";
            byte[] encrypted = null;

            Start_Button.Enabled = false;
            Server_ComboBox.Enabled = false;
            //Connect_Button.Enabled = false;
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

            sql = "USE [MASTER];" + "\n"
            + "CREATE TABLE [dbo].[#Values]" + "\n"
            + "( [ProcessInfo] VARCHAR(50) NULL," + "\n"
            + " [Text] VARCHAR(MAX) NULL) ;" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('HostName',HOST_NAME())" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('InstanceName',CONVERT(VARCHAR(MAX),SERVERPROPERTY('InstanceName')))" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('ProductLevel',CONVERT(VARCHAR(MAX),SERVERPROPERTY('ProductLevel')))" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('ProductVersion',CONVERT(VARCHAR(MAX),SERVERPROPERTY('ProductVersion')))" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text])" + "\n"
            + "    SELECT 'SQLVersion', SUBSTRING(@@VERSION, 1, CHARINDEX('-', @@VERSION) - 1)" + "\n"
            + "       + CONVERT(VARCHAR(100), SERVERPROPERTY('edition'))" + "\n"
            + "    AS 'Server Version';" + "\n"
            + "CREATE TABLE #MaxDOP (NAME VARCHAR(255), minimum INT, maximum INT, config_value INT, run_value INT)" + "\n"
            + "EXEC [master].[dbo].[sp_configure] 'show advanced options', 1;" + "\n"
            + "RECONFIGURE;" + "\n"
            + "INSERT INTO #MaxDOP" + "\n"
            + "EXEC sp_configure 'max degree of parallelism'" + "\n"
            + "EXEC sp_configure 'show advanced options', 0;" + "\n"
            + "RECONFIGURE;" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text]) SELECT 'Max Degree Of Parallelism',run_value FROM #MaxDOP WHERE name='max degree of parallelism'" + "\n"
            + "DROP TABLE #MaxDOP;" + "\n"
            + "INSERT INTO [#Values] ([ProcessInfo], [Text])" + "\n"
            + "SELECT [description], CONVERT(VARCHAR(50),value_in_use) FROM sys.configurations" + "\n"
            + "WHERE name like '%server memory%';" + "\n"
            + "SELECT * FROM [#Values];";

            connection = new SqlConnection(connectionString);

            // CAN REMOVE THIS PART, JUST FOR TESTING PURPOSES
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    MessageBox.Show(dataReader.GetValue(0) + "");
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can not open connection ! ");
            }
            //------------------------------------------------------

            // http://csharp.net-informations.com/xml/xml-from-sql.htm
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();
            adapter = new SqlDataAdapter(sql, connection);
            adapter.Fill(ds);
            connection.Close();

            // http://stackoverflow.com/questions/963870/dataset-writexml-to-string
            StringWriter sw = new StringWriter();
            ds.WriteXml(@"SQLServer.xml");
            ds.WriteXml(sw);
            string result = sw.ToString();

            try
            {
                // Encrypt the string to an array of bytes.
                encrypted = EncryptStringToBytes_Aes(result);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: {0}", exception.Message);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "SQLServer.dbe";
            saveFileDialog1.Filter = "DBE File|*.dbe";
            saveFileDialog1.Title = "Save a DBE File";
            saveFileDialog1.ShowDialog();
            // http://stackoverflow.com/questions/6397235/write-bytes-to-file
            ByteArrayToFile(saveFileDialog1.FileName, encrypted);
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
            Start_Button.Enabled = true;
            Start_Button.Enabled = true;
            Server_ComboBox.Enabled = true;
            //Connect_Button.Enabled = true;
            DatabaseName_CheckBox.Enabled = true;
            Database_ComboBox.Enabled = true;
        }

        /// Notification is performed here to the progress bar
        void ClientApplication_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Here you play with the main UI thread
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
            }
            //Report 100% completion on operation completed
            ClientApplication_BackgroundWorker.ReportProgress(100);
        }

        private void Server_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckServerConnection();
        }

        private void Server_ComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CheckServerConnection();
            }
        }

        private void CheckServerConnection()
        {
            if (ConnectStatus_PictureBox.Image != null)
            {
                ConnectStatus_PictureBox.Image.Dispose();
                ConnectStatus_PictureBox.Image = null;
                ConnectStatus_PictureBox.InitialImage = null;
            }

            if (DatabaseName_CheckBox.Enabled)
            {
                DatabaseName_CheckBox.Checked = false;
                DatabaseName_CheckBox.Enabled = false;
                Database_ComboBox.Enabled = false;
                Database_ComboBox.Text = "";
            }

            Global_ProgressBar.Style = ProgressBarStyle.Marquee;
            Global_ProgressBar.MarqueeAnimationSpeed = 70;

            SERVER = Server_ComboBox.Text;

            CheckServerConnection_BackgroundWorker.RunWorkerAsync();

            Server_ComboBox.Enabled = false;
            Instance_TableLayoutPanel.Enabled = false;
        }

        private void CheckServerConnection_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SERVER_OK = false;
            string connectionString = null;
            SqlConnection connection;
            connectionString =
            "Data Source=" + SERVER + ";" +
            "Integrated Security=SSPI;";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                SERVER_OK = true;
            }
            catch (SqlException exception)
            {
                MessageBox.Show("Can not open connection ! ");
            }
            CheckServerConnection_BackgroundWorker.ReportProgress(100);
        }

        private void CheckServerConnectionn_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // http://stackoverflow.com/questions/312936/windows-forms-progressbar-easiest-way-to-start-stop-marquee
            Global_ProgressBar.Style = ProgressBarStyle.Continuous;
            Global_ProgressBar.MarqueeAnimationSpeed = 0;
            Server_ComboBox.Enabled = true;
            Instance_TableLayoutPanel.Enabled = true;

            if (SERVER_OK)
            {
                populateDatabaseDropdown();
                DatabaseName_CheckBox.Enabled = true;
                Start_Button.Enabled = true;
                ConnectStatus_PictureBox.Image = global::ClientApplication.Properties.Resources.success;
            }
            else
            {
                ConnectStatus_PictureBox.Image = global::ClientApplication.Properties.Resources.error;
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
