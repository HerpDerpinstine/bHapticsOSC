using System;
using System.Runtime.InteropServices;

namespace OscLib.Utils
{
    public static class Extensions
    {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }
        public static Int16 Clamp(this Int16 value, Int16 min, Int16 max)
            => Clamp<Int16>(value, min, max);
        public static UInt16 Clamp(this UInt16 value, UInt16 min, UInt16 max)
            => Clamp<UInt16>(value, min, max);
        public static Int32 Clamp(this Int32 value, Int32 min, Int32 max)
            => Clamp<Int32>(value, min, max);
        public static UInt32 Clamp(this UInt32 value, UInt32 min, UInt32 max)
            => Clamp<UInt32>(value, min, max);
        public static Double Clamp(this Double value, Double min, Double max)
            => Clamp<Double>(value, min, max);
        public static Single Clamp(this Single value, Single min, Single max)
            => Clamp<Single>(value, min, max);

        public static void GetDelegate<T>(this IntPtr ptr, out T output) where T : Delegate
                  => output = GetDelegate<T>(ptr);
        public static T GetDelegate<T>(this IntPtr ptr) where T : Delegate
            => GetDelegate(ptr, typeof(T)) as T;
        public static Delegate GetDelegate(this IntPtr ptr, Type type)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Delegate del = Marshal.GetDelegateForFunctionPointer(ptr, type);
            if (del == null)
                throw new Exception($"Unable to Get Delegate of Type {type.FullName} for Function Pointer!");
            return del;
        }
        public static IntPtr GetFunctionPointer(this Delegate del)
            => Marshal.GetFunctionPointerForDelegate(del);

        public static NativeLibrary ToNewNativeLibrary(this IntPtr ptr)
            => new NativeLibrary(ptr);
        public static NativeLibrary<T> ToNewNativeLibrary<T>(this IntPtr ptr)
            => new NativeLibrary<T>(ptr);
        public static IntPtr GetNativeLibraryExport(this IntPtr ptr, string name)
            => NativeLibrary.GetExport(ptr, name);
    }
}
