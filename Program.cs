using CommandLine;
using bHapticsOSC.Utils;
using bHapticsOSC.Managers;
using System;
using Rug.Osc;

namespace bHapticsOSC
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            WelcomeMessage();

            int result = Parser.Default.ParseArguments<LaunchOptions>(args).MapResult(o => LaunchOptions.OnParse(o), errors => LaunchOptions.OnError(errors));
            if (result != 0)
                return result;

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
            Console.WriteLine($"===== OSC =====");
            Console.WriteLine();
            Console.WriteLine($"[Address] = {LaunchOptions._instance.Address}:{LaunchOptions._instance.Port}");
            Console.WriteLine($"[Update Rate] = {LaunchOptions._instance.UpdateRate}ms");
            Console.WriteLine();

            Console.WriteLine($"===== Intensity =====");
            Console.WriteLine();
            Console.WriteLine($"[Head] = {LaunchOptions._instance.Intensity_Head}");
            Console.WriteLine();
            Console.WriteLine($"[Vest | Front] = {LaunchOptions._instance.Intensity_Vest_Front}");
            Console.WriteLine($"[Vest | Back] = {LaunchOptions._instance.Intensity_Vest_Back}");
            Console.WriteLine();
            Console.WriteLine($"[Arm | Left] = {LaunchOptions._instance.Intensity_Arm_Left}");
            Console.WriteLine($"[Arm | Right] = {LaunchOptions._instance.Intensity_Arm_Right}");
            Console.WriteLine();
            Console.WriteLine($"[Hand | Left] = {LaunchOptions._instance.Intensity_Hand_Left}");
            Console.WriteLine($"[Hand | Right] = {LaunchOptions._instance.Intensity_Hand_Right}");
            Console.WriteLine();
            Console.WriteLine($"[Foot | Left] = {LaunchOptions._instance.Intensity_Foot_Left}");
            Console.WriteLine($"[Foot | Right] = {LaunchOptions._instance.Intensity_Foot_Right}");
            Console.WriteLine();
            Console.WriteLine($"===============");
            Console.WriteLine();
        }
    }
}
