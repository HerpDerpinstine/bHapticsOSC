using System;
using System.Collections.Generic;

namespace bHapticsLib
{
    public class DotPoint
    {
        public DotPoint(int index, int intensity)
        {
            if (index < 0)
            {
                throw new HapticException("Invalid argument index : " + index);
            }
            Intensity = CommonUtils.Clamp(intensity, 0, 100);

            Index = index;
        }

        public int Index { get; set; }
        public int Intensity { get; set; }
        
        public override string ToString()
        {
            return "DotPoint { Index=" + Index +
                   ", Intensity=" + Intensity + "}";
        }

        public JSONObject ToJsonObject()
        {
            var jsonObject = new JSONObject();
            jsonObject["index"] = Index;
            jsonObject["intensity"] = Intensity;

            return jsonObject;
        }
    }

    public class PathPoint
    {
        public PathPoint(float x, float y, int intensity, int motorCount = 3)
        {
            X = CommonUtils.Clamp(x, 0f, 1f);
            Y = CommonUtils.Clamp(y, 0f, 1f);
            Intensity = CommonUtils.Clamp(intensity, 0, 100);
            MotorCount = CommonUtils.Clamp(motorCount, 0, 3);
        }

        public float X { get; set; }
        public float Y { get; set; }
        public int Intensity { get; set; }

        // Number of maximum motors to vibrate
        // if 0 means default motor count, now 3
        public int MotorCount { get; set; }

        public override string ToString()
        {
            return "PathPoint { X=" + X +
                   ", Y=" + Y +
                   ", MotorCount=" + MotorCount +
                   ", Intensity=" + Intensity + "}";
        }

        public JSONObject ToJsonObject()
        {
            var jsonObject = new JSONObject();
            jsonObject["x"] = X;
            jsonObject["y"] = Y;
            jsonObject["intensity"] = Intensity;
            jsonObject["motorCount"] = MotorCount;

            return jsonObject;
        }
    }

    public class HapticFeedback
    {
        #region public members
        public PositionType Position { get; set; }
        public byte[] Values { get; set; }
        #endregion

        #region Constructor
        public HapticFeedback(PositionType position, byte[] values)
        {
            Position = position;
            Values = values;
        }
        #endregion

        public override string ToString()
        {
            return "HapticFeedback {Position=" + Position +
                ", Values=" + "}";
        }
    }

    public enum PositionType
    {
        All = 0,
        [Obsolete("Use ForearmL instead")]
        Left = 1,
        [Obsolete("Use ForearmR instead")]
        Right = 2,        
        Vest = 3,
        Head = 4,
        [Obsolete("Will not work anymore")]
        Racket = 5, // Deprecated
        HandL = 6,
        HandR = 7,
        FootL = 8,
        FootR = 9,
        ForearmL = 10, ForearmR = 11,
        VestFront =201, VestBack=202,
        GloveL =203, GloveR=204,
        Custom1 =251, Custom2 = 252, Custom3 = 253, Custom4 = 254
    }
    
    public enum FeedbackMode
    {
        PATH_MODE = 1,
        DOT_MODE = 2
    }

    public class EnumParser
    {
        private static Dictionary<string, PositionType> _positionMappings;
        private static Dictionary<string, FeedbackMode> _modeMappings;
        private static Dictionary<string, PlaybackType> _playbackMapping;
        private static Dictionary<string, PathMovingPattern> _movingPatternMapping;

        public static PositionType ToPositionType(string str)
        {
            if (_positionMappings == null)
            {
                _positionMappings = new Dictionary<string, PositionType>();

                foreach (PositionType positionType in Enum.GetValues(typeof(PositionType)))
                {
                    _positionMappings[positionType.ToString()] = positionType;
                }
            }

            PositionType type;
            if (_positionMappings.TryGetValue(str, out type))
            {
                return type;
            }

            return PositionType.All;
        }

        public static FeedbackMode ToMode(string str)
        {
            if (_modeMappings == null)
            {
                _modeMappings = new Dictionary<string, FeedbackMode>();

                foreach (FeedbackMode positionType in Enum.GetValues(typeof(FeedbackMode)))
                {
                    _modeMappings[positionType.ToString()] = positionType;
                }
            }

            FeedbackMode type;
            if (_modeMappings.TryGetValue(str, out type))
            {
                return type;
            }

            return FeedbackMode.DOT_MODE;
        }

        public static PlaybackType ToPlaybackType(string str)
        {
            if (_playbackMapping == null)
            {
                _playbackMapping = new Dictionary<string, PlaybackType>();

                foreach (PlaybackType positionType in Enum.GetValues(typeof(PlaybackType)))
                {
                    _playbackMapping[positionType.ToString()] = positionType;
                }
            }

            PlaybackType type;
            if (_playbackMapping.TryGetValue(str, out type))
            {
                return type;
            }

            return PlaybackType.NONE;
        }

        public static PathMovingPattern ToMovingPattern(string str)
        {
            if (_movingPatternMapping == null)
            {
                _movingPatternMapping = new Dictionary<string, PathMovingPattern>();

                foreach (PathMovingPattern positionType in Enum.GetValues(typeof(PathMovingPattern)))
                {
                    _movingPatternMapping[positionType.ToString()] = positionType;
                }
            }

            PathMovingPattern type;
            if (_movingPatternMapping.TryGetValue(str, out type))
            {
                return type;
            }

            return PathMovingPattern.CONST_SPEED;
        }
    }
}
