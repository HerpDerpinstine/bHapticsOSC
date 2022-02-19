#pragma warning disable 0649

namespace bHapticsOSC_VRC.Managers
{
    internal class ConnectionInfo
    {
        // Static
        internal static ConnectionInfo Sender;
        internal static ConnectionInfo Receiver;

        // Instance
        internal string Address;
        internal int Port;

        internal static void Load()
        {

        }
    }
}