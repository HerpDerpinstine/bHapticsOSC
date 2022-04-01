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
        private static bool InStation = false;

        internal static void SetupDevices()
        {
            string Prefix = "/avatar/parameters";
            foreach (Tuple<int, bHaptics.PositionType, string> device in new Tuple<int, bHaptics.PositionType, string>[]
            {
                new Tuple<int, bHaptics.PositionType, string>(6, bHaptics.PositionType.Head, $"{Prefix}/bHaptics_Head"),

                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestFront, $"{Prefix}/bHaptics_Vest_Front"),
                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestBack, $"{Prefix}/bHaptics_Vest_Back"),

                new Tuple<int, bHaptics.PositionType, string>(6, bHaptics.PositionType.ForearmL, $"{Prefix}/bHaptics_Arm_Left"),
                new Tuple<int, bHaptics.PositionType, string>(6, bHaptics.PositionType.ForearmR, $"{Prefix}/bHaptics_Arm_Right"),

                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.HandL, $"{Prefix}/bHaptics_Hand_Left"),
                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.HandR, $"{Prefix}/bHaptics_Hand_Right"),

                //new Tuple<int, bHaptics.PositionType, string>(0, bHaptics.PositionType.GloveLeft, $"{Prefix}/bHaptics_Glove_Left"),
                //new Tuple<int, bHaptics.PositionType, string>(0, bHaptics.PositionType.GloveRight, $"{Prefix}/bHaptics_Glove_Right"),

                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.FootL, $"{Prefix}/bHaptics_Foot_Left"),
                new Tuple<int, bHaptics.PositionType, string>(3, bHaptics.PositionType.FootR, $"{Prefix}/bHaptics_Foot_Right"),
            })
            {
                if (device.Item1 <= 0)
                    continue;
                Devices[device.Item2] = new Device(device.Item2, device.Item3);
                for (int i = 1; i < device.Item1 + 1; i++)
                    OscManager.Attach($"{device.Item3}_{i}_bool", (OscMessage msg) => OnNode(msg, i, device.Item2));
            }

            ConfigManager.Devices.OnFileModified += RefreshNodeIntensity;
        }

        [VRC_InStation]
        private static void OnInStationChanged(bool status)
            => InStation = status;

        [VRC_AvatarChange]
        private static void OnChange(string address, OscMessage msg)
        {
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Reset();
        }

        private static void OnNode(OscMessage msg, int node, bHaptics.PositionType position)
        {
            if (msg == null)
                return;
            if (!(msg[0] is bool))
                return;

            if ((bool)msg[0])
                SetDeviceNodeIntensity(position, node, ConfigManager.Devices.PositionTypeToIntensity(position));
            else
                SetDeviceNodeIntensity(position, node, 0);
        }

        internal static void SubmitPackets()
        {
            if (Devices.Count <= 0)
                return;
            if (InStation && !ConfigManager.VRChat.vrchat.Value.InStation)
                return;

            foreach (Device device in Devices.Values)
                device.SubmitPacket();
        }

        private static void RefreshNodeIntensity()
        {
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.RefreshNodeIntensity();
        }

        private static void SetDeviceNodeIntensity(bHaptics.PositionType positionType, int node, int intensity)
        {
            if (Devices.Count <= 0)
                return;
            if (!Devices.TryGetValue(positionType, out Device device))
                return;
            device.SetNodeIntensity(node, intensity);
        }

        private class Device
        {
            private static string Address;
            private bHaptics.PositionType Position;
            private byte[] Packet = new byte[bHaptics.MaxBufferSize];

            internal Device(bHaptics.PositionType position, string address)
            {
                Position = position;
                Address = address;
            }

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

                bHaptics.Submit($"{BuildInfo.Name}_{Position}", Position, Value, ThreadedTask.UpdateRate + DurationOffset);

                for (int i = 1; i < Value.Length + 1; i++)
                {
                    int node_val = Value[i - 1];
                    OscManager.Send(new OscMessage($"{Address}_{i}_bool", new object[] { (node_val > 0) ? true : false }));
                }
            }

            internal int GetNodeIntensity(int node)
                => Packet[node - 1];

            internal void SetNodeIntensity(int node, int intensity)
                => Packet[node - 1] = (byte)intensity;

            internal void RefreshNodeIntensity()
            {
                int intensity = ConfigManager.Devices.PositionTypeToIntensity(Position);
                for (int i = 0; i < Packet.Length; i++)
                    if (Packet[i] > 0)
                        Packet[i] = (byte)intensity;
            }

            internal void Reset()
            {
                for (int i = 1; i < Packet.Length + 1; i++)
                    SetNodeIntensity(i, 0);
            }
        }
    }
}
