using bHapticsOSC.Config.Interface;
using System;
using System.IO;

namespace bHapticsOSC.Config
{
    public static class ConfigManager
    {
        private static ConfigFile[] AllConfigFiles;

        public static ConnectionConfig Connection;
        public static DevicesConfig Devices;
        public static VRChatConfig VRChat;

        static ConfigManager()
        {
            string baseFolder = Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location);

            AllConfigFiles = new ConfigFile[]
            {
                Connection = CreateConfig<ConnectionConfig>(baseFolder, nameof(Connection)),
                Devices = CreateConfig<DevicesConfig>(baseFolder, nameof(Devices)),
                VRChat = CreateConfig<VRChatConfig>(baseFolder, nameof(VRChat))
            };
        }

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

        private static T CreateConfig<T>(string baseFolder, string fileName) where T : ConfigFile
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
