
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXPrintingPolicyImpl
{
    public void* pointer;
    public CXPrintingPolicyImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



