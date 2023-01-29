
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXEvalResultImpl
{
    public void* pointer;
    public CXEvalResultImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



