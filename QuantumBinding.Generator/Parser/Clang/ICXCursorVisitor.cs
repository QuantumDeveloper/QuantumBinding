using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Generator.Parser.Clang;

internal interface ICXCursorVisitor
{
    CXChildVisitResult VisitDelegate(CXCursor cursor, CXCursor parent, CXClientData client_data);
}