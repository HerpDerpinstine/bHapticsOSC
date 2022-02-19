using Rug.Osc;
using System;
using System.Net;

namespace bHapticsOSC_VRC.Managers
{
    internal static class OpenSoundControl
    {
        internal static OscReceiver Receiver;
        internal static Action<OscPacket> OnPacketReceived;

        internal static void Run()
        {
            Console.WriteLine("Connecting OSC Receiver...");

            Receiver = new OscReceiver(IPAddress.Parse(LaunchOptions._instance.Address), LaunchOptions._instance.Port);
            Receiver.Connect();

            Console.WriteLine("Awaiting Packets...");
            Console.WriteLine();

            ReceiverThread();

            Receiver.Close();
            Console.WriteLine("Disconnected!");
        }
        
        private static void ReceiverThread()
        {
            while (Receiver.State != OscSocketState.Closed)
            {
                try
                {
                    if (Receiver.State == OscSocketState.Connected)
                    {
                        OscPacket packet = Receiver.Receive();
                        if (packet != null)
                            OnPacketReceived?.Invoke(packet);
                    }
                }
                catch (Exception ex)
                {
                    if (Receiver.State != OscSocketState.Connected)
                        return;
                    Console.WriteLine($"Exception in ReceiverThread: {ex}");
                }
            }
        }
    }
}
