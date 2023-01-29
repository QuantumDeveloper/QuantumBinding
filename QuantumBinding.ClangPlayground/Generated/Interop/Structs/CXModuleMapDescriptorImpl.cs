
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXModuleMapDescriptorImpl
{
    public void* pointer;
    public CXModuleMapDescriptorImpl(void* pointer)
    {
        this.pointer = pointer;
    }

}



