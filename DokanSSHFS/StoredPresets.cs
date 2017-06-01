using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace SshFileSystem.WinForms
{
    public class Preset
    {
        public string Name { get; set; }
        public string Host { get; set; } = "";
        public int Port { get; set; } = 22;
        public string User { get; set; }
        public string PrivateKey { get; set; }
        public bool UsePassword { get; set; } = true;
        public string EncryptedPassword { get; set; }
        public string ServerRoot { get; set; }  = "/";
        public string Drive { get; set; } = "N";
    }

    public class StoredPresets
    {
        private List<Preset> _presets = new List<Preset>();
        
        public string GetNewName()
        {
            var index = 0;
            var newName = string.Empty;
            do
            {
                newName = ("New Preset " + (index == 0 ? "" : index.ToString())).Trim();
                index++;
            } while ((_presets.Any(x => x.Name == newName)));

            return newName;
        }

        public List<Preset> Presets => _presets;

        public bool Delete(string preset)
        {
            return _presets.RemoveAll(x => x.Name == preset) > 0;
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(_presets.GetType());
            using (FileStream stream = new FileStream(GetSettingsFile(), FileMode.Create))
            {
                serializer.Serialize(stream, _presets);
            }
        }

        private static string GetSettingsFile()
        {
            return Path.Combine(Application.UserAppDataPath, "presets.xml");
        }

        public void Load()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(_presets.GetType());
                using (FileStream stream = new FileStream(GetSettingsFile(), FileMode.OpenOrCreate))
                {
                    _presets = (List<Preset>)serializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                _presets = new List<Preset>();
            }
        }

        public void AddPreset(Preset preset)
        {
            if (!_presets.Contains(preset))
                _presets.Add(preset);
        }

        public Preset LoadOrCreatePreset(string presetName)
        {
            var existingPreset = _presets.SingleOrDefault(x => x.Name == presetName);
            if (existingPreset != null)
                return existingPreset;

            var newPreset = new Preset { Name = presetName };
            return newPreset;
        }
    }
}
