
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxClientASTFileImpl
{
    public void* pointer;
    public CXIdxClientASTFileImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



