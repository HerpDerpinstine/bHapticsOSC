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

            Console.WriteLine($"Using {LaunchOptions._instance.Address}:{LaunchOptions._instance.Port}");
            Console.WriteLine();

            bHaptics.Load();

            PacketHandler.Setup();
            OpenSoundControl.Run();

            return 0;
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine(Console.Title = $"{BuildInfo.Name} v{BuildInfo.Version}");
            Console.WriteLine($"Created by {BuildInfo.Author}");
            Console.WriteLine();
        }
    }
}
