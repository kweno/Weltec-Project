using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Data.Sql;
using System.Threading;
using System.Security.Cryptography;
using System.Drawing;

namespace ClientApplication
{
    public partial class ClientApplicationMain_Form : Form
    {
        /// <summary>
        /// The Key for Encoding and Decoding
        /// </summary>
        private string EVALUATOR_KEY = "AAECAwQFBgcICQoLDA0ODw==";

        /// <summary>
        /// The Server-Instance Selected by the user
        /// </summary>
        private string SERVER = "";

        /// <summary>
        /// The Database selected by the user
        /// </summary>
        private string DATABASE = "";

        /// <summary>
        /// Indicates if the connection to the Server chosen is successful
        /// </summary>
        private bool SERVER_OK = false;

        /// <summary>
        /// Indicates if the Database checkbox is checked
        /// </summary>
        private bool DATABASENAME_CHECKED = false;

        /// <summary>
        /// The encrypted parameters
        /// </summary>
        private byte[] ENCYRPTED = null;

        /// <summary>
        /// The delay used for the threads
        /// </summary>
        private int SLEEP = 200;

        /// The backgroundworker object on which the time consuming operation shall be executed
        /// http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners

        /// <summary>
        /// BackgroundWorker which collects database parameters
        /// </summary>
        BackgroundWorker ClientApplication_BackgroundWorker;

        /// <summary>
        /// BackgroundWorker which checks server connection
        /// </summary>
        BackgroundWorker CheckServerConnection_BackgroundWorker;

        /// <summary>
        /// Constructor
        /// </summary>
        public ClientApplicationMain_Form()
        {
            // Initializes GUI components
            InitializeComponent();
            populateServerDropdown();

            // Adds event handler for combo box
            Server_ComboBox.TextChanged += new System.EventHandler(Server_ComboBox_TextChanged);

            // Makes the windows form not resizable
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

            // Adds event handler for gathering db parameters
            ClientApplication_BackgroundWorker = new BackgroundWorker();
            ClientApplication_BackgroundWorker.DoWork += new DoWorkEventHandler(ClientApplication_BackgroundWorker_DoWork);
            ClientApplication_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ClientApplication_BackgroundWorker_ProgressChanged);
            ClientApplication_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ClientApplication_BackgroundWorker_RunWorkerCompleted);
            ClientApplication_BackgroundWorker.WorkerReportsProgress = true;
            ClientApplication_BackgroundWorker.WorkerSupportsCancellation = true;

