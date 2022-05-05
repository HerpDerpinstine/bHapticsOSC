using OscLib.Utils;
using Rug.Osc;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace OscLib
{
    public class OscPacketQueue : ThreadedTask
    {
        public ConcurrentQueue<OscPacket> PacketQueue = new ConcurrentQueue<OscPacket>();
        private bool ShouldRun;

        public override bool BeginInitInternal()
        {
            if (ShouldRun)
                EndInit();

            ShouldRun = true;
            return true;
        }

        public override bool EndInitInternal()
        {
            ShouldRun = false;
            while (IsAlive()) { Thread.Sleep(1); }
            return true;
        }

        public override void WithinThread()
        {
            while (ShouldRun)
            {
                while (PacketQueue.TryDequeue(out OscPacket packet))
                {
                    if (OscManager.Connection.sender.Value.PipeAllPackets)
                        OscManager.oscSender.Send(packet);

                    switch (OscManager.ShouldInvoke(packet))
                    {
                        case OscPacketInvokeAction.Pospone:
                        case OscPacketInvokeAction.Invoke:

                            if (!OscManager.Connection.sender.Value.PipeAllPackets)
                                OscManager.oscSender.Send(packet);

                            OscManager.Invoke(packet);

                            goto default;
                        case OscPacketInvokeAction.HasError:
                            throw new Exception($"Error while reading OscPacket: {packet.Error}");
                        case OscPacketInvokeAction.DontInvoke:
                        default:
                            break;
                    }
                }

                if (ShouldRun)
                    Thread.Sleep(1);
            }
        }
    }
}
