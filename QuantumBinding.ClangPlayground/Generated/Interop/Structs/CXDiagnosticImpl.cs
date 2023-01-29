
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXDiagnosticImpl
{
    public void* pointer;
    public CXDiagnosticImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



