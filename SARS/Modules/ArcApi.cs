using ARC.Models;
using ARC.Models.ExternalModels;
using ARC.Models.InternalModels;
using CoreSystem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace ARC.Modules
{
    public class ArcApi
    {
        public string ArcVersion { get; set; }

        public ArcApi(string arcVersion)
        {
            this.ArcVersion = arcVersion;
        }

        /// <summary>
        /// Adds an avatar to the ARES API
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public List<AvatarModel> AvatarSearch(AvatarSearch avatar, bool avatarSearch, CookieContainer cookies)
        {
            string apiUrl = "https://api.avatarrecovery.com/AuthAvatar/GetKeyAvatar";

            if (!avatarSearch)
            {
                apiUrl = "https://api.avatarrecovery.com/AuthAvatar/GetLazyWorlds";
            }
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"ARC" + ArcVersion;
            httpWebRequest.CookieContainer = cookies;
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
                    if (result.Contains("content=\"Discord\""))
                    {
                        MessageBox.Show("Your auth has expired, please relogin");
                        return new List<AvatarModel>();
                    }
                    AvatarResponse avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(result);
                    if (avatarResponse.Banned)
                    {
                        MessageBox.Show("Your key has been banned");
                        return new List<AvatarModel>();
                    }
                    else if (!avatarResponse.Authorized)
                    {
                        MessageBox.Show("The key you have entered is invalid");
                        return new List<AvatarModel>();
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
        public List<DownloadQueueList> DownloadQueueRefresh(GetRequests key, CookieContainer cookies)
        {
            string apiUrl = $"https://api.avatarrecovery.com/AuthAvatar/GetMyRequests?key={key.Key}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"SARS" + ArcVersion;
            httpWebRequest.CookieContainer = cookies;
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
        public bool RequestAvatar(RequestAvatar avatar, CookieContainer cookies)
        {
            string apiUrl = $"https://api.avatarrecovery.com/AuthAvatar/RequestDownload";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"SARS" + ArcVersion;
            httpWebRequest.CookieContainer = cookies;
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

        public UserKey CheckKey(string key)
        {
            string apiUrl = $"https://api.avatarrecovery.com/Avatar/CheckLogin?search={key}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"ARC";
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

        public CookieAuth GetCookie(string guid)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string jsonString = webClient.DownloadString(new Uri($"https://api.avatarrecovery.com/AuthAvatar/GetCookie?guid={guid}"));
                    CookieAuth cookieAuth = JsonConvert.DeserializeObject<CookieAuth>(jsonString);
                    return cookieAuth;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return new CookieAuth();
        }


    }
}