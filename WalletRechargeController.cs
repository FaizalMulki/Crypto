using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vigilance.Web;


namespace OrchestratorAsset.Web.Controllers
{

    public class WalletRechargeController : Controller
    {
        // GET: WalletRecharge
        public ActionResult Recharge()
        {
            return View();
        }


        public void GetData()
        {

            try
            {
                string ContactUs = "https://125.19.66.195/kbxwnetc/RechargeCustomerAccount";
                string MyProxyHostString = "172.16.240.153";
                int MyProxyPort = 8080;
                var request = (HttpWebRequest)WebRequest.Create(ContactUs);
                request.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                RequestClass obj = new RequestClass();

                obj.flag = "RECH";
                obj.etcCustId = "";
                obj.vrn = "KA19";
                obj.rechargeTxnid = "1234";
                obj.rechargeAmt = "120.00";

                string json = JsonConvert.SerializeObject(obj);
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                requestWriter.Write(json);
                requestWriter.Close();
                StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream());
                string responseData = responseReader.ReadToEnd();
                responseReader.Close();
                request.GetResponse().Close();
                // return "";
            }
            catch (Exception e)
            {
                var k = e.Message;
            }
        }



        public async Task<String> GetDetails()
        {
            string ContactUs = "https://125.19.66.195/kbxwnetc/RechargeCustomerAccount";
            string MyProxyHostString = "172.16.240.153";
            int MyProxyPort = 8080;
            //Bypass SSL Verification
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            var request = (HttpWebRequest)WebRequest.Create(ContactUs);
            request.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            RequestClass obj = new RequestClass();

            obj.flag = "RECH";
            obj.etcCustId = "";
            obj.vrn = "KA19";
            obj.rechargeTxnid = "1234";
            obj.rechargeAmt = "120.00";

            string json = JsonConvert.SerializeObject(obj);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync(ContactUs, obj);
            string result = response.Content.ReadAsStringAsync().Result;
            return null;
        }



        public string GetText()
        {
            // var str = "4cdf7b17b7db00d7911498dec913d3e4 1e55c4e950b772685ccfdb831c82fede SCQXvM7GeKxP0jLtX5xbuF0WvBC/C81wwxtYNduUe9lVzaYztaJ8ifivjaCBWd7O2zSa+/A+vtFfdSWSnN5+RcjWka42QQl4f+yZ8C1Y/efIsUlDVXBXmSEjSUp/4sflXNz7qg62Ka+atpj0aiG6QvU+T5tnafmsDhx/M3zE+Tg=";

            RequestClass obj = new RequestClass();
            obj.flag = "RECH";
            obj.etcCustId = "";
            obj.vrn = "KT01AA1152";
            obj.rechargeTxnid = "KARN000000000093674";
            obj.rechargeAmt = "500.00";

           string json = JsonConvert.SerializeObject(obj);
           var encryptedMessage= encryptMessage(json);
           var decrypMessage = decryptMessage(encryptedMessage);





            return decrypMessage;

        }


   



     


        public string generateKey(String password, byte[] saltBytes)
        {
           
            int iterations = 100;
            var rfc2898 =
            new System.Security.Cryptography.Rfc2898DeriveBytes(password, saltBytes, iterations);
            byte[] key = rfc2898.GetBytes(16);
            String keyB64 = Convert.ToBase64String(key);
            return keyB64;
        }

        public static byte[] hexStringToByteArray(string hexString)
        {

         

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;


        }


        static Random random = new Random();
        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }


        public string encryptMessage(string message)
        {
           
            String combineData = "";
            try
            {
                
                string passphrase = "46ea428a97ba4c3094fc66e112d1d678";
                string saltHex = GetRandomHexNumber(32);
                string ivHex = GetRandomHexNumber(32);
                byte[] salt = hexStringToByteArray(saltHex);
                byte[] iv = hexStringToByteArray(ivHex);

                string sKey = generateKey(passphrase, salt);
                byte[] keyBytes = System.Convert.FromBase64String(sKey);
                AesManaged aesCipher = new AesManaged();
               
                aesCipher.KeySize = 128;
                aesCipher.BlockSize = 128;
                aesCipher.Mode = CipherMode.CBC;
                aesCipher.Padding = PaddingMode.PKCS7;
                aesCipher.Key = keyBytes;

                byte[] b = System.Text.Encoding.UTF8.GetBytes(message);
                ICryptoTransform encryptTransform = aesCipher.CreateEncryptor(keyBytes, iv);
                byte[] ctext = encryptTransform.TransformFinalBlock(b, 0, b.Length);

                var plainTextBytes = System.Convert.ToBase64String(ctext);
              
                combineData = saltHex + " " + ivHex + " " + plainTextBytes;
                
            }
            catch (Exception e)
            {
               
            }
            combineData = combineData.Replace("\n", "").Replace("\t", "").Replace("\r", "");
            return combineData;
        }


        public string decryptMessage(string str)
        {
           
            string myKey = "46ea428a97ba4c3094fc66e112d1d678";
            string decrypted = null;
            try
            {
                if ((str != null) && (str.Contains(' ')))
                {
                    string salt = str.Split(' ')[0];
                    string iv = str.Split(' ')[1];
                    String encryptedText = str.Split(' ')[2];
                 
                    decrypted = DecryptAlter(salt, iv, myKey, encryptedText);
                    return decrypted;
                }
                else
                {
                    decrypted = str;
                    return decrypted;
                }
            }
            catch (Exception e)
            {

            }

            return decrypted;

        }


        public string DecryptAlter(string salt, string iv, string passphrase, string EncryptedText)
        {
            string decryptedValue = null;
            try
            {
                byte[] saltBytes = hexStringToByteArray(salt);

                string sKey = generateKey(passphrase, saltBytes);
                byte[] ivBytes = hexStringToByteArray(iv);


                byte[] keyBytes = System.Convert.FromBase64String(sKey);


                AesManaged aesCipher = new AesManaged();
                aesCipher.IV = ivBytes;
                aesCipher.KeySize = 128;
                aesCipher.BlockSize = 128;
                aesCipher.Mode = CipherMode.CBC;
                aesCipher.Padding = PaddingMode.PKCS7;
                byte[] b = System.Convert.FromBase64String(EncryptedText);
                ICryptoTransform decryptTransform = aesCipher.CreateDecryptor(keyBytes, ivBytes);
                byte[] plainText = decryptTransform.TransformFinalBlock(b, 0, b.Length);

                var res = System.Text.Encoding.UTF8.GetString(plainText);
                return res;
            }
            catch (Exception e)
            {
                var k = e.Message;
            }
            return "";
        }

        

        public class RequestClass
        {
            public string flag { get; set; }
            public string vrn { get; set; }
            public string etcCustId { get; set; }
            public string rechargeTxnid { get; set; }
            public string rechargeAmt { get; set; }
        }



      



        
    }


   






}

