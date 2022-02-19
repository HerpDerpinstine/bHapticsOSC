using CommandLine;
using bHapticsOSC_VRC.Utils;
using bHapticsOSC_VRC.Managers;

namespace bHapticsOSC_VRC
{
    internal static class Program
    {
        internal static bool KillMainThread = false;

        internal static int Main(string[] args)
        {
            // Parse Launch Options
            int result = Parser.Default.ParseArguments<LaunchOptions>(args).MapResult(o => LaunchOptions.OnParse(o), errors => LaunchOptions.OnError(errors));
            if (result != 0)
                return result;

            // Load and Initialize bHaptics Native Library
            bHaptics.Load();

            // Load Connection Information
            ConnectionInfo.Load();

            // Initialize OSC Management

            // Connect OSC to VRChat using Connection Information

            // Run Sender and Receiver Threads

            while (!KillMainThread) { }
            return 0;
        }
    }
}
