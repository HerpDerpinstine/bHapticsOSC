using System;
using System.Runtime.InteropServices;

namespace bHapticsOSC.Utils
{
    internal static class Extensions
    {
        internal static void GetDelegate<T>(this IntPtr ptr, out T output) where T : Delegate
               => output = GetDelegate<T>(ptr);
        internal static T GetDelegate<T>(this IntPtr ptr) where T : Delegate
            => GetDelegate(ptr, typeof(T)) as T;
        internal static Delegate GetDelegate(this IntPtr ptr, Type type)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Delegate del = Marshal.GetDelegateForFunctionPointer(ptr, type);
            if (del == null)
                throw new Exception($"Unable to Get Delegate of Type {type.FullName} for Function Pointer!");
            return del;
        }
        internal static IntPtr GetFunctionPointer(this Delegate del)
            => Marshal.GetFunctionPointerForDelegate(del);

        internal static NativeLibrary ToNewNativeLibrary(this IntPtr ptr)
            => new NativeLibrary(ptr);
        internal static NativeLibrary<T> ToNewNativeLibrary<T>(this IntPtr ptr)
            => new NativeLibrary<T>(ptr);
        internal static IntPtr GetNativeLibraryExport(this IntPtr ptr, string name)
            => NativeLibrary.GetExport(ptr, name);

        internal static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }
        internal static int Clamp(this int value, int min, int max)
            => Clamp<int>(value, min, max);
        internal static float Clamp(this float value, float min, float max)
            => Clamp<float>(value, min, max);
    }
}
