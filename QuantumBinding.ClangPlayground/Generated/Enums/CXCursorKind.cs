
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the kind of entity that a cursor refers to.
///</summary>
public enum CXCursorKind : uint
{
    ///<summary>
    /// Declarations
    ///</summary>
    CXCursor_UnexposedDecl = 1,

    ///<summary>
    /// A C or C++ struct.
    ///</summary>
    CXCursor_StructDecl = 2,

    ///<summary>
    /// A C or C++ union.
    ///</summary>
    CXCursor_UnionDecl = 3,

    ///<summary>
    /// A C++ class.
    ///</summary>
    CXCursor_ClassDecl = 4,

    ///<summary>
    /// An enumeration.
    ///</summary>
    CXCursor_EnumDecl = 5,

    ///<summary>
    /// A field (in C) or non-static data member (in C++) in a struct, union, or C++ class.
    ///</summary>
    CXCursor_FieldDecl = 6,

    ///<summary>
    /// An enumerator constant.
    ///</summary>
    CXCursor_EnumConstantDecl = 7,

    ///<summary>
    /// A function.
    ///</summary>
    CXCursor_FunctionDecl = 8,

    ///<summary>
    /// A variable.
    ///</summary>
    CXCursor_VarDecl = 9,

    ///<summary>
    /// A function or method parameter.
    ///</summary>
    CXCursor_ParmDecl = 10,

    ///<summary>
    /// An Objective-C @interface.
    ///</summary>
    CXCursor_ObjCInterfaceDecl = 11,

    ///<summary>
    /// An Objective-C @interface for a category.
    ///</summary>
    CXCursor_ObjCCategoryDecl = 12,

    ///<summary>
    /// An Objective-C @protocol declaration.
    ///</summary>
    CXCursor_ObjCProtocolDecl = 13,

    ///<summary>
    /// An Objective-C @property declaration.
    ///</summary>
    CXCursor_ObjCPropertyDecl = 14,

    ///<summary>
    /// An Objective-C instance variable.
    ///</summary>
    CXCursor_ObjCIvarDecl = 15,

    ///<summary>
    /// An Objective-C instance method.
    ///</summary>
    CXCursor_ObjCInstanceMethodDecl = 16,

    ///<summary>
    /// An Objective-C class method.
    ///</summary>
    CXCursor_ObjCClassMethodDecl = 17,

    ///<summary>
    /// An Objective-C @implementation.
    ///</summary>
    CXCursor_ObjCImplementationDecl = 18,

    ///<summary>
    /// An Objective-C @implementation for a category.
    ///</summary>
    CXCursor_ObjCCategoryImplDecl = 19,

    ///<summary>
    /// A typedef.
    ///</summary>
    CXCursor_TypedefDecl = 20,

    ///<summary>
    /// A C++ class method.
    ///</summary>
    CXCursor_CXXMethod = 21,

    ///<summary>
    /// A C++ namespace.
    ///</summary>
    CXCursor_Namespace = 22,

    ///<summary>
    /// A linkage specification, e.g. 'extern "C"'.
    ///</summary>
    CXCursor_LinkageSpec = 23,

    ///<summary>
    /// A C++ constructor.
    ///</summary>
    CXCursor_Constructor = 24,

    ///<summary>
    /// A C++ destructor.
    ///</summary>
    CXCursor_Destructor = 25,

    ///<summary>
    /// A C++ conversion function.
    ///</summary>
    CXCursor_ConversionFunction = 26,

    ///<summary>
    /// A C++ template type parameter.
    ///</summary>
    CXCursor_TemplateTypeParameter = 27,

    ///<summary>
    /// A C++ non-type template parameter.
    ///</summary>
    CXCursor_NonTypeTemplateParameter = 28,

    ///<summary>
    /// A C++ template template parameter.
    ///</summary>
    CXCursor_TemplateTemplateParameter = 29,

    ///<summary>
    /// A C++ function template.
    ///</summary>
    CXCursor_FunctionTemplate = 30,

    ///<summary>
    /// A C++ class template.
    ///</summary>
    CXCursor_ClassTemplate = 31,

    ///<summary>
    /// A C++ class template partial specialization.
    ///</summary>
    CXCursor_ClassTemplatePartialSpecialization = 32,

    ///<summary>
    /// A C++ namespace alias declaration.
    ///</summary>
    CXCursor_NamespaceAlias = 33,

