using System;
using System.Collections.Generic;
using bHapticsLib;
using OscLib;
using OscLib.Utils;
using OscLib.VRChat;
using Rug.Osc;
using System.Collections.Concurrent;
using System.Threading;

namespace bHapticsOSC
{
    internal class VRChatSupport : ThreadedTask
    {
        private bool ShouldRun;
        private Dictionary<PositionType, Device> Devices = new Dictionary<PositionType, Device>();
        private bool AFK;
        private bool InStation;
        private bool Seated;
        private int UdonAudioLink = 0;

        private class VRChatPacket { }
        private class VRChatPacketAvatarChange : VRChatPacket { internal string id; }
        private class VRChatPacketAFK : VRChatPacket { internal bool value; }
        private class VRChatPacketInStation : VRChatPacket { internal bool value; }
        private class VRChatPacketSeated : VRChatPacket { internal bool value; }
        private class VRChatNodePacket : VRChatPacket
        {
            internal PositionType position;
            internal int node;
            internal int intensity;
        }
        private ConcurrentQueue<VRChatPacket> PacketQueue = new ConcurrentQueue<VRChatPacket>();

        internal VRChatSupport() : base()
        {
            string Prefix = "/avatar/parameters";
            foreach (Tuple<int, PositionType, string, string> device in new Tuple<int, PositionType, string, string>[]
            {
                new Tuple<int, PositionType, string, string>(6, PositionType.Head, $"{Prefix}/bHapticsOSC_Head", string.Empty),

                new Tuple<int, PositionType, string, string>(20, PositionType.VestFront, $"{Prefix}/bHapticsOSC_Vest_Front", string.Empty),
                new Tuple<int, PositionType, string, string>(20, PositionType.VestBack, $"{Prefix}/bHapticsOSC_Vest_Back", string.Empty),

                new Tuple<int, PositionType, string, string>(6, PositionType.ForearmL, $"{Prefix}/bHapticsOSC_Arm_Left", string.Empty),
                new Tuple<int, PositionType, string, string>(6, PositionType.ForearmR, $"{Prefix}/bHapticsOSC_Arm_Right", string.Empty),

                new Tuple<int, PositionType, string, string>(3, PositionType.HandL, $"{Prefix}/bHapticsOSC_Hand_Left", string.Empty),
                new Tuple<int, PositionType, string, string>(3, PositionType.HandR, $"{Prefix}/bHapticsOSC_Hand_Right", string.Empty),

                //new Tuple<int, PositionType, string, string>(0, PositionType.GloveLeft, $"{Prefix}/bHapticsOSC_Glove_Left", string.Empty),
                //new Tuple<int, PositionType, string, string>(0, PositionType.GloveRight, $"{Prefix}/bHapticsOSC_Glove_Right", string.Empty),

                new Tuple<int, PositionType, string, string>(3, PositionType.FootL, $"{Prefix}/bHapticsOSC_Foot_Left", string.Empty),
                new Tuple<int, PositionType, string, string>(3, PositionType.FootR, $"{Prefix}/bHapticsOSC_Foot_Right", string.Empty),
            })
            {
                if (device.Item1 <= 0)
                    continue;
                Devices[device.Item2] = new Device(device.Item2);

                string[] nodeAddressesArr = new string[device.Item1];
                for (int i = 1; i < device.Item1 + 1; i++)
                    nodeAddressesArr[i - 1] = $"{device.Item3}_{i}";

                switch (device.Item2)
                {
                    case PositionType.VestFront:
                        Array.Reverse(nodeAddressesArr, 0, 4);
                        Array.Reverse(nodeAddressesArr, 4, 4);
                        Array.Reverse(nodeAddressesArr, 8, 4);
                        Array.Reverse(nodeAddressesArr, 12, 4);
                        Array.Reverse(nodeAddressesArr, 16, 4);
                        break;

                    case PositionType.Head:
                        Array.Reverse(nodeAddressesArr, 0, 6);
                        break;

                    case PositionType.FootR:
                        Array.Reverse(nodeAddressesArr, 0, 3);
                        break;

                    default:
                        break;
                }

                for (int i = 0; i < nodeAddressesArr.Length; i++)
                {
                    string path = nodeAddressesArr[i];
                    int index = i + 1;
                    OscManager.Attach(path, (OscMessage msg) => OnNode(msg, index, device.Item2));
                    OscManager.Attach($"{path.Replace("bHapticsOSC_", "bHaptics_")}_bool", (OscMessage msg) => OnNode(msg, index, device.Item2));
                }
            }
        }

