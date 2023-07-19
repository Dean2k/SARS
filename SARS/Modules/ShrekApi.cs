using Newtonsoft.Json;
using SARS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace SARS.Modules
{
    internal class ShrekApi
    {
        private string coreApiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public ShrekApi()
        {
        }

        public void checkLogin()
        {
            //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(apiUrl + "&size=1");
            //webReq.UserAgent = $"SARS V" + coreApiVersion;
            //webReq.Method = "POST";
            //webReq.Headers.Add("X-API-Key: " + apiKey);
            //HttpWebResponse webResp = null;
            //try
            //{
            //    webResp = (HttpWebResponse)webReq.GetResponse();
            //}
            //catch (Exception ex)
            //{
            //    if (ex.Message.Contains("403"))
            //    {
            //        MessageBox.Show("API Key Invalid");
            //    }
            //    return;
            //}
        }

        /// <summary>
        /// Adds an avatar to the ARES API
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public List<AvatarModel> AvatarSearch(AvatarSearch avatar, bool altApi, string customApi, bool avatarSearch)
        {
            string apiUrl = "https://unlocked.modvrc.com/Avatar/GetKeyAvatar";
            if (altApi)
            {
                apiUrl = "https://unlocked.ares-mod.com/Avatar/GetKeyAvatar";
            }
            if(!string.IsNullOrEmpty(customApi))
            {
                apiUrl = customApi;
            }

            if (!avatarSearch)
            {
                apiUrl = "https://unlocked.modvrc.com/Avatar/GetLazyWorlds";
            }
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"SARS" + coreApiVersion;
            string jsonPost = JsonConvert.SerializeObject(avatar);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonPost);
            }
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    AvatarResponse avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(result);
                    if (avatarResponse.Banned)
                    {
                        MessageBox.Show("Your key has been banned");
                    }
                    else if(!avatarResponse.Authorized)
                    {
                        MessageBox.Show("The key you have entered is invalid");
                    }

                    return avatarResponse.Avatars;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("403"))
                {
                    MessageBox.Show("ERROR KEY INVALID");
                }
                else
                {
                    MessageBox.Show($"Unknown Error: {ex.Message}");
                }
                return new List<AvatarModel>();
            }
        }
    }
}