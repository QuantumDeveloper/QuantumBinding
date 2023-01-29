
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Bits that represent the context under which completion is occurring.
///</summary>
public enum CXCompletionContext : uint
{
    ///<summary>
    /// The context for completions is unexposed, as only Clang results should be included. (This is equivalent to having no context bits set.)
    ///</summary>
    CXCompletionContext_Unexposed = 0,

    ///<summary>
    /// Completions for any possible type should be included in the results.
    ///</summary>
    CXCompletionContext_AnyType = 1,

    ///<summary>
    /// Completions for any possible value (variables, function calls, etc.) should be included in the results.
    ///</summary>
    CXCompletionContext_AnyValue = 2,

    ///<summary>
    /// Completions for values that resolve to an Objective-C object should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCObjectValue = 4,

    ///<summary>
    /// Completions for values that resolve to an Objective-C selector should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCSelectorValue = 8,

    ///<summary>
    /// Completions for values that resolve to a C++ class type should be included in the results.
    ///</summary>
    CXCompletionContext_CXXClassTypeValue = 16,

    ///<summary>
    /// Completions for fields of the member being accessed using the dot operator should be included in the results.
    ///</summary>
    CXCompletionContext_DotMemberAccess = 32,

    ///<summary>
    /// Completions for fields of the member being accessed using the arrow operator should be included in the results.
    ///</summary>
    CXCompletionContext_ArrowMemberAccess = 64,

    ///<summary>
    /// Completions for properties of the Objective-C object being accessed using the dot operator should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCPropertyAccess = 128,

    ///<summary>
    /// Completions for enum tags should be included in the results.
    ///</summary>
    CXCompletionContext_EnumTag = 256,

    ///<summary>
    /// Completions for union tags should be included in the results.
    ///</summary>
    CXCompletionContext_UnionTag = 512,

    ///<summary>
    /// Completions for struct tags should be included in the results.
    ///</summary>
    CXCompletionContext_StructTag = 1024,

    ///<summary>
    /// Completions for C++ class names should be included in the results.
    ///</summary>
    CXCompletionContext_ClassTag = 2048,

    ///<summary>
    /// Completions for C++ namespaces and namespace aliases should be included in the results.
    ///</summary>
    CXCompletionContext_Namespace = 4096,

    ///<summary>
    /// Completions for C++ nested name specifiers should be included in the results.
    ///</summary>
    CXCompletionContext_NestedNameSpecifier = 8192,

    ///<summary>
    /// Completions for Objective-C interfaces (classes) should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCInterface = 16384,

    ///<summary>
    /// Completions for Objective-C protocols should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCProtocol = 32768,

    ///<summary>
    /// Completions for Objective-C categories should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCCategory = 65536,

    ///<summary>
    /// Completions for Objective-C instance messages should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCInstanceMessage = 131072,

    ///<summary>
    /// Completions for Objective-C class messages should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCClassMessage = 262144,

    ///<summary>
    /// Completions for Objective-C selector names should be included in the results.
    ///</summary>
    CXCompletionContext_ObjCSelectorName = 524288,

    ///<summary>
    /// Completions for preprocessor macro names should be included in the results.
    ///</summary>
    CXCompletionContext_MacroName = 1048576,

    ///<summary>
    /// Natural language completions should be included in the results.
    ///</summary>
    CXCompletionContext_NaturalLanguage = 2097152,

    ///<summary>
    /// #include file completions should be included in the results.
    ///</summary>
    CXCompletionContext_IncludedFile = 4194304,

    ///<summary>
    /// The current context is unknown, so set all contexts.
    ///</summary>
    CXCompletionContext_Unknown = 8388607,

}



