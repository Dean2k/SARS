﻿using CoreSystem;
using FACS01.Utilities;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using ARC.Models;
using ARC.Modules;
using ARC.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRChatAPI_New;
using VRChatAPI_New.Models;
using VRChatAPI_New.Modules.Game;
using ARC.Models.ExternalModels;
using ARC.Models.InternalModels;
using AssetsTools.NET.Extra;
using System.Web;

namespace ARC
{
    public partial class AvatarSystem : MetroForm
    {
        private ArcApi arcApi;
        public List<AvatarModel> avatars;
        public List<AvatarModel> cacheList;
        public ConfigSave<ListDown> downloadQueue;
        public List<LanguageTranslation> languageTranslations;
        public List<string> requestedAvatarIds = new List<string>();
        private HotswapConsole hotSwapConsole;
        private Thread _vrcaThread;
        private string userAgent = "UnityPlayer/2022.3.22f1-DWRL (UnityWebRequest/1.0, libcurl/8.5.0-DEV)";
        private string vrcaLocation = "";
        private CookieContainer _cookies;
        private string SystemName;
        private string unity2019 = "2019.4.31f1";
        private string unity2022L = "2022.3.22f1";

        public AvatarSystem()
        {
            InitializeComponent();
            StyleManager = metroStyleManager1;
        }

        private void CreateDirectoryIfNotExist(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void CheckFolders()
        {
            CreateDirectoryIfNotExist(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ARC"));
            CreateDirectoryIfNotExist(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ARC"));
            CreateDirectoryIfNotExist(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ARC", "AssetRipper"));
            CreateDirectoryIfNotExist(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ARC", "VRCA"));
            CreateDirectoryIfNotExist(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ARC", "VRCW"));
            CreateDirectoryIfNotExist(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ARC", "Images"));
        }

        private void AvatarSystem_Load(object sender, EventArgs e)
        {
            CheckFolders();
            if (string.IsNullOrEmpty(StaticValues.Config.Config.DocumentLocation))
            {
                StaticValues.Config.Config.DocumentLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\";
                StaticValues.Config.Save();
                StaticValues.ArcDocuments = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\";
                StaticValues.VrcaDownloadFolder = $"{StaticValues.ArcDocuments}VRCA\\";
                StaticValues.VrcwDownloadFolder = $"{StaticValues.ArcDocuments}VRCW\\";
                StaticValues.ImagesDownloadFolder = $"{StaticValues.ArcDocuments}Images\\";
                StaticValues.UnityProject = $"{StaticValues.ArcDocuments}Unity\\";
                StaticValues.VrcaViewer = $"{StaticValues.ArcDocuments}Viewer\\";
                StaticValues.AssetRipper = $"{StaticValues.ArcDocuments}AssetRipper\\";
                StaticValues.ExtractedFiles = $"{StaticValues.ArcDocuments}ExtractedFiles\\";
            }
        }

        private void CheckFav()
        {
            StaticValues.FavList.Config.RemoveAll(x => x == null);
            StaticValues.FavList.Save();
        }

        private void CheckRipped()
        {
            StaticValues.RippedList.Config.RemoveAll(x => x == null);
            StaticValues.RippedList.Save();
        }

        private void LoadSettings()
        {
            cbThemeColour.Text = StaticValues.Config.Config.ThemeColor;
            if (StaticValues.Config.Config.LightMode)
            {
                metroStyleManager1.Theme = MetroThemeStyle.Light;
            }
            ArcClient.GetClientVersion(txtClientVersion, StaticValues.Config);
            ArcClient.GetLatestVersion();


            if (string.IsNullOrEmpty(StaticValues.Config.Config.UnityLocation2019))
            {
                StaticValues.Config.Config.UnityLocation2019 = ArcClient.UnitySetup(unity2019);
            }

            if (string.IsNullOrEmpty(StaticValues.Config.Config.UnityLocation2022L))
            {
                StaticValues.Config.Config.UnityLocation2022L = ArcClient.UnitySetup(unity2022L);
            }

            if (string.IsNullOrEmpty(StaticValues.Config.Config.MacAddress))
            {
                Random rnd = new Random();
                StaticValues.Config.Config.MacAddress = EasyHash.GetSHA1String(new byte[] { (byte)rnd.Next(254), (byte)rnd.Next(254), (byte)rnd.Next(254), (byte)rnd.Next(254), (byte)rnd.Next(254) });
                StaticValues.Config.Save();
            }

            if (!string.IsNullOrEmpty(StaticValues.Config.Config.PreSelectedAvatarLocation))
            {
                txtAvatarOutput.Text = StaticValues.Config.Config.PreSelectedAvatarLocation;
            }
            if (!string.IsNullOrEmpty(StaticValues.Config.Config.PreSelectedWorldLocation))
            {
                txtWorldOutput.Text = StaticValues.Config.Config.PreSelectedWorldLocation;
            }

            if (StaticValues.Config.Config.PreSelectedAvatarLocationChecked != null)
            {
                toggleAvatar.Checked = StaticValues.Config.Config.PreSelectedAvatarLocationChecked;
            }

            if (StaticValues.Config.Config.PreSelectedWorldLocation != null)
            {
                toggleWorld.Checked = StaticValues.Config.Config.PreSelectedWorldLocationChecked;
            }

            StartUp.SetupDownloader(StaticValues.Config.Config.MacAddress);
            if (!string.IsNullOrEmpty(StaticValues.Config.Config.AuthKey))
                try
                {
                    var check = Login.LoginWithTokenAsync(StaticValues.Config.Config.AuthKey, StaticValues.Config.Config.TwoFactor).Result;
                    if (check == null)
                    {
                        MessageBox.Show("VRChat credentials expired, please relogin");
                        DeleteLoginInfo();
                        if (this.Text.Contains("- Authed"))
                        {
                            this.Text.Replace(" - Authed", "- Not Authed");
                        }
                        else if (!this.Text.Contains("Not Authed"))
                        {
                            this.Text = $"{this.Text} - Not Authed";
                        }
                    }
                }
                catch { }
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            _loadedAvatars = new List<string>();
            CacheScannerTimer.Enabled = false;
            cacheFolderAuto = null;
            string limit = cbLimit.Text;
            bool avatar = true;
            if (limit == "")
            {
                limit = "5000";
            }
            if (_cookies == null)
            {
                MessageBox.Show("Please enter your login with discord first.");
                return;
            }
            AvatarSearch avatarSearch = new AvatarSearch { Key = StaticValues.Config.Config.ApiKey, Amount = Convert.ToInt32(limit), PrivateAvatars = chkPrivate.Checked, PublicAvatars = chkPublic.Checked, ContainsSearch = chkContains.Checked, DebugMode = true, PcAvatars = chkPC.Checked, QuestAvatars = chkQuest.Checked };
            if (cbSearchTerm.Text == "Avatar Name")
            {
                avatarSearch.AvatarName = txtSearchTerm.Text;
            }
            else if (cbSearchTerm.Text == "Author Name")
            {
                avatarSearch.AuthorName = txtSearchTerm.Text;
            }
            else if (cbSearchTerm.Text == "Avatar ID")
            {
                avatarSearch.AvatarId = txtSearchTerm.Text;
            }
            else if (cbSearchTerm.Text == "Author ID")
            {
                avatarSearch.AuthorId = txtSearchTerm.Text;
            }
            else if (cbSearchTerm.Text == "World Name")
            {
                avatarSearch.AvatarName = txtSearchTerm.Text;
                avatar = false;
            }
            else if (cbSearchTerm.Text == "World ID")
            {
                avatarSearch.AvatarId = txtSearchTerm.Text;
                avatar = false;
            }
            string customApi = "";

            avatars = arcApi.AvatarSearch(avatarSearch, avatar, _cookies);

            avatarGrid.Rows.Clear();
            if (avatars != null && avatars.Count > 0)
            {
                SendMessage(avatarGrid.Handle, WM_SETREDRAW, false, 0);
                LoadData(false, avatar);
                SendMessage(avatarGrid.Handle, WM_SETREDRAW, true, 0);
                avatarGrid.Refresh();
                LoadImages();
            }
            else
            {
                MessageBox.Show("No results");
            }
        }

        private List<string> _loadedAvatars = new List<string>();

        private void LoadData(bool local = false, bool avatar = false)
        {
            Bitmap bitmap2 = null;
            try
            {
                if (local)
                {
                    bitmap2 = new Bitmap(Resources.No_Image);
                }
                else
                {
                    bitmap2 = new Bitmap(Resources.download);
                }
            }
            catch { }

            try
            {
                avatars.RemoveAll(x => x.Avatar == null);
                avatars.RemoveAll(x => x.Avatar.AvatarId == null);
                avatars.RemoveAll(x => x.Avatar.AvatarId == "");
            }
            catch { }

            try
            {
                avatarGrid.AllowUserToAddRows = true;
                avatarGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                for (int i = 0; i < avatars.Count; i++)
                {
                    if (!_loadedAvatars.Contains(avatars[i].Avatar.AvatarId))
                    {
                        try
                        {
                            DataGridViewRow row = (DataGridViewRow)avatarGrid.Rows[0].Clone();

                            row.Cells[0].Value = bitmap2;
                            row.Cells[1].Value = avatars[i].Avatar.AvatarName;
                            row.Cells[2].Value = avatars[i].Avatar.AuthorName;
                            row.Cells[3].Value = avatars[i].Avatar.AvatarId;
                            row.Cells[4].Value = avatars[i].Avatar.RecordCreated;
                            row.Cells[5].Value = avatars[i].Avatar.ThumbnailUrl;
                            row.Cells[6].Value = ArcClient.FormatSize(avatars[i].Avatar.FileSize);
                            if (StaticValues.RippedList.Config.Any(x => x.Avatar.AvatarId == avatars[i].Avatar.AvatarId))
                            {
                                row.Cells[7].Value = true;
                            }
                            if (StaticValues.FavList.Config.Any(x => x.Avatar.AvatarId == avatars[i].Avatar.AvatarId))
                            {
                                row.Cells[8].Value = true;
                            }
                            if (!local)
                            {
                                row.Cells[9].Value = avatar;
                            }
                            else
                            {
                                if (avatars[i].Avatar.AvatarId.Contains("wrld_"))
                                {
                                    row.Cells[9].Value = false;
                                }
                                else
                                {
                                    row.Cells[9].Value = true;
                                }
                            }
                            avatarGrid.Rows.Add(row);
                        }
                        catch { }
                        _loadedAvatars.Add(avatars[i].Avatar.AvatarId);
                    }
                }
                avatarGrid.AllowUserToAddRows = false;
                int count = avatarGrid.Rows.Count;

                lblAvatarCount.Text = (count).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void LoadImages()
        {
            if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\images"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\images");
            }
            ArcClient.ThreadLoadImages(userAgent, avatarGrid, avatars);
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in avatarGrid.SelectedRows)
            {
                AvatarModel info = avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value.ToString().Trim());
                if (info == null)
                {
                    info = cacheList.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value.ToString().Trim());
                }
                Avatar_Info avatar = new Avatar_Info();
                avatar.txtAvatarInfo.Text = SetAvatarInfo(info);
                avatar._selectedAvatar = info;
                avatar.Show();
            }
        }

        public string SetAvatarInfo(AvatarModel avatar)
        {
            string avatarString = $"Time Detected: {avatar.Avatar.RecordCreated} {Environment.NewLine}" +
                $"Avatar ID: {avatar.Avatar.AvatarId} {Environment.NewLine}" +
                $"Avatar Name: {avatar.Avatar.AvatarName} {Environment.NewLine}" +
                $"Avatar Description {avatar.Avatar.AvatarDescription} {Environment.NewLine}" +
                $"Author ID: {avatar.Avatar.AuthorId} {Environment.NewLine}" +
                $"Author Name: {avatar.Avatar.AuthorName} {Environment.NewLine}" +
                $"PC Asset URL: {avatar.Avatar.PcAssetUrl} {Environment.NewLine}" +
                $"Quest Asset URL: {avatar.Avatar.QuestAssetUrl} {Environment.NewLine}" +
                $"Image URL: {avatar.Avatar.ImageUrl} {Environment.NewLine}" +
                $"Thumbnail URL: {avatar.Avatar.ThumbnailUrl} {Environment.NewLine}" +
                $"Unity Version: {avatar.Avatar.UnityVersion} {Environment.NewLine}" +
                $"Release Status: {avatar.Avatar.ReleaseStatus} {Environment.NewLine}" +
                $"Tags: {String.Join(", ", avatar.Tags.Select(p => p.ToString()).ToArray())}";
            return avatarString;
        }

        private void btnBrowserView_Click(object sender, EventArgs e)
        {
            if (avatars != null)
            {
                GenerateHtml.GenerateHtmlPage(avatars);
                Process.Start("avatars.html");
            }
        }

        private void metroTabPage2_Click(object sender, EventArgs e)
        {
        }

        private void btnRipped_Click(object sender, EventArgs e)
        {
            avatars = StaticValues.RippedList.Config;
            avatarGrid.Rows.Clear();
            LoadData();
            LoadImages();
        }

        private void btnSearchFavorites_Click(object sender, EventArgs e)
        {
            avatars = StaticValues.FavList.Config;
            avatarGrid.Rows.Clear();
            LoadData();
            LoadImages();
        }

        private void btnToggleFavorite_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in avatarGrid.SelectedRows)
            {
                try
                {
                    if (!StaticValues.FavList.Config.Any(x => x.Avatar.AvatarId == row.Cells[3].Value.ToString()))
                    {
                        StaticValues.FavList.Config.Add(avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value.ToString()));
                        row.Cells[7].Value = "true";
                    }
                    else
                    {
                        StaticValues.FavList.Config.Remove(avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value.ToString()));
                        row.Cells[7].Value = "false";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some error" + ex.Message);
                }
            }

            StaticValues.FavList.Save();
        }

        private void avatarGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            vrcaLocation = "";
            this.Text = SystemName;
            this.Update();
            this.Refresh();
        }
        private Guid Guid = Guid.NewGuid();
        private void btnDiscordAuth_Click(object sender, EventArgs e)
        {
            Guid = Guid.NewGuid();
            Process.Start($"https://api.avatarrecovery.com/AuthAvatar/Login?guid={Guid}");
            CookieChecker.Enabled = true;
        }

