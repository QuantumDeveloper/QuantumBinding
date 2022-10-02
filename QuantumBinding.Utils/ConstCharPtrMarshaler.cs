using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public class ConstCharPtrMarshaler : ICustomMarshaler
{
    private static readonly ConstCharPtrMarshaler instance = new ConstCharPtrMarshaler();

    public static ICustomMarshaler GetInstance(string cookie)
    {
        return instance;
    }

    public object MarshalNativeToManaged(IntPtr pNativeData)
    {
        return Marshal.PtrToStringAnsi(pNativeData);
    }

    public IntPtr MarshalManagedToNative(object managedObj)
    {
        var str = (string)managedObj;
        return Marshal.StringToHGlobalAnsi(str);
    }

    public void CleanUpNativeData(IntPtr pNativeData)
    {
    }

    public void CleanUpManagedData(object managedObj)
    {
    }

    public int GetNativeDataSize()
    {
        return IntPtr.Size;
    }
}