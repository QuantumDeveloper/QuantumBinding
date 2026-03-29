using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace QuantumBinding.Utils;

public static unsafe class NativeUtils
{
    public static uint StackAllocThreshold = 16;
    
    public static IReadOnlyList<T> PointerToManagedArray<T>(T* unmanagedArray, long arraySize) where T : unmanaged
    {
        var managedArray = new T[arraySize];
        for (int i = 0; i < managedArray.Length; i++)
        {
            managedArray[i] = unmanagedArray[i];
        }

        return managedArray;
    }

    public static void WritePointerToManagedArray<T>(T* unmanagedArray, T[] array) where T : unmanaged
    {
        if (unmanagedArray == null || array == null) return;
        
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = unmanagedArray[i];
        }
    }

    public static T* ManagedArrayToPointer<T>(IReadOnlyList<T> input) where T : unmanaged
    {
        if (input == null) return null;
        T* ptr = null;
        var size = input.Count * sizeof(T);
#if NET6_0_OR_GREATER
        ptr = (T*)NativeMemory.Alloc((nuint)input.Count, (nuint)sizeof(T));
#else
        var intPtr = Marshal.AllocHGlobal(size);
        ptr = (T*)intPtr.ToPointer();
#endif
        
        if (input is T[] array)
        {
            fixed (T* pSource = array)
            {
                Buffer.MemoryCopy(pSource, ptr, size, size);
            }
        }
        else if (input is List<T> list)
        {
#if NET6_0_OR_GREATER
            var span = CollectionsMarshal.AsSpan(list);
            fixed (T* pSource = span)
            {
                Buffer.MemoryCopy(pSource, ptr, size, size);
            }
#else
            var rentedArray = ArrayPool<T>.Shared.Rent(list.Count);
            try
            {
                list.CopyTo(rentedArray, 0);

                fixed (T* pSource = rentedArray)
                {
                    Buffer.MemoryCopy(pSource, ptr, size, size);
                }
            }
            finally
            {
                ArrayPool<T>.Shared.Return(rentedArray);
            }
#endif
        }
        
        
        return ptr;
    }

    public static T* GetPointerToManagedArray<T>(long arraySize) where T : unmanaged
    {
        if (arraySize <= 0) return null;
#if NET6_0_OR_GREATER
        return (T*)NativeMemory.Alloc((nuint)arraySize, (nuint)sizeof(T));
#else
        var intPtr = Marshal.AllocHGlobal((int)arraySize * Marshal.SizeOf<T>());
        return (T*)intPtr.ToPointer();
#endif
    }

    public static T* StructOrEnumToPointer<T>(T input) where T : unmanaged
    {
#if NET6_0_OR_GREATER
        var ptr = (T*)NativeMemory.Alloc((nuint)sizeof(T));
        *ptr = input;
        return ptr;
#else
        var intPtr = Marshal.AllocHGlobal(sizeof(T));
        var ptr = (T*)intPtr.ToPointer();
        *ptr = input;
        return ptr;
#endif
    }

    public static T* GetPointerForStructure<T>() where T : unmanaged
    {
#if NET6_0_OR_GREATER
        var ptr = (T*)NativeMemory.Alloc((nuint)sizeof(T));
        return ptr;
#else
        var intPtr = Marshal.AllocHGlobal(sizeof(T));
        return (T*)intPtr.ToPointer();
#endif
    }

    public static void* StringToPointer(string input, bool isUnicode)
    {
        void* ptr;
        var bytes = isUnicode ? Encoding.Unicode.GetBytes(input + '\0') : Encoding.ASCII.GetBytes(input + '\0');

#if NET6_0_OR_GREATER
        ptr = NativeMemory.Alloc((nuint)bytes.Length);
#else
        ptr = Marshal.AllocHGlobal(bytes.Length).ToPointer();
#endif
        fixed (byte* source = bytes)
        {
            System.Buffer.MemoryCopy(source, ptr, bytes.Length, bytes.Length);
        }

        return ptr;
    }
    
    public static void ExecuteWithUtf8String(string text, Action<IntPtr> action)
    {
#if NET6_0_OR_GREATER
        if (string.IsNullOrEmpty(text))
        {
            action(IntPtr.Zero);
            return;
        }

        int byteCount = Encoding.UTF8.GetByteCount(text) + 1;

        if (byteCount > 1024)
        {
            throw new ArgumentException("String is too long", nameof(text));
        }

        Span<byte> utf8Bytes = stackalloc byte[byteCount];

        Encoding.UTF8.GetBytes(text, utf8Bytes);

        utf8Bytes[byteCount - 1] = 0;

        fixed (byte* pUtf8Bytes = utf8Bytes)
        {
            action((IntPtr)pUtf8Bytes);
        }
#endif
    }

    public static void* GetPointerToStringArray(uint arrayLength, bool isUnicode)
    {
        void* stringPtr;
#if NET6_0_OR_GREATER
        if (isUnicode)
        {
            stringPtr = NativeMemory.Alloc((nuint)(sizeof(char*) * arrayLength));
        }
        else
        {
            stringPtr = NativeMemory.Alloc((nuint)(sizeof(sbyte*) * arrayLength));
        }
#else
        if (isUnicode)
        {
            stringPtr = Marshal.AllocHGlobal(sizeof(char*) * (int)arrayLength).ToPointer();
        }
        else
        {
            stringPtr = Marshal.AllocHGlobal(sizeof(sbyte*) * (int)arrayLength).ToPointer();
        }
#endif
        return stringPtr;
    }

    public static void* StringArrayToPointer(IReadOnlyList<string> input, bool isUnicode)
    {
        if (isUnicode)
        {
            var ptr = (char**)GetPointerToStringArray((uint)input.Count, isUnicode); 
            for (int i = 0; i < input.Count; i++)
            {
                ptr[i] = (char*)StringToPointer(input[i], isUnicode);
            }

            return ptr;
        }
        else
        {
            var ptr = (sbyte**)GetPointerToStringArray((uint)input.Count, isUnicode); 
            for (int i = 0; i < input.Count; i++)
            {
                ptr[i] = (sbyte*)StringToPointer(input[i], isUnicode);
            }

            return ptr;
        }
    }

    public static string[] PointerToStringArray(sbyte** input, uint size)
    {
        if (input == null) return null;
        
        var array = new string[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = new string(input[i]);
        }

        return array;
    }
    
    public static string[] PointerToStringArray(char** input, uint size)
    {
        if (input == null) return null;
        
        var array = new string[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = new string(input[i]);
        }

        return array;
    }

    public static void StringToFixedArray(void* destination, int destinationSize, string input, bool isUnicode)
    {
        var bytes = isUnicode ? Encoding.Unicode.GetBytes(input + '\0') : Encoding.ASCII.GetBytes(input + '\0');

        fixed (byte* source = bytes)
        {
            System.Buffer.MemoryCopy(source, destination, destinationSize, bytes.Length);
        }
    }

    public static void PrimitiveToFixedArray<T>(T* destination, int destinationSize, IReadOnlyList<T> input) where T : unmanaged
    {
        if (input == null || input.Count == 0)
        {
            return;
        }

        long sourceSizeInBytes = (long)input.Count * sizeof(T);
        long destinationsSizeInBytes = destinationSize * sizeof(T);
        long bytesToCopy = Math.Min(destinationsSizeInBytes, sourceSizeInBytes);

        if (input is T[] array)
        {
            fixed (T* pSource = array)
            {
                Buffer.MemoryCopy(pSource, destination, destinationsSizeInBytes, bytesToCopy);
            }
            return;
        }

        if (input is List<T> list)
        {
#if NET6_0_OR_GREATER
            var span = CollectionsMarshal.AsSpan(list);
            fixed (T* pSource = span)
            {
                Buffer.MemoryCopy(pSource, destination, destinationsSizeInBytes, bytesToCopy);
            }
#else
            var rentedArray = ArrayPool<T>.Shared.Rent(list.Count);
            try
            {
                list.CopyTo(rentedArray, 0);

                fixed (T* pSource = rentedArray)
                {
                    Buffer.MemoryCopy(pSource, destination, destinationsSizeInBytes, bytesToCopy);
                }
            }
            finally
            {
                ArrayPool<T>.Shared.Return(rentedArray);
            }
#endif
            return;
        }

    }

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