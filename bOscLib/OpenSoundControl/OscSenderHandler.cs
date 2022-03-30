using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using bHapticsOSC.Config;
using bHapticsOSC.Utils;
using Rug.Osc;

namespace bHapticsOSC.OpenSoundControl
{
    internal class OscSenderHandler : ThreadedTask
    {
        private OscSender Sender;
        private List<OscPacket> PacketQueue = new List<OscPacket>();

        internal void SendPacket(OscPacket packet)
        {
            if (!ConfigManager.Connection.sender.Value.Enabled
                || (Sender == null)
                || (Sender.State == OscSocketState.Closed))
                return;

            PacketQueue.Add(packet);
        }

        public override bool BeginInitInternal()
        {
            if (!ConfigManager.Connection.sender.Value.Enabled)
                return false;

            if (Sender == null)
                Sender = new OscSender(IPAddress.Parse(ConfigManager.Connection.sender.Value.IP), ConfigManager.Connection.sender.Value.Port);
            else if (Sender.State != OscSocketState.Closed)
                return false;

            Sender.Connect();

            Console.WriteLine("[Sender] Connected!");
            return true;
        }

        public override bool EndInitInternal()
        {
            if (!ConfigManager.Connection.sender.Value.Enabled 
                || (Sender == null)
                || (Sender.State == OscSocketState.Closed))
                return false;

            Sender.Close();
            Sender = null;
            Console.WriteLine("[Sender] Disconnected!");

            return true;
        }

        public override void WithinThread()
        {
            while (Sender.State != OscSocketState.Closed)
            {
                if (PacketQueue.Count > 0)
                {
                    lock (PacketQueue)
                    {
                        foreach (OscPacket packet in PacketQueue)
                            Sender.Send(packet);
                        PacketQueue.Clear();
                    }
                }

                Thread.Sleep(UpdateRate);
            }
        }
    }
}
