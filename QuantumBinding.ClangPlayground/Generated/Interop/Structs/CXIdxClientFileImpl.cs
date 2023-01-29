
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxClientFileImpl
{
    public void* pointer;
    public CXIdxClientFileImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



