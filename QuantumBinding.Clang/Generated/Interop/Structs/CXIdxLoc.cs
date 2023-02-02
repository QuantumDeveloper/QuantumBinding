
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Source location passed to index callbacks.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxLoc
{
    public Void0__FixedBuffer ptr_data;
    public uint int_data;

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



