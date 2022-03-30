using System;
using System.Threading;
using Rug.Osc;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;

namespace bHapticsOSC.OpenSoundControl
{
    internal class OscSenderHandler : ThreadedTask
    {
        public override bool BeginInitInternal()
        {
            if (!ConfigManager.Connection.sender.Value.Enabled)
                return false;

            // Connect Sender

            return true;
        }

        public override bool EndInitInternal()
        {
            if (!ConfigManager.Connection.sender.Value.Enabled)
                return false;

            // Disconnect Sender

            return true;
        }

        public override void WithinThread()
        {
            // Process Queue
        }
    }
}
