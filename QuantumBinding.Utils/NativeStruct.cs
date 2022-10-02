using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;
#if NET6_0_OR_GREATER
public unsafe struct NativeStruct<T> : IDisposable where T : unmanaged
{
    private readonly T* nativeStructPtr;

    public NativeStruct(T nativeStruct)
    {
        nativeStructPtr = (T*)NativeMemory.Alloc((nuint)sizeof(T));
        *nativeStructPtr = nativeStruct;
    }

    public T* GetRawPointer()
    {
        return nativeStructPtr;
    }
    

    public void Dispose()
    {
        NativeMemory.Free(nativeStructPtr);
    }
}
#endif