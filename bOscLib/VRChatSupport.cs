using System;
using System.Collections.Generic;
using bHapticsOSC.Utils;
using bHapticsOSC.Config;
using bHapticsOSC.OpenSoundControl;
using bHapticsOSC.VRChat;
using Rug.Osc;
using Bhaptics.Tact;

namespace bHapticsOSC
{
    internal static class VRChatSupport
    {
        private static Dictionary<PositionType, Device> Devices = new Dictionary<PositionType, Device>();
        private static bool AFK = false;
        private static bool InStation = false;
        private static bool Seated = false;
        private static int UdonAudioLink = 0;

        internal static void SetupDevices()
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
            Console.WriteLine("Avatar Changed");
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Reset();
        }

        //[VRC_AvatarParameter("bHapticsOSC_UdonAudioLink")]
        //private static void OnUdonAudioLink(int amplitude)
        //    => UdonAudioLink = amplitude;

        private static void OnNode(OscMessage msg, int node, PositionType position)
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

        private static void SetDeviceNodeIntensity(PositionType positionType, int node, int intensity)
        {
            if ((Devices.Count <= 0) || !Devices.TryGetValue(positionType, out Device device))
                return;
            device.SetNodeIntensity(node, intensity);
        }

        private class Device
        {
            private PositionType Position;
            private byte[] Packet = new byte[bHaptics.MaxBufferSize];

            internal Device(PositionType position)
                => Position = position;

            internal void SubmitPacket()
            {
                if (!ConfigManager.Devices.PositionTypeToEnabled(Position))
                    return;

                if (!bHaptics.IsDeviceConnected(Position))
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

                bHaptics.Submit($"{BuildInfo.Name}_{Position}", Position, Packet, 100);
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
