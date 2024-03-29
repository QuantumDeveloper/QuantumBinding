using System;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Parser
{
    internal sealed unsafe class ForwardDeclarationVisitor : ICXCursorVisitor
    {
        private readonly QBCursor beginningCursor;

        private bool beginningCursorReached;
        private Delegates.CXCursorVisitor nativeVisitor;
        
        public ForwardDeclarationVisitor(QBCursor beginningCursor)
        {
            this.beginningCursor = beginningCursor;
            nativeVisitor = VisitDelegate;
            VisitorPtr = Marshal.GetFunctionPointerForDelegate(nativeVisitor);
        }
        
        public QBCursor ForwardDeclarationCursor { get; private set; }

        public IntPtr VisitorPtr { get; }

        public CXChildVisitResult VisitDelegate(CXCursor cursor, CXCursor parent, CXClientDataImpl data)
        {
            return Visit(cursor, parent, data);
        }

        public CXChildVisitResult Visit(QBCursor cursor, QBCursor parent, QBClientData client_data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            if (cursor.EqualCursors(beginningCursor) != 0)
            {
                beginningCursorReached = true;
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            if (beginningCursorReached)
            {
                ForwardDeclarationCursor = cursor;
                return CXChildVisitResult.CXChildVisit_Break;
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}