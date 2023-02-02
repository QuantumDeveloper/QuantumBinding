
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

[Flags]
public enum CXIndexOptFlags : uint
{
    ///<summary>
    /// Used to indicate that no special indexing options are needed.
    ///</summary>
    CXIndexOpt_None = 0,

    ///<summary>
    /// Used to indicate that IndexerCallbacks#indexEntityReference should be invoked for only one reference of an entity per source file that does not also include a declaration/definition of the entity.
    ///</summary>
    CXIndexOpt_SuppressRedundantRefs = 1,

    ///<summary>
    /// Function-local symbols should be indexed. If this is not set function-local symbols will be ignored.
    ///</summary>
    CXIndexOpt_IndexFunctionLocalSymbols = 2,

    ///<summary>
    /// Implicit function/class template instantiations should be indexed. If this is not set, implicit instantiations will be ignored.
    ///</summary>
    CXIndexOpt_IndexImplicitTemplateInstantiations = 4,

    ///<summary>
    /// Suppress all compiler warnings when parsing for indexing.
    ///</summary>
    CXIndexOpt_SuppressWarnings = 8,

    ///<summary>
    /// Skip a function/method body that was already parsed during an indexing session associated with a CXIndexAction object. Bodies in system headers are always skipped.
    ///</summary>
    CXIndexOpt_SkipParsedBodiesInSession = 16,

}



