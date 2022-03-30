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
        private static int DurationOffset = 50; // ms
        private static bool InStation = false;

        private static Dictionary<bHaptics.PositionType, Device> Devices = new Dictionary<bHaptics.PositionType, Device>()
        {
            { bHaptics.PositionType.Head, new Device() { Position = bHaptics.PositionType.Head } },

            { bHaptics.PositionType.VestFront, new Device() { Position = bHaptics.PositionType.VestFront } },
            { bHaptics.PositionType.VestBack, new Device() { Position = bHaptics.PositionType.VestBack } },

            { bHaptics.PositionType.ForearmL, new Device() { Position = bHaptics.PositionType.ForearmL } },
            { bHaptics.PositionType.ForearmR, new Device() { Position = bHaptics.PositionType.ForearmR } },

            { bHaptics.PositionType.HandL, new Device() { Position = bHaptics.PositionType.HandL } },
            { bHaptics.PositionType.HandR, new Device() { Position = bHaptics.PositionType.HandR } },

            { bHaptics.PositionType.FootL, new Device() { Position = bHaptics.PositionType.FootL } },
            { bHaptics.PositionType.FootR, new Device() { Position = bHaptics.PositionType.FootR } },
        };

        [OscAddress("/avatar/parameters/bHaptics_Head", 6, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Vest_Front", 20, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Vest_Back", 20, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Arm_Left", 6, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Arm_Right", 6, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Hand_Left", 3, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Hand_Right", 3, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Foot_Left", 3, "bool")]
        [OscAddress("/avatar/parameters/bHaptics_Foot_Right", 3, "bool")]
        private static void OnNodeStatusReceived(string address, OscMessage msg)
        {
            if (msg == null)
                return;
            if (!(msg[0] is bool))
                return;

            bHaptics.PositionType positionType = bHaptics.PositionType.All; // Parse Address to Position
            int node = 1; // Parse Address to Node
            int intensity = 100; // Get Config Value for Position Type

            if ((bool)msg[0])
                SetDeviceNodeIntensity(positionType, node, intensity);
            else
                SetDeviceNodeIntensity(positionType, node, 0);
        }

        [OscAddress("/avatar/parameters/InStation")]
        private static void OnStationStatusReceived(string address, OscMessage msg)
        {
            if (msg == null)
                return;
            if (!(msg[0] is bool))
                return;

            InStation = (bool)msg[0];
        }

        [OscAddress("/avatar/change")]
        private static void OnResetReceived(string address, OscMessage msg)
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
            internal bHaptics.PositionType Position;
            private byte[] Packet = new byte[bHaptics.MaxBufferSize];

            internal void SubmitPacket()
            {
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

                bHaptics.Submit($"{BuildInfo.Name}_{Position}", Position, Value, ConfigManager.Connection.threading.Value.UpdateRate + DurationOffset);
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