    ///<summary>
    /// A C++ using directive.
    ///</summary>
    CXCursor_UsingDirective = 34,

    ///<summary>
    /// A C++ using declaration.
    ///</summary>
    CXCursor_UsingDeclaration = 35,

    ///<summary>
    /// A C++ alias declaration
    ///</summary>
    CXCursor_TypeAliasDecl = 36,

    ///<summary>
    /// An Objective-C @synthesize definition.
    ///</summary>
    CXCursor_ObjCSynthesizeDecl = 37,

    ///<summary>
    /// An Objective-C @dynamic definition.
    ///</summary>
    CXCursor_ObjCDynamicDecl = 38,

    ///<summary>
    /// An access specifier.
    ///</summary>
    CXCursor_CXXAccessSpecifier = 39,

    ///<summary>
    /// An access specifier.
    ///</summary>
    CXCursor_FirstDecl = 1,

    ///<summary>
    /// An access specifier.
    ///</summary>
    CXCursor_LastDecl = 39,

    ///<summary>
    /// Decl references
    ///</summary>
    CXCursor_FirstRef = 40,

    CXCursor_ObjCSuperClassRef = 40,

    CXCursor_ObjCProtocolRef = 41,

    CXCursor_ObjCClassRef = 42,

    ///<summary>
    /// A reference to a type declaration.
    ///</summary>
    CXCursor_TypeRef = 43,

    ///<summary>
    /// A reference to a type declaration.
    ///</summary>
    CXCursor_CXXBaseSpecifier = 44,

    ///<summary>
    /// A reference to a class template, function template, template template parameter, or class template partial specialization.
    ///</summary>
    CXCursor_TemplateRef = 45,

    ///<summary>
    /// A reference to a namespace or namespace alias.
    ///</summary>
    CXCursor_NamespaceRef = 46,

    ///<summary>
    /// A reference to a member of a struct, union, or class that occurs in some non-expression context, e.g., a designated initializer.
    ///</summary>
    CXCursor_MemberRef = 47,

    ///<summary>
    /// A reference to a labeled statement.
    ///</summary>
    CXCursor_LabelRef = 48,

    ///<summary>
    /// A reference to a set of overloaded functions or function templates that has not yet been resolved to a specific function or function template.
    ///</summary>
    CXCursor_OverloadedDeclRef = 49,

    ///<summary>
    /// A reference to a variable that occurs in some non-expression context, e.g., a C++ lambda capture list.
    ///</summary>
    CXCursor_VariableRef = 50,

    ///<summary>
    /// A reference to a variable that occurs in some non-expression context, e.g., a C++ lambda capture list.
    ///</summary>
    CXCursor_LastRef = 50,

    ///<summary>
    /// Error conditions
    ///</summary>
    CXCursor_FirstInvalid = 70,

    ///<summary>
    /// Error conditions
    ///</summary>
    CXCursor_InvalidFile = 70,

    ///<summary>
    /// Error conditions
    ///</summary>
    CXCursor_NoDeclFound = 71,

    ///<summary>
    /// Error conditions
    ///</summary>
    CXCursor_NotImplemented = 72,

    ///<summary>
    /// Error conditions
    ///</summary>
    CXCursor_InvalidCode = 73,

    ///<summary>
    /// Error conditions
    ///</summary>
    CXCursor_LastInvalid = 73,

    ///<summary>
    /// Expressions
    ///</summary>
    CXCursor_FirstExpr = 100,

    ///<summary>
    /// An expression whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_UnexposedExpr = 100,

    ///<summary>
    /// An expression that refers to some value declaration, such as a function, variable, or enumerator.
    ///</summary>
    CXCursor_DeclRefExpr = 101,

    ///<summary>
    /// An expression that refers to a member of a struct, union, class, Objective-C class, etc.
    ///</summary>
    CXCursor_MemberRefExpr = 102,

    ///<summary>
    /// An expression that calls a function.
    ///</summary>
    CXCursor_CallExpr = 103,

    ///<summary>
    /// An expression that sends a message to an Objective-C object or class.
    ///</summary>
    CXCursor_ObjCMessageExpr = 104,

    ///<summary>
    /// An expression that represents a block literal.
    ///</summary>
    CXCursor_BlockExpr = 105,

    ///<summary>
    /// An integer literal.
    ///</summary>
    CXCursor_IntegerLiteral = 106,

