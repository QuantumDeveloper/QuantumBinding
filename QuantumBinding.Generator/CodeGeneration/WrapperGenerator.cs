using System;
using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class WrapperGenerator : CSharpCodeGenerator
    {
        private readonly string interopNamespace = TranslationUnit.InteropNamespaceExtension.SubNamespace;
        private const string UnsafeClassName = "System.Runtime.CompilerServices.Unsafe";
        private const string MemoryMarshalClassName = "System.Runtime.InteropServices.MemoryMarshal";
        private const string MarshalContextClassName = "QuantumBinding.Utils.MarshallingContext";
        private const string MarshalInterfaceName = "IMarshallable";
        private const string MarshalObjectInterfaceName = "IMarshallableObject";
        private const string SpanClassName = "System.Span";
        private const string ReadonlySpanClassName = "System.ReadOnlySpan";
        private const string MarshalFromMethodName = "MarshalFrom";
        
        public WrapperGenerator(ProcessingContext context, TranslationUnit unit, GeneratorCategory category) : 
            base(context, unit, category)
        {
        }

        public WrapperGenerator(ProcessingContext context, IEnumerable<TranslationUnit> units, GeneratorCategory category) : 
            base(context, units, category)
        {
        }

        public override void Run()
        {
            if (Category == GeneratorCategory.Undefined)
            {
                return;
            }
            
            Name = Category.ToString();
            
            PushBlock(CodeBlockKind.Root);

            GenerateFileHeader();

            foreach (var unit in TranslationUnits)
            {
                CurrentTranslationUnit = unit;

                TypePrinter.PushModule(unit.Module);

                NewLine();

                PushBlock(CodeBlockKind.Namespace);
                
                GenerateUsings();

                WriteCurrentNamespace(CurrentTranslationUnit);

                UsingsBlock.WriteLine($"using {CurrentTranslationUnit.Module.OutputNamespace};");

                NewLine();

                GenerateWrappers(CurrentTranslationUnit);

                PopBlock();
            }

            PopBlock();
        }
        
        protected override bool IsDeclarationEqualsSpec(Declaration decl, GeneratorSpecializations spec)
        {
            switch (spec)
            {
                case GeneratorSpecializations.StructWrappers when decl is Class @class && @class.ClassType == ClassType.StructWrapper:
                case GeneratorSpecializations.UnionWrappers when decl is Class @struct && @struct.ClassType == ClassType.UnionWrapper:
                    return true;
                default:
                    return false;
            }
        }
        
        protected override void GenerateDeclaration(Declaration declaration)
        {
            switch (declaration)
            {
                case Class { ClassType: ClassType.StructWrapper or ClassType.UnionWrapper } @class:
                    GenerateStructWrapper(@class);
                    break;
            }
        }

        private void GenerateWrappers(TranslationUnit unit)
        {
            var dict = new Dictionary<GeneratorCategory, Action>
            {
                { GeneratorCategory.StructWrappers, () => GenerateStructWrappers(unit) },
                { GeneratorCategory.UnionWrappers, () => GenerateUnionWrappers(unit) }
            };
            
            dict[Category].Invoke();
            NewLine();
        }

        private void GenerateUnionWrappers(TranslationUnit unit)
        {
            foreach (var @class in unit.UnionWrappers)
            {
                GenerateStructWrapper(@class);
            }
        }

        private void GenerateStructWrappers(TranslationUnit unit)
        {
            foreach (var @class in unit.StructWrappers)
            {
                GenerateStructWrapper(@class);
            }
        }

        protected override void GenerateUsings()
        {
            PushBlock(CodeBlockKind.Usings);
            WriteLine("using System.Runtime.InteropServices;");
            WriteLine($"using {Module.UtilsNamespace};");
            var fullInteropNamespace =  $"{@CurrentTranslationUnit.FullNamespace}.{TranslationUnit.InteropNamespaceExtension.SubNamespace}";
            WriteLine($"using {fullInteropNamespace};");
            UsedUsings.Add(fullInteropNamespace);

            UsingsBlock = PopBlock();
        }

        private void GenerateStructWrapper(Class @class)
        {
            if (@class.Owner != CurrentTranslationUnit)
            {
                IsEmpty = true;
                return;
            }
            
            if (@class.IsSimpleType && CurrentTranslationUnit.Module.SkipGenerationForSimpleTypes)
            {
                IsEmpty = true;
                return;
            }

            PushBlock(CodeBlockKind.Class);

            GenerateCommentIfNotEmpty(@class.Comment);
            
            var classVisitResult = TypePrinter.VisitClass(@class).ToString();
            if (TargetRuntime == TargetRuntime.Net8Plus)
            {
                WriteLine($"{classVisitResult} : {MarshalObjectInterfaceName}, {MarshalInterfaceName}<{@class.NativeStruct.FullName}>");
            }
            else
            {
                WriteLine($"{classVisitResult} : {MarshalInterfaceName}<{@class.NativeStruct.FullName}>");
            }

            WriteOpenBraceAndIndent();

            GenerateConstructors(@class);

            GenerateWrappedProperties(@class);

            GenerateMethods(@class);

            GenerateOverloads(@class);
            
            GenerateIMarshallableInterface(@class);

            if (TargetRuntime == TargetRuntime.Net8Plus)
            {
                GenerateIMarshallableObjectInterface(@class);
            }

            GenerateStructMarshaller(@class);

            UnindentAndWriteCloseBrace();

            NewLine();

            PopBlock();

            IsEmpty = false;
        }

        private void GenerateIMarshallableInterface(Class @class)
        {
            GenerateGetSizeMethod(@class);
            NewLine();
            GenerateMarshalToMethod(@class);
            NewLine();
            GenerateMarshalFromMethod(@class);
        }

        private void GenerateIMarshallableObjectInterface(Class @class)
        {
            WriteLine($"public void* GetNativePointer<TContext>(ref TContext context) where TContext : IMarshallingContext, allows ref struct");
            WriteOpenBraceAndIndent();
            WriteLine($"var nativeSpan = context.AllocateNative<{@class.NativeStruct.FullName}>(1);");
            WriteLine($"var dataCursor = context.GetDataCursor();");
            WriteLine($"var internalContext = new MarshallingContext<{@class.NativeStruct.FullName}>(nativeSpan, dataCursor);");
            WriteLine($"this.MarshalTo(ref internalContext);");
            WriteLine($"context.SetDataCursor(internalContext.DataCursor);");
            WriteLine($"return {UnsafeClassName}.AsPointer(ref nativeSpan[0]);");
            UnindentAndWriteCloseBrace();
        }

        private void GenerateGetSizeMethod(Class @class)
        {
            WriteLine($"public int GetSize()");
            WriteOpenBraceAndIndent();
            WriteLine($"var size = Marshal.SizeOf<{@class.NativeStruct.FullName}>();");

            foreach (var property in @class.Properties)
            {
                if (property.Type.IsPointerToObject())
                {
                    WriteLine($"if ({property.Name} is IMarshallableObject marshallable)");
                    WriteOpenBraceAndIndent();
                    WriteLine($"size += marshallable.GetSize();");
                    UnindentAndWriteCloseBrace();
                }
                else if (property.Type.IsString())
                {
                    WriteLine($"if (!string.IsNullOrEmpty({property.Name}))");
                    PushIndent();
                    WriteLine($"size += System.Text.Encoding.UTF8.GetByteCount({property.Name}) + 1;");
                    PopIndent();
                }
                else if (property.Type.IsStringArray())
                {
                    if (TargetRuntime == TargetRuntime.Net8Plus)
                    {
                        WriteLine(
                            $"size += QuantumBinding.Utils.MarshalContextUtils.CalculateRequiredSizeForStringArray({property.Name}.Span);");
                    }
                    else
                    {
                        WriteLine($"size += MarshalContextUtils.CalculateRequiredSizeForStringArray({property.Name});");
                    }
                }
                else if (property.Type.IsPointerToArray())
                {
                    if (property.Type.IsConstArray())
                        continue;
                    
                    var declaration = property.Type.Declaration as Class;
                    if (declaration is { IsWrapper: true })
                    {
                        if (TargetRuntime == TargetRuntime.Net8Plus)
                        {
                            WriteLine($"if (!{property.Name}.IsEmpty)");
                        }
                        else
                        {
                            WriteLine($"if ({property.Name} is not null)");
                        }
                        WriteOpenBraceAndIndent();
                        WriteLine($"for (int i = 0; i < {property.Name}.Length; i++)");
                        WriteOpenBraceAndIndent();

                        if (TargetRuntime == TargetRuntime.Net8Plus)
                        {
                            WriteLine($"if ({property.Name}.Span[i] == null)");
                        }
                        else
                        {
                            WriteLine($"if ({property.Name}[i] == null)");
                        }

                        PushIndent();
                        WriteLine($"size += Marshal.SizeOf<{declaration.NativeStruct.FullName}>();");
                        PopIndent();
                        WriteLine("else");
                        PushIndent();
                        WriteLine(TargetRuntime == TargetRuntime.Net8Plus
                            ? $"size += {property.Name}.Span[i].GetSize();"
                            : $"size += {property.Name}[i].GetSize();");
                        PopIndent();
                        UnindentAndWriteCloseBrace();
                        UnindentAndWriteCloseBrace();
                    }
                    else if (declaration is { ClassType: ClassType.Class })
                    {
                        if (TargetRuntime == TargetRuntime.Net8Plus)
                        {
                            WriteLine($"if (!{property.Name}.IsEmpty)");
                            PushIndent();
                            WriteLine($"size += {property.Name}.Span.Length * Marshal.SizeOf<{declaration.NativeStruct.FullName}>();");
                            PopIndent();
                        }
                        else
                        {
                            WriteLine($"if ({property.Name} is not null)");
                            PushIndent();
                            WriteLine($"size += {property.Name}.Length * Marshal.SizeOf<{declaration.NativeStruct.FullName}>();");
                            PopIndent();
                        }
                    }
                    else if (property.Type.IsPointerToArrayOfHandleWrappers())
                    {
                        if (TargetRuntime == TargetRuntime.Net8Plus)
                        {
                            WriteLine($"if (!{property.Name}.IsEmpty)");
                        }
                        else
                        {
                            WriteLine($"if ({property.Name} is not null)");
                        }
                        
                        PushIndent();
                        Write(TargetRuntime == TargetRuntime.Net8Plus
                            ? $"size += {property.Name}.Span.Length"
                            : $"size += {property.Name}.Length");
                        Write($" * Marshal.SizeOf<{declaration.NativeStruct.FullName}>();");
                        NewLine();
                        PopIndent();
                    }
                    else if (property.Type.IsPointerToArrayOfEnums())
                    {
                        if (TargetRuntime == TargetRuntime.Net8Plus)
                        {
                            WriteLine($"if (!{property.Name}.IsEmpty)");
                        }
                        else
                        {
                            WriteLine($"if ({property.Name} is not null)");
                        }
                        
                        var decl = property.Type.Declaration as Enumeration;
                        PushIndent();
                        Write(TargetRuntime == TargetRuntime.Net8Plus
                            ? $"size += {property.Name}.Span.Length"
                            : $"size += {property.Name}.Length");
                        Write($" * sizeof({decl.InheritanceType});");
                        NewLine();
                        PopIndent();
                    }
                    else if (property.Type.IsPointerToArrayOfPrimitiveTypes(out var primitiveType))
                    {
                        if (TargetRuntime == TargetRuntime.Net8Plus)
                        {
                            WriteLine($"if (!{property.Name}.IsEmpty)");
                        }
                        else
                        {
                            WriteLine($"if ({property.Name} is not null)");
                        }
                        
                        PushIndent();
                        Write(TargetRuntime == TargetRuntime.Net8Plus
                            ? $"size += {property.Name}.Span.Length"
                            : $"size += {property.Name}.Length");
                        Write($" * Marshal.SizeOf<{primitiveType.Type.GetDisplayName()}>();");
                        NewLine();
                        PopIndent();
                    }
                }
                else if (property.Type.IsPointerToWrapper(out var wrapper))
                {
                    WriteLine($"if ({property.Name} != default)");
                    WriteOpenBraceAndIndent();
                    WriteLine($"size += {property.Name}.GetSize();");
                    UnindentAndWriteCloseBrace();
                }
                else if (property.Type.IsPointerToSimpleType())
                {
                    WriteLine($"if ({property.Name} != default)");
                    WriteOpenBraceAndIndent();
                    WriteLine($"size += Marshal.SizeOf<{property.Type.Declaration.FullName}>();");
                    UnindentAndWriteCloseBrace();
                }
            }
            WriteLine($"return size;");
            UnindentAndWriteCloseBrace();
        }

        private void GenerateMarshalToMethod(Class @class)
        {
            WriteLine($"public void MarshalTo(ref MarshallingContext<{@class.NativeStruct.FullName}> context)");
            WriteOpenBraceAndIndent();
            WriteLine($"new {@class.MarshalerStructName}(this, ref context);");
            UnindentAndWriteCloseBrace();
        }

        private void GenerateMarshalFromMethod(Class @class)
        {
            WriteLine($"public void MarshalFrom(in {@class.NativeStruct.FullName} {@class.NativeStructFieldName})");
            WriteOpenBraceAndIndent();
            GenerateNativeToManagedCode(@class);
            UnindentAndWriteCloseBrace();
        }

        private void GenerateStructMarshaller(Class @class)
        {
            WriteLine($"private ref struct {@class.MarshalerStructName}");
            WriteOpenBraceAndIndent();
            GenerateMarshallerCtor(@class);
            UnindentAndWriteCloseBrace();
        }

        private void GenerateMarshallerCtor(Class @class)
        {
            string contextName = "context";
            @class.InputClassName = $"{@class.Name[0].ToString().ToLower()}{@class.Name.Substring(1)}";
            WriteLine($"public {@class.MarshalerStructName}({@class.FullName} {@class.InputClassName}, ref {MarshalContextClassName}<{@class.NativeStruct.FullName}> context)");
            WriteOpenBraceAndIndent();
            GenerateManagedToNativeCode(@class, contextName);
            UnindentAndWriteCloseBrace();
        }

        protected virtual void GenerateNativeToManagedCode(Class @class)
        {
            PushBlock(CodeBlockKind.Constructor);
            int constArrayIndex = 0;
            foreach (var property in @class.Properties)
            {
                TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
                var propertyTypeName = property.Type.Visit(TypePrinter);
                TypePrinter.PopMarshalType();

                var decl = property.Type.Declaration as Class;

                if (property.Field.HasPredefinedValue)
                {
                    if (!property.Field.IsPredefinedValueReadOnly)
                    {
                        WriteLine($"{property.Name} = {property.Field.PredefinedValue};");
                    }

                    continue;
                }

                if (property.Type.IsPointer())
                {
                    if (property.Type.IsStringArray(out var isUnicode))
                    {
                        MarshalPointerToStringArray(property, @class, isUnicode);
                    }
                    else if (property.Type.IsAnsiString() || property.Type.IsUnicodeString())
                    {
                        WriteLine($"{property.Name} = new string({@class.NativeStructFieldName}.{property.Field.Name});");
                    }
                    else if (property.Type.IsPointerToEnum() || property.Type.IsEnum())
                    {
                        MarshalPointerToEnum(property, @class);
                    }
                    else if (property.Type.IsPointerToArray())
                    {
                        MarshalFromPointerToArray(property, @class, constArrayIndex++);
                    }
                    else if (property.Type.IsPointerToObject())
                    {
                        WriteLine($"{property.Name} = (System.IntPtr){@class.NativeStructFieldName}.{property.Field.Name};");
                    }
                    else if (property.Type.IsDoublePointer() ||
                             property.Type.IsPointerToVoid() ||
                             property.Type.IsPointerToIntPtr() ||
                             property.Type.IsPointerToSystemType(out var systemType))
                    {
                        WriteLine($"{property.Name} = {@class.NativeStructFieldName}.{property.Field.Name};");
                    }
                    else // pointer to struct and pointer to simple types
                    {
                        MarshalPointerToStruct(property, @class, decl, propertyTypeName.Type);
                    }
                }
                else if (property.Type.IsConstArray(out var size))
                {
                    MarshalFixedArrayToManaged(property, @class, decl, propertyTypeName);
                }
                else if (decl is { IsSimpleType: false })
                {
                    WriteLine($"{property.Name} = new {propertyTypeName}({@class.NativeStructFieldName}.{property.Field.Name});");
                }
                else if (property.Type.Declaration is Enumeration)
                {
                    WriteLine($"{property.Name} = {@class.NativeStructFieldName}.{property.Field.Name};");
                }
                else
                {
                    if (property.Type is BuiltinType builtinType)
                    {
                        FromNativeToManaged(builtinType);
                    }
                    else if (decl is { IsSimpleType: true } && Options.PodTypesAsSimpleTypes)
                    {
                        if (decl.UnderlyingNativeType is BuiltinType primitiveType)
                        {
                            FromNativeToManaged(primitiveType);
                        }
                    }
                    else
                    {
                        WriteLine($"{property.Name} = {@class.NativeStructFieldName}.{property.Field.Name};");
                    }

                    void FromNativeToManaged(BuiltinType builtin)
                    {
                        switch (builtin.Type)
                        {
                            case PrimitiveType.Bool:
                            case PrimitiveType.Bool32:
                                WriteLine(
                                    $"{property.Name} = System.Convert.ToBoolean({@class.NativeStructFieldName}.{property.Field.Name});");
                                break;
                            default:
                                WriteLine($"{property.Name} = {@class.NativeStructFieldName}.{property.Field.Name};");
                                break;
                        }
                    }
                }
            }
            NewLine();

            PopBlock();
        }

        protected virtual void GenerateManagedToNativeCode(Class @class, string contextName)
        {
            int structIndex = 0;
            int pointerArrayIndex = 0;
            int constArrayIndex = 0;

            PushBlock(CodeBlockKind.Method);
            foreach (var property in @class.Properties)
            {
                if (property.Setter == null) continue;

                var decl = property.Type.Declaration as Class;
                if (property.Setter == null)
                {
                    continue;
                }

                if (property.Type.IsPointer())
                {
                    if (property.Type.IsPointerToVoid() || property.Type.IsPointerToIntPtr() || property.Type.IsPointerToSystemType(out var systemType))
                    {
                        WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                    }
                    else if (property.Type.IsPointerToArrayOfEnums() || property.Type.IsPointerToEnum())
                    {
                        MarshalPointerToEnum(property, @class, contextName);
                    }
                    else
                    {
                        if (property.Type.IsPointerToObject())
                        {
                            if (TargetRuntime == TargetRuntime.Net8Plus)
                            {
                                WriteLine(
                                    $"if ({@class.InputClassName}.{property.Name} is IMarshallableObject marshallable)");
                                WriteOpenBraceAndIndent();
                                WriteLine(
                                    $"{contextName}.Destination[0].{property.Field.Name} = marshallable.GetNativePointer(ref {contextName});");
                                UnindentAndWriteCloseBrace();
                                WriteLine($"else if ({@class.InputClassName}.{property.Name} is System.IntPtr ptr)");
                                WriteOpenBraceAndIndent();
                                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = (void*)ptr;");
                                UnindentAndWriteCloseBrace();
                            }
                            else
                            {
                                WriteLine($"if ({@class.InputClassName}.{property.Name} is System.IntPtr ptr)");
                                WriteOpenBraceAndIndent();
                                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = (void*)ptr;");
                                UnindentAndWriteCloseBrace();
                            }

                            NewLine();
                            continue;
                        }
                        if ((property.Type.IsPointerToSimpleType() ||
                            property.Type.IsPointerToBuiltInType(out var type)) &&
                            !property.Type.IsPointerToArray())
                        {
                            WriteLine($"if ({@class.InputClassName}.{property.Name}.HasValue)");
                        }
                        else if (property.Type.IsPointerToArray())
                        {
                            if (TargetRuntime == TargetRuntime.Net8Plus)
                            {
                                WriteLine($"if (!{@class.InputClassName}.{property.Name}.IsEmpty)");
                            }
                            else
                            {
                                WriteLine($"if ({@class.InputClassName}.{property.Name} != default && {@class.InputClassName}.{property.Name}.{CounterProperty} > 0)");
                            }
                        }
                        else
                        {
                            WriteLine($"if ({@class.InputClassName}.{property.Name} != default)");
                        }
                        
                        WriteOpenBraceAndIndent();
                        
                        if (property.Type.IsPointerToString())
                        {
                            if (property.Type.IsStringArray())
                            {
                                MarshalStringArrayToPointer(property, @class, contextName);
                            }
                            else
                            {
                                MarshalStringToPointer(property, @class, contextName);
                            }
                        }
                        else if (property.Type.IsPointerToArray())
                        {
                            MarshalFromArrayToPointer(property, @class, contextName);
                        }
                        else if (property.Type.IsPointerToIntPtr())
                        {
                            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
                            var fieldResult = TypePrinter.VisitProperty(property);
                            TypePrinter.PopMarshalType();
                            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({fieldResult}){@class.InputClassName}.{property.Name};");
                        }
                        else if (property.Type.IsDoublePointer() || property.Type.IsPointerToVoid())
                        {
                            WriteLine(
                                $"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                        }
                        else // pointer to struct and pointer to simple types
                        {
                            MarshalStructToPointer(property, @class, contextName, structIndex);
                        }

                        UnindentAndWriteCloseBrace();
                    }
                }
                else
                {
                    if (property.Type.IsConstArray(out var size))
                    {
                        MarshalConstArrayToNative(property, @class, decl, contextName, ref pointerArrayIndex);
                        constArrayIndex++;
                    }
                    else if (decl is { ClassType: ClassType.Class, IsSimpleType: false })
                    {
                        WriteDefaultCondition(() =>
                        {
                            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                        });
                    }
                    else if (decl is { ClassType: ClassType.StructWrapper or ClassType.UnionWrapper })
                    {
                        WriteDefaultCondition(() =>
                        {
                            string childContext = "childContext";
                            var nativeDecl = property.Field.Type.Declaration as Class;
                            
                            WriteLine($"fixed ({nativeDecl.FullName}* pField = &{contextName}.Destination[0].{property.Field.Name})");
                            WriteOpenBraceAndIndent();
                            WriteLine($"var fieldSpan = new {SpanClassName}<{nativeDecl.FullName}>(pField, 1);");
                            WriteLine($"var childContext = new MarshallingContext<{nativeDecl.FullName}>(fieldSpan, {contextName}.DataCursor);");
                            WriteLine($"{@class.InputClassName}.{property.Name}.{MarshalToMethodName}(ref childContext);");
                            WriteLine($"{contextName}.DataCursor = childContext.DataCursor;");
                            UnindentAndWriteCloseBrace();
                        });
                    }
                    else if (property.Type.Declaration is Enumeration @enum)
                    {
                        WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                    }
                    else
                    {
                        if (property.Type is BuiltinType builtinType)
                        {
                            MarshalFromWrappedToNative(builtinType);
                        }
                        else if (decl is { IsSimpleType: true } && Options.PodTypesAsSimpleTypes)
                        {
                            if (decl.UnderlyingNativeType is BuiltinType primitiveType)
                            {
                                MarshalFromWrappedToNative(primitiveType);
                            }
                        }
                        else
                        {
                            // We will convert all Simple types to their native underlying types because 
                            // for some types we could have implicit casting operators, 
                            // and this will prevent compiler for correctly using 'default' keyword.
                            // To fix this, we are casting to concrete type
                            if (decl is { IsSimpleType: true } && !Options.PodTypesAsSimpleTypes)
                            {
                                var nativeTypeName = decl.UnderlyingNativeType.Visit(TypePrinter);
                                WriteLine($"if ({@class.InputClassName}.{property.Name} != ({nativeTypeName})default)");
                                WriteOpenBraceAndIndent();
                                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                                UnindentAndWriteCloseBrace();
                            }
                            else
                            {
                                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                            }
                        }

                        void MarshalFromWrappedToNative(BuiltinType builtin)
                        {
                            switch (builtin.Type)
                            {
                                case PrimitiveType.Bool:
                                    WriteLine($"{contextName}.Destination[0].{property.Field.Name} = System.Convert.ToByte({@class.InputClassName}.{property.Name});");
                                    break;
                                case PrimitiveType.Bool32:
                                    WriteLine($"{contextName}.Destination[0].{property.Field.Name} = System.Convert.ToUInt32({@class.InputClassName}.{property.Name});");
                                    break;
                                default:
                                    WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {@class.InputClassName}.{property.Name};");
                                    break;
                            }
                        }
                    }
                }
                
                NewLine();
                void WriteDefaultCondition(Action action)
                {
                    WriteLine($"if ({@class.InputClassName}.{property.Name} != default)");
                    WriteOpenBraceAndIndent();
                    action?.Invoke();
                    UnindentAndWriteCloseBrace();
                }
            }

            PopBlock();
        }

        public override bool IsInteropGenerator => false;

        protected override void GenerateConstructor(Constructor ctor)
        {
            var @class = ctor.Class;
            PushBlock(CodeBlockKind.Constructor, ctor);
            if (ctor.IsDefault)
            {
                WriteLine($"{ctor.AccessSpecifier.ToString().ToLower()} {ctor.Class.Name}()");
                WriteOpenBraceAndIndent();
                foreach(var property in @class.Properties)
                {
                    if (property.Field.HasPredefinedValue && !property.Field.IsPredefinedValueReadOnly)
                    {
                        WriteLine($"{property.Name} = {property.Field.PredefinedValue};");
                    }
                }
                UnindentAndWriteCloseBrace();
            }
            else
            {
                Write($"{ctor.AccessSpecifier.ToString().ToLower()} {ctor.Class.Name}(");

                int index = 0;
                TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                foreach (var param in ctor.InputParameters)
                {
                    index++;
                    var visitResult = TypePrinter.VisitParameter(param);
                    Write($"{visitResult}");

                    if (index < ctor.InputParameters.Count)
                    {
                        Write(",");
                    }
                }

                TypePrinter.PopMarshalType();
                Write(")");
                NewLine();
                WriteOpenBraceAndIndent();
                TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                
                foreach (var param in ctor.InputParameters)
                {
                    var visitResult = TypePrinter.VisitParameter(param);
                    if (visitResult.Type == ctor.Class.NativeStruct.FullName)
                    {
                        if (string.IsNullOrEmpty(visitResult.ParameterModifier))
                        {
                            WriteLine($"{MarshalFromMethodName}({param.Name});");
                        }
                        else
                        {
                            WriteLine($"{MarshalFromMethodName}({visitResult.ParameterModifier} {param.Name});");
                        }
                    }
                    else
                    {
                        WriteLine($"this.{param.Name} = {param.Name};");
                    }
                }
                TypePrinter.PopMarshalType();
                UnindentAndWriteCloseBrace();
            }

            NewLine();

            PopBlock();
        }

        protected override void GenerateFields(Class @class)
        {
            try
            {
                TypePrinter.PushMarshalType(MarshalTypes.Property);
                foreach (var field in @class.Fields)
                {
                    PushBlock(CodeBlockKind.Field, field);

                    GenerateCommentIfNotEmpty(field.Comment);

                    var fieldStr = TypePrinter.VisitField(field);
                    
                    var typeResult = field.Type.Visit(TypePrinter);

                    if (typeResult.Type == @class.Name)
                    {
                        var splittedStr = fieldStr.Type.Split(' ');
                        splittedStr[1] =
                            $"{field.Type.Declaration.Owner.FullNamespace}.{interopNamespace}.{splittedStr[1]}";
                        fieldStr.Type = string.Join(' ', splittedStr);
                    }

                    AddUsingIfNeeded(field.Type);

                    PushBlock(CodeBlockKind.FieldDefinition, field);
                    WriteLine($"{fieldStr.Type};");

                    PopBlock();

                    NewLine();

                    PopBlock();
                }

                TypePrinter.PopMarshalType();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OperatorOverloadOverride(Operator @operator, string variableName)
        {
            if (@operator.TransformationKind == TransformationKind.FromValueToClass)
            {
                return;
            }

            var @class = @operator.Class;

            foreach(var property in @class.Properties)
            {
                if (property.Type.IsSimpleType() 
                    || property.Type.IsEnum() 
                    || property.Type.IsPointer()
                    || property.Type.IsConstArray()
                    || !property.Type.IsCustomType(out var custom))
                {
                    continue;
                }

                WriteLine($"if ({variableName}.{property.Name} != null)");
                WriteOpenBraceAndIndent();
                WriteLine($"{variableName}.{@class.NativeStructFieldName}.{property.Field.Name} = {variableName}.{property.Name};");
                UnindentAndWriteCloseBrace();
            }
        }

        private void GeneratePropertyField(Property property)
        {
            PushBlock(CodeBlockKind.Field, property.Field);

            var access = property.Field.AccessSpecifier;
            property.Field.AccessSpecifier = AccessSpecifier.Private;
            TypePrinter.PushMarshalType(MarshalTypes.Property);

            var originalName = property.Field.Name;
            var name = originalName[0].ToString().ToLower() + originalName.Substring(1);
            property.Field.Name = name;
            var fieldStr = TypePrinter.VisitField(property.Field);
            property.Field.Name = originalName;

            TypePrinter.PopMarshalType();
            property.Field.AccessSpecifier = access;
            AddUsingIfNeeded(property.Field.Type);

            PushBlock(CodeBlockKind.FieldDefinition, property.Field);
            WriteLine($"{fieldStr.Type};");
            PopBlock();

            PopBlock();
        }

        private void GenerateWrappedProperties(Class @class)
        {
            foreach (var property in @class.Properties)
            {
                AddUsingIfNeeded(property.Type);

                PushBlock(CodeBlockKind.Property);
                GenerateCommentIfNotEmpty(property.Comment);
                TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
                var propertyTypeName = property.Type.Visit(TypePrinter);
                TypePrinter.PopMarshalType();

                PushBlock(CodeBlockKind.AccessSpecifier);
                Write($"{TypePrinter.GetAccessSpecifier(property.AccessSpecifier)}");
                PopBlock(NewLineStrategy.SpaceBeforeNextBlock);

                // Here we are creating only getter because we don't want this property to be changed outside
                if (property.Field.HasPredefinedValue)
                {
                    if (property.Field.IsPredefinedValueReadOnly)
                    {
                        WriteLine($"{propertyTypeName} {property.Name} => {property.Field.PredefinedValue};");
                        PopBlock();
                        continue;
                    }
                }

                Write($"{propertyTypeName} {property.Name}");
                var getterAccessSpecifier = TypePrinter.GetAccessSpecifier(property.Getter?.AccessSpecifier);
                var setterAccessSpecifier = TypePrinter.GetAccessSpecifier(property.Setter?.AccessSpecifier);

                Write(" { ");

                if (property.Getter != null)
                {
                    if (property.Getter.AccessSpecifier != AccessSpecifier.Public)
                    {
                        Write($"{getterAccessSpecifier} ");
                    }
                    Write($"get; ");
                }
                if (property.Setter != null)
                {
                    if (property.Setter.AccessSpecifier != AccessSpecifier.Public)
                    {
                        Write($"{setterAccessSpecifier} ");
                    }
                    Write("set;");
                }
                Write(" }");

                NewLine();

                PopBlock();
            }
        }

        private void MarshalPointerToEnum(Property property, Class parentClass)
        {
            var internalFieldName = $"{parentClass.NativeStructFieldName}.{property.Field.Name}";
            if (property.Type.IsPointerToArrayOfEnums())
            {
                var pointerType = property.Type as PointerType;
                if (pointerType == null) return;
                var arrayType = pointerType.Pointee as ArrayType;
                if (string.IsNullOrEmpty(arrayType.ArraySizeSource))
                    return;
                
                string tmpPropertyName = $"tmp{property.Name}";
                WriteLine($"var {tmpPropertyName} = new {arrayType.ElementType}[{parentClass.NativeStructFieldName}.{arrayType.ArraySizeSource}];");
                WriteLine($"{TextGenerator.MarshalFromPointerToArray}({internalFieldName}, {parentClass.NativeStructFieldName}.{arrayType.ArraySizeSource}, {tmpPropertyName});");
                WriteLine($"{property.Name} = {tmpPropertyName};");
            }
            else
            {
                WriteLine($"{property.Name} = *{internalFieldName};");
            }
        }
        
        private void MarshalPointerToEnum(Property property, Class parentClass, string contextName)
        {
            var isPointerToArray = property.Type.IsPointerToArrayOfEnums();
            var decl = property.Type.Declaration as Enumeration;
            
            string enumSpanName = $"enumSpan{property.Name}";
            string enumValueName = $"enumValue{property.Name}";
            
            if (isPointerToArray)
            {
                WriteLine(TargetRuntime == TargetRuntime.Net8Plus
                    ? $"if (!{parentClass.InputClassName}.{property.Name}.IsEmpty)"
                    : $"if ({parentClass.InputClassName}.{property.Name} != null && {parentClass.InputClassName}.{property.Name}.{CounterProperty} > 0)");

                WriteOpenBraceAndIndent();
                WriteLine($"var sizeInBytes = sizeof({decl.InheritanceType}) * {parentClass.InputClassName}.{property.Name}.{CounterProperty};");
                WriteLine($"var byteSpan = {contextName}.AllocateData(sizeInBytes);");
                WriteLine($"var {enumSpanName} = {MemoryMarshalClassName}.Cast<byte, {decl.InheritanceType}>(byteSpan);");
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({decl.FullName}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({enumSpanName}));");
                WriteLine($"for (int i = 0; i < {enumSpanName}.Length; i++)");
                WriteOpenBraceAndIndent();
                var parameterName = TargetRuntime == TargetRuntime.Net8Plus ? $"{parentClass.InputClassName}.{property.Name}.Span" : $"{parentClass.InputClassName}.{property.Name}";
                WriteLine($"{enumSpanName}[i] = ({decl.InheritanceType}){parameterName}[i];");
                UnindentAndWriteCloseBrace();
                UnindentAndWriteCloseBrace();
            }
            else
            {
                WriteLine($"var {enumSpanName} = {contextName}.AllocateData(sizeof({decl.InheritanceType}));");
                WriteLine($"ref {decl.InheritanceType} {enumValueName} = ref {MemoryMarshalClassName}.AsRef<{decl.InheritanceType}>({enumSpanName});");
                WriteLine($"{enumValueName} = ({decl.InheritanceType}){parentClass.InputClassName}.{property.Name};");
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({decl.FullName}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({enumSpanName}));");
            }
        }

        private void MarshalFromPointerToArray(Property property, Class parentClass, int index)
        {
            var pointerType = property.Type as PointerType;
            if (pointerType == null) return;
            var arrayType = pointerType.Pointee as ArrayType;
            if (string.IsNullOrEmpty(arrayType.ArraySizeSource))
                return;

            var arrayPtr = $"{parentClass.NativeStructFieldName}.{property.Field.Name}";
            var arraySizeFieldName = $"{parentClass.NativeStructFieldName}.{arrayType.ArraySizeSource}";
            TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
            arrayType.ElementType.Declaration = property.Type.Declaration;
            var arrayElementType = arrayType.ElementType.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
            var nativeArrayType = property.Field.Type.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            string tempArrayName = $"tmp{property.Name}";
            WriteLine($"var {tempArrayName} = new {arrayElementType}[{arraySizeFieldName}];");

            var nativeArrayName = $"nativeTmpArray{index}";
            if (pointerType.Declaration is Class { IsSimpleType: false } classDecl)
            {
                WriteLine($"var {nativeArrayName} = new {parentClass.NativeStruct.Namespace}.{nativeArrayType.Type}[{arraySizeFieldName}];");
                WriteLine($"{TextGenerator.MarshalFromPointerToArray}({arrayPtr}, {arraySizeFieldName}, {nativeArrayName});");
                WriteLine($"for (int i = 0; i < {nativeArrayName}.{CounterProperty}; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{tempArrayName}[i] = new {classDecl.Name}(in {nativeArrayName}[i]);");
                UnindentAndWriteCloseBrace();
                WriteLine($"{property.Name} = {tempArrayName};");
            }
            else if (arrayType.ElementType.IsPrimitiveType(out var primitive) && 
                     primitive is PrimitiveType.Bool32 or PrimitiveType.Bool)
            {
                WriteLine($"var {nativeArrayName} = new {nativeArrayType}[{arraySizeFieldName}];");
                WriteLine($"{TextGenerator.MarshalFromPointerToArray}({arrayPtr}, {arraySizeFieldName}, {nativeArrayName});");
                WriteLine($"for (int i = 0; i < {nativeArrayName}.{CounterProperty}; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{tempArrayName}[i] = System.Convert.ToBoolean({nativeArrayName}[i]);");
                UnindentAndWriteCloseBrace();
                WriteLine($"{property.Name} = {tempArrayName};");
            }
            else
            {
                WriteLine($"{TextGenerator.MarshalFromPointerToArray}({arrayPtr}, {arraySizeFieldName}, {tempArrayName});");
            }
        }

        private void WriteFreeMemory(string pointer)
        {
            WriteLine($"NativeUtils.Free({pointer});");
        }

        private void MarshalPointerToStringArray(Property property, Class parentClass, bool isUnicode)
        {
            var pointer = property.Type as PointerType;
            var array = pointer.Pointee as ArrayType;
            if (array != null && !string.IsNullOrEmpty(array.ArraySizeSource))
            {
                var arrayName = $"{parentClass.NativeStructFieldName}.{property.Field.Name}";
                var arraySizeFieldName = $"{parentClass.NativeStructFieldName}.{array.ArraySizeSource}";
                WriteLine($"{property.Name} = {TextGenerator.MarshalPointerToStringArray}({arrayName}, (uint){arraySizeFieldName});");
            }
        }

        private void MarshalFixedArrayToManaged(Property property, Class parentClass, Class decl, TypePrinterResult propertyTypeName)
        {
            var arrayType = property.Type as ArrayType;
            var size = arrayType.Size;
            
            TypePrinter.PushMarshalType(MarshalTypes.Property);
            var managedType = property.Type.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            
            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
            var nativeType = property.Field.Type.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            
            if (arrayType.ElementType.CanConvertToFixedArray())
            {
                var arrayName = $"{parentClass.NativeStructFieldName}.{property.Field.Name}";

                if (propertyTypeName.Type == "string")
                {
                    if (property.Type.IsAnsiString()
                        || arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar))
                    {
                        WriteLine($"fixed(sbyte* pSource = {arrayName})");
                        WriteOpenBraceAndIndent();
                        WriteLine($"{property.Name} = {TextGenerator.MarshalFixedByteArrayToString}(pSource, {size});");
                        UnindentAndWriteCloseBrace();
                        
                    }
                    else if (property.Type.IsUnicodeString() || arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        WriteLine($"fixed(char* pSource = {arrayName})");
                        WriteOpenBraceAndIndent();
                        WriteLine($"{property.Name} = {TextGenerator.MarshalFixedCharArrayToString}({arrayName}, {size});");
                        UnindentAndWriteCloseBrace();
                    }
                }
                else
                {
                    string tempArrayName = $"tmp{property.Name}";
                
                    WriteLine($"var {tempArrayName} = new {managedType.Type}[{size}];");
                    WriteLine($"var p{property.Name} = ({managedType.Type}*){UnsafeClassName}.AsPointer(ref {UnsafeClassName}.AsRef(in {parentClass.NativeStructFieldName}.{property.Field.Name}[0]));");
                    WriteLine($"{TextGenerator.MarshalFromPointerToArray}(p{property.Name}, {size}, {tempArrayName});");
                    WriteLine($"{property.Name} = {tempArrayName};");
                }
            }
            else if (arrayType.ElementType.IsPurePointer())
            {
                var propertyArrayElementType = arrayType.ElementType.Visit(TypePrinter);
                string tempArrayName = $"tmp{property.Name}";
                WriteLine($"var {tempArrayName} = new {propertyArrayElementType}[{size}];");
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{property.Name}[i] = {parentClass.NativeStructFieldName}.{property.Field.Name}[i];");
                UnindentAndWriteCloseBrace();
                WriteLine($"{property.Name} = {tempArrayName};");
            }
            else if (arrayType.Declaration is Enumeration)
            {
                string tempArrayName = $"tmp{property.Name}";
                WriteLine($"var {tempArrayName} = new {arrayType.Declaration.Name}[{size}];");
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                var typeName = arrayType.Declaration.Name;
                WriteLine($"{tempArrayName}[i] = ({typeName})({parentClass.NativeStructFieldName}.{property.Field.Name}[i]);");
                UnindentAndWriteCloseBrace();
                WriteLine($"{property.Name} = {tempArrayName};");
            }
            else //const array of structs
            {
                TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
                arrayType.ElementType.Declaration = decl;
                var propertyArrayElementType = arrayType.ElementType.Visit(TypePrinter);
                arrayType.ElementType.Declaration = null;
                TypePrinter.PopMarshalType();
                
                string tempArrayName = $"tmp{property.Name}";

                WriteLine($"var {tempArrayName} = new {propertyArrayElementType}[{size}];");
                WriteLine($"var p{property.Name} = ({nativeType.Type}*){UnsafeClassName}.AsPointer(ref {UnsafeClassName}.AsRef(in {parentClass.NativeStructFieldName}.{property.Field.Name}));");

                if (decl.LinkedTo is { ClassType: ClassType.Class })
                {
                    string spanName = $"span{property.Name}";
                    WriteLine($"var {spanName} = new {ReadonlySpanClassName}<{decl.FullName}>(p{property.Name}, {size});");
                    WriteLine($"for (int i = 0; i < {size}; ++i)");
                    WriteOpenBraceAndIndent();
                    WriteLine($"{tempArrayName}[i] = new {decl.LinkedTo.FullName}({spanName}[i]);");
                    UnindentAndWriteCloseBrace();
                }
                else
                {
                    WriteLine($"{property.Name} = {TextGenerator.MarshalFromPointerToArrayOfStructs}<{decl.FullName}, {decl.InteropNamespace}.{nativeType.Type}>(p{property.Name}, {size}, {size});");
                }
            }
        }

        private void MarshalPointerToStruct(Property property, Class parentClass, Class decl, string propertyTypeName)
        {
            var structFieldPath = $"{parentClass.NativeStructFieldName}.{property.Field.Name}";

            var pointer = property.Field.Type as PointerType;
            if (pointer == null)
            {
                throw new ArgumentException($"Binding type should be pointer to struct, but type is {property.Field.Type}");
            }

            TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
            var fieldTypeName = property.Field.Type.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            var fullTypeName = fieldTypeName;
            if (decl is { IsSimpleType: false } && 
                !string.IsNullOrEmpty(decl.Namespace)
                && decl.Name == propertyTypeName)
            {
                fullTypeName = $"{decl.Namespace}.{fieldTypeName}";
            }

            if (propertyTypeName == CSharpTypePrinter.ObjectType)
            {
                WriteLine($"{property.Name} = {structFieldPath};");
            }
            else if ((property.Type.IsPointerToBuiltInType(out var primitive) || (decl is { IsSimpleType: true }) ))
            {
                WriteLine($"if ({structFieldPath} != null)");
                WriteOpenBraceAndIndent();
                WriteLine($"{property.Name} = *{structFieldPath};");
                WriteFreeMemory(structFieldPath);
                UnindentAndWriteCloseBrace();
            }
            else
            {
                if (fullTypeName.Type.EndsWith('?'))
                {
                    fullTypeName.Type = fullTypeName.Type.Replace("?", "");
                }
                WriteLine($"{property.Name} = new {propertyTypeName}(in *{structFieldPath});");
                WriteFreeMemory(structFieldPath);
            }
        }
        
        private void MarshalStringToPointer(Property property, Class @class, string contextName)
        {
            var conversionType = property.Type.IsUnicodeString() ? "char" : "sbyte";
            string byteCountName = "byteCount";
            string stringSpanName = "stringSpan";
            WriteLine($"var {byteCountName} = System.Text.Encoding.UTF8.GetByteCount({@class.InputClassName}.{property.Name});");
            WriteLine($"var {stringSpanName} = {contextName}.AllocateData({byteCountName}+1);");
            WriteLine($"{TextGenerator.MarshalStringToFixedUtf8Buffer}({@class.InputClassName}.{property.Name}, {stringSpanName});");
            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({conversionType}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({stringSpanName}));");
        }

        private void MarshalStringArrayToPointer(Property property, Class @class, string contextName)
        {
            var conversionType = property.Type.IsUnicodeString() ? "char" : "sbyte";
            string pointerArraySizeName = "pointerArraySize";
            string pointerSpanName = "pointerSpan";
            var inputSpanName = TargetRuntime == TargetRuntime.Net8Plus ? $"{@class.InputClassName}.{property.Name}.Span" : $"{@class.InputClassName}.{property.Name}";
            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {TextGenerator.MarshalStringArrayToUtf8Buffer}({inputSpanName}, ref {contextName});");
        }
        
        private void MarshalFromArrayToPointer(Property property, Class @class, string contextName)
        {
            var decl = property.Field.Type.Declaration as Class;
            var pointerType = property.Type as PointerType;
            var array = pointerType.Pointee as ArrayType;
            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
            array.ElementType.Declaration = property.Field.Type.Declaration;
            var arrayTypeName = array.ElementType.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            var size = $"{@class.InputClassName}.{property.Name}.{CounterProperty}";
            var castTypeName = arrayTypeName;
            
            if (decl is { IsSimpleType: false })
            {
                var interop = InteropNamespace;
                var structName = decl.Name;
                if (decl.NativeStruct != null)
                {
                    interop = decl.NativeStruct.Namespace;
                    structName = decl.NativeStruct.Name;
                }

                castTypeName = $"{interop}.{structName}";
            }

            string sizeInBytes = "sizeInBytes";
            string byteSpan = "byteSpan";
            if (decl is { ClassType: ClassType.Class } or { IsSimpleType: true})
            {
                var parameterName = TargetRuntime == TargetRuntime.Net8Plus
                    ? $"{@class.InputClassName}.{property.Name}.Span"
                    : $"{@class.InputClassName}.{property.Name}";
                WriteLine($"{ReadonlySpanClassName}<{decl.FullName}> sourceSpan = {parameterName};");
                WriteLine($"var byteSpan = {contextName}.AllocateData(sourceSpan.Length * sizeof({castTypeName}));");
                WriteLine($"var destinationSpan = {MemoryMarshalClassName}.Cast<byte, {castTypeName}>(byteSpan);");
                WriteLine($"for (int i = 0; i < sourceSpan.Length; i++)");
                WriteOpenBraceAndIndent();
                WriteLine($"destinationSpan[i] = sourceSpan[i];");
                UnindentAndWriteCloseBrace();
                
                WriteLine($"var pDestination = ({castTypeName}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference(destinationSpan));");
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = pDestination;");
            }
            else if (property.Type.IsPointerToArrayOfPrimitiveTypes())
            {
                var parameterName = TargetRuntime == TargetRuntime.Net8Plus
                    ? $"{@class.InputClassName}.{property.Name}.Span"
                    : $"{@class.InputClassName}.{property.Name}";
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {TextGenerator.MarshalBlittableArrayToPointer}<{castTypeName}, {@class.NativeStruct.FullName}>({parameterName}, ref {contextName});");
            }
            else
            {
                TypePrinter.PushMarshalType(MarshalTypes.Property);
                array.ElementType.Declaration = property.Type.Declaration;
                var managedArrayTypeName = array.ElementType.Visit(TypePrinter);
                TypePrinter.PopMarshalType();
                
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {TextGenerator.MarshalArrayToPointer}<{@class.Namespace}.{managedArrayTypeName}, {castTypeName}, {@class.NativeStruct.FullName}>({@class.InputClassName}.{property.Name}, ref {contextName});");
            }
            
            /*
            WriteLine($"var {sizeInBytes} = sizeof({castTypeName}) * {@class.InputClassName}.{property.Name}.{CounterProperty};");
            WriteLine($"var {byteSpan} = {contextName}.AllocateData({sizeInBytes});");
            WriteLine($"var {tmpSpanName} = {MemoryMarshalClassName}.Cast<byte, {castTypeName}>({byteSpan});");
            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({castTypeName}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({tmpSpanName}));");

            WriteLine($"for (int i = 0; i < {@class.InputClassName}.{property.Name}.{CounterProperty}; ++i)");
            WriteOpenBraceAndIndent();
            if (decl != null && (decl.ClassType == ClassType.Class || decl.IsSimpleType)
                || property.Type.IsPointerToArrayOfPrimitiveTypes())
            {
                if (property.Type.IsPointerToArrayOfPrimitiveTypes(out var primitive) && primitive.Type == PrimitiveType.Bool32)
                {
                    string conversionName = string.Empty;
                    if (array.ElementType is BuiltinType builtinType)
                    {
                        switch (builtinType.Type)
                        {
                            case PrimitiveType.Bool32:
                            case PrimitiveType.UInt32:
                                conversionName = "System.Convert.ToUInt32";
                                break;
                            case PrimitiveType.Int32:
                                conversionName = "System.Convert.ToInt32";
                                break;
                            case PrimitiveType.Float:
                                conversionName = "System.Convert.ToSingle";
                                break;
                        }
                    }
                    
                    WriteLine($"{tmpSpanName}[i] = ({conversionName}){property.Name}[i];");
                }
                else
                {
                    WriteLine($"{tmpSpanName}[i] = {@class.InputClassName}.{property.Name}[i];");
                }
            }
            else
            {
                string destinationName = "destinationSlice";
                WriteLine($"var {destinationName} = {tmpSpanName}.Slice(i, 1);");
                WriteLine($"var innerContext = new {MarshalContextClassName}<{decl.FullName}>({destinationName}, {contextName}.DataCursor);");
                WriteLine($"{@class.InputClassName}.{property.Name}[i].{MarshalMethodName}(ref innerContext);");
                WriteLine($"{contextName}.DataCursor = innerContext.DataCursor;");
            }
            UnindentAndWriteCloseBrace();
            */
        }
        
        private void MarshalConstArrayToNative(Property property, Class @class, Class decl, string contextName, ref int increment)
        {
            var arrayType = property.Field.Type as ArrayType;
            if (arrayType == null)
            {
                throw new ArgumentException("Type for array is not an ArrayType");
            }

            var size = arrayType.Size;
            arrayType.ElementType.Declaration = arrayType.Declaration;

            TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
            var result = arrayType.ElementType.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            
            var destinationFieldName = $"tmpDestination{increment++}";
            var fieldName = $"{property.Field.Name}";
            if (arrayType.ElementType.CanConvertToFixedArray())
            {
                if (arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                    arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                {
                    var isUnicode = arrayType.ElementType.IsUnicodeString();
                    string destinationSpanName = "destinationSpan";
                    WriteLine($"ref var {destinationFieldName} = ref {contextName}.Destination[0];");
                    WriteLine($"fixed ({result}* pDest = {destinationFieldName}.{fieldName})");
                    WriteOpenBraceAndIndent();
                    string typeName = isUnicode ? "char" : "byte";
                    WriteLine($"var {destinationSpanName} = new {SpanClassName}<{typeName}>(({typeName}*)pDest, {size});");
                    WriteLine($"{TextGenerator.MarshalStringToFixedUtf8Buffer}({@class.InputClassName}.{property.Name}, {destinationSpanName});");
                    UnindentAndWriteCloseBrace();
                }
                else
                {
                    WriteLine($"ref var {destinationFieldName} = ref {contextName}.Destination[0];");
                    WriteLine($"fixed ({result}* pDest = {destinationFieldName}.{fieldName})");
                    WriteOpenBraceAndIndent();
                    WriteLine($"{TextGenerator.MarshalFixedArrayToPointer}({@class.InputClassName}.{property.Name}.Span, pDest, {size});");
                    UnindentAndWriteCloseBrace();
                }
            }
            else if (arrayType.ElementType.IsPurePointer())
            {
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{contextName}.Destination[0].{property.Field.Name}[i] = {property.Name}[i];");
                UnindentAndWriteCloseBrace();
            }
            else if (arrayType.Declaration is Enumeration @enum)
            {
                WriteLine($"for (int i = 0; i < {@class.InputClassName}.{property.Name}.{CounterProperty}; ++i)");
                WriteOpenBraceAndIndent();
                if (TargetRuntime == TargetRuntime.Net8Plus)
                {
                    WriteLine($"{contextName}.Destination[0].{property.Field.Name}[i] = {@class.InputClassName}.{property.Name}.Span[i];");
                }
                else
                {
                    WriteLine($"{contextName}.Destination[0].{property.Field.Name}[i] = {@class.InputClassName}.{property.Name}[i];");
                }
                
                UnindentAndWriteCloseBrace();
            }
            else // Fixed array of structs
            {
                TypePrinter.PushMarshalType(MarshalTypes.Property);
                var propType = arrayType.ElementType.Visit(TypePrinter);
                TypePrinter.PopMarshalType();

                if (decl != null && !string.IsNullOrEmpty(decl.Namespace) &&
                    decl.Name == propType.Type)
                {
                    result = $"{InteropNamespace}.{propType}";
                }

                string fixedFieldName = $"fixedField{increment++}";
                WriteLine($"ref var {fixedFieldName} = ref {contextName}.Destination[0].{property.Field.Name};");
                if (TargetRuntime == TargetRuntime.Net8Plus)
                {
                    WriteLine($"var {fixedFieldName}Span = {@class.InputClassName}.{property.Name}.Span;");
                }
                else
                {
                    WriteLine($"var {fixedFieldName}Span = {@class.InputClassName}.{property.Name}.AsSpan();");
                }

                string pointerName = $"p{property.Name}";
                WriteLine($"var {pointerName} = ({result}*){UnsafeClassName}.AsPointer(ref {fixedFieldName}.item0);");
                if (decl is { LinkedTo.ClassType: ClassType.Class })
                {
                    WriteLine($"{TextGenerator.MarshalArrayOfHandleWrappersToFixedBuffer}({fixedFieldName}Span , {pointerName}, {size});");
                }
                else
                {
                    WriteLine($"{TextGenerator.MarshalArrayOfWrappersToFixedBuffer}({fixedFieldName}Span , {pointerName}, {size}, ref {contextName}.DataCursor);");
                }
            }
        }

        private void MarshalStructToPointer(Property property, Class @class, string contextName, int index)
        {
            var decl = property.Field.Type.Declaration as Class;
            var propDecl = property.Type.Declaration as Class;
            if (property.Type.IsPointerToObject())
            {
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {TextGenerator.MarshalStructToPointer}({@class.InputClassName}.{property.Name}, ref {contextName});");
            }
            else if (decl == null || decl.IsSimpleType || property.Type.IsPointerToBuiltInType(out var type))
            {
                WriteLine($"{contextName}.Destination[0].{property.Field.Name} = {TextGenerator.MarshalStructToPointer}({@class.InputClassName}.{property.Name}.Value, ref {contextName});");
            }
            else if (!decl.IsSimpleType)
            {
                var sliceName = $"structSlice{index}";
                var destinationName = $"structDestination{index}";
                if (decl.ClassType is ClassType.Struct or ClassType.Union)
                {
                    WriteLine($"var {sliceName} = {contextName}.AllocateData(sizeof({decl.FullName}));");
                    WriteLine($"var {destinationName} = {MemoryMarshalClassName}.Cast<byte, {decl.FullName}>({sliceName}).Slice(0, 1);");
                    WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({decl.FullName}*){UnsafeClassName}.AsPointer(ref {destinationName}[0]);");
                    WriteLine($"var childContext = new {MarshalContextClassName}<{decl.FullName}>({destinationName}, {contextName}.DataCursor);");
                    WriteLine($"{@class.InputClassName}.{property.Name}.{MarshalToMethodName}(ref childContext);");
                    WriteLine($"{contextName}.DataCursor = childContext.DataCursor;");
                }
                else
                {
                    if (propDecl != null)
                    {
                        var structName = $"struct{index}";
                        if (propDecl.Name == decl.Name && decl.ClassType != ClassType.Class)
                        {
                            string nativeType = $"{decl.NativeStruct.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.Name}";
                            WriteLine($"{decl.NativeStruct.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.Name} {structName} = {@class.InputClassName}.{property.Name};");
                            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({nativeType}*){UnsafeClassName}.AsPointer(ref {structName});");
                        }
                        else if (decl.ClassType == ClassType.Class && decl.NativeStruct != null)
                        {
                            string nativeType = $"{decl.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.NativeStruct.Name}";
                            WriteLine($"{decl.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.NativeStruct.Name} {structName} = {@class.InputClassName}.{property.Name};");
                            WriteLine($"{contextName}.Destination[0].{property.Field.Name} = ({nativeType}*){UnsafeClassName}.AsPointer(ref {structName});");
                        }
                        else
                        {
                            WriteLine($"{decl.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.Name} {structName} = {@class.InputClassName}.{property.Name};");
                        }
                    }
                }
            }
        }
    }
}
