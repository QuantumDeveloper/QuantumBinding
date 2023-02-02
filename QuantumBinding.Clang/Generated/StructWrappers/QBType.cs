
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBType
{
    public QBType()
    {
    }

    public QBType(QuantumBinding.Clang.Interop.CXType _internal)
    {
        Kind = _internal.kind;
        Data = new void*[2];
        for (int i = 0; i < 2; ++i)
        {
            Data[i] = _internal.data[i];
        }
    }

    public CXTypeKind Kind { get; set; }
    public void*[] Data { get; set; }
    ///<summary>
    /// Determine whether two CXTypes represent the same type.
    ///</summary>
    public uint equalTypes(QBType B)
    {
        var arg1 = ReferenceEquals(B, null) ? new QuantumBinding.Clang.Interop.CXType() : B.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_equalTypes(ToNative(), arg1);
        return result;
    }

    ///<summary>
    /// Returns the address space of the given type.
    ///</summary>
    public uint getAddressSpace()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getAddressSpace(ToNative());
    }

    ///<summary>
    /// Retrieve the type of a parameter of a function type.
    ///</summary>
    public QBType getArgType(uint i)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getArgType(ToNative(), i);
    }

    ///<summary>
    /// Return the element type of an array type.
    ///</summary>
    public QBType getArrayElementType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getArrayElementType(ToNative());
    }

    ///<summary>
    /// Return the array size of a constant array.
    ///</summary>
    public long getArraySize()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getArraySize(ToNative());
    }

    ///<summary>
    /// Return the canonical type for a CXType.
    ///</summary>
    public QBType getCanonicalType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCanonicalType(ToNative());
    }

    ///<summary>
    /// Return the element type of an array, complex, or vector type.
    ///</summary>
    public QBType getElementType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getElementType(ToNative());
    }

    ///<summary>
    /// Retrieve the exception specification type associated with a function type. This is a value of type CXCursor_ExceptionSpecificationKind.
    ///</summary>
    public int getExceptionSpecificationType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getExceptionSpecificationType(ToNative());
    }

    ///<summary>
    /// Retrieve the calling convention associated with a function type.
    ///</summary>
    public CXCallingConv getFunctionTypeCallingConv()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getFunctionTypeCallingConv(ToNative());
    }

    ///<summary>
    /// For reference types (e.g., "const int&"), returns the type that the reference refers to (e.g "const int").
    ///</summary>
    public QBType getNonReferenceType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNonReferenceType(ToNative());
    }

    ///<summary>
    /// Retrieve the number of non-variadic parameters associated with a function type.
    ///</summary>
    public int getNumArgTypes()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNumArgTypes(ToNative());
    }

    ///<summary>
    /// Return the number of elements of an array or vector type.
    ///</summary>
    public long getNumElements()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNumElements(ToNative());
    }

    ///<summary>
    /// For pointer types, returns the type of the pointee.
    ///</summary>
    public QBType getPointeeType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getPointeeType(ToNative());
    }

    ///<summary>
    /// Retrieve the return type associated with a function type.
    ///</summary>
    public QBType getResultType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getResultType(ToNative());
    }

    ///<summary>
    /// Return the cursor for the declaration of the given type.
    ///</summary>
    public QBCursor getTypeDeclaration()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTypeDeclaration(ToNative());
    }

    ///<summary>
    /// Returns the typedef name of the given type.
    ///</summary>
    public QBString getTypedefName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTypedefName(ToNative());
    }

    ///<summary>
    /// Pretty-print the underlying type using the rules of the language of the translation unit from which it came.
    ///</summary>
    public QBString getTypeSpelling()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTypeSpelling(ToNative());
    }

    ///<summary>
    /// Retrieve the unqualified variant of the given type, removing as little sugar as possible.
    ///</summary>
    public QBType getUnqualifiedType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getUnqualifiedType(ToNative());
    }

    ///<summary>
    /// Determine whether a CXType has the "const" qualifier set, without looking through typedefs that may have added "const" at a different level.
    ///</summary>
    public uint isConstQualifiedType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isConstQualifiedType(ToNative());
    }

    ///<summary>
    /// Return 1 if the CXType is a variadic function type, and 0 otherwise.
    ///</summary>
    public uint isFunctionTypeVariadic()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isFunctionTypeVariadic(ToNative());
    }

    ///<summary>
    /// Return 1 if the CXType is a POD (plain old data) type, and 0 otherwise.
    ///</summary>
    public uint isPODType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isPODType(ToNative());
    }

    ///<summary>
    /// Determine whether a CXType has the "restrict" qualifier set, without looking through typedefs that may have added "restrict" at a different level.
    ///</summary>
    public uint isRestrictQualifiedType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isRestrictQualifiedType(ToNative());
    }

    ///<summary>
    /// Determine whether a CXType has the "volatile" qualifier set, without looking through typedefs that may have added "volatile" at a different level.
    ///</summary>
    public uint isVolatileQualifiedType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isVolatileQualifiedType(ToNative());
    }

    ///<summary>
    /// Return the alignment of a type in bytes as per C++[expr.alignof] standard.
    ///</summary>
    public long Type_getAlignOf()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getAlignOf(ToNative());
    }

    ///<summary>
    /// Return the class type of an member pointer type.
    ///</summary>
    public QBType Type_getClassType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getClassType(ToNative());
    }

    ///<summary>
    /// Retrieve the ref-qualifier kind of a function or method.
    ///</summary>
    public CXRefQualifierKind Type_getCXXRefQualifier()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getCXXRefQualifier(ToNative());
    }

    ///<summary>
    /// Return the type that was modified by this attributed type.
    ///</summary>
    public QBType Type_getModifiedType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getModifiedType(ToNative());
    }

    ///<summary>
    /// Retrieve the type named by the qualified-id.
    ///</summary>
    public QBType Type_getNamedType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getNamedType(ToNative());
    }

    ///<summary>
    /// Retrieve the nullability kind of a pointer type.
    ///</summary>
    public CXTypeNullabilityKind Type_getNullability()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getNullability(ToNative());
    }

    ///<summary>
    /// Retrieve the number of protocol references associated with an ObjC object/id.
    ///</summary>
    public uint Type_getNumObjCProtocolRefs()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getNumObjCProtocolRefs(ToNative());
    }

    ///<summary>
    /// Retrieve the number of type arguments associated with an ObjC object.
    ///</summary>
    public uint Type_getNumObjCTypeArgs()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getNumObjCTypeArgs(ToNative());
    }

    ///<summary>
    /// Returns the number of template arguments for given template specialization, or -1 if type T is not a template specialization.
    ///</summary>
    public int Type_getNumTemplateArguments()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getNumTemplateArguments(ToNative());
    }

    ///<summary>
    /// Returns the Objective-C type encoding for the specified CXType.
    ///</summary>
    public QBString Type_getObjCEncoding()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getObjCEncoding(ToNative());
    }

    ///<summary>
    /// Retrieves the base type of the ObjCObjectType.
    ///</summary>
    public QBType Type_getObjCObjectBaseType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getObjCObjectBaseType(ToNative());
    }

    ///<summary>
    /// Retrieve the decl for a protocol reference for an ObjC object/id.
    ///</summary>
    public QBCursor Type_getObjCProtocolDecl(uint i)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getObjCProtocolDecl(ToNative(), i);
    }

    ///<summary>
    /// Retrieve a type argument associated with an ObjC object.
    ///</summary>
    public QBType Type_getObjCTypeArg(uint i)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getObjCTypeArg(ToNative(), i);
    }

    ///<summary>
    /// Return the offset of a field named S in a record of type T in bits as it would be returned by __offsetof__ as per C++11[18.2p4]
    ///</summary>
    public long Type_getOffsetOf(string S)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(S, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getOffsetOf(ToNative(), arg1);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Return the size of a type in bytes as per C++[expr.sizeof] standard.
    ///</summary>
    public long Type_getSizeOf()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getSizeOf(ToNative());
    }

    ///<summary>
    /// Returns the type template argument of a template class specialization at given index.
    ///</summary>
    public QBType Type_getTemplateArgumentAsType(uint i)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getTemplateArgumentAsType(ToNative(), i);
    }

    ///<summary>
    /// Gets the type contained by this atomic type.
    ///</summary>
    public QBType Type_getValueType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_getValueType(ToNative());
    }

    ///<summary>
    /// Determine if a typedef is 'transparent' tag.
    ///</summary>
    public uint Type_isTransparentTagTypedef()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_isTransparentTagTypedef(ToNative());
    }

    ///<summary>
    /// Visit the fields of a particular type.
    ///</summary>
    public uint Type_visitFields(void* visitor, QBClientData client_data)
    {
        var arg2 = ReferenceEquals(client_data, null) ? new CXClientDataImpl() : (CXClientDataImpl)client_data;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Type_visitFields(ToNative(), visitor, arg2);
    }


    public QuantumBinding.Clang.Interop.CXType ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXType();
        _internal.kind = Kind;
        if(Data != null)
        {
            if (Data.Length > 2)
                throw new System.ArgumentOutOfRangeException(nameof(Data), "Array is out of bounds. Size should not be more than 2");

            for (int i = 0; i < 2; ++i)
            {
                _internal.data[i] = Data[i];
            }
        }
        return _internal;
    }

    public static implicit operator QBType(QuantumBinding.Clang.Interop.CXType q)
    {
        return new QBType(q);
    }

}



