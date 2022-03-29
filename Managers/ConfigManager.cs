using bHapticsOSC.Config;
using System.IO;

namespace bHapticsOSC.Managers
{
    public static class ConfigManager
    {
        public static ConnectionConfig Connection;
        public static DevicesConfig Devices;

        public static void Setup()
        {
            string baseFolder = Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location);

            Connection = new ConnectionConfig(Path.Combine(baseFolder, "Connection.ini"));
            Devices = new DevicesConfig(Path.Combine(baseFolder, "Devices.ini"));
        }
    }
}
