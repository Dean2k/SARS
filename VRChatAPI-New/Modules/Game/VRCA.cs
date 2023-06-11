using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRChatAPI_New.Models;

namespace VRChatAPI_New.Modules.Game
{
    public static class VRCA
    {
        public static async Task DownloadVrcaFile(string url, string fileLocation, System.Windows.Forms.ProgressBar progressBar)
        {
            url += "?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
            WebClient webClient = new WebClient();
            try
            {
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    SafeProgress(progressBar, e.ProgressPercentage);
                };
                webClient.BaseAddress = "https://api.vrchat.cloud";
                webClient.Headers.Add("Accept", $"*/*");
                webClient.Headers.Add("Cookie", $"auth={StaticGameValues.AuthKey}; twoFactorAuth={StaticGameValues.TwoFactorKey};");
                webClient.Headers.Add("X-MacAddress", StaticGameValues.MacAddress);
                webClient.Headers.Add("X-Client-Version", StaticGameValues.GameVersion);
                webClient.Headers.Add("X-Platform", "standalonewindows");
                webClient.Headers.Add("user-agent", "VRC.Core.BestHTTP");
                webClient.Headers.Add("X-Unity-Version", "2019.4.40f1");
                await webClient.DownloadFileTaskAsync(new Uri(url), fileLocation);

                webClient.Dispose();
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
                    MessageBox.Show("Login with a alt VRChat account in the settings page");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
                webClient.Dispose();
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
            using WebClient webClient = new WebClient();
            webClient.BaseAddress = "https://api.vrchat.cloud";
            webClient.Headers.Add("Accept", $"*/*");
            webClient.Headers.Add("Cookie", $"auth={StaticGameValues.AuthKey}; twoFactorAuth={StaticGameValues.TwoFactorKey}");
            webClient.Headers.Add("X-MacAddress", StaticGameValues.MacAddress);
            webClient.Headers.Add("X-Client-Version", StaticGameValues.GameVersion);
            webClient.Headers.Add("X-Platform", "standalonewindows");
            webClient.Headers.Add("user-agent", "VRC.Core.BestHTTP");
            webClient.Headers.Add("X-Unity-Version", "2019.4.40f1");
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
    }
}