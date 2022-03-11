using Rug.Osc;
using System;
using System.Net;
using System.Threading;

namespace bHapticsOSC.Managers
{
    internal static class OpenSoundControl
    {
        internal static OscAddressManager AddressManager = new OscAddressManager();
        internal static OscReceiver Receiver;

        internal static void Run()
        {
            Console.WriteLine("Creating OSC Connection...");
            Receiver = new OscReceiver(IPAddress.Parse(ConfigManager.Connection.Address), ConfigManager.Connection.Port);
            Receiver.Connect();
        }

        internal static void Disconnect()
        {
            if (Receiver.State == OscSocketState.Closed)
                return;
            Receiver.Close();
            Console.WriteLine("Disconnected!");
        }

        internal static void ReceivePackets()
        {
            Console.WriteLine("Awaiting Packets...");
            Console.WriteLine();
            while (Receiver.State != OscSocketState.Closed)
            {
                try
                {
                    while (Receiver.TryReceive(out OscPacket packet) && (packet != null))
                    {
                        switch (AddressManager.ShouldInvoke(packet))
                        {
                            case OscPacketInvokeAction.Pospone:
                            case OscPacketInvokeAction.Invoke:
                                if (AddressManager.Invoke(packet) && packet is OscMessage)
                                    Console.WriteLine(((OscMessage)packet).Address);
                                goto default;
                            case OscPacketInvokeAction.HasError:
                                throw new Exception($"Error while reading OscPacket: {packet.Error}");
                            case OscPacketInvokeAction.DontInvoke:
                            default:
                                break;
                        }
                    }

                    HapticsHandler.RunThread();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in ReceiverThread: {ex}");
                }
                Thread.Sleep(ConfigManager.Connection.UpdateRate);
            }
        }
    }
}
