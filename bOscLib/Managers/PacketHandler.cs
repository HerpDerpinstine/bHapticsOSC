using Rug.Osc;
using bHapticsOSC.Utils;

namespace bHapticsOSC.Managers
{
	public static class PacketHandler
	{
		public static void Setup()
		{
			OpenSoundControl.AddressManager.Attach($"{Addresses.VRChatAvatar.Prefix}{Addresses.VRChatAvatar.OnChange}", (OscMessage msg) => HapticsHandler.ResetAllDevices());

			AttachVRCAPNodes(bHaptics.PositionType.Head, ConfigManager.Devices.Head.Value.Intensity, 6);

			AttachVRCAPNodes(bHaptics.PositionType.VestFront, ConfigManager.Devices.Vest.Value.Intensity, 20);
			AttachVRCAPNodes(bHaptics.PositionType.VestBack, ConfigManager.Devices.Vest.Value.Intensity, 20);

			AttachVRCAPNodes(bHaptics.PositionType.ForearmL, ConfigManager.Devices.ArmLeft.Value.Intensity, 6);
			AttachVRCAPNodes(bHaptics.PositionType.ForearmR, ConfigManager.Devices.ArmRight.Value.Intensity, 6);

			AttachVRCAPNodes(bHaptics.PositionType.HandL, ConfigManager.Devices.HandLeft.Value.Intensity, 3);
			AttachVRCAPNodes(bHaptics.PositionType.HandR, ConfigManager.Devices.HandRight.Value.Intensity, 3);

			AttachVRCAPNodes(bHaptics.PositionType.FootL, ConfigManager.Devices.FootLeft.Value.Intensity, 3);
			AttachVRCAPNodes(bHaptics.PositionType.FootR, ConfigManager.Devices.FootRight.Value.Intensity, 3);

			OpenSoundControl.AddressManager.Attach($"{Addresses.VRChatAvatar.Prefix}{Addresses.VRChatAvatar.Parameters}{Addresses.VRChatAvatar.InStation}", (OscMessage msg) => 
			{
				if (!(msg[0] is bool))
					return;
				HapticsHandler.InStation = (bool)msg[0];
			});
		}

		private static void AttachVRCAPNodes(bHaptics.PositionType positionType, int intensity, int nodeCount)
		{
			for (int i = 1; i < nodeCount + 1; i++)
				AttachVRCAPAddress(positionType, i, intensity);
		}

		private static void AttachVRCAPAddress(bHaptics.PositionType positionType, int node, int intensity)
		{
			string address = Addresses.PositionToVRCAPAddress(positionType, node);
			OpenSoundControl.AddressManager.Attach(address, (OscMessage msg) => PlayHaptics(msg, positionType, node, intensity));
		}

		private static void PlayHaptics(OscMessage msg, bHaptics.PositionType positionType, int node, int intensity)
		{
			if (!(msg[0] is bool))
				return;

			if ((bool)msg[0])
				HapticsHandler.SetDeviceNodeIntensity(positionType, node, intensity);
			else
				HapticsHandler.SetDeviceNodeIntensity(positionType, node, 0);
		}

		internal static class Addresses
		{
			internal static string Unknown = "/unknown";

			internal static class VRChatAvatar
			{
				internal static string Prefix = "/avatar";
				internal static string OnChange = "/change";
				internal static string Parameters = "/parameters";
				internal static string InStation = "/InStation";
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
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Head_{node}_bool";

					// Vest
					case bHaptics.PositionType.VestFront:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Vest_Front_{node}_bool";
					case bHaptics.PositionType.VestBack:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Vest_Back_{node}_bool";

					// Arms
					case bHaptics.PositionType.ForearmL:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Arm_Left_{node}_bool";
					case bHaptics.PositionType.ForearmR:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Arm_Right_{node}_bool";

					// Hands
					case bHaptics.PositionType.HandL:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Hand_Left_{node}_bool";
					case bHaptics.PositionType.HandR:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Hand_Right_{node}_bool";

					// Feet
					case bHaptics.PositionType.FootL:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Foot_Left_{node}_bool";
					case bHaptics.PositionType.FootR:
						return $"{VRChatAvatar.Prefix}{VRChatAvatar.Parameters}/bHaptics_Foot_Right_{node}_bool";

					// Unknown
					default:
						return Unknown;
				}
			}
		}
	}
}
