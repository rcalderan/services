using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using Microsoft.Win32;


namespace Services
{
    class Arquivos
    {
        private static string dRaiz = System.Windows.Forms.Application.StartupPath;
        private static string dirLog = @"\log";
        private static string dirSys = @"\sys";
        private static string securityKey = "nMlHvNcR";

        private static Dictionary<string,string> arqs =  new Dictionary<string,string>()
        { 
            {"log.txt",dirLog},
            {"erro.txt",dirLog},
            {"tp.txt",dirSys}
        };
        
        public Arquivos()
        {
            try
            {
                //checa diretorios
                /*if (!Directory.Exists(dRaiz))
                {
                    Directory.CreateDirectory(dRaiz);
                }
                if (!Directory.Exists(dRaiz + dirSys))
                {
                    Directory.CreateDirectory(dRaiz + dirSys);
                }
                if (!Directory.Exists(dRaiz + dirLog))
                {
                    Directory.CreateDirectory(dRaiz + dirLog);
                }*/

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }


        public static Dictionary<string,string> getDataFromReg()
        { //host, user, pass
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                string aux= (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\services", "data", null);
                if (aux != null)
                {
                    string decr = Decrypt(aux, true);
                    string[] s = decr.Split(' ');
                    result.Add("server", s[0]);
                    result.Add("user", s[1]);
                    result.Add("pass", s[2]);
                }
                else
                {
                    string[] s = {"192.168.0.10","root","33722363"};
                    string valor=Encrypt(s[0]+" "+s[1]+" "+s[2],true);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\services", "data", valor, RegistryValueKind.String);
                    result.Add("server", s[0]);
                    result.Add("user", s[1]);
                    result.Add("pass", s[2]);
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\services\settings", "confAoFecharCh", null);
                if (aux != null)
                {
                    result.Add("confAoFecharCh", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\services\settings", "confAoFecharCh", "1", RegistryValueKind.String);
                    result.Add("confAoFecharCh", "1");
                }
                
                return result;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }

        }

        public static void updateReg(string key,string value)
        {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\services\settings", key, value, RegistryValueKind.String);
        }

        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();

            // Get the key from config file

            //string securityKey = (string)settingsReader.GetValue("nMlHvNcR",typeof(String));//caso resolva utizar app.config file
            
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(securityKey);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            //System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
            //Get your key from config file to open the lock!
            //string key = (string)settingsReader.GetValue("SecurityKey",typeof(String));

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(securityKey);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        
        public static void SetDataToArq(string host,string user,string pass)
        {
            string[] r = new string[3];
            if (!File.Exists(dRaiz + @"\" + dirSys + @"\data"))
                File.CreateText(dRaiz + @"\" + dirSys + @"\data");

            string line = Encrypt(host + " " + user + " " + pass, true);
            using (StreamWriter sw = new StreamWriter(dRaiz + @"\" + dirSys + @"\data"))
            {
                sw.WriteLine(line);
                sw.Close();
            }

        }

        public static void setNmData(string host, string user, string pass)
        {
            string encrpted = Encrypt(host + " " + user + " " + pass, true);

            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {//caso admin, salve no reg 
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\services", "data", encrpted, RegistryValueKind.String);
            }

            //if (!File.Exists(dRaiz + @"\" + dirSys + @"\data"))
            //  File.CreateText(dRaiz + @"\" + dirSys + @"\data");

            using (StreamWriter sw = new StreamWriter(dRaiz + @"\" + dirSys + @"\data"))
            {
                sw.WriteLine(encrpted);
                sw.Close();
            }
        }
        public static string[] getNmData()
        {
            string decrpted = "";
            using (StreamReader sr = new StreamReader(dRaiz + @"\" + dirSys + @"\data"))
            {
                decrpted = sr.ReadLine();
                sr.Close();
            }
            decrpted = Decrypt(decrpted, true);
            return decrpted.Split(' ');
        }

        public static string[] getDataFromArq()
        {
            string[] r = new string[3];
            string line;
            if (!File.Exists(dRaiz + @"\" + dirSys + @"\data"))
            {
                File.CreateText(dRaiz + @"\" + dirSys + @"\data");
            }
            using (StreamReader sr = new StreamReader(dRaiz + @"\" + dirSys + @"\data"))
            {
                line = sr.ReadLine();
                sr.Close();
            }
            line = Decrypt(line, true);
            r = line.Split(' ');
            return r;

        }

        public List<string> getIpFromFile(string x)
        { //host, user, pass
            try
            {
                List<string> result = new List<string>();
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                //A 64 bit key and IV is required for this provider.
                //Set secret key For DES algorithm.
                DES.Key = ASCIIEncoding.ASCII.GetBytes(securityKey);
                //Set initialization vector.
                DES.IV = ASCIIEncoding.ASCII.GetBytes(securityKey);

                //Create a file stream to read the encrypted file back.
                FileStream fsread = new FileStream(dRaiz + @"\" + dirSys + @"\conf.nm",
                   FileMode.Open,
                   FileAccess.Read);
                //Create a DES decryptor from the DES instance.
                ICryptoTransform desdecrypt = DES.CreateDecryptor();
                //Create crypto stream set to read and do a 
                //DES decryption transform on incoming bytes.
                CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read);
                //Print the contents of the decrypted file.
                StreamWriter fsDecrypted = new StreamWriter(dRaiz + @"\" + dirSys + @"\conf2.nm");
                fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
                fsDecrypted.Flush();
                fsDecrypted.Close();
                using (StreamReader srw = new StreamReader(dRaiz + @"\" + dirSys + @"\conf2.nm"))
                    if (srw != null)
                    {
                        while (!srw.EndOfStream)
                            result.Add(srw.ReadLine());
                        srw.Close();
                        File.Delete(dRaiz + @"\" + dirSys + @"\conf2.nm");
                        return result;
                    }
                    else
                    {
                        File.Delete(dRaiz + @"\" + dirSys + @"\conf2.nm");
                        return null;
                    }    
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }

        }
         
        public string criaLog(string log, string tipo_de_log)
        {
            try
            {
                string path = dRaiz + dirLog + @"\log.txt";
                string linha = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "(" + tipo_de_log + ") -  " + log;
                List<string> linhas = new List<string>();
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                        linhas.Add(sr.ReadLine());
                    linhas.Add(linha);
                }
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (String ln in linhas)
                    {
                        sw.WriteLine(ln);
                    }
                }
                return "";
            }
            catch (Exception erro)
            {
                return erro.Message;
            }
        }

        public string logErro(string log)
        {
            try
            {
                string path = dRaiz + dirLog + @"\erro.txt";
                string linha = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " -  " + log;
                List<string> linhas = new List<string>();
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                        linhas.Add(sr.ReadLine());
                    linhas.Add(linha);
                    sr.Close();
                }
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (String ln in linhas)
                    {
                        sw.WriteLine(ln);
                    }
                    sw.Close();
                }
                return "";
            }
            catch (Exception erro)
            {
                return erro.Message;
            }
        }
        
        private static void EncryptData(String inName, String outName)
        {
            
            byte[] desKey = new byte[8], desIV = new byte[8];
            //Create the file streams to handle the input and output files.
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.
            byte[] bin = new byte[100]; //This is intermediate storage for the encryption.
            long rdlen = 0;              //This is the total number of bytes written.
            long totlen = fin.Length;    //This is the total length of the input file.
            int len;                     //This is the number of bytes to be written at a time.

            DES des = new DESCryptoServiceProvider();
            for (int i=0;i<8;i++)
            {
                desKey[i] = Convert.ToByte(securityKey[i]);
                desIV[i] = Convert.ToByte(securityKey[i]);
            }
            CryptoStream encStream = new CryptoStream(fout, des.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);


            //Read from the input file, then encrypt and write to the output file.
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }

            encStream.Close();
            fout.Close();
            fin.Close();
        }

        private static void EncryptFile(string sInputFilename,
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
            System.Security.Cryptography.ICryptoTransform desencrypt = DES.CreateEncryptor();
            System.Security.Cryptography.CryptoStream cryptostream = new System.Security.Cryptography.CryptoStream(fsEncrypted,
               desencrypt,
               System.Security.Cryptography.CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();
        }

        private static void DecryptFile(string sInputFilename,
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
            CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt,CryptoStreamMode.Read);
            //Print the contents of the decrypted file.
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
        } 
    }
}
