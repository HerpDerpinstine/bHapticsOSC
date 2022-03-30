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

            return true;
        }

        public override bool EndInitInternal()
        {

            return true;
        }

        public override void WithinThread()
        {

        }
    }
}
