﻿using bHapticsOSC.Config.Interface;
using System;
using System.IO;

namespace bHapticsOSC.Config
{
    public static class ConfigManager
    {
        public static ConnectionConfig Connection;
        public static DevicesConfig Devices;
        public static VRChatConfig VRChat;

        public static void Setup()
        {
            string baseFolder = Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location);

            Connection = CreateConfig<ConnectionConfig>(baseFolder, nameof(Connection));
            Devices = CreateConfig<DevicesConfig>(baseFolder, nameof(Devices));
            VRChat = CreateConfig<VRChatConfig>(baseFolder, nameof(VRChat));
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

            T file = (T)Activator.CreateInstance(typeof(T), new object[] { newFile });
            file.Load();
            file.Save();
            return file;
        }
    }
}