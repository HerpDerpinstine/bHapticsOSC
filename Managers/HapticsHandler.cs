using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using bHapticsOSC.Utils;
using Rug.Osc;

namespace bHapticsOSC.Managers
{
    internal static class HapticsHandler
    {
        internal static Dictionary<bHaptics.PositionType, Device> Devices = new Dictionary<bHaptics.PositionType, Device>()
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

        internal static void RunThread()
        {
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.SubmitPacket();
        }

        internal static void SetDeviceNodeIntensity(bHaptics.PositionType positionType, int node, int intensity)
        {
            Console.WriteLine($"SetDeviceNodeIntensity: {positionType} [{node}]");
            if (Devices.Count <= 0)
                return;
            if (!Devices.TryGetValue(positionType, out Device device))
                return;
            device.SetNodeIntensity(node, intensity);
        }

        internal static void ResetAllDevices()
        {
            Console.WriteLine("ResetAllDevices");
            if (Devices.Count <= 0)
                return;
            foreach (Device device in Devices.Values)
                device.Reset();
        }

        internal class Device
        {
            internal bHaptics.PositionType Position;
            internal byte[] Packet = new byte[bHaptics.MaxBufferSize];

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

                bHaptics.Submit($"{BuildInfo.Name}_{Position}", Position, Value, ConfigManager.Connection.UpdateRate + 50);
            }

            internal void SetNodeIntensity(int node, int intensity)
                => Packet[node - 1] = (byte)intensity;

            internal void Reset()
            {
                for (int i = 0; i < Packet.Length; i++)
                    SetNodeIntensity(i, 0);
            }
        }
    }
}
