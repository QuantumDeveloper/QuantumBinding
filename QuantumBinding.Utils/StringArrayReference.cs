using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public class StringArrayReference : QBDisposableObject
{
    IntPtr[] stringReferences;

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

    public StringArrayReference(in string[] strArray, bool isUnicode)
    {
        if (strArray != null && strArray.Length > 0)
        {
            stringReferences = new IntPtr[strArray.Length];
            int cnt = 0;
            foreach (var str in strArray)
            {
                if (!isUnicode)
                {
                    stringReferences[cnt++] = Marshal.StringToHGlobalAnsi(str);
                }
                else
                {
                    stringReferences[cnt++] = Marshal.StringToHGlobalUni(str);
                }
            }
            reference = GCHandle.Alloc(stringReferences, GCHandleType.Pinned);
        }
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (stringReferences != null)
        {
            foreach (var ptr in stringReferences)
            {
                Marshal.FreeHGlobal(ptr);
            }
            reference.Free();
        }
    }
}