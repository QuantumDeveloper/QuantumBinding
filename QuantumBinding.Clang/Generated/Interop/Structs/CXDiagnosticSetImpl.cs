
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXDiagnosticSetImpl
{
    public void* pointer;
    public CXDiagnosticSetImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



