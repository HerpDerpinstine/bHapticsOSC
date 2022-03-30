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

        private const string Address_Head = "/avatar/parameters/bHaptics_Head";

        //private const string Address_Vest = "/avatar/parameters/bHaptics_Vest";

#region TO_REMOVE_LATER
        private const string Address_Vest_Front = "/avatar/parameters/bHaptics_Vest_Front";
        private const string Address_Vest_Back = "/avatar/parameters/bHaptics_Vest_Back";
#endregion

        private const string Address_Arm_Left = "/avatar/parameters/bHaptics_Arm_Left";
        private const string Address_Arm_Right = "/avatar/parameters/bHaptics_Arm_Right";

        private const string Address_Hand_Left = "/avatar/parameters/bHaptics_Hand_Left";
        private const string Address_Hand_Right = "/avatar/parameters/bHaptics_Hand_Right";

        private const string Address_Foot_Left = "/avatar/parameters/bHaptics_Foot_Left";
        private const string Address_Foot_Right = "/avatar/parameters/bHaptics_Foot_Right";

        private static Dictionary<bHaptics.PositionType, Device> Devices = new Dictionary<bHaptics.PositionType, Device>()
        {
            { bHaptics.PositionType.Head, new Device(bHaptics.PositionType.Head) },

            //{ bHaptics.PositionType.Vest, new Device(bHaptics.PositionType.Vest) },

#region TO_REMOVE_LATER
            { bHaptics.PositionType.VestFront, new Device(bHaptics.PositionType.VestFront) },
            { bHaptics.PositionType.VestBack, new Device(bHaptics.PositionType.VestBack) },
#endregion

            { bHaptics.PositionType.ForearmL, new Device(bHaptics.PositionType.ForearmL) },
            { bHaptics.PositionType.ForearmR, new Device(bHaptics.PositionType.ForearmR) },

            { bHaptics.PositionType.HandL, new Device(bHaptics.PositionType.HandL) },
            { bHaptics.PositionType.HandR, new Device(bHaptics.PositionType.HandR) },

            { bHaptics.PositionType.FootL, new Device(bHaptics.PositionType.FootL) },
            { bHaptics.PositionType.FootR, new Device(bHaptics.PositionType.FootR)  },
        };
        
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

        [OscAddress(Address_Head, 6, "bool")]
        private static void OnHeadNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.Head, Address_Head);

        //[OscAddress(Address_Vest, 40, "bool")]
        //private static void OnVestNode(string address, OscMessage msg)
        //    => OnNode(address, msg, bHaptics.PositionType.Vest, Address_Vest);

#region TO_REMOVE_LATER
        [OscAddress(Address_Vest_Front, 20, "bool")]
        private static void OnVestFrontNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.VestFront, Address_Vest_Front);
        [OscAddress(Address_Vest_Back, 20, "bool")]
        private static void OnVestBackNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.VestBack, Address_Vest_Back);
#endregion

        [OscAddress(Address_Arm_Left, 6, "bool")]
        private static void OnArmLeftNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.ForearmL, Address_Arm_Left);
        [OscAddress(Address_Arm_Right, 6, "bool")]
        private static void OnArmRightNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.ForearmR, Address_Arm_Right);

        [OscAddress(Address_Hand_Left, 3, "bool")]
        private static void OnHandLeftNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.ForearmL, Address_Hand_Left);
        [OscAddress(Address_Hand_Right, 3, "bool")]
        private static void OnHandRightNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.ForearmR, Address_Hand_Right);

        [OscAddress(Address_Foot_Left, 3, "bool")]
        private static void OnFootLeftNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.ForearmL, Address_Foot_Left);
        [OscAddress(Address_Foot_Right, 3, "bool")]
        private static void OnFootRightNode(string address, OscMessage msg)
            => OnNode(address, msg, bHaptics.PositionType.ForearmR, Address_Foot_Right);

        private static void OnNode(string address, OscMessage msg, bHaptics.PositionType position, string partialAddress)
        {
            if (!ConfigManager.Devices.PositionTypeToEnabled(position))
                return;

            if (msg == null)
                return;
            if (!(msg[0] is bool))
                return;

            string nodestr = address.Substring(partialAddress.Length + 1);
            nodestr = nodestr.Substring(0, nodestr.Length - 5);
            if (!int.TryParse(nodestr, out int node))
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
