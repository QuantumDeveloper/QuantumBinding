
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXTargetInfoImpl
{
    public void* pointer;
    public CXTargetInfoImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



