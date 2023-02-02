
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// A parsed comment.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXComment
{
    public void* ASTNode;
    public CXTranslationUnitImpl TranslationUnit;
}