    ///<summary>
    /// A floating point number literal.
    ///</summary>
    CXCursor_FloatingLiteral = 107,

    ///<summary>
    /// An imaginary number literal.
    ///</summary>
    CXCursor_ImaginaryLiteral = 108,

    ///<summary>
    /// A string literal.
    ///</summary>
    CXCursor_StringLiteral = 109,

    ///<summary>
    /// A character literal.
    ///</summary>
    CXCursor_CharacterLiteral = 110,

    ///<summary>
    /// A parenthesized expression, e.g. "(1)".
    ///</summary>
    CXCursor_ParenExpr = 111,

    ///<summary>
    /// This represents the unary-expression's (except sizeof and alignof).
    ///</summary>
    CXCursor_UnaryOperator = 112,

    ///<summary>
    /// [C99 6.5.2.1] Array Subscripting.
    ///</summary>
    CXCursor_ArraySubscriptExpr = 113,

    ///<summary>
    /// A builtin binary operation expression such as "x + y" or "x <= y".
    ///</summary>
    CXCursor_BinaryOperator = 114,

    ///<summary>
    /// Compound assignment such as "+=".
    ///</summary>
    CXCursor_CompoundAssignOperator = 115,

    ///<summary>
    /// The ?: ternary operator.
    ///</summary>
    CXCursor_ConditionalOperator = 116,

    ///<summary>
    /// An explicit cast in C (C99 6.5.4) or a C-style cast in C++ (C++ [expr.cast]), which uses the syntax (Type)expr.
    ///</summary>
    CXCursor_CStyleCastExpr = 117,

    ///<summary>
    /// [C99 6.5.2.5]
    ///</summary>
    CXCursor_CompoundLiteralExpr = 118,

    ///<summary>
    /// Describes an C or C++ initializer list.
    ///</summary>
    CXCursor_InitListExpr = 119,

    ///<summary>
    /// The GNU address of label extension, representing &&label.
    ///</summary>
    CXCursor_AddrLabelExpr = 120,

    ///<summary>
    /// This is the GNU Statement Expression extension: ({int X=4; X;})
    ///</summary>
    CXCursor_StmtExpr = 121,

    ///<summary>
    /// Represents a C11 generic selection.
    ///</summary>
    CXCursor_GenericSelectionExpr = 122,

    ///<summary>
    /// Implements the GNU __null extension, which is a name for a null pointer constant that has integral type (e.g., int or long) and is the same size and alignment as a pointer.
    ///</summary>
    CXCursor_GNUNullExpr = 123,

    ///<summary>
    /// C++'s static_cast<> expression.
    ///</summary>
    CXCursor_CXXStaticCastExpr = 124,

    ///<summary>
    /// C++'s dynamic_cast<> expression.
    ///</summary>
    CXCursor_CXXDynamicCastExpr = 125,

    ///<summary>
    /// C++'s reinterpret_cast<> expression.
    ///</summary>
    CXCursor_CXXReinterpretCastExpr = 126,

    ///<summary>
    /// C++'s const_cast<> expression.
    ///</summary>
    CXCursor_CXXConstCastExpr = 127,

    ///<summary>
    /// Represents an explicit C++ type conversion that uses "functional" notion (C++ [expr.type.conv]).
    ///</summary>
    CXCursor_CXXFunctionalCastExpr = 128,

    ///<summary>
    /// A C++ typeid expression (C++ [expr.typeid]).
    ///</summary>
    CXCursor_CXXTypeidExpr = 129,

    ///<summary>
    /// [C++ 2.13.5] C++ Boolean Literal.
    ///</summary>
    CXCursor_CXXBoolLiteralExpr = 130,

    ///<summary>
    /// [C++0x 2.14.7] C++ Pointer Literal.
    ///</summary>
    CXCursor_CXXNullPtrLiteralExpr = 131,

    ///<summary>
    /// Represents the "this" expression in C++
    ///</summary>
    CXCursor_CXXThisExpr = 132,

    ///<summary>
    /// [C++ 15] C++ Throw Expression.
    ///</summary>
    CXCursor_CXXThrowExpr = 133,

    ///<summary>
    /// A new expression for memory allocation and constructor calls, e.g: "new CXXNewExpr(foo)".
    ///</summary>
    CXCursor_CXXNewExpr = 134,

