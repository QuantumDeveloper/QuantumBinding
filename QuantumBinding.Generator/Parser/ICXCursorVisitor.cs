using System;
using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Generator.Parser
{
    internal interface ICXCursorVisitor
    {
        CXChildVisitResult VisitDelegate(CXCursor cursor, CXCursor parent, CXClientDataImpl client_data);
    }
}