using System;
using System.Threading;
using Rug.Osc;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;

namespace bHapticsOSC.OpenSoundControl
{
    internal class OscReceiverHandler : ThreadedTask
    {
        private static OscReceiver Receiver;

        public override bool BeginInitInternal()
        {
            if (Receiver == null)
                Receiver = new OscReceiver(ConfigManager.Connection.receiver.Value.Port);
            else if (Receiver.State != OscSocketState.Closed)
                return false;

            Receiver.Connect();

            Console.WriteLine("[Receiver] Connected!");

            return true;
        }

        public override bool EndInitInternal()
        {
            if ((Receiver == null) || (Receiver.State == OscSocketState.Closed))
                return false;

            Receiver.Close();
            Receiver = null;
            Console.WriteLine("[Receiver] Disconnected!");

            return true;
        }

        public override void WithinThread()
        {
            while (Receiver.State != OscSocketState.Closed)
            {
                try
                {
                    while (Receiver.TryReceive(out OscPacket packet) && (packet != null))
                    {
                        OscManager.Send(packet);
                        switch (OscManager.ShouldInvoke(packet))
                        {
                            case OscPacketInvokeAction.Pospone:
                            case OscPacketInvokeAction.Invoke:
                                OscManager.Invoke(packet);
                                goto default;
                            case OscPacketInvokeAction.HasError:
                                throw new Exception($"Error while reading OscPacket: {packet.Error}");
                            case OscPacketInvokeAction.DontInvoke:
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in ReceiverThread: {ex}");
                }

                VRChatAvatar.SubmitPackets();
                Thread.Sleep(UpdateRate);
            }
        }
    }
}
