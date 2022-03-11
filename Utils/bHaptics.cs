using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace bHapticsOSC.Utils
{
    internal static class bHaptics
    {
        private static NativeExports NativeLib = null;
        internal const int MaxBufferSize = 20;

        internal static void Load()
        {
            Console.WriteLine("Loading bHaptics Native Library...");

            string filename = "bHaptics.x" + (Environment.Is64BitOperatingSystem ? "64" : "86") + ".dll";
            string filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "NativeLibs", filename);
            if (!File.Exists(filepath))
                throw new FileNotFoundException($"Failed to find the bHaptics Native Library in the NativeLibs folder! {filepath}", filename);

            NativeLib = NativeLibrary.ReflectiveLoad<NativeExports>(filepath);

            if (!ExePathCheck()
                && !SteamLibraryCheck())
                throw new Exception("bHaptics Player is Not Installed!");

            NativeLib?.Initialise(Marshal.StringToHGlobalAnsi("bHapticsOSC"), Marshal.StringToHGlobalAnsi("bHapticsOSC"));
        }

        private static bool ExePathCheck()
        {
            byte[] buf = new byte[500];
            int size = 0;
            return NativeLib?.TryGetExePath(buf, ref size) ?? false;
        }

        private static bool SteamLibraryCheck()
            => !string.IsNullOrEmpty(SteamManifestReader.GetInstallPathFromAppId("1573010"));

        internal static void Quit()
        {
            NativeLib?.TurnOff();
            NativeLib?.Destroy();
        }

        internal static bool IsPlaying() => NativeLib?.IsPlaying() ?? false;
        internal static bool IsPlaying(string key) => NativeLib?.IsPlayingKey(Marshal.StringToHGlobalAnsi(key)) ?? false;
        internal static bool IsDeviceConnected(PositionType type) => NativeLib?.IsDevicePlaying(type) ?? false;
        internal static bool IsDeviceConnected(DeviceType type, bool isLeft = true) => NativeLib?.IsDevicePlaying(DeviceTypeToPositionType(type, isLeft)) ?? false;
        internal static bool IsFeedbackRegistered(string key) => NativeLib?.IsFeedbackRegistered(Marshal.StringToHGlobalAnsi(key)) ?? false;

        internal static void RegisterFeedback(string key, string tactFileStr) => NativeLib?.RegisterFeedback(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr));
        internal static void RegisterFeedbackFromTactFile(string key, string tactFileStr) => NativeLib?.RegisterFeedbackFromTactFile(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr));
        internal static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr) => NativeLib?.RegisterFeedbackFromTactFileReflected(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(tactFileStr));

        internal static void SubmitRegistered(string key) => NativeLib?.SubmitRegistered(Marshal.StringToHGlobalAnsi(key));
        internal static void SubmitRegistered(string key, int startTimeMillis) => NativeLib?.SubmitRegisteredStartMillis(Marshal.StringToHGlobalAnsi(key), startTimeMillis);
        internal static void SubmitRegistered(string key, string altKey, ScaleOption option) => NativeLib?.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), option.Intensity, option.Duration, 1f, 1f);
        internal static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption) => NativeLib?.SubmitRegisteredWithOption(Marshal.StringToHGlobalAnsi(key), Marshal.StringToHGlobalAnsi(altKey), sOption.Intensity, sOption.Duration, rOption.OffsetX, rOption.OffsetY);

        internal static void TurnOff() => NativeLib?.TurnOff();
        internal static void TurnOff(string key) => NativeLib?.TurnOffKey(Marshal.StringToHGlobalAnsi(key));

        internal static void Submit(string key, DeviceType type, bool isLeft, byte[] bytes, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), bytes, durationMillis);
        internal static void Submit(string key, PositionType position, byte[] bytes, int durationMillis)
        {
            int bytes_size = bytes.Length;
            if (bytes_size != MaxBufferSize)
            {
                byte[] newbytes = new byte[MaxBufferSize];
                for (int i = 0; i < bytes_size; i++)
                    newbytes[i] = bytes[i];
            }
            NativeLib?.SubmitByteArray(Marshal.StringToHGlobalAnsi(key), position, bytes, MaxBufferSize, durationMillis);
        }
        internal static void Submit(string key, DeviceType type, bool isLeft, List<DotPoint> points, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        internal static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            byte[] bytes = new byte[MaxBufferSize];
            for (var i = 0; i < points.Count; i++)
            {
                DotPoint point = points[i];
                if ((point.Index < 0) || (point.Index > MaxBufferSize))
                    continue;
                bytes[point.Index] = (byte)point.Intensity;
            }
            NativeLib?.SubmitByteArray(Marshal.StringToHGlobalAnsi(key), position, bytes, MaxBufferSize, durationMillis);
        }
        internal static void Submit(string key, DeviceType type, bool isLeft, List<PathPoint> points, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        internal static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            PathPoint[] pathPoints = points.ToArray();
            NativeLib?.SubmitPathArray(Marshal.StringToHGlobalAnsi(key), position, pathPoints, pathPoints.Length, durationMillis);
        }

        internal static FeedbackStatus GetCurrentFeedbackStatus(DeviceType type, bool isLeft = true) => GetCurrentFeedbackStatus(DeviceTypeToPositionType(type, isLeft));
        internal static FeedbackStatus GetCurrentFeedbackStatus(PositionType pos)
        {
            FeedbackStatus status = default;
            NativeLib?.TryGetResponseForPosition(pos, out status);
            return status;
        }

        internal static PositionType DeviceTypeToPositionType(DeviceType pos, bool isLeft = true)
        {
            switch (pos)
            {
                case DeviceType.Tactal:
                    return PositionType.Head;
                case DeviceType.TactSuit:
                    return PositionType.Vest;
                case DeviceType.Tactosy_arms:
                    return isLeft ? PositionType.ForearmL : PositionType.ForearmR;
                case DeviceType.Tactosy_feet:
                    return isLeft ? PositionType.FootL : PositionType.FootR;
                case DeviceType.Tactosy_hands:
                    return isLeft ? PositionType.HandL : PositionType.HandR;
                case DeviceType.None:
                    break;
            }
            return PositionType.Head;
        }

        internal enum DeviceType
        {
            None = 0,
            Tactal = 1,
            TactSuit = 2,
            Tactosy_arms = 3,
            Tactosy_hands = 4,
            Tactosy_feet = 5
        }

        internal enum PositionType
        {
            All = 0,
            Left = 1, Right = 2,
            Vest = 3,
            Head = 4,
            Racket = 5,
            HandL = 6, HandR = 7,
            FootL = 8, FootR = 9,
            ForearmL = 10, ForearmR = 11,
            VestFront = 201, VestBack = 202,
            GloveLeft = 203, GloveRight = 204,
            Custom1 = 251, Custom2 = 252, Custom3 = 253, Custom4 = 254
        }

        internal class RotationOption
        {
            internal RotationOption(float offsetX, float offsetY)
            {
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
            internal float OffsetX, OffsetY;
            public override string ToString() => "RotationOption { OffsetX=" + OffsetX.ToString() +
                       ", OffsetY=" + OffsetY.ToString() + " }";
        }

        internal class ScaleOption
        {
            internal ScaleOption(float intensity = 1f, float duration = 1f)
            {
                Intensity = intensity;
                Duration = duration;
            }
            internal float Intensity, Duration;
            public override string ToString() => "ScaleOption { Intensity=" + Intensity.ToString() +
                       ", Duration=" + Duration.ToString() + " }";
        }

        internal class DotPoint
        {
            internal DotPoint(int index, int intensity = 50)
            {
                if ((index < 0) || (index > MaxBufferSize))
                    throw new Exception("Invalid argument index : " + index);
                Intensity = intensity.Clamp(0, 100);
                Index = index;
            }
            internal int Index, Intensity;
            public override string ToString() => "DotPoint { Index=" + Index.ToString() +
                       ", Intensity=" + Intensity.ToString() + " }";
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PathPoint
        {
            internal PathPoint(float x, float y, int intensity = 50, int motorCount = 3)
            {
                X = x.Clamp(0f, 1f);
                Y = y.Clamp(0f, 1f);
                Intensity = intensity.Clamp(0, 100);
                MotorCount = motorCount.Clamp(0, 3);
            }
            internal float X, Y;
            internal int Intensity;

            // Number of maximum motors to vibrate
            // if 0 means default motor count, now 3
            internal int MotorCount;

            public override string ToString() => "PathPoint { X=" + X.ToString() +
                       ", Y=" + Y.ToString() +
                       ", MotorCount=" + MotorCount.ToString() +
                       ", Intensity=" + Intensity.ToString() + " }";
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FeedbackStatus
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            internal int[] values;
        };

        private class NativeExports
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dInitialise(IntPtr appId, IntPtr appName);
            internal dInitialise Initialise = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dVoid();
            internal dVoid TurnOff = null;
            internal dVoid Destroy = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dRegisterFeedback(IntPtr key, IntPtr tactFileStr);
            internal dRegisterFeedback RegisterFeedback = null;
            internal dRegisterFeedback RegisterFeedbackFromTactFile = null;
            internal dRegisterFeedback RegisterFeedbackFromTactFileReflected = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitRegistered(IntPtr key);
            internal dSubmitRegistered SubmitRegistered = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitRegisteredStartMillis(IntPtr key, int startTimeMillis);
            internal dSubmitRegisteredStartMillis SubmitRegisteredStartMillis = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitRegisteredWithOption(IntPtr key, IntPtr altKey, float intensity, float duration, float offsetX, float offsetY);
            internal dSubmitRegisteredWithOption SubmitRegisteredWithOption = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitByteArray(IntPtr key, PositionType pos, byte[] bytes, int length, int durationMillis);
            internal dSubmitByteArray SubmitByteArray = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitPathArray(IntPtr key, PositionType pos, PathPoint[] points, int length, int durationMillis);
            internal dSubmitPathArray SubmitPathArray = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsFeedbackRegistered(IntPtr key);
            internal dIsFeedbackRegistered IsFeedbackRegistered = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsPlaying();
            internal dIsPlaying IsPlaying = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsPlayingKey(IntPtr key);
            internal dIsPlayingKey IsPlayingKey = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dTurnOffKey(IntPtr key);
            internal dTurnOffKey TurnOffKey = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsDevicePlaying(PositionType pos);
            internal dIsDevicePlaying IsDevicePlaying = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dTryGetResponseForPosition(PositionType pos, out FeedbackStatus status);
            internal dTryGetResponseForPosition TryGetResponseForPosition = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dTryGetExePath(byte[] buf, ref int size);
            internal dTryGetExePath TryGetExePath = null;
        }
    }
}