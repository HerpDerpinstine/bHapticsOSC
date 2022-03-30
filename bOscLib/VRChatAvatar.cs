using System;
using System.Collections.Generic;
using System.Linq;
using bHapticsOSC.Utils;
using bHapticsOSC.Config;
using bHapticsOSC.OpenSoundControl;
using Rug.Osc;

namespace bHapticsOSC
{
    internal static class VRChatAvatar
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

                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestFront, $"{Prefix}/bHaptics_Arm_Left"),
                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestBack, $"{Prefix}/bHaptics_Arm_Right"),

                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestFront, $"{Prefix}/bHaptics_Hand_Left"),
                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestBack, $"{Prefix}/bHaptics_Hand_Right"),

                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestFront, $"{Prefix}/bHaptics_Foot_Left"),
                new Tuple<int, bHaptics.PositionType, string>(20, bHaptics.PositionType.VestBack, $"{Prefix}/bHaptics_Foot_Right"),
            })
            {
                Devices[device.Item2] = new Device(device.Item2);
                for (int i = 0; i < device.Item1 + 1; i++)
                    OscManager.Attach($"{device.Item3}_{i}_bool", (string address, OscMessage msg) => OnNode(msg, i, device.Item2));
            }
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

        [OscAddress("/avatar/parameters/InStation")]
        private static void OnStation(string address, OscMessage msg)
        {
            if (msg == null)
                return;
            if (!(msg[0] is bool))
                return;

            InStation = (bool)msg[0];
        }

        [OscAddress("/avatar/change")]
        private static void OnChange(string address, OscMessage msg)
        {
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Reset();
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

                bHaptics.Submit($"{BuildInfo.Name}_{Position}", Position, Value, ThreadedTask.UpdateRate + DurationOffset);
            }

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
