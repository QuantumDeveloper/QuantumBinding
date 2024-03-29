// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBSourceRange
{
    public QBSourceRange()
    {
    }

    public QBSourceRange(QuantumBinding.Clang.Interop.CXSourceRange _internal)
    {
        Ptr_data = new void*[2];
        for (int i = 0; i < 2; ++i)
        {
            Ptr_data[i] = _internal.ptr_data[i];
        }
        Begin_int_data = _internal.begin_int_data;
        End_int_data = _internal.end_int_data;
    }

    public void*[] Ptr_data { get; set; }
    public uint Begin_int_data { get; set; }
    public uint End_int_data { get; set; }
    ///<summary>
    /// Determine whether two ranges are equivalent.
    ///</summary>
    public uint EqualRanges(QBSourceRange range2)
    {
        var arg1 = ReferenceEquals(range2, null) ? new QuantumBinding.Clang.Interop.CXSourceRange() : range2.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_equalRanges(ToNative(), arg1);
        return result;
    }

    ///<summary>
    /// Retrieve a source location representing the last character within a source range.
    ///</summary>
    public QBSourceLocation GetRangeEnd()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getRangeEnd(ToNative());
    }

    ///<summary>
    /// Retrieve a source location representing the first character within a source range.
    ///</summary>
    public QBSourceLocation GetRangeStart()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getRangeStart(ToNative());
    }

    ///<summary>
    /// Returns non-zero if range is null.
    ///</summary>
    public int Range_isNull()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Range_isNull(ToNative());
    }


    public QuantumBinding.Clang.Interop.CXSourceRange ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXSourceRange();
        if(Ptr_data != null)
        {
            if (Ptr_data.Length > 2)
                throw new System.ArgumentOutOfRangeException(nameof(Ptr_data), "Array is out of bounds. Size should not be more than 2");

            for (int i = 0; i < 2; ++i)
            {
                _internal.ptr_data[i] = Ptr_data[i];
            }
        }
        _internal.begin_int_data = Begin_int_data;
        _internal.end_int_data = End_int_data;
        return _internal;
    }

    public static implicit operator QBSourceRange(QuantumBinding.Clang.Interop.CXSourceRange q)
    {
        return new QBSourceRange(q);
    }

}



