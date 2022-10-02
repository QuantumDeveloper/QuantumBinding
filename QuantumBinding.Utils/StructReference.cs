using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public class StructReference : QBDisposableObject
{
    bool isInitialized;
    IntPtr reference;

    public IntPtr Handle => reference;

    public StructReference(object obj)
    {
        if (obj != null)
        {
            isInitialized = true;
            reference = MarshalUtils.MarshalStructToPtr(obj);
        }
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (isInitialized)
        {
            Marshal.FreeHGlobal(reference);
        }
    }
}