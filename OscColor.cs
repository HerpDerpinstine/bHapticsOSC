using System;

namespace bHapticsOSC
{
	internal class OscColor
	{
		internal float R;
		internal float G;
		internal float B;
		internal float A;

		internal OscColor(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		internal static OscColor[] FromByteArray(byte[] rawdata)
		{
			OscColor[] colors = new OscColor[rawdata.Length / 4];
			for (int i = 0; i < rawdata.Length; i += 4)
				colors[i / 4] = new OscColor(rawdata[i] / 255f, rawdata[i + 1] / 255f, rawdata[i + 2] / 255f, rawdata[i + 3] / 255f);
			return colors;
		}

		public override bool Equals(Object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is OscColor))
				return false;
			OscColor right = (OscColor)obj;
			return ((R == right.R)
				&& (G == right.G)
				&& (B == right.B)
				&& (A == right.A));
		}

		public static bool operator ==(OscColor left, OscColor right)
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

		public static bool operator !=(OscColor left, OscColor right) => !(left == right);
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString() => $"Pixel:({R}, {G}, {B}, {A})";
	}
}
