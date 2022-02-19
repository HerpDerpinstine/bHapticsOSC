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
            int result = -2;
            if (errors.Any(x => (x is HelpRequestedError || x is VersionRequestedError)))
                result = -1;

            // To-Do: Print and Log each error.

            return result;
        }

        [Option("receiver.address", Required = false, HelpText = "Set the Address for the OSC Receiver.")]
        internal string Receiver_Address { get; set; }
        [Option("receiver.port", Required = false, HelpText = "Set the Port for the OSC Receiver.")]
        internal int Receiver_Port { get; set; }

        [Option("sender.address", Required = false, HelpText = "Set the Address for the OSC Sender.")]
        internal string Sender_Address { get; set; }
        [Option("sender.port", Required = false, HelpText = "Set the Port for the OSC Sender.")]
        internal int Sender_Port { get; set; }
    }
}
