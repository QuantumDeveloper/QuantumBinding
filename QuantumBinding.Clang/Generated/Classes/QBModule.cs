
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// The functions in this group provide access to information about modules.
///</summary>
public unsafe partial class QBModule
{
    internal CXModuleImpl __Instance;
    public QBModule()
    {
    }

    public QBModule(QuantumBinding.Clang.Interop.CXModuleImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Returns the module file where the provided module object came from.
    ///</summary>
    public QBFile Module_getASTFile()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_getASTFile(this);
    }

    ///<summary>
    /// Returns the full name of the module, e.g. "std.vector".
    ///</summary>
    public QBString Module_getFullName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_getFullName(this);
    }

    ///<summary>
    /// Returns the name of the module, e.g. for the 'std.vector' sub-module it will return "vector".
    ///</summary>
    public QBString Module_getName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_getName(this);
    }

    ///<summary>
    /// Returns the parent of a sub-module or NULL if the given module is top-level, e.g. for 'std.vector' it will return the 'std' module.
    ///</summary>
    public QBModule Module_getParent()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_getParent(this);
    }

    ///<summary>
    /// Returns non-zero if the module is a system one.
    ///</summary>
    public int Module_isSystem()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_isSystem(this);
    }

    public ref readonly CXModuleImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXModuleImpl(QBModule q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXModuleImpl();
    }

    public static implicit operator QBModule(QuantumBinding.Clang.Interop.CXModuleImpl q)
    {
        return new QBModule(q);
    }

}


