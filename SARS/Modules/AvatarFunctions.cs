﻿using ARC.Models;
using ARC;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;
using VRChatAPI_New;
using VRChatAPI_New.Models;
using VRChatAPI_New.Modules.Game;
using ARC.Models.ExternalModels;
using Newtonsoft.Json;

namespace ARC.Modules
{
    internal static class AvatarFunctions
    {
        public static void ExtractHSB(string hotSwapName, bool bypassCheck)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (bypassCheck)
            {
                ExtractData(hotSwapName, filePath);
            }
            else if (!Directory.Exists($"{StaticValues.ArcDocuments}\\{hotSwapName}\\"))
            {
                ExtractData(hotSwapName, filePath);
            }
        }

        public static void ExtractHSB2019(string hotSwapName, bool bypassCheck)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (bypassCheck)
            {
                ExtractData2019(hotSwapName, filePath);
            }
            else if (!Directory.Exists($"{StaticValues.ArcDocuments}\\{hotSwapName}\\"))
            {
                ExtractData2019(hotSwapName, filePath);
            }
        }

        public static void ExtractHSB2022L(string hotSwapName, bool bypassCheck)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (bypassCheck)
            {
                ExtractData2022L(hotSwapName, filePath);
            }
            else if (!Directory.Exists($"{StaticValues.ArcDocuments}\\{hotSwapName}\\"))
            {
                ExtractData2022L(hotSwapName, filePath);
            }
        }

        public static void ExtractData2022L(string hotSwapName, string filePath)
        {
            ZipFile.ExtractToDirectory($@"{filePath}\Assets\ARC2022L.zip", $"{StaticValues.ArcDocuments}\\{hotSwapName}");
            try
            {
                string text = File.ReadAllText($"{StaticValues.ArcDocuments}\\{hotSwapName}\\ProjectSettings\\ProjectSettings.asset");
                text = text.Replace("newpack", hotSwapName);
                File.WriteAllText($"{StaticValues.ArcDocuments}{hotSwapName}\\ProjectSettings\\ProjectSettings.asset", text);
            }
            catch { }
        }

        public static void ExtractData(string hotSwapName, string filePath)
        {
            ZipFile.ExtractToDirectory($@"{filePath}\Assets\ARC.zip", $"{StaticValues.ArcDocuments}\\{hotSwapName}");
            try
            {
                string text = File.ReadAllText($"{StaticValues.ArcDocuments}\\{hotSwapName}\\ProjectSettings\\ProjectSettings.asset");
                text = text.Replace("TestingVCCHS", hotSwapName);
                File.WriteAllText($"{StaticValues.ArcDocuments}{hotSwapName}\\ProjectSettings\\ProjectSettings.asset", text);
            }
            catch { }
        }

        public static void ExtractData2019(string hotSwapName, string filePath)
        {
            ZipFile.ExtractToDirectory($@"{filePath}\Assets\ARC2019.zip", $"{StaticValues.ArcDocuments}\\{hotSwapName}");
            try
            {
                string text = File.ReadAllText($"{StaticValues.ArcDocuments}\\{hotSwapName}\\ProjectSettings\\ProjectSettings.asset");
                text = text.Replace("SARS", hotSwapName);
                File.WriteAllText($"{StaticValues.ArcDocuments}{hotSwapName}\\ProjectSettings\\ProjectSettings.asset", text);
            }
            catch { }
        }

        public static bool pcDownload = true;

        private static VRChatCacheResult GetDetails(string avatarId)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.BaseAddress = "https://api.vrchat.cloud";
                    webClient.Headers.Add("Accept", $"*/*");
                    webClient.Headers.Add("Cookie", $"auth={StaticValues.Config.Config.AuthKey}; twoFactorAuth={StaticValues.Config.Config.TwoFactor}");
                    webClient.Headers.Add("X-MacAddress", StaticGameValues.MacAddress);
                    webClient.Headers.Add("X-Client-Version",
                        StaticGameValues.GameVersion);
                    webClient.Headers.Add("X-Platform",
                        "standalonewindows");
                    webClient.Headers.Add("user-agent",
                        "VRC.Core.BestHTTP");
                    webClient.Headers.Add("X-Unity-Version",
                        "2022.3.22f1-DWRL");
                    string jsonString = webClient.DownloadString(new Uri($"https://api.vrchat.cloud/api/1/avatars/{avatarId}"));
                    VRChatCacheResult vrChatCacheResult = JsonConvert.DeserializeObject<VRChatCacheResult>(jsonString);
                    return vrChatCacheResult;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }

        public static async Task<bool> DownloadVrcaAsync(AvatarModel avatar, decimal pcVersion, decimal questVersion, Download download, bool both = false)
        {
            try
            {
                MessageBoxManager.Yes = "PC";
                MessageBoxManager.No = "Quest";
                MessageBoxManager.Register();
            }
            catch { }

            var temp = GetDetails(avatar.Avatar.AvatarId);


            if (temp == null && avatar.Avatar.PcAssetUrl.StartsWith("https"))
            {
                MessageBox.Show("Private avatar downloading is currently disabled to prevent bans");
                return false;
            }

            var filePath = $"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}.vrca";
            if (avatar.Avatar.PcAssetUrl.ToLower() != "none" && avatar.Avatar.QuestAssetUrl.ToLower() != "none" && avatar.Avatar.PcAssetUrl != null && avatar.Avatar.QuestAssetUrl != null)
            {
                if (both)
                {
                    Console.WriteLine("Starting Download");
                    if (avatar.Avatar.QuestAssetUrl.ToLower() != "none")
                    {
                        try
                        {
                            var version = avatar.Avatar.QuestAssetUrl.Split('/');
                            if (questVersion > 0)
                            {
                                version[7] = questVersion.ToString();
                            }
                            await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrca", "_quest.vrca"), download.downloadProgress, false));
                        }
                        catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.QuestAssetUrl, filePath.Replace(".vrca", "_quest.vrca"), download.downloadProgress, false)); }
                        ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrca", "_quest.vrca"));
                    }
                    Console.WriteLine("QUEST DOWNLOAD");
                    if (avatar.Avatar.PcAssetUrl.ToLower() != "none")
                    {
                        try
                        {
                            var version = avatar.Avatar.PcAssetUrl.Split('/');
                            if (pcVersion > 0)
                            {
                                version[7] = pcVersion.ToString();
                            }
                            await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrca", "_pc.vrca"), download.downloadProgress, false));
                        }
                        catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.PcAssetUrl, filePath.Replace(".vrca", "_pc.vrca"), download.downloadProgress, false)); }
                        ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrca", "_pc.vrca"));
                    }
                    Console.WriteLine("PC DOWNLOAD");
                    return true;
                }
                else
                {
                    var dlgResult = MessageBox.Show("Select which version to download", "VRCA Select",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (dlgResult == DialogResult.No)
                    {
                        if (avatar.Avatar.QuestAssetUrl.ToLower() != "none")
                        {
                            try
                            {
                                var version = avatar.Avatar.QuestAssetUrl.Split('/');
                                if (questVersion > 0)
                                {
                                    version[7] = questVersion.ToString();
                                }
                                await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrca", "_quest.vrca"), download.downloadProgress, false));
                            }
                            catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.QuestAssetUrl, filePath.Replace(".vrca", "_quest.vrca"), download.downloadProgress, false)); }
                            ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrca", "_quest.vrca"));
                            pcDownload = false;
                        }
                        else
                        {
                            MessageBox.Show("Quest version doesn't exist", "ERROR", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else if (dlgResult == DialogResult.Yes)
                    {
                        if (avatar.Avatar.PcAssetUrl.ToLower() != "none")
                        {
                            try
                            {
                                var version = avatar.Avatar.PcAssetUrl.Split('/');
                                if (pcVersion > 0)
                                {
                                    version[7] = pcVersion.ToString();
                                }
                                await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrca", "_pc.vrca"), download.downloadProgress, false));
                            }
                            catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.PcAssetUrl, filePath.Replace(".vrca", "_pc.vrca"), download.downloadProgress, false)); }
                            ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrca", "_pc.vrca"));
                            pcDownload = true;
                        }
                        else
                        {
                            MessageBox.Show("PC version doesn't exist", "ERROR", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (avatar.Avatar.PcAssetUrl.ToLower() != "none" && avatar.Avatar.PcAssetUrl != null)
            {
                try
                {
                    var version = avatar.Avatar.PcAssetUrl.Split('/');
                    if (pcVersion > 0)
                    {
                        version[7] = pcVersion.ToString();
                    }
                    await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrca", "_pc.vrca"), download.downloadProgress, false));
                }
                catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.PcAssetUrl, filePath.Replace(".vrca", "_pc.vrca"), download.downloadProgress, false)); }
                ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrca", "_pc.vrca"));
                pcDownload = true;
            }
            else if (avatar.Avatar.QuestAssetUrl.ToLower() != "none" && avatar.Avatar.QuestAssetUrl != null)
            {
                try
                {
                    var version = avatar.Avatar.QuestAssetUrl.Split('/');
                    if (questVersion > 0)
                    {
                        version[7] = questVersion.ToString();
                    }
                    await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrca", "_quest.vrca"), download.downloadProgress, false));
                }
                catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.QuestAssetUrl, filePath.Replace(".vrca", "_quest.vrca"), download.downloadProgress, false)); }
                ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrca", "_quest.vrca"));
                pcDownload = false;
            }
            else
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> DownloadVrcwAsync(AvatarModel avatar, decimal pcVersion, Download download)
        {
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\VRCW\\{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}.vrcw";
            //download.Text = $"{avatar.avatar.avatarName} - {avatar.avatar.avatarId}";
            if (avatar.Avatar.PcAssetUrl.ToLower() != "none" && avatar.Avatar.PcAssetUrl != null)
            {
                try
                {
                    var version = avatar.Avatar.PcAssetUrl.Split('/');
                    if (pcVersion > 0)
                    {
                        version[7] = pcVersion.ToString();
                    }
                    await Task.Run(() => VRCA.DownloadVrcaFile(string.Join("/", version), filePath.Replace(".vrcw", "_pc.vrcw"), download.downloadProgress, false));
                }
                catch { await Task.Run(() => VRCA.DownloadVrcaFile(avatar.Avatar.PcAssetUrl, filePath.Replace(".vrcw", "_pc.vrcw"), download.downloadProgress, false)); }
                ShowSelectedInExplorer.FileOrFolder(filePath.Replace(".vrcw", "_pc.vrcw"));
                pcDownload = false;
                return true;
            }
            return false;
        }

        public static Tuple<int, int, VRChatFileInformation, VRChatFileInformation> GetVersion(string pcUrl, string questUrl)
        {
            int pcVersion;
            int questVersion;
            VRChatFileInformation pcRoot = null;
            VRChatFileInformation questRoot = null;
            if (!string.IsNullOrEmpty(pcUrl) && pcUrl.ToLower() != "none")
            {
                try
                {
                    var version = pcUrl.Split('/');
                    var urlCheck = pcUrl.Replace(version[6] + "/" + version[7] + "/file", version[6]);
                    var versionList = VRCA.GetVersions(urlCheck);
                    if (versionList != null)
                    {
                        pcRoot = versionList;
                        pcVersion = Convert.ToInt32(versionList.Versions.LastOrDefault().Version);
                    }
                    else
                    {
                        pcVersion = 1;
                    }
                }
                catch
                {
                    pcVersion = 1;
                }
            }
            else
            {
                pcVersion = 0;
            }

            if (!string.IsNullOrEmpty(questUrl) && questUrl.ToLower() != "none")
            {
                try
                {
                    var version = questUrl.Split('/');
                    var urlCheck = questUrl.Replace(version[6] + "/" + version[7] + "/file", version[6]);
                    var versionList = VRCA.GetVersions(urlCheck);
                    if (versionList != null)
                    {
                        questRoot = versionList;
                        questVersion = Convert.ToInt32(versionList.Versions.LastOrDefault().Version);
                    }
                    else
                    {
                        questVersion = 0;
                    }
                }
                catch
                {
                    questVersion = 1;
                }
            }
            else
            {
                questVersion = 0;
            }

            return Tuple.Create(pcVersion, questVersion, pcRoot, questRoot);
        }
    }
}