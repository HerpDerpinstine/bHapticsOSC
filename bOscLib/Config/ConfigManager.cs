using System;
using System.IO;
using bHapticsOSC.Config.Interface;

namespace bHapticsOSC.Config
{
    public static class ConfigManager
    {
        public static ConnectionConfig Connection;
        public static DevicesConfig Devices;
        public static VRChatConfig VRChat;

        private static ConfigFile[] AllConfigFiles;

        static ConfigManager()
        {
            string basefolder = Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location);

            AllConfigFiles = new ConfigFile[]
            {
                Connection = CreateConfig<ConnectionConfig>(basefolder, nameof(Connection)),
                Devices = CreateConfig<DevicesConfig>(basefolder, nameof(Devices)),
                VRChat = CreateConfig<VRChatConfig>(basefolder,  nameof(VRChat))
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

            T returnval = (T)Activator.CreateInstance(typeof(T), new object[] { newFile });

            return returnval;
        }
    }
}