        private void btnLight_Click(object sender, EventArgs e)
        {
            metroStyleManager1.Theme = MetroThemeStyle.Light;
            StaticValues.Config.Config.LightMode = true;
            StaticValues.Config.Save();
        }

        private void btnDark_Click(object sender, EventArgs e)
        {
            metroStyleManager1.Theme = MetroThemeStyle.Dark;
            StaticValues.Config.Config.LightMode = false;
            StaticValues.Config.Save();
        }

        private void LoadStyle(string style)
        {
            switch (style)
            {
                case "Black":
                    metroStyleManager1.Style = MetroColorStyle.Black;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "White":
                    metroStyleManager1.Style = MetroColorStyle.White;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Silver":
                    metroStyleManager1.Style = MetroColorStyle.Silver;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Green":
                    metroStyleManager1.Style = MetroColorStyle.Green;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Blue":
                    metroStyleManager1.Style = MetroColorStyle.Blue;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Lime":
                    metroStyleManager1.Style = MetroColorStyle.Lime;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Teal":
                    metroStyleManager1.Style = MetroColorStyle.Teal;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Orange":
                    metroStyleManager1.Style = MetroColorStyle.Orange;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Brown":
                    metroStyleManager1.Style = MetroColorStyle.Brown;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Pink":
                    metroStyleManager1.Style = MetroColorStyle.Pink;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Magenta":
                    metroStyleManager1.Style = MetroColorStyle.Magenta;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Purple":
                    metroStyleManager1.Style = MetroColorStyle.Purple;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Red":
                    metroStyleManager1.Style = MetroColorStyle.Red;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                case "Yellow":
                    metroStyleManager1.Style = MetroColorStyle.Yellow;
                    StaticValues.Config.Config.ThemeColor = style;
                    StaticValues.Config.Save();
                    break;

                default:
                    metroStyleManager1.Style = MetroColorStyle.Default;
                    StaticValues.Config.Config.ThemeColor = "Default";
                    StaticValues.Config.Save();
                    break;
            }
        }

        private void cbThemeColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStyle(cbThemeColour.Text);
        }

