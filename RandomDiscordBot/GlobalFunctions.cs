using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RandomDiscordBot
{
    public class GlobalFunctions
    {
        public static string[] explode(string str, string delimitter)
        {
            return str.Split(new string[] { delimitter }, StringSplitOptions.None);
        }
        public static JObject parseJson(string data)
        {
            return (JObject)JsonConvert.DeserializeObject(data, new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
        }
        public static string FormatNumber(string number)
        {
            string num = number;
            char[] arr = num.ToCharArray();
            string num2 = "";
            int curplace = 0;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                curplace++;
                num2 += arr[i];
                if (curplace >= 3)
                {
                    curplace = 0;
                    if (i > 0)
                        num2 += ",";
                }
            }
            char[] n2 = num2.ToCharArray();
            Array.Reverse(n2);
            num2 = new string(n2);
            return num2;
        }

        public static string GetMD5Hash(string text)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
