using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AkhbaarAlYawm.Application.Helper
{
    public static class ArgaamAPIHelper
    {
        public static UserModel GetUserData()
        {
            UserModel retVal = new UserModel();
             string url = "http://localhost:52697/api/user";
            ArgaamJson stringJson = new ArgaamJson();
            ArgaamUserJson argaamUserJson = new ArgaamUserJson();
            if (!GetJSONResponseFromArgaam(url, ref stringJson, ref argaamUserJson))
            {
               // errMsg = getErrorMsg(stringJson.Data);
            }
            return argaamUserJson.Data;
        }
        private static bool GetJSONResponseFromArgaam(string url, ref ArgaamJson stringJson, ref ArgaamUserJson argaamUserJson )
        {

            bool isSuccess = false;
            string responseFromServer = GetResponseFromServer(url);
            if (responseFromServer != string.Empty)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                if (responseFromServer.Contains("##error##"))
                {
                    stringJson = serializer.Deserialize<ArgaamJson>(responseFromServer);
                }
                else
                {
                    isSuccess = true;
                    argaamUserJson.Data = serializer.Deserialize<UserModel>(responseFromServer);
                    if (argaamUserJson.Data == null)
                    {
                        isSuccess = false;
                        stringJson.Data = "##error##Error";
                    }
                }

            }

            return isSuccess;
        }

        private static string getErrorMsg(string errMsg)
        {
            if (errMsg != null && errMsg.Length > 9)
            {
                errMsg = errMsg.Substring(9);
            }
            else
            {
                errMsg = "Error";
            }
            return errMsg;
        }

        public static string GetResponseFromServer(string url)
        {
            string responseFromServer = "";
            HttpWebResponse response = null;
            StreamReader reader = null;
            Stream dataStream = null;

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                responseFromServer = responseFromServer.Trim();
            }
            catch
            {
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (dataStream != null)
                {
                    dataStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            return responseFromServer;
        }


    }
}
