// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// The client's data object that is associated with a CXFile.
///</summary>
public unsafe partial class QBIdxClientFile
{
    internal CXIdxClientFileImpl __Instance;
    public QBIdxClientFile()
    {
    }

    public QBIdxClientFile(QuantumBinding.Clang.Interop.CXIdxClientFileImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    public ref readonly CXIdxClientFileImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXIdxClientFileImpl(QBIdxClientFile q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXIdxClientFileImpl();
    }

    public static implicit operator QBIdxClientFile(QuantumBinding.Clang.Interop.CXIdxClientFileImpl q)
    {
        return new QBIdxClientFile(q);
    }

}



