
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXCursorSetImpl
{
    public void* pointer;
    public CXCursorSetImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



