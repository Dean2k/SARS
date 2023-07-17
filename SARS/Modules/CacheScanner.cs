using SARS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SARS.Modules
{
    public class CacheScanner
    {
        public CacheScanner()
        {
        }

        public static List<CacheAvatar> avatarIds = new List<CacheAvatar>();

        public static void ReadUntilId(string filePath)
        {
            try
            {
                string stringToFind = "avtr_";
                string stringToFindWorld = "wrld_";
                Regex avatarIdRegEx = new Regex(@"avtr_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}");
                Regex worldIdRegEx = new Regex(@"wrld_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}");

                foreach (var str in File.ReadLines(filePath).Where(s => s.Contains(stringToFind) || s.Contains(stringToFindWorld)))
                {
                    if (str.Contains(stringToFind))
                    {
                        string[] stringSeparators = new string[] { stringToFind };
                        string[] words = str.Split(stringSeparators, StringSplitOptions.None);
                        string avatarId = stringToFind + words[1].Substring(0, 36);
                        if (avatarIdRegEx.IsMatch(avatarId))
                        {
                            CacheAvatar cacheAvatar = new CacheAvatar { AvatarId = avatarId.Trim(), FileLocation = filePath, AvatarDetected = File.GetCreationTime(filePath) };
                            avatarIds.Add(cacheAvatar);
                            NewMessage($"Avatar found: {avatarId}{Environment.NewLine}");
                            break;

                        }
                    }

                    if (str.Contains(stringToFindWorld))
                    {
                        string[] stringSeparatorsWorld = new string[] { stringToFindWorld };
                        string[] wordsWorlds = str.Split(stringSeparatorsWorld, StringSplitOptions.None);
                        string worldId = stringToFindWorld + wordsWorlds[1].Substring(0, 36);
                        Console.WriteLine(worldId);
                        if (worldIdRegEx.IsMatch(worldId))
                        {
                            CacheAvatar cacheAvatar = new CacheAvatar { AvatarId = worldId.Trim(), FileLocation = filePath, AvatarDetected = File.GetCreationTime(filePath) };
                            avatarIds.Add(cacheAvatar);
                            NewMessage($"World found: {worldId}{Environment.NewLine}");
                            break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                if (IsFileLocked(e))
                {
                    NewMessage($"File is locked (VRChat open?){Environment.NewLine}");
                }
                return;
            }
            catch (Exception e)
            {
                NewMessage($"Some other error occurred {e.Message} {Environment.NewLine}");
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

        public static async Task ScanCache(string cacheLocation)
        {
            stop.Start();
            await Task.Run(() => ScanFunction(cacheLocation));
        }

        private static async Task ScanFunction(string cacheLocation)
        {
            avatarIds = new List<CacheAvatar>();
            List<Task> tasks = new List<Task>();
            List<string> locations = GetCacheLocations(cacheLocation);

            foreach (string cache in locations)
            {
                tasks.Add(Task.Run(() => ReadUntilId(cache)));
            }
            Task.WaitAll(tasks.ToArray());


            string outputBuffer = "";
            string outputBufferWorld = "";

            avatarIds = avatarIds.Distinct().ToList();

            foreach (var item in avatarIds)
            {
                try
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
                catch { }
            }
            File.WriteAllText("avatarLog.txt", outputBuffer);
            File.WriteAllText("worldLog.txt", outputBufferWorld);
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
                            FileInfo info = new FileInfo(cacheDataPath);
                            dataLocations.Add(cacheDataPath);
                        }
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