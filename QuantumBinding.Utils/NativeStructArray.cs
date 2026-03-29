using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

// https://stackoverflow.com/questions/42754123/conditional-per-targetframework-in-msbuild-15/42754213#42754213
public unsafe struct NativeStructArray<T> where T : unmanaged
{
    public readonly T* Handle;
    private bool isDisposed;

    public NativeStructArray(IReadOnlyList<T> input)
    {
        if (input == null)
        {
            Handle = null;
        }
        else
        {
            var size = (nuint)input.Count * (nuint)sizeof(T);
            Handle = NativeUtils.GetPointerToManagedArray<T>(input.Count);
            
            if (input is T[] array)
            {
                fixed (T* pSource = array)
                {
                    Buffer.MemoryCopy(pSource, Handle, size, size);
                }
            }
            else if (input is List<T> list)
            {
#if NET6_0_OR_GREATER
            var span = CollectionsMarshal.AsSpan(list);
            fixed (T* pSource = span)
            {
                Buffer.MemoryCopy(pSource, Handle, size, size);
            }
#else
                var rentedArray = ArrayPool<T>.Shared.Rent(list.Count);
                try
                {
                    list.CopyTo(rentedArray, 0);

                    fixed (T* pSource = rentedArray)
                    {
                        Buffer.MemoryCopy(pSource, Handle, size, size);
                    }
                }
                finally
                {
                    ArrayPool<T>.Shared.Return(rentedArray);
                }
#endif
            }
        }

        isDisposed = false;
    }

    public void Dispose()
    {
        if (!isDisposed)
        {
            NativeUtils.Free(Handle);
            isDisposed = true;
        }
    }
}
