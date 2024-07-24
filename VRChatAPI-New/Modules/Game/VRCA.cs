using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRChatAPI_New.Models;

namespace VRChatAPI_New.Modules.Game
{
    public static class VRCA
    {

        public static async Task DownloadVrcaFile(string url, string fileLocation, ProgressBar progressBar, bool bypass)
        {
            if (url.StartsWith("http"))
            {
                using WebClient webClient = SetupWebClient(bypass);
                try
                {
                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        SafeProgress(progressBar, e.ProgressPercentage);
                    };
                    await webClient.DownloadFileTaskAsync(new Uri(url), fileLocation);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (ex.Message.Contains("403"))
                    {
                        MessageBox.Show("Error downloading Avatar, its likely that the account you are using has been banned.\nPlease login with a new account or try another avatar.");
                    }
                    else if (ex.Message.Contains("404"))
                    {
                        MessageBox.Show("Avatar has been removed from VRChat servers or this version doesn't exist");
                    }
                    else if (ex.Message.Contains("401"))
                    {
                        MessageBox.Show("Login with a alt VRChat account in the Accounts Tab");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                    try
                    {
                        File.Delete(fileLocation);
                    }
                    catch { }
                }
            }
            else
            {
                if (!File.Exists(fileLocation))
                {
                    File.Copy(url, fileLocation);
                    ShowSelectedInExplorer.FileOrFolder(fileLocation);
                }
                else
                {
                }
            }
        }

        private static void SafeProgress(ProgressBar progress, int value)
        {
            try
            {
                if (progress.InvokeRequired)
                {
                    progress.Invoke((MethodInvoker)delegate
                    {
                        progress.Value = value;
                    });
                }
            }
            catch { }
        }

        public static VRChatFileInformation GetVersions(string url)
        {
            using WebClient webClient = SetupWebClient(false);
            try
            {
                string web = webClient.DownloadString(url);
                VRChatFileInformation items = JsonConvert.DeserializeObject<VRChatFileInformation>(web);
                return items;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return null;
                //skip as its likely avatar is been yeeted from VRC servers
            }
        }

        private static WebClient SetupWebClient(bool bypass)
        {
            WebClient webClient = new WebClient();
            if (!bypass)
            {
                webClient.BaseAddress = "https://api.vrchat.cloud";
                webClient.Headers.Add("Accept", $"*/*");
                webClient.Headers.Add("Cookie", $"auth={StaticGameValues.AuthKey}; twoFactorAuth={StaticGameValues.TwoFactorKey};");
                webClient.Headers.Add("X-MacAddress", StaticGameValues.MacAddress);
                webClient.Headers.Add("X-Client-Version", StaticGameValues.GameVersion);
                webClient.Headers.Add("X-Platform", "standalonewindows");
                webClient.Headers.Add("X-GameServer-Version", StaticGameValues.ServerVersion);
                webClient.Headers.Add("User-Agent", "VRC.Core.BestHTTP");
                webClient.Headers.Add("X-Unity-Version", StaticGameValues.UnityVersion);
                webClient.Headers.Add("X-Store", StaticGameValues.Store);
                if(string.IsNullOrEmpty(StaticGameValues.MacAddress) || string.IsNullOrEmpty(StaticGameValues.GameVersion) || string.IsNullOrEmpty(StaticGameValues.ServerVersion) || string.IsNullOrEmpty(StaticGameValues.UnityVersion) || string.IsNullOrEmpty(StaticGameValues.Store))
                {
                    MessageBox.Show("A needed value is blank, please restart ARC");
                    return null;
                }
            }

            return webClient;
        }
    }
}