using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace bHapticsOSC_VRC.Managers
{
    internal static class OpenSoundControl
    {
        internal static OscReceiver Receiver;
        internal static Dictionary<string, Action<OscMessage>> OnMessageReceived = new Dictionary<string, Action<OscMessage>>();

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

        private static void ParseMessageThread(OscMessage oscMessage)
        {
            if (!OnMessageReceived.TryGetValue(oscMessage.Address, out Action<OscMessage> method))
                return;
            method?.Invoke(oscMessage);
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
                        if ((packet != null) && (packet is OscMessage))
                            new Thread(() => ParseMessageThread((OscMessage)packet)).Start();
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
