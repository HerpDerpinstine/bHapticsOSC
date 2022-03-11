using Rug.Osc;
using bHapticsOSC.Utils;
using System.IO;
using System.Text;
using System;

namespace bHapticsOSC.Managers
{
    internal static class PacketHandler
	{
		internal static void Setup()
		{
			OpenSoundControl.AddressManager.Attach($"{Addresses.VRChat_Avatar.Prefix}{Addresses.VRChat_Avatar.OnChange}", (OscMessage msg) => HapticsHandler.ResetAllDevices());

			AttachVRCAPNodes(bHaptics.PositionType.Head, 6);
			AttachVRCAPNodes(bHaptics.PositionType.VestFront, 20);
			AttachVRCAPNodes(bHaptics.PositionType.VestBack, 20);
			AttachVRCAPNodes(bHaptics.PositionType.ForearmL, 3);
			AttachVRCAPNodes(bHaptics.PositionType.ForearmR, 3);
			AttachVRCAPNodes(bHaptics.PositionType.HandL, 3);
			AttachVRCAPNodes(bHaptics.PositionType.HandR, 3);
			AttachVRCAPNodes(bHaptics.PositionType.FootL, 3);
			AttachVRCAPNodes(bHaptics.PositionType.FootR, 3);
		}

		private static void AttachVRCAPNodes(bHaptics.PositionType positionType, int nodeCount)
		{
			for (int i = 1; i < nodeCount + 1; i++)
				AttachVRCAPAddress(positionType, i);
		}

		private static void AttachVRCAPAddress(bHaptics.PositionType positionType, int node)
		{
			string address = Addresses.PositionToVRCAPAddress(positionType, node);
			OpenSoundControl.AddressManager.Attach(address, (OscMessage msg) => PlayHaptics(msg, positionType, node));
		}

		private static void PlayHaptics(OscMessage msg, bHaptics.PositionType positionType, int node)
		{
			if (!(msg[0] is bool))
				return;

			if ((bool)msg[0])
				HapticsHandler.SetDeviceNodeIntensity(positionType, node, Config.Intensity);
			else
				HapticsHandler.SetDeviceNodeIntensity(positionType, node, 0);
		}

		internal static class Addresses
		{
			internal static string Unknown = "/unknown";

			internal static class VRChat_Avatar
			{
				internal static string Prefix = "/avatar";
				internal static string OnChange = "/change";
				internal static string Parameters = "/parameters";
			}

			internal static string PositionToAddress(bHaptics.PositionType positionType)
			{
				switch (positionType)
				{
					// Head
					case bHaptics.PositionType.Head:
						return "/head";

					// Vest
					case bHaptics.PositionType.Vest:
						return "/vest";
					case bHaptics.PositionType.VestFront:
						return "/vest/front";
					case bHaptics.PositionType.VestBack:
						return "/vest/back";

					// Arms
					case bHaptics.PositionType.ForearmL:
						return "/arm/left";
					case bHaptics.PositionType.ForearmR:
						return "/arm/right";

					// Hands
					case bHaptics.PositionType.HandL:
						return "/hand/left";
					case bHaptics.PositionType.HandR:
						return "/hand/right";

					// Feet
					case bHaptics.PositionType.FootL:
						return "/foot/left";
					case bHaptics.PositionType.FootR:
						return "/foot/right";

					// Unknown
					default:
						return Unknown;
				}
			}

			internal static string PositionToVRCAPAddress(bHaptics.PositionType positionType, int node)
			{
				switch (positionType)
				{
					// Head
					case bHaptics.PositionType.Head:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Head_{node}_bool";

					// Vest
					case bHaptics.PositionType.VestFront:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Vest_Front_{node}_bool";
					case bHaptics.PositionType.VestBack:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Vest_Back_{node}_bool";

					// Arms
					case bHaptics.PositionType.ForearmL:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Arm_Left_{node}_bool";
					case bHaptics.PositionType.ForearmR:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Arm_Right_{node}_bool";

					// Hands
					case bHaptics.PositionType.HandL:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Hand_Left_{node}_bool";
					case bHaptics.PositionType.HandR:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Hand_Right_{node}_bool";

					// Feet
					case bHaptics.PositionType.FootL:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Foot_Left_{node}_bool";
					case bHaptics.PositionType.FootR:
						return $"{VRChat_Avatar.Prefix}{VRChat_Avatar.Parameters}/bHaptics_Foot_Right_{node}_bool";

					// Unknown
					default:
						return Unknown;
				}
			}
		}
	}
}
