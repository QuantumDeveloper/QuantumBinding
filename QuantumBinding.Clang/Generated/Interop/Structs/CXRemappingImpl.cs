
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXRemappingImpl
{
    public void* pointer;
    public CXRemappingImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



