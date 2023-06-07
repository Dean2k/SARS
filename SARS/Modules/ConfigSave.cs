using Newtonsoft.Json;
using System;
using System.IO;

namespace SARS.Modules
{
    public class ConfigSave<T> where T : class
    {
        private string FilePath { get; }
        public T Config { get; private set; }

        public ConfigSave(string path)
        {
            FilePath = path;
            CheckConfig();
            Config = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
        }

        private void CheckConfig()
        {
            if (!File.Exists(FilePath) || new System.IO.FileInfo(FilePath).Length < 2)
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(Activator.CreateInstance(typeof(T)), Formatting.Indented, new JsonSerializerSettings()));
        }

        private void UpdateConfig(object obj, FileSystemEventArgs args)
        {
            try
            {
                var UpdatedObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));

                if (UpdatedObject != null)
                    foreach (var Prop in UpdatedObject.GetType()?.GetProperties())
                    {
                        var Original = Config.GetType().GetProperty(Prop?.Name);
                        if (Original != null
                            && Prop.GetValue(UpdatedObject) != Original.GetValue(Config))
                        {
                            Config = UpdatedObject;
                            break;
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Save() =>
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Config, Formatting.Indented, new JsonSerializerSettings()));
    }
}