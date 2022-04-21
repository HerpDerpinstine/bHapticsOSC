using System;
using System.Collections.Generic;
using System.Linq;
using bHapticsOSC.Utils;
using bHapticsOSC.Config;
using bHapticsOSC.OpenSoundControl;
using bHapticsOSC.VRChat;
using Rug.Osc;

namespace bHapticsOSC
{
    internal static class VRChatSupport
    {
        private static Dictionary<bHaptics.PositionType, Device> Devices = new Dictionary<bHaptics.PositionType, Device>();
        private static int DurationOffset = 50; // ms
        private static bool AFK = false;
        private static bool InStation = false;
        private static bool Seated = false;
        private static int UdonAudioLink = 0;

        internal static void SetupDevices()
        {
            string Prefix = "/avatar/parameters";
            foreach (Tuple<int, bHaptics.PositionType, string> device in new Tuple<int, bHaptics.PositionType, string>[]
            {
                new Tuple<int, bHaptics.PositionType, string>(6, bHaptics.PositionType.Head, $"{Prefix}/bHapticsOSC_Head"),

                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestFront, $"{Prefix}/bHapticsOSC_Vest_Front"),
                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestBack, $"{Prefix}/bHapticsOSC_Vest_Back"),

                new Tuple<int, bHaptics.PositionType, string>(6, bHaptics.PositionType.ForearmL, $"{Prefix}/bHapticsOSC_Arm_Left"),
                new Tuple<int, bHaptics.PositionType, string>(6, bHaptics.PositionType.ForearmR, $"{Prefix}/bHapticsOSC_Arm_Right"),

                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.HandL, $"{Prefix}/bHapticsOSC_Hand_Left"),
                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.HandR, $"{Prefix}/bHapticsOSC_Hand_Right"),

                //new Tuple<int, bHaptics.PositionType, string>(0, bHaptics.PositionType.GloveLeft, $"{Prefix}/bHapticsOSC_Glove_Left"),
                //new Tuple<int, bHaptics.PositionType, string>(0, bHaptics.PositionType.GloveRight, $"{Prefix}/bHapticsOSC_Glove_Right"),

                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.FootL, $"{Prefix}/bHapticsOSC_Foot_Left"),
                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.FootR, $"{Prefix}/bHapticsOSC_Foot_Right"),
            })
            {
                if (device.Item1 <= 0)
                    continue;
                Devices[device.Item2] = new Device(device.Item2);
                for (int i = 1; i < device.Item1 + 1; i++)
                {
                    int motorIndex = i;
                    string path = $"{device.Item3}_{motorIndex}";
                    OscManager.Attach(path, (OscMessage msg) => OnNode(msg, motorIndex, device.Item2));
                    OscManager.Attach(path.Replace("bHapticsOSC_", "bHaptics_"), (OscMessage msg) => OnNode(msg, motorIndex, device.Item2));
                }
            }
        }

        [VRC_AFK]
        private static void OnAFK(bool status)
            => AFK = status;

        [VRC_InStation]
        private static void OnInStation(bool status)
            => InStation = status;

        [VRC_Seated]
        private static void OnSeated(bool status)
            => Seated = status;

        [VRC_AvatarChange]
        private static void OnAvatarChange(string avatarId)
        {
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Reset();
        }

        //[VRC_AvatarParameter("bHapticsOSC_UdonAudioLink")]
        //private static void OnUdonAudioLink(int amplitude)
        //    => UdonAudioLink = amplitude;

        private static void OnNode(OscMessage msg, int node, bHaptics.PositionType position)
        {
            if ((msg == null) || (!(msg[0] is bool)))
                return;
            if ((bool)msg[0])
                SetDeviceNodeIntensity(position, node, ConfigManager.Devices.PositionTypeToIntensity(position));
            else
                SetDeviceNodeIntensity(position, node, 0);
        }

        internal static void SubmitPackets()
        {
            if ((AFK && !ConfigManager.VRChat.vrchat.Value.AFK) 
                || ((UdonAudioLink <= 0) && 
                    ((InStation && !ConfigManager.VRChat.vrchat.Value.InStation) 
                        || (Seated && !ConfigManager.VRChat.vrchat.Value.Seated)))
                || (Devices.Count <= 0))
                return;
            foreach (Device device in Devices.Values)
                device.SubmitPacket();
        }

        private static void SetDeviceNodeIntensity(bHaptics.PositionType positionType, int node, int intensity)
        {
            if ((Devices.Count <= 0) || !Devices.TryGetValue(positionType, out Device device))
                return;
            device.SetNodeIntensity(node, intensity);
        }

        private class Device
        {
            private bHaptics.PositionType Position;
            private byte[] Packet = new byte[bHaptics.MaxBufferSize];

            internal Device(bHaptics.PositionType position)
                => Position = position;

            internal void SubmitPacket()
            {
                if (!ConfigManager.Devices.PositionTypeToEnabled(Position))
                    return;

                byte[] Value = Packet.ToArray();
                switch (Position)
                {
                    case bHaptics.PositionType.VestFront:
                        Array.Reverse(Value, 0, 4);
                        Array.Reverse(Value, 4, 4);
                        Array.Reverse(Value, 8, 4);
                        Array.Reverse(Value, 12, 4);
                        Array.Reverse(Value, 16, 4);
                        break;

                    case bHaptics.PositionType.Head:
                        Array.Reverse(Value, 0, 6);
                        break;

                    case bHaptics.PositionType.FootR:
                        Array.Reverse(Value, 0, 3);
                        break;
                }

                /*
                if (ConfigManager.UdonAudioLink.PositionTypeToUALEnabled(Position) && (UdonAudioLink > 0))
                {
                    int audioLinkIntensity = (ConfigManager.VRChat.PositionTypeToUALIntensity(Position) * (UdonAudioLink / 100));
                    for (int i = 0; i < Value.Length; i++)
                        if (ConfigManager.UdonAudioLink.udonAudioLink.Value.OverrideTouch || (Value[i] < audioLinkIntensity))
                            Value[i] = (byte)audioLinkIntensity;
                }
                */

                bHaptics.Submit($"{BuildInfo.Name}_{Position}", Position, Value, ThreadedTask.UpdateRate + DurationOffset);
            }

            internal int GetNodeIntensity(int node)
                => Packet[node - 1];

            internal void SetNodeIntensity(int node, int intensity)
                => Packet[node - 1] = (byte)intensity;

            internal void Reset()
            {
                for (int i = 1; i < Packet.Length + 1; i++)
                    SetNodeIntensity(i, 0);
            }
        }
    }
}
