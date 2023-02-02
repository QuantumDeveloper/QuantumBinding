
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXClientDataImpl
{
    public void* pointer;
    public CXClientDataImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



