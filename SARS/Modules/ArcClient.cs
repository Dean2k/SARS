using MetroFramework.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using ARC.Models;
using ARC.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using VRChatAPI_New;
using VRChatAPI_New.Models;
using ARC.Models.ExternalModels;
using ARC.Models.InternalModels;
using System.Diagnostics;

namespace ARC.Modules
{
    public static class ArcClient
    {
        public static void GetLatestVersion()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData("https://avatarrecovery.com/Latest.txt");
            string version = System.Text.Encoding.UTF8.GetString(raw);
            Version latestVersion = new Version(version);

            if (Assembly.GetExecutingAssembly().GetName().Version < latestVersion)
            {
                MessageBox.Show($"You are running an out of date version of ARC please update to stay secure\nYour Version: {Assembly.GetExecutingAssembly().GetName().Version}\nLatest Version: {version}", "Out of date");
            }
            else if (Assembly.GetExecutingAssembly().GetName().Version > latestVersion)
            {
                MessageBox.Show($"Are you magic? you are running a later version than whats released!?!?!?!\nYour Version: {Assembly.GetExecutingAssembly().GetName().Version}\nLatest Version: {version}", "DA FUDGE");
            }
        }

        public static void GetClientVersion(MetroTextBox text, ConfigSave<Config> configSave)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData("https://avatarrecovery.com/Version.txt");

            text.Text = System.Text.Encoding.UTF8.GetString(raw);
            configSave.Config.ClientVersion = text.Text;

            raw = wc.DownloadData("https://avatarrecovery.com/VersionUpdated.txt");

