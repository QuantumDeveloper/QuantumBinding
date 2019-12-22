using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.Parser
{
    public class ClangParser : ICXCursorVisitor
    {
        private readonly ISet<string> printedEnums = new HashSet<string>();
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

        private CXTranslationUnit translationUnit;

        public ParseResult Parse(CXIndex index, string filePath, List<string> arguments)
        {
            ParseResult parseResult = ParseResult.Success;
            var flags = CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord |
                        CXTranslationUnit_Flags.CXTranslationUnit_IncludeBriefCommentsInCodeCompletion;
            try
            {
                CXUnsavedFile[] unsavedFile = new CXUnsavedFile[0];
                var translationUnitResult = clang.parseTranslationUnit2(
                    index,
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
                    var numDiagnostics = clang.getNumDiagnostics(translationUnit);

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = clang.getDiagnostic(translationUnit, i);
                        Console.WriteLine(clang.getDiagnosticSpelling(diagnostic).ToString());
                        clang.disposeDiagnostic(diagnostic);
                    }
                    parseResult = (ParseResult)translationUnitResult;
                    return parseResult;
                }

                clang.visitChildren(
                    clang.getTranslationUnitCursor(translationUnit),
                    Visit,
                    new CXClientData());

                parseResult = (ParseResult)translationUnitResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                clang.disposeTranslationUnit(translationUnit);
            }

            return parseResult;
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = clang.getCursorKind(cursor);
            switch (curKind)
            {
                case CXCursorKind.CXCursor_EnumDecl:
                    return VisitEnum(cursor, parent, data);
                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_UnionDecl:
                    return VisitStruct(cursor, parent, data);
                case CXCursorKind.CXCursor_TypedefDecl:
                    return VisitTypedef(cursor, parent, data);
                case CXCursorKind.CXCursor_FunctionDecl:
                    return VisitFunction(cursor, parent, data);
                case CXCursorKind.CXCursor_MacroDefinition:
                    return VisitMacro(cursor, parent, data);
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }

        private CXChildVisitResult VisitEnum(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            var enumName = clang.getCursorSpelling(cursor).ToString();

            // enumName can be empty because of typedef enum { .. } enumName;
            // so we have to find the sibling, and this is the only way I've found
            // to do with libclang, maybe there is a better way?
            if (string.IsNullOrEmpty(enumName))
            {
                var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                clang.visitChildren(clang.getCursorLexicalParent(cursor), forwardDeclaringVisitor.Visit, new CXClientData());
                enumName = clang.getCursorSpelling(forwardDeclaringVisitor.ForwardDeclarationCursor).ToString();

                if (string.IsNullOrEmpty(enumName))
                {
                    enumName = "_";
                }
            }

            // if we've printed these previously, skip them
            if (printedEnums.Contains(enumName))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            string inheritedEnumType = cursor.GetEnumUnderlyingType();

            var @enum = new Enumeration
            {
                Location = Utils.GetCurrentCursorLocation(cursor),
                Name = enumName,
                InheritanceType = inheritedEnumType,
                Comment = GetComment(cursor)
            };

            printedEnums.Add(enumName);

            // visit all the enum values
            clang.visitChildren(cursor, (cxCursor, visitor, cxData) =>
            {
                var enumItemDescriptor = new EnumerationItem
                {
                    Name = clang.getCursorSpelling(cxCursor).ToString(),
                    Value = clang.getEnumConstantDeclValue(cxCursor),
                    Comment = GetComment(cxCursor)
                };
                @enum.Items.Add(enumItemDescriptor);
                return CXChildVisitResult.CXChildVisit_Continue;
            }, new CXClientData());

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

        private CXChildVisitResult VisitStruct(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            this.fieldPosition = 0;
            var structName = clang.getCursorSpelling(cursor).ToString();

            // struct names can be empty, and so we visit its sibling to find the name
            if (string.IsNullOrEmpty(structName))
            {
                var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                clang.visitChildren(
                    clang.getCursorSemanticParent(cursor),
                    forwardDeclaringVisitor.Visit,
                    new CXClientData());
                structName = clang.getCursorSpelling(forwardDeclaringVisitor.ForwardDeclarationCursor).ToString();

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
                ClassType = ClassType.Struct,
                Location = Utils.GetCurrentCursorLocation(cursor),
                Comment = GetComment(cursor)
            };

            var kind = clang.getCursorKind(cursor);
            if (kind == CXCursorKind.CXCursor_UnionDecl)
            {
                @class.ClassType = ClassType.Union;
            }

            clang.visitChildren(
                cursor,
                (cxCursor, visitor, cxData) =>
                {
                    var fieldName = clang.getCursorSpelling(cxCursor).ToString();
                    //var cursorType0 = clang.getCanonicalType(clang.getCursorType(cxCursor));
                    var cursorType = clang.getCursorType(cxCursor);
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
                        Comment = GetComment(cxCursor)
                    };
                    @class.AddField(field);
                    fieldPosition++;
                    return CXChildVisitResult.CXChildVisit_Continue;
                },
                new CXClientData());

            AddDeclaration(@class);

            this.visitedStructs.Add(structName);

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitTypedef(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            var spelling = clang.getCursorSpelling(cursor).ToString();

            if (this.visitedTypeDefs.Contains(spelling))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            this.visitedTypeDefs.Add(spelling);

            CXType type = clang.getCanonicalType(clang.getTypedefDeclUnderlyingType(cursor));

            // we handle enums and records in struct and enum visitors with forward declarations also
            if (type.kind == CXTypeKind.CXType_Record || type.kind == CXTypeKind.CXType_Enum)
            {
                var @class = unit.AllClasses.FirstOrDefault(x => x.Name == type.ToString());
                if (@class != null && @class.Name != spelling)
                {
                    unit.AddDummyType(spelling, @class.Name);
                }

                return CXChildVisitResult.CXChildVisit_Continue;
            }

            // no idea what this is? -- template stuff?
            if (type.kind == CXTypeKind.CXType_Unexposed)
            {
                var canonical = clang.getCanonicalType(type);
                if (canonical.kind == CXTypeKind.CXType_Unexposed)
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }
            }

            if (type.kind == CXTypeKind.CXType_Pointer)
            {
                var pointee = clang.getPointeeType(type);
                if (pointee.kind == CXTypeKind.CXType_Record || pointee.kind == CXTypeKind.CXType_Void)
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
                        Location = Utils.GetCurrentCursorLocation(cursor),
                        Name = spelling,
                        IsTypedef = true,
                        IsPointer = true,
                        ClassType = classType,
                        InnerStruct = unit.AllClasses.FirstOrDefault(x => x.Name == pointeeName),
                        Comment = GetComment(cursor)
                    };

                    if (@class.InnerStruct != null)
                    {
                        @class.InnerStruct.ConnectedTo = @class;
                    }

                    var dependentType = new DependentNameType(@class.Name, pointeeName);
                    dependentType.Declaration = @class;
                    @class.UnderlyingNativeType = dependentType;

                    var field = new Field();
                    if (classType == ClassType.Class)
                    {
                        field.AccessSpecifier = AccessSpecifier.Internal;
                        field.Name = "__Instance";
                        field.Type = new CustomType(pointeeName);
                    }
                    else if (classType == ClassType.Struct)
                    {
                        field.AccessSpecifier = AccessSpecifier.Public;
                        field.Name = "pointer";
                        field.Type = new PointerType() {Pointee = new BuiltinType(PrimitiveType.IntPtr)};
                    }

                    @class.AddField(field);

                    var getter = new Method();
                    getter.Class = @class;
                    getter.AccessSpecifier = AccessSpecifier.Public;

                    var setter = new Method();
                    setter.Class = @class;
                    setter.AccessSpecifier = AccessSpecifier.Internal;

                    var op = new Operator();
                    op.Class = @class;
                    op.Field = field;
                    op.TransformationKind = TransformationKind.FromClassToValue;
                    op.OperatorKind = OperatorKind.Implicit;
                    @class.Operators.Add(op);

                    op = new Operator();
                    op.Class = @class;
                    op.Field = field;
                    op.TransformationKind = TransformationKind.FromValueToClass;
                    op.OperatorKind = OperatorKind.Implicit;
                    @class.Operators.Add(op);

                    if (convertToClass)
                    {
                        Constructor defaultCtr = new Constructor() {Class = @class, IsDefault = true};
                        @class.Constructors.Add(defaultCtr);
                        
                        Constructor ctr = new Constructor() { Class = @class };
                        ctr.InputParameters.Add(field);
                        @class.Constructors.Add(ctr);

                        if (@class.InnerStruct.Constructors.Count == 0)
                        {
                            var innerField = new Field();
                            innerField.AccessSpecifier = AccessSpecifier.Public;
                            innerField.Name = "pointer";
                            innerField.Type = new PointerType() { Pointee = new BuiltinType(PrimitiveType.IntPtr) };
                            Constructor ctr2 = new Constructor() { Class = @class.InnerStruct };
                            ctr2.InputParameters.Add(innerField);
                            @class.InnerStruct.AddConstructor(ctr2);
                        }
                    }

                    AddDeclaration(@class);

                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                if (pointee.kind == CXTypeKind.CXType_FunctionProto)
                {
                    var callback = new Delegate();
                    callback.Location = Utils.GetCurrentCursorLocation(cursor);
                    callback.CallingConvention = pointee.GetCallingConvention();
                    callback.ReturnType = clang.getResultType(pointee).GetBindingType();
                    callback.Name = spelling;
                    callback.Comment = GetComment(cursor);

                    if (unit.Module.SuppressUnmanagedCodeSecurity)
                    {
                        callback.SuppressUnmanagedCodeSecurity = true;
                    }

                    uint argumentCounter = 0;

                    clang.visitChildren(cursor, (cxCursor, parent1, ptr) =>
                    {
                        if (cxCursor.kind == CXCursorKind.CXCursor_ParmDecl)
                        {
                            var argumentDescriptor = Utils.ArgumentHelper(pointee, cxCursor, argumentCounter);
                            argumentCounter++;
                            argumentDescriptor.Parent = callback;
                            callback.Parameters.Add(argumentDescriptor);
                        }

                        return CXChildVisitResult.CXChildVisit_Continue;
                    }, new CXClientData());

                    AddDeclaration(callback);

                    return CXChildVisitResult.CXChildVisit_Continue;
                }
            }

            if (clang.isPODType(type) != 0)
            {
                // POD - plain old data type (C++ 3.9p10)
                var podType = type.GetPrimitiveType();

                var @class = new Class
                {
                    ClassType = ClassType.Struct,
                    Location = Utils.GetCurrentCursorLocation(cursor),
                    Name = spelling,
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
                    Field = field,
                    TransformationKind = TransformationKind.FromClassToValue,
                    OperatorKind = OperatorKind.Implicit
                };
                @class.Operators.Add(op);

                op = new Operator
                {
                    Class = @class,
                    Field = field,
                    TransformationKind = TransformationKind.FromValueToClass,
                    OperatorKind = OperatorKind.Implicit
                };
                @class.Operators.Add(op);

                AddDeclaration(@class);
            }

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitFunction(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            var functionName = clang.getCursorSpelling(cursor).ToString();

            if (this.visitedFunctions.Contains(functionName))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            this.visitedFunctions.Add(functionName);
            var function = Utils.GetFunctionInfo(cursor);
            if (unit.Module.SuppressUnmanagedCodeSecurity)
            {
                function.SuppressUnmanagedCodeSecurity = true;
            }
            function.Comment = GetComment(cursor);
            function.Location = Utils.GetCurrentCursorLocation(cursor);
            AddDeclaration(function);

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private CXChildVisitResult VisitMacro(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            var spelling = clang.getCursorSpelling(cursor);
            var macroName = spelling.ToString();
            if (this.visitedMacros.Contains(spelling.ToString()))
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            visitedMacros.Add(macroName);

            var macro = new Macro();
            macro.Location = Utils.GetCurrentCursorLocation(cursor);
            macro.Name = macroName;
            macro.IsBuiltIn = clang.Cursor_isMacroBuiltin(cursor) != 0;
            macro.IsFunctionLike = clang.Cursor_isMacroFunctionLike(cursor) != 0;
            macro.Comment = GetComment(cursor);

            var extCur = clang.getCursorExtent(cursor);
            clang.tokenize(translationUnit, extCur, out var tokens, out uint numTokens);

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (i == 0)
                {
                    continue;
                }

                var tkn = new MacroToken
                {
                    Item = clang.getTokenSpelling(translationUnit, token).ToString(),
                    TokenKind = (MacroTokenKind)clang.getTokenKind(token)
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

        private Comment GetComment(CXCursor cursor)
        {
            var cxComment = clang.Cursor_getParsedComment(cursor);
            var commentKind = clang.Comment_getKind(cxComment);

            if (commentKind == CXCommentKind.CXComment_Null)
            {
                return null;
            }

            var briefText = clang.Cursor_getBriefCommentText(cursor).ToString();
            var rawComment = clang.Cursor_getRawCommentText(cursor).ToString();

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