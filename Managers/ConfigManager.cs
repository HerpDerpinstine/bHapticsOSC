using bHapticsOSC.Config;
using System;
using System.IO;

namespace bHapticsOSC.Managers
{
    internal static class ConfigManager
    {
        internal static ConnectionConfig Connection;
        internal static DevicesConfig Devices;

        internal static void Setup()
        {
            string baseFolder = Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location);

            Connection = new ConnectionConfig(Path.Combine(baseFolder, "Connection.ini"));
            Devices = new DevicesConfig(Path.Combine(baseFolder, "Devices.ini"));
        }
    }
}
