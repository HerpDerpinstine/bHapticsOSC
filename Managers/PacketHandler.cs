using Rug.Osc;
using System.Collections.Generic;
using bHapticsOSC.Utils;
using bHapticsOSC.OscParsers;
using System;

namespace bHapticsOSC.Managers
{
    internal static class PacketHandler
	{
		private static string bHapticsOscHeader = "/bhaptics";
		private static Dictionary<string, List<OscParserBase>> Parsers = new Dictionary<string, List<OscParserBase>>();

		internal static void Setup()
		{
			// VRChat Avatar Change
			OpenSoundControl.OnMessageReceived["/avatar/change"] = (OscMessage msg) => ResetParsers();

			// Head
			AddPositionRTParser(bHaptics.PositionType.Head);

			// Vest
			AddPositionRTParser(bHaptics.PositionType.VestFront);
			AddPositionRTParser(bHaptics.PositionType.VestBack);

			// Arms
			AddPositionRTParser(bHaptics.PositionType.ForearmL);
			AddPositionRTParser(bHaptics.PositionType.ForearmR);

			// Hands
			AddPositionRTParser(bHaptics.PositionType.HandL);
			AddPositionRTParser(bHaptics.PositionType.HandR);

			// Feet
			AddPositionRTParser(bHaptics.PositionType.FootL);
			AddPositionRTParser(bHaptics.PositionType.FootR);
		}

		private static void AddParser<T>(string address, T parser) where T : OscParserBase
		{
			if (!Parsers.TryGetValue(address, out List<OscParserBase> parsertbl))
				lock (Parsers)
					Parsers[address] = parsertbl = new List<OscParserBase>();
			parsertbl.Add(parser);
			OpenSoundControl.OnMessageReceived[address] = (OscMessage msg) => parser.Process(msg);
		}

		private static PositionRTParser AddPositionRTParser(bHaptics.PositionType positionType)
		{
			PositionRTParser parser = new PositionRTParser(positionType);
			AddParser(ParserToOscAddress(positionType, parser), parser);
			return parser;
		}

		private static void ResetParsers()
        {
			foreach (List<OscParserBase> parserList in Parsers.Values)
				foreach (OscParserBase parser in parserList)
					parser.Reset();
		}

		private static string ParserToOscAddress(bHaptics.PositionType positionType, OscParserBase oscParser)
        {
			string positionStr = "/unknown";
			switch (positionType)
            {
				// Head
				case bHaptics.PositionType.Head:
					positionStr = "/head";
					goto default;

				// Vest
				case bHaptics.PositionType.Vest:
					positionStr = "/vest";
					goto default;
				case bHaptics.PositionType.VestFront:
					positionStr = "/vest/front";
					goto default;
				case bHaptics.PositionType.VestBack:
					positionStr = "/vest/back";
					goto default;

				// Arms
				case bHaptics.PositionType.ForearmL:
					positionStr = "/arm/left";
					goto default;
				case bHaptics.PositionType.ForearmR:
					positionStr = "/arm/right";
					goto default;

				// Hands
				case bHaptics.PositionType.HandL:
					positionStr = "/hand/left";
					goto default;
				case bHaptics.PositionType.HandR:
					positionStr = "/hand/right";
					goto default;

				// Feet
				case bHaptics.PositionType.FootL:
					positionStr = "/foot/left";
					goto default;
				case bHaptics.PositionType.FootR:
					positionStr = "/foot/right";
					goto default;

				// Unknown
				default:
					break;
            }

			return $"{bHapticsOscHeader}{oscParser.GetAddress()}{positionStr}";
		}
	}
}
