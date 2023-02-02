
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Identifies a half-open character range in the source code.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXSourceRange
{
    public Void0__FixedBuffer ptr_data;
    public uint begin_int_data;
    public uint end_int_data;

    [StructLayout(LayoutKind.Sequential)]
    public partial struct Void0__FixedBuffer
    {
        public nuint item0;
        public nuint item1;

        public void* this[int index]
        {
            get => (void*)Unsafe.Add(ref item0, index);
            set => Unsafe.Add(ref item0, index) = (nuint)value;
        }
    }
}


