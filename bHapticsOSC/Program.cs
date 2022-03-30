using System;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;
using bHapticsOSC.OpenSoundControl;

namespace bHapticsOSC
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            WelcomeMessage();

            ConfigManager.LoadAll();
            PrintConfig();

            bHaptics.Load();
            OscManager.Connect();

            Console.WriteLine();
            Console.WriteLine("Awaiting Packets...");
            Console.WriteLine("Press ESC to Exit...");

            ConsoleKeyInfo keyInfo;
            while (((keyInfo = Console.ReadKey(true)) == null) || (keyInfo.Key != ConsoleKey.Escape)) { }

            OscManager.Disconnect();
            bHaptics.Quit();
            ConfigManager.SaveAll();

            return 0;
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine(Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version}");
            Console.WriteLine($"Created by Herp Derpinstine");
            Console.WriteLine();
        }

        private static void PrintConfig()
        {
            Console.WriteLine($"===== Receiver =====");
            Console.WriteLine();
            Console.WriteLine($"[Port] = {ConfigManager.Connection.receiver.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Sender =====");
            Console.WriteLine();
            Console.WriteLine($"[Enabled] = {ConfigManager.Connection.sender.Value.Enabled}");
            Console.WriteLine($"[IP] = {ConfigManager.Connection.sender.Value.IP}");
            Console.WriteLine($"[Port] = {ConfigManager.Connection.sender.Value.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Devices =====");
            Console.WriteLine();
            Console.WriteLine($"[Head] = {ConfigManager.Devices.Head.Value.Enabled}");
            Console.WriteLine($"[Vest] = {ConfigManager.Devices.Vest.Value.Enabled}");
            Console.WriteLine($"[Arm | Left] = {ConfigManager.Devices.ArmLeft.Value.Enabled}");
            Console.WriteLine($"[Arm | Right] = {ConfigManager.Devices.ArmRight.Value.Enabled}");
            Console.WriteLine($"[Hand | Left] = {ConfigManager.Devices.HandLeft.Value.Enabled}");
            Console.WriteLine($"[Hand | Right] = {ConfigManager.Devices.HandRight.Value.Enabled}");
            Console.WriteLine($"[Foot | Left] = {ConfigManager.Devices.FootLeft.Value.Enabled}");
            Console.WriteLine($"[Foot | Right] = {ConfigManager.Devices.FootRight.Value.Enabled}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Intensity =====");
            Console.WriteLine();
            Console.WriteLine($"[Head] = {ConfigManager.Devices.Head.Value.Intensity}");
            Console.WriteLine($"[Vest] = {ConfigManager.Devices.Vest.Value.Intensity}");
            Console.WriteLine($"[Arm | Left] = {ConfigManager.Devices.ArmLeft.Value.Intensity}");
            Console.WriteLine($"[Arm | Right] = {ConfigManager.Devices.ArmRight.Value.Intensity}");
            Console.WriteLine($"[Hand | Left] = {ConfigManager.Devices.HandLeft.Value.Intensity}");
            Console.WriteLine($"[Hand | Right] = {ConfigManager.Devices.HandRight.Value.Intensity}");
            Console.WriteLine($"[Foot | Left] = {ConfigManager.Devices.FootLeft.Value.Intensity}");
            Console.WriteLine($"[Foot | Right] = {ConfigManager.Devices.FootRight.Value.Intensity}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"===============");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
