using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public static class MarshalUtils
    {
        public static void IntPtrToManagedArray2<T>(IntPtr unmanagedArray, T[] managedArray) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            for (int i = 0; i < managedArray.Length; i++)
            {
                IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                managedArray[i] = (T)Activator.CreateInstance(typeof(T), ins);
            }
        }

        public static void IntPtrToManagedArray<T>(IntPtr unmanagedArray, T[] managedArray) where T: struct
        {
            var size = Marshal.SizeOf(typeof(T));
            for (int i = 0; i < managedArray.Length; i++)
            {
                IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                managedArray[i] = Marshal.PtrToStructure<T>(ins);
            }
        }

        public static string[] IntPtrToStringArray(IntPtr unmanagedArray, uint count, bool isUnicode = false)
        {
            var array = new string[count];
            for (int i = 0; i < count; ++i)
            {
                IntPtr strPtr = new IntPtr(unmanagedArray.ToInt64() + i * IntPtr.Size);
                if (isUnicode)
                {
                    array[i] = Marshal.PtrToStringUni(strPtr);
                }
                else
                {
                    array[i] = Marshal.PtrToStringAnsi(strPtr);
                }
            }
            return array;
        }

        public static IntPtr MarshalStructToPtr<T>(T @struct)
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(@struct));
            Marshal.StructureToPtr(@struct, ptr, false);
            return ptr;
        }
    }