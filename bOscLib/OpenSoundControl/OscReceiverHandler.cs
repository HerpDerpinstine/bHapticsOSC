using System;
using System.Threading;
using Rug.Osc;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;

namespace bHapticsOSC.OpenSoundControl
{
    internal class OscReceiverHandler : ThreadedTask
    {
        private OscReceiver Receiver;
        private bool ShouldRun = true;

        public override bool BeginInitInternal()
        {
            if (Receiver != null)
                EndInitInternal();
            
            ShouldRun = true;
            Receiver = new OscReceiver(ConfigManager.Connection.receiver.Value.Port);
            Receiver.Connect();
            Console.WriteLine("[OscReceiver] Connected!");

            return true;
        }

        public override bool EndInitInternal()
        {
            if (Receiver == null)
                return false;
            
            ShouldRun = false;
            while(IsAlive()) { Thread.Sleep(UpdateRate); }
            return true;
        }

        public override void WithinThread()
        {
            while (ShouldRun && (Receiver != null) && (Receiver.State != OscSocketState.Closed))
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
                    Console.WriteLine($"Exception in OscReceiver Thread: {ex}");
                }

                VRChatSupport.SubmitPackets();

                if (ShouldRun)
                    Thread.Sleep(UpdateRate);
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
