using System; 
using System.Data; 
using System.Configuration; 
using System.Collections; 
using System.Web; 
using System.Web.Security; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Web.UI.WebControls.WebParts; 
using System.Web.UI.HtmlControls; 
using System.Text; 
using System.Security.Cryptography; 
using System.IO;

namespace BP
{
    public partial class jiami 
    {
        private void Bind()
        {
            //¼ÓÃÜ 
            //this.Title =  DesEncrypt("pwd", "abcd1234");
            //this.Title += DesDecrypt(this.Title, "abcd1234");
            //Response.Write(DesDecrypt("2ikCw0TqKGo=", "abcd1234"));
        }
        /// <summary> 
        /// ¼ÓÃÜ×Ö·û´® 
        /// ×¢Òâ:ÃÜÔ¿±ØÐëÎª£¸Î»
        /// </summary> 
        /// <param name="strText">×Ö·û´®</param> 
        /// <param name="encryptKey">ÃÜÔ¿</param> 
        /// <param name="encryptKey">·µ»Ø¼ÓÃÜºóµÄ×Ö·û´®</param> 
        public string Lock(string inputString, string encryptKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(inputString);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (System.Exception error)
            {
                //return error.Message;
                return null;
            }
        }
        /// <summary> 
        /// ½âÃÜ×Ö·û´® 
        /// </summary> 
        /// <param name="this.inputString">¼ÓÁËÃÜµÄ×Ö·û´®</param> 
        /// <param name="decryptKey">ÃÜÔ¿</param> 
        /// <param name="decryptKey">·µ»Ø½âÃÜºóµÄ×Ö·û´®</param> 
        public string UnLock(string inputString, string decryptKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = new Byte[inputString.Length];
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(inputString);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetString(ms.ToArray());
            }
            catch (System.Exception error)
            {
                //return error.Message; 
                return null;
            }
        }
    }
}

