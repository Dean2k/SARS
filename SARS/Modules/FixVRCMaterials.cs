using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FACS01.Utilities
{
    public class FixVRCMaterials
    {
        private readonly string header = "Material:";
        private readonly string beginParams = "  m_SavedProperties:";

        private int fixedFilesCount;
        private string output_print;

        public string FixMaterials(string directory)
        {
            fixedFilesCount = 0;
            output_print = "";

            string[] filePaths = Directory.GetFiles(directory, "*.mat", SearchOption.AllDirectories);

            bool refresh = false;
            int progress = 0; float progressTotal = filePaths.Length;
            foreach (string filePath in filePaths)
            {
                progress++;
                string path2 = filePath.Replace('\\', '/');
                Console.WriteLine("Fix Materials", $"Processing file: {Path.GetFileName(filePath)}", progress / progressTotal);
                if (IsYAML(path2)) { Fix(path2); refresh = true; }
            }

            GenerateResults();
            return(output_print);
        }

        private void GenerateResults()
        {
            string end = $"";
            output_print = $"Results:\nReassigned Materials: {fixedFilesCount}\n";
            output_print += end;
        }

        private bool ChangeGUID(string[] arrLine, int scriptLineIndex, (string, string) scriptGUID, List<string> paramsCollection)
        {
            bool WasModified = false;
            arrLine[scriptLineIndex] = "  m_Shader: {fileID: 4800000, guid: 5ca92f1e9fc35504aba297fd1acd62df, type: 3}";
            WasModified = true;
            return WasModified;
        }

        private void Fix(string path)
        {
            bool WasModified = false;
            string[] arrLine = File.ReadAllLines(path);
            int arrLineIndex = 2; 
            int arrLineCount = arrLine.Length;
            bool inspect = false; 
            int scriptLineIndex = 0;
            bool collect = false; 
            (string, string) scriptGUID = ("", "");
            List<string> paramsCollection = new List<string>();
            while (arrLineIndex < arrLineCount)
            {
                string line = arrLine[arrLineIndex];

                if (inspect)
                {
                    if (line[2] != ' ')
                    {
                        if (line.StartsWith("  m_"))
                        {
                            if (collect)
                            {
                                collect = false;
                            }
                            if (line.StartsWith("  m_Shader: "))
                            {
                                scriptLineIndex = arrLineIndex;
                                scriptGUID = GetGUIDfromLine(line);
                                inspect = collect = false;
                                // process:scriptGUID, paramsCollection
                                WasModified = ChangeGUID(arrLine, scriptLineIndex, scriptGUID, paramsCollection) || WasModified;
                                scriptGUID = ("", ""); paramsCollection = new List<string>();
                            }
                            else if (line.StartsWith(beginParams))
                            {
                                collect = true;
                            }
                        }
                        else if (collect && !line.StartsWith("  - "))
                        {
                            paramsCollection.Add(line.Substring(2, line.IndexOf(":") - 2));
                        }
                    }
                }
                else if (line == header)
                {
                    inspect = true;
                    paramsCollection = new List<string>();
                }

                arrLineIndex++;
            }
            //if had something, process
            if (paramsCollection.Any())
            {
                WasModified = ChangeGUID(arrLine, scriptLineIndex, scriptGUID, paramsCollection) || WasModified;
            }

            if (WasModified)
            {
                File.WriteAllLines(path, arrLine); fixedFilesCount++;
            }
        }

        private (string, string) GetGUIDfromLine(string line)
        {
            if (!line.Contains("fileID: ") || !line.Contains("guid: "))
            {
                return ("", "");
            }
            string tmp = line.Substring(line.IndexOf("fileID: ") + 8);
            int itmp = tmp.IndexOf(",");
            string FileID = tmp.Substring(0, itmp);
            tmp = tmp.Substring(tmp.IndexOf("guid: ") + 6);
            itmp = tmp.IndexOf(",");
            string GUID = tmp.Substring(0, itmp);

            return (GUID, FileID);
        }

        private static bool IsYAML(string path)
        {
            bool isYAML = false;
            if (!File.Exists(path)) return isYAML;
            using (StreamReader sr = new StreamReader(path))
            {
                if (sr.Peek() >= 0 && sr.ReadLine().Contains("%YAML 1.1")) isYAML = true;
            }
            return isYAML;
        }
    }
}