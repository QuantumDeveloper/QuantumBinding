
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIndexImpl
{
    public void* pointer;
    public CXIndexImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



