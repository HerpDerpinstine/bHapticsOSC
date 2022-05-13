using System;

namespace bHapticsLib
{
    public class CommonUtils
    {
        public static long GetCurrentMillis()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            return milliseconds;
        }

        public static T Clamp<T>(T aValue, T aMin, T aMax) where T : IComparable<T>
        {
            var result = aValue;
            if (aValue.CompareTo(aMax) > 0)
                result = aMax;
            else if (aValue.CompareTo(aMin) < 0)
                result = aMin;
            return result;
        }

        public static HapticFeedbackFile ConvertJsonStringToTactosyFile(string json)
        {
            return Parse(json);
        }

        private static HapticFeedbackFile Parse(string json)
        {
            return HapticFeedbackFile.ToHapticFeedbackFile(json);
        }
        
    }
}
