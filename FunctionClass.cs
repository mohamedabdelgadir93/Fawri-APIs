using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fawry.APIStantderd
{
    public class FunctionClass
    {
        public static string SendSms(string text, string MobileNo)
        {
            try
            {

                string msg = text;
                string tel = MobileNo.Contains("+") ? MobileNo.Replace("+", "") : MobileNo;
                string url = $"https://mshastra.com/sendurl.aspx?user=20098773&pwd=2d6kin&senderid=IBTIKARSOFT&mobileno={tel}&msgtext={text}&priority=High&CountryCode=ALL";
                //DownloadString(url);
                var r = string.Empty;
                using (var web = new System.Net.WebClient())
                    r = web.DownloadString(url);

                return "00";

            }
            catch (Exception ex)
            {
                return "01" + ex.Message;
            }
        }
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "Lamati#ibtikar";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {



            string EncryptionKey = "Lamati#ibtikar";
            byte[] cipherBytes = Convert.FromBase64String(cipherText.Replace(" ", "+"));
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;


        }
        public static object splitlca(string str)
        {
            if (str != "")
            {
                string[] tokens = str.Split(',');
                if (tokens.Length != 0)
                {
                    var lng = tokens[0];
                    var lan = tokens[1];
                    var dd = new
                    {
                        latitude = lng,
                        longitude = lan
                    };
                    return dd;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }


        }
        public static void InsertLog(string Message, int LogType)

        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var floder = "LamatiAPILog";
            var logPath = path + floder;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            string filename = logPath + "/API_Log_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            var fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
            var s = new StreamWriter(fs);
            s.BaseStream.Seek(0, SeekOrigin.End);
            s.WriteLine("Time : " + DateTime.Now.ToString());
            var logTypeString = LogType == 1 ? "Json Request " : "Json Response :";
            s.WriteLine(logTypeString);
            s.WriteLine(Message);
            s.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
            s.WriteLine("");
            s.Close();
        }
        public static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
     
    }
}
