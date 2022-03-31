using System;
using System.Threading;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            bool isFirst;
            Mutex mutex = new Mutex(true, BuildInfo.Name, out isFirst);
            if (!isFirst)
                return 0;

            WelcomeMessage();

            ConfigManager.LoadAll();

            ConfigManager.Connection.OnFileModified = () =>
            {
                Console.WriteLine();
                Console.WriteLine("Connection.cfg Reloaded!");
                Console.WriteLine();
                PrintConnection();
                OscManager.Connect();
            };

            ConfigManager.Devices.OnFileModified += () =>
            {
                Console.WriteLine();
                Console.WriteLine("Devices.cfg Reloaded!");
                Console.WriteLine();
                PrintDevices();
            };

            ConfigManager.VRChat.OnFileModified += () =>
            {
                Console.WriteLine();
                Console.WriteLine("VRChat.cfg Reloaded!");
                Console.WriteLine();
                PrintVRChat();
            };

            PrintConnection();
            PrintDevices();
            PrintVRChat();

            bHaptics.Load();
            OscManager.Connect();

            Console.WriteLine();
            Console.WriteLine("Awaiting Packets...");
            Console.WriteLine("Press ESC to Exit...");
            Console.WriteLine();

            ConsoleKeyInfo keyInfo;
            while (((keyInfo = Console.ReadKey(true)) == null) || (keyInfo.Key != ConsoleKey.Escape))
                Thread.Sleep(ThreadedTask.UpdateRate);

            OnQuit();
            return 0;
        }

        private static void OnQuit()
        {
            OscManager.Disconnect();
            bHaptics.Quit();
            ConfigManager.SaveAll();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine(Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version}");
            Console.WriteLine($"Created by Herp Derpinstine");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintVRChat()
        {
            Console.WriteLine($"===== VRChat =====");
            Console.WriteLine();
            Console.WriteLine($"[InStation] = {ConfigManager.VRChat.vrchat.Value.InStation}");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintConnection()
        {
            Console.WriteLine($"===== OscReceiver =====");
            Console.WriteLine();
            Console.WriteLine($"[Port] = {ConfigManager.Connection.receiver.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== OscSender =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {ConfigManager.Connection.sender.Value.Enabled}");
            Console.WriteLine($"[IP] = {ConfigManager.Connection.sender.Value.IP}");
            Console.WriteLine($"[Port] = {ConfigManager.Connection.sender.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintDevices()
        {
            PrintDevice("Head", bHaptics.PositionType.Head);

            PrintDevice("Vest", bHaptics.PositionType.Vest);

            PrintDevice("Arm Left", bHaptics.PositionType.ForearmL);
            PrintDevice("Arm Right", bHaptics.PositionType.ForearmR);

            PrintDevice("Hand Left", bHaptics.PositionType.HandL);
            PrintDevice("Hand Right", bHaptics.PositionType.HandR);

            PrintDevice("Foot Left", bHaptics.PositionType.FootL);
            PrintDevice("Foot Right", bHaptics.PositionType.FootR);
        }

        private static void PrintDevice(string name, bHaptics.PositionType positionType)
        {
            Console.WriteLine($"===== {name} =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {ConfigManager.Devices.PositionTypeToEnabled(positionType)}");
            Console.WriteLine($"[Intensity] = {ConfigManager.Devices.PositionTypeToIntensity(positionType)}");
            Console.WriteLine();
            Console.WriteLine();

        }
    }
}
