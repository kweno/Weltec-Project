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
        //private string INSTANCE = "B105-01";
        private string INSTANCE = "DESKTOP-FVFO8GL\\SQL2016N";
        private string EVALUATOR_KEY = "AAECAwQFBgcICQoLDA0ODw==";
        private string PARAMETER_VALUES = "";

        private Font whiteBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
        private BaseColor darkBlue = new BaseColor(79, 129, 188);
        private Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

        /// <summary>
        /// BackgroundWorker which does the Evaluation
        /// </summary>
        // http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners
        BackgroundWorker DatabaseEvaluator_BackgroundWorker;

        public DatabaseEvaluatorMain_Form()
        {
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("instance.txt");
            while ((line = file.ReadLine()) != null)
            {
                INSTANCE = line;
                counter++;
            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();

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
            try
            {
                MessageBox.Show(PARAMETER_VALUES);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = "SQLServer.pdf";
                saveFileDialog1.Filter = "PDF File|*.pdf";
                saveFileDialog1.Title = "Save a PDF File";
                saveFileDialog1.ShowDialog();

                // CREATE THE PDF
                // http://www.codeproject.com/Articles/686994/Create-Read-Advance-PDF-Report-using-iTextSharp-in
                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                Document doc = new Document();
                PdfWriter pdfwriter = PdfWriter.GetInstance(doc, fs);
                doc.Open();



                // CREATE THE TITLE PAGE
                // http://stackoverflow.com/questions/11854726/adding-absolute-positioned-text
                PdfContentByte cb = pdfwriter.DirectContent;
                cb.BeginText();
                BaseFont f_cn = BaseFont.CreateFont("c:\\windows\\fonts\\calibri.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetFontAndSize(f_cn, 10);
                cb.SetTextMatrix(180, 290);  //(xPos, yPos)
                cb.ShowText("Created By: SQL Server Team");
                cb.SetTextMatrix(180, 280);  //(xPos, yPos)
                cb.ShowText("Date: " + DateTime.Now.ToShortDateString());
                var logo = Image.GetInstance(Properties.Resources.header_logo, System.Drawing.Imaging.ImageFormat.Png);
                logo.ScalePercent(75f);
                logo.SetAbsolutePosition(100, 500);
                cb.AddImage(logo);
                cb.EndText();



                // Add a new page to the pdf file
                // http://stackoverflow.com/questions/9236931/set-page-margins-with-itextsharp
                doc.SetMargins(60f, 60f, 60f, 60f);
                doc.NewPage();



                // CREATE 2ND PAGE
                // http://www.mikesdotnetting.com/article/83/lists-with-itextsharp
                var overviewFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLDOBLIQUE, 16);
                

                // https://mlichtenberg.wordpress.com/2011/04/13/using-c-and-itextsharp-to-create-a-pdf/
                List goals = new List(List.UNORDERED, 20f);
                goals.SetListSymbol("\u2022");
                goals.IndentationLeft = 30f;
                goals.Add(new ListItem("Assess risks and evaluate health of the SQL Server environment."));
                goals.Add(new ListItem(25, "Identify key areas where the environment deviates from Microsoft best practices and"
                                + " configuration guidance."));
                goals.Add(new ListItem(25, "Establish assessment results that can generate a remediation plan used to complete"
                                + " improvements to the health of the environment and to resolve or mitigate risks."));

                List phases = new List(List.UNORDERED, 20f);
                phases.SetListSymbol("\u2022");
                phases.IndentationLeft = 30f;
                phases.Add(new ListItem(25, "Environmental Assessment:   The SQL Server Team collects data from the environment"
                                                + " focusing on key known areas."));
                phases.Add(new ListItem(25, "Analysis and Reporting:   The SQL Server Team analyzes the results to compare against best practices,"
                                + " identify risks and health related problems, and prepares a findings report."));
                phases.Add(new ListItem(25, "Remediation Planning:   Once problems and risks have been discovered, a full remediation action"
                                + " plan should be established to assist in the effort to remediate and stabilize the environment."));

                Paragraph paragraph = new Paragraph("Database Evaluation Overview", overviewFont);
                doc.Add(paragraph);
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("Program Goals", boldFont));
                doc.Add(new Paragraph(" "));
                doc.Add(goals);
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("Program Phases", boldFont));
                doc.Add(new Paragraph(" "));
                doc.Add(phases);


                doc.NewPage();



                // CREATE 3RD PAGE
                paragraph = new Paragraph("Scorecard", overviewFont);
                doc.Add(paragraph);
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("The scorecards for the Evaluation Report are provided below. These show the state of the system with"
                                        + " respect to health(current issues) and risk(potential for future issues)."));
                doc.Add(new Paragraph(" "));


                var darkBlue = new BaseColor(79, 129, 188);
                var lightBlue = new BaseColor(219, 229, 241);
                var red = new BaseColor(255, 0, 0);
                var orange = new BaseColor(255, 153, 0);
                var yellow = new BaseColor(255, 255, 102);
                var green = new BaseColor(155, 187, 89);

                // SEVERITY TABLE
                PdfPTable severity_table = new PdfPTable(1);
                severity_table.WidthPercentage = 40;
                PdfPCell level_cell = new PdfPCell(new Phrase("Issue Severity Levels", whiteBoldFont));
                level_cell.BackgroundColor = darkBlue;
                level_cell.FixedHeight = 20f;
                severity_table.AddCell(level_cell);
                PdfPCell critical_cell = new PdfPCell(new Phrase("Critical"));
                critical_cell.BackgroundColor = red;
                severity_table.AddCell(critical_cell);
                PdfPCell high_cell = new PdfPCell(new Phrase("High"));
                high_cell.BackgroundColor = orange;
                severity_table.AddCell(high_cell);
                PdfPCell medium_cell = new PdfPCell(new Phrase("Medium"));
                medium_cell.BackgroundColor = yellow;
                severity_table.AddCell(medium_cell);
                PdfPCell low_cell = new PdfPCell(new Phrase("Low"));
                low_cell.BackgroundColor = lightBlue;
                severity_table.AddCell(low_cell);
                PdfPCell noissues_cell = new PdfPCell(new Phrase("No Issues"));
                noissues_cell.BackgroundColor = green;
                severity_table.AddCell(noissues_cell);
                doc.Add(severity_table);
                doc.Add(new Paragraph(" "));

                // LEGEND
                doc.Add(new Paragraph("The following legend will be used throughout the rest of this document: "));
                doc.Add(new Paragraph(" "));
                PdfPTable legend_table = new PdfPTable(2);
                legend_table.DefaultCell.Border = Rectangle.NO_BORDER;
                float[] scorecard_widths = new float[] { 50f, 500f };
                legend_table.SetWidths(scorecard_widths);
                legend_table.WidthPercentage = 95;

                var success = Image.GetInstance(Properties.Resources.success, System.Drawing.Imaging.ImageFormat.Png);
                success.ScalePercent(2f);
                var success_cell = new PdfPCell(success);
                success_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                success_cell.PaddingTop = 3f;
                success_cell.Border = Rectangle.NO_BORDER;
                legend_table.AddCell(success_cell);
                Phrase scorecard_phrase = new Phrase("Indicates that there are no issues in this item");
                PdfPCell scorecard_cell = new PdfPCell(scorecard_phrase);
                scorecard_cell.PaddingLeft = 20f;
                scorecard_cell.Border = Rectangle.NO_BORDER;
                legend_table.AddCell(scorecard_cell);

                var error = Image.GetInstance(Properties.Resources.error, System.Drawing.Imaging.ImageFormat.Png);
                error.ScalePercent(2f);
                var error_cell = new PdfPCell(error);
                error_cell.HorizontalAlignment = Element.ALIGN_CENTER;
                error_cell.PaddingTop = 3f;
                error_cell.Border = Rectangle.NO_BORDER;
                legend_table.AddCell(error_cell);
                scorecard_phrase = new Phrase("Indicates that there are issues in this item");
                scorecard_cell = new PdfPCell(scorecard_phrase);
                scorecard_cell.PaddingLeft = 20f;
                scorecard_cell.Border = Rectangle.NO_BORDER;
                legend_table.AddCell(scorecard_cell);

                doc.Add(legend_table);

                // SERVER DETAILS
                string script = File.ReadAllText(@"FinalEvaluator.sql");
                var connectionString = "Data Source=" + INSTANCE + ";" +
                                        "Integrated Security=SSPI;";
                SqlCommand command;
                SqlDataReader dataReader;
                var connection = new SqlConnection(connectionString);
                connection.Open();

                command = new SqlCommand(script, connection);
                command.ExecuteNonQuery();

                script = "SELECT * FROM [#ServerDetails]";

                try
                {

                    command = new SqlCommand(script, connection);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        PARAMETER_VALUES += dataReader.GetValue(0) + " " + dataReader.GetValue(1)
                            + "\n";
                    }
                    dataReader.Close();
                    command.Dispose();
                    MessageBox.Show(PARAMETER_VALUES);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can not open connection ! ");
                }



                doc.NewPage();




                // CREATE 4TH PAGE
                paragraph = new Paragraph("Consolidated Scorecard", overviewFont);
                doc.Add(paragraph);
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("This scorecard gives an executive level summary of the issues discovered."));
                doc.Add(new Paragraph(" "));

                // 4TH PAGE INSTANCE
                PdfPTable server_table = null;
                addNewTable("SQL Server Instance", out server_table);

                addNewHeaderTable("Installation", server_table, out server_table);
                addNewItemTable("SQL Server Software Installation Drive", server_table, true, out server_table);
                addNewItemTable("SQL Server Version and Service Pack", server_table, true, out server_table);

                addNewHeaderTable("Configuration", server_table, out server_table);
                addNewItemTable("Max Degree Of Parallelism", server_table, true, out server_table);
                addNewItemTable("Memory", server_table, true, out server_table);
                addNewItemTable("Enable Traceflag 2371, 1117 and 1118", server_table, true, out server_table);
                addNewItemTable("Default Index Fill Factor", server_table, true, out server_table);

                addNewHeaderTable("Security", server_table, out server_table);
                addNewItemTable("Server Authentication", server_table, true, out server_table);
                addNewItemTable("SQL Server Network Port", server_table, true, out server_table);

                doc.Add(server_table);
                doc.Add(new Paragraph(" "));


                // 4TH PAGE DATABSE
                PdfPTable database_table = null;
                addNewTable("SQL Server Database", out database_table);

                addNewHeaderTable("Implemetation", database_table, out database_table);
                addNewItemTable("SQL *.MDF, *.NDF and *.LDF Log File Placement", database_table, false, out database_table);

                addNewHeaderTable("Configuration Options", database_table, out database_table);
                addNewItemTable("Recovery Model", database_table, false, out database_table);
                addNewItemTable("Compatibility Level", database_table, false, out database_table);
                addNewItemTable("Is RCSI Enabled (Read Committed Snapshot Isolation)", database_table, false, out database_table);
                addNewItemTable("Do All Tables have Clustered Indexes", database_table, false, out database_table);
                addNewItemTable("Database Auto Growth", database_table, false, out database_table);
                addNewItemTable("Auto Create Statistics", database_table, false, out database_table);
                addNewItemTable("Auto Shrink", database_table, false, out database_table);
                addNewItemTable("Auto Update Statistics", database_table, false, out database_table);

                addNewHeaderTable("Maintenance", database_table, out database_table);
                addNewItemTable("Daily Index Rebuild", database_table, false, out database_table);
                addNewItemTable("Daily Database Full Backup", database_table, false, out database_table);

                addNewHeaderTable("Security", database_table, out database_table);
                addNewItemTable("Blank SQL 'SA' Password", database_table, false, out database_table);
                addNewItemTable("Blank Server Administrator", database_table, false, out database_table);
                addNewItemTable("Domain Accounts used for SQL Services", database_table, false, out database_table);

                doc.Add(database_table);
                doc.Add(new Paragraph(" "));



                doc.NewPage();


                // 5TH PAGE
                doc.Add(new Paragraph("SQL Server", boldFont));
                doc.Add(new Paragraph(" "));
                    
                script = "SELECT * from #ParameterDetails ORDER BY PassValue,IssueSeverity, ParameterName";

                try
                {

                    command = new SqlCommand(script, connection);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        PARAMETER_VALUES += dataReader.GetValue(0) + " " + dataReader.GetValue(1)
                            + "\n";
                        // http://www.mikesdotnetting.com/article/86/itextsharp-introducing-tables
                        PdfPTable table = new PdfPTable(4);
                        table.WidthPercentage = 90;
                        PdfPCell header_cell = new PdfPCell(new Phrase("Issue: " + dataReader.GetValue(1), whiteBoldFont));
                        header_cell.Colspan = 4;
                        header_cell.BackgroundColor = darkBlue;
                        table.AddCell(header_cell);
                        PdfPCell issueTypeHeader_cell = new PdfPCell(new Phrase("Issue Type", boldFont));
                        issueTypeHeader_cell.BackgroundColor = lightBlue;
                        table.AddCell(issueTypeHeader_cell);
                        PdfPCell issueType_cell = new PdfPCell(new Phrase(dataReader.GetValue(4) + " "));
                        issueType_cell.BackgroundColor = lightBlue;
                        table.AddCell(issueType_cell);
                        PdfPCell issueSeverityHeader_cell = new PdfPCell(new Phrase("Issue Severity", boldFont));
                        issueSeverityHeader_cell.BackgroundColor = lightBlue;
                        table.AddCell(issueSeverityHeader_cell);

                        var severity = dataReader.GetValue(5) +  "";
                        if ("high".Equals(severity.ToLower()))
                            table.AddCell(high_cell);
                        if ("medium".Equals(severity.ToLower()))
                            table.AddCell(medium_cell);
                        if ("low".Equals(severity.ToLower()))
                            table.AddCell(low_cell);
                        if ("noissues".Equals(severity.ToLower()))
                            table.AddCell(noissues_cell);

                        addNewEvaluation("Problem", dataReader.GetValue(6) + " ", table, out table);
                        addNewEvaluation("Recommendation", dataReader.GetValue(7) + " ", table, out table);
                        addNewEvaluation("Why", dataReader.GetValue(8) + " ", table, out table);
                        addNewEvaluation("Reference 1", dataReader.GetValue(9) + " ", table, out table);
                        addNewEvaluation("Reference 2", dataReader.GetValue(10) + " ", table, out table);

                        doc.Add(table);
                        doc.Add(new Paragraph(" "));

                    }
                    MessageBox.Show(PARAMETER_VALUES);
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can not open connection ! ");
                }


                doc.Close();

                Global_ProgressBar.Style = ProgressBarStyle.Continuous;
                Global_ProgressBar.MarqueeAnimationSpeed = 0;
                PathToXML_TextBox.Enabled = true;
                Browse_Button.Enabled = true;
                Start_Button.Enabled = true;
            }
            catch (IOException exception)
            {
                MessageBox.Show("File Permissions Error.");
            }
        }

        private void addNewEvaluation(string header, string details, PdfPTable temp_table, out PdfPTable table)
        {
            table = temp_table;
            PdfPCell bold_header_cell = new PdfPCell(new Phrase(header, boldFont));
            bold_header_cell.Colspan = 4;
            table.AddCell(bold_header_cell);
            PdfPCell details_cell = new PdfPCell(new Phrase(details));
            details_cell.Colspan = 4;
            table.AddCell(details_cell);
        }

        private void addNewTable(string tableName, out PdfPTable table)
        {
            table = new PdfPTable(2);
            float[] widths = new float[] { 900f, 200f };
            table.SetWidths(widths);
            table.WidthPercentage = 95;
            PdfPCell header_cell = new PdfPCell(new Phrase(tableName, whiteBoldFont));
            header_cell.Colspan = 2;
            header_cell.BackgroundColor = darkBlue;
            header_cell.FixedHeight = 20f;
            table.AddCell(header_cell);
        }

        private void addNewHeaderTable(string headerName, PdfPTable temp_table, out PdfPTable table)
        {
            table = temp_table;
            PdfPCell database_main_cell = new PdfPCell(new Phrase(headerName));
            database_main_cell.Colspan = 2;
            table.AddCell(database_main_cell);
        }

        private void addNewItemTable(string itemName, PdfPTable temp_table, bool pass, out PdfPTable table)
        {
            table = temp_table;
            Phrase data_phrase = new Phrase(itemName);
            PdfPCell data_cell = new PdfPCell(data_phrase);
            data_cell.PaddingLeft = 20f;
            table.AddCell(data_cell);
            Image status = null;
            if (pass)
                status = Image.GetInstance(Properties.Resources.success, System.Drawing.Imaging.ImageFormat.Png);
            else
                status = Image.GetInstance(Properties.Resources.error, System.Drawing.Imaging.ImageFormat.Png);
            status.ScalePercent(2f);
            data_cell = new PdfPCell(status);
            data_cell.HorizontalAlignment = Element.ALIGN_CENTER;
            data_cell.PaddingTop = 3f;
            table.AddCell(data_cell);
        }


        private void DatabaseEvaluator_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var fileLocation = this.PathToXML_TextBox.Text;

            // Decrypt the bytes to a string.
            // http://stackoverflow.com/questions/2030847/best-way-to-read-a-large-file-into-a-byte-array-in-c
            string decrypted = DecryptStringFromBytes_Aes(File.ReadAllBytes(fileLocation));

            Console.WriteLine(decrypted);
            //MessageBox.Show(decrypted);

            if (!Directory.Exists(@"C:\XML"))
            {
                Directory.CreateDirectory(@"C:\XML");
            }
            System.IO.File.WriteAllText(@"C:\XML\SQLServer.xml", decrypted);

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


            // Use a background worker to check server connection
            DatabaseEvaluator_BackgroundWorker.RunWorkerAsync();

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

        private void button1_Click(object sender, EventArgs e)
        {
            DatabaseEvaluator_BackgroundWorker_RunWorkerCompleted(sender, null);
        }

        /// <summary>
        /// Closes the Application
        /// </summary>
        private void Close_Button_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
            //Close();
            Application.Exit();
        }
    }
}
