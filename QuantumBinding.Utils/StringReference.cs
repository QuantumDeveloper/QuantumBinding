using System;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public class StringReference : QBDisposableObject
{
    bool isInitialized;
    IntPtr reference;

    public IntPtr Handle => reference;

    public StringReference(string str, bool isUnicode)
    {
        if (string.IsNullOrEmpty(str))
        {
            return;
        }

        if (!isUnicode)
        {
            reference = Marshal.StringToHGlobalAnsi(str);
        }
        else
        {
            reference = Marshal.StringToHGlobalUni(str);
        }
        isInitialized = true;
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