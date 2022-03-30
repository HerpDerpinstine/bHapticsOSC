using System;
using System.Net;
using bHapticsOSC.Config;
using Rug.Osc;

namespace bHapticsOSC.OpenSoundControl
{
    internal class OscSenderHandler
    {
        private OscSender Sender;

        internal void Send(OscPacket packet)
        {
            if (!ConfigManager.Connection.sender.Value.Enabled
                || (Sender == null)
                || (Sender.State == OscSocketState.Closed))
                return;

            Sender.Send(packet);
        }

        public void BeginInit()
        {
            if (!ConfigManager.Connection.sender.Value.Enabled)
                return;

            if (Sender == null)
                Sender = new OscSender(IPAddress.Parse(ConfigManager.Connection.sender.Value.IP), ConfigManager.Connection.sender.Value.Port);
            else if (Sender.State != OscSocketState.Closed)
                return;

            Sender.Connect();

            Console.WriteLine("[Sender] Connected!");
        }

        public void EndInit()
        {
            if (!ConfigManager.Connection.sender.Value.Enabled 
                || (Sender == null)
                || (Sender.State == OscSocketState.Closed))
                return;

            Sender.WaitForAllMessagesToComplete();
            Sender.Close();
            Sender = null;
            Console.WriteLine("[Sender] Disconnected!");
        }
    }
}
