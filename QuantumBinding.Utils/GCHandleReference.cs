using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public class GCHandleReference : QBDisposableObject
{
    GCHandle reference;

    public IntPtr Handle
    {
        get
        {
            if (reference.IsAllocated)
            {
                return reference.AddrOfPinnedObject();
            }
            return IntPtr.Zero;
        }
    }

    public GCHandleReference(object obj)
    {
        if (obj != null)
        {
            reference = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (reference.IsAllocated)
        {
            reference.Free();
        }
    }
}