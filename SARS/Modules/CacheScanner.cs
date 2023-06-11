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

        public static void ReadUntilAvatarId(string filePath)
        {
            try
            {
                string stringToFind = "avtr_";
                Regex avatarIdRegEx = new Regex(@"avtr_[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}");

                foreach (var str in File.ReadLines(filePath).Where(s => s.Contains(stringToFind)))
                {
                    string[] stringSeparators = new string[] { "avtr_" };
                    string[] words = str.Split(stringSeparators, StringSplitOptions.None);
                    string avatarId = "avtr_"  + words[1].Substring(0, 36);
                    Console.WriteLine(avatarId);
                    if (avatarIdRegEx.IsMatch(avatarId))
                    {
                        CacheAvatar cacheAvatar = new CacheAvatar { AvatarId = avatarId.Trim(), FileLocation = filePath, AvatarDetected = File.GetCreationTime(filePath) };
                        avatarIds.Add(cacheAvatar);
                        NewMessage($"Avatar found: {avatarId}{Environment.NewLine}");
                        break;
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
        private static Stopwatch stop = new Stopwatch();

        private static bool IsFileLocked(IOException exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33;
        }

        public static async Task ScanCache(string cacheLocation)
        {
            stop.Start();
            Task.Run(() => ScanFunction(cacheLocation));
        }

        private static async Task ScanFunction(string cacheLocation)
        {
            List<Task> tasks = new List<Task>();
            List<string> locations = GetCacheLocations(cacheLocation);

            foreach (string cache in locations)
            {
                tasks.Add(Task.Run(() => ReadUntilAvatarId(cache)));
            }
            Task.WaitAll(tasks.ToArray());

            string outputBuffer = "";

            avatarIds = avatarIds.Distinct().ToList();

            foreach (var item in avatarIds)
            {
                outputBuffer += $"{item.AvatarId};\n";

                avatarsFound++;
            }
            File.WriteAllText("avatarLog.txt", outputBuffer);
            stop.Stop();
            string timeRan = TimeSpan.FromMilliseconds(stop.ElapsedMilliseconds).TotalSeconds.ToString();
            NewMessage($"Found {avatarsFound} avatars in {timeRan} seconds{Environment.NewLine}");
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