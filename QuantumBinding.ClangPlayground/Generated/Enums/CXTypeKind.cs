
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the kind of type
///</summary>
public enum CXTypeKind : uint
{
    ///<summary>
    /// Represents an invalid type (e.g., where no type is available).
    ///</summary>
    CXType_Invalid = 0,

    ///<summary>
    /// A type whose specific kind is not exposed via this interface.
    ///</summary>
    CXType_Unexposed = 1,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Void = 2,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Bool = 3,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Char_U = 4,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_UChar = 5,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Char16 = 6,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Char32 = 7,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_UShort = 8,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_UInt = 9,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ULong = 10,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ULongLong = 11,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_UInt128 = 12,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Char_S = 13,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_SChar = 14,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_WChar = 15,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Short = 16,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Int = 17,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Long = 18,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_LongLong = 19,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Int128 = 20,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Float = 21,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Double = 22,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_LongDouble = 23,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_NullPtr = 24,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Overload = 25,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Dependent = 26,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ObjCId = 27,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ObjCClass = 28,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ObjCSel = 29,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Float128 = 30,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Half = 31,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Float16 = 32,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ShortAccum = 33,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Accum = 34,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_LongAccum = 35,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_UShortAccum = 36,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_UAccum = 37,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ULongAccum = 38,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_BFloat16 = 39,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Ibm128 = 40,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_FirstBuiltin = 2,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_LastBuiltin = 40,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Complex = 100,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Pointer = 101,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_BlockPointer = 102,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_LValueReference = 103,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_RValueReference = 104,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Record = 105,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Enum = 106,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Typedef = 107,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ObjCInterface = 108,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ObjCObjectPointer = 109,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_FunctionNoProto = 110,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_FunctionProto = 111,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_ConstantArray = 112,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Vector = 113,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_IncompleteArray = 114,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_VariableArray = 115,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_DependentSizedArray = 116,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_MemberPointer = 117,

    ///<summary>
    /// Builtin types
    ///</summary>
    CXType_Auto = 118,

    ///<summary>
    /// Represents a type that was referred to using an elaborated type keyword.
    ///</summary>
    CXType_Elaborated = 119,

    ///<summary>
    /// OpenCL PipeType.
    ///</summary>
    CXType_Pipe = 120,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dRO = 121,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dArrayRO = 122,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dBufferRO = 123,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dRO = 124,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayRO = 125,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dDepthRO = 126,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayDepthRO = 127,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dMSAARO = 128,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayMSAARO = 129,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dMSAADepthRO = 130,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayMSAADepthRO = 131,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage3dRO = 132,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dWO = 133,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dArrayWO = 134,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dBufferWO = 135,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dWO = 136,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayWO = 137,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dDepthWO = 138,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayDepthWO = 139,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dMSAAWO = 140,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayMSAAWO = 141,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dMSAADepthWO = 142,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayMSAADepthWO = 143,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage3dWO = 144,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dRW = 145,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dArrayRW = 146,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage1dBufferRW = 147,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dRW = 148,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayRW = 149,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dDepthRW = 150,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayDepthRW = 151,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dMSAARW = 152,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayMSAARW = 153,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dMSAADepthRW = 154,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage2dArrayMSAADepthRW = 155,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLImage3dRW = 156,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLSampler = 157,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLEvent = 158,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLQueue = 159,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLReserveID = 160,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_ObjCObject = 161,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_ObjCTypeParam = 162,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_Attributed = 163,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCMcePayload = 164,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCImePayload = 165,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCRefPayload = 166,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCSicPayload = 167,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCMceResult = 168,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCImeResult = 169,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCRefResult = 170,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCSicResult = 171,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCImeResultSingleRefStreamout = 172,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCImeResultDualRefStreamout = 173,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCImeSingleRefStreamin = 174,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_OCLIntelSubgroupAVCImeDualRefStreamin = 175,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_ExtVector = 176,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_Atomic = 177,

    ///<summary>
    /// OpenCL builtin types.
    ///</summary>
    CXType_BTFTagAttributed = 178,

}



