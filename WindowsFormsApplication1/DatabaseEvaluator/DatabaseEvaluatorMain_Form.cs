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
        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
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

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

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
        BackgroundWorker ClientApplication_BackgroundWorker;

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
                string original = "Here is some data to encrypt!";
                // Create a new instance of the AesCryptoServiceProvider
                // class.  This generates a new key and initialization 
                // vector (IV).
                using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                {
                    Console.WriteLine("Original:   {0} \n", original);
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes_Aes(original, Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw=="), Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw=="));
                    Console.WriteLine("Encrypted:   {0} \n", System.Text.Encoding.UTF8.GetString(encrypted));
                    // Decrypt the bytes to a string.
                    string decrypted = DecryptStringFromBytes_Aes(encrypted, Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw=="), Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw=="));

                    //Display the original data and the decrypted data.                   
                    Console.WriteLine("Decrypted Trip: {0}", decrypted);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.Message);
            }

            // Must be 64 bits, 8 bytes.
            // Distribute this key to the user who will decrypt this file.
            string sSecretKey;

            // Get the Key for the file to Encrypt.
            sSecretKey = GenerateKey();

            //// Define a byte array.
            //byte[] bytes = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            //// Convert the array to a base 64 sring.
            //String s = Convert.ToBase64String(bytes);
            //// Restore the byte array.
            //sSecretKey = ASCIIEncoding.ASCII.GetString(bytes);

            // For additional security Pin the key.
            //GCHandle gch = GCHandle.Alloc(sSecretKey, GCHandleType.Pinned);

            var fileLocation = this.textBox1.Text;

            // Decrypt the file.
            //DecryptFile(fileLocation,
            //   @"Decrypted_SQLServer.xml",
            //   sSecretKey);

            // Remove the Key from memory. 
            //ZeroMemory(gch.AddrOfPinnedObject(), sSecretKey.Length * 2);
            //gch.Free();

            // http://www.codeproject.com/Articles/686994/Create-Read-Advance-PDF-Report-using-iTextSharp-in
            FileStream fs = new FileStream("SQLServer.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
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
                this.textBox1.Text = fdlg.FileName;
            }
        }
    }
}
