using CommandLine;
using System.Collections.Generic;
using System.Linq;

namespace bHapticsOSC.Managers
{
    internal class LaunchOptions
    {
        internal static LaunchOptions _instance;

        internal static int OnParse(LaunchOptions instance)
        {
            _instance = instance;
            return 0;
        }

        internal static int OnError(IEnumerable<Error> errors)
        {
            int result = -2;
            if (errors.Any(x => (x is HelpRequestedError || x is VersionRequestedError)))
                result = -1;

            // To-Do: Print and Log each error.

            return result;
        }

        [Option("osc.address", Required = false, HelpText = "Set the Address for the OSC Connection.")]
        internal string Address { get; set; } = "127.0.0.1";
        [Option("osc.port", Required = false, HelpText = "Set the Port for the OSC Connection.")]
        internal int Port { get; set; } = 9001;
    }
}