    ///<summary>
    /// A delete expression for memory deallocation and destructor calls, e.g. "delete[] pArray".
    ///</summary>
    CXCursor_CXXDeleteExpr = 135,

    ///<summary>
    /// A unary expression. (noexcept, sizeof, or other traits)
    ///</summary>
    CXCursor_UnaryExpr = 136,

    ///<summary>
    /// An Objective-C string literal i.e. "foo".
    ///</summary>
    CXCursor_ObjCStringLiteral = 137,

    ///<summary>
    /// An Objective-C @encode expression.
    ///</summary>
    CXCursor_ObjCEncodeExpr = 138,

    ///<summary>
    /// An Objective-C @selector expression.
    ///</summary>
    CXCursor_ObjCSelectorExpr = 139,

    ///<summary>
    /// An Objective-C @protocol expression.
    ///</summary>
    CXCursor_ObjCProtocolExpr = 140,

    ///<summary>
    /// An Objective-C "bridged" cast expression, which casts between Objective-C pointers and C pointers, transferring ownership in the process.
    ///</summary>
    CXCursor_ObjCBridgedCastExpr = 141,

    ///<summary>
    /// Represents a C++0x pack expansion that produces a sequence of expressions.
    ///</summary>
    CXCursor_PackExpansionExpr = 142,

    ///<summary>
    /// Represents an expression that computes the length of a parameter pack.
    ///</summary>
    CXCursor_SizeOfPackExpr = 143,

    ///<summary>
    /// Represents a C++ lambda expression that produces a local function object.
    ///</summary>
    CXCursor_LambdaExpr = 144,

    ///<summary>
    /// Objective-c Boolean Literal.
    ///</summary>
    CXCursor_ObjCBoolLiteralExpr = 145,

    ///<summary>
    /// Represents the "self" expression in an Objective-C method.
    ///</summary>
    CXCursor_ObjCSelfExpr = 146,

    ///<summary>
    /// OpenMP 5.0 [2.1.5, Array Section].
    ///</summary>
    CXCursor_OMPArraySectionExpr = 147,

    ///<summary>
    /// Represents an (...) check.
    ///</summary>
    CXCursor_ObjCAvailabilityCheckExpr = 148,

    ///<summary>
    /// Fixed point literal
    ///</summary>
    CXCursor_FixedPointLiteral = 149,

    ///<summary>
    /// OpenMP 5.0 [2.1.4, Array Shaping].
    ///</summary>
    CXCursor_OMPArrayShapingExpr = 150,

    ///<summary>
    /// OpenMP 5.0 [2.1.6 Iterators]
    ///</summary>
    CXCursor_OMPIteratorExpr = 151,

    ///<summary>
    /// OpenCL's addrspace_cast<> expression.
    ///</summary>
    CXCursor_CXXAddrspaceCastExpr = 152,

    ///<summary>
    /// Expression that references a C++20 concept.
    ///</summary>
    CXCursor_ConceptSpecializationExpr = 153,

    ///<summary>
    /// Expression that references a C++20 concept.
    ///</summary>
    CXCursor_RequiresExpr = 154,

    ///<summary>
    /// Expression that references a C++20 parenthesized list aggregate initializer.
    ///</summary>
    CXCursor_CXXParenListInitExpr = 155,

    ///<summary>
    /// Expression that references a C++20 parenthesized list aggregate initializer.
    ///</summary>
    CXCursor_LastExpr = 155,

    ///<summary>
    /// Statements
    ///</summary>
    CXCursor_FirstStmt = 200,

    ///<summary>
    /// A statement whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_UnexposedStmt = 200,

    ///<summary>
    /// A labelled statement in a function.
    ///</summary>
    CXCursor_LabelStmt = 201,

    ///<summary>
    /// A group of statements like { stmt stmt }.
    ///</summary>
    CXCursor_CompoundStmt = 202,

    ///<summary>
    /// A case statement.
    ///</summary>
    CXCursor_CaseStmt = 203,

    ///<summary>
    /// A default statement.
    ///</summary>
    CXCursor_DefaultStmt = 204,

    ///<summary>
    /// An if statement
    ///</summary>
    CXCursor_IfStmt = 205,

    ///<summary>
    /// A switch statement.
    ///</summary>
    CXCursor_SwitchStmt = 206,

    ///<summary>
    /// A while statement.
    ///</summary>
    CXCursor_WhileStmt = 207,

