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
            int result = Parser.Default.ParseArguments<LaunchOptions>(args).MapResult(o => LaunchOptions.OnParse(o), errors => LaunchOptions.OnError(errors));
            if (result != 0)
                return result;

            bHaptics.Load();

            while (!KillMainThread) { }
            return 0;
        }
    }
}