            // Adds event handler for checking server connection
            CheckServerConnection_BackgroundWorker = new BackgroundWorker();
            CheckServerConnection_BackgroundWorker.DoWork += new DoWorkEventHandler(CheckServerConnection_BackgroundWorker_DoWork);
            CheckServerConnection_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CheckServerConnection_BackgroundWorker_RunWorkerCompleted);
            CheckServerConnection_BackgroundWorker.WorkerReportsProgress = true;
            CheckServerConnection_BackgroundWorker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Does the text encryption
        /// </summary>
        /// <param name="plainText">The string to be encrypted</param>
        /// <returns></returns>
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

                // Create encryptor to perform the stream transform.
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

        /// <summary>
        /// Save a given byte array (for example the encrypted parameters) to a file
        /// </summary>
        /// <param name="_FileName">File Name where to save byte array</param>
        /// <param name="_ByteArray">Byte Array to save to file</param>
        /// <returns></returns>
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

        /// <summary>
        /// Event Handler for when Start Button is clicked
        /// </summary>
        /// <param name="sender">GUI Component that triggered Click</param>
        /// <param name="e">Event Arguments</param>
        private void Start_Button_Click(object sender, EventArgs e)
        {
            //Start the async operation here
            ClientApplication_BackgroundWorker.RunWorkerAsync();

            Start_Button.Enabled = false;
            Server_ComboBox.Enabled = false;
            DatabaseName_CheckBox.Enabled = false;
            Database_ComboBox.Enabled = false;
        }

        /// <summary>
        /// Populate the Server-Instance Dropdown
        /// </summary>
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

        /// <summary>
        /// Populate the Database Dropdown
        /// </summary>
        private void populateDatabaseDropdown()
        {
            DATABASE = "";
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
            if (Database_ComboBox.Items.Count > 0)
            {
                Database_ComboBox.SelectedIndex = 0;
                DatabaseName_CheckBox.Enabled = true;
                DATABASE = Database_ComboBox.SelectedItem.ToString();
            }
            else
            {
                DatabaseName_CheckBox.Enabled = false;
            }
        }

        /// On completed do the appropriate task
        void ClientApplication_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "SQLServer.dbe";
            saveFileDialog1.Filter = "DBE File|*.dbe";
            saveFileDialog1.Title = "Save a DBE File";
            saveFileDialog1.ShowDialog();
            // http://stackoverflow.com/questions/6397235/write-bytes-to-file
            ByteArrayToFile(saveFileDialog1.FileName, ENCYRPTED);

            Start_Button.Enabled = true;
            Start_Button.Enabled = true;
            Server_ComboBox.Enabled = true;
            DatabaseName_CheckBox.Enabled = true;
            Database_ComboBox.Enabled = true;

            RemoveImage(InstanceMainProgress_PictureBox);
            RemoveImage(InstanceProgress_PictureBox1);
            RemoveImage(InstanceProgress_PictureBox2);
            RemoveImage(InstanceProgress_PictureBox3);
            RemoveImage(DatabaseMainProgress_PictureBox);
            RemoveImage(DatabaseProgress_PictureBox1);
            RemoveImage(DatabaseProgress_PictureBox2);
            RemoveImage(DatabaseProgress_PictureBox3);
            RemoveImage(DatabaseProgress_PictureBox4);
        }

        /// Notification is performed here to the progress bar
        void ClientApplication_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Here you play with the main UI thread
            if (e.ProgressPercentage == 0)
            {
                InstanceMainProgress_PictureBox.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 5)
            {
                InstanceProgress_PictureBox1.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 10)
            {
                InstanceProgress_PictureBox1.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 15)
            {
                InstanceProgress_PictureBox2.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 20)
            {
                InstanceProgress_PictureBox2.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 25)
            {
                InstanceProgress_PictureBox3.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 30)
            {
                InstanceProgress_PictureBox3.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 31)
            {
                InstanceMainProgress_PictureBox.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 32)
            {
                DatabaseMainProgress_PictureBox.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 35)
            {
                DatabaseProgress_PictureBox1.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 40)
            {
                DatabaseProgress_PictureBox1.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 45)
            {
                DatabaseProgress_PictureBox2.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 50)
            {
                DatabaseProgress_PictureBox2.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 55)
            {
                DatabaseProgress_PictureBox3.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 60)
            {
                DatabaseProgress_PictureBox3.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 65)
            {
                DatabaseProgress_PictureBox4.Image = global::ClientApplication.Properties.Resources.right_arrow_3;
            }
            else if (e.ProgressPercentage == 70)
            {
                DatabaseProgress_PictureBox4.Image = global::ClientApplication.Properties.Resources.success;
            }
            else if (e.ProgressPercentage == 71)
            {
                DatabaseMainProgress_PictureBox.Image = global::ClientApplication.Properties.Resources.success;
            }
        }

        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        void ClientApplication_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ENCYRPTED = null;

                string connectionString = null;
                SqlConnection connection;
                SqlCommand command;
                string sql = null;
                SqlDataReader dataReader;

                // http://stackoverflow.com/questions/15631602/how-to-set-sql-server-connection-string
                connectionString =
                "Data Source=" + SERVER + ";" +
                "Initial Catalog=test;" +
                "User id=test;" +
                "Password=test;";
                connectionString =
                "Data Source=" + SERVER + ";" +
                //"Initial Catalog=test;" +
                "Integrated Security=SSPI;";
                //connectionString =
                //"Server=" + serverName + "\\" + instanceName + ";" +
                //"Initial Catalog=test;" +
                //"User id=test;" +
                //"Password=test;";

                String varname1 = "";
                varname1 = varname1 + "DECLARE @ExpressionToSearch VARCHAR(200)";


                String varname11 = "";
                varname11 = varname11 + "DECLARE  @ExpressionToFind VARCHAR(200)";


                String varname12 = "";
                varname12 = varname12 + "DECLARE @rc int";


                String varname13 = "";
                varname13 = varname13 + "DECLARE @dir nvarchar(4000)";


                String varname14 = "";
                varname14 = varname14 + "IF OBJECT_ID('tempdb..#Values') IS NOT NULL DROP TABLE #Values";


                String varname15 = "";
                varname15 = varname15 + "CREATE TABLE [dbo].[#Values] " + "\n";
                varname15 = varname15 + "( [ProcessInfo] VARCHAR(50) NULL, " + "\n";
                varname15 = varname15 + " [Text] VARCHAR(MAX) NULL) ; " + "\n";
                varname15 = varname15 + "  " + "\n";
                varname15 = varname15 + " " + "\n";
                varname15 = varname15 + " " + "\n";
                varname15 = varname15 + "------------- SQL Server Instance ";


                String varname16 = "";
                varname16 = varname16 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('HostName',HOST_NAME())";


                String varname17 = "";
                varname17 = varname17 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('InstanceName',CONVERT(VARCHAR(MAX),SERVERPROPERTY('InstanceName'))) " + "\n";
                varname17 = varname17 + " " + "\n";
                varname17 = varname17 + " " + "\n";
                varname17 = varname17 + " " + "\n";
                varname17 = varname17 + " " + "\n";
                varname17 = varname17 + "-- SQL Server Instance Installation Directory";


                String varname18 = "";
                varname18 = varname18 + "exec @rc = master.dbo.xp_instance_regread " + "\n";
                varname18 = varname18 + "      N'HKEY_LOCAL_MACHINE', " + "\n";
                varname18 = varname18 + "      N'Software\\Microsoft\\MSSQLServer\\Setup', " + "\n";
                varname18 = varname18 + "      N'SQLPath', " + "\n";
                varname18 = varname18 + "      @dir output, 'no_output'";


                String varname19 = "";
                varname19 = varname19 + "SET @ExpressionToFind = 'C:\\Program Files\\'";


                String varname110 = "";
                varname110 = varname110 + "SELECT @ExpressionToSearch = @dir";


                String varname111 = "";
                varname111 = varname111 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname111 = varname111 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Installation Directory','System Drive') " + "\n";
                varname111 = varname111 + "ELSE " + "\n";
                varname111 = varname111 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Installation Directory','Doesn''t use system drive') " + "\n";
                varname111 = varname111 + " " + "\n";
                varname111 = varname111 + " " + "\n";
                varname111 = varname111 + "-- SQL Server Version and Service Pack";


                String varname112 = "";
                varname112 = varname112 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname112 = varname112 + "    SELECT 'SQLVersion', SUBSTRING(@@VERSION, 1, CHARINDEX('-', @@VERSION) - 1) " + "\n";
                varname112 = varname112 + "        + CONVERT(VARCHAR(100), SERVERPROPERTY('edition')) " + "\n";
                varname112 = varname112 + "    AS 'Server Version';";


                String varname113 = "";
                varname113 = varname113 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('ProductLevel',CONVERT(VARCHAR(MAX),SERVERPROPERTY('ProductLevel')))";


                String varname114 = "";
                varname114 = varname114 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('ProductVersion',CONVERT(VARCHAR(MAX),SERVERPROPERTY('ProductVersion'))) " + "\n";
                varname114 = varname114 + " " + "\n";
                varname114 = varname114 + " " + "\n";
                varname114 = varname114 + " " + "\n";
                varname114 = varname114 + " " + "\n";
                varname114 = varname114 + "-- Max DOP";


                String varname115 = "";
                varname115 = varname115 + "IF OBJECT_ID('tempdb..#MaxDOP') IS NOT NULL DROP TABLE #MaxDOP";


                String varname116 = "";
                varname116 = varname116 + "CREATE TABLE [dbo].[#MaxDOP] (NAME VARCHAR(255), minimum INT, maximum INT, config_value INT, run_value INT)";


                String varname117 = "";
                varname117 = varname117 + "EXEC [sp_configure] 'show advanced options', 1;";


                String varname118 = "";
                varname118 = varname118 + "RECONFIGURE;";


                String varname119 = "";
                varname119 = varname119 + "INSERT INTO [#MaxDOP] " + "\n";
                varname119 = varname119 + "EXEC [sp_configure] 'max degree of parallelism' " + "\n";
                varname119 = varname119 + " " + "\n";
                varname119 = varname119 + "EXEC [sp_configure] 'show advanced options', 0;";


                String varname120 = "";
                varname120 = varname120 + "RECONFIGURE;";


                String varname121 = "";
                varname121 = varname121 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) SELECT 'Max Degree Of Parallelism',run_value FROM #MaxDOP WHERE name='max degree of parallelism' " + "\n";
                varname121 = varname121 + " " + "\n";
                varname121 = varname121 + " " + "\n";
                varname121 = varname121 + "-- Memory";


                String varname122 = "";
                varname122 = varname122 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname122 = varname122 + "SELECT [description], CONVERT(VARCHAR(50),value_in_use) FROM sys.configurations " + "\n";
                varname122 = varname122 + "WHERE name like '%server memory%' " + "\n";
                varname122 = varname122 + " " + "\n";
                varname122 = varname122 + " " + "\n";
                varname122 = varname122 + "-- Trace Flags " + "\n";
                varname122 = varname122 + " " + "\n";
                varname122 = varname122 + " " + "\n";
                varname122 = varname122 + " " + "\n";
                varname122 = varname122 + " " + "\n";
                varname122 = varname122 + "-- Command will create the temporary table in tempdb";


                String varname123 = "";
                varname123 = varname123 + "IF OBJECT_ID('tempdb..#TmpErrorLog') IS NOT NULL DROP TABLE #TmpErrorLog";


                String varname124 = "";
                varname124 = varname124 + "CREATE TABLE [dbo].[#TmpErrorLog] " + "\n";
                varname124 = varname124 + "([LogDate] DATETIME NULL, " + "\n";
                varname124 = varname124 + " [ProcessInfo] VARCHAR(20) NULL, " + "\n";
                varname124 = varname124 + " [Text] VARCHAR(MAX) NULL ) ; " + "\n";
                varname124 = varname124 + " " + "\n";
                varname124 = varname124 + "-- Command will insert the errorlog data into temporary table";


                String varname125 = "";
                varname125 = varname125 + "INSERT INTO #TmpErrorLog ([LogDate], [ProcessInfo], [Text]) " + "\n";
                varname125 = varname125 + "EXEC [master].[dbo].[xp_readerrorlog] 0, 1, N'DBCC TRACEON'; " + "\n";
                varname125 = varname125 + " " + "\n";
                varname125 = varname125 + "-- retrieves the data from temporary table";


                String varname126 = "";
                varname126 = varname126 + "SET @ExpressionToFind = 'DBCC TRACEON 2371'";


                String varname127 = "";
                varname127 = varname127 + "SELECT @ExpressionToSearch = [Text] FROM #TmpErrorLog";


                String varname128 = "";
                varname128 = varname128 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname128 = varname128 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Trace Flag 2371','1') " + "\n";
                varname128 = varname128 + "ELSE " + "\n";
                varname128 = varname128 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Trace Flag 2371','0')";


                String varname129 = "";
                varname129 = varname129 + "SET @ExpressionToFind = 'DBCC TRACEON 1117'";


                String varname130 = "";
                varname130 = varname130 + "SELECT @ExpressionToSearch = [Text] FROM #TmpErrorLog";


                String varname131 = "";
                varname131 = varname131 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname131 = varname131 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Trace Flag 1117','1') " + "\n";
                varname131 = varname131 + "ELSE " + "\n";
                varname131 = varname131 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Trace Flag 1117','0')";


                String varname132 = "";
                varname132 = varname132 + "SET @ExpressionToFind = 'DBCC TRACEON 1118'";


                String varname133 = "";
                varname133 = varname133 + "SELECT @ExpressionToSearch = [Text] FROM #TmpErrorLog";


                String varname134 = "";
                varname134 = varname134 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname134 = varname134 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Trace Flag 1118','1') " + "\n";
                varname134 = varname134 + "ELSE " + "\n";
                varname134 = varname134 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Trace Flag 1118','0') " + "\n";
                varname134 = varname134 + " " + "\n";
                varname134 = varname134 + " " + "\n";
                varname134 = varname134 + "-- Default index fill factor";


                String varname135 = "";
                varname135 = varname135 + "IF OBJECT_ID('tempdb..#Fill') IS NOT NULL DROP TABLE #Fill";


                String varname136 = "";
                varname136 = varname136 + "CREATE TABLE [dbo].[#Fill] (NAME VARCHAR(255), minimum INT, maximum INT, config_value INT, run_value INT)";


                String varname137 = "";
                varname137 = varname137 + "EXEC [sp_configure] 'show advanced options', 1;";


                String varname138 = "";
                varname138 = varname138 + "RECONFIGURE;";


                String varname139 = "";
                varname139 = varname139 + "INSERT INTO [#Fill] " + "\n";
                varname139 = varname139 + "EXEC [sp_configure] 'fill factor (%)' " + "\n";
                varname139 = varname139 + " " + "\n";
                varname139 = varname139 + "EXEC [sp_configure] 'show advanced options', 0;";


                String varname140 = "";
                varname140 = varname140 + "RECONFIGURE;";


                String varname141 = "";
                varname141 = varname141 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) SELECT 'Fill Factor Values in (%)',run_value FROM #Fill WHERE name='fill factor (%)' " + "\n";
                varname141 = varname141 + " " + "\n";
                varname141 = varname141 + " " + "\n";
                varname141 = varname141 + "-- Server authentication";


                String varname142 = "";
                varname142 = varname142 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname142 = varname142 + "SELECT 'SQL Server Authentication Mode', CASE SERVERPROPERTY('IsIntegratedSecurityOnly') " + "\n";
                varname142 = varname142 + "WHEN 1 THEN 'Windows Authentication' " + "\n";
                varname142 = varname142 + "WHEN 0 THEN 'Windows and SQL Server Authentication' " + "\n";
                varname142 = varname142 + "END as [Authentication Mode] " + "\n";
                varname142 = varname142 + " " + "\n";
                varname142 = varname142 + " " + "\n";
                varname142 = varname142 + "-- SQL Server Network Port";


                String varname143 = "";
                varname143 = varname143 + "IF OBJECT_ID('tempdb..#TmpErrorLogNetworkPort') IS NOT NULL DROP TABLE #TmpErrorLogNetworkPort";


                String varname144 = "";
                varname144 = varname144 + "CREATE TABLE [dbo].[#TmpErrorLogNetworkPort] " + "\n";
                varname144 = varname144 + "([LogDate] DATETIME NULL, " + "\n";
                varname144 = varname144 + " [ProcessInfo] VARCHAR(20) NULL, " + "\n";
                varname144 = varname144 + " [Text] VARCHAR(MAX) NULL ) ; " + "\n";
                varname144 = varname144 + " " + "\n";
                varname144 = varname144 + "-- Command will insert the errorlog data into temporary table";


                String varname145 = "";
                varname145 = varname145 + "INSERT INTO #TmpErrorLogNetworkPort ([LogDate], [ProcessInfo], [Text]) " + "\n";
                varname145 = varname145 + "EXEC [master].[dbo].[xp_readerrorlog] 0, 1, N'Server is listening on'; " + "\n";
                varname145 = varname145 + " " + "\n";
                varname145 = varname145 + "-- retrieves the data from temporary table";


                String varname146 = "";
                varname146 = varname146 + "SET @ExpressionToFind = '1433'";


                String varname147 = "";
                varname147 = varname147 + "SELECT @ExpressionToSearch = [Text] FROM #TmpErrorLogNetworkPort where text like '%any%' and text like '%<ipv4>%' and text like '%1433%' and ProcessInfo = 'Server'";


                String varname148 = "";
                varname148 = varname148 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname148 = varname148 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('SQL Port','1433') " + "\n";
                varname148 = varname148 + "ELSE " + "\n";
                varname148 = varname148 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('SQL Port','SQL Server doesn''t use default port') " + "\n";
                varname148 = varname148 + " " + "\n";
                varname148 = varname148 + " " + "\n";
                varname148 = varname148 + " " + "\n";
                varname148 = varname148 + "----- SQL Server Database  " + "\n";
                varname148 = varname148 + " " + "\n";
                varname148 = varname148 + "-- DataFiles ";


                String varname149 = "";
                varname149 = varname149 + "DECLARE @DBName VARCHAR(200);";


                String varname150 = "";
                varname150 = varname150 + "declare @sql varchar(200);";


                String varname151 = "";
                varname151 = varname151 + "SET @DBName = '"+DATABASE+"'";


                String varname152 = "";
                varname152 = varname152 + "SELECT @sql = 'USE [' + @DBName + ']'";


                String varname153 = "";
                varname153 = varname153 + "EXEC sp_sqlexec @Sql";


                String varname154 = "";
                varname154 = varname154 + "IF OBJECT_ID('tempdb..#DataFile') IS NOT NULL DROP TABLE #DataFile";


                String varname155 = "";
                varname155 = varname155 + "CREATE TABLE [dbo].[#DataFile] " + "\n";
                varname155 = varname155 + "	([name] VARCHAR(200) NULL, " + "\n";
                varname155 = varname155 + "	[fileid] int NULL, " + "\n";
                varname155 = varname155 + "	[filename] VARCHAR(max) NULL, " + "\n";
                varname155 = varname155 + "	[filegroup] VARCHAR(50) NULL, " + "\n";
                varname155 = varname155 + "	[size] VARCHAR(50) NULL, " + "\n";
                varname155 = varname155 + "	[maxsize] VARCHAR(50) NULL, " + "\n";
                varname155 = varname155 + "	[growth] VARCHAR(50) NULL, " + "\n";
                varname155 = varname155 + "	[usage] VARCHAR(50) NULL) ; " + "\n";
                varname155 = varname155 + " " + "\n";
                varname155 = varname155 + "-- Command will insert the errorlog data into temporary table";


                String varname156 = "";
                varname156 = varname156 + "INSERT INTO #DataFile ([name], [fileid], [filename], [filegroup], [size], [maxsize], [growth], [usage]) " + "\n";
                varname156 = varname156 + "EXEC [sp_helpfile]";


                String varname157 = "";
                varname157 = varname157 + "SET @ExpressionToFind = 'C:\\Program Files\\'";


                String varname158 = "";
                varname158 = varname158 + "SELECT @ExpressionToSearch = [filename] FROM #DataFile where [filename] like '%.mdf'";


                String varname159 = "";
                varname159 = varname159 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname159 = varname159 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Datafile Location','System Drive') " + "\n";
                varname159 = varname159 + "ELSE " + "\n";
                varname159 = varname159 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Datafile Location','Datafile doesn''t use system drive')";


                String varname160 = "";
                varname160 = varname160 + "SELECT @ExpressionToSearch = [filename] FROM #DataFile where [filename] like '%.ldf'";


                String varname161 = "";
                varname161 = varname161 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname161 = varname161 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Logfile Location','System Drive') " + "\n";
                varname161 = varname161 + "ELSE " + "\n";
                varname161 = varname161 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Logfile Location','Logfile doesn''t use system drive') " + "\n";
                varname161 = varname161 + " " + "\n";
                varname161 = varname161 + " " + "\n";
                varname161 = varname161 + "-- Recovery Model";


                String varname162 = "";
                varname162 = varname162 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname162 = varname162 + "SELECT 'Recovery Model' , recovery_model_desc FROM sys.databases WHERE name = @DBName " + "\n";
                varname162 = varname162 + " " + "\n";
                varname162 = varname162 + " " + "\n";
                varname162 = varname162 + "-- Compatibility Level";


                String varname163 = "";
                varname163 = varname163 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname163 = varname163 + "SELECT 'Compatibility Level' , [compatibility_level] FROM sys.databases WHERE name = @DBName " + "\n";
                varname163 = varname163 + " " + "\n";
                varname163 = varname163 + "-- Read Committed Snapshot Isolation";


                String varname164 = "";
                varname164 = varname164 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname164 = varname164 + "SELECT 'Snapshot Isolation', [snapshot_isolation_state_desc] FROM sys.databases WHERE name = @DBName";


                String varname165 = "";
                varname165 = varname165 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname165 = varname165 + "SELECT 'Read Committed Snapshot Isolation', [is_read_committed_snapshot_on] FROM sys.databases WHERE name = @DBName " + "\n";
                varname165 = varname165 + " " + "\n";
                varname165 = varname165 + "-- Database Auto growth";


                String varname166 = "";
                varname166 = varname166 + "SET @ExpressionToFind = 'KB'";


                String varname167 = "";
                varname167 = varname167 + "SELECT @ExpressionToSearch = CASE is_percent_growth WHEN 1 THEN CONVERT(VARCHAR(10),growth) +'%' ELSE Convert(VARCHAR(10),growth*8) +' KB' END " + "\n";
                varname167 = varname167 + "FROM sys.master_files " + "\n";
                varname167 = varname167 + "WHERE  name = @DBName and [physical_name] like '%.mdf'";


                String varname168 = "";
                varname168 = varname168 + "print @ExpressionToSearch";


                String varname169 = "";
                varname169 = varname169 + "IF @ExpressionToSearch LIKE '%' + @ExpressionToFind + '%' " + "\n";
                varname169 = varname169 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Datafile Growth','Fix in size') " + "\n";
                varname169 = varname169 + "ELSE " + "\n";
                varname169 = varname169 + "    INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('Datafile Growth','In percent') " + "\n";
                varname169 = varname169 + " " + "\n";
                varname169 = varname169 + "-- Auto Create Statistics";


                String varname170 = "";
                varname170 = varname170 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname170 = varname170 + "SELECT 'Auto Create Statistics' , [is_auto_create_stats_on] FROM sys.databases WHERE name = @DBName " + "\n";
                varname170 = varname170 + " " + "\n";
                varname170 = varname170 + "-- Auto Update Statistics";


                String varname171 = "";
                varname171 = varname171 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname171 = varname171 + "SELECT 'Auto Update Statistics' , [is_auto_update_stats_on] FROM sys.databases WHERE name = @DBName " + "\n";
                varname171 = varname171 + " " + "\n";
                varname171 = varname171 + "-- Auto Shrink";


                String varname172 = "";
                varname172 = varname172 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname172 = varname172 + "SELECT 'Auto Shrink' , [is_auto_shrink_on] FROM sys.databases WHERE name = @DBName " + "\n";
                varname172 = varname172 + " " + "\n";
                varname172 = varname172 + " " + "\n";
                varname172 = varname172 + "-- Daily Index Rebuild";


                String varname173 = "";
                varname173 = varname173 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname173 = varname173 + "SELECT TOP 1 'Old Statistics Found' " + "\n";
                varname173 = varname173 + ",DATEDIFF(d, (STATS_DATE(i.OBJECT_ID, index_id)), Getdate()) as Days_Since_Last " + "\n";
                varname173 = varname173 + "FROM sys.indexes i " + "\n";
                varname173 = varname173 + "INNER JOIN sys.objects o ON i.object_id = o.object_id " + "\n";
                varname173 = varname173 + "INNER JOIN sys.schemas sc ON o.schema_id = sc.schema_id " + "\n";
                varname173 = varname173 + "WHERE i.name IS NOT NULL " + "\n";
                varname173 = varname173 + "AND o.type = 'U' " + "\n";
                varname173 = varname173 + "ORDER BY (STATS_DATE(i.OBJECT_ID, index_id)) ASC " + "\n";
                varname173 = varname173 + " " + "\n";
                varname173 = varname173 + "-- Daily database Full backup";


                String varname174 = "";
                varname174 = varname174 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) " + "\n";
                varname174 = varname174 + "SELECT 'Last Full Backup', DATEDIFF(d, MAX(Backup_Finish_Date), Getdate()) as Days_Since_Last " + "\n";
                varname174 = varname174 + "FROM MSDB.dbo.BackupSet " + "\n";
                varname174 = varname174 + "WHERE Type = 'd' " + "\n";
                varname174 = varname174 + "GROUP BY Database_Name " + "\n";
                varname174 = varname174 + " " + "\n";
                varname174 = varname174 + "-- Blank SQL 'SA' Password";


                String varname175 = "";
                varname175 = varname175 + "IF NOT EXISTS " + "\n";
                varname175 = varname175 + "( " + "\n";
                varname175 = varname175 + "SELECT name,* FROM sys.sql_logins " + "\n";
                varname175 = varname175 + "WHERE name = 'sa' AND (PWDCOMPARE('', password_hash) = 1 OR PWDCOMPARE('', password_hash, 1) = 1) " + "\n";
                varname175 = varname175 + ") " + "\n";
                varname175 = varname175 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('SA Login','Does not have a blank password') " + "\n";
                varname175 = varname175 + "ELSE " + "\n";
                varname175 = varname175 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('SA Login','Does have a blank password') " + "\n";
                varname175 = varname175 + " " + "\n";
                varname175 = varname175 + " " + "\n";
                varname175 = varname175 + "-- NT AUTHORITY\\SYSTEM Administrator";


                String varname176 = "";
                varname176 = varname176 + "IF NOT EXISTS " + "\n";
                varname176 = varname176 + "( " + "\n";
                varname176 = varname176 + "SELECT name,* FROM syslogins WHERE name = 'NT AUTHORITY\\SYSTEM' AND hasaccess = 1 " + "\n";
                varname176 = varname176 + ") " + "\n";
                varname176 = varname176 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('NT AUTHORITY\\SYSTEM','Does not have a access') " + "\n";
                varname176 = varname176 + "ELSE " + "\n";
                varname176 = varname176 + "INSERT INTO [#Values] ([ProcessInfo], [Text]) VALUES ('NT AUTHORITY\\SYSTEM','Does have a access')";


                String varname177 = "";
                varname177 = varname177 + "SELECT * FROM [#Values]";

                connection = new SqlConnection(connectionString);
                connection.Open();

                Thread.Sleep(SLEEP);
                ClientApplication_BackgroundWorker.ReportProgress(0);

                string header = 
                        varname1 + Environment.NewLine +
                        varname11 + Environment.NewLine +
                        varname12 + Environment.NewLine +
                        varname13 + Environment.NewLine;

                // http://www.dpriver.com/pp/sqlformat.htm
                sql =
                        // SQL Server Instance 

                        // 1.Installation
                        header +
                        varname14 + Environment.NewLine +
                        varname15 + Environment.NewLine +
                        varname16 + Environment.NewLine +
                        varname17 + Environment.NewLine +
                        varname18 + Environment.NewLine +
                        varname19 + Environment.NewLine +
                        varname110 + Environment.NewLine +
                        varname111 + Environment.NewLine +
                        varname112 + Environment.NewLine +
                        varname113 + Environment.NewLine +
                        varname114 + Environment.NewLine;

                Thread.Sleep(SLEEP);
                ClientApplication_BackgroundWorker.ReportProgress(5);

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                Thread.Sleep(SLEEP);
                ClientApplication_BackgroundWorker.ReportProgress(10);

                sql =
                        // 2. Configuration 
                        header +
                        varname115 + Environment.NewLine +
                        varname116 + Environment.NewLine +
                        varname117 + Environment.NewLine +
                        varname118 + Environment.NewLine +
                        varname119 + Environment.NewLine +
                        varname120 + Environment.NewLine +
                        varname121 + Environment.NewLine +
                        varname122 + Environment.NewLine +
                        varname123 + Environment.NewLine +
                        varname124 + Environment.NewLine +
                        varname125 + Environment.NewLine +
                        varname126 + Environment.NewLine +
                        varname127 + Environment.NewLine +
                        varname128 + Environment.NewLine +
                        varname129 + Environment.NewLine +
                        varname130 + Environment.NewLine +
                        varname131 + Environment.NewLine +
                        varname132 + Environment.NewLine +
                        varname133 + Environment.NewLine +
                        varname134 + Environment.NewLine +
                        varname135 + Environment.NewLine +
                        varname136 + Environment.NewLine +
                        varname137 + Environment.NewLine +
                        varname138 + Environment.NewLine +
                        varname139 + Environment.NewLine +
                        varname140 + Environment.NewLine +
                        varname141 + Environment.NewLine +
                        varname142 + Environment.NewLine;

                Thread.Sleep(SLEEP);
                ClientApplication_BackgroundWorker.ReportProgress(15);

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                Thread.Sleep(200);
                ClientApplication_BackgroundWorker.ReportProgress(20);

                sql =
                        // 3. Security
                        header +
                        varname143 + Environment.NewLine +
                        varname144 + Environment.NewLine +
                        varname145 + Environment.NewLine +
                        varname146 + Environment.NewLine +
                        varname147 + Environment.NewLine +
                        varname148 + Environment.NewLine;

                Thread.Sleep(200);
                ClientApplication_BackgroundWorker.ReportProgress(25);

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                Thread.Sleep(200);
                ClientApplication_BackgroundWorker.ReportProgress(30);

                Thread.Sleep(200);
                ClientApplication_BackgroundWorker.ReportProgress(31);


                if (DATABASENAME_CHECKED)
                {
                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(32);

                    string dbHeader =
                            varname149 + Environment.NewLine +
                            varname150 + Environment.NewLine +
                            varname151 + Environment.NewLine;

                    sql =
                            // SQL Server Database

                            // 1. Implemetation 
                            header +
                            dbHeader +
                            varname152 + Environment.NewLine +
                            varname153 + Environment.NewLine +
                            varname154 + Environment.NewLine +
                            varname155 + Environment.NewLine +
                            varname156 + Environment.NewLine +
                            varname157 + Environment.NewLine +
                            varname158 + Environment.NewLine +
                            varname159 + Environment.NewLine +
                            varname160 + Environment.NewLine +
                            varname161 + Environment.NewLine;

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(35);

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(40);

                    sql =
                            // 2. Configuration Options
                            header +
                            dbHeader +
                            varname162 + Environment.NewLine +
                            varname163 + Environment.NewLine +
                            varname164 + Environment.NewLine +
                            varname165 + Environment.NewLine +
                            varname166 + Environment.NewLine +
                            varname167 + Environment.NewLine +
                            varname168 + Environment.NewLine +
                            varname169 + Environment.NewLine +
                            varname170 + Environment.NewLine +
                            varname171 + Environment.NewLine +
                            varname172 + Environment.NewLine;

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(45);

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(50);

                    sql =
                            // 3. Maintenance
                            header +
                            dbHeader +
                            varname173 + Environment.NewLine +
                            varname174 + Environment.NewLine;

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(55);

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(60);

                    sql =
                            // 4. Security
                            header +
                            dbHeader +
                            varname175 + Environment.NewLine +
                            varname176 + Environment.NewLine;

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(65);

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(70);

                    Thread.Sleep(200);
                    ClientApplication_BackgroundWorker.ReportProgress(71);
                }


                // Retrive all Values to put in XML
                sql = varname177;


                // http://csharp.net-informations.com/xml/xml-from-sql.htm
                SqlDataAdapter adapter;
                DataSet ds = new DataSet();
                adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(ds);
                command.Dispose();
                connection.Close();

                // http://stackoverflow.com/questions/963870/dataset-writexml-to-string
                StringWriter sw = new StringWriter();
                ds.WriteXml(@"SQLServer.xml"); // COMMENT OUT THIS LINE AFTER TESTING
                ds.WriteXml(sw);
                string result = sw.ToString();

                

                try
                {
                    // Encrypt the string to an array of bytes.
                    ENCYRPTED = EncryptStringToBytes_Aes(result);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error: {0}", exception.Message);
                }

                
                //Report 100% completion on operation completed
                ClientApplication_BackgroundWorker.ReportProgress(100);
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show("Can not open connection ! ");
            }
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
            RemoveImage(ConnectStatus_PictureBox);

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

                // https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.serverversion(v=vs.110).aspx
                // https://support.microsoft.com/en-nz/kb/321185
                var ver = Version.Parse(connection.ServerVersion);
                var majorMinorString = ver.Major + "." + ver.Minor;
                double majorMinorDouble;
                if (double.TryParse(majorMinorString, out majorMinorDouble))
                    //if(10.5 <= majorMinorDouble && majorMinorDouble < 12)
                        SERVER_OK = true;
                    //else
                        //MessageBox.Show("SQL Server Version " + ver.Major + "." + ver.Minor + " not supported", "Information");
                connection.Close();
            }
            catch (SqlException exception)
            {
                MessageBox.Show("Can not open connection ! ");
            }
            CheckServerConnection_BackgroundWorker.ReportProgress(100);
        }

        private void CheckServerConnection_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // http://stackoverflow.com/questions/312936/windows-forms-progressbar-easiest-way-to-start-stop-marquee
            Global_ProgressBar.Style = ProgressBarStyle.Continuous;
            Global_ProgressBar.MarqueeAnimationSpeed = 0;
            Server_ComboBox.Enabled = true;
            Instance_TableLayoutPanel.Enabled = true;

            if (SERVER_OK)
            {
                populateDatabaseDropdown();
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
                DATABASENAME_CHECKED = true;
            }
            else
            {
                Database_ComboBox.Enabled = false;
                Database_ComboBox.Items.Clear();
                Database_TableLayoutPanel.Enabled = false;
                DATABASENAME_CHECKED = false;
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

        private void Database_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DATABASE = Database_ComboBox.SelectedItem.ToString();
        }

        private void RemoveImage(PictureBox picBox)
        {
            if (picBox.Image != null)
            {
                picBox.Image.Dispose();
                picBox.Image = null;
                picBox.InitialImage = null;
            }
        }




    }
}
