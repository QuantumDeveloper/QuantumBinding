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
/// A single diagnostic, containing the diagnostic's severity, location, text, source ranges, and fix-it hints.
///</summary>
public unsafe partial class QBDiagnostic
{
    internal CXDiagnosticImpl __Instance;
    public QBDiagnostic()
    {
    }

    public QBDiagnostic(QuantumBinding.Clang.Interop.CXDiagnosticImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Destroy a diagnostic.
    ///</summary>
    public void DisposeDiagnostic()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeDiagnostic(this);
    }

    ///<summary>
    /// Format the given diagnostic in a manner that is suitable for display.
    ///</summary>
    public QBString FormatDiagnostic(uint options)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_formatDiagnostic(this, options);
    }

    ///<summary>
    /// Retrieve the child diagnostics of a CXDiagnostic.
    ///</summary>
    public QBDiagnosticSet GetChildDiagnostics()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getChildDiagnostics(this);
    }

    ///<summary>
    /// Retrieve the category number for this diagnostic.
    ///</summary>
    public uint GetDiagnosticCategory()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticCategory(this);
    }

    ///<summary>
    /// Retrieve the diagnostic category text for a given diagnostic.
    ///</summary>
    public QBString GetDiagnosticCategoryText()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticCategoryText(this);
    }

    ///<summary>
    /// Retrieve the replacement information for a given fix-it.
    ///</summary>
    public QBString GetDiagnosticFixIt(uint fixIt, QBSourceRange replacementRange)
    {
        var arg2 = ReferenceEquals(replacementRange, null) ? null : NativeUtils.StructOrEnumToPointer(replacementRange.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticFixIt(this, fixIt, arg2);
        NativeUtils.Free(arg2);
        return result;
    }

    ///<summary>
    /// Retrieve the source location of the given diagnostic.
    ///</summary>
    public QBSourceLocation GetDiagnosticLocation()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticLocation(this);
    }

    ///<summary>
    /// Determine the number of fix-it hints associated with the given diagnostic.
    ///</summary>
    public uint GetDiagnosticNumFixIts()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticNumFixIts(this);
    }

    ///<summary>
    /// Determine the number of source ranges associated with the given diagnostic.
    ///</summary>
    public uint GetDiagnosticNumRanges()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticNumRanges(this);
    }

    ///<summary>
    /// Retrieve the name of the command-line option that enabled this diagnostic.
    ///</summary>
    public QBString GetDiagnosticOption(QBString disable)
    {
        var arg1 = ReferenceEquals(disable, null) ? null : NativeUtils.StructOrEnumToPointer(disable.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticOption(this, arg1);
        disable?.Dispose();
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Retrieve a source range associated with the diagnostic.
    ///</summary>
    public QBSourceRange GetDiagnosticRange(uint range)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticRange(this, range);
    }

    ///<summary>
    /// Determine the severity of the given diagnostic.
    ///</summary>
    public CXDiagnosticSeverity GetDiagnosticSeverity()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticSeverity(this);
    }

    ///<summary>
    /// Retrieve the text of the given diagnostic.
    ///</summary>
    public QBString GetDiagnosticSpelling()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticSpelling(this);
    }

    public ref readonly CXDiagnosticImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXDiagnosticImpl(QBDiagnostic q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXDiagnosticImpl();
    }

    public static implicit operator QBDiagnostic(QuantumBinding.Clang.Interop.CXDiagnosticImpl q)
    {
        return new QBDiagnostic(q);
    }

}



