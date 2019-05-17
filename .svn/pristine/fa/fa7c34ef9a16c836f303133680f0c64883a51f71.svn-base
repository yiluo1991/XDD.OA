using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace XDD.Web.Infrastructure
{
    public static class AES_Decrypt
    {
        public static string AES_decrypt(string encryptedDataStr, string key, string iv)  
        {  
            RijndaelManaged rijalg = new RijndaelManaged();  
            //-----------------    
            //设置 cipher 格式 AES-128-CBC    
  
            rijalg.KeySize = 128;  
  
            rijalg.Padding = PaddingMode.PKCS7;  
            rijalg.Mode = CipherMode.CBC;  
  
            rijalg.Key = Convert.FromBase64String(key);  
            rijalg.IV = Convert.FromBase64String(iv);  
  
  
            byte[] encryptedData= Convert.FromBase64String(encryptedDataStr);  
            //解密    
            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);  
  
            string result;  
              
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))  
            {  
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))  
                {  
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))  
                    {  
  
                        result= srDecrypt.ReadToEnd();  
                    }  
                }  
            }  
  
            return result;  
        }

    }
}