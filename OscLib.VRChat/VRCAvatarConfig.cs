using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OscLib.VRChat
{
    public static class VRCAvatarConfig
    {
        public static void AddParameter(string avatarID, string parameter, string address, string type) => AddParameter(avatarID, parameter, address, type, address, type);
        public static void AddParameter(string avatarID, string parameter, string inAddress, string inType, string outAddress, string outType)
        {
            string appdataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low";
            string VRChatFolder = Path.Combine(appdataFolder, "VRChat", "VRChat", "OSC");
            
            string[] userFolders = Directory.GetDirectories(VRChatFolder);
            if (userFolders.Length <= 0)
                return;

            foreach (string folderPath in userFolders)
            {
                string filePath = Path.Combine(folderPath, $"{avatarID}.json");
                if (!File.Exists(filePath))
                    continue;

                string rawFile = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(rawFile))
                    continue;

                JArray baseJArray = JArray.Parse(rawFile);
                if (!baseJArray.Contains("parameters"))
                    baseJArray["parameters"] = new JArray();

                JArray parameterTableArray = (JArray)baseJArray["parameters"];
                if (parameterTableArray.Count > 0)
                    foreach (JArray parameterArray in parameterTableArray.ToArray())
                        if (parameterArray["name"].ToObject<string>().Equals(parameter))
                            parameterTableArray.Remove(parameterArray);

                JArray newParameterArray = new JArray();
                newParameterArray["name"] = parameter;
                parameterTableArray.Add(newParameterArray);

                newParameterArray["input"] = new JArray();
                newParameterArray["input"]["address"] = inAddress;
                newParameterArray["input"]["type"] = inType;

                newParameterArray["output"] = new JArray();
                newParameterArray["output"]["address"] = outAddress;
                newParameterArray["output"]["type"] = outType;

                File.WriteAllText(filePath, baseJArray.ToString(Formatting.Indented));
            }
        }
    }
}
