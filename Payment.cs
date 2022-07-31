using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fawry.APIStantderd
{
    public class Payment
    {
        //private readonly AppSettings _appSettings;

        //public Payment(IOptions<AppSettings> appSettings)
        //{
        //    _appSettings = appSettings.Value;
        //}
        public string DoPayment(string TransactionId, string Amount
            , string Currency, string first_name, string last_name, string address, string city, string state, string zip, string Phoneno, string customerEmail, string country, out int Status, out string payid)
        {
            string strpipeSeperatedString = null;
            string finalUrl = "";
            string merchantIp;
            string hostName = Dns.GetHostName();
            merchantIp = Dns.GetHostEntry(hostName).AddressList[1].ToString();
            try
            {

                //String Terminal = _appSettings.terminal;
                //String password = _appSettings.password;
                //String secret = _appSettings.secret;
                //var baseAddress = _appSettings.url;
                String Terminal = "haypertech";// System.Configuration.ConfigurationManager.AppSettings.Get("terminal").ToString();
                String password = "haypertech@123";//System.Configuration.ConfigurationManager.AppSettings.Get("password").ToString();
                String secret = "1044af246938a68d613a51775ccd5f8a0dd4e8620286c1f18e004879a089d6d5";//System.Configuration.ConfigurationManager.AppSettings.Get("secret").ToString();
                var baseAddress = "https://payments-dev.urway-tech.com/URWAYPGService/transaction/jsonProcess/JSONrequest";//System.Configuration.ConfigurationManager.AppSettings.Get("url").ToString();
                //  String trackid = System.Configuration.ConfigurationManager.AppSettings.Get("trackid").ToString();
                strpipeSeperatedString = TransactionId + "|" + Terminal + "|" + password + "|" + secret + "|" + Amount + "|" + Currency;
                string strHash = sha256_hash(strpipeSeperatedString);
                JObject generatedJson = generateJson(country, first_name, last_name, address, city, state, zip, Phoneno, customerEmail, Terminal, password, secret, Amount, Currency, "1", strHash, merchantIp, TransactionId);
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
                string parsedContent = generatedJson.ToString();
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();
                string strcontentlength = Convert.ToString(response);
                //if (strcontentlength == null)
                //{
                //    Status = 3;
                //    // Label1.Text = "Wrong value entered";
                //}
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();

                //   var contentJson = await SendRequest(request);
                dynamic dvresponse = JsonConvert.DeserializeObject(content);
                string strTargetUrl = string.Empty;
                string strpayid = string.Empty;

                if (dvresponse["targetUrl"].Value != null)
                {
                    strTargetUrl = dvresponse["targetUrl"].Value;
                }
                if (dvresponse["payid"].Value != null && dvresponse["payid"].Value != "")
                {
                    strpayid = dvresponse["payid"].Value;
                }



                if (strTargetUrl != null && strTargetUrl != "")
                {
                    Status = 1;
                    payid = strpayid;
                    finalUrl = strTargetUrl + "?paymentid=" + strpayid;
                }
                else
                {
                    finalUrl = "";
                    payid = "";
                    Status = 2;
                }

                //    Response.Redirect(finalUrl);


                //}

            }

            catch (Exception Ex)
            {
                //Label1.Text = "The operation has timed out";
                Status = 2;
                payid = "";
                WriteErrorToFile("btnsubmit_Click: " + Ex.Message);


            }
            return finalUrl;
        }

        #region generateJson
        public JObject generateJson(String txtCountry, String txtfirst_name, String txtlast_name, String txtaddress, String txtcity, String txtstate, String txtzip, String txtPhoneno, String txtcustomerEmail, String Terminal, String password, String secret, String amount, String Currency, String Action, String strHash, String merchantIp, String Trackid)
        {
            JObject testJson = new JObject();

            try
            {
                testJson["country"] = txtCountry;
                testJson["First_name"] = txtfirst_name;
                testJson["Last_name"] = txtlast_name;
                //testJson["address"] = txtaddress;
                //testJson["city"] = txtcity;
                //testJson["State"] = txtstate;
                //testJson["Zip"] = txtzip;
                testJson["Phoneno"] = txtPhoneno;
                testJson["customerEmail"] = txtcustomerEmail;
                testJson["transid"] = "";
                testJson["terminalId"] = Terminal;
                testJson["password"] = password;
                testJson["Secret"] = secret;
                testJson["amount"] = amount;
                testJson["currency"] = Currency;
                testJson["action"] = Action;
                testJson["requestHash"] = strHash;
                testJson["merchantIp"] = merchantIp;
                testJson["trackid"] = Trackid;
                testJson["udf2"] = "https://fawry-invoices.com/PaymentResponse/UpdateStatus";



            }

            catch (Exception ex)
            {
            }
            return testJson;
        }
        #endregion

        #region Sha256hsh

        public static String sha256_hash(string value)
        {

            StringBuilder Sb = new StringBuilder();

            try
            {


                using (var hash = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                    foreach (Byte b in result)
                        Sb.Append(b.ToString("x2"));
                }
            }
            catch (Exception ex)
            {
            }
            return Sb.ToString();
        }
        #endregion

        #region Write Error Log

        public bool WriteErrorToFile(string sText)
        {
            try
            {

                string sFileName = "Error_" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt";
                string sMonth = "";
                string sFolder = "C:\\PG_Log\\";
                string sHeaderMessage = "PGLog " + DateTime.Now.ToString() + Environment.NewLine;

                if (System.IO.Directory.Exists(sFolder) == false)
                {
                    System.IO.Directory.CreateDirectory(sFolder);
                }
                if (!System.IO.File.Exists(sFolder + sFileName))
                {
                    sText = Environment.NewLine + sHeaderMessage + sText + Environment.NewLine;
                }
                else
                {
                    sText = Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + sText + Environment.NewLine;
                }

                StreamWriter str = new StreamWriter(sFolder + sFileName, true);
                str.Write(sText);
                str.Flush();
                str.Close();

                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
    }
}
