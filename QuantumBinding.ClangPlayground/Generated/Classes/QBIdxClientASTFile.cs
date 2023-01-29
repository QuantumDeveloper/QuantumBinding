
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// The client's data object that is associated with an AST file (PCH or module).
///</summary>
public unsafe partial class QBIdxClientASTFile
{
    internal CXIdxClientASTFileImpl __Instance;
    public QBIdxClientASTFile()
    {
    }

    public QBIdxClientASTFile(QuantumBinding.Clang.Interop.CXIdxClientASTFileImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    public ref readonly CXIdxClientASTFileImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXIdxClientASTFileImpl(QBIdxClientASTFile q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXIdxClientASTFileImpl();
    }

    public static implicit operator QBIdxClientASTFile(QuantumBinding.Clang.Interop.CXIdxClientASTFileImpl q)
    {
        return new QBIdxClientASTFile(q);
    }

}


