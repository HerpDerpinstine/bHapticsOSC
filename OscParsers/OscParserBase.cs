using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bHapticsOSC.OscParsers
{
    internal class OscParserBase
    {
        internal virtual string GetAddress() => "/unknown";
        internal virtual void Reset() { }
        internal virtual void Process(OscMessage oscMessage) { }
    }
}
