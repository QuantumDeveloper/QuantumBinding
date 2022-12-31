using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace QuantumBinding.Utils;

public static unsafe class NativeUtils
{
    public static T[] PointerToManagedArray<T>(T* unmanagedArray, long arraySize) where T : unmanaged
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

    public static T* ManagedArrayToPointer<T>(T[] arr) where T : unmanaged
    {
        if (arr == null) return null;
#if NET6_0_OR_GREATER
        var size = (nuint)arr.Length * (nuint)sizeof(T);
        var ptr = (T*)NativeMemory.Alloc((nuint)arr.Length, (nuint)sizeof(T));
        fixed (T* t = arr)
        {
            System.Buffer.MemoryCopy(t, ptr, (int)size, (int)size);
        }
        return ptr;
#else
        var size = arr.Length * Marshal.SizeOf<T>();
        var intPtr = Marshal.AllocHGlobal(size);
        var ptr = (T*)intPtr.ToPointer();
        fixed (T* t = arr)
        {
            System.Buffer.MemoryCopy(t, ptr, size, size);
        }

        return ptr;
#endif
    }

    public static T* GetPointerToManagedArray<T>(long arraySize) where T : unmanaged
    {
        if (arraySize <= 0) return null;
#if NET6_0_OR_GREATER
        var ptr = (T*)NativeMemory.Alloc((nuint)arraySize, (nuint)sizeof(T));
        return ptr;
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

    public static void* PointerToString(string input, bool isUnicode)
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

    public static void* StringArrayToPointer(string[] input, bool isUnicode)
    {
        if (isUnicode)
        {
            var ptr = (char**)GetPointerToStringArray((uint)input.Length, isUnicode); 
            for (int i = 0; i < input.Length; i++)
            {
                ptr[i] = (char*)PointerToString(input[i], isUnicode);
            }

            return ptr;
        }
        else
        {
            var ptr = (sbyte**)GetPointerToStringArray((uint)input.Length, isUnicode); 
            for (int i = 0; i < input.Length; i++)
            {
                ptr[i] = (sbyte*)PointerToString(input[i], isUnicode);
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

    public static void PrimitiveToFixedArray<T>(T* destination, int destinationSize, T[] input) where T : unmanaged
    {
        fixed (T* source = input)
        {
            System.Buffer.MemoryCopy(source, destination, destinationSize * sizeof(T), input.Length * sizeof(T));
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