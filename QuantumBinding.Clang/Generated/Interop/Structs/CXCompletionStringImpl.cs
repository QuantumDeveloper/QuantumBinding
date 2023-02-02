
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXCompletionStringImpl
{
    public void* pointer;
    public CXCompletionStringImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



