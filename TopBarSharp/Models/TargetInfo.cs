using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TopBarSharp.Models
{
    public class TargetInfo
    {
        public string ProcessName { get; set; }
        public string WindowName { get; set; }

        [JsonConstructor]
        public TargetInfo(string processName, string windowName)
        {
            ProcessName = processName;
            WindowName = windowName;
        }

        public TargetInfo(string path)
        {
            if (!File.Exists(path))
                throw new IOException("Save file not found!");
            var tempInfo = JsonSerializer.Deserialize<TargetInfo>(File.ReadAllText(path));
            if (tempInfo == null)
                throw new Exception("Could not deserialise save file!");

            ProcessName = tempInfo.ProcessName;
            WindowName = tempInfo.WindowName;
        }

        public void Save(string path)
        {
            var text = JsonSerializer.Serialize(this);
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, text);
        }
    }
}
