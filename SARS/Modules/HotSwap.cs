using AssetsTools.NET;
using AssetsTools.NET.Extra;
using SARS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Windows.Forms;
using VRChatAPI_New;

namespace SARS.Modules
{
    public static class HotSwap
    {
        public static string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string firstLineReplace;
        public static async void HotswapProcess(HotswapConsole hotSwapConsole, AvatarSystem avatarSystem, string avatarFile, string customAvatarId = null, string imgFileLocation = null, string avatarName = null, string ReuploaderVrChat = null, bool replaceUnityVer = false)
        {
            var fileDecompressed = filePath + @"\decompressed.vrca";
            var fileDecompressed2 = filePath + @"\decompressed1.vrca";
            var fileDecompressedFinal = filePath + @"\finalDecompressed.vrca";
            var fileDummy = filePath + @"\dummy.vrca";
            var fileTarget = filePath + @"\target.vrca";
            var tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                .Replace("\\Roaming", "");
            var unityVrca = tempFolder + $"\\Local\\Temp\\DefaultCompany\\{avatarSystem.configSave.Config.HotSwapName}\\custom.vrca";
            var regexId = @"avtr_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}";
            var regexPrefabId = @"prefab-id-v1_avtr_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}_[\d]{10}\.prefab";
            var regexCab = @"CAB-[\w]{32}";
            var regexUnity = @"20[\d]{2}\.[\d]\.[\d]{2}f[\d]";
            var avatarIdRegex = new Regex(regexId);
            var avatarPrefabIdRegex = new Regex(regexPrefabId);
            var avatarCabRegex = new Regex(regexCab);
            var unityRegex = new Regex(regexUnity);

            RandomFunctions.tryDelete(fileDecompressed);
            RandomFunctions.tryDelete(fileDecompressed2);
            RandomFunctions.tryDelete(fileDecompressedFinal);
            RandomFunctions.tryDelete(fileDummy);
            RandomFunctions.tryDelete(fileTarget);
            MatchModel matchModelNew = null;
            if (customAvatarId == null)
            {
                try
                {
                    File.Copy(unityVrca, fileDummy);
                }
                catch
                {
                    MessageBox.Show("Make sure you've started the build and publish on unity", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (hotSwapConsole.InvokeRequired)
                        hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                    return;
                }

                try
                {
                    SafeWrite(hotSwapConsole.txtStatusText, $"Step 1 - Decompressing Unity temp VRCA!{Environment.NewLine}");
                    DecompressToFileStr(fileDummy, fileDecompressed, hotSwapConsole);
                }
                catch (Exception ex)
                {
                    //CoreFunctions.WriteLog(string.Format("{0}", ex.Message), this);
                    MessageBox.Show("Error decompressing VRCA file" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (hotSwapConsole.InvokeRequired)
                        hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                    return;
                }

                matchModelNew = getMatches(fileDecompressed, avatarIdRegex, avatarCabRegex, unityRegex,
                    avatarPrefabIdRegex, true);
            }

            try
            {
                SafeWrite(hotSwapConsole.txtStatusText, $"Step 2 - Decompressing selected VRCA!{Environment.NewLine}");
                DecompressToFileStr(avatarFile, fileDecompressed2, hotSwapConsole);
            }
            catch (Exception ex)
            {
                //CoreFunctions.WriteLog(string.Format("{0}", ex.Message), this);
                MessageBox.Show("Error decompressing VRCA file" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (hotSwapConsole.InvokeRequired)
                    hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                return;
            }

            var matchModelOld = getMatches(fileDecompressed2, avatarIdRegex, avatarCabRegex, unityRegex,
                avatarPrefabIdRegex, false);
            if (matchModelOld.UnityVersion == null)
            {
                var dialogResult = MessageBox.Show("Possible risky hotswap detected", "Risky Upload",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.Cancel)
                {
                    if (hotSwapConsole.InvokeRequired)
                        hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                    return;
                }
                replaceUnityVer = false;
            }

            if (matchModelOld.UnityVersion != null)
                if (matchModelOld.UnityVersion.Contains("2017.") || matchModelOld.UnityVersion.Contains("2018."))
                {
                    var dialogResult = MessageBox.Show(
                        "Replace 2017-2018 unity version, replacing this can cause issues but not replacing it can also increase a ban chance (Press OK to replace and cancel to skip replacements)",
                        "Possible 2017-2018 unity issue", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Cancel) { matchModelOld.UnityVersion = null; replaceUnityVer = false; } 
                }
            if (matchModelNew == null)
            {
                matchModelNew = new MatchModel
                {
                    UnityVersion = "2019.4.31f1",
                    AvatarAssetId = "prefab-id-v1_" + customAvatarId + "_" + RandomString(10) + ".prefab",
                    AvatarCab = "CAB-" + RandomString(32),
                    AvatarId = customAvatarId
                };
            }

            GetReadyForCompress(fileDecompressed2, fileDecompressedFinal, matchModelOld, matchModelNew, replaceUnityVer);

            try
            {
                CompressBundle(fileDecompressedFinal, fileTarget, hotSwapConsole);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error compressing VRCA file\n{ex.Message}");
                if (hotSwapConsole.InvokeRequired)
                    hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                return;
            }

            try
            {
                File.Copy(fileTarget, unityVrca, true);
            }
            catch
            {
            }

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(fileTarget).Length;
            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            var compressedSize = $"{len:0.##} {sizes[order]}";

            len = new FileInfo(fileDecompressed2).Length;
            order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            var uncompressedSize = $"{len:0.##} {sizes[order]}";

            if (avatarSystem.avatars != null)
            {
                var avatar = avatarSystem.avatars.FirstOrDefault(x => x.Avatar.AvatarId == matchModelOld.AvatarId);
                if (avatar != default && avatar != null)
                {
                    avatarSystem.rippedList.Config.Add(avatar);
                    avatarSystem.rippedList.Save();
                }
            }

            RandomFunctions.tryDelete(fileDecompressed);
            RandomFunctions.tryDelete(fileDecompressed2);
            RandomFunctions.tryDelete(fileDecompressedFinal);
            RandomFunctions.tryDelete(fileDummy);
            if (customAvatarId == null)
            {
                RandomFunctions.tryDelete(fileTarget);
            }

            if (hotSwapConsole.InvokeRequired)
                hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });

            if (customAvatarId == null)
            {
                MessageBox.Show($"Got file sizes, comp:{compressedSize}, decomp:{uncompressedSize}", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static async void HotswapWorldProcess(HotswapConsole hotSwapConsole, AvatarSystem avatarSystem, string worldFile, string customWorldId = null)
        {

            var fileDecompressed = filePath + @"\decompressed.vrcw";
            var fileDecompressed2 = filePath + @"\decompressed1.vrcw";
            var fileDecompressedFinal = filePath + @"\finalDecompressed.vrcw";
            var fileDummy = filePath + @"\dummy.vrcw";
            var fileTarget = filePath + @"\target.vrcw";
            var tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                .Replace("\\Roaming", "");
            var unityVrcw = tempFolder + $"\\LocalLow\\VRChat\\VRChat\\Worlds\\scene-StandaloneWindows64-Scene.vrcw";
            var regexId = @"wrld_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}";
            var regexUnity = @"20[\d]{2}\.[\d]\.[\d]{2}f[\d]";
            var worldIdRegex = new Regex(regexId);
            var unityRegex = new Regex(regexUnity);

            RandomFunctions.tryDelete(fileDecompressed);
            RandomFunctions.tryDelete(fileDecompressed2);
            RandomFunctions.tryDelete(fileDecompressedFinal);
            RandomFunctions.tryDelete(fileDummy);
            RandomFunctions.tryDelete(fileTarget);
            MatchModel matchModelNew = null;
            if (customWorldId == null)
            {
                try
                {
                    File.Copy(unityVrcw, fileDummy);
                }
                catch
                {
                    MessageBox.Show("Make sure you've started the build and publish on unity", "ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (hotSwapConsole.InvokeRequired)
                        hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                    return;
                }

                try
                {
                    DecompressToFileStr(fileDummy, fileDecompressed, hotSwapConsole);
                }
                catch (Exception ex)
                {
                    //CoreFunctions.WriteLog(string.Format("{0}", ex.Message), this);
                    MessageBox.Show("Error decompressing VRCW file" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (hotSwapConsole.InvokeRequired)
                        hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                    return;
                }

                matchModelNew = getMatchesWorld(fileDecompressed, worldIdRegex, unityRegex);
            }

            try
            {
                DecompressToFileStr(worldFile, fileDecompressed2, hotSwapConsole);
            }
            catch (Exception ex)
            {
                //CoreFunctions.WriteLog(string.Format("{0}", ex.Message), this);
                MessageBox.Show("Error decompressing VRCW file" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (hotSwapConsole.InvokeRequired)
                    hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                return;
            }

            var matchModelOld = getMatchesWorld(fileDecompressed2, worldIdRegex, unityRegex);
            if (matchModelOld.UnityVersion == null)
            {
                var dialogResult = MessageBox.Show("Possible risky hotswap detected", "Risky Upload",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.Cancel)
                {
                    if (hotSwapConsole.InvokeRequired)
                        hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                    return;
                }
            }

            if (matchModelOld.UnityVersion != null)
                if (matchModelOld.UnityVersion.Contains("2017.") || matchModelOld.UnityVersion.Contains("2018."))
                {
                    var dialogResult = MessageBox.Show(
                        "Replace 2017-2018 unity version, replacing this can cause issues but not replacing it can also increase a ban chance (Press OK to replace and cancel to skip replacements)",
                        "Possible 2017-2018 unity issue", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Cancel) { matchModelOld.UnityVersion = null; }
                }

            if (matchModelNew == null)
            {
                matchModelNew = new MatchModel
                {
                    UnityVersion = "2019.4.31f1",
                    AvatarId = customWorldId
                };
            }

            GetReadyForCompressWorld(fileDecompressed2, fileDecompressedFinal, matchModelOld, matchModelNew);

            try
            {
                CompressBundle(fileDecompressedFinal, fileTarget, hotSwapConsole);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error compressing VRCW file\n{ex.Message}");
                if (hotSwapConsole.InvokeRequired)
                    hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });
                return;
            }

            try
            {
                File.Copy(fileTarget, unityVrcw, true);
            }
            catch
            {
            }

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(fileTarget).Length;
            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            var compressedSize = $"{len:0.##} {sizes[order]}";

            len = new FileInfo(fileDecompressed2).Length;
            order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            var uncompressedSize = $"{len:0.##} {sizes[order]}";

            if (avatarSystem.avatars != null)
            {
                var avatar = avatarSystem.avatars.FirstOrDefault(x => x.Avatar.AvatarId == matchModelOld.AvatarId);
                if (avatar != default && avatar != null)
                {
                    avatarSystem.rippedList.Config.Add(avatar);
                    avatarSystem.rippedList.Save();
                }
            }

            RandomFunctions.tryDelete(fileDecompressed);
            RandomFunctions.tryDelete(fileDecompressed2);
            RandomFunctions.tryDelete(fileDecompressedFinal);
            RandomFunctions.tryDelete(fileDummy);
            if (customWorldId == null)
            {
                RandomFunctions.tryDelete(fileTarget);
            }

            if (hotSwapConsole.InvokeRequired)
                hotSwapConsole.Invoke((MethodInvoker)delegate { hotSwapConsole.Close(); });

            if (customWorldId == null)
            {
                MessageBox.Show($"Got file sizes, comp:{compressedSize}, decomp:{uncompressedSize}", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }

        private static MatchModel getMatches(string file, Regex avatarId, Regex avatarCab, Regex unityVersion,
            Regex avatarAssetId, bool firstRead)
        {
            MatchCollection avatarIdMatch = null;
            MatchCollection avatarAssetIdMatch = null;
            MatchCollection avatarCabMatch = null;
            MatchCollection unityMatch = null;
            var enc = Encoding.GetEncoding(28591);
            bool firstLine = true;
            var unityCount = 0;

            using (var vReader = new StreamReaderOver(file, enc))
            {
                while (!vReader.EndOfStream)
                {
                    var vLine = vReader.ReadLine();
                    if (firstLine && firstRead)
                    {
                        int indexCount = vLine.IndexOf("2019.4.31f1");
                        firstLineReplace = vLine.Substring(0, indexCount);
                        firstLine = false;
                    }
                    var tempId = avatarId.Matches(vLine);
                    var tempAssetId = avatarAssetId.Matches(vLine);
                    var tempCab = avatarCab.Matches(vLine);
                    var tempUnity = unityVersion.Matches(vLine);
                    if (tempAssetId.Count > 0) avatarAssetIdMatch = tempAssetId;
                    if (tempId.Count > 0) avatarIdMatch = tempId;
                    if (tempCab.Count > 0) avatarCabMatch = tempCab;
                    if (tempUnity.Count > 0)
                    {
                        unityMatch = tempUnity;
                        unityCount++;
                    }
                }
            }

            if (avatarAssetIdMatch == null) avatarAssetIdMatch = avatarIdMatch;

            var matchModel = new MatchModel
            {
                AvatarId = avatarIdMatch[0].Value,
                AvatarCab = avatarCabMatch[0].Value,
                AvatarAssetId = avatarAssetIdMatch[0].Value
            };

            if (unityMatch != null) matchModel.UnityVersion = unityMatch[0].Value;
            return matchModel;
        }
        private static MatchModel getMatchesWorld(string file, Regex worldId, Regex unityVersion)
        {
            MatchCollection avatarIdMatch = null;
            MatchCollection unityMatch = null;
            var unityCount = 0;

            foreach (var line in File.ReadLines(file))
            {
                var tempId = worldId.Matches(line);
                var tempUnity = unityVersion.Matches(line);

                if (tempId.Count > 0) avatarIdMatch = tempId;

                if (tempUnity.Count > 0)
                {
                    unityMatch = tempUnity;
                    unityCount++;
                }
            }

            var matchModel = new MatchModel
            {
                AvatarId = avatarIdMatch[0].Value
            };

            if (unityMatch != null) matchModel.UnityVersion = unityMatch[0].Value;
            return matchModel;
        }

        private static void GetReadyForCompress(string oldFile, string newFile, MatchModel old, MatchModel newModel, bool unityReplace = false)
        {
            var enc = Encoding.GetEncoding(28591);
            bool firstLine = unityReplace;
            using (var vReader = new StreamReaderOver(oldFile, enc))
            {
                using (var vWriter = new StreamWriter(newFile, false, enc))
                {
                    while (!vReader.EndOfStream)
                    {
                        var vLine = vReader.ReadLine();
                        var replace = CheckAndReplaceLine(vLine, old, newModel, unityReplace);
                        if (firstLine && unityReplace)
                        {
                            int indexCount = replace.IndexOf("2022.3.5f1-DWR");
                            if(indexCount == -1)
                            {
                                indexCount = replace.IndexOf("2019.4.40f1");
                            }
                            if (indexCount == -1)
                            {
                                indexCount = replace.IndexOf("2019.4.31f1");
                                firstLine = false;
                            }
                            else if (indexCount != -1)
                            {
                                replace = firstLineReplace + replace.Substring(indexCount);
                                firstLine = false;
                            }
                            else
                            {
                                replace = "";
                            }
                        }

                        vWriter.Write(replace);
                    }

                }
            }
        }

        private static string CheckAndReplaceLine(string line, MatchModel old, MatchModel newModel, bool unityReplace = false)
        {
            var enc = Encoding.GetEncoding(28591);
            var edited = line;

            if (edited.Contains(old.AvatarAssetId)) edited = edited.Replace(old.AvatarAssetId, newModel.AvatarAssetId);
            if (edited.Contains(old.AvatarId)) edited = edited.Replace(old.AvatarId, newModel.AvatarId);
            if (edited.Contains(old.AvatarCab)) edited = edited.Replace(old.AvatarCab, newModel.AvatarCab);
            if (unityReplace) { 
            if (old.UnityVersion != null)
                if (edited.Contains(old.UnityVersion))
                    edited = edited.Replace(old.UnityVersion, newModel.UnityVersion);
            }

            return edited;
        }

        private static void GetReadyForCompressWorld(string oldFile, string newFile, MatchModel old, MatchModel newModel)
        {
            var enc = Encoding.GetEncoding(28591);
            using (var vReader = new StreamReaderOver(oldFile, enc))
            {
                using (var vWriter = new StreamWriter(newFile, false, enc))
                {
                    while (!vReader.EndOfStream)
                    {
                        var vLine = vReader.ReadLine();
                        var replace = CheckAndReplaceLineWorld(vLine, old, newModel);
                        vWriter.Write(replace);
                    }
                }
            }
        }

        private static string CheckAndReplaceLineWorld(string line, MatchModel old, MatchModel newModel)
        {
            var edited = line;
            if (edited.Contains(old.AvatarId)) edited = edited.Replace(old.AvatarId, newModel.AvatarId);
            if (old.UnityVersion != null)
                if (edited.Contains(old.UnityVersion))
                    edited = edited.Replace(old.UnityVersion, newModel.UnityVersion);
            return edited;
        }

        private static void DecompressToFile(string filePath, string savePath, HotswapConsole hotSwap)
        {
            var manager = new AssetsManager();
            var bunInst = manager.LoadBundleFile(filePath, false);
            using (var bunStream = File.Open(savePath, FileMode.Create))
            {
                var progressBar = new SZProgress(hotSwap);
                SafeWrite(hotSwap.txtStatusText, $"Decompressing Asset, this may take a while!{Environment.NewLine}");
                bunInst.file = BundleHelper.UnpackBundleToStream(bunInst.file, bunStream);
            }

            SafeWrite(hotSwap.txtStatusText, $"----------------------------------------------------------{Environment.NewLine}Asset Decompressed!{Environment.NewLine}");
            SafeProgress(hotSwap.pbProgress, 100);
        }

        //Creates function allowing it to be used with string imputs
        private static void DecompressToFileStr(string bundlePath, string unpackedBundlePath, HotswapConsole hotSwap)
        {
            SafeWrite(hotSwap.txtStatusText, $"Declared new asset manager!{Environment.NewLine}");
            SafeProgress(hotSwap.pbProgress, 1);
            DecompressToFile(bundlePath, unpackedBundlePath, hotSwap);
        }

        private static void SafeWrite(TextBox text, string textWrite)
        {
            if (text.InvokeRequired)
            {
                text.Invoke((MethodInvoker)delegate
                {
                    text.Text = textWrite + text.Text;
                });
            }
        }

        private static void SafeProgress(ProgressBar progress, int value)
        {
            if (progress.InvokeRequired)
            {
                progress.Invoke((MethodInvoker)delegate
                {
                    progress.Value = value;
                });
            }
        }

        //Creates function to compress asset bundles
        private static void CompressBundle(string file, string compFile, HotswapConsole hotSwap)
        {
            var newUncompressedBundle = new AssetBundleFile();
            newUncompressedBundle.Read(new AssetsFileReader(File.OpenRead(file)));

            SafeWrite(hotSwap.txtStatusText, $"Final Step, Compressing File, this will take a while!{Environment.NewLine}");
            var progressBar = new SZProgress(hotSwap);

            using (AssetsFileWriter writer = new AssetsFileWriter(compFile))
            {
                newUncompressedBundle.Pack(writer, AssetBundleCompressionType.LZMA, progressed: progressBar);
            }
            SafeWrite(hotSwap.txtStatusText, $"Compressed file packing complete!{Environment.NewLine}");

        }
    }
}