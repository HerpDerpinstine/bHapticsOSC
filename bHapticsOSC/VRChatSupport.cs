﻿using System;
using System.Collections.Generic;
using bHapticsLib;
using OscLib;
using OscLib.Utils;
using OscLib.VRChat;
using Rug.Osc;
using System.Collections.Concurrent;
using System.Threading;
using OscLib.VRChat.Attributes;

namespace bHapticsOSC
{
    internal class VRChatSupport : ThreadedTask
    {
        private bool ShouldRun;
        private Dictionary<PositionID, Device> Devices = new Dictionary<PositionID, Device>();
        private bool AFK;
        private bool InStation;
        private bool Seated;
        private int UdonAudioLink;

        private const string AvatarParameterPrefix = "/avatar/parameters";
        private static Tuple<int, PositionID, string, string>[] DeviceSchemes = new Tuple<int, PositionID, string, string>[]
        {
            new Tuple<int, PositionID, string, string>(6, PositionID.Head, "bHapticsOSC_Head", string.Empty),

            new Tuple<int, PositionID, string, string>(20, PositionID.VestFront, "bHapticsOSC_Vest_Front", string.Empty),
            new Tuple<int, PositionID, string, string>(20, PositionID.VestBack, "bHapticsOSC_Vest_Back", string.Empty),

            new Tuple<int, PositionID, string, string>(6, PositionID.ArmLeft, "bHapticsOSC_Arm_Left", string.Empty),
            new Tuple<int, PositionID, string, string>(6, PositionID.ArmRight, "bHapticsOSC_Arm_Right", string.Empty),

            new Tuple<int, PositionID, string, string>(3, PositionID.HandLeft, "bHapticsOSC_Hand_Left", string.Empty),
            new Tuple<int, PositionID, string, string>(3, PositionID.HandRight, "bHapticsOSC_Hand_Right", string.Empty),

            //new Tuple<int, PositionID, string, string>(0, PositionID.GloveLeft, "bHapticsOSC_Glove_Left", string.Empty),
            //new Tuple<int, PositionID, string, string>(0, PositionID.GloveRight, "bHapticsOSC_Glove_Right", string.Empty),

            new Tuple<int, PositionID, string, string>(3, PositionID.FootLeft, "bHapticsOSC_Foot_Left", string.Empty),
            new Tuple<int, PositionID, string, string>(3, PositionID.FootRight, "bHapticsOSC_Foot_Right", string.Empty),
        };

        private class VRChatPacket { }

        private class VRChatPacket_Node : VRChatPacket
        {
            internal PositionID position;
            internal int node;
            internal int intensity;
        }
        private ConcurrentQueue<VRChatPacket> PacketQueue = new ConcurrentQueue<VRChatPacket>();

        private class VRChatPacket_int : VRChatPacket { internal int value; }
        private class VRChatPacket_bool : VRChatPacket { internal bool value; }
        private class VRChatPacket_string : VRChatPacket { internal string value; }

        private class VRChatPacket_AvatarChange : VRChatPacket_string { }
        private class VRChatPacket_AFK : VRChatPacket_bool { }
        private class VRChatPacket_InStation : VRChatPacket_bool { }
        private class VRChatPacket_Seated : VRChatPacket_bool { }
        private class VRChatPacket_UdonAudioLink : VRChatPacket_int { }

