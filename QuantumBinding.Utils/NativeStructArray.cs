using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

// https://stackoverflow.com/questions/42754123/conditional-per-targetframework-in-msbuild-15/42754213#42754213
#if NET6_0_OR_GREATER
public unsafe struct NativeStructArray<T> : IDisposable where T : unmanaged
{
    public T* ptr;

    public NativeStructArray(T[] arr)
    {
        var size = (nuint)arr.Length * (nuint)sizeof(T);
        ptr = (T*)NativeMemory.Alloc((nuint)arr.Length, (nuint)sizeof(T));
        fixed (T* t = arr)
        {
            ptr = t;
            Buffer.MemoryCopy(t, ptr, (int)size, (int)size);
        }
    }
    
    public void Dispose()
    {
        NativeMemory.Free(ptr);
    }
}

#endif