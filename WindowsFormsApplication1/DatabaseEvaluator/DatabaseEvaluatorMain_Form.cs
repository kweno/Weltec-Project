using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;

namespace DatabaseEvaluator
{
    public partial class DatabaseEvaluatorMain_Form : Form
    {
        private string INSTANCE = "DESKTOP-FVFO8GL\\SQL2016N";
        private string EVALUATOR_KEY = "AAECAwQFBgcICQoLDA0ODw==";
        private string PARAMETER_VALUES = "";
        
        /// <summary>
        /// BackgroundWorker which does the Evaluation
        /// </summary>
        // http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners
        BackgroundWorker DatabaseEvaluator_BackgroundWorker;

        public DatabaseEvaluatorMain_Form()
        {
            InitializeComponent();

            // Adds event handler for checking server connection
            DatabaseEvaluator_BackgroundWorker = new BackgroundWorker();
            DatabaseEvaluator_BackgroundWorker.DoWork += new DoWorkEventHandler(DatabaseEvaluator_BackgroundWorker_DoWork);
            DatabaseEvaluator_BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DatabaseEvaluator_BackgroundWorker_RunWorkerCompleted);
            DatabaseEvaluator_BackgroundWorker.WorkerReportsProgress = true;
            DatabaseEvaluator_BackgroundWorker.WorkerSupportsCancellation = true;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
        }

        private void DatabaseEvaluator_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(PARAMETER_VALUES);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "SQLServer.pdf";
            saveFileDialog1.Filter = "PDF File|*.pdf";
            saveFileDialog1.Title = "Save a PDF File";
            saveFileDialog1.ShowDialog();

            // http://www.codeproject.com/Articles/686994/Create-Read-Advance-PDF-Report-using-iTextSharp-in
            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter pdfwriter = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // http://stackoverflow.com/questions/17087154/itextsharp-how-to-place-text-at-the-middle-of-a-page
            //Create a single column table
            var t = new PdfPTable(1);

            //Tell it to fill the page horizontally
            t.WidthPercentage = 100;

            //Create a single cell
            var c = new PdfPCell();
            c.Border = Rectangle.NO_BORDER;

            //Tell the cell to vertically align in the middle
            c.VerticalAlignment = Element.ALIGN_MIDDLE;

            //Tell the cell to fill the page vertically
            c.MinimumHeight = doc.PageSize.Height - (doc.BottomMargin + doc.TopMargin);

            //Create a test paragraph
            var p = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam iaculis sem diam, quis accumsan ipsum venenatis ac. Pellentesque nec gravida tortor. Suspendisse dapibus quis quam sed sollicitudin.");

            //Add it a couple of times
            c.AddElement(p);
            c.AddElement(p);

            //Add the cell to the paragraph
            t.AddCell(c);

            //Add the table to the document
            doc.Add(t);

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var whiteBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
            var darkBlue = new BaseColor(79, 129, 188);
            var lightBlue = new BaseColor(219, 229, 241);
            doc.Add(new Paragraph("SQL Server", boldFont));
            doc.Add(new Paragraph(" "));

            // http://www.mikesdotnetting.com/article/86/itextsharp-introducing-tables
            for (int i = 0; i < 20; i++)
            {
                PdfPTable table = new PdfPTable(4);
                PdfPCell header_cell = new PdfPCell(new Phrase("Issue 1: Install SQL 2008 R2 SP2 CU11", whiteBoldFont));
                header_cell.Colspan = 4;
                header_cell.BackgroundColor = darkBlue;
                table.AddCell(header_cell);
                PdfPCell issueTypeHeader_cell = new PdfPCell(new Phrase("Issue Type", boldFont));
                issueTypeHeader_cell.BackgroundColor = lightBlue;
                table.AddCell(issueTypeHeader_cell);
                PdfPCell issueType_cell = new PdfPCell(new Phrase("Health"));
                issueType_cell.BackgroundColor = lightBlue;
                table.AddCell(issueType_cell);
                PdfPCell issueSeverityHeader_cell = new PdfPCell(new Phrase("Issue Severity", boldFont));
                issueSeverityHeader_cell.BackgroundColor = lightBlue;
                table.AddCell(issueSeverityHeader_cell);
                PdfPCell issueSeverity_cell = new PdfPCell(new Phrase("Critical"));
                issueSeverity_cell.BackgroundColor = lightBlue;
                table.AddCell(issueSeverity_cell);
                PdfPCell summary_cell = new PdfPCell(new Phrase("Currently Installed version of SQL Sserver is SQL server 2008 R2 SP1 (10.50)", boldFont));
                summary_cell.Colspan = 4;
                table.AddCell(summary_cell);
                PdfPCell comments_cell = new PdfPCell(new Phrase("This version is unsupported. We recommend installing the latest update of SQL Server, which is SQL server 2008 R2 SP2 CU11 http://support.microsoft.com/kb/2926028"));
                comments_cell.Colspan = 4;
                table.AddCell(comments_cell);
                doc.Add(table);
                doc.Add(new Paragraph(" "));
            }
            doc.Close();