        internal VRChatSupport() : base()
        {
            foreach (Tuple<int, PositionID, string, string> device in DeviceSchemes)
            {
                if (device.Item1 <= 0)
                    continue;
                Devices[device.Item2] = new Device(device.Item2);

                string[] nodeAddressesArr = new string[device.Item1];
                for (int i = 1; i < device.Item1 + 1; i++)
                    nodeAddressesArr[i - 1] = $"{AvatarParameterPrefix}/{device.Item3}_{i}";

                switch (device.Item2)
                {
                    case PositionID.VestFront:
                        Array.Reverse(nodeAddressesArr, 0, 4);
                        Array.Reverse(nodeAddressesArr, 4, 4);
                        Array.Reverse(nodeAddressesArr, 8, 4);
                        Array.Reverse(nodeAddressesArr, 12, 4);
                        Array.Reverse(nodeAddressesArr, 16, 4);
                        break;

                    case PositionID.Head:
                        Array.Reverse(nodeAddressesArr, 0, 6);
                        break;

                    case PositionID.FootRight:
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
                    OscManager.Attach($"{path}_int", (OscMessage msg) => OnNodeIntensity(msg, index, device.Item2));
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
                    if (packet is VRChatPacket_AFK)
                        AFK = ((VRChatPacket_AFK)packet).value;
                    else if (packet is VRChatPacket_InStation)
                        InStation = ((VRChatPacket_InStation)packet).value;
                    else if (packet is VRChatPacket_Seated)
                        Seated = ((VRChatPacket_Seated)packet).value;
                    else if (packet is VRChatPacket_UdonAudioLink)
                        UdonAudioLink = ((VRChatPacket_UdonAudioLink)packet).value;
                    else if (packet is VRChatPacket_AvatarChange)
                    {
                        ResetDevices();
                        if (Program.VRChat.avatarOSCConfigReset.Value.Enabled)
                            VRCAvatarConfig.RemoveFile(((VRChatPacket_AvatarChange)packet).value);
                    }
                    else if (packet is VRChatPacket_Node)
                    {
                        VRChatPacket_Node nodePacket = (VRChatPacket_Node)packet;
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
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_AFK { value = status });

        [VRC_InStation]
        private static void OnInStation(bool status)
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_InStation { value = status });

        [VRC_Seated]
        private static void OnSeated(bool status)
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_Seated { value = status });

        [VRC_AvatarChange]
        private static void OnAvatarChange(string avatarId)
        {
            Console.WriteLine($"Avatar Changed to {avatarId}");
            Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_AvatarChange { value = avatarId });
        }

        [VRC_AvatarParameter("bHapticsOSC_UdonAudioLink")]
        private void OnUdonAudioLink(int amplitude)
            => Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_UdonAudioLink { value = amplitude });

        private static void OnNode(OscMessage msg, int node, PositionID position)
        {
            if ((msg == null) || (!(msg[0] is bool)))
                return;
            Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_Node
            {
                position = position,
                node = node,
                intensity = ((bool)msg[0]) ? Program.Devices.PositionIDToIntensity(position) : 0,
            });
        }

        private static void OnNodeIntensity(OscMessage msg, int node, PositionID position)
        {
            if ((msg == null) || (!(msg[0] is int)))
                return;
            Program.VRCSupport?.PacketQueue.Enqueue(new VRChatPacket_Node
            {
                position = position,
                node = node,
                intensity = (int)msg[0],
            });
        }

        private void SubmitDevices()
        {
            if ((AFK && !Program.VRChat.reactivity.Value.AFK) 
                || (InStation && !Program.VRChat.reactivity.Value.InStation) 
                || (Seated && !Program.VRChat.reactivity.Value.Seated)
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

        private void SetDeviceNodeIntensity(PositionID PositionID, int node, int intensity)
        {
            if ((Devices.Count <= 0) || !Devices.TryGetValue(PositionID, out Device device))
                return;
            device.SetNodeIntensity(node, intensity);
        }

        private class Device
        {
            private PositionID Position;
            private byte[] Buffer = new byte[bHapticsManager.MaxMotorsPerDotPoint];

            internal Device(PositionID position)
                => Position = position;

            internal void Submit()
            {
                if (!Program.Devices.PositionIDToEnabled(Position))
                    return;

                if (!bHapticsManager.IsDeviceConnected(Position))
                    return;

               /* if (Program.UdonAudioLink.PositionIDToEnabled(Position))
                {
                    switch (Program.UdonAudioLink.udonAudioLink.Value.ReactionMode)
                    {
                        case UdonAudioLinkConfig.UdonAudioLink.UdonAudioLinkModeEnum.FULL:
                            Submit_UdonAudioLink_Full();
                            goto default;

                        default:
                            break;
                    }
                }*/

                bHapticsManager.Play($"{BuildInfo.Name}_{Position}", 150, Position, Buffer);
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

            /*
            private void Submit_UdonAudioLink_Full()
            {
                if (Program.VRCSupport.UdonAudioLink <= 0)
                    return;

                int audioLinkIntensity = (Program.UdonAudioLink.PositionIDToIntensity(Position) * (Program.VRCSupport.UdonAudioLink / 100));
                for (int i = 0; i < Buffer.Length; i++)
                    if (Program.UdonAudioLink.udonAudioLink.Value.OverrideTouch || (Buffer[i] < audioLinkIntensity))
                        Buffer[i] = (byte)audioLinkIntensity;
            }
            */
        }
    }
}
