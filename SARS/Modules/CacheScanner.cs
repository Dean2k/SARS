using Microsoft.VisualBasic.Devices;
using ARC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using ARC.Models.InternalModels;

namespace ARC.Modules
{
    public class CacheScanner
    {
        public CacheScanner()
        {
        }

        public static List<CacheAvatar> avatarIds = new List<CacheAvatar>();
        public static List<CacheAvatar> autoAvatarIds = new List<CacheAvatar>();

        public static void ReadUntilId(string filePath)
        {
            string stringToFind = "prefab-id-v1_avtr_";
            string stringToFindWorld = "wrld_";
            Regex avatarIdRegEx = new Regex(@"avtr_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}");
            Regex worldIdRegEx = new Regex(@"wrld_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}");

            if (File.Exists(filePath))
            {
                foreach (var str in File.ReadLines(filePath).Where(s => s.Contains(stringToFind) || s.Contains(stringToFindWorld)))
                {
                    try
                    {
                        if (str.Contains(stringToFind))
                        {
                            string[] stringSeparators = new string[] { stringToFind };
                            string[] words = str.Split(stringSeparators, StringSplitOptions.None);
                            if (words.Length > 0)
                            {
                                if (words[1].Length >= 36)
                                {
                                    string avatarId = stringToFind + words[1].Substring(0, 36);
                                    if (avatarIdRegEx.IsMatch(avatarId))
                                    {
                                        CacheAvatar cacheAvatar = new CacheAvatar { AvatarId = avatarId.Replace("prefab-id-v1_", "").Trim(), FileLocation = filePath, AvatarDetected = File.GetCreationTime(filePath), FileSize = new FileInfo(filePath).Length };
                                        avatarIds.Add(cacheAvatar);
                                        autoAvatarIds.Add(cacheAvatar);
                                        NewMessage($"Avatar found: {avatarId.Replace("prefab-id-v1_", "")}{Environment.NewLine}");
                                        break;
                                    }
                                }
                            }
                        }

                        if (str.Contains(stringToFindWorld))
                        {
                            string[] stringSeparatorsWorld = new string[] { stringToFindWorld };
                            string[] wordsWorlds = str.Split(stringSeparatorsWorld, StringSplitOptions.None);
                            if (wordsWorlds.Length > 0)
                            {
                                if (wordsWorlds[1].Length >= 36)
                                {
                                    string worldId = stringToFindWorld + wordsWorlds[1].Substring(0, 36);
                                    if (worldIdRegEx.IsMatch(worldId))
                                    {
                                        CacheAvatar cacheAvatar = new CacheAvatar { AvatarId = worldId.Trim(), FileLocation = filePath, AvatarDetected = File.GetCreationTime(filePath), FileSize = new FileInfo(filePath).Length };
                                        avatarIds.Add(cacheAvatar);
                                        autoAvatarIds.Add(cacheAvatar);
                                        NewMessage($"World found: {worldId}{Environment.NewLine}");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        if (IsFileLocked(e))
                        {
                            NewMessage($"File is locked (VRChat open?){Environment.NewLine}");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        NewMessage($"Some other error occurred {e.Message} {Environment.NewLine}");
                        return;
                    }
                }
            }
        }

        public static Queue<string> messages = new Queue<string>();

        public static void NewMessage(string text)
        {
            lock (messages)
            {
                messages.Enqueue(text);
            }
        }

        private static int avatarsFound = 0;
        private static int worldsFound = 0;
        private static Stopwatch stop = new Stopwatch();

        private static bool IsFileLocked(IOException exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33;
        }

        public static async Task ScanCache(string cacheLocation, bool bypassLoaded)
        {
            stop = new Stopwatch();
            stop.Start();
            await Task.Run(() => ScanFunction(cacheLocation, bypassLoaded));
        }

        private static async Task ScanFunction(string cacheLocation, bool bypassLoaded)
        {
            if (!bypassLoaded)
            {
                avatarIds = new List<CacheAvatar>();
            }
            else
            {
                autoAvatarIds = new List<CacheAvatar>();
            }
            List<Task> tasks = new List<Task>();
            List<string> locations = GetCacheLocations(cacheLocation);
            Console.WriteLine("getting avatar Ids");
            foreach (string cache in locations)
            {
                if (!bypassLoaded)
                {
                    try
                    {
                        tasks.Add(Task.Run(() => ReadUntilId(cache)));
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        if (!avatarIds.Any(x => x.FileLocation.Contains(cache)))
                        {
                            tasks.Add(Task.Run(() => ReadUntilId(cache)));
                        }
                    }
                    catch { }
                }
            }
            Console.WriteLine("finished getting ids");
            if (bypassLoaded)
            {
                Console.WriteLine("awaiting tasks");
                Task.WaitAll(tasks.ToArray(), 10000);
            }
            else
            {
                Task.WaitAll(tasks.ToArray());
            }

            string outputBuffer = "";
            string outputBufferWorld = "";

            if (!bypassLoaded)
            {
                avatarIds = avatarIds.Distinct().ToList();

                foreach (var item in avatarIds)
                {
                    if (item != null)
                    {
                        if (item.AvatarId.Contains("avtr_"))
                        {
                            outputBuffer += $"{item.AvatarId};\n";
                            avatarsFound++;
                        }
                        else
                        {
                            outputBufferWorld += $"{item.AvatarId};\n";
                            worldsFound++;
                        }
                    }
                }
            }
            else
            {
                autoAvatarIds = autoAvatarIds.Distinct().ToList();

                foreach (var item in autoAvatarIds)
                {
                    if (item != null)
                    {
                        if (item.AvatarId.Contains("avtr_"))
                        {
                            outputBuffer += $"{item.AvatarId};\n";
                            avatarsFound++;
                        }
                        else
                        {
                            outputBufferWorld += $"{item.AvatarId};\n";
                            worldsFound++;
                        }
                    }
                }
            }

            stop.Stop();
            string timeRan = TimeSpan.FromMilliseconds(stop.ElapsedMilliseconds).TotalSeconds.ToString();
            NewMessage($"Found {avatarsFound} avatars & {worldsFound} Worlds in {timeRan} seconds{Environment.NewLine}");
        }

        public static List<string> GetCacheLocations(string path)
        {
            List<string> dataLocations = new List<string>();

            string[] directory = Directory.GetDirectories(path);
            foreach (string item in directory)
            {
                try
                {
                    string[] dataFolders = Directory.GetDirectories(item);
                    if (dataFolders.Length > 0)
                    {
                        string cacheDataPath = dataFolders[0] + "\\__data";
                        if (File.Exists(cacheDataPath))
                        {
                            dataLocations.Add(cacheDataPath);
                        }
                    }

                    if (Directory.GetFiles(item).Length == 0 && Directory.GetDirectories(item).Length == 0)
                    {
                        Directory.Delete(item, false);
                        Console.WriteLine($"Deleting empty folder {item}{Environment.NewLine}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }

            var cacheList = dataLocations;
            return cacheList;
        }
    }
}