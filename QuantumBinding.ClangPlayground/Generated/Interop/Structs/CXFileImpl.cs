
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXFileImpl
{
    public void* pointer;
    public CXFileImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



