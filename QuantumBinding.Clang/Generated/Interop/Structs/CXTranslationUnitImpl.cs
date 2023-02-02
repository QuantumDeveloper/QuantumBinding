
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXTranslationUnitImpl
{
    public void* pointer;
    public CXTranslationUnitImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



