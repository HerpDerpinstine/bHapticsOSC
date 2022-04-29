using System;
using System.Collections.Generic;
using System.IO;

namespace OscLib.Config
{
    public static class ConfigManager
    {
        internal static List<ConfigFile> AllConfigFiles = new List<ConfigFile>();

        public static void LoadAll()
        {
            foreach (ConfigFile configFile in AllConfigFiles)
                configFile.Load();
        }

        public static void SaveAll()
        {
            foreach (ConfigFile configFile in AllConfigFiles)
                configFile.Save();
        }

        public static T CreateConfig<T>(string baseFolder, string fileName) where T : ConfigFile
        {
            string newFile = Path.Combine(baseFolder, $"{fileName}.cfg");

            string oldFile = Path.Combine(baseFolder, $"{fileName}.ini");
            if (File.Exists(oldFile))
            {
                if (File.Exists(newFile))
                    File.Delete(newFile);
                File.Move(oldFile, newFile);
            }

            return (T)Activator.CreateInstance(typeof(T), new object[] { newFile });
        }
    }
}
