using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

/// <summary>Releases memory the bindings allocated natively. What else lived here belonged to the marshalling this
/// library no longer does - see <see cref="MarshalContextUtils"/> for the path the generator emits today.</summary>
public static unsafe class NativeUtils
{
    public static void Free(void* ptr)
    {
#if NET6_0_OR_GREATER
        NativeMemory.Free(ptr);
#else
        Marshal.FreeHGlobal((IntPtr)ptr);
#endif
    }

    public static void Free<T>(T* ptr) where T : unmanaged
    {
#if NET6_0_OR_GREATER
        NativeMemory.Free(ptr);
#else
        Marshal.FreeHGlobal((IntPtr)ptr);
#endif
    }

    public static void Free<T>(T** ptr) where T : unmanaged
    {
#if NET6_0_OR_GREATER
        NativeMemory.Free(ptr);
#else
        Marshal.FreeHGlobal((IntPtr)ptr);
#endif
    }

    public static void Free<T>(T*** ptr) where T : unmanaged
    {
#if NET6_0_OR_GREATER
        NativeMemory.Free(ptr);
#else
        Marshal.FreeHGlobal((IntPtr)ptr);
#endif
    }
}