    ///<summary>
    /// A do statement.
    ///</summary>
    CXCursor_DoStmt = 208,

    ///<summary>
    /// A for statement.
    ///</summary>
    CXCursor_ForStmt = 209,

    ///<summary>
    /// A goto statement.
    ///</summary>
    CXCursor_GotoStmt = 210,

    ///<summary>
    /// An indirect goto statement.
    ///</summary>
    CXCursor_IndirectGotoStmt = 211,

    ///<summary>
    /// A continue statement.
    ///</summary>
    CXCursor_ContinueStmt = 212,

    ///<summary>
    /// A break statement.
    ///</summary>
    CXCursor_BreakStmt = 213,

    ///<summary>
    /// A return statement.
    ///</summary>
    CXCursor_ReturnStmt = 214,

    ///<summary>
    /// A GCC inline assembly statement extension.
    ///</summary>
    CXCursor_GCCAsmStmt = 215,

    ///<summary>
    /// A GCC inline assembly statement extension.
    ///</summary>
    CXCursor_AsmStmt = 215,

    ///<summary>
    /// Objective-C's overall @try-@catch-@finally statement.
    ///</summary>
    CXCursor_ObjCAtTryStmt = 216,

    ///<summary>
    /// Objective-C's @catch statement.
    ///</summary>
    CXCursor_ObjCAtCatchStmt = 217,

    ///<summary>
    /// Objective-C's @finally statement.
    ///</summary>
    CXCursor_ObjCAtFinallyStmt = 218,

    ///<summary>
    /// Objective-C's @throw statement.
    ///</summary>
    CXCursor_ObjCAtThrowStmt = 219,

    ///<summary>
    /// Objective-C's @synchronized statement.
    ///</summary>
    CXCursor_ObjCAtSynchronizedStmt = 220,

    ///<summary>
    /// Objective-C's autorelease pool statement.
    ///</summary>
    CXCursor_ObjCAutoreleasePoolStmt = 221,

    ///<summary>
    /// Objective-C's collection statement.
    ///</summary>
    CXCursor_ObjCForCollectionStmt = 222,

    ///<summary>
    /// C++'s catch statement.
    ///</summary>
    CXCursor_CXXCatchStmt = 223,

    ///<summary>
    /// C++'s try statement.
    ///</summary>
    CXCursor_CXXTryStmt = 224,

    ///<summary>
    /// C++'s for (* : *) statement.
    ///</summary>
    CXCursor_CXXForRangeStmt = 225,

    ///<summary>
    /// Windows Structured Exception Handling's try statement.
    ///</summary>
    CXCursor_SEHTryStmt = 226,

    ///<summary>
    /// Windows Structured Exception Handling's except statement.
    ///</summary>
    CXCursor_SEHExceptStmt = 227,

    ///<summary>
    /// Windows Structured Exception Handling's finally statement.
    ///</summary>
    CXCursor_SEHFinallyStmt = 228,

    ///<summary>
    /// A MS inline assembly statement extension.
    ///</summary>
    CXCursor_MSAsmStmt = 229,

    ///<summary>
    /// The null statement ";": C99 6.8.3p3.
    ///</summary>
    CXCursor_NullStmt = 230,

    ///<summary>
    /// Adaptor class for mixing declarations with statements and expressions.
    ///</summary>
    CXCursor_DeclStmt = 231,

    ///<summary>
    /// OpenMP parallel directive.
    ///</summary>
    CXCursor_OMPParallelDirective = 232,

    ///<summary>
    /// OpenMP SIMD directive.
    ///</summary>
    CXCursor_OMPSimdDirective = 233,

    ///<summary>
    /// OpenMP for directive.
    ///</summary>
    CXCursor_OMPForDirective = 234,

    ///<summary>
    /// OpenMP sections directive.
    ///</summary>
    CXCursor_OMPSectionsDirective = 235,

    ///<summary>
    /// OpenMP section directive.
    ///</summary>
    CXCursor_OMPSectionDirective = 236,

    ///<summary>
    /// OpenMP single directive.
    ///</summary>
    CXCursor_OMPSingleDirective = 237,

    ///<summary>
    /// OpenMP parallel for directive.
    ///</summary>
    CXCursor_OMPParallelForDirective = 238,

