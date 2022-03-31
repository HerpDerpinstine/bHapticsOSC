using System;
using System.Net;
using bHapticsOSC.Config;
using Rug.Osc;

namespace bHapticsOSC.OpenSoundControl
{
    internal class OscSenderHandler
    {
        private OscSender Sender;

        public void BeginInit()
        {
            if (Sender != null)
                EndInit();

            if (!ConfigManager.Connection.sender.Value.Enabled)
                return;

            Sender = new OscSender(IPAddress.Parse(ConfigManager.Connection.sender.Value.IP), 0, ConfigManager.Connection.sender.Value.Port);
            Sender.Connect();
            Console.WriteLine("[OscSender] Connected!");
        }

        public void EndInit()
        {
            if (Sender == null)
                return;

            if ((Sender.State != OscSocketState.Closing) && (Sender.State != OscSocketState.Closed))
                Sender.Close();
            Sender.Dispose();
            Sender = null;

            Console.WriteLine("[OscSender] Disconnected!");
        }

        internal void Send(OscPacket packet)
        {
            if (!ConfigManager.Connection.sender.Value.Enabled
                || (Sender == null)
                || (Sender.State == OscSocketState.Closing)
                || (Sender.State == OscSocketState.Closed))
                return;

            Sender.Send(packet);
        }
    }
}
