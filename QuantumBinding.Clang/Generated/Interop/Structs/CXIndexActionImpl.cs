
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIndexActionImpl
{
    public void* pointer;
    public CXIndexActionImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



