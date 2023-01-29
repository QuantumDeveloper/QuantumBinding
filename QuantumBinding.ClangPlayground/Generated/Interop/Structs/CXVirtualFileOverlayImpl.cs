
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXVirtualFileOverlayImpl
{
    public void* pointer;
    public CXVirtualFileOverlayImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



