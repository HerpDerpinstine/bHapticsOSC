using System;
using System.Net;
using Rug.Osc;

namespace OscLib
{
    internal class OscSenderHandler
    {
        private OscSender Sender;
        private string Name;

        internal OscSenderHandler(string name)
            => Name = name;

        public void BeginInit(string ipAddress, int port)
        {
            if (Sender != null)
                EndInit();

            Sender = new OscSender(IPAddress.Parse(ipAddress), 0, port);
            Sender.Connect();
            Console.WriteLine($"[{Name}] Connected!");
        }

        public void EndInit()
        {
            if (Sender == null)
                return;

            if ((Sender.State != OscSocketState.Closing) && (Sender.State != OscSocketState.Closed))
                Sender.Close();
            Sender.Dispose();
            Sender = null;

            Console.WriteLine($"[{Name}] Disconnected!");
        }

        internal void Send(OscPacket packet)
        {
            if ((Sender == null)
                || (Sender.State == OscSocketState.Closing)
                || (Sender.State == OscSocketState.Closed))
                return;

            Sender.Send(packet);
        }
    }
}
