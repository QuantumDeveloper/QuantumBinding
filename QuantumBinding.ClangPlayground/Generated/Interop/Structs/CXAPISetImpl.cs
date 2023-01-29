
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXAPISetImpl
{
    public void* pointer;
    public CXAPISetImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