    ///<summary>
    /// OpenMP parallel sections directive.
    ///</summary>
    CXCursor_OMPParallelSectionsDirective = 239,

    ///<summary>
    /// OpenMP task directive.
    ///</summary>
    CXCursor_OMPTaskDirective = 240,

    ///<summary>
    /// OpenMP master directive.
    ///</summary>
    CXCursor_OMPMasterDirective = 241,

    ///<summary>
    /// OpenMP critical directive.
    ///</summary>
    CXCursor_OMPCriticalDirective = 242,

    ///<summary>
    /// OpenMP taskyield directive.
    ///</summary>
    CXCursor_OMPTaskyieldDirective = 243,

    ///<summary>
    /// OpenMP barrier directive.
    ///</summary>
    CXCursor_OMPBarrierDirective = 244,

    ///<summary>
    /// OpenMP taskwait directive.
    ///</summary>
    CXCursor_OMPTaskwaitDirective = 245,

    ///<summary>
    /// OpenMP flush directive.
    ///</summary>
    CXCursor_OMPFlushDirective = 246,

    ///<summary>
    /// Windows Structured Exception Handling's leave statement.
    ///</summary>
    CXCursor_SEHLeaveStmt = 247,

    ///<summary>
    /// OpenMP ordered directive.
    ///</summary>
    CXCursor_OMPOrderedDirective = 248,

    ///<summary>
    /// OpenMP atomic directive.
    ///</summary>
    CXCursor_OMPAtomicDirective = 249,

    ///<summary>
    /// OpenMP for SIMD directive.
    ///</summary>
    CXCursor_OMPForSimdDirective = 250,

    ///<summary>
    /// OpenMP parallel for SIMD directive.
    ///</summary>
    CXCursor_OMPParallelForSimdDirective = 251,

    ///<summary>
    /// OpenMP target directive.
    ///</summary>
    CXCursor_OMPTargetDirective = 252,

    ///<summary>
    /// OpenMP teams directive.
    ///</summary>
    CXCursor_OMPTeamsDirective = 253,

    ///<summary>
    /// OpenMP taskgroup directive.
    ///</summary>
    CXCursor_OMPTaskgroupDirective = 254,

    ///<summary>
    /// OpenMP cancellation point directive.
    ///</summary>
    CXCursor_OMPCancellationPointDirective = 255,

    ///<summary>
    /// OpenMP cancel directive.
    ///</summary>
    CXCursor_OMPCancelDirective = 256,

    ///<summary>
    /// OpenMP target data directive.
    ///</summary>
    CXCursor_OMPTargetDataDirective = 257,

    ///<summary>
    /// OpenMP taskloop directive.
    ///</summary>
    CXCursor_OMPTaskLoopDirective = 258,

    ///<summary>
    /// OpenMP taskloop simd directive.
    ///</summary>
    CXCursor_OMPTaskLoopSimdDirective = 259,

    ///<summary>
    /// OpenMP distribute directive.
    ///</summary>
    CXCursor_OMPDistributeDirective = 260,

    ///<summary>
    /// OpenMP target enter data directive.
    ///</summary>
    CXCursor_OMPTargetEnterDataDirective = 261,

    ///<summary>
    /// OpenMP target exit data directive.
    ///</summary>
    CXCursor_OMPTargetExitDataDirective = 262,

    ///<summary>
    /// OpenMP target parallel directive.
    ///</summary>
    CXCursor_OMPTargetParallelDirective = 263,

    ///<summary>
    /// OpenMP target parallel for directive.
    ///</summary>
    CXCursor_OMPTargetParallelForDirective = 264,

    ///<summary>
    /// OpenMP target update directive.
    ///</summary>
    CXCursor_OMPTargetUpdateDirective = 265,

    ///<summary>
    /// OpenMP distribute parallel for directive.
    ///</summary>
    CXCursor_OMPDistributeParallelForDirective = 266,

    ///<summary>
    /// OpenMP distribute parallel for simd directive.
    ///</summary>
    CXCursor_OMPDistributeParallelForSimdDirective = 267,

    ///<summary>
    /// OpenMP distribute simd directive.
    ///</summary>
    CXCursor_OMPDistributeSimdDirective = 268,

    ///<summary>
    /// OpenMP target parallel for simd directive.
    ///</summary>
    CXCursor_OMPTargetParallelForSimdDirective = 269,

