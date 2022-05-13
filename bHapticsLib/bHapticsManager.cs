using System;
using System.Collections.Generic;

namespace bHapticsLib
{
    public static class bHapticsManager
    {
        public const int MaxBufferSize = 20;
        private static IHapticPlayer hapticPlayer;

        public static void Load(string appId, string appName)
        {
            if (!ExePathCheck()
                && !SteamLibraryCheck())
                throw new Exception("bHaptics Player is Not Installed!");

            Console.WriteLine("Loading bHaptics Library...");

            appName = appName.Replace(" ", "_");
            appId = appId.Replace(" ", "_");

            hapticPlayer = new HapticPlayer(appId, appName);
            hapticPlayer.Enable();
        }

        private static bool ExePathCheck()
        {
            /*
            byte[] buf = new byte[500];
            int size = 0;
            return NativeLib?.TryGetExePath(buf, ref size) ?? false;
            */
            return true;
        }

        private static bool SteamLibraryCheck()
            => !string.IsNullOrEmpty(SteamManifestReader.GetInstallPathFromAppId("1573010"));

        public static void Quit() => hapticPlayer?.Dispose();

        public static bool IsPlaying() => hapticPlayer?.IsPlaying() ?? false;
        public static bool IsPlaying(string key) => hapticPlayer?.IsPlaying(key) ?? false;
        public static bool IsDeviceConnected(PositionType type) => hapticPlayer?.IsActive(type) ?? false;

        //public static void RegisterFeedback(string key, string tactFileStr) => hapticPlayer?.Register(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr));
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr) => hapticPlayer?.RegisterTactFileStr(key, tactFileStr);
        //public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr) => hapticPlayer?.RegisterTactFileStrReflected(key, tactFileStr);

        public static void SubmitRegistered(string key) => hapticPlayer?.SubmitRegistered(key);
        public static void SubmitRegistered(string key, int startTimeMillis) => hapticPlayer?.SubmitRegistered(key, startTimeMillis);
        public static void SubmitRegistered(string key, string altKey, ScaleOption option) => hapticPlayer?.SubmitRegistered(key, altKey, option);
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption) => hapticPlayer?.SubmitRegisteredVestRotation(key, altKey, rOption, sOption);

        public static void TurnOff() => hapticPlayer?.TurnOff();
        public static void TurnOff(string key) => hapticPlayer?.TurnOff(key);

        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis) => hapticPlayer?.Submit(key, position, bytes, durationMillis);
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis) => hapticPlayer?.Submit(key, position, points, durationMillis);
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis) => hapticPlayer?.Submit(key, position, points, durationMillis);

        public static string PositionTypeToOscAddress(PositionType positionType)
        {
            switch (positionType)
            {
                // Head
                case PositionType.Head:
                    return "/head";

                // Vest
                case PositionType.Vest:
                    return "/vest";
                case PositionType.VestFront:
                    return "/vest/front";
                case PositionType.VestBack:
                    return "/vest/back";

                // Arms
                case PositionType.ForearmL:
                    return "/arm/left";
                case PositionType.ForearmR:
                    return "/arm/right";

                // Hands
                case PositionType.HandL:
                    return "/hand/left";
                case PositionType.HandR:
                    return "/hand/right";

                // Feet
                case PositionType.FootL:
                    return "/foot/left";
                case PositionType.FootR:
                    return "/foot/right";

                // Unknown
                default:
                    return "/unknown";
            }
        }
    }
}