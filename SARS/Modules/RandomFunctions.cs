using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SARS.Modules
{
    public static class RandomFunctions
    {
        public static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static void tryDelete(string location)
        {
            try
            {
                if (File.Exists(location))
                {
                    File.Delete(location);
                    //CoreFunctions.WriteLog($"Deleted file {location}", this);
                }
            }
            catch
            {
                //CoreFunctions.WriteLog($"{ex.Message}", this);
            }
        }

        public static void tryDeleteDirectory(string location)
        {
            try
            {
                Directory.Delete(location, true);
            }
            catch
            {
            }
        }

        public static void OpenUnity(string unityPath, string hotSwapName)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string commands = string.Format("/C \"{0}\" -ProjectPath " + hotSwapName, unityPath);

            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "CMD.EXE",
                Arguments = commands,
                WorkingDirectory = filePath,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            process.StartInfo = processStartInfo;
            process.Start();
        }

        public static void KillProcess(string processName)
        {
            try
            {
                Process.Start("taskkill", "/F /IM \"" + processName + "\"");
                Console.WriteLine("Killed Process: " + processName);
                //WriteLog(string.Format("Killed Process", processName), main);
            }
            catch { }
        }
    }
}