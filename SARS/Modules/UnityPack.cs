using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using YamlDotNet.RepresentationModel;

namespace SARS.Modules
{
    public class OnDiskFile
    {
        private readonly string _diskPath;
        private readonly YamlMappingNode _meta;

        public string PackPath { get; }

        public OnDiskFile(string packPath, string diskPath, YamlMappingNode meta)
        {
            PackPath = packPath;
            _diskPath = diskPath;
            _meta = meta;
        }

        public string GetDiskPath()
        {
            return _diskPath;
        }

        public Stream GetFile()
        {
            return File.Open(_diskPath, FileMode.Open, FileAccess.Read);
        }

        public YamlMappingNode GetMeta()
        {
            return _meta;
        }

        public string GetHash()
        {
            return ((YamlScalarNode)_meta["guid"]).Value;
        }
    }

    internal static class RandomUtils
    {
        static string CreateMd5(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (byte t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        static readonly Random r = new Random();

        public static string RandomString(int len = 32)
        {
            var c = "";
            for (int i = 0; i < len; i++)
            {
                c += r.Next(0, 128);
            }
            return c;
        }

        public static string RandomHash()
        {
            return CreateMd5(RandomString()).ToLower();
        }
    }

    public class Package : IEnumerable<KeyValuePair<string, OnDiskFile>>
    {
        private readonly string _name;
        private readonly Dictionary<string, OnDiskFile> _files = new Dictionary<string, OnDiskFile>();

        /// <summary>
        /// Creates an empty package with the given name
        /// </summary>
        /// <param name="name">name of the package</param>
        public Package(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Adds the given file on disk to this package
        /// </summary>
        /// <param name="file">The file to be added</param>
        public void PushFile(OnDiskFile file)
        {
            _files.Add(file.PackPath, file);
        }

        public OnDiskFile GetFile(string path)
        {
            return _files[path];
        }

        /// <summary>
        /// Generates a .unitypackage file from this package
        /// </summary>
        /// <param name="root">Root directory name, usually starts with Assets/</param>
        public void GeneratePackage(string root = "Assets/", string saveLocation = "")
        {
            var tmpPath = Path.Combine(Path.GetTempPath(), "packUnity" + _name);
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }
            Directory.CreateDirectory(tmpPath);

            foreach (var file in _files)
            {
                /*
                 * For every file there exists a directory named file.guid in the tar archive that looks like:
                 *     + /asset -> actual asset data
                 *     + /asset.meta -> meta file
                 *     + /pathname -> actual path in the package
                 *
                 * There can be more files such as preview but are optional.
                 */

                string fdirpath = Path.Combine(tmpPath, file.Value.GetHash());
                Directory.CreateDirectory(fdirpath);

                File.Copy(file.Value.GetDiskPath(), Path.Combine(fdirpath, "asset")); // copy to asset file

                using (StreamWriter writer = new StreamWriter(Path.Combine(fdirpath, "pathname"))) // the pathname file
                {
                    var altName = file.Value.PackPath;
                    if (altName.StartsWith("."))
                        altName = altName.Replace("." + Path.DirectorySeparatorChar, "");

                    writer.Write(root + altName.Replace(Path.DirectorySeparatorChar + "", "/"));
                }
                using (StreamWriter writer = new StreamWriter(Path.Combine(fdirpath, "asset.meta"))) // the meta file
                {
                    var doc = new YamlDocument(file.Value.GetMeta());
                    var ys = new YamlStream(doc);
                    ys.Save(writer);
                }
                var fi = new FileInfo(Path.Combine(fdirpath, "asset.meta"));
                using (var fs = fi.Open(FileMode.Open))
                {
                    fs.SetLength(fi.Length - 3 - Environment.NewLine.Length);
                }
            }

            CreateTarGZ(saveLocation + _name + ".unitypackage", tmpPath);
            Directory.Delete(tmpPath, true);
        }

        static YamlMappingNode ReadMeta(string file)
        {
            using (var read = new StreamReader(file))
            {
                var metaYaml = new YamlStream();
                metaYaml.Load(read);
                return (YamlMappingNode)metaYaml.Documents[0].RootNode;
            }
        }

        /// <summary>
        /// Creates a Package object from the given directory
        /// </summary>
        /// <param name="dir">Directory to pack</param>
        /// <param name="packName">Name of the package</param>
        /// <param name="respectMeta">Whether metadata files should be kept or not</param>
        /// <param name="omitExts">Extensions to omit</param>
        /// <param name="omitDirs">Directories to omit</param>
        /// <returns></returns>
        public static Package FromDirectory(string dir, string packName, bool respectMeta, string[] omitExts,
            string[] omitDirs)
        {
            var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

            // Create a package object from the given directory
            var pack = new Package(packName);

            foreach (var file in files)
            {
                var altName = file;
                if (file.StartsWith("."))
                    altName = file.Replace("." + Path.DirectorySeparatorChar, "");

                var skip = omitDirs.Any(d => altName.StartsWith(d)); // if the filename starts with any skipped dirname

                var extension = Path.GetExtension(file).Replace(".", "");
                if (skip || omitExts.Contains(extension))
                    continue;

                var meta = new YamlMappingNode
                {
                    {"guid", RandomUtils.RandomHash()},
                    {"fileFormatVersion", "2"}
                };
                if (respectMeta && File.Exists(file + ".meta"))
                {
                    var metaFile = file + ".meta";
                    meta = ReadMeta(metaFile);
                }

                var f = new OnDiskFile(file.Replace(dir + "\\", ""), file, meta);
                pack.PushFile(f);
            }

            return pack;
        }

        public static Package FromPackage(string pathToPack)
        {
            Stream inStream = File.Open(pathToPack, FileMode.Open, FileAccess.Read);
            Stream gziStream = new GZipInputStream(inStream);
            TarArchive ar = TarArchive.CreateInputTarArchive(gziStream);

            var name = Path.GetFileNameWithoutExtension(pathToPack);

            var tmpPath = Path.Combine(Path.GetTempPath(), "packUnity" + name);
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }

            ar.ExtractContents(tmpPath);
            ar.Close();
            gziStream.Close();
            inStream.Close();

            var dirs = Directory.GetDirectories(tmpPath);

            var pack = new Package(name);
            foreach (var dir in dirs)
            {
                var assetPath = File.ReadAllText(Path.Combine(dir, "pathname"));
                var meta = ReadMeta(Path.Combine(dir, "asset.meta"));
                var diskPath = Path.Combine(dir, "asset");
                var guid = Path.GetFileName(dir);
                if (((YamlScalarNode)meta["guid"]).Value != guid)
                {
                    using (var stderr = new StreamWriter(Console.OpenStandardError()))
                    {
                        stderr.WriteLine("Erroneous File In Package! {0}, {1}", assetPath, guid);
                    }
                    continue;
                }
                var file = new OnDiskFile(assetPath, diskPath, meta);
                pack.PushFile(file);
            }

            return pack;
        }

        /// <summary>
        /// Creates a folder from this package with the correct structure
        /// </summary>
        public void GenerateFolder(string to = "")
        {
            foreach (var file in this)
            {
                var outPath = Path.Combine(to, file.Value.PackPath);
                var metaPath = outPath + ".meta";
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.Copy(file.Value.GetDiskPath(), outPath);
                using (var writer = new StreamWriter(metaPath)) // the meta file
                {
                    var doc = new YamlDocument(file.Value.GetMeta());
                    var ys = new YamlStream(doc);
                    ys.Save(writer);
                }
                var fi = new FileInfo(metaPath);
                using (var fs = fi.Open(FileMode.Open))
                {
                    fs.SetLength(fi.Length - 3 - Environment.NewLine.Length);
                }
            }
        }

        private static void CreateTarGZ(string tgzFilename, string sourceDirectory)
        {
            Stream outStream = File.Create(tgzFilename);
            Stream gzoStream = new GZipOutputStream(outStream);
            TarArchive tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);

            tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
            if (tarArchive.RootPath.EndsWith("/"))
                tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

            AddDirectoryFilesToTar(tarArchive, sourceDirectory, true);

            tarArchive.Close();
        }

        private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
        {
            var filenames = Directory.GetFiles(sourceDirectory);
            foreach (var filename in filenames)
            {
                var tarEntry = TarEntry.CreateEntryFromFile(filename);
                tarEntry.Name = filename.Remove(0, tarArchive.RootPath.Length + 1);
                tarArchive.WriteEntry(tarEntry, true);
            }

            if (!recurse) return;

            var directories = Directory.GetDirectories(sourceDirectory);
            foreach (var directory in directories)
            {
                AddDirectoryFilesToTar(tarArchive, directory, true);
            }
        }

        public IEnumerator<KeyValuePair<string, OnDiskFile>> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
