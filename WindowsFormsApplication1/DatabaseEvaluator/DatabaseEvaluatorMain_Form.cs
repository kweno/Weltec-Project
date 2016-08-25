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
            var fileLocation = this.PathToXML_TextBox.Text;

            // Decrypt the file.
            DecryptFile(fileLocation,
               @"Decrypted_SQLServer.xml");

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
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
            doc.Add(new Paragraph("SQL Server", boldFont));
            doc.Add(new Paragraph(" "));

            // http://www.mikesdotnetting.com/article/86/itextsharp-introducing-tables
            for (int i = 0; i < 20; i++)
            {
                PdfPTable table = new PdfPTable(4);
                PdfPCell cell = new PdfPCell(new Phrase("Issue 1: Install SQL 2008 R2 SP2 CU11", boldFont));
                cell.Colspan = 4;
                table.AddCell(cell);
                table.AddCell(new Phrase("Issue Type", boldFont));
                table.AddCell("Health");
                table.AddCell(new Phrase("Issue Severity", boldFont));
                table.AddCell("Critical");
                PdfPCell cell2 = new PdfPCell(new Phrase("Currently Installed version of SQL Sserver is SQL server 2008 R2 SP1 (10.50)", boldFont));
                cell2.Colspan = 4;
                table.AddCell(cell2);
                PdfPCell cell3 = new PdfPCell(new Phrase("This version is unsupported. We recommend installing the latest update of SQL Server, which is SQL server 2008 R2 SP2 CU11 http://support.microsoft.com/kb/2926028"));
                cell3.Colspan = 4;
                table.AddCell(cell3);
                doc.Add(table);
                doc.Add(new Paragraph(" "));
            }
            doc.Close();
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
