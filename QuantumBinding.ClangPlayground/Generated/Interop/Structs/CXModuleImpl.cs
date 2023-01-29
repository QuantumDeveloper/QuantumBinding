
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXModuleImpl
{
    public void* pointer;
    public CXModuleImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