    ///<summary>
    /// OpenMP target simd directive.
    ///</summary>
    CXCursor_OMPTargetSimdDirective = 270,

    ///<summary>
    /// OpenMP teams distribute directive.
    ///</summary>
    CXCursor_OMPTeamsDistributeDirective = 271,

    ///<summary>
    /// OpenMP teams distribute simd directive.
    ///</summary>
    CXCursor_OMPTeamsDistributeSimdDirective = 272,

    ///<summary>
    /// OpenMP teams distribute parallel for simd directive.
    ///</summary>
    CXCursor_OMPTeamsDistributeParallelForSimdDirective = 273,

    ///<summary>
    /// OpenMP teams distribute parallel for directive.
    ///</summary>
    CXCursor_OMPTeamsDistributeParallelForDirective = 274,

    ///<summary>
    /// OpenMP target teams directive.
    ///</summary>
    CXCursor_OMPTargetTeamsDirective = 275,

    ///<summary>
    /// OpenMP target teams distribute directive.
    ///</summary>
    CXCursor_OMPTargetTeamsDistributeDirective = 276,

    ///<summary>
    /// OpenMP target teams distribute parallel for directive.
    ///</summary>
    CXCursor_OMPTargetTeamsDistributeParallelForDirective = 277,

    ///<summary>
    /// OpenMP target teams distribute parallel for simd directive.
    ///</summary>
    CXCursor_OMPTargetTeamsDistributeParallelForSimdDirective = 278,

    ///<summary>
    /// OpenMP target teams distribute simd directive.
    ///</summary>
    CXCursor_OMPTargetTeamsDistributeSimdDirective = 279,

    ///<summary>
    /// C++2a std::bit_cast expression.
    ///</summary>
    CXCursor_BuiltinBitCastExpr = 280,

    ///<summary>
    /// OpenMP master taskloop directive.
    ///</summary>
    CXCursor_OMPMasterTaskLoopDirective = 281,

    ///<summary>
    /// OpenMP parallel master taskloop directive.
    ///</summary>
    CXCursor_OMPParallelMasterTaskLoopDirective = 282,

    ///<summary>
    /// OpenMP master taskloop simd directive.
    ///</summary>
    CXCursor_OMPMasterTaskLoopSimdDirective = 283,

    ///<summary>
    /// OpenMP parallel master taskloop simd directive.
    ///</summary>
    CXCursor_OMPParallelMasterTaskLoopSimdDirective = 284,

    ///<summary>
    /// OpenMP parallel master directive.
    ///</summary>
    CXCursor_OMPParallelMasterDirective = 285,

    ///<summary>
    /// OpenMP depobj directive.
    ///</summary>
    CXCursor_OMPDepobjDirective = 286,

    ///<summary>
    /// OpenMP scan directive.
    ///</summary>
    CXCursor_OMPScanDirective = 287,

    ///<summary>
    /// OpenMP tile directive.
    ///</summary>
    CXCursor_OMPTileDirective = 288,

    ///<summary>
    /// OpenMP canonical loop.
    ///</summary>
    CXCursor_OMPCanonicalLoop = 289,

    ///<summary>
    /// OpenMP interop directive.
    ///</summary>
    CXCursor_OMPInteropDirective = 290,

    ///<summary>
    /// OpenMP dispatch directive.
    ///</summary>
    CXCursor_OMPDispatchDirective = 291,

    ///<summary>
    /// OpenMP masked directive.
    ///</summary>
    CXCursor_OMPMaskedDirective = 292,

    ///<summary>
    /// OpenMP unroll directive.
    ///</summary>
    CXCursor_OMPUnrollDirective = 293,

    ///<summary>
    /// OpenMP metadirective directive.
    ///</summary>
    CXCursor_OMPMetaDirective = 294,

    ///<summary>
    /// OpenMP loop directive.
    ///</summary>
    CXCursor_OMPGenericLoopDirective = 295,

    ///<summary>
    /// OpenMP teams loop directive.
    ///</summary>
    CXCursor_OMPTeamsGenericLoopDirective = 296,

    ///<summary>
    /// OpenMP target teams loop directive.
    ///</summary>
    CXCursor_OMPTargetTeamsGenericLoopDirective = 297,

    ///<summary>
    /// OpenMP parallel loop directive.
    ///</summary>
    CXCursor_OMPParallelGenericLoopDirective = 298,