            configSave.Config.ClientVersionLastUpdated = Convert.ToDateTime(System.Text.Encoding.UTF8.GetString(raw));
            configSave.Save();
        }

        public static void UnitySetup2019(ConfigSave<Config> configSave)
        {
            var unityPath = UnityRegistry2019();
            if (unityPath != null)
            {
                var dlgResult =
                    MessageBox.Show(
                        $"Possible unity 2019 path found, Location: '{unityPath + @"\Editor\Unity.exe"}' is this correct?",
                        "Unity", MessageBoxButtons.YesNo);
                if (dlgResult == DialogResult.Yes)
                {
                    if (File.Exists(unityPath + @"\Editor\Unity.exe"))
                    {
                        configSave.Config.UnityLocation2019 = unityPath + @"\Editor\Unity.exe";
                        configSave.Save();
                    }
                    else
                    {
                        MessageBox.Show("Someone didn't check because that file doesn't exist!");
                        SelectFile2019(configSave);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Please select unity.exe (2019), if you wanna skip this press Cancel");
                    SelectFile2019(configSave);
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select unity.exe (2019), if you wanna skip this press Cancel");
                SelectFile2019(configSave);
            }
        }

        public static void UnitySetup2022(ConfigSave<Config> configSave)
        {
            var unityPath = UnityRegistry2022();
            if (unityPath != null)
            {
                var dlgResult =
                    MessageBox.Show(
                        $"Possible unity 2022 path found, Location: '{unityPath + @"\Editor\Unity.exe"}' is this correct?",
                        "Unity", MessageBoxButtons.YesNo);
                if (dlgResult == DialogResult.Yes)
                {
                    if (File.Exists(unityPath + @"\Editor\Unity.exe"))
                    {
                        configSave.Config.UnityLocation2022 = unityPath + @"\Editor\Unity.exe";
                        configSave.Save();
                    }
                    else
                    {
                        MessageBox.Show("Someone didn't check because that file doesn't exist!");
                        SelectFile2022(configSave);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Please select unity.exe (2022), if you wanna skip this press Cancel");
                    SelectFile2022(configSave);
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select unity.exe (2022), if you wanna skip this press Cancel");
                SelectFile2022(configSave);
            }
        }

        public static void SelectFile2022(ConfigSave<Config> configSave)
        {
            var filePath = string.Empty;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Unity (Unity.exe)|Unity.exe";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Select Unity exe";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);

            if (!versionInfo.FileVersion.Contains("2022.3.6"))
            {
                MessageBox.Show($"Incorrect unity version, please select again.\nWanted 2022.3.6.x got {versionInfo.FileVersion}");
                return;
            }

            configSave.Config.UnityLocation2022 = filePath;
            configSave.Save();
        }

        private static string UnityRegistry2022()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Unity Technologies\Installer\Unity"))
                {
                    if (key == null) return null;
                    var version = key.GetValue("Version");
                    var o = key.GetValue("Location x64");
                    if (version != null && version.ToString() == "2022.3.6f1")
                    {
                        if (o != null) return o.ToString();
                    }  else
                    {
                        using (var key1 = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Unity Technologies\Installer\Unity 2022.3.6f1"))
                        {
                            if (key == null) return null;
                            var version1 = key1.GetValue("Version");
                            var o1 = key1.GetValue("Location x64");
                            if (version1 != null && version1.ToString() == "2022.3.6f1")
                            {
                                if (o1 != null) return o1.ToString();
                            }
                            else
                            {

                            }
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static void SelectFile2019(ConfigSave<Config> configSave)
        {
            var filePath = string.Empty;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Unity (Unity.exe)|Unity.exe";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Select Unity exe";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);

            if(!versionInfo.FileVersion.Contains("2019.4.31"))
            {
                MessageBox.Show($"Incorrect unity version, please select again.\nWanted 2019.4.31.x got {versionInfo.FileVersion}");
                return;
            }

            configSave.Config.UnityLocation2019 = filePath;
            configSave.Save();
        }

        private static string UnityRegistry2019()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Unity Technologies\Installer\Unity"))
                {
                    if (key == null) return null;
                    var version = key.GetValue("Version");
                    var o = key.GetValue("Location x64");
                    if (version != null && version.ToString() == "2019.4.31f1")
                    {
                        if (o != null) return o.ToString();
                    }
                    else
                    {
                        using (var key1 = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Unity Technologies\Installer\Unity 2019.4.31f1"))
                        {
                            if (key == null) return null;
                            var version1 = key1.GetValue("Version");
                            var o1 = key1.GetValue("Location x64");
                            if (version1 != null && version1.ToString() == "2019.4.31f1")
                            {
                                if (o1 != null) return o1.ToString();
                            }
                            else
                            {

                            }
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }


        public static VRChatFileInformation avatarVersionPc;
        public static VRChatFileInformation avatarVersionQuest;
        public static void AvatarSizeAndVersions(DataGridView avatarGrid, List<AvatarModel> avatars, NumericUpDown nmPcVersion, NumericUpDown nmQuestVersion, MetroTextBox txtAvatarSizePc, MetroTextBox txtAvatarSizeQuest)
        {
            if (avatarGrid.SelectedRows.Count == 1)
            {
                AvatarModel info = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarGrid.SelectedRows[0].Cells[3].Value.ToString().Trim());
                if (info == null || info.Avatar.AuthorId == "Unknown Cache")
                {
                    nmPcVersion.Value = 0;
                    nmQuestVersion.Value = 0;
                    try
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(info.Avatar.PcAssetUrl);
                        txtAvatarSizePc.Text = FormatSize(fi.Length);
                    }
                    catch
                    {
                        txtAvatarSizePc.Text = "Local PC";
                    }
                    txtAvatarSizeQuest.Text = "";
                    return;
                }
                var versions = AvatarFunctions.GetVersion(info.Avatar.PcAssetUrl, info.Avatar.QuestAssetUrl);
                avatarVersionPc = versions.Item3;
                avatarVersionQuest = versions.Item4;
                if (avatarVersionPc != null && info.Avatar.PcAssetUrl.StartsWith("http"))
                {
                    nmPcVersion.Maximum = versions.Item1;
                    nmPcVersion.Value = versions.Item1;
                    long fileSize = avatarVersionPc.Versions.FirstOrDefault(x => x.Version == nmPcVersion.Value).File.SizeInBytes;
                    txtAvatarSizePc.Text = FormatSize(fileSize);
                    UpdateFileSize(info.Avatar.AvatarId, fileSize);
                }
                else if (!info.Avatar.PcAssetUrl.StartsWith("http"))
                {
                    nmPcVersion.Maximum = 1;
                    nmPcVersion.Value = 1;
                    System.IO.FileInfo fi = new System.IO.FileInfo(info.Avatar.PcAssetUrl);
                    txtAvatarSizePc.Text = FormatSize(fi.Length);
                }
                else
                {
                    nmPcVersion.Maximum = 1;
                    nmPcVersion.Value = 1;
                    txtAvatarSizePc.Text = "Error";
                }
                if (avatarVersionQuest != null)
                {
                    nmQuestVersion.Maximum = versions.Item2;
                    nmQuestVersion.Value = versions.Item2;
                    txtAvatarSizeQuest.Text = FormatSize(avatarVersionQuest.Versions.FirstOrDefault(x => x.Version == nmQuestVersion.Value).File.SizeInBytes);
                }
                else
                {
                    nmQuestVersion.Maximum = 1;
                    txtAvatarSizeQuest.Text = "0MB";
                }
            }
        }

        // Load all suffixes in an array
        private static readonly string[] suffixes =
        { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatSize(Int64 bytes, bool includeMB = true)
        {
            try
            {
                int counter = 0;
                decimal number = (decimal)bytes;
                while (Math.Round(number / 1024) >= 1)
                {
                    number = number / 1024;
                    counter++;
                }
                if (includeMB)
                {
                    return string.Format("{0:n1}{1}", number, suffixes[counter]);
                }
                else
                {
                    return string.Format("{0:n1}", number);
                }
            }
            catch
            {
                return "0MB";
            }
        }
        public static void ThreadLoadImages(string userAgent, DataGridView avatarGrid, List<AvatarModel> avatars)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                for (int i = 0; i < avatarGrid.Rows.Count; i++)
                {
                    try
                    {
                        if (avatarGrid.Rows[i] != null)
                        {
                            string fileName = $"{StaticValues.ArcDocuments}\\images\\{avatarGrid.Rows[i].Cells[3].Value}.png";
                            if (!File.Exists(fileName))
                            {
                                if (avatarGrid.Rows[i].Cells[5].Value != null)
                                {
                                    if (!string.IsNullOrEmpty(avatarGrid.Rows[i].Cells[5].Value.ToString().Trim()))
                                    {
                                        try
                                        {
                                            AvatarModel info = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarGrid.Rows[i].Cells[3].Value.ToString().Trim());
                                            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(info.Avatar.ThumbnailUrl);
                                            myRequest.Method = "GET";
                                            myRequest.UserAgent = userAgent;
                                            myRequest.Accept = "*/*";
                                            myRequest.Headers["X-Unity-Version"] = StaticGameValues.UnityVersion ?? "2022.3.6f1-DWR";
                                            using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                                            {
                                                if (myResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    Bitmap bmp = new Bitmap(myResponse.GetResponseStream());
                                                    avatarGrid.Rows[i].Cells[0].Value = bmp;
                                                    bmp.Save(fileName, ImageFormat.Png);
                                                }
                                                else
                                                {
                                                    myRequest = (HttpWebRequest)WebRequest.Create(info.Avatar.ImageUrl);
                                                    using (HttpWebResponse myResponse2 = (HttpWebResponse)myRequest.GetResponse())
                                                    {
                                                        if (myResponse2.StatusCode == HttpStatusCode.OK)
                                                        {
                                                            Bitmap bmp = new Bitmap(myResponse2.GetResponseStream());
                                                            avatarGrid.Rows[i].Cells[0].Value = bmp;
                                                            bmp.Save(fileName, ImageFormat.Png);
                                                        }
                                                        else
                                                        {
                                                            Bitmap bmp = new Bitmap(Resources.No_Image);
                                                            avatarGrid.Rows[i].Cells[0].Value = bmp;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.ToString());
                                            try
                                            {
                                                Bitmap bmp = new Bitmap(Resources.No_Image);
                                                avatarGrid.Rows[i].Cells[0].Value = bmp;
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Bitmap bmp = new Bitmap(fileName);
                                avatarGrid.Rows[i].Cells[0].Value = bmp;
                            }
                        }
                    }
                    catch { }
                }
            });
        }

        public static void CleanHsb(ConfigSave<Config> configSave)
        {
            RandomFunctions.KillProcess("Unity Hub.exe");
            RandomFunctions.KillProcess("Unity.exe");
            RandomFunctions.tryDeleteDirectory($"{StaticValues.ArcDocuments}\\{configSave.Config.HotSwapName2022}");
            RandomFunctions.tryDeleteDirectory(@"C:\Users\" + Environment.UserName + $"\\AppData\\Local\\Temp\\DefaultCompany\\{configSave.Config.HotSwapName2022}");
            RandomFunctions.tryDeleteDirectory(@"C:\Users\" + Environment.UserName + $"\\AppData\\LocalLow\\DefaultCompany\\{configSave.Config.HotSwapName2022}");
        }

        public static void CleanHsb2019(ConfigSave<Config> configSave)
        {
            RandomFunctions.KillProcess("Unity Hub.exe");
            RandomFunctions.KillProcess("Unity.exe");
            RandomFunctions.tryDeleteDirectory($"{StaticValues.ArcDocuments}\\{configSave.Config.HotSwapName2019}");
            RandomFunctions.tryDeleteDirectory(@"C:\Users\" + Environment.UserName + $"\\AppData\\Local\\Temp\\DefaultCompany\\{configSave.Config.HotSwapName2019}");
            RandomFunctions.tryDeleteDirectory(@"C:\Users\" + Environment.UserName + $"\\AppData\\LocalLow\\DefaultCompany\\{configSave.Config.HotSwapName2019}");
        }

        public static void ClearOldViewer()
        {
            RandomFunctions.KillProcess("AssetViewer.exe");
            RandomFunctions.tryDeleteDirectory(StaticValues.VrcaViewer);
        }

        public static void ExtractViewer()
        {
            if (!Directory.Exists(StaticValues.VrcaViewer))
            {
                Directory.CreateDirectory(StaticValues.VrcaViewer);
            }
            if (!File.Exists(StaticValues.VrcaViewer + "AssetViewer.exe"))
            {
                ZipFile.ExtractToDirectory($@"{AppContext.BaseDirectory}\Assets\NewestViewer.zip", StaticValues.VrcaViewer);
            }
        }

        public static void ExtractRipper()
        {
            if (!Directory.Exists(StaticValues.AssetRipper))
            {
                Directory.CreateDirectory(StaticValues.AssetRipper);
            }
            if (!File.Exists(StaticValues.AssetRipper + "AssetRipper.exe"))
            {
                ZipFile.ExtractToDirectory($@"{AppContext.BaseDirectory}\Assets\AssetRipper.zip", StaticValues.AssetRipper);
            }
        }

        public static void CopyFiles(ConfigSave<Config> configSave)
        {
            try
            {
                File.Copy($@"{AppContext.BaseDirectory}\Template\Scene.unity", $"{StaticValues.ArcDocuments}\\{configSave.Config.HotSwapName2022}\\Assets\\Scene.unity", true);
                File.Copy($@"{AppContext.BaseDirectory}\Template\SceneCube.unity", $"{StaticValues.ArcDocuments}\\{configSave.Config.HotSwapName2022}\\Assets\\SceneCube.unity", true);
            }
            catch (Exception ex) { 
             
                Console.WriteLine(ex.Message);
            
            }
        }

        public static void CopyFiles2019(ConfigSave<Config> configSave)
        {
            try
            {
                File.Copy($@"{AppContext.BaseDirectory}\Template\Scene.unity", $"{StaticValues.ArcDocuments}\\{configSave.Config.HotSwapName2019}\\Assets\\Scene.unity", true);
                File.Copy($@"{AppContext.BaseDirectory}\Template\SceneCube.unity", $"{StaticValues.ArcDocuments}\\{configSave.Config.HotSwapName2019}\\Assets\\SceneCube.unity", true);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }
        }

        public static void UpdateFileSize(string avatarId, long size)
        {
            try
            {
                string apiUrl = "https://api.avatarrecovery.com/Avatar/AddSize";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.UserAgent = $"ARC" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                FileSizeUpdate fileSize = new FileSizeUpdate { AvatarId = avatarId, Size = size };
                string jsonPost = JsonConvert.SerializeObject(fileSize);
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
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unknown Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}