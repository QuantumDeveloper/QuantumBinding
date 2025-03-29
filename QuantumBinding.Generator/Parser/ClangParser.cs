using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using QuantumBinding.Generator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.Parser
{
    public unsafe class ClangParser : ICXCursorVisitor
    {
        private readonly ISet<string> visitedEnums = new HashSet<string>();
        private readonly ISet<string> visitedStructs = new HashSet<string>();
        private readonly ISet<string> visitedTypeDefs = new HashSet<string>();
        private readonly HashSet<string> visitedFunctions = new HashSet<string>();
        private readonly HashSet<string> visitedMacros = new HashSet<string>();

        private readonly TranslationUnit unit;
        private int fieldPosition;

        public ClangParser(TranslationUnit unit)
        {
            this.unit = unit;
        }

        private QBTranslationUnit translationUnit;
        private Delegates.CXCursorVisitor visitor;
        private Delegates.CXCursorVisitor functionPtr;

        public ParseResult Parse(QBIndex index, string filePath, List<string> arguments)
        {
            ParseResult parseResult = ParseResult.Success;
            var flags = CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord |
                        CXTranslationUnit_Flags.CXTranslationUnit_IncludeBriefCommentsInCodeCompletion;
            try
            {
                QBUnsavedFile[] unsavedFile = Array.Empty<QBUnsavedFile>();
                var translationUnitResult = index.ParseTranslationUnit2(
                    filePath,
                    arguments.ToArray(),
                    arguments.Count,
                    unsavedFile,
                    0,
                    (uint)flags,
                    out translationUnit);

                if (translationUnitResult != CXErrorCode.CXError_Success)
                {
                    Console.WriteLine("Error: " + translationUnitResult);
                    var numDiagnostics = translationUnit.GetNumDiagnostics();

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = translationUnit.GetDiagnostic(i);
                        Console.WriteLine(diagnostic.GetDiagnosticSpelling().ToString());
                        diagnostic.DisposeDiagnostic();
                    }
                    parseResult = (ParseResult)translationUnitResult;
                    return parseResult;
                }

                visitor = VisitDelegate;

                translationUnit.GetTranslationUnitCursor().VisitChildren(Marshal.GetFunctionPointerForDelegate(visitor).ToPointer() , new QBClientData());

                parseResult = (ParseResult)translationUnitResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                translationUnit.DisposeTranslationUnit();
            }

            return parseResult;
        }

        public CXChildVisitResult VisitDelegate(CXCursor cursor, CXCursor parent, CXClientDataImpl data)
        {
            return Visit(cursor, parent, data);
        }

        public CXChildVisitResult Visit(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = cursor.GetCursorKind();
            switch (curKind)
            {
                case CXCursorKind.CXCursor_EnumDecl:
                    return VisitEnum(cursor, parent, data);
                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_UnionDecl:
                    return VisitStruct(cursor, parent, data);
                case CXCursorKind.CXCursor_TypedefDecl:
                    return VisitTypedef(cursor, parent, data);
                case CXCursorKind.CXCursor_VarDecl:
                    return VisitVarDecl(cursor, parent, data);
                case CXCursorKind.CXCursor_FunctionDecl:
                    return VisitFunction(cursor, parent, data);
                case CXCursorKind.CXCursor_MacroDefinition:
                    return VisitMacro(cursor, parent, data);
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }

        private CXChildVisitResult VisitEnum(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            var enumName = cursor.GetCursorSpelling().ToString();

            // enumName can be empty because of typedef enum { .. } enumName;
            // so we have to find the sibling, and this is the only way I've found
            // to do with libclang, maybe there is a better way?
            if (string.IsNullOrEmpty(enumName))
            {
                var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                cursor.GetCursorLexicalParent().VisitChildren(forwardDeclaringVisitor.VisitorPtr.ToPointer(), data);
                enumName = forwardDeclaringVisitor.ForwardDeclarationCursor.GetCursorSpelling().ToString();

                if (string.IsNullOrEmpty(enumName))
                {
                    enumName = "_";
                }
            }

            // if we've printed these previously, skip them
            if (visitedEnums.Contains(enumName))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            string inheritedEnumType = cursor.GetEnumUnderlyingType();

            var @enum = new Enumeration
            {
                Location = ClangUtils.GetCurrentCursorLocation(cursor),
                Name = enumName,
                OriginalName = enumName,
                InheritanceType = inheritedEnumType,
                Comment = GetComment(cursor)
            };

            visitedEnums.Add(enumName);

            functionPtr = VisitEnumItemsDelegate;

            // visit all the enum values
            cursor.VisitChildren(Marshal.GetFunctionPointerForDelegate(functionPtr).ToPointer(), new QBClientData());

            CXChildVisitResult VisitEnumItemsDelegate(CXCursor cursor, CXCursor parent, CXClientDataImpl data)
            {
                return VisitEnumItems(cursor, parent, data);
            }

            CXChildVisitResult VisitEnumItems(QBCursor cursor, QBCursor parent, QBClientData data)
            {
                var enumItemDescriptor = new EnumerationItem
                {
                    Name = cursor.GetCursorSpelling().ToString(),
                    Value = cursor.GetEnumConstantDeclValue(),
                    Comment = GetComment(cursor)
                };
                @enum.Items.Add(enumItemDescriptor);
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            // Cross-plat hack:
            // For whatever reason, libclang detects untyped enums (i.e. your average 'enum X { A, B }')
            // as uints on Linux and ints on Windows.
            // Since we want to have the same generated code everywhere, we try to force 'int'
            // if it doesn't change semantics, i.e. if all enum values are in the right range.
            // Remember that 2's complement ints use the same binary representation as uints for positive numbers.

            var minValue = @enum.Items.Select(x => x.Value).Min();

            if (minValue < 0)
            {
                switch (@enum.InheritanceType)
                {
                    case "uint":
                        @enum.InheritanceType = "int";
                        break;
                    case "ushort":
                        @enum.InheritanceType = "short";
                        break;
                    case "ulong":
                        @enum.InheritanceType = "long";
                        break;
                    default:
                        @enum.InheritanceType = "int";
                        break;
                }
            }
            else
            {
                switch (@enum.InheritanceType)
                {
                    case "int":
                        @enum.InheritanceType = "uint";
                        break;
                    case "short":
                        @enum.InheritanceType = "ushort";
                        break;
                    case "long":
                        @enum.InheritanceType = "ulong";
                        break;
                    default:
                        @enum.InheritanceType = "uint";
                        break;
                }
            }

            AddDeclaration(@enum);

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitStruct(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            fieldPosition = 0;
            var structName = cursor.GetCursorSpelling().ToString();

            // struct names can be empty, and so we visit its sibling to find the name
            if (string.IsNullOrEmpty(structName))
            {
                var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                cursor.GetCursorSemanticParent().VisitChildren(
                    forwardDeclaringVisitor.VisitorPtr.ToPointer(),
                    new QBClientData());
                structName = forwardDeclaringVisitor.ForwardDeclarationCursor.GetCursorSpelling().ToString();

                if (string.IsNullOrEmpty(structName))
                {
                    structName = "_";
                }
            }

            if (visitedStructs.Contains(structName))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            var @class = new Class
            {
                Name = structName,
                OriginalName = structName,
                ClassType = ClassType.Struct,
                Location = ClangUtils.GetCurrentCursorLocation(cursor),
                Comment = GetComment(cursor)
            };

            var kind = cursor.GetCursorKind();
            if (kind == CXCursorKind.CXCursor_UnionDecl)
            {
                @class.ClassType = ClassType.Union;
            }

            functionPtr = VisitStructFieldsNative;

            cursor.VisitChildren(Marshal.GetFunctionPointerForDelegate(functionPtr).ToPointer(), new QBClientData());

            CXChildVisitResult VisitStructFieldsNative(CXCursor cursor, CXCursor parent, CXClientDataImpl data)
            {
                return VisitStructFields(cursor, parent, data);
            }

            CXChildVisitResult VisitStructFields(QBCursor cursor, QBCursor parent, QBClientData data)
            {
                var fieldName = cursor.GetCursorSpelling().ToString();
                //var cursorType0 = clang.getCanonicalType(clang.getCursorType(cxCursor));
                var cursorType = cursor.GetCursorType();
                var fieldType = cursorType.GetBindingType();
                if (string.IsNullOrEmpty(fieldName))
                {
                    fieldName = "field" + this.fieldPosition;
                }

                var field = new Field
                {
                    Name = fieldName,
                    Type = fieldType,
                    Index = fieldPosition,
                    Comment = GetComment(cursor)
                };
                @class.AddField(field);
                fieldPosition++;
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            AddDeclaration(@class);

            visitedStructs.Add(structName);

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private Dictionary<string, Enumeration> enums = new Dictionary<string, Enumeration>();

        private CXChildVisitResult VisitVarDecl(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            var cursorType = cursor.GetCursorType();
            if (cursorType.IsConstQualifiedType() == 1)
            {
                var decl = cursorType.GetTypeDeclaration();
                if (decl.Kind == CXCursorKind.CXCursor_NoDeclFound)
                    return CXChildVisitResult.CXChildVisit_Continue;
                    
                var enumName = decl.ToString();

                if (!enums.TryGetValue(enumName, out var @enum))
                {
                    @enum = new Enumeration
                    {
                        InheritanceType = "ulong",
                        Name = enumName,
                        OriginalName = enumName,
                        Location = ClangUtils.GetCurrentCursorLocation(cursor)
                    };

                    enums.Add(@enumName, @enum);
                    AddDeclaration(@enum);
                }
                var spelling = cursor.GetCursorSpelling().ToString();
                var result = cursor.Cursor_Evaluate();
                var enumValue = result.EvalResult_getAsLongLong();
                var enumItem = new EnumerationItem
                {
                    Name = spelling,
                    Value = enumValue
                };
                @enum.Items.Add(enumItem);
            }
            
            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitTypedef(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            var spelling = cursor.GetCursorSpelling().ToString();

            if (visitedTypeDefs.Contains(spelling))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            visitedTypeDefs.Add(spelling);

            var type = cursor.GetTypedefDeclUnderlyingType().GetCanonicalType();

            // we handle enums and records in struct and enum visitors with forward declarations also
            if (type.Kind is CXTypeKind.CXType_Record or CXTypeKind.CXType_Enum)
            {
                var @class = unit.AllClasses.FirstOrDefault(x => x.Name == type.ToString());
                if (@class != null && @class.Name != spelling)
                {
                    unit.AddDummyType(spelling, @class.Name);
                }
                else if (@class == null && type.Kind == CXTypeKind.CXType_Enum)
                {
                    unit.AddDummyType(spelling, type.ToString());
                }

                return CXChildVisitResult.CXChildVisit_Continue;
            }

            // no idea what this is? -- template stuff?
            if (type.Kind == CXTypeKind.CXType_Unexposed)
            {
                var canonical = type.GetCanonicalType();
                if (canonical.Kind == CXTypeKind.CXType_Unexposed)
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }
            }

            if (type.Kind == CXTypeKind.CXType_Pointer)
            {
                var pointee = type.GetPointeeType();
                if (pointee.Kind is CXTypeKind.CXType_Record or CXTypeKind.CXType_Void)
                {
                    var convertToClass = unit.Module.AllowConvertStructToClass;
                    var classType = convertToClass ? ClassType.Class : ClassType.Struct;
                    var pointeeName = pointee.ToString();
                    if (pointeeName.StartsWith("const"))
                    {
                        pointeeName = pointeeName.Replace("const", "").Trim();
                    }

                    var @class = new Class
                    {
                        Location = ClangUtils.GetCurrentCursorLocation(cursor),
                        Name = spelling,
                        OriginalName = spelling,
                        IsTypedef = true,
                        IsPointer = true,
                        ClassType = classType,
                        InnerStruct = unit.AllClasses.FirstOrDefault(x => x.Name == pointeeName),
                        Comment = GetComment(cursor)
                    };

                    if (convertToClass && classType == ClassType.Class && @class.InnerStruct == null)
                    {
                        var @struct = (Class)@class.Clone();
                        @struct.ClassType = ClassType.Struct;
                        @struct.Name += "Impl";
                        var field1 = new Field();
                        field1.AccessSpecifier = AccessSpecifier.Public;
                        field1.Name = "pointer";
                        field1.Type = new PointerType() { Pointee = new BuiltinType(PrimitiveType.Void) };
                        @struct.AddField(field1);
                        AddDeclaration(@struct);
                        @class.InnerStruct = @struct;
                    }

                    if (@class.InnerStruct != null)
                    {
                        @class.InnerStruct.LinkedTo = @class;
                    }

                    var dependentType = new DependentNameType(@class.Name, pointeeName);
                    dependentType.Declaration = @class;
                    @class.UnderlyingNativeType = dependentType;

                    if (@class.Name == "spv_binary_t")
                    {
                        int x = 0;
                    }
                    
                    var field = new Field();
                    if (classType == ClassType.Class)
                    {
                        field.AccessSpecifier = AccessSpecifier.Internal;
                        field.Name = "__Instance";
                        field.Type = new CustomType(@class.InnerStruct.Name);
                        field.Type.Declaration = @class.InnerStruct;
                    }
                    else
                    {
                        field.AccessSpecifier = AccessSpecifier.Public;
                        field.Name = "pointer";
                        field.Type = new PointerType() { Pointee = new BuiltinType(PrimitiveType.Void) };
                    }

                    @class.AddField(field);

                    var op = new Operator();
                    op.Class = @class;
                    op.FieldName = field.Name;
                    op.Type = field.Type;
                    op.TransformationKind = TransformationKind.FromClassToValue;
                    op.OperatorKind = OperatorKind.Implicit;
                    @class.Operators.Add(op);

                    op = new Operator();
                    op.Class = @class;
                    op.FieldName = field.Name;
                    op.Type = field.Type;
                    op.TransformationKind = TransformationKind.FromValueToClass;
                    op.OperatorKind = OperatorKind.Implicit;
                    op.PassValueToConstructor = true;
                    @class.Operators.Add(op);

                    if (convertToClass)
                    {
                        Constructor defaultCtr = new Constructor() { Class = @class, IsDefault = true };
                        @class.Constructors.Add(defaultCtr);

                        Constructor ctr = new Constructor() { Class = @class };
                        ctr.InputParameters.Add(field);
                        @class.Constructors.Add(ctr);
                    }

                    AddDeclaration(@class);

                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                if (pointee.Kind == CXTypeKind.CXType_FunctionProto)
                {
                    var callback = new Delegate();
                    callback.Location = ClangUtils.GetCurrentCursorLocation(cursor);
                    callback.CallingConvention = pointee.GetCallingConvention();
                    callback.ReturnType = pointee.GetResultType().GetBindingType();
                    callback.Name = spelling;
                    callback.Comment = GetComment(cursor);

                    if (unit.Module.SuppressUnmanagedCodeSecurity)
                    {
                        callback.SuppressUnmanagedCodeSecurity = true;
                    }

                    uint argumentCounter = 0;

                    functionPtr = VisitFunctionProtoNative;

                    cursor.VisitChildren(Marshal.GetFunctionPointerForDelegate(functionPtr).ToPointer(), new QBClientData());

                    CXChildVisitResult VisitFunctionProtoNative(CXCursor cxCursor, CXCursor parent, CXClientDataImpl client_data)
                    {
                        return VisitFunctionProto(cxCursor, parent, client_data);
                    }

                    CXChildVisitResult VisitFunctionProto(QBCursor cxCursor, QBCursor parent, QBClientData client_data)
                    {
                        if (cxCursor.Kind == CXCursorKind.CXCursor_ParmDecl)
                        {
                            var argumentDescriptor = ClangUtils.ArgumentHelper(pointee, cxCursor, argumentCounter);
                            argumentCounter++;
                            argumentDescriptor.Parent = callback;
                            callback.Parameters.Add(argumentDescriptor);
                        }

                        return CXChildVisitResult.CXChildVisit_Continue;
                    }

                    AddDeclaration(callback);

                    return CXChildVisitResult.CXChildVisit_Continue;
                }
            }

            if (type.IsPODType() != 0)
            {
                // POD - plain old data type (C++ 3.9p10)
                var podType = type.GetPrimitiveType();

                var @class = new Class
                {
                    ClassType = ClassType.Struct,
                    Location = ClangUtils.GetCurrentCursorLocation(cursor),
                    Name = spelling,
                    OriginalName = spelling,
                    IsTypedef = true,
                    IsSimpleType = true,
                    Comment = GetComment(cursor)
                };

                var builtinType = new BuiltinType();
                builtinType.Declaration = @class;
                builtinType.Type = podType;

                @class.UnderlyingNativeType = builtinType;

                var field = new Field();
                field.Name = "value";
                field.Type = builtinType;

                @class.AddField(field);

                var op = new Operator
                {
                    Class = @class,
                    FieldName = field.Name,
                    Type = field.Type,
                    TransformationKind = TransformationKind.FromClassToValue,
                    OperatorKind = OperatorKind.Implicit
                };
                @class.Operators.Add(op);

                op = new Operator
                {
                    Class = @class,
                    FieldName = field.Name,
                    Type = field.Type,
                    TransformationKind = TransformationKind.FromValueToClass,
                    OperatorKind = OperatorKind.Implicit
                };
                @class.Operators.Add(op);

                AddDeclaration(@class);
            }

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitFunction(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            var functionName = cursor.GetCursorSpelling().ToString();

            if (this.visitedFunctions.Contains(functionName))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            this.visitedFunctions.Add(functionName);
            var function = ClangUtils.GetFunctionInfo(cursor);
            if (unit.Module.SuppressUnmanagedCodeSecurity)
            {
                function.SuppressUnmanagedCodeSecurity = true;
            }
            function.Comment = GetComment(cursor);
            function.Location = ClangUtils.GetCurrentCursorLocation(cursor);
            AddDeclaration(function);

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitMacro(QBCursor cursor, QBCursor parent, QBClientData data)
        {
            var spelling = cursor.GetCursorSpelling();
            var macroName = spelling.ToString();
            if (visitedMacros.Contains(spelling.ToString()))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            visitedMacros.Add(macroName);

            var macro = new Macro();
            macro.Location = ClangUtils.GetCurrentCursorLocation(cursor);
            macro.Name = macroName;
            macro.IsBuiltIn = cursor.Cursor_isMacroBuiltin() != 0;
            macro.IsFunctionLike = cursor.Cursor_isMacroFunctionLike() != 0;
            macro.Comment = GetComment(cursor);

            var extCur = cursor.GetCursorExtent();
            translationUnit.Tokenize(extCur, out var tokens, out uint numTokens);

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (i == 0)
                {
                    continue;
                }

                var tkn = new MacroToken
                {
                    Item = translationUnit.GetTokenSpelling(token).ToString(),
                    TokenKind = (MacroTokenKind)token.GetTokenKind()
                };
                macro.Value += tkn.Item;
                macro.Tokens.Add(tkn);
            }

            if (!string.IsNullOrEmpty(macro.Value))
            {
                AddDeclaration(macro);
            }

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private Comment GetComment(QBCursor cursor)
        {
            var cxComment = cursor.Cursor_getParsedComment();
            var commentKind = cxComment.Comment_getKind();

            if (commentKind == CXCommentKind.CXComment_Null)
            {
                return null;
            }

            var briefText = cursor.Cursor_getBriefCommentText().ToString();
            var rawComment = cursor.Cursor_getRawCommentText().ToString();

            var comment = new Comment()
            {
                BriefText = briefText,
                Text = rawComment,
                Kind = (RawCommentKind)commentKind
            };

            return comment;
        }

        private void AddDeclaration(DeclarationUnit declaration)
        {
            declaration.Owner = unit;
            unit.AddDeclaration(declaration);
        }
    }
}