    ///<summary>
    /// OpenMP target parallel loop directive.
    ///</summary>
    CXCursor_OMPTargetParallelGenericLoopDirective = 299,

    ///<summary>
    /// OpenMP parallel masked directive.
    ///</summary>
    CXCursor_OMPParallelMaskedDirective = 300,

    ///<summary>
    /// OpenMP masked taskloop directive.
    ///</summary>
    CXCursor_OMPMaskedTaskLoopDirective = 301,

    ///<summary>
    /// OpenMP masked taskloop simd directive.
    ///</summary>
    CXCursor_OMPMaskedTaskLoopSimdDirective = 302,

    ///<summary>
    /// OpenMP parallel masked taskloop directive.
    ///</summary>
    CXCursor_OMPParallelMaskedTaskLoopDirective = 303,

    ///<summary>
    /// OpenMP parallel masked taskloop simd directive.
    ///</summary>
    CXCursor_OMPParallelMaskedTaskLoopSimdDirective = 304,

    ///<summary>
    /// OpenMP error directive.
    ///</summary>
    CXCursor_OMPErrorDirective = 305,

    ///<summary>
    /// OpenMP error directive.
    ///</summary>
    CXCursor_LastStmt = 305,

    ///<summary>
    /// Cursor that represents the translation unit itself.
    ///</summary>
    CXCursor_TranslationUnit = 350,

    ///<summary>
    /// Attributes
    ///</summary>
    CXCursor_FirstAttr = 400,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_UnexposedAttr = 400,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_IBActionAttr = 401,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_IBOutletAttr = 402,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_IBOutletCollectionAttr = 403,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CXXFinalAttr = 404,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CXXOverrideAttr = 405,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_AnnotateAttr = 406,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_AsmLabelAttr = 407,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_PackedAttr = 408,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_PureAttr = 409,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ConstAttr = 410,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_NoDuplicateAttr = 411,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CUDAConstantAttr = 412,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CUDADeviceAttr = 413,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CUDAGlobalAttr = 414,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CUDAHostAttr = 415,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_CUDASharedAttr = 416,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_VisibilityAttr = 417,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_DLLExport = 418,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_DLLImport = 419,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_NSReturnsRetained = 420,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_NSReturnsNotRetained = 421,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_NSReturnsAutoreleased = 422,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_NSConsumesSelf = 423,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_NSConsumed = 424,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCException = 425,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCNSObject = 426,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCIndependentClass = 427,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCPreciseLifetime = 428,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCReturnsInnerPointer = 429,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCRequiresSuper = 430,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCRootClass = 431,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCSubclassingRestricted = 432,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCExplicitProtocolImpl = 433,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCDesignatedInitializer = 434,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCRuntimeVisible = 435,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ObjCBoxable = 436,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_FlagEnum = 437,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_ConvergentAttr = 438,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_WarnUnusedAttr = 439,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_WarnUnusedResultAttr = 440,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_AlignedAttr = 441,

    ///<summary>
    /// An attribute whose specific kind is not exposed via this interface.
    ///</summary>
    CXCursor_LastAttr = 441,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_PreprocessingDirective = 500,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_MacroDefinition = 501,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_MacroExpansion = 502,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_MacroInstantiation = 502,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_InclusionDirective = 503,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_FirstPreprocessing = 500,

    ///<summary>
    /// Preprocessing
    ///</summary>
    CXCursor_LastPreprocessing = 503,

    ///<summary>
    /// Extra Declarations
    ///</summary>
    CXCursor_ModuleImportDecl = 600,

    ///<summary>
    /// Extra Declarations
    ///</summary>
    CXCursor_TypeAliasTemplateDecl = 601,

    ///<summary>
    /// A static_assert or _Static_assert node
    ///</summary>
    CXCursor_StaticAssert = 602,

    ///<summary>
    /// a friend declaration.
    ///</summary>
    CXCursor_FriendDecl = 603,

    ///<summary>
    /// a concept declaration.
    ///</summary>
    CXCursor_ConceptDecl = 604,

    ///<summary>
    /// a concept declaration.
    ///</summary>
    CXCursor_FirstExtraDecl = 600,

    ///<summary>
    /// a concept declaration.
    ///</summary>
    CXCursor_LastExtraDecl = 604,

    ///<summary>
    /// A code completion overload candidate.
    ///</summary>
    CXCursor_OverloadCandidate = 700,

}