        private async Task<bool> hotSwap()
        {
            if (_vrcaThread != null)
            {
                if (_vrcaThread.IsAlive)
                {
                    MessageBox.Show("VRCA Still hotswapping please try again later");
                    return false;
                }
            }

            string fileLocation = "";
            if (vrcaLocation == "")
            {
                if (avatarGrid.SelectedRows.Count > 1)
                {
                    MessageBox.Show("Please only select 1 row at a time for hotswapping.");
                    return false;
                }
                if (avatarGrid.SelectedRows.Count < 1)
                {
                    MessageBox.Show("You must at least select one avatar to hotswap");
                    return false;
                }
                AvatarModel avatar = null;
                foreach (DataGridViewRow row in avatarGrid.SelectedRows)
                {
                    avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarGrid.SelectedRows[0].Cells[3].Value);

                    try
                    {
                        avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value);
                    }
                    catch { }
                    Download download = new Download { Text = $"{avatar.Avatar.AvatarName} - {avatar.Avatar.AvatarId}" };
                    download.Show();
                    await Task.Run(() => AvatarFunctions.DownloadVrcaAsync(avatar, nmPcVersion.Value, nmQuestVersion.Value, download));
                    download.Close();
                }
                if (AvatarFunctions.pcDownload)
                {
                    fileLocation = $"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}_pc.vrca";
                }
                else
                {
                    fileLocation = $"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}_quest.vrca";
                }
                if (!File.Exists(fileLocation))
                {
                    MessageBox.Show("Download failed, hotswap can't continue");
                    return false;
                }
            }
            else
            {
                fileLocation = vrcaLocation;
            }
            hotSwapConsole = new HotswapConsole();
            hotSwapConsole.Show();
            string inputName = Interaction.InputBox("Please name the internal asset name something.", "Avatar Name", "Avatar");
            string languageCode;
            try
            {
                languageCode = languageTranslations.FirstOrDefault(x => x.name == cbLanguage.Text).code;
            }
            catch { languageCode = "en"; }

            _vrcaThread = new Thread(() => HotSwap.HotswapProcess(fileLocation, hotSwapConsole.txtStatusText, hotSwapConsole.pbProgress, inputName, chkUnlockPassword.Checked, chkAdvanceUnlock.Checked, StaticValues.Config.Config.ApiKey, chkTranslate.Checked, languageCode, chkAdvancedDic.Checked, StaticValues.Config.Config.UnityLocation2019, StaticValues.Config.Config.UnityLocation2022L, StaticValues.Config.Config.HotSwapName2019, StaticValues.Config.Config.HotSwapName2022L, StaticValues.Config.Config.DocumentLocation, chkUnityOpen.Checked, StaticValues.Config.Config.DocumentLocation));
            _vrcaThread.Start();

            return true;
        }

        private void unityExtract(string hotswapName)
        {
            var tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                .Replace("\\Roaming", "");
            var unityTemp = $"\\Local\\Temp\\DefaultCompany\\{hotswapName}";
            var unityTemp2 = $"\\LocalLow\\Temp\\DefaultCompany\\{hotswapName}";

            RandomFunctions.tryDeleteDirectory(tempFolder + unityTemp);
            RandomFunctions.tryDeleteDirectory(tempFolder + unityTemp2);

            AvatarFunctions.ExtractHSB(hotswapName, false);
            ArcClient.CopyFiles(hotswapName, StaticValues.Config.Config.DocumentLocation);
            //RandomFunctions.OpenUnity(StaticValues.Config.Config.UnityLocation2022, StaticValues.Config.Config.HotSwapName2022);
        }

        private void avatarGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == null || e.RowIndex == -1)
                return;

            if (avatarGrid.Columns[e.ColumnIndex].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
            {
            }

            var s = e.Graphics.MeasureString(e.Value.ToString(), new Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Pixel));
            if (e.Value.ToString().Length / (double)avatarGrid.Columns[e.ColumnIndex].Width >= .189)
            {
                SolidBrush backColorBrush;
                if (avatarGrid.SelectedRows[0].Index == e.RowIndex)
                    backColorBrush = new SolidBrush(e.CellStyle.SelectionBackColor);
                else
                    backColorBrush = new SolidBrush(e.CellStyle.BackColor);

                using (backColorBrush)
                {
                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                    e.Graphics.DrawString(e.Value.ToString(), avatarGrid.Font, Brushes.Black, e.CellBounds, StringFormat.GenericDefault);
                    //avatarGrid.Rows[e.RowIndex].Height = System.Convert.ToInt32((s.Height * Math.Ceiling(s.Width / (double)avatarGrid.Columns[e.ColumnIndex].Width)));
                    e.Handled = true;
                }
            }
        }

        private void btnResetScene_Click(object sender, EventArgs e)
        {
            try
            {
                ArcClient.CopyFiles(StaticValues.Config.Config.HotSwapName2019, StaticValues.Config.Config.DocumentLocation);
            }
            catch { }
            try
            {
                ArcClient.CopyFiles(StaticValues.Config.Config.HotSwapName2022L, StaticValues.Config.Config.DocumentLocation);
            }
            catch { }
        }

        private void btnHsbClean_Click(object sender, EventArgs e)
        {
            ArcClient.CleanHsb2019(StaticValues.Config);
            ArcClient.CleanHsb2022L(StaticValues.Config);
            AvatarFunctions.ExtractHSB2019(StaticValues.Config.Config.HotSwapName2019, true);
            AvatarFunctions.ExtractHSB2022L(StaticValues.Config.Config.HotSwapName2022L, true);
        }

        private string previousSelectedFolder = null;

        private void btnLoadVRCA_Click(object sender, EventArgs e)
        {
            vrcaLocation = SelectFileVrca(previousSelectedFolder);
            previousSelectedFolder = vrcaLocation;
            if (!string.IsNullOrEmpty(vrcaLocation) && chkAdditional.Checked)
            {
                DecompressToFileStr(vrcaLocation);

                var manager = new AssetsManager();
                manager.LoadClassPackage("class.tpk");
                var bundle = manager.LoadBundleFile($"{vrcaLocation}_decomp");
                var afileInst = manager.LoadAssetsFileFromBundle(bundle, 0, true);
                manager.LoadClassDatabaseFromPackage(bundle.file.Header.EngineVersion);
                var afile = afileInst.file;

                lblUnityVersion.Text = afile.Metadata.UnityVersion;

                bundle.DataStream.Close();
                bundle.DataStream.Dispose();
                bundle.file.Close();

                manager.Bundles.Clear();
                afileInst.file.Close();

                File.Delete($"{vrcaLocation}_decomp");
            }
        }

        private void DecompressToFileStr(string bundlePath)
        {
            try
            {
                string commands = string.Format($"\"{bundlePath}\" \"cacheDecompress\"");
                Console.WriteLine(commands);
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "AssetViewer.exe",
                    Arguments = commands,
                    WorkingDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\Viewer\\",
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }


        private string SelectFileVrca(string directory)
        {
            var filePath = string.Empty;

            if (directory == null)
            {
                directory = "C:\\";
            }

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = directory;
                openFileDialog.Filter = "vrca files (*.vrca)|*.vrca|vrcw files (*.vrcw)|*.vrcw";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    this.Text = SystemName + " | VRCA FILE LOADED";
                    this.Update();
                    this.Refresh();
                }
            }

            return filePath;
        }

        private void avatarGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            vrcaLocation = "";
            this.Text = SystemName;
            this.Update();
            this.Refresh();
        }

        private void DeleteLoginInfo()
        {
            StaticValues.Config.Config.UserId = null;
            StaticValues.Config.Config.AuthKey = null;
            StaticValues.Config.Config.TwoFactor = null;
            StaticValues.Config.Save();

            try
            {
                Login.Logout();
            }
            catch
            {
            }
            try
            {
                File.Delete("auth.txt");
            }
            catch { }

            try
            {
                File.Delete("2fa.txt");
            }
            catch { }
        }

        private void btnSaveVRC_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteLoginInfo();
                StartUp.SetupDownloader(StaticValues.Config.Config.MacAddress);
            }
            catch
            {
            }
            if (txtVRCUsername.Text != "" && txtVRCPassword.Text != "" && txtClientVersion.Text != "")
            {
                VRChatUserInfo info = null;
                try
                {
                    info = Login.LoginWithCredentials(txtVRCUsername.Text, txtVRCPassword.Text, null).Result;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Couldn't verify 2FA code")
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                if (info != null)
                {
                    if (string.IsNullOrEmpty(info.Details.AuthKey))
                    {
                        MessageBox.Show("Login failed");
                        return;
                    }
                    if (string.IsNullOrEmpty(info.DisplayName))
                    {
                        MessageBox.Show("Login failed");
                        return;
                    }
                    StaticValues.Config.Config.UserId = info.Id;
                    StaticValues.Config.Config.AuthKey = info.Details.AuthKey;
                    StaticValues.Config.Config.TwoFactor = info.Details.TwoFactorKey;
                    StaticValues.Config.Save();
                    MessageBox.Show("Login Successful");
                }
                else
                {
                    MessageBox.Show("Login Failed");
                }
            }
            else
            {
                MessageBox.Show("Please enter your Username/Password and make sure the client version has been filled\nIf the client version isn't filled turn off any VPN and reopen this app");
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            Download();
        }

        private async Task<bool> Download()
        {
            if (avatarGrid.SelectedRows.Count > 1)
            {
                AvatarModel avatar = null;
                foreach (DataGridViewRow row in avatarGrid.SelectedRows)
                {
                    avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value);

                    if (string.IsNullOrEmpty(StaticValues.Config.Config.AuthKey))
                    {
                        MessageBox.Show("Please Login with an alt first.");
                        return false;
                    }
                    Download download = new Download { Text = $"{avatar.Avatar.AvatarName} - {avatar.Avatar.AvatarId}" };
                    download.Show();
                    if ((bool)row.Cells[9].Value)
                    {
                        await Task.Run(() => AvatarFunctions.DownloadVrcaAsync(avatar, 0, 0, download));
                    }
                    else
                    {
                        await Task.Run(() => AvatarFunctions.DownloadVrcwAsync(avatar, 0, download));
                    }
                    download.Close();
                }
            }
            else
            {
                AvatarModel avatar = null;
                foreach (DataGridViewRow row in avatarGrid.SelectedRows)
                {
                    avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value);

                    Download download = new Download { Text = $"{avatar.Avatar.AvatarName} - {avatar.Avatar.AvatarId}" };
                    download.Show();
                    if ((bool)row.Cells[9].Value)
                    {
                        await Task.Run(() => AvatarFunctions.DownloadVrcaAsync(avatar, nmPcVersion.Value, nmQuestVersion.Value, download));
                    }
                    else
                    {
                        await Task.Run(() => AvatarFunctions.DownloadVrcwAsync(avatar, nmPcVersion.Value, download));
                    }
                    download.Close();
                }
            }
            return true;
        }

        private async void btnExtractVRCA_Click(object sender, EventArgs e)
        {
            if (avatarGrid.SelectedRows.Count == 1 || vrcaLocation != "")
            {
                string avatarFile;
                AvatarModel avatar = null;
                bool worldFile = false;
                if (vrcaLocation == "")
                {
                    avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarGrid.SelectedRows[0].Cells[3].Value);
                    Download download = new Download { Text = $"{avatar.Avatar.AvatarName} - {avatar.Avatar.AvatarId}" };
                    download.Show();
                    if (avatar.Avatar.AvatarId.StartsWith("avtr_"))
                    {
                        worldFile = false;
                        if (await Task.Run(() => AvatarFunctions.DownloadVrcaAsync(avatar, nmPcVersion.Value, nmQuestVersion.Value, download)) == false) return;
                        avatarFile = $"{StaticValues.VrcaDownloadFolder}{avatar.Avatar.AvatarName}-{avatar.Avatar.AvatarId}_pc.vrca";
                        download.Close();
                    }
                    else
                    {
                        worldFile = true;
                        if (await Task.Run(() => AvatarFunctions.DownloadVrcwAsync(avatar, nmPcVersion.Value, download)) == false) return;
                        avatarFile = $"{StaticValues.VrcaDownloadFolder}{avatar.Avatar.AvatarName}-{avatar.Avatar.AvatarId}_pc.vrcw";
                        download.Close();
                    }
                }
                else
                {
                    avatarFile = vrcaLocation;
                    if (vrcaLocation.EndsWith(".vrcw"))
                    {
                        worldFile = true;
                    }
                }
                string questFile = avatarFile.Replace("_pc", "_quest");
                if (File.Exists(avatarFile) && File.Exists(questFile) && avatarFile != questFile)
                {
                    var dlgResult = MessageBox.Show("Select which version to extract", "VRCA Select",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (dlgResult == DialogResult.No)
                    {
                        avatarFile = avatarFile.Replace("_pc", "_quest");
                    }
                }
                else
                {
                    if (!File.Exists(avatarFile))
                    {
                        if (File.Exists(avatarFile.Replace("_pc", "_quest")))
                        {
                            avatarFile = avatarFile.Replace("_pc", "_quest");
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong with avatar file location, either it failed to download or the file doesn't exist");
                            return;
                        }
                    }
                }

                var folderDlg = new CommonOpenFileDialog
                {
                    IsFolderPicker = true
                };
                // Show the FolderBrowserDialog.
                CommonFileDialogResult result = CommonFileDialogResult.Ok;
                if (toggleAvatar.Checked && txtAvatarOutput.Text != "" && !worldFile)
                {
                    folderDlg.InitialDirectory = txtAvatarOutput.Text;
                }
                else if (toggleWorld.Checked && txtWorldOutput.Text != "" && worldFile)
                {
                    folderDlg.InitialDirectory = txtWorldOutput.Text;
                }
                else
                {
                    result = folderDlg.ShowDialog();
                }

                if (result == CommonFileDialogResult.Ok || toggleAvatar.Checked && txtAvatarOutput.Text != "" && !worldFile || toggleWorld.Checked && txtWorldOutput.Text != "" && worldFile)
                {
                    var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var invalidFileNameChars = Path.GetInvalidFileNameChars();
                    string folderExtractLocation;
                    if (!toggleAvatar.Checked && !toggleWorld.Checked)
                    {
                        folderExtractLocation = folderDlg.FileName + @"\" + Path.GetFileNameWithoutExtension(avatarFile);
                    }
                    else if (avatarFile.EndsWith(".vrcw"))
                    {
                        folderExtractLocation = txtWorldOutput.Text + @"\" + Path.GetFileNameWithoutExtension(avatarFile);
                    }
                    else
                    {
                        folderExtractLocation = txtAvatarOutput.Text + @"\" + Path.GetFileNameWithoutExtension(avatarFile);
                    }
                    if (!Directory.Exists(folderExtractLocation)) Directory.CreateDirectory(folderExtractLocation);
                    var commands =
                        string.Format(
                            "/C AssetRipper.exe \"{1}\" -o \"{0}\"",
                             folderExtractLocation, avatarFile);

                    var p = new Process();
                    var psi = new ProcessStartInfo
                    {
                        FileName = "CMD.EXE",
                        Arguments = commands,
                        WorkingDirectory = StaticValues.AssetRipper
                    };
                    p.StartInfo = psi;
                    p.Start();
                    p.WaitForExit();

                    if (!Directory.Exists(folderExtractLocation)) { return; }

                    RandomFunctions.tryDeleteDirectory(folderExtractLocation + @"\AssetRipper\GameAssemblies");
                    try
                    {
                        Directory.Move(folderExtractLocation + @"\Assets\Shader",
                            folderExtractLocation + @"\Assets\.Shader");
                    }
                    catch
                    {
                    }
                    try
                    {
                        Directory.Move(folderExtractLocation + @"\Assets\Scripts",
                            folderExtractLocation + @"\Assets\.Scripts");
                    }
                    catch
                    {
                    }
                    RandomFunctions.tryDeleteDirectory(folderExtractLocation + @"\AuxiliaryFiles");
                    RandomFunctions.tryDeleteDirectory(folderExtractLocation + @"\ExportedProject\AssetRipper");
                    RandomFunctions.tryDeleteDirectory(folderExtractLocation + @"\ExportedProject\ProjectSettings");
                    try
                    {
                        Directory.Move(folderExtractLocation + @"\ExportedProject\Assets\Shader",
                            folderExtractLocation + @"\ExportedProject\Assets\.Shader");
                        Directory.Move(folderExtractLocation + @"\ExportedProject\Assets\Scripts",
                            folderExtractLocation + @"\ExportedProject\Assets\.Scripts");
                        Directory.Move(folderExtractLocation + @"\ExportedProject\Assets\MonoScript",
                            folderExtractLocation + @"\ExportedProject\Assets\.MonoScript");
                    }
                    catch
                    {
                    }
                    FixVRC3Scripts fixVRC3Scripts = new FixVRC3Scripts();
                    string message = fixVRC3Scripts.FixScripts(folderExtractLocation);
                    if (chkReassignShaders.Checked)
                    {
                        FixVRCMaterials fixVRCMaterials = new FixVRCMaterials();
                        message = message + Environment.NewLine + fixVRCMaterials.FixMaterials(folderExtractLocation);
                    }

                    if (chkUnityPackage.Checked)
                    {
                        RandomFunctions.tryDeleteDirectory(folderExtractLocation + @"\ExportedProject\.Scripts");
                        RandomFunctions.tryDeleteDirectory(folderExtractLocation + @"\ExportedProject\.Shader");
                        var inpath = folderExtractLocation + @"\ExportedProject\Assets";

                        var extensions = new List<string>()
                        {
                            "meta"
                        };

                        var skipFolders = new List<string>()
                        {
                            ".Scripts",
                            ".Shader"
                        };

                        try
                        {

                            var pack = Package.FromDirectory(inpath, Path.GetFileNameWithoutExtension(avatarFile), true, extensions.ToArray(), skipFolders.ToArray());
                            pack.GeneratePackage(saveLocation: folderExtractLocation.Replace(Path.GetFileNameWithoutExtension(avatarFile), ""));
                            RandomFunctions.tryDeleteDirectory(folderExtractLocation);
                        }
                        catch (Exception ex) { MessageBox.Show("Failed to generate unity package\n" + ex.Message); }

                    }

                    MessageBox.Show(message);
                    if (vrcaLocation == "")
                    {
                        StaticValues.RippedList.Config.Add(avatar);
                        StaticValues.RippedList.Save();
                    }
                }
            }
            else
            {
                MetroMessageBox.Show(this, "Please select an avatar or world first.", "ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Preview();
        }

        private async Task<bool> Preview()
        {
            string fileLocation;
            if (avatarGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in avatarGrid.SelectedRows)
                {
                    AvatarModel avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value);
                    if (avatar.Avatar.AuthorId == "Unknown Cache")
                    {
                        if (!File.Exists($"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}_pc.vrca"))
                        {
                            File.Copy(avatar.Avatar.PcAssetUrl, $"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}_pc.vrca");
                        }
                        fileLocation = $"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}_pc.vrca";
                        try
                        {
                            string commands = string.Format($"\"{fileLocation}\"");
                            Console.WriteLine(commands);
                            Process p = new Process();
                            ProcessStartInfo psi = new ProcessStartInfo
                            {
                                FileName = "AssetViewer.exe",
                                Arguments = commands,
                                WorkingDirectory = StaticValues.VrcaViewer,
                            };
                            p.StartInfo = psi;
                            p.Start();
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        return true;
                    }
                }
            }
            if (string.IsNullOrEmpty(StaticValues.Config.Config.UserId) && vrcaLocation == "")
            {
                MessageBox.Show("Please Login with an alt first.");
                return false;
            }

            if (vrcaLocation == "")
            {
                if (avatarGrid.SelectedRows.Count > 1)
                {
                    MessageBox.Show("Please only select 1 row at a time for hotswapping.");
                    return false;
                }
                if (avatarGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select an avatar first");
                    return false;
                }
                bool downloaded = false;
                AvatarModel avatar = null;
                Download download = null;
                foreach (DataGridViewRow row in avatarGrid.SelectedRows)
                {
                    avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == row.Cells[3].Value);
                    avatar.Avatar.QuestAssetUrl = "None";
                    download = new Download() { Text = $"{avatar.Avatar.AvatarName} - {avatar.Avatar.AvatarId}" };
                    download.Show();
                    await Task.Run(() => AvatarFunctions.DownloadVrcaAsync(avatar, nmPcVersion.Value, nmQuestVersion.Value, download));
                }
                download.Close();
                fileLocation = $"{StaticValues.VrcaDownloadFolder}{RandomFunctions.ReplaceInvalidChars(avatar.Avatar.AvatarName)}-{avatar.Avatar.AvatarId}_pc.vrca";
            }
            else
            {
                if (string.IsNullOrEmpty(vrcaLocation))
                {
                    MessageBox.Show("Please select an avatar first or load an VRCA file");
                    return false;
                }
                fileLocation = vrcaLocation;
            }

            try
            {
                string commands = string.Format($"\"{fileLocation}\"");
                Console.WriteLine(commands);
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "AssetViewer.exe",
                    Arguments = commands,
                    WorkingDirectory = StaticValues.VrcaViewer,
                };
                p.StartInfo = psi;
                p.Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return true;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (StaticValues.Config.Config.AuthKey != null)
            {
                var check = Login.LoginWithTokenAsync(StaticValues.Config.Config.AuthKey, StaticValues.Config.Config.TwoFactor);
                if (check == null)
                {
                    MessageBox.Show("VRChat credentials expired, please relogin");
                    DeleteLoginInfo();
                }
                else
                {
                    MessageBox.Show("Token Works :D");
                }
            }
            else
            {
                MessageBox.Show("Login First");
            }
        }

        private void btnAvatarOut_Click(object sender, EventArgs e)
        {
            var folderDlg = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
            var result = folderDlg.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                txtAvatarOutput.Text = folderDlg.FileName;
                StaticValues.Config.Config.PreSelectedAvatarLocation = folderDlg.FileName;
                StaticValues.Config.Save();
            }
        }

        private void btnWorldOut_Click(object sender, EventArgs e)
        {
            var folderDlg = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
            var result = folderDlg.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                txtWorldOutput.Text = folderDlg.FileName;
                StaticValues.Config.Config.PreSelectedWorldLocation = folderDlg.FileName;
                StaticValues.Config.Save();
            }
        }

        private void toggleAvatar_CheckedChanged(object sender, EventArgs e)
        {
            StaticValues.Config.Config.PreSelectedAvatarLocationChecked = toggleAvatar.Checked;
            StaticValues.Config.Save();
        }

        private void toggleWorld_CheckedChanged(object sender, EventArgs e)
        {
            StaticValues.Config.Config.PreSelectedWorldLocationChecked = toggleWorld.Checked;
            StaticValues.Config.Save();
        }

        private void btn2FA_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.google.com/accounts/answer/1066447?hl=en&ref_topic=2954345");
        }

        private void nmPcVersion_ValueChanged(object sender, EventArgs e)
        {
            if (ArcClient.avatarVersionPc != null && nmPcVersion.Value > 0)
            {
                txtAvatarSizePc.Text = ArcClient.FormatSize(ArcClient.avatarVersionPc.Versions.FirstOrDefault(x => x.Version == nmPcVersion.Value).File.SizeInBytes);
            }
        }

        private void nmQuestVersion_ValueChanged(object sender, EventArgs e)
        {
            if (ArcClient.avatarVersionQuest != null && nmQuestVersion.Value > 0)
            {
                txtAvatarSizeQuest.Text = ArcClient.FormatSize(ArcClient.avatarVersionQuest.Versions.FirstOrDefault(x => x.Version == nmQuestVersion.Value).File.SizeInBytes);
            }
        }

        private void btnScanCacheFolder_Click(object sender, EventArgs e)
        {
            _loadedAvatars = new List<string>();
            ScanCacheAvatar(false);
        }

        private string cacheFolderAuto = null;
        private List<string> loadedImage = new List<string>();

        private async Task<bool> ScanCacheAvatar(bool bypassLoaded)
        {
            if (!bypassLoaded)
            {
                loadedImage = new List<string>();
                string cachePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low\\VRChat\\VRChat\\Cache-WindowsPlayer";

                CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
                dialog.Title = "Select your VRChat Cache folder called Cache-WindowsPlayer";

                if (Directory.Exists(cachePath))
                {
                    dialog.InitialDirectory = cachePath;
                }

                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    MessageBox.Show("No Folder selected");
                    return false;
                }

                cachePath = dialog.FileName;
                cacheFolderAuto = cachePath;
            }
            CacheMessages.Enabled = true;
            await CacheScanner.ScanCache(cacheFolderAuto, bypassLoaded);

            if (!bypassLoaded)
            {
                List<AvatarModel> list = new List<AvatarModel>();
                foreach (var item in CacheScanner.avatarIds)
                {
                    if (item != null)
                    {
                        AvatarModel avatar = new AvatarModel { Tags = new List<string>(), Avatar = new AvatarDetails { ThumbnailUrl = "https://avatarrecovery.com/avatars/Image_not_available.png", ImageUrl = "https://avatarrecovery.com/avatars/Image_not_available.png", PcAssetUrl = item.FileLocation, AvatarId = item.AvatarId, AvatarName = "From cache no names", AvatarDescription = "Avatar is from the game cache no names are located", RecordCreated = item.AvatarDetected, ReleaseStatus = "????", UnityVersion = "????", QuestAssetUrl = "None", AuthorName = "Unknown", AuthorId = "Unknown Cache", FileSize = item.FileSize } };
                        list.Add(avatar);
                    }
                }
                avatars = list;
                avatarGrid.Rows.Clear();
            }
            else
            {
                List<AvatarModel> list = new List<AvatarModel>();
                foreach (var item in CacheScanner.autoAvatarIds)
                {
                    if (item != null)
                    {
                        AvatarModel avatar = new AvatarModel { Tags = new List<string>(), Avatar = new AvatarDetails { ThumbnailUrl = "https://avatarrecovery.com/avatars/Image_not_available.png", ImageUrl = "https://avatarrecovery.com/avatars/Image_not_available.png", PcAssetUrl = item.FileLocation, AvatarId = item.AvatarId, AvatarName = "From cache no names", AvatarDescription = "Avatar is from the game cache no names are located", RecordCreated = item.AvatarDetected, ReleaseStatus = "????", UnityVersion = "????", QuestAssetUrl = "None", AuthorName = "Unknown", AuthorId = "Unknown Cache", FileSize = item.FileSize } };
                        list.Add(avatar);
                    }
                }
                avatars = avatars.Concat(list).ToList();
            }
            if (avatars != null)
            {
                SendMessage(avatarGrid.Handle, WM_SETREDRAW, false, 0);
                LoadData(true);
                SendMessage(avatarGrid.Handle, WM_SETREDRAW, true, 0);
                avatarGrid.Refresh();
            }

            uploadedAvatars = 0;
            alreadyOnApi = 0;
            FromApi = 0;
            if (string.IsNullOrEmpty(StaticValues.Config.Config.AuthKey))
            {
                return false;
            }
            SQLite.Setup();

            if (!StaticValues.Config.Config.IndexAdded)
            {
                SQLite.CreateIndex();
                StaticValues.Config.Config.IndexAdded = true;
                StaticValues.Config.Save();
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                cacheList = new List<AvatarModel>();
                for (int i = 0; i < avatarGrid.Rows.Count; i++)
                {
                    try
                    {
                        if (!loadedImage.Contains(avatarGrid.Rows[i].Cells[3].Value.ToString()))
                        {
                            if (avatarGrid.Rows[i] != null)
                            {
                                if (avatarGrid.Rows[i].Cells[3].Value != null && (bool)avatarGrid.Rows[i].Cells[9].Value == true)
                                {
                                    string avatarId = avatarGrid.Rows[i].Cells[3].Value.ToString();
                                    VRChatCacheResult local = null;
                                    try
                                    {
                                        local = DbCheckAvatar(avatarId);
                                    }
                                    catch { }
                                    VRChatCacheResult vRChatCacheResult = null;
                                    if (local == null)
                                    {
                                        vRChatCacheResult = GetDetails(avatarId);
                                    }
                                    else
                                    {
                                        vRChatCacheResult = local;
                                    }

                                    if (vRChatCacheResult != null)
                                    {
                                        if (local == null)
                                        {
                                            SaveAvatarData(vRChatCacheResult);
                                            UploadCacheResultAvatar(vRChatCacheResult);
                                        }
                                        GetAvatarInfo(vRChatCacheResult, avatarId, avatarGrid.Rows[i]);
                                    }
                                    if (vRChatCacheResult == null)
                                    {
                                        if (string.IsNullOrEmpty(StaticValues.Config.Config.ApiKey))
                                        {
                                            vRChatCacheResult = GetDetailsApi(avatarId);
                                            if (vRChatCacheResult != null)
                                            {
                                                if (local == null)
                                                {
                                                    SaveAvatarData(vRChatCacheResult);
                                                }
                                                GetAvatarInfo(vRChatCacheResult, avatarId, avatarGrid.Rows[i]);
                                            }
                                        }
                                    }
                                    // just incase images are already took
                                    if (vRChatCacheResult == null)
                                    {
                                        LoadImageCache(avatarId, avatarGrid.Rows[i]);
                                    }
                                }
                                if (avatarGrid.Rows[i].Cells[3].Value != null && (bool)avatarGrid.Rows[i].Cells[9].Value == false)
                                {
                                    string worldId = avatarGrid.Rows[i].Cells[3].Value.ToString();
                                    VRChatCacheResultWorld local = null;
                                    try
                                    {
                                        local = DbCheckWorld(worldId);
                                    }
                                    catch { }
                                    VRChatCacheResultWorld vRChatCacheResult = null;
                                    if (local == null)
                                    {
                                        vRChatCacheResult = GetDetailsWorlds(worldId);
                                    }
                                    else
                                    {
                                        vRChatCacheResult = local;
                                    }

                                    if (vRChatCacheResult != null)
                                    {
                                        if (local == null)
                                        {
                                            SaveWorldData(vRChatCacheResult);
                                            UploadCacheResultWorld(vRChatCacheResult);
                                        }
                                        GetWorldInfo(vRChatCacheResult, worldId, avatarGrid.Rows[i]);
                                    }
                                }
                            }
                            loadedImage.Add(avatarGrid.Rows[i].Cells[3].Value.ToString());
                        }
                    }
                    catch { }
                }
                try
                {
                    if (!bypassLoaded)
                    {
                        MessageBox.Show($"Finished Getting avatar information\nNewly added avatars {uploadedAvatars}\nAlready On API {alreadyOnApi}\nTotal Public + Self Private Found {uploadedAvatars + alreadyOnApi}\nTotal Private Found {FromApi}\nTotal left not able to grab details {avatars.Count() - uploadedAvatars - alreadyOnApi - FromApi}");
                    }
                }
                catch { }
            });

            lblLocalDb.Text = StaticValues.Config.Config.AvatarsInLocalDatabase.ToString();
            lblLoggedMe.Text = StaticValues.Config.Config.AvatarsLoggedToApi.ToString();
            return true;
        }

        private void CacheMessages_Tick(object sender, EventArgs e)
        {
            if (CacheScanner.messages.Count == 0) return;
            lock (CacheScanner.messages)
            {
                foreach (var message in CacheScanner.messages)
                {
                    txtCacheScannerLog.Text = message + txtCacheScannerLog.Text;
                }
                CacheScanner.messages.Clear();
            }
        }

        private void txtClientVersion_TextChanged(object sender, EventArgs e)
        {
            StaticGameValues.GameVersion = txtClientVersion.Text;
        }

        private VRChatCacheResult GetDetails(string avatarId)
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

        private VRChatCacheResultWorld GetDetailsWorlds(string worldId)
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
                            "2019.4.40f1");
                    string jsonString = webClient.DownloadString(new Uri($"https://api.vrchat.cloud/api/1/worlds/{worldId}"));
                    VRChatCacheResultWorld vrChatCacheResult = JsonConvert.DeserializeObject<VRChatCacheResultWorld>(jsonString);
                    return vrChatCacheResult;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }

        private int FromApi = 0;

        private VRChatCacheResult GetDetailsApi(string avatarId)
        {
            AvatarSearch avatarSearch = new AvatarSearch { Key = StaticValues.Config.Config.ApiKey, Amount = 1, PrivateAvatars = true, PublicAvatars = true, ContainsSearch = false, DebugMode = true, PcAvatars = true, QuestAvatars = chkQuest.Checked, AvatarId = avatarId };
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.avatarrecovery.com/Avatar/GetKeyAvatar");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = $"ARC" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string jsonPost = JsonConvert.SerializeObject(avatarSearch);
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
                    else if (!avatarResponse.Authorized)
                    {
                        MessageBox.Show("The key you have entered is invalid");
                    }
                    if (avatarResponse.Avatars.Count > 0)
                    {
                        VRChatCacheResult vRChatCacheResult = new VRChatCacheResult
                        {
                            AssetUrl = avatarResponse.Avatars.FirstOrDefault().Avatar.PcAssetUrl,
                            AuthorId = avatarResponse.Avatars.FirstOrDefault().Avatar.AuthorId,
                            AuthorName = avatarResponse.Avatars.FirstOrDefault().Avatar.AuthorName,
                            CreatedAt = avatarResponse.Avatars.FirstOrDefault().Avatar.RecordCreated,
                            Description = avatarResponse.Avatars.FirstOrDefault().Avatar.AvatarDescription,
                            Featured = false,
                            Id = avatarId,
                            ImageUrl = avatarResponse.Avatars.FirstOrDefault().Avatar.ImageUrl,
                            Name = avatarResponse.Avatars.FirstOrDefault().Avatar.AvatarName,
                            ReleaseStatus = avatarResponse.Avatars.FirstOrDefault().Avatar.ReleaseStatus,
                            Tags = new List<string>(),
                            ThumbnailImageUrl = avatarResponse.Avatars.FirstOrDefault().Avatar.ThumbnailUrl,
                            UnityPackages = new List<UnityPackage> { new UnityPackage { AssetUrl = avatarResponse.Avatars.FirstOrDefault().Avatar.PcAssetUrl, Platform = "standalonewindows" } },
                            UnityPackageUrl = "",
                            UpdatedAt = avatarResponse.Avatars.FirstOrDefault().Avatar.RecordCreated,
                            Version = 1
                        };
                        FromApi++;
                        return vRChatCacheResult;
                    }
                    return null;
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
                return null;
            }
        }

        private void UploadCacheResultWorld(VRChatCacheResultWorld model)
        {
            AvatarDetailsSend avatarDetails = new AvatarDetailsSend
            {
                AuthorId = model.AuthorId,
                AuthorName = model.AuthorName,
                ImageUrl = model.ImageUrl,
                AvatarDescription = model.Description,
                AvatarId = model.Id,
                AvatarName = model.Name,
                RecordCreated = DateTime.Now,
                QuestAssetUrl = "None",
                ReleaseStatus = model.ReleaseStatus,
                ThumbnailUrl = model.ThumbnailImageUrl,
                Tags = String.Join(",", model.Tags)
            };
            if (model.UnityPackages != null)
            {
                if (model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "standalonewindows") != null)
                {
                    avatarDetails.PcAssetUrl = model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "standalonewindows").AssetUrl;
                    avatarDetails.UnityVersion = model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "standalonewindows").UnityVersion;
                }
            }
            if (string.IsNullOrEmpty(avatarDetails.Tags))
            {
                avatarDetails.Tags = "None";
            }

            if (!string.IsNullOrEmpty(avatarDetails.PcAssetUrl) || !string.IsNullOrEmpty(avatarDetails.QuestAssetUrl))
            {
                try
                {
                    string apiUrl = "https://api.avatarrecovery.com/Avatar/AddWorld";
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.UserAgent = $"ARC" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    string jsonPost = JsonConvert.SerializeObject(avatarDetails);
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
                            if (avatarResponse)
                            {
                                uploadedWorlds++;
                                Console.WriteLine("World Added");
                            }
                            else if (!avatarResponse)
                            {
                                alreadyOnApi++;
                                Console.WriteLine("World already on API");
                            }
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

        private void UploadCacheResultAvatar(VRChatCacheResult model)
        {
            AvatarDetailsSend avatarDetails = new AvatarDetailsSend
            {
                AuthorId = model.AuthorId,
                AuthorName = model.AuthorName,
                ImageUrl = model.ImageUrl,
                AvatarDescription = model.Description,
                AvatarId = model.Id,
                AvatarName = model.Name,
                RecordCreated = DateTime.Now,
                ReleaseStatus = model.ReleaseStatus,
                ThumbnailUrl = model.ThumbnailImageUrl,
                Tags = String.Join(",", model.Tags),
                FileSize = 0
            };

            if (!_logSelf)
            {
                if (StaticValues.Config.Config.UserId == avatarDetails.AuthorId)
                {
                    return;
                }
            }

            if (avatars != null)
            {
                var fileSize = avatars.SingleOrDefault(x => x.Avatar.AvatarId == model.Id);

                if (fileSize != null)
                {
                    avatarDetails.FileSize = new System.IO.FileInfo(fileSize.Avatar.PcAssetUrl).Length;
                }
            }

            if (model.UnityPackages != null)
            {
                if (model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "standalonewindows") != null)
                {
                    avatarDetails.PcAssetUrl = model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "standalonewindows").AssetUrl;
                    avatarDetails.UnityVersion = model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "standalonewindows").UnityVersion;
                }
                if (model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android") != null)
                {
                    avatarDetails.QuestAssetUrl = model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android").AssetUrl;
                    avatarDetails.UnityVersion = model.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android").UnityVersion;
                }
                else
                {
                    avatarDetails.QuestAssetUrl = "None";
                }
            }
            if (string.IsNullOrEmpty(avatarDetails.Tags))
            {
                avatarDetails.Tags = "None";
            }

            if (!string.IsNullOrEmpty(avatarDetails.PcAssetUrl) || (!string.IsNullOrEmpty(avatarDetails.QuestAssetUrl) && avatarDetails.QuestAssetUrl != "None"))
            {
                try
                {
                    string apiUrl = "https://api.avatarrecovery.com/Avatar/AddModel";
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.UserAgent = $"ARC" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    string jsonPost = JsonConvert.SerializeObject(avatarDetails);
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
                            if (avatarResponse)
                            {
                                uploadedAvatars++;
                                Console.WriteLine("Avatar Added");
                                StaticValues.Config.Config.AvatarsLoggedToApi++;
                                StaticValues.Config.Save();
                                lblLoggedMe.Text = StaticValues.Config.Config.AvatarsLoggedToApi.ToString();
                            }
                            else if (!avatarResponse)
                            {
                                alreadyOnApi++;
                                Console.WriteLine("Avatar already on API");
                                ArcClient.UpdateFileSize(avatarDetails.AvatarId, avatarDetails.FileSize);
                            }
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
                try
                {
                    if (avatarDetails.QuestAssetUrl != "")
                    {
                        using (var client = new WebClient())
                        {
                            string url = $"https://api.avatarrecovery.com/Avatar/AddQuestSideCheat?questUrl={System.Uri.EscapeDataString(avatarDetails.QuestAssetUrl)}&avatarId={System.Uri.EscapeDataString(avatarDetails.AvatarId)}";
                            var response = client.DownloadString(url);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private int uploadedAvatars = 0;
        private int uploadedWorlds = 0;
        private int alreadyOnApi = 0;

        private void btnParseImages_Click(object sender, EventArgs e)
        {
        }

        private VRChatCacheResult DbCheckAvatar(string avatarId)
        {
            string data = SQLite.ReadDataAvatar(avatarId);
            if (data != null)
            {
                alreadyOnApi++;
                return JsonConvert.DeserializeObject<VRChatCacheResult>(data);
            }

            return null;
        }

        private VRChatCacheResultWorld DbCheckWorld(string worldId)
        {
            string data = SQLite.ReadDataWorld(worldId);
            if (data != null)
            {
                alreadyOnApi++;
                return JsonConvert.DeserializeObject<VRChatCacheResultWorld>(data);
            }

            return null;
        }

        private void SaveAvatarData(VRChatCacheResult result)
        {
            string strJson = JsonConvert.SerializeObject(result);
            SQLite.WriteDataAvatar(result.Id, strJson);
            StaticValues.Config.Config.AvatarsInLocalDatabase++;
            StaticValues.Config.Save();
        }

        private void SaveWorldData(VRChatCacheResultWorld result)
        {
            string strJson = JsonConvert.SerializeObject(result);
            SQLite.WriteDataWorld(result.Id, strJson);
        }

        private void LoadImageCache(string avatarId, DataGridViewRow row)
        {
            string fileName = $"{StaticValues.ImagesDownloadFolder}{avatarId}.png";
            if (File.Exists(fileName))
            {
                Bitmap bmp = new Bitmap(fileName);
                row.Cells[0].Value = bmp;
            }
        }

        private void GetAvatarInfo(VRChatCacheResult vRChatCacheResult, string avatarId, DataGridViewRow row)
        {
            var temp = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarId);
            //avatars.Remove(temp);
            if (!Directory.Exists(StaticValues.ImagesDownloadFolder))
            {
                Directory.CreateDirectory(StaticValues.ImagesDownloadFolder);
            }
            AvatarModel avatar = new AvatarModel
            {
                Avatar = new AvatarDetails
                {
                    AvatarId = avatarId,
                    AuthorId = vRChatCacheResult.AuthorId,
                    AuthorName = vRChatCacheResult.AuthorName,
                    AvatarDescription = vRChatCacheResult.Description,
                    AvatarName = vRChatCacheResult.Name,
                    ImageUrl = vRChatCacheResult.ImageUrl,
                    ThumbnailUrl = vRChatCacheResult.ThumbnailImageUrl,
                    PcAssetUrl = temp.Avatar.PcAssetUrl,
                    QuestAssetUrl = temp.Avatar.QuestAssetUrl,
                    RecordCreated = temp.Avatar.RecordCreated,
                    ReleaseStatus = vRChatCacheResult.ReleaseStatus,
                    UnityVersion = ""
                },
                Tags = new List<string>()
            };

            if (vRChatCacheResult.UnityPackages != null)
            {
                if (vRChatCacheResult.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android") != null)
                {
                    avatar.Avatar.QuestAssetUrl = vRChatCacheResult.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android").AssetUrl;
                    avatar.Avatar.UnityVersion = vRChatCacheResult.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android").UnityVersion;
                }
                else
                {
                    avatar.Avatar.QuestAssetUrl = "None";
                }
            }

            int index = avatars.IndexOf(temp);
            if (index != -1)
            {
                avatars[index] = avatar;
            }

            cacheList.Add(avatar);
            row.Cells[1].Value = vRChatCacheResult.Name;
            row.Cells[2].Value = vRChatCacheResult.AuthorName;
            row.Cells[5].Value = vRChatCacheResult.ThumbnailImageUrl;

            string fileName = $"{StaticValues.ImagesDownloadFolder}{avatar.Avatar.AvatarId}.png";
            if (!File.Exists(fileName))
            {
                if (row.Cells[5].Value != null)
                {
                    if (!string.IsNullOrEmpty(row.Cells[5].Value.ToString().Trim()) && row.Cells[5].Value != "https://avatarrecovery.com/avatars/Image_not_available.png")
                    {
                        try
                        {
                            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(row.Cells[5].Value.ToString());
                            myRequest.Method = "GET";
                            myRequest.UserAgent = userAgent;
                            using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                            {
                                if (myResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    Bitmap bmp = new Bitmap(myResponse.GetResponseStream());
                                    row.Cells[0].Value = bmp;
                                    bmp.Save(fileName, ImageFormat.Png);
                                }
                                else
                                {
                                    Bitmap bmp = new Bitmap(Resources.No_Image);
                                    row.Cells[0].Value = bmp;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            try
                            {
                                Bitmap bmp = new Bitmap(Resources.No_Image);
                                row.Cells[0].Value = bmp;
                            }
                            catch (Exception exc) { Console.WriteLine(exc.Message); }
                        }
                    }
                    else
                    {
                        try
                        {
                            Bitmap bmp = new Bitmap(Resources.No_Image);
                            row.Cells[0].Value = bmp;
                        }
                        catch (Exception exc) { Console.WriteLine(exc.Message); }
                    }
                }
            }
            else
            {
                Bitmap bmp = new Bitmap(fileName);
                row.Cells[0].Value = bmp;
            }
        }

        private void GetWorldInfo(VRChatCacheResultWorld vRChatCacheResult, string avatarId, DataGridViewRow row)
        {
            var temp = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarId);
            //avatars.Remove(temp);
            AvatarModel avatar = new AvatarModel
            {
                Avatar = new AvatarDetails
                {
                    AvatarId = avatarId,
                    AuthorId = vRChatCacheResult.AuthorId,
                    AuthorName = vRChatCacheResult.AuthorName,
                    AvatarDescription = vRChatCacheResult.Description,
                    AvatarName = vRChatCacheResult.Name,
                    ImageUrl = vRChatCacheResult.ImageUrl,
                    ThumbnailUrl = vRChatCacheResult.ThumbnailImageUrl,
                    PcAssetUrl = temp.Avatar.PcAssetUrl,
                    QuestAssetUrl = temp.Avatar.QuestAssetUrl,
                    RecordCreated = temp.Avatar.RecordCreated,
                    ReleaseStatus = vRChatCacheResult.ReleaseStatus,
                    UnityVersion = ""
                },
                Tags = new List<string>()
            };

            if (vRChatCacheResult.UnityPackages != null)
            {
                if (vRChatCacheResult.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android") != null)
                {
                    avatar.Avatar.QuestAssetUrl = vRChatCacheResult.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android").AssetUrl;
                    avatar.Avatar.UnityVersion = vRChatCacheResult.UnityPackages.FirstOrDefault(x => x.Platform.ToLower() == "android").UnityVersion;
                }
                else
                {
                    avatar.Avatar.QuestAssetUrl = "None";
                }
            }
            int index = avatars.IndexOf(temp);
            if (index != -1)
            {
                avatars[index] = avatar;
            }

            cacheList.Add(avatar);
            row.Cells[1].Value = vRChatCacheResult.Name;
            row.Cells[2].Value = vRChatCacheResult.AuthorName;
            row.Cells[5].Value = vRChatCacheResult.ThumbnailImageUrl;

            if (row.Cells[5].Value != null)
            {
                if (!string.IsNullOrEmpty(row.Cells[5].Value.ToString().Trim()))
                {
                    try
                    {
                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(row.Cells[5].Value.ToString());
                        myRequest.Method = "GET";
                        myRequest.UserAgent = userAgent;
                        using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                        {
                            if (myResponse.StatusCode == HttpStatusCode.OK)
                            {
                                Bitmap bmp = new Bitmap(myResponse.GetResponseStream());
                                row.Cells[0].Value = bmp;
                            }
                            else
                            {
                                Bitmap bmp = new Bitmap(Resources.No_Image);
                                row.Cells[0].Value = bmp;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        try
                        {
                            Bitmap bmp = new Bitmap(Resources.No_Image);
                            row.Cells[0].Value = bmp;
                        }
                        catch (Exception exc) { Console.WriteLine(exc.Message); }
                    }
                }
            }
        }

        private void lblDownload_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/poiyomi/PoiyomiToonShader/releases/tag/V8.1.166");
        }

        private void avatarGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (avatarGrid.SelectedRows.Count == 1)
            {
                AvatarPreview avatarImage = new AvatarPreview((Bitmap)avatarGrid.SelectedRows[0].Cells[0].Value);
                avatarImage.Show();
            }
        }

        private void avatarGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Copy Avatar ID", new System.EventHandler(CopyAvatarId)));
                m.MenuItems.Add(new MenuItem("Request Avatar PC", new System.EventHandler(RequestAvatarDownloadPc)));
                m.MenuItems.Add(new MenuItem("Request Avatar Quest", new System.EventHandler(RequestAvatarDownloadQuest)));
                m.MenuItems.Add(new MenuItem("Preview Image", new System.EventHandler(PreviewImage)));
                m.MenuItems.Add(new MenuItem("Preview VRCA", new System.EventHandler(PreviewVRCA)));
                m.MenuItems.Add(new MenuItem("Hotswap 2022", new System.EventHandler(HotswapRC)));
                m.MenuItems.Add(new MenuItem("Hotswap 2019", new System.EventHandler(HotswapRC2019)));

                int currentMouseOverRow = avatarGrid.HitTest(e.X, e.Y).RowIndex;

                avatarGrid.ClearSelection();
                if (currentMouseOverRow != -1)
                {
                    avatarGrid.Rows[currentMouseOverRow].Selected = true;
                    m.Show(avatarGrid, new Point(e.X, e.Y));
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                //ARCClient.AvatarSizeAndVersions(avatarGrid, avatars, nmPcVersion, nmQuestVersion, txtAvatarSizePc, txtAvatarSizeQuest);
            }
        }

        private void PreviewImage(Object sender, EventArgs e)
        {
            if (avatarGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                AvatarPreview avatarImage = new AvatarPreview((Bitmap)avatarGrid.SelectedRows[0].Cells[0].Value);
                avatarImage.Show();
            }
        }

        private void RequestAvatarDownloadPc(Object sender, EventArgs e)
        {
            if (avatarGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                AvatarModel avatar = avatars.SingleOrDefault(x => x.Avatar.AvatarId == avatarGrid.SelectedRows[0].Cells[3].Value);
                if (!avatar.Avatar.PcAssetUrl.StartsWith("http"))
                {
                    MessageBox.Show("You don't need to request to download this as its already in your cache, just download the normal way");
                    return;
                }
                if (string.IsNullOrEmpty(avatar.Avatar.PcAssetUrl) && avatar.Avatar.PcAssetUrl.ToLower() != "none")
                {
                    MessageBox.Show("PC asset url doesn't exist");
                    return;
                }
                RequestAvatar requestAvatar = new RequestAvatar { AvatarId = avatar.Avatar.AvatarId, Key = new Guid(StaticValues.Config.Config.ApiKey), Quest = false };
                bool requested = arcApi.RequestAvatar(requestAvatar, _cookies);
                if (!requested)
                {
                    MessageBox.Show("You need to be premium member to do this");
                }
            }
        }

        private void RequestAvatarDownloadQuest(Object sender, EventArgs e)
        {
            if (avatarGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                AvatarModel avatar = avatars.SingleOrDefault(x => x.Avatar.AvatarId == avatarGrid.SelectedRows[0].Cells[3].Value);
                if (string.IsNullOrEmpty(avatar.Avatar.QuestAssetUrl) && avatar.Avatar.QuestAssetUrl.ToLower() != "none")
                {
                    MessageBox.Show("Quest asset url doesn't exist");
                    return;
                }
                RequestAvatar requestAvatar = new RequestAvatar { AvatarId = avatar.Avatar.AvatarId, Key = new Guid(StaticValues.Config.Config.ApiKey), Quest = true };
                bool requested = arcApi.RequestAvatar(requestAvatar, _cookies);
                if (!requested)
                {
                    MessageBox.Show("You need to be premium member to do this");
                }
                else
                {
                    MessageBox.Show("Avatar requested, keep an eye out on the Download queue tab");
                }
            }
        }

        private void PreviewVRCA(Object sender, EventArgs e)
        {
            if (avatarGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                Preview();
            }
        }

        private void HotswapRC(Object sender, EventArgs e)
        {
            if (avatarGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                if (avatarGrid.SelectedRows[0].Cells[3].Value.ToString().StartsWith("avtr_"))
                {
                    hotSwap();
                }
                else
                {
                    MessageBox.Show("World hotswapping support has been deprecated");
                }
            }
        }

        private void HotswapRC2019(Object sender, EventArgs e)
        {
            if (avatarGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                if (avatarGrid.SelectedRows[0].Cells[3].Value.ToString().StartsWith("avtr_"))
                {
                    hotSwap();
                }
                else
                {
                    MessageBox.Show("World hotswapping support has been deprecated");
                }
            }
        }

        private void CopyAvatarId(Object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(avatarGrid.SelectedRows[0].Cells[3].Value);
            }
            catch (ExternalException)
            {
                MessageBox.Show("Clipboard could not be accessed. Please try again.");
            }
        }

        private void btnGetScreenshots_Click(object sender, EventArgs e)
        {
            ScreenshotTaker();
        }

        private async Task<bool> ScreenshotTaker()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                string textFileData = "";
                foreach (DataGridViewRow item in avatarGrid.Rows)
                {
                    if (item.Cells[1] != null)
                    {
                        string avatarName = item.Cells[1].Value.ToString();
                        string avatarId = item.Cells[3].Value.ToString();
                        string fileName = $"{StaticValues.ImagesDownloadFolder}{avatarId}.png";
                        if (item.Cells[1].Value.ToString() == avatarName)
                        {
                            if (avatarId.ToLower().StartsWith("wrld_")) continue;
                            if (!File.Exists(fileName))
                            {
                                AvatarModel avatar = avatars.FirstOrDefault(x => x.Avatar.AvatarId == avatarId);

                                string screenshotLocation = $@"{StaticValues.VrcaViewer}AssetViewer_Data\avatarscreen.png";
                                string screenshotLocationNew = $"{StaticValues.ImagesDownloadFolder}{avatar.Avatar.AvatarId}.png";
                                textFileData = $"{textFileData}{avatar.Avatar.PcAssetUrl};{avatar.Avatar.AvatarId}{Environment.NewLine}";
                            }
                        }
                    }
                }

                string filePathAvatar = $"{StaticValues.VrcaViewer}avatarInfo.txt";

                if (File.Exists(filePathAvatar))
                {
                    try
                    {
                        File.Delete(filePathAvatar);
                    }
                    catch { }
                }

                File.WriteAllText(filePathAvatar, textFileData);

                try
                {
                    string commands = string.Format($"\"blank.vrca\" \"screen.shot\"");
                    Console.WriteLine(commands);
                    Process p = new Process();
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "AssetViewer.exe",
                        Arguments = commands,
                        WorkingDirectory = StaticValues.VrcaViewer,
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                    p.StartInfo = psi;
                    p.Start();
                    p.WaitForExit();
                    Console.WriteLine(p.ExitCode);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }



                string[] lines = File.ReadAllLines(filePathAvatar);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] avatarInfoSplit = line.Split(';');
                        string screenshotName = avatarInfoSplit[1];
                        string screenshotLocation = $"{StaticValues.VrcaViewer}AssetViewer_Data\\{screenshotName}.png";
                        string screenshotLocationNew = $"{StaticValues.ImagesDownloadFolder}{screenshotName}.png";
                        try
                        {
                            File.Move(screenshotLocation, screenshotLocationNew);
                        }
                        catch
                        {
                            try
                            {
                                string baseFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\error.png";
                                File.Copy(baseFile, screenshotLocationNew);
                            }
                            catch { }
                        }
                        Bitmap bmp = new Bitmap(screenshotLocationNew);
                        foreach (DataGridViewRow row in avatarGrid.Rows)
                        {
                            if (row.Cells[3].Value.ToString().Equals(screenshotName))
                            {
                                row.Cells[0].Value = bmp;
                                break;
                            }
                        }

                    }
                }
                MessageBox.Show("Screenshots done.");
            });

            return true;
        }

        private void chkAutoScan_CheckedChanged(object sender, EventArgs e)
        {
            if (cacheFolderAuto != null)
            {
                CacheScannerTimer.Enabled = chkAutoScan.Checked;
            }
            else if (chkAutoScan.Checked)
            {
                MessageBox.Show("Scan cache normally first");
                chkAutoScan.Checked = false;
            }
        }

        private void CacheScannerTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Scanning Cache");
            CacheScanner.NewMessage($"Auto Scanning Cache{Environment.NewLine}");
            ScanCacheAvatar(true);
        }

        private void txtCacheScannerLog_Click(object sender, EventArgs e)
        {
        }

        private void avatarGrid_Row(object sender, DataGridViewCellEventArgs e)
        {
            ArcClient.AvatarSizeAndVersions(avatarGrid, avatars, nmPcVersion, nmQuestVersion, txtAvatarSizePc, txtAvatarSizeQuest);
        }

        private void btnDownloadSafe_Click(object sender, EventArgs e)
        {
            SafeDownload();
        }



        private async Task<bool> SafeDownload()
        {
            if (dgSafeDownload.SelectedRows.Count >= 1)
            {
                foreach (DataGridViewRow row in dgSafeDownload.SelectedRows)
                {
                    if (!(bool)row.Cells[3].Value)
                    {
                        if ((bool)row.Cells[2].Value)
                        {
                            string avatarId = row.Cells[0].Value.ToString();
                            Download download = new Download { Text = $"{avatarId}" };
                            download.Show();
                            var filePath = $"{StaticValues.VrcaDownloadFolder}{avatarId}.vrca";
                            string url = $"https://vrca.avatarrecovery.com/SARS/{avatarId}";
                            if ((bool)row.Cells[1].Value)
                            {
                                url += "_quest.vrca";
                            }
                            else
                            {
                                url += "_pc.vrca";
                            }
                            await Task.Run(() => VRCA.DownloadVrcaFile(url, filePath, download.downloadProgress, true));
                            ShowSelectedInExplorer.FileOrFolder(filePath);
                            download.Close();
                        }
                        else
                        {
                            MessageBox.Show("Download not ready yet, try again later");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Downloading file failed, this is likely because its been deleted from VRChat");
                    }
                }
            }
            return true;
        }

        private void UpdateDownloadList()
        {
            GetRequests key = new GetRequests { Key = new Guid(StaticValues.Config.Config.ApiKey) };
            List<DownloadQueueList> download = arcApi.DownloadQueueRefresh(key, _cookies);
            dgSafeDownload.Rows.Clear();
            dgSafeDownload.AllowUserToAddRows = true;
            string alertText = "";
            foreach (var item in download)
            {
                if (item.Downloaded)
                {
                    if (!downloadQueue.Config.Download.Contains(item.AvatarId))
                    {
                        downloadQueue.Config.Download.Add(item.AvatarId);
                        downloadQueue.Save();
                        alertText = $"{alertText}{Environment.NewLine}{item.AvatarId}";
                    }
                }
                try
                {
                    DataGridViewRow row = (DataGridViewRow)dgSafeDownload.Rows[0].Clone();
                    row.Cells[0].Value = item.AvatarId;
                    row.Cells[1].Value = item.Quest;
                    row.Cells[2].Value = item.Downloaded;
                    row.Cells[3].Value = item.Failed;
                    dgSafeDownload.Rows.Add(row);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            dgSafeDownload.AllowUserToAddRows = false;
            if (alertText != "")
            {
                MessageBox.Show($"The following items have finished downloading {alertText}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (StaticValues.Config.Config.ApiKey != null)
            {
                UpdateDownloadList();
            }
            else
            {
                MessageBox.Show("Please enter your API Key");
            }
        }

        private bool _logSelf = true;

        private void chkSelfAvatars_CheckedChanged(object sender, EventArgs e)
        {
            _logSelf = !chkSelfAvatars.Checked;
        }

        private void DownloadRefresh_Tick(object sender, EventArgs e)
        {
            if (StaticValues.Config.Config != null && StaticValues.Config.Config.ApiKey != null)
            {
                UpdateDownloadList();
            }
            else
            {
                chkAutoRefreshDownload.Checked = false;
                DownloadRefresh.Enabled = false;
            }
        }

        private void chkAutoRefreshDownload_CheckedChanged(object sender, EventArgs e)
        {
            DownloadRefresh.Enabled = chkAutoRefreshDownload.Checked;
        }

        private void metroLabel21_Click(object sender, EventArgs e)
        {
            Process.Start("https://ko-fi.com/ShrekamusChrist");
        }

        private void btnHotswap2019_Click(object sender, EventArgs e)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists($"{StaticValues.ArcDocuments}{StaticValues.Config.Config.HotSwapName2022L}"))
            {
                AvatarFunctions.ExtractData2022L(StaticValues.Config.Config.HotSwapName2022L, $"{filePath}");
            }
            if (!Directory.Exists($"{StaticValues.ArcDocuments}{StaticValues.Config.Config.HotSwapName2019}"))
            {
                AvatarFunctions.ExtractData2019(StaticValues.Config.Config.HotSwapName2019, $"{filePath}");
            }
            hotSwap();
        }

        private void btnChangeUnity2019_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBoxManager.Yes = "Yes";
                MessageBoxManager.No = "No";
                MessageBoxManager.Register();
            }
            catch { }
            StaticValues.Config.Config.UnityLocation2019 = ArcClient.UnitySetup(unity2019);
            StaticValues.Config.Save();
        }

        private void chkUnlockPassword_CheckedChanged(object sender, EventArgs e)
        {
            //TODO REWORK            
            var key = arcApi.CheckKey(StaticValues.Config.Config.ApiKey);
            if (key == null)
            {
                MessageBox.Show("This feature is locked to donators");
                chkUnlockPassword.Checked = false;
            }
            if (key != null && !key.premium && chkUnlockPassword.Checked)
            {
                MessageBox.Show("This feature is locked to donators");
                chkUnlockPassword.Checked = false;
            }

            if (!chkUnlockPassword.Checked)
            {
                chkAdvanceUnlock.Checked = false;
                chkAdvancedDic.Checked = false;
            }

        }

        private void chkAdvanceUnlock_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkUnlockPassword.Checked && chkAdvanceUnlock.Checked)
            {
                chkAdvanceUnlock.Checked = false;
            }
        }

        private void txtApiKey_Click(object sender, EventArgs e)
        {

        }

        private void chkAdvancedDic_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkUnlockPassword.Checked && chkAdvancedDic.Checked)
            {
                chkAdvancedDic.Checked = false;
            }
        }

        private void LoopControlsAndTranslate(Control control, string languageCode)
        {
            switch (control)
            {
                case Button button:
                    button.Text = Translate.TranslateAppItem(button.Text, languageCode, button.Name);
                    break;
                case CheckBox checkBox:
                    checkBox.Text = Translate.TranslateAppItem(checkBox.Text, languageCode, checkBox.Name);
                    break;
                case GroupBox group:
                    group.Text = Translate.TranslateAppItem(group.Text, languageCode, group.Name);
                    break;
                case MetroLabel label2:
                    label2.Text = Translate.TranslateAppItem(label2.Text, languageCode, label2.Name);
                    break;
                case Label label:
                    label.Text = Translate.TranslateAppItem(label.Text, languageCode, label.Name);
                    break;
                case Panel panel:
                    panel.Text = Translate.TranslateAppItem(panel.Text, languageCode, panel.Name);
                    break;
                case TabControl tabcontrol:
                    tabcontrol.Text = Translate.TranslateAppItem(tabcontrol.Text, languageCode, tabcontrol.Name);
                    break;
            }
            foreach (Control child in control.Controls)
                LoopControlsAndTranslate(child, languageCode);
        }

        private bool AlreadyTranslated = false;
        private void btnTranslateApp_Click(object sender, EventArgs e)
        {
            if (AlreadyTranslated)
            {
                MessageBox.Show("Please restart the app to translate again");
                return;
            }
            string languageCode;
            try
            {
                languageCode = languageTranslations.FirstOrDefault(x => x.name == cbAppTranslate.Text).code;
            }
            catch { languageCode = "en"; }

            foreach (Control child in this.Controls)
                LoopControlsAndTranslate(child, languageCode);
            AlreadyTranslated = true;
        }

        private void btnAddPublic_Click(object sender, EventArgs e)
        {
            if (StaticValues.Config.Config.AuthKey == null) { MessageBox.Show("login with an alt first"); }
            if (!txtSearchTerm.Text.Contains("avtr_")) { MessageBox.Show("please enter an avatar id first"); }
            VRChatCacheResult vRChatCacheResult = GetDetails(txtSearchTerm.Text);

            if (vRChatCacheResult != null)
            {
                SaveAvatarData(vRChatCacheResult);
                UploadCacheResultAvatar(vRChatCacheResult);
                MessageBox.Show("ADDED");
            }
        }
        private void CookieChecker_Tick(object sender, EventArgs e)
        {
            if (arcApi == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                arcApi = new ArcApi(fileVersionInfo.FileVersion);
            }
            if (Guid == null)
            {
                Guid = Guid.NewGuid();
            }
            if (arcApi != null)
            {
                CookieAuth cookieAuth = arcApi.GetCookie(Guid.ToString());
                if (cookieAuth != null && cookieAuth.Cookie != null && cookieAuth.Key != null)
                {
                    StaticValues.Config.Config.ApiKey = cookieAuth.Key.ToString();

                    string[] cookie = cookieAuth.Cookie.Split(new string[] { ";" }, StringSplitOptions.None);
                    if (cookie != null)
                    {
                        if (_cookies == null)
                        {
                            _cookies = new CookieContainer();
                        }
                        foreach (string cookieValue in cookie)
                        {
                            string[] temp = cookieValue.Split(new string[] { "=" }, StringSplitOptions.None);
                            _cookies.Add(new Uri("https://api.avatarrecovery.com/"), new Cookie(HttpUtility.UrlEncode(temp[0]), $"{HttpUtility.UrlEncode(temp[1])}"));
                        }
                        CookieChecker.Enabled = false;
                        StaticValues.Config.Config.CookiesValues = cookie;
                        StaticValues.Config.Save();
                        MessageBox.Show("login successful");
                    }
                }
            }
            else
            {
                CookieChecker.Enabled = false;
                MessageBox.Show("Program error, try either use a VPN or disconnect from VPN if already using one");
            }
        }

        private void AvatarSystem_Shown(object sender, EventArgs e)
        {


            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, avatarGrid, new object[] { true });
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            if (filePath.ToLower().Contains("\\local\\temp"))
            {
                MessageBox.Show("EXTRACT THE PROGRAM FIRST");
                Close();
            }
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                SystemName = $"Avatar Recovery Client (A.R.C) V{fileVersionInfo.ProductVersion}";
                this.Text = SystemName;
                txtAbout.Text = Resources.About;
                cbSearchTerm.SelectedIndex = 0;
                cbLimit.SelectedIndex = 5;
                tabControl.SelectedIndex = 0;
                arcApi = new ArcApi(fileVersionInfo.FileVersion);
            }
            catch { }

            SetupConfig();
            LoadSettings();

            if (string.IsNullOrEmpty(StaticValues.Config.Config.HotSwapName2022L))
            {
                int randomAmount = RandomFunctions.random.Next(8);
                StaticValues.Config.Config.HotSwapName2022L = RandomFunctions.RandomString(randomAmount);
                StaticValues.Config.Save();
            }

            if (string.IsNullOrEmpty(StaticValues.Config.Config.HotSwapName2019))
            {
                int randomAmount = RandomFunctions.random.Next(8);
                StaticValues.Config.Config.HotSwapName2019 = RandomFunctions.RandomString(randomAmount);
                StaticValues.Config.Save();
            }

            try
            {
                CheckFav();
            }
            catch { }
            try
            {
                CheckRipped();
            }
            catch { }



            if (File.Exists(SQLite._databaseLocation))
            {
                StaticValues.Config.Config.AvatarsInLocalDatabase = SQLite.CountAvatars();
                StaticValues.Config.Save();
            }

            try
            {

                lblLocalDb.Text = StaticValues.Config.Config.AvatarsInLocalDatabase.ToString();
                lblLoggedMe.Text = StaticValues.Config.Config.AvatarsLoggedToApi.ToString();
            }
            catch { }



            try
            {
                var databaseStats = arcApi.DatabaseStats();

                if (databaseStats != null)
                {
                    lblPublic.Text = databaseStats.TotalPublic.ToString();
                    lblPrivate.Text = databaseStats.TotalPrivate.ToString();
                    lblSize.Text = databaseStats.TotalDatabase.ToString();
                }
            }
            catch { }

            try
            {
                MessageBoxManager.Yes = "PC";
                MessageBoxManager.No = "Quest";
                MessageBoxManager.Register();
            }
            catch { }

            try
            {
                languageTranslations = arcApi.LanguageList();
                foreach (var language in languageTranslations)
                {
                    cbLanguage.Items.Add(language.name);
                    cbAppTranslate.Items.Add(language.name);

                }
                cbLanguage.SelectedIndex = 0;
            }
            catch { }

            SetupDocumentLocation();

            if (StaticValues.Config.Config.ViewerVersion != 5)
            {
                ArcClient.ClearOldViewer();
                StaticValues.Config.Config.ViewerVersion = 5;
                StaticValues.Config.Save();
            }

            try
            {
                ArcClient.ExtractViewer();
            }
            catch { }

            if (StaticValues.Config.Config.RipperVersion != 3)
            {
                ArcClient.ClearOldRipper();
                StaticValues.Config.Config.RipperVersion = 3;
                StaticValues.Config.Save();
            }

            try
            {
                ArcClient.ExtractRipper();
            }
            catch { }

            _cookies = new CookieContainer();

            if (StaticValues.Config.Config.CookiesValues != null && StaticValues.Config.Config.ApiKey != null)
            {
                foreach (string cookieValue in StaticValues.Config.Config.CookiesValues)
                {
                    string[] temp = cookieValue.Split(new string[] { "=" }, StringSplitOptions.None);
                    _cookies.Add(new Uri("https://api.avatarrecovery.com/"), new Cookie(HttpUtility.UrlEncode(temp[0]), $"{HttpUtility.UrlEncode(temp[1])}"));
                }
            }

            DownloadRefresh.Enabled = true;
        }

        private void SetupDocumentLocation()
        {
            if (!string.IsNullOrEmpty(StaticValues.Config.Config.DocumentLocation))
            {
                StaticValues.ArcDocuments = $"{StaticValues.Config.Config.DocumentLocation}";
                StaticValues.VrcaDownloadFolder = $"{StaticValues.ArcDocuments}VRCA\\";
                StaticValues.VrcwDownloadFolder = $"{StaticValues.ArcDocuments}VRCW\\";
                StaticValues.ImagesDownloadFolder = $"{StaticValues.ArcDocuments}Images\\";
                StaticValues.UnityProject = $"{StaticValues.ArcDocuments}Unity\\";
                StaticValues.VrcaViewer = $"{StaticValues.ArcDocuments}Viewer\\";
                StaticValues.AssetRipper = $"{StaticValues.ArcDocuments}AssetRipper\\";
                StaticValues.ExtractedFiles = $"{StaticValues.ArcDocuments}ExtractedFiles\\";
                SetupNewLocation();
            }
        }

        private void SetupConfig()
        {

            try
            {
                StaticValues.Config = new ConfigSave<Config>(StaticValues.ConfigLocation);
            }
            catch
            {
                MessageBox.Show("Error with config file, settings reset");
                if (File.Exists(StaticValues.ConfigLocation))
                {
                    File.Delete(StaticValues.ConfigLocation);
                }
                Console.WriteLine("Error with config");
            }

            try
            {
                downloadQueue = new ConfigSave<ListDown>(StaticValues.DownloadLocation);
                if (downloadQueue.Config.Download == null)
                {
                    downloadQueue.Config.Download = new List<string>();
                }
            }
            catch
            {
                MessageBox.Show("Error with download file, list reset");
                File.Delete(StaticValues.DownloadLocation);
                Console.WriteLine("Error with download");
            }

            try
            {
                StaticValues.RippedList = new ConfigSave<List<AvatarModel>>(StaticValues.RippedLocation);
            }
            catch
            {
                MessageBox.Show("Error with ripped file, ripped list reset");
                File.Delete(StaticValues.RippedLocation);
                StaticValues.RippedList = new ConfigSave<List<AvatarModel>>(StaticValues.RippedLocation);
                Console.WriteLine("Error with ripped list");
            }

            try
            {
                StaticValues.FavList = new ConfigSave<List<AvatarModel>>(StaticValues.FavLocation);
            }
            catch
            {
                MessageBox.Show("Error with favorites file, favorites list reset");
                File.Delete(StaticValues.FavLocation);
                StaticValues.FavList = new ConfigSave<List<AvatarModel>>(StaticValues.FavLocation);
                Console.WriteLine("Error with fav list");
            }
        }

        private void btnDocumentLocation_Click(object sender, EventArgs e)
        {
            var folderDlg = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };
            var result = folderDlg.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                txtDocumentLocation.Text = $"{folderDlg.FileName}\\";
                StaticValues.Config.Config.DocumentLocation = $"{folderDlg.FileName}\\";
                StaticValues.Config.Save();

                StaticValues.ArcDocuments = StaticValues.Config.Config.DocumentLocation;
                StaticValues.VrcaDownloadFolder = $"{StaticValues.ArcDocuments}VRCA\\";
                StaticValues.VrcwDownloadFolder = $"{StaticValues.ArcDocuments}VRCW\\";
                StaticValues.ImagesDownloadFolder = $"{StaticValues.ArcDocuments}Images\\";
                StaticValues.UnityProject = $"{StaticValues.ArcDocuments}Unity\\";
                StaticValues.VrcaViewer = $"{StaticValues.ArcDocuments}Viewer\\";
                StaticValues.AssetRipper = $"{StaticValues.ArcDocuments}AssetRipper\\";
                StaticValues.ExtractedFiles = $"{StaticValues.ArcDocuments}ExtractedFiles\\";
                SetupNewLocation();
                ArcClient.ExtractViewer();
                ArcClient.ExtractRipper();
            }

            if (string.IsNullOrEmpty(StaticValues.Config.Config.DocumentLocation))
            {
                StaticValues.Config.Config.DocumentLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\";
                StaticValues.Config.Save();
                StaticValues.ArcDocuments = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\";
                StaticValues.VrcaDownloadFolder = $"{StaticValues.ArcDocuments}VRCA\\";
                StaticValues.VrcwDownloadFolder = $"{StaticValues.ArcDocuments}VRCW\\";
                StaticValues.ImagesDownloadFolder = $"{StaticValues.ArcDocuments}Images\\";
                StaticValues.UnityProject = $"{StaticValues.ArcDocuments}Unity\\";
                StaticValues.VrcaViewer = $"{StaticValues.ArcDocuments}Viewer\\";
                StaticValues.AssetRipper = $"{StaticValues.ArcDocuments}AssetRipper\\";
                StaticValues.ExtractedFiles = $"{StaticValues.ArcDocuments}ExtractedFiles\\";
            }
        }

        private void SetupNewLocation()
        {
            CreateDirectoryIfNotExist(StaticValues.ArcDocuments);
            CreateDirectoryIfNotExist(StaticValues.VrcaDownloadFolder);
            CreateDirectoryIfNotExist(StaticValues.VrcwDownloadFolder);
            CreateDirectoryIfNotExist(StaticValues.ImagesDownloadFolder);
            CreateDirectoryIfNotExist(StaticValues.UnityProject);
            CreateDirectoryIfNotExist(StaticValues.VrcaViewer);
            CreateDirectoryIfNotExist(StaticValues.AssetRipper);
            CreateDirectoryIfNotExist(StaticValues.ExtractedFiles);
        }

        private void btnClearDocument_Click(object sender, EventArgs e)
        {
            txtDocumentLocation.Text = "";
            StaticValues.Config.Config.DocumentLocation = null;
            StaticValues.Config.Save();

            StaticValues.ArcDocuments = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\";
            StaticValues.VrcaDownloadFolder = $"{StaticValues.ArcDocuments}VRCA\\";
            StaticValues.VrcwDownloadFolder = $"{StaticValues.ArcDocuments}VRCW\\";
            StaticValues.ImagesDownloadFolder = $"{StaticValues.ArcDocuments}Images\\";
            StaticValues.UnityProject = $"{StaticValues.ArcDocuments}Unity\\";
            StaticValues.VrcaViewer = $"{StaticValues.ArcDocuments}Viewer\\";
            StaticValues.AssetRipper = $"{StaticValues.ArcDocuments}AssetRipper\\";
            StaticValues.ExtractedFiles = $"{StaticValues.ArcDocuments}ExtractedFiles\\";
        }

        private void btnUnityLocation2022L_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBoxManager.Yes = "Yes";
                MessageBoxManager.No = "No";
                MessageBoxManager.Register();
            }
            catch { }
            StaticValues.Config.Config.UnityLocation2022L = ArcClient.UnitySetup(unity2022L);
            StaticValues.Config.Save();
        }

        private void btnOpen2019_Click(object sender, EventArgs e)
        {
            OpenUnity(StaticValues.Config.Config.UnityLocation2019, StaticValues.Config.Config.HotSwapName2019, StaticValues.Config.Config.DocumentLocation);
        }

        private void OpenUnity(string unityLocation, string hotswapName, string arcDocuments)
        {
            var tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                .Replace("\\Roaming", "");

            CopyFiles(arcDocuments, hotswapName);

            string commands = string.Format("/C \"{0}\" -ProjectPath " + hotswapName, unityLocation);

            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "CMD.EXE",
                Arguments = commands,
                WorkingDirectory = arcDocuments,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            process.StartInfo = processStartInfo;
            process.Start();
        }

        public void CopyFiles(string arcDocuments, string hotswapName)
        {
            try
            {
                File.Copy($@"{AppContext.BaseDirectory}\Template\Scene.unity", $"{arcDocuments}\\{hotswapName}\\Assets\\Scene.unity", true);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }
        }

        private void btnOpen2022L_Click(object sender, EventArgs e)
        {
            OpenUnity(StaticValues.Config.Config.UnityLocation2022L, StaticValues.Config.Config.HotSwapName2022L, StaticValues.Config.Config.DocumentLocation);
        }

    }
}