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
        //private string INSTANCE = "DESKTOP-FVFO8GL\\SQL2016N";
        private string INSTANCE = "";
        private string EVALUATOR_KEY = "AAECAwQFBgcICQoLDA0ODw==";
        private string FILE_NAME = "";
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
                Global_ProgressBar.Style = ProgressBarStyle.Continuous;
                Global_ProgressBar.MarqueeAnimationSpeed = 0;
                PathToXML_TextBox.Enabled = true;
                Browse_Button.Enabled = true;
                Start_Button.Enabled = true;
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

        private void addServerDetailsTable(out PdfPTable table)
        {
            table = new PdfPTable(2);
            float[] widths = new float[] { 300f, 500f };
            table.SetWidths(widths);
            //table.WidthPercentage = 70;
        }

        private void addNewItemToServerDetailsTable(string itemName, string itemValue, PdfPTable temp_table, out PdfPTable table)
        {
            table = temp_table;
            Phrase data_phrase = new Phrase(itemName);
            PdfPCell data_cell = new PdfPCell(data_phrase);
            table.AddCell(data_cell);
            data_phrase = new Phrase(itemValue);
            data_cell = new PdfPCell(data_phrase);
            table.AddCell(data_cell);
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
            try
            {
                // http://stackoverflow.com/questions/18219880/showdialog-and-ui-interaction-in-backgroundworker-thread
                this.Invoke((MethodInvoker)delegate
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.FileName = "SQLServer.pdf";
                    saveFileDialog1.Filter = "PDF File|*.pdf";
                    saveFileDialog1.Title = "Save a PDF File";
                    saveFileDialog1.ShowDialog();
                    FILE_NAME = saveFileDialog1.FileName;
                });


                // CREATE THE PDF
                // http://www.codeproject.com/Articles/686994/Create-Read-Advance-PDF-Report-using-iTextSharp-in
                FileStream fs = new FileStream(FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None);
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

                    doc.Add(new Paragraph(" "));
                    paragraph = new Paragraph("Server Details", overviewFont);
                    doc.Add(paragraph);
                    doc.Add(new Paragraph(" "));

                    PdfPTable server_details_table = null;
                    addServerDetailsTable(out server_details_table);

                    while (dataReader.Read())
                    {
                        PARAMETER_VALUES += dataReader.GetValue(0) + " " + dataReader.GetValue(1)
                            + "\n";
                        addNewItemToServerDetailsTable(dataReader.GetValue(0) + " ", dataReader.GetValue(1) + " ", server_details_table, out server_details_table);
                    }
                    doc.Add(server_details_table);
                    doc.Add(new Paragraph(" "));
                    dataReader.Close();
                    command.Dispose();
                    MessageBox.Show(PARAMETER_VALUES);
                }
                catch (Exception exception)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Can not open connection ! ");
                    });
                }


                // SCORECARD
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

                        var severity = dataReader.GetValue(5) + "";
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
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Can not open connection ! ");
                    });
                }


                doc.Close();
            }
            catch (IOException exception)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("File Permissions Error.");
                });
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
            FILE_NAME = "";

            Global_ProgressBar.Style = ProgressBarStyle.Marquee;
            Global_ProgressBar.MarqueeAnimationSpeed = 70;

            PathToXML_TextBox.Enabled = false;
            Browse_Button.Enabled = false;
            Start_Button.Enabled = false;

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

        /// <summary>
        /// Closes the Application
        /// </summary>
        private void Close_Button_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
            //Close();
            Application.Exit();
        }

        // for testing purposes
        private void button1_Click(object sender, EventArgs e)
        {
            DatabaseEvaluator_BackgroundWorker_DoWork(sender, null);
        }





    }
}
