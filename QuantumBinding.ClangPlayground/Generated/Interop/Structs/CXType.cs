
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// The type of an element in the abstract syntax tree.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXType
{
    public CXTypeKind kind;
    public Void0__FixedBuffer data;

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