        public override bool BeginInitInternal()
        {
            if (ShouldRun)
                EndInit();

            ShouldRun = true;
            return true;
        }

        public override void WithinThread()
        {
            while (ShouldRun)
            {
                while (PacketQueue.TryDequeue(out VRChatPacket packet))
                {
                    if (packet is VRChatPacketAvatarChange)
                        ResetDevices();
                    else if (packet is VRChatPacketAFK)
                        AFK = ((VRChatPacketAFK)packet).value;
                    else if (packet is VRChatPacketInStation)
                        InStation = ((VRChatPacketInStation)packet).value;
                    else if (packet is VRChatPacketSeated)
                        Seated = ((VRChatPacketSeated)packet).value;
                    else if (packet is VRChatNodePacket)
                    {
                        VRChatNodePacket nodePacket = (VRChatNodePacket)packet;
                        SetDeviceNodeIntensity(nodePacket.position, nodePacket.node, nodePacket.intensity);
                    }
                }

                SubmitDevices();

                if (ShouldRun)
                    Thread.Sleep(100);
            }
        }

        public override bool EndInitInternal()
        {
            ShouldRun = false;
            while (IsAlive()) { Thread.Sleep(100); }
            return true;
        }

        [VRC_AFK]
        private static void OnAFK(bool status)
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacketAFK { value = status });

        [VRC_InStation]
        private static void OnInStation(bool status)
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacketInStation { value = status });

        [VRC_Seated]
        private static void OnSeated(bool status)
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacketSeated { value = status });

        [VRC_AvatarChange]
        private static void OnAvatarChange(string avatarId)
        {
            Console.WriteLine($"Avatar Changed to {avatarId}");

            // To-Do: Append VRChat OSC Avatar Config - JSON

            Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacketAvatarChange { id = avatarId });
        }

        //[VRC_AvatarParameter("bHapticsOSC_UdonAudioLink")]
        //private void OnUdonAudioLink(int amplitude)
        //    => UdonAudioLink = amplitude;

        private static void OnNode(OscMessage msg, int node, PositionType position)
        {
            if ((msg == null) || (!(msg[0] is bool)))
                return;
            Program.VRCSupport.PacketQueue.Enqueue(new VRChatNodePacket
            {
                position = position,
                node = node,
                intensity = ((bool)msg[0]) ? Program.Devices.PositionTypeToIntensity(position) : 0,
            });
        }

        private void SubmitDevices()
        {
            if ((AFK && !Program.VRChat.vrchat.Value.AFK) 
                || ((UdonAudioLink <= 0) && 
                    ((InStation && !Program.VRChat.vrchat.Value.InStation) 
                        || (Seated && !Program.VRChat.vrchat.Value.Seated)))
                || (Devices.Count <= 0))
                return;

            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Submit();
        }

        private void ResetDevices()
        {
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Reset();
        }

        private void SetDeviceNodeIntensity(PositionType positionType, int node, int intensity)
        {
            if ((Devices.Count <= 0) || !Devices.TryGetValue(positionType, out Device device))
                return;
            device.SetNodeIntensity(node, intensity);
        }

        private class Device
        {
            private PositionType Position;
            private byte[] Buffer = new byte[bHapticsManager.MaxBufferSize];

            internal Device(PositionType position)
                => Position = position;

            internal void Submit()
            {
                if (!Program.Devices.PositionTypeToEnabled(Position))
                    return;

                if (!bHapticsManager.IsDeviceConnected(Position))
                    return;

                /*
                if (ConfigManager.UdonAudioLink.PositionTypeToUALEnabled(Position) && (UdonAudioLink > 0))
                {
                    int audioLinkIntensity = (ConfigManager.VRChat.PositionTypeToUALIntensity(Position) * (UdonAudioLink / 100));
                    for (int i = 0; i < Value.Length; i++)
                        if (ConfigManager.UdonAudioLink.udonAudioLink.Value.OverrideTouch || (Value[i] < audioLinkIntensity))
                            Value[i] = (byte)audioLinkIntensity;
                }
                */

                bHapticsManager.Submit($"{BuildInfo.Name}_{Position}", Position, Buffer, 100);
            }

            internal int GetNodeIntensity(int node)
                => Buffer[node - 1];

            internal void SetNodeIntensity(int node, int intensity)
                => Buffer[node - 1] = (byte)intensity;

            internal void Reset()
            {
                for (int i = 1; i < Buffer.Length + 1; i++)
                    SetNodeIntensity(i, 0);
            }
        }
    }
}
