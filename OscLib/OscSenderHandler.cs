using System;
using System.Net;
using Rug.Osc;

namespace OscLib
{
    internal class OscSenderHandler
    {
        private OscSender Sender;

        public void BeginInit()
        {
            if (Sender != null)
                EndInit();

            if (!OscManager.Connection.sender.Value.Enabled)
                return;

            Sender = new OscSender(IPAddress.Parse(OscManager.Connection.sender.Value.IP), OscManager.Connection.sender.Value.Port, OscManager.Connection.sender.Value.Port);
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
            if (!OscManager.Connection.sender.Value.Enabled
                || (Sender == null)
                || (Sender.State == OscSocketState.Closing)
                || (Sender.State == OscSocketState.Closed))
                return;

            Sender.Send(packet);
        }
    }
}
