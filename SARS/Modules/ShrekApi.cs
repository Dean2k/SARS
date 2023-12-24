using CoreSystem.Model;
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
            string apiUrl = "https://api.avatarrecovery.com/Avatar/GetKeyAvatar";
            if (altApi)
            {
                apiUrl = "https://api.avatarrecovery.com/Avatar/GetKeyAvatar";
            }
            if(!string.IsNullOrEmpty(customApi))
            {
                apiUrl = customApi;
            }

            if (!avatarSearch)
            {
                apiUrl = "https://api.avatarrecovery.com/Avatar/GetLazyWorlds";
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

        /// <summary>
        /// Gets download queue
        /// </summary>
        /// <returns></returns>
        public List<DownloadQueueList> DownloadQueueRefresh(GetRequests key)
        {
            string apiUrl = $"https://api.avatarrecovery.com/Avatar/GetMyRequests?key={key.Key}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"SARS" + coreApiVersion;
            string jsonPost = JsonConvert.SerializeObject(key.Key);
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
                    List<DownloadQueueList> avatarResponse = JsonConvert.DeserializeObject<List<DownloadQueueList>>(result);

                    return avatarResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<DownloadQueueList>();
            }
        }

        /// <summary>
        /// Gets download queue
        /// </summary>
        /// <returns></returns>
        public bool RequestAvatar(RequestAvatar avatar)
        {
            string apiUrl = $"https://api.avatarrecovery.com/Avatar/RequestDownload";

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
                    bool avatarResponse = JsonConvert.DeserializeObject<bool>(result);

                    return avatarResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public UserKey CheckKey(string key)
        {
            string apiUrl = $"https://api.avatarrecovery.com/Avatar/CheckLogin?search={key}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"SARS";
            string jsonPost = JsonConvert.SerializeObject(key);
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
                    UserKey avatarResponse = JsonConvert.DeserializeObject<UserKey>(result);

                    return avatarResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }

        public DatabaseStats DatabaseStats()
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string jsonString = webClient.DownloadString(new Uri($"https://api.avatarrecovery.com/Avatar/DatabaseStats"));
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

        public List<LanguageTranslation> LanguageList()
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string jsonString = webClient.DownloadString(new Uri($"https://translate.avatarrecovery.com/languages"));
                    List<LanguageTranslation> vrChatCacheResult = JsonConvert.DeserializeObject<List<LanguageTranslation>>(jsonString);
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