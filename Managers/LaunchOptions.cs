using CommandLine;
using System.Collections.Generic;
using System.Linq;

namespace bHapticsOSC_VRC.Managers
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
            var result = -2;
            if (errors.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                result = -1;
            return result;
        }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
