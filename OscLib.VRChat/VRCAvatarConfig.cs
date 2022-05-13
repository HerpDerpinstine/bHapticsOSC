using System;
using System.IO;

namespace OscLib.VRChat
{
    public static class VRCAvatarConfig
    {
        public static void RemoveFile(string avatarID)
        {
            string appdataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low";
            string VRChatFolder = Path.Combine(appdataFolder, "VRChat", "VRChat", "OSC");

            string[] userFolders = Directory.GetDirectories(VRChatFolder);
            if (userFolders.Length <= 0)
                return;

            foreach (string folderPath in userFolders)
            {
                string filePath = Path.Combine(folderPath, "Avatars", $"{avatarID}.json");
                if (!File.Exists(filePath))
                    continue;
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
