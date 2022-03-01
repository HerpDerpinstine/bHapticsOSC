using bHapticsOSC_VRC.Managers;
using System;

namespace bHapticsOSC_VRC.Utils
{
    internal class RTParser
    {
		internal byte[] Value = new byte[20];
		internal bHaptics.PositionType Position;
		internal Pixel[] OldColors;

		internal RTParser(bHaptics.PositionType positionType)
			=> Position = positionType;

		internal void Reset()
			=> OldColors = null;

		internal void ParsePixels(Pixel[] pixelcolors, int width, int height)
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

						Pixel pixel = pixelcolors[colorpos];
						Pixel oldpixel = OldColors[colorpos];
						Value[colorpos] = (byte)((pixel != oldpixel) ? Config.Intensity : 0);
					}

				RearrangeValueBuffer();
				bHaptics.Submit($"vrchat_{Position}", Position, Value, 100);
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

		internal class Pixel
		{
			internal float R;
			internal float G;
			internal float B;
			internal float A;

			internal Pixel(float r, float g, float b, float a)
            {
				R = r;
				G = g;
				B = b;
				A = a;
            }

			public override bool Equals(Object obj)
			{
				if (obj == null)
					return false;
				if (!(obj is Pixel))
					return false;
				Pixel right = (Pixel)obj;
				return ((R == right.R)
					&& (G == right.G)
					&& (B == right.B)
					&& (A == right.A));
			}

			public static bool operator ==(Pixel left, Pixel right)
			{
				if (left is null)
				{
					if (right is null)
						return true;
					return false;
				}
				else if (right is null)
					return false;
				return left.Equals(right);
			}

			public static bool operator !=(Pixel left, Pixel right) => !(left == right);
			public override int GetHashCode() => base.GetHashCode();
			public override string ToString() => $"Pixel:({R}, {G}, {B}, {A})";
        }
	}
}
