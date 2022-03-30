using bHapticsOSC.Config.Interface;
using System;
using System.IO;

namespace bHapticsOSC.Config
{
    public static class ConfigManager
    {
        public static ConnectionConfig Connection;
        public static DevicesConfig Devices;
        public static VRChatConfig VRChat;

        private static ConfigFile[] AllConfigFiles = new ConfigFile[]
        {
            Connection = CreateConfig<ConnectionConfig>(Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location), nameof(Connection)),
            Devices = CreateConfig<DevicesConfig>(Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location), nameof(Devices)),
            VRChat = CreateConfig<VRChatConfig>(Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location), nameof(VRChat))
        };

        public static void LoadAll()
        {
            foreach (ConfigFile configFile in AllConfigFiles)
            {
                configFile.Load();
                configFile.Save();
            }
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
