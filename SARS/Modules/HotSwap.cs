﻿using AssetsTools.NET;
using AssetsTools.NET.Extra;
using SARS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SARS.Modules
{
    public static class HotSwap
    {
        public static async void HotswapProcess(HotswapConsole hotSwapConsole, AvatarSystem avatarSystem, string avatarFile, string customAvatarId = null, string imgFileLocation = null, string avatarName = null, string ReuploaderVrChat = null)
        {
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
                    avatarPrefabIdRegex);
            }

            try
            {
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
                avatarPrefabIdRegex);
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
                    if (dialogResult == DialogResult.Cancel) matchModelOld.UnityVersion = null;
                }

            if (matchModelNew == null)
            {
                matchModelNew = new MatchModel();
                matchModelNew.UnityVersion = "2019.4.31f1";//BaseUploader._unityVersion;
                matchModelNew.AvatarAssetId = "prefab-id-v1_" + customAvatarId + "_" + RandomString(10) + ".prefab";
                matchModelNew.AvatarCab = "CAB-" + RandomString(32);
                matchModelNew.AvatarId = customAvatarId;
            }

            GetReadyForCompress(fileDecompressed2, fileDecompressedFinal, matchModelOld, matchModelNew);

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
                len = len / 1024;
            }

            var uncompressedSize = $"{len:0.##} {sizes[order]}";

            if (avatarSystem.avatars != null)
            {
                var avatar = avatarSystem.avatars.FirstOrDefault(x => x.avatar.avatarId == matchModelOld.AvatarId);
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
            Regex avatarAssetId)
        {
            MatchCollection avatarIdMatch = null;
            MatchCollection avatarAssetIdMatch = null;
            MatchCollection avatarCabMatch = null;
            MatchCollection unityMatch = null;
            var unityCount = 0;

            foreach (var line in File.ReadLines(file))
            {
                var tempId = avatarId.Matches(line);
                var tempAssetId = avatarAssetId.Matches(line);
                var tempCab = avatarCab.Matches(line);
                var tempUnity = unityVersion.Matches(line);
                if (tempAssetId.Count > 0) avatarAssetIdMatch = tempAssetId;
                if (tempId.Count > 0) avatarIdMatch = tempId;
                if (tempCab.Count > 0) avatarCabMatch = tempCab;
                if (tempUnity.Count > 0)
                {
                    unityMatch = tempUnity;
                    unityCount++;
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

        private static void GetReadyForCompress(string oldFile, string newFile, MatchModel old, MatchModel newModel)
        {
            var enc = Encoding.GetEncoding(28591);
            using (var vReader = new StreamReaderOver(oldFile, enc))
            {
                using (var vWriter = new StreamWriter(newFile, false, enc))
                {
                    while (!vReader.EndOfStream)
                    {
                        var vLine = vReader.ReadLine();
                        var replace = CheckAndReplaceLine(vLine, old, newModel);
                        vWriter.Write(replace);
                    }
                }
            }
        }

        private static string CheckAndReplaceLine(string line, MatchModel old, MatchModel newModel)
        {
            var edited = line;
            if (edited.Contains(old.AvatarAssetId)) edited = edited.Replace(old.AvatarAssetId, newModel.AvatarAssetId);
            if (edited.Contains(old.AvatarId)) edited = edited.Replace(old.AvatarId, newModel.AvatarId);
            if (edited.Contains(old.AvatarCab)) edited = edited.Replace(old.AvatarCab, newModel.AvatarCab);
            if (old.UnityVersion != null)
                if (edited.Contains(old.UnityVersion))
                    edited = edited.Replace(old.UnityVersion, newModel.UnityVersion);
            return edited;
        }

        private static void DecompressToFile(BundleFileInstance bundleInst, string savePath, HotswapConsole hotSwap)
        {
            AssetBundleFile bundle = bundleInst.file;
            SafeWrite(hotSwap.txtStatusText, $"22.2% Bundle file assigned!" + Environment.NewLine);
            SafeProgress(hotSwap.pbProgress, 22);
            using (FileStream bundleStream = File.Open(savePath, FileMode.OpenOrCreate))
            {
                using (BufferedStream bs = new BufferedStream(bundleStream))
                {
                    SafeWrite(hotSwap.txtStatusText, $"33.3% Loaded file to bundle stream!" + Environment.NewLine);
                    SafeProgress(hotSwap.pbProgress, 33);
                    var progressBar = new SZProgress(hotSwap);
                    bundle.Unpack(bundle.reader, new AssetsFileWriter(bs), progressBar);
                    SafeWrite(hotSwap.txtStatusText, $"44.4% Unpack stream complete!" + Environment.NewLine);
                    SafeProgress(hotSwap.pbProgress, 44);
                    bundleStream.Position = 0;
                    SafeWrite(hotSwap.txtStatusText, $"55.5% Bundle stream position assigned!" + Environment.NewLine);
                    SafeProgress(hotSwap.pbProgress, 55);
                    AssetBundleFile newBundle = new AssetBundleFile();
                    SafeWrite(hotSwap.txtStatusText, $"66.6% Created new asset bundle file!" + Environment.NewLine);
                    SafeProgress(hotSwap.pbProgress, 66);
                    newBundle.Read(new AssetsFileReader(bs), false);
                    SafeWrite(hotSwap.txtStatusText, $"77.7% Bundle written to file!" + Environment.NewLine);
                    SafeProgress(hotSwap.pbProgress, 77);
                    bundle.reader.Close();
                    SafeWrite(hotSwap.txtStatusText, $"88.8% Bundle closed!" + Environment.NewLine);
                    SafeProgress(hotSwap.pbProgress, 88);
                    bundleInst.file = newBundle;
                }
            }
            SafeWrite(hotSwap.txtStatusText, $"100% Bundle instance cleaned!" + Environment.NewLine);
            SafeProgress(hotSwap.pbProgress, 100);
        }

        //Creates function allowing it to be used with string imputs
        private static void DecompressToFileStr(string bundlePath, string unpackedBundlePath, HotswapConsole hotSwap)
        {
            var am = new AssetsManager();
            SafeWrite(hotSwap.txtStatusText, "11.1% Declared new asset manager!" + Environment.NewLine);
            SafeProgress(hotSwap.pbProgress, 11);
            DecompressToFile(am.LoadBundleFile(bundlePath), unpackedBundlePath, hotSwap);
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
            using (var stream = new AssetsFileWriter(compFile))
            {
                var am = new AssetsManager();
                SafeWrite(hotSwap.txtStatusText, $"25% Declared new asset manager!" + Environment.NewLine);
                var bun = am.LoadBundleFile(file, false);
                SafeWrite(hotSwap.txtStatusText, $"50% Bundle file initialized!" + Environment.NewLine);
                var progressBar = new SZProgress(hotSwap);
                bun.file.Pack(bun.file.reader, stream, AssetBundleCompressionType.LZMA, progressBar);
                SafeWrite(hotSwap.txtStatusText, $"100% Compressed file packing complete!" + Environment.NewLine);
                am.UnloadAll();
                bun = null;
            }
        }
    }
}