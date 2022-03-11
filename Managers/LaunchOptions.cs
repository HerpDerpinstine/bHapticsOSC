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

        [Option("osc.address", Required = false, HelpText = "Sets the Address for the OSC Connection.")]
        internal string Address { get; set; } = "127.0.0.1";
        [Option("osc.port", Required = false, HelpText = "Sets the Port for the OSC Connection.")]
        internal int Port { get; set; } = 9001;

        [Option("osc.updaterate", Required = false, HelpText = "Sets the Update Rate for the OSC Receiver Connection in Milliseconds.")]
        internal int OscUpdateRate { get; set; } = 100; // ms

        [Option("intensity.head", Required = false, HelpText = "Sets the Intensy of the HEAD.")]
        internal int Intensity_Head { get; set; } = 100;

        [Option("intensity.vestfront", Required = false, HelpText = "Sets the Intensy of the Front of the VEST.")]
        internal int Intensity_Vest_Front { get; set; } = 100;
        [Option("intensity.vestback", Required = false, HelpText = "Sets the Intensy of the Back of the VEST.")]
        internal int Intensity_Vest_Back { get; set; } = 100;

        [Option("intensity.leftarm", Required = false, HelpText = "Sets the Intensy of the LEFT ARM.")]
        internal int Intensity_Arm_Left { get; set; } = 100;
        [Option("intensity.rightarm", Required = false, HelpText = "Sets the Intensy of the RIGHT ARM.")]
        internal int Intensity_Arm_Right { get; set; } = 100;

        [Option("intensity.lefthand", Required = false, HelpText = "Sets the Intensy of the LEFT HAND.")]
        internal int Intensity_Hand_Left { get; set; } = 100;
        [Option("intensity.righthand", Required = false, HelpText = "Sets the Intensy of the RIGHT HAND.")]
        internal int Intensity_Hand_Right { get; set; } = 100;

        [Option("intensity.leftfoot", Required = false, HelpText = "Sets the Intensy of the LEFT FOOT.")]
        internal int Intensity_Foot_Left { get; set; } = 100;
        [Option("intensity.rightfoot", Required = false, HelpText = "Sets the Intensy of the RIGHT FOOT.")]
        internal int Intensity_Foot_Right { get; set; } = 100;
    }
}