            Global_ProgressBar.Style = ProgressBarStyle.Continuous;
            Global_ProgressBar.MarqueeAnimationSpeed = 0;
            PathToXML_TextBox.Enabled = true;
            Browse_Button.Enabled = true;
            Start_Button.Enabled = true;
        }

        private void DatabaseEvaluator_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var fileLocation = this.PathToXML_TextBox.Text;

            // Decrypt the bytes to a string.
            // http://stackoverflow.com/questions/2030847/best-way-to-read-a-large-file-into-a-byte-array-in-c
            string decrypted = DecryptStringFromBytes_Aes(File.ReadAllBytes(fileLocation));

            Console.WriteLine(decrypted);
            //MessageBox.Show(decrypted);

            DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();

            //var instance = "";

            //if ((servers.Rows[0]["InstanceName"] as string) != null)
            //    instance = servers.Rows[0]["ServerName"] + "\\" + servers.Rows[0]["InstanceName"];
            //else
            //    instance = servers.Rows[0]["ServerName"] + "";

            //var connectionString = "Data Source=" + intance + ";" +
            var connectionString = "Data Source=" + INSTANCE + ";" +
            //"Initial Catalog=test;" +
            "Integrated Security=SSPI;";

            var sql = "DECLARE @xml xml" + "\n"
                    + "SET @xml = N'"
                    + decrypted + "'\n"
                    //+ "<NewDataSet>" + "\n"
                    //+ "  <Table>" + "\n"
                    //+ "    <ProcessInfo>HostName</ProcessInfo>" + "\n"
                    //+ "    <Text>B105-01</Text>" + "\n"
                    //+ "  </Table>" + "\n"
                    //+ "  <Table>" + "\n"
                    //+ "    <ProcessInfo>InstanceName</ProcessInfo>" + "\n"
                    //+ "  </Table>" + "\n"
                    //+ "  <Table>" + "\n"
                    //+ "    <ProcessInfo>ProductLevel</ProcessInfo>" + "\n"
                    //+ "    <Text>RTM</Text>" + "\n"
                    //+ "  </Table>" + "\n"
                    //+ "  <Table>" + "\n"
                    //+ "    <ProcessInfo>ProductVersion</ProcessInfo>" + "\n"
                    //+ "    <Text>10.50.1600.1</Text>" + "\n"
                    //+ "  </Table>" + "\n"
                    //+ "  <Table>" + "\n"
                    //+ "    <ProcessInfo>SQLVersion</ProcessInfo>" + "\n"
                    //+ "    <Text>Microsoft SQL Server 2008 R2 (RTM) Enterprise Evaluation Edition (64-bit)</Text>" + "\n"
                    //+ "  </Table>" + "\n"
                    //+ "</NewDataSet>'" + "\n"
                    + "DECLARE @ProductVersion nvarchar(20)" + "\n"
                    + "DECLARE @SQLName nvarchar(50)" + "\n"
                    + "SELECT @ProductVersion = doc.col.value('Text[1]', 'nvarchar(50)')" + "\n"
                    + "FROM @xml.nodes('/NewDataSet/Table') doc(col)" + "\n"
                    + "WHERE doc.col.value('ProcessInfo[1]', 'varchar(100)') = 'ProductVersion'" + "\n"
                    + "Print @ProductVersion" + "\n"
                    + "IF (LEFT(@ProductVersion ,2) = '10')" + "\n"
                    + "   SET @SQLName = 'Microsoft SQL Server 2008 R2'" + "\n"
                    + "ELSE IF (LEFT(@ProductVersion ,2) = '11')" + "\n"
                    + "   SET @SQLName = 'Microsoft SQL Server 2012'" + "\n"
                    + "Select * from [EVALUATOR].[dbo].[ServicePack]" + "\n"
                    + "WHERE [LatestServicePackValue] != @ProductVersion and [SQLServerVersion] = @SQLName";

            String varname1 = "";
            varname1 = varname1 + "DECLARE @xml xml";


            String varname11 = "";
            //varname11 = varname11 + "SET @xml = N'\n" + decrypted + "'";
            varname11 = varname11 + "SET @xml = N' " + "\n";
            varname11 = varname11 + "<NewDataSet> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>HostName</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>DESKTOP-FVFO8GL</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>InstanceName</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>SQL2016N2</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Installation Directory</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>System Drive</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>SQLVersion</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>Microsoft SQL Server 2016 (RTM) Express Edition (64-bit)</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>ProductLevel</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>RTM</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>ProductVersion</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>13.0.1601.5</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Max Degree Of Parallelism</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Minimum size of server memory (MB)</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>16</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Maximum size of server memory (MB)</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>2147483647</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Trace Flag 2371</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Trace Flag 1117</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Trace Flag 1118</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Fill Factor Values in (%)</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>SQL Server Authentication Mode</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>Windows Authentication</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>SQL Port</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>SQL Server doesnt use default port</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Datafile Location</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>System Drive</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Logfile Location</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>System Drive</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Recovery Model</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>SIMPLE</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Compatibility Level</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>130</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Snapshot Isolation</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>OFF</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Read Committed Snapshot Isolation</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Datafile Growth</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>In percent</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Auto Create Statistics</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>1</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Auto Update Statistics</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>1</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>Auto Shrink</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>0</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>SA Login</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>Does not have a blank password</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "  <Table> " + "\n";
            varname11 = varname11 + "    <ProcessInfo>NT AUTHORITY\\SYSTEM</ProcessInfo> " + "\n";
            varname11 = varname11 + "    <Text>Does have a access</Text> " + "\n";
            varname11 = varname11 + "  </Table> " + "\n";
            varname11 = varname11 + "</NewDataSet>'";


            String varname12 = "";
            varname12 = varname12 + "DECLARE @ExpressionToSearch VARCHAR(200)";


            String varname13 = "";
            varname13 = varname13 + "DECLARE  @ExpressionToFind VARCHAR(200)";


            String varname14 = "";
            varname14 = varname14 + "IF OBJECT_ID('tempdb..#ParameterDetails') IS NOT NULL DROP TABLE #ParameterDetails";


            String varname15 = "";
            varname15 = varname15 + "IF OBJECT_ID('tempdb..#XMLwithOpenXML') IS NOT NULL DROP TABLE #XMLwithOpenXML";


            String varname16 = "";
            varname16 = varname16 + "CREATE TABLE #ParameterDetails( " + "\n";
            varname16 = varname16 + "	[PassValue] [nvarchar](5) NULL, " + "\n";
            varname16 = varname16 + "	[ParameterName] [nvarchar](100) NULL, " + "\n";
            varname16 = varname16 + "	[ClientParameterName] [nvarchar](50) NULL, " + "\n";
            varname16 = varname16 + "	[BestPracticeValue] [nvarchar](100) NULL, " + "\n";
            varname16 = varname16 + "	[IssueType] [nvarchar](50) NULL, " + "\n";
            varname16 = varname16 + "	[IssueSeverity] [nvarchar](15) NULL, " + "\n";
            varname16 = varname16 + "	[Problem] [nvarchar](max) NULL, " + "\n";
            varname16 = varname16 + "	[Recommendation] [nvarchar](max) NULL, " + "\n";
            varname16 = varname16 + "	[Why] [nvarchar](max) NULL, " + "\n";
            varname16 = varname16 + "	[Link] [nvarchar](max) NULL, " + "\n";
            varname16 = varname16 + "	[Link2] [nvarchar](max) NULL " + "\n";
            varname16 = varname16 + ")";


            String varname17 = "";
            varname17 = varname17 + "CREATE TABLE #XMLwithOpenXML( " + "\n";
            varname17 = varname17 + "	Id INT IDENTITY PRIMARY KEY, " + "\n";
            varname17 = varname17 + "	XMLData XML, " + "\n";
            varname17 = varname17 + "	LoadedDateTime DATETIME " + "\n";
            varname17 = varname17 + ") " + "\n";
            varname17 = varname17 + " " + "\n";
            varname17 = varname17 + " " + "\n";
            varname17 = varname17 + " " + "\n";
            varname17 = varname17 + " " + "\n";
            varname17 = varname17 + " " + "\n";
            varname17 = varname17 + "-- SQL Server Instance Installation Directory";


            String varname18 = "";
            varname18 = varname18 + "DECLARE @InstallationDirectory nvarchar(MAX)";


            String varname19 = "";
            varname19 = varname19 + "SELECT @InstallationDirectory = doc.col.value('Text[1]', 'nvarchar(MAX)') " + "\n";
            varname19 = varname19 + "FROM @xml.nodes('/NewDataSet/Table') doc(col) " + "\n";
            varname19 = varname19 + "WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Installation Directory'";


            String varname110 = "";
            varname110 = varname110 + "SET @ExpressionToFind = 'Not System Drive'";


            String varname111 = "";
            varname111 = varname111 + "SELECT @ExpressionToSearch = @InstallationDirectory";


            String varname112 = "";
            varname112 = varname112 + "IF @ExpressionToSearch = @ExpressionToFind " + "\n";
            varname112 = varname112 + "    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname112 = varname112 + "									 [Problem],[Recommendation],[Why],[Link],[Link2]) " + "\n";
            varname112 = varname112 + "    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname112 = varname112 + "									 [Problem],[Recommendation],[Why],[Link],[Link2] " + "\n";
            varname112 = varname112 + "									 from [EVALUATOR].[dbo].[ParameterDesc] " + "\n";
            varname112 = varname112 + "									 WHERE ParameterName = 'SQL Server Instance Installation Directory' " + "\n";
            varname112 = varname112 + "ELSE " + "\n";
            varname112 = varname112 + "	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname112 = varname112 + "									 [Problem],[Recommendation],[Why],[Link],[Link2]) " + "\n";
            varname112 = varname112 + "    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname112 = varname112 + "									 [Problem],[Recommendation],[Why],[Link],[Link2] " + "\n";
            varname112 = varname112 + "									 from [EVALUATOR].[dbo].[ParameterDesc] " + "\n";
            varname112 = varname112 + "									 WHERE ParameterName = 'SQL Server Instance Installation Directory' " + "\n";
            varname112 = varname112 + "     " + "\n";
            varname112 = varname112 + " " + "\n";
            varname112 = varname112 + " " + "\n";
            varname112 = varname112 + "-- SQL Server Version and Service Pack";


            String varname113 = "";
            varname113 = varname113 + "DECLARE @ProductVersion nvarchar(20)";


            String varname114 = "";
            varname114 = varname114 + "DECLARE @SQLName nvarchar(50)";


            String varname115 = "";
            varname115 = varname115 + "SELECT @ProductVersion = doc.col.value('Text[1]', 'nvarchar(50)') " + "\n";
            varname115 = varname115 + "FROM @xml.nodes('/NewDataSet/Table') doc(col) " + "\n";
            varname115 = varname115 + "WHERE doc.col.value('ProcessInfo[1]', 'varchar(100)') = 'ProductVersion'";


            String varname116 = "";
            varname116 = varname116 + "IF (LEFT(@ProductVersion ,2) = '10') " + "\n";
            varname116 = varname116 + "   SET @SQLName = 'Microsoft SQL Server 2008 R2' " + "\n";
            varname116 = varname116 + "ELSE IF (LEFT(@ProductVersion ,2) = '11') " + "\n";
            varname116 = varname116 + "   SET @SQLName = 'Microsoft SQL Server 2012'";


            String varname117 = "";
            varname117 = varname117 + "IF (Select COUNT(1) from [EVALUATOR].[dbo].[ServicePack]WHERE [LatestServicePackValue] != @ProductVersion and [SQLServerVersion] = @SQLName) > 0 " + "\n";
            varname117 = varname117 + "    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname117 = varname117 + "									 [Problem],[Recommendation],[Why],[Link],[Link2]) " + "\n";
            varname117 = varname117 + "    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname117 = varname117 + "									 [Problem],[Recommendation],[Why],[Link],[Link2] " + "\n";
            varname117 = varname117 + "									 from [EVALUATOR].[dbo].[ParameterDesc] " + "\n";
            varname117 = varname117 + "									 WHERE ParameterName = 'SQL Server Version and Service Pack' " + "\n";
            varname117 = varname117 + "ELSE " + "\n";
            varname117 = varname117 + "	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname117 = varname117 + "									 [Problem],[Recommendation],[Why],[Link],[Link2]) " + "\n";
            varname117 = varname117 + "    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname117 = varname117 + "									 [Problem],[Recommendation],[Why],[Link],[Link2] " + "\n";
            varname117 = varname117 + "									 from [EVALUATOR].[dbo].[ParameterDesc] " + "\n";
            varname117 = varname117 + "									 WHERE ParameterName = 'SQL Server Version and Service Pack' " + "\n";
            varname117 = varname117 + "								 " + "\n";
            varname117 = varname117 + "								 " + "\n";
            varname117 = varname117 + "-- Max Degree Of Parallelism	";


            String varname118 = "";
            varname118 = varname118 + "DECLARE @MaxDegree nvarchar(2)";


            String varname119 = "";
            varname119 = varname119 + "SELECT @MaxDegree = doc.col.value('Text[1]', 'nvarchar(MAX)') " + "\n";
            varname119 = varname119 + "FROM @xml.nodes('/NewDataSet/Table') doc(col) " + "\n";
            varname119 = varname119 + "WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Max Degree Of Parallelism'";


            String varname120 = "";
            varname120 = varname120 + "SET @ExpressionToFind = 'Not Default'";


            String varname121 = "";
            varname121 = varname121 + "SELECT @ExpressionToSearch = @MaxDegree";


            String varname122 = "";
            varname122 = varname122 + "IF @ExpressionToSearch = @ExpressionToFind " + "\n";
            varname122 = varname122 + "    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname122 = varname122 + "									 [Problem],[Recommendation],[Why],[Link],[Link2]) " + "\n";
            varname122 = varname122 + "    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname122 = varname122 + "									 [Problem],[Recommendation],[Why],[Link],[Link2] " + "\n";
            varname122 = varname122 + "									 from [EVALUATOR].[dbo].[ParameterDesc] " + "\n";
            varname122 = varname122 + "									 WHERE ParameterName = 'Max Degree Of Parallelism' " + "\n";
            varname122 = varname122 + "ELSE " + "\n";
            varname122 = varname122 + "	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname122 = varname122 + "									 [Problem],[Recommendation],[Why],[Link],[Link2]) " + "\n";
            varname122 = varname122 + "    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity], " + "\n";
            varname122 = varname122 + "									 [Problem],[Recommendation],[Why],[Link],[Link2] " + "\n";
            varname122 = varname122 + "									 from [EVALUATOR].[dbo].[ParameterDesc] " + "\n";
            varname122 = varname122 + "									 WHERE ParameterName = 'Max Degree Of Parallelism'";


            String varname123 = "";
            varname123 = varname123 + "SELECT * from #ParameterDetails";

            sql = varname1 + Environment.NewLine +
                varname11 + Environment.NewLine +
                varname12 + Environment.NewLine +
                varname13 + Environment.NewLine +
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
                varname114 + Environment.NewLine +
                varname115 + Environment.NewLine +
                varname116 + Environment.NewLine +
                varname117 + Environment.NewLine +
                varname118 + Environment.NewLine +
                varname119 + Environment.NewLine +
                varname120 + Environment.NewLine +
                varname121 + Environment.NewLine +
                varname122 + Environment.NewLine +
                varname123 + Environment.NewLine;



            SqlCommand command;
            SqlDataReader dataReader;
            var connection = new SqlConnection(connectionString);
            PARAMETER_VALUES = "";

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    PARAMETER_VALUES += dataReader.GetValue(0) + " " + dataReader.GetValue(1)
                        + " " + dataReader.GetValue(2)
                        + " " + dataReader.GetValue(3)
                        + "\n";
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can not open connection ! ");
            }
            DatabaseEvaluator_BackgroundWorker.ReportProgress(100);
        }

        // https://dotnetfiddle.net/bFvxp8
        private string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Convert.FromBase64String(EVALUATOR_KEY); 
                aesAlg.IV = Convert.FromBase64String(EVALUATOR_KEY); 

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            Global_ProgressBar.Style = ProgressBarStyle.Marquee;
            Global_ProgressBar.MarqueeAnimationSpeed = 70;

            PathToXML_TextBox.Enabled = false;
            Browse_Button.Enabled = false;
            Start_Button.Enabled = false;

            try
            {
                // Use a background worker to check server connection
                DatabaseEvaluator_BackgroundWorker.RunWorkerAsync();
            }
            catch (IOException exception)
            {
                MessageBox.Show("File Permissions Error.");
            }
        }

        private void Browse_Button_Click(object sender, EventArgs e)
        {
            // http://stackoverflow.com/questions/4999734/how-to-add-browse-file-button-to-windows-form-using-c-sharp
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Browse for XML file";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "All files (*.*)|*.*";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                this.PathToXML_TextBox.Text = fdlg.FileName;
            }
        }




    }
}
