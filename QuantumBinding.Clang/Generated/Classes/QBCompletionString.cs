
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// A semantic string that describes a code-completion result.
///</summary>
public unsafe partial class QBCompletionString
{
    internal CXCompletionStringImpl __Instance;
    public QBCompletionString()
    {
    }

    public QBCompletionString(QuantumBinding.Clang.Interop.CXCompletionStringImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Retrieve the annotation associated with the given completion string.
    ///</summary>
    public QBString getCompletionAnnotation(uint annotation_number)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionAnnotation(this, annotation_number);
    }

    ///<summary>
    /// Determine the availability of the entity that this code-completion string refers to.
    ///</summary>
    public CXAvailabilityKind getCompletionAvailability()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionAvailability(this);
    }

    ///<summary>
    /// Retrieve the brief documentation comment attached to the declaration that corresponds to the given completion string.
    ///</summary>
    public QBString getCompletionBriefComment()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionBriefComment(this);
    }

    ///<summary>
    /// Retrieve the completion string associated with a particular chunk within a completion string.
    ///</summary>
    public QBCompletionString getCompletionChunkCompletionString(uint chunk_number)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionChunkCompletionString(this, chunk_number);
    }

    ///<summary>
    /// Determine the kind of a particular chunk within a completion string.
    ///</summary>
    public CXCompletionChunkKind getCompletionChunkKind(uint chunk_number)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionChunkKind(this, chunk_number);
    }

    ///<summary>
    /// Retrieve the text associated with a particular chunk within a completion string.
    ///</summary>
    public QBString getCompletionChunkText(uint chunk_number)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionChunkText(this, chunk_number);
    }

    ///<summary>
    /// Retrieve the number of annotations associated with the given completion string.
    ///</summary>
    public uint getCompletionNumAnnotations()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionNumAnnotations(this);
    }

    ///<summary>
    /// Retrieve the parent context of the given completion string.
    ///</summary>
    public QBString getCompletionParent(ref CXCursorKind kind)
    {
        var arg1 = NativeUtils.StructOrEnumToPointer(kind);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionParent(this, arg1);
        if (arg1 is not null)
        {
            kind = *arg1;
        }
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Determine the priority of this code completion.
    ///</summary>
    public uint getCompletionPriority()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionPriority(this);
    }

    ///<summary>
    /// Retrieve the number of chunks in the given code-completion string.
    ///</summary>
    public uint getNumCompletionChunks()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNumCompletionChunks(this);
    }

    public ref readonly CXCompletionStringImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXCompletionStringImpl(QBCompletionString q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXCompletionStringImpl();
    }

    public static implicit operator QBCompletionString(QuantumBinding.Clang.Interop.CXCompletionStringImpl q)
    {
        return new QBCompletionString(q);
    }

}



