using System;
using System.Collections.Generic;
using bHapticsOSC.Managers;
using bHapticsOSC.Utils;
using Rug.Osc;

namespace bHapticsOSC.OscParsers
{
	internal class PositionRTParser : OscParserBase
	{
		internal byte[] Value = new byte[20];
		internal bHaptics.PositionType Position;
		internal OscColor[] OldColors;

		internal PositionRTParser(bHaptics.PositionType positionType)
			=> Position = positionType;

		internal override string GetAddress() => "/rt";

        internal override void Reset()
			=> OldColors = null;

		internal override void Process(OscMessage oscMessage)
		{
			if (!(oscMessage[0] is byte[]))
				return;

			byte[] blob = (byte[])oscMessage[0];

			int width = blob[0];
			int height = blob[1];

			List<byte> pixels = new List<byte>(blob);
			pixels.RemoveAt(0);
			pixels.RemoveAt(0);
			Parse(OscColor.FromByteArray(pixels.ToArray()), width, height);
		}

		private void Parse(OscColor[] pixelcolors, int width, int height)
		{
			if ((pixelcolors == null)
				|| (pixelcolors.Length <= 0))
				return;

			if (OldColors == null)
				OldColors = pixelcolors;
			else
			{
				for (int col = 0; col < height; col++)
					for (int row = 0; row < width; row++)
					{
						int bytepos = row * height + col;
						int colorpos = bytepos - 1;

						if (colorpos < 0)
							colorpos = 0;
						else if (colorpos >= 0)
							colorpos += 1;

						OscColor pixel = pixelcolors[colorpos];
						OscColor oldpixel = OldColors[colorpos];
						Value[colorpos] = (byte)((pixel != oldpixel) ? Config.Intensity : 0);
					}

				RearrangeValueBuffer();
				bHaptics.Submit($"{BuildInfo.Name}{Position}", Position, Value, 100);
			}
		}

		private void RearrangeValueBuffer()
		{
			Array.Reverse(Value, 0, Value.Length);

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
		}
	}
}
