using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Security.Cryptography;

namespace DatabaseEvaluator
{

    public partial class DatabaseEvaluatorMain_Form : Form
    {
        // https://support.microsoft.com/en-nz/kb/307010
        // https://dotnetfiddle.net/bFvxp8
        // http://stackoverflow.com/questions/2919228/specified-key-is-not-a-valid-size-for-this-algorithm
        private void DecryptFile(string sInputFilename,
           string sOutputFilename)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                //Create a file stream to read the encrypted file back.
                FileStream fsread = new FileStream(sInputFilename,
               FileMode.Open,
               FileAccess.Read);
                aesAlg.Key = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
                aesAlg.IV = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
                // Create a decrytor to perform the stream transform.
                ICryptoTransform desdecrypt = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
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
        }

        /// The backgroundworker object on which the time consuming operation shall be executed
        /// http://www.codeproject.com/Articles/99143/BackgroundWorker-Class-Sample-for-Beginners
        BackgroundWorker DatabaseEvaluator_BackgroundWorker;

        public DatabaseEvaluatorMain_Form()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            try
            {
                var fileLocation = this.PathToXML_TextBox.Text;

                // Decrypt the file.
                DecryptFile(fileLocation,
                   @"Decrypted_SQLServer.xml");

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
            }
            catch (IOException exception)
            {
                MessageBox.Show("File Permissions Error.");
            }
            catch (CryptographicException exception)
            {
                MessageBox.Show("Invalid Dump File.");
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
