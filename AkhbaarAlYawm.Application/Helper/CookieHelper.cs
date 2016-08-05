using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace AkhbaarAlYawm.Application.Helper
{
    public static class CookieHelper
    {

        private const string COOKIE_FIELD_NAME = "ObjValue";
        private const string COOKIE_CRYPT_PWS = "FFA74384-A3E1-11E1-867C-39A06188709B";

        public static void SetObjectToCookie<T>(string key, T myObject)
        {
            if (myObject != null)
            {
                ////just test start
                //string strValue1 = myObject.SerializeObject();
                //string strValue2 = CompressAndSerializeObject<string>(strValue1);
                //string strValue3 = CompressAndSerializeObject<T>(myObject);
                ////T retVal = DecompressAndDeSerializeString<T>(strValue3);
                ////just test end


                string strValue = myObject.SerializeObject();
                AddStringToCookie(key, strValue);
            }
            else
            {
                AddStringToCookie(key, "");
            }
        }

        public static T GetObjectFromCookie<T>(string key)
        {
            T retVal = default(T);
            string strValue = GetStringFromCookie(key);
            if (strValue != "")
            {
                retVal = DeSerializeObject<T>(strValue);
                //retVal = DecompressAndDeSerializeString<T>(strValue);
            }
            return retVal;
        }


        private static string GetStringFromCookie(string key)
        {
            string retVal = "";
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[key];
            if (myCookie != null)
            {
                //string keyVal  = HttpUtility.UrlDecode(myCookie.Values[COOKIE_FIELD_NAME]);
                //retVal = CryptoHelper.Decrypt(keyVal ,COOKIE_CRYPT_PWS);
                //retVal = HttpUtility.UrlDecode(myCookie.Values[COOKIE_FIELD_NAME]);
                retVal = HttpUtility.UrlDecode(myCookie.Values[COOKIE_FIELD_NAME]);
            }
            return retVal;
        }

        private static void AddStringToCookie(string key, string strValue)
        {
            HttpCookie myCookie = new HttpCookie(key);

            myCookie.Values.Add(COOKIE_FIELD_NAME, HttpUtility.UrlEncode(strValue));

            myCookie.Expires = DateTime.Now.AddMonths(1);

            myCookie.HttpOnly = true;

            if (!ApplicationConstants.UseDomainlessCookie)
            {
                myCookie.Domain = ApplicationConstants.CON_SessionCookieDomain;
            }

            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        //internal static string CompressAndSerializeObject<T>(T sValue)
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    MemoryStream ms = new MemoryStream();
        //    bf.Serialize(ms, sValue);
        //    byte[] inbyt = ms.ToArray();
        //    System.IO.MemoryStream objStream = new MemoryStream();
        //    System.IO.Compression.DeflateStream objZS = new System.IO.Compression.DeflateStream(objStream,
        //            System.IO.Compression.CompressionMode.Compress);
        //    objZS.Write(inbyt, 0, inbyt.Length);
        //    objZS.Flush();
        //    objZS.Close();
        //    byte[] b = objStream.ToArray();
        //    // store as Base64String - tested. working . generate big string 672 b
        //    return Convert.ToBase64String(b, 0, b.Length); 

        //    //// store as string- tested. working . generate big string 252 b
        //    //return GetString(b);
        //}

        //internal static T DecompressAndDeSerializeString<T>(string sValue)
        //{
        //    //// process Base64String - tested. working 
        //    int mod4 = sValue.Length % 4;
        //    if (mod4 > 0)
        //    {
        //        sValue += new string('=', 4 - mod4);
        //    }
        //    sValue.Replace(' ', '+');
        //    byte[] bytCook = Convert.FromBase64String(sValue);

        //    //// process Base64String - Not tested. 
        //    //byte[] bytCook = GetBytes(sValue);

        //    MemoryStream inMs = new MemoryStream(bytCook);
        //    inMs.Seek(0, 0);
        //    DeflateStream zipStream = new DeflateStream(inMs, CompressionMode.Decompress, true);
        //    byte[] outByt = ReadFullStream(zipStream);
        //    zipStream.Flush();
        //    zipStream.Close();
        //    MemoryStream outMs = new MemoryStream(outByt);
        //    outMs.Seek(0, 0);
        //    BinaryFormatter bf = new BinaryFormatter();
        //    return (T)bf.Deserialize(outMs, null);
        //}

        //private static byte[] ReadFullStream(Stream stream)
        //{
        //    byte[] buffer = new byte[32768];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        while (true)
        //        {
        //            int read = stream.Read(buffer, 0, buffer.Length);
        //            if (read <= 0)
        //            {
        //                return ms.ToArray();
        //            }
        //            ms.Write(buffer, 0, read);
        //        }
        //    }
        //}

        //static byte[] GetBytes(string str)
        //{
        //    byte[] bytes = new byte[str.Length * sizeof(char)];
        //    System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        //    return bytes;
        //}

        //static string GetString(byte[] bytes)
        //{
        //    char[] chars = new char[bytes.Length / sizeof(char)];
        //    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        //    return new string(chars);
        //}

        internal static string SerializeObject<T>(this T toSerialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(toSerialize);
        }

        internal static T DeSerializeObject<T>(string objValue)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(objValue);
        }

    }
}
