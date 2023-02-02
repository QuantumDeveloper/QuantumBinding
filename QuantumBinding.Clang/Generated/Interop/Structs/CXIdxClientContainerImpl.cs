
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxClientContainerImpl
{
    public void* pointer;
    public CXIdxClientContainerImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



