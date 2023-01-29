
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxClientEntityImpl
{
    public void* pointer;
    public CXIdxClientEntityImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



