using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using Rug.Osc;

namespace AvatarEmulator
{
    public static class AvatarActor
    {
        private static OscSender Sender;
        // private static OscReceiver receiver;

        private static Timer timer;

        public static OscSocketState State { get; private set; }
        public static Action ConnectionChanged;
        
        public static void Connect(string Ip = "127.0.0.1")
        {
            if (Sender != null)
            {
                EndInit();
            }

            Sender = new OscSender(IPAddress.Parse(Ip), 9001);
            Sender.Connect();
            
            UpdateState(Sender.State);
        }

        public static void EndInit()
        {
            if ((Sender == null)
                || (Sender.State == OscSocketState.Closed))
                return;

            Sender.WaitForAllMessagesToComplete();
            Sender.Close();
            Sender = null;

            UpdateState(OscSocketState.NotConnected);
            Console.WriteLine("[AvatarActor] Sender Disconnected!");
        }

        public static void Send(OscPacket packet)
        {
            if (Sender == null
                || Sender.State == OscSocketState.Closed)
            {

                UpdateState(OscSocketState.Closed);
                Console.WriteLine("[AvatarActor] Sender Disconnected!");
                return;
            }

            UpdateState(Sender.State);

            Console.WriteLine($"[AvatarActor] Sender Send {packet} ${Sender.State}");
            Sender.Send(packet);
        }

        private static void UpdateState(OscSocketState cState)
        {
            if (cState != State)
            {
                State = cState;
                ConnectionChanged?.Invoke();
            }

        }

    }
}
