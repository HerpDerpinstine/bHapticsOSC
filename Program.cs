using bHapticsOSC.Utils;
using bHapticsOSC.Managers;
using System;

namespace bHapticsOSC
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            WelcomeMessage();

            ConfigManager.Setup();
            PrintConfig();

            bHaptics.Load();

            PacketHandler.Setup();
            OpenSoundControl.Run();
            OpenSoundControl.ReceivePackets();

            OpenSoundControl.Disconnect();
            bHaptics.Quit();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            while (Console.ReadKey(true) == null) { }
            return 0;
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine(Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version}");
            Console.WriteLine($"Created by {BuildInfo.Author}");
            Console.WriteLine();
        }

        private static void PrintConfig()
        {
            Console.WriteLine($"===== Connection =====");
            Console.WriteLine();
            Console.WriteLine($"[Port] = {ConfigManager.Connection.Port}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Threading =====");
            Console.WriteLine();
            Console.WriteLine($"[Update Rate] = {ConfigManager.Connection.UpdateRate}ms");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Devices =====");
            Console.WriteLine();
            Console.WriteLine($"[Head] = {ConfigManager.Devices.Head.Enabled}");
            Console.WriteLine($"[Vest] = {ConfigManager.Devices.Vest.Enabled}");
            Console.WriteLine($"[Arm | Left] = {ConfigManager.Devices.ArmLeft.Enabled}");
            Console.WriteLine($"[Arm | Right] = {ConfigManager.Devices.ArmRight.Enabled}");
            Console.WriteLine($"[Hand | Left] = {ConfigManager.Devices.HandLeft.Enabled}");
            Console.WriteLine($"[Hand | Right] = {ConfigManager.Devices.HandRight.Enabled}");
            Console.WriteLine($"[Foot | Left] = {ConfigManager.Devices.FootLeft.Enabled}");
            Console.WriteLine($"[Foot | Right] = {ConfigManager.Devices.FootRight.Enabled}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"===== Intensity =====");
            Console.WriteLine();
            Console.WriteLine($"[Head] = {ConfigManager.Devices.Head.Intensity}");
            Console.WriteLine($"[Vest] = {ConfigManager.Devices.Vest.Intensity}");
            Console.WriteLine($"[Arm | Left] = {ConfigManager.Devices.ArmLeft.Intensity}");
            Console.WriteLine($"[Arm | Right] = {ConfigManager.Devices.ArmRight.Intensity}");
            Console.WriteLine($"[Hand | Left] = {ConfigManager.Devices.HandLeft.Intensity}");
            Console.WriteLine($"[Hand | Right] = {ConfigManager.Devices.HandRight.Intensity}");
            Console.WriteLine($"[Foot | Left] = {ConfigManager.Devices.FootLeft.Intensity}");
            Console.WriteLine($"[Foot | Right] = {ConfigManager.Devices.FootRight.Intensity}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"===============");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
