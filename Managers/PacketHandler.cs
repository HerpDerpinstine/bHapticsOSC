using Rug.Osc;
using System.Collections.Generic;
using bHapticsOSC_VRC.Utils;

namespace bHapticsOSC_VRC.Managers
{
    internal static class PacketHandler
    {
		internal static RTParser Vest_Front = new RTParser(bHaptics.PositionType.VestFront);
		internal static RTParser Vest_Back = new RTParser(bHaptics.PositionType.VestBack);

		internal static void Setup()
        {
			// Avatar Change
			OpenSoundControl.OnMessageReceived["/avatar/change"] = (OscMessage msg) => ResetParsers();

			// Vest
			OpenSoundControl.OnMessageReceived["/bhaptics/vest/front"] = (OscMessage msg) => ProcessMessage(msg, Vest_Front);
			OpenSoundControl.OnMessageReceived["/bhaptics/vest/back"] = (OscMessage msg) => ProcessMessage(msg, Vest_Back);
		}

		private static void ResetParsers()
        {
			Vest_Front.Reset();
			Vest_Back.Reset();
        }

		private static void ProcessMessage(OscMessage oscMessage, RTParser parser)
        {
			byte[] blob = (byte[])oscMessage[0];

			int width = blob[0];
			int height = blob[1];

			List<byte> pixels = new List<byte>(blob);
			pixels.RemoveAt(0);
			pixels.RemoveAt(0);
			parser.ParsePixels(RawDataToColorArray(pixels.ToArray(), pixels.Count), width, height);
		}

		private static unsafe RTParser.Pixel[] RawDataToColorArray(byte[] rawdata, int rawdata_length)
		{
			RTParser.Pixel[] colors = new RTParser.Pixel[rawdata_length / 4];
			for (int i = 0; i < rawdata_length; i += 4)
				colors[i / 4] = new RTParser.Pixel(rawdata[i] / 255f, rawdata[i + 1] / 255f, rawdata[i + 2] / 255f, rawdata[i + 3] / 255f);
			return colors;
		}
	}
}
