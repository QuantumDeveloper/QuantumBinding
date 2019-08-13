using System;
using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Generator.Parser
{
    internal interface ICXCursorVisitor
    {
        CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, CXClientData client_data);
    }
}