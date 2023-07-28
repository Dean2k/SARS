using Newtonsoft.Json;
using SARS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using VRChatAPI_New;

namespace SARS.Modules
{
    internal class ShrekApi
    {
        private string coreApiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public ShrekApi()
        {
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

        public DatabaseStats DatabaseStats()
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string jsonString = webClient.DownloadString(new Uri($"https://unlocked.modvrc.com/Avatar/DatabaseStats"));
                    DatabaseStats vrChatCacheResult = JsonConvert.DeserializeObject<DatabaseStats>(jsonString);
                    return vrChatCacheResult;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
    }
}