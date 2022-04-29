using System;
using System.Threading;
using Rug.Osc;
using OscLib.Utils;

namespace OscLib
{
    internal class OscReceiverHandler : ThreadedTask
    {
        private OscReceiver Receiver;
        private bool ShouldRun;

        public override bool BeginInitInternal()
        {
            if (Receiver != null)
                EndInit();
            
            ShouldRun = true;
            Receiver = new OscReceiver(System.Net.IPAddress.Any, OscManager.Connection.receiver.Value.Port);
            Receiver.Connect();
            Console.WriteLine("[OscReceiver] Connected!");

            return true;
        }

        public override bool EndInitInternal()
        {
            if (Receiver == null)
                return false;
            
            ShouldRun = false;
            while(IsAlive()) { Thread.Sleep(1); }
            return true;
        }

        public override void WithinThread()
        {
            while (ShouldRun && (Receiver != null) && (Receiver.State != OscSocketState.Closed))
            {
                try
                {
                    while (Receiver.TryReceive(out OscPacket packet) && (packet != null))
                        OscManager.oscPacketQueue.PacketQueue.Enqueue(packet);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in OscReceiver Thread: {ex}");
                }

                if (ShouldRun)
                    Thread.Sleep(1);
                else
                {
                    if ((Receiver.State != OscSocketState.Closing) && (Receiver.State != OscSocketState.Closed))
                        Receiver.Close();
                    Receiver.Dispose();
                    Receiver = null;
                }
            }

            Console.WriteLine("[OscReceiver] Disconnected!");
        }
    }
}
