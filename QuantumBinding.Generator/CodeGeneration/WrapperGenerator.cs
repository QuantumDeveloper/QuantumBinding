using System;
using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class WrapperGenerator : CSharpCodeGenerator
    {
        private string interopNamespace = TranslationUnit.InteropNamespaceExtension.SubNamespace;
        
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
            
            if (@class.IsSimpleType && CurrentTranslationUnit.Module.SkipTypedefsGeneration)
            {
                IsEmpty = true;
                return;
            }

            PushBlock(CodeBlockKind.Class);

            GenerateCommentIfNotEmpty(@class.Comment);

            var classVisitResult = TypePrinter.VisitClass(@class).ToString();
            if (@class.IsDisposable && !string.IsNullOrEmpty(@class.DisposableBaseClass))
            {
                WriteLine($"{classVisitResult} : {@class.DisposableBaseClass}");
            }
            else
            {
                WriteLine(classVisitResult);
            }

            WriteOpenBraceAndIndent();

            GenerateFields(@class);

            GenerateConstructors(@class);

            GenerateWrappedProperties(@class);

            GenerateMethods(@class);

            GenerateConversionMethod(@class);

            GenerateDisposePattern(@class);

            GenerateOverloads(@class);

            UnindentAndWriteCloseBrace();

            NewLine();

            PopBlock();

            IsEmpty = false;
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
                    var visitResult = TypePrinter.VisitField(param);
                    if (param.Type.Declaration is Class decl && !string.IsNullOrEmpty(decl.Namespace))
                    {
                        visitResult.Type = $"{decl.Namespace}.{visitResult.Type}";
                    }

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
                    var visitResult = TypePrinter.VisitField(param).Type.Split(' ')[0];
                    if (visitResult == ctor.Class.WrappedStruct.Name)
                    {
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
                                    WriteStringArrayGetter(property, @class, isUnicode);
                                }
                                else if (property.Type.IsAnsiString() || property.Type.IsUnicodeString())
                                {
                                    WriteLine($"{property.Name} = new string({param.Name}.{property.Field.Name});");
                                }
                                else if (property.Type.IsPointerToEnum() || property.Type.IsEnum())
                                {
                                    WritePointerToEnumGetter(property, @class);
                                }
                                else if (property.Type.IsPointerToArray())
                                {
                                    WritePointerToArrayGetter(property, @class, constArrayIndex++);
                                }
                                else if (property.Type.IsDoublePointer() || 
                                         property.Type.IsPointerToVoid() ||
                                         property.Type.IsPointerToIntPtr() ||
                                         property.Type.IsPointerToSystemType(out var systemType))
                                {
                                    WriteLine($"{property.Name} = {param.Name}.{property.Field.Name};");
                                }
                                else // pointer to struct and pointer to simple types
                                {
                                    WritePointerToStructGetter(property, @class, decl, propertyTypeName.Type);
                                }
                            }
                            else if (property.Type.IsConstArray(out var size))
                            {
                                ConstArrayConversionGetter(property, @class, decl, propertyTypeName);
                            }
                            else if (decl != null && !decl.IsSimpleType)
                            {
                                WriteLine($"{property.Name} = new {propertyTypeName}({param.Name}.{property.Field.Name});");
                            }
                            else if (property.Type.Declaration is Enumeration)
                            {
                                WriteLine($"{property.Name} = {param.Name}.{property.Field.Name};");
                            }
                            else
                            {
                                if (property.Type is BuiltinType builtinType)
                                {
                                    FromNativeToWrapper(builtinType);
                                }
                                else if (decl != null && decl.IsSimpleType && Options.PodTypesAsSimpleTypes)
                                {
                                    if (decl.UnderlyingNativeType is BuiltinType primitiveType)
                                    {
                                        FromNativeToWrapper(primitiveType);
                                    }
                                }
                                else
                                {
                                    WriteLine($"{property.Name} = {param.Name}.{property.Field.Name};");
                                }

                                void FromNativeToWrapper(BuiltinType builtin)
                                {
                                    switch (builtin.Type)
                                    {
                                        case PrimitiveType.Bool:
                                        case PrimitiveType.Bool32:
                                            WriteLine($"{property.Name} = System.Convert.ToBoolean({@class.WrappedStructFieldName}.{property.Field.Name});");
                                            break;
                                        default:
                                            WriteLine($"{property.Name} = {@class.WrappedStructFieldName}.{property.Field.Name};");
                                            break;
                                    }
                                }
                            }
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

        protected virtual void GenerateConversionMethod(Class @class)
        {
            int structIndex = 0;
            int pointerArrayIndex = 0;
            int constArrayIndex = 0;

            PushBlock(CodeBlockKind.Method);
            NewLine();
            WriteLine($"{TypePrinter.GetAccessSpecifier(@class.WrapperMethodAccessSpecifier)} {@class.WrappedStruct.Namespace}.{@class.WrappedStruct.Name} {ConversionMethodName}");
            WriteOpenBraceAndIndent();
            WriteLine($"var {@class.WrappedStructFieldName} = new {@class.WrappedStruct.Namespace}.{@class.WrappedStruct.Name}();");
            foreach (var property in @class.Properties)
            {
                if (property.Setter == null) continue;

                var decl = property.Type.Declaration as Class;
                TypePrinter.PushMarshalType(MarshalTypes.Property);
                var propertyStr = TypePrinter.VisitProperty(property);
                var propertyTypeName = property.Type.Visit(TypePrinter);
                TypePrinterResult pairedFieldType = "";
                if (property.PairedField != null && property.PairedField.Type != null)
                {
                    pairedFieldType = property.PairedField.Type.Visit(TypePrinter);
                }
                TypePrinter.PopMarshalType();

                if (property.Setter == null)
                {
                    continue;
                }

                if (property.Type.IsPointer())
                {
                    if (property.Type.IsPointerToVoid() || property.Type.IsPointerToIntPtr() || property.Type.IsPointerToSystemType(out var systemType))
                    {
                        WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name};");
                    }
                    else if (property.Type.IsPointerToArrayOfEnums() || property.Type.IsPointerToEnum())
                    {
                        WritePointerToEnumSetter(property, @class);
                    }
                    else
                    {
                        WriteLine($"{property.PairedField.Name}.Dispose();");
                        if ((property.Type.IsPointerToSimpleType() ||
                            property.Type.IsPointerToBuiltInType(out var type) ||
                            property.Type.IsPointerToEnum()) &&
                            !property.Type.IsPointerToArray())
                        {
                            WriteLine($"if ({property.Name}.HasValue)");
                        }
                        else
                        {
                            WriteLine($"if ({property.Name} != null)");
                        }
                        
                        WriteOpenBraceAndIndent();
                        
                        if (property.Type.IsPointerToString())
                        {
                            WriteLine($"{property.PairedField.Name} = new {pairedFieldType.Type}({property.Name}, {property.Type.IsUnicodeString().ToString().ToLower()});");
                            var conversionType = property.Type.IsUnicodeString() ? "char" : "sbyte";

                            if (property.Type.IsStringArray())
                            {
                                WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = ({conversionType}**){TextGenerator.NativeUtilsGetPointerToStringArray}((uint){property.Name}.Length, {property.Type.IsUnicodeString().ToString().ToLower()});");
                                WriteLine($"{property.PairedField.Name}.Fill({@class.WrappedStructFieldName}.{property.Field.Name});");
                            }
                            else
                            {
                                WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = ({conversionType}*){property.PairedField.Name};");
                            }
                        }
                        else if (property.Type.IsPointerToArray())
                        {
                            WritePointerToArraySetter(property, pairedFieldType, pointerArrayIndex);
                            pointerArrayIndex++;
                        }
                        else if (property.Type.IsPointerToIntPtr())
                        {
                            WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name};");
                        }
                        else if (property.Type.IsDoublePointer() || property.Type.IsPointerToVoid())
                        {
                            WriteLine($"{property.PairedField.Name} = new {pairedFieldType.Type}({property.Name});");
                        }
                        else // pointer to struct and pointer to simple types
                        {
                            WritePointerToStructSetter(property, pairedFieldType.Type, structIndex++);
                        }

                        if (!property.Type.IsPointerToString())
                        {
                            WriteLine(
                                $"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.PairedField.Name}.Handle;");
                        }
                        
                        UnindentAndWriteCloseBrace();
                    }
                }
                else
                {
                    if (property.Type.IsConstArray(out var size))
                    {
                        WriteLine($"if({property.Name} != null)");
                        WriteOpenBraceAndIndent();
                        ConstArrayConversionSetter(property, @class, decl, constArrayIndex);
                        UnindentAndWriteCloseBrace();
                        constArrayIndex++;
                    }
                    else if (decl != null && (decl.ClassType == ClassType.Class && !decl.IsSimpleType))
                    {
                        WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name};");
                    }
                    else if (decl != null && decl.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper)
                    {
                        WriteLine($"if ({property.Name} != null)");
                        WriteOpenBraceAndIndent();
                        WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name}.{ConversionMethodName};");
                        UnindentAndWriteCloseBrace();
                    }
                    else if (property.Type.Declaration is Enumeration @enum)
                    {
                        WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name};");
                    }
                    else
                    {
                        if (property.Type is BuiltinType builtinType)
                        {
                            FromWrappedToNative(builtinType);
                        }
                        else if (decl != null && decl.IsSimpleType && Options.PodTypesAsSimpleTypes)
                        {
                            if (decl.UnderlyingNativeType is BuiltinType primitiveType)
                            {
                                FromWrappedToNative(primitiveType);
                            }
                        }
                        else
                        {
                            WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name};");
                        }

                        void FromWrappedToNative(BuiltinType builtin)
                        {
                            switch (builtin.Type)
                            {
                                case PrimitiveType.Bool:
                                    WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = System.Convert.ToByte({property.Name});");
                                    break;
                                case PrimitiveType.Bool32:
                                    WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = System.Convert.ToUInt32({property.Name});");
                                    break;
                                default:
                                    WriteLine($"{@class.WrappedStructFieldName}.{property.Field.Name} = {property.Name};");
                                    break;
                            }
                        }
                    }
                }
            }
            WriteLine($"return {@class.WrappedStructFieldName};");
            UnindentAndWriteCloseBrace();

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
                WriteLine($"{variableName}.{@class.WrappedStructFieldName}.{property.Field.Name} = {variableName}.{property.Name};");
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

                // Here we creating only getter because we dont want this property to be changed outside
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

        private void WritePointerToEnumGetter(Property property, Class parentClass)
        {
            var internalFieldName = $"{parentClass.WrappedStructFieldName}.{property.Field.Name}";
            if (property.Type.IsPointerToArrayOfEnums())
            {
                var pointerType = property.Type as PointerType;
                if (pointerType == null) return;
                var arrayType = pointerType.Pointee as ArrayType;
                if (string.IsNullOrEmpty(arrayType.ArraySizeSource))
                    return;
                
                WriteLine($"{property.Name} = {TextGenerator.NativeUtilsPointerToArray}({internalFieldName}, {parentClass.WrappedStructFieldName}.{arrayType.ArraySizeSource});");
            }
            else
            {
                WriteLine($"{property.Name} = *{internalFieldName};");
            }
        }
        
        private void WritePointerToEnumSetter(Property property, Class parentClass)
        {
            TypePrinter.PushMarshalType(MarshalTypes.Property);
            TypePrinterResult pairedFieldType = "";
            if (property.PairedField != null && property.PairedField.Type != null)
            {
                pairedFieldType = property.PairedField.Type.Visit(TypePrinter);
            }
            TypePrinter.PopMarshalType();
            var isPointerToArray = property.Type.IsPointerToArrayOfEnums();
            WriteLine($"{property.PairedField.Name}.Dispose();");
            if (isPointerToArray)
            {
                WriteLine($"if ({property.Name} != null)");
            }
            else
            {
                WriteLine($"if ({property.Name}.HasValue)");
            }
            WriteOpenBraceAndIndent();
            if (isPointerToArray)
            {
                WriteLine($"{property.PairedField.Name} = new {pairedFieldType.Type}({property.Name});");
            }
            else
            {
                WriteLine($"{property.PairedField.Name} = new {pairedFieldType.Type}({property.Name}.Value);");
            }
            
            WriteLine($"{parentClass.WrappedStructFieldName}.{property.Field.Name} = {property.PairedField.Name}.Handle;");
            UnindentAndWriteCloseBrace();
        }

        private void WritePointerToArrayGetter(Property property, Class parentClass, int index)
        {
            var pointerType = property.Type as PointerType;
            if (pointerType == null) return;
            var arrayType = pointerType.Pointee as ArrayType;
            if (string.IsNullOrEmpty(arrayType.ArraySizeSource))
                return;

            var arrayPtr = $"{parentClass.WrappedStructFieldName}.{property.Field.Name}";
            var arraySizeFieldName = $"{parentClass.WrappedStructFieldName}.{arrayType.ArraySizeSource}";
            TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
            arrayType.ElementType.Declaration = property.Type.Declaration;
            var arrayElementType = arrayType.ElementType.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
            TypePrinter.PopMarshalType();
            WriteLine($"{property.Name} = new {arrayElementType}[{arraySizeFieldName}];");

            if (pointerType.Declaration is Class classDecl && !classDecl.IsSimpleType)
            {
                var arrayName = $"nativeTmpArray{index}";

                WriteLine($"var {arrayName} = {TextGenerator.NativeUtilsPointerToArray}({arrayPtr}, {arraySizeFieldName});");
                WriteLine($"for (int i = 0; i < {arrayName}.Length; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{property.Name}[i] = new {classDecl.Name}({arrayName}[i]);");
                UnindentAndWriteCloseBrace();
            }
            else if (arrayType.ElementType.IsPrimitiveType(out var primitive) && 
                     primitive is PrimitiveType.Bool32 or PrimitiveType.Bool)
            {
                var arrayName = $"nativeTmpArray{index}";
                WriteLine($"var {arrayName} = {TextGenerator.NativeUtilsPointerToArray}({arrayPtr}, {arraySizeFieldName});");
                WriteLine($"for (int i = 0; i < {arrayName}.Length; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{property.Name}[i] = System.Convert.ToBoolean({arrayName}[i]);");
                UnindentAndWriteCloseBrace();
            }
            else
            {
                WriteLine($"{property.Name} = {TextGenerator.NativeUtilsPointerToArray}({arrayPtr}, (long){arraySizeFieldName});");
            }

            WriteFreeMemory(arrayPtr);
        }

        private void WriteFreeMemory(string pointer)
        {
            WriteLine($"NativeUtils.Free({pointer});");
        }

        private void WriteStringArrayGetter(Property property, Class parentClass, bool isUnicode)
        {
            var pointer = property.Type as PointerType;
            var array = pointer.Pointee as ArrayType;
            if (array != null && !string.IsNullOrEmpty(array.ArraySizeSource))
            {
                var arrayName = $"{parentClass.WrappedStructFieldName}.{property.Field.Name}";
                var arraySizeFieldName = $"{parentClass.WrappedStructFieldName}.{array.ArraySizeSource}";
                WriteLine($"{property.Name} = {TextGenerator.NativeUtilsPointerToStringArray}({arrayName}, (uint){arraySizeFieldName});");
            }
        }

        private void ConstArrayConversionGetter(Property property, Class parentClass, Class decl, TypePrinterResult propertyTypeName)
        {
            var arrayType = property.Type as ArrayType;
            var size = arrayType.Size;
            // TODO: fix generation of fixed statements and simplify them
            if (arrayType.ElementType.CanConvertToFixedArray())
            {
                var arrayName = $"{parentClass.WrappedStructFieldName}.{property.Field.Name}";

                if (propertyTypeName.Type == "string")
                {
                    if (property.Type.IsAnsiString()
                        || arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar))
                    {
                        WriteLine($"{property.Name} = new string((sbyte*){arrayName});");
                    }
                    else if (property.Type.IsUnicodeString() || arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        WriteLine($"{property.Name} = new string((char*){arrayName});");
                    }
                }
                else
                {
                    WriteLine($"{property.Name} = {TextGenerator.NativeUtilsPointerToArray}({arrayName}, {size});");
                }
            }
            else if (arrayType.ElementType.IsPurePointer())
            {
                var propertyArrayElementType = arrayType.ElementType.Visit(TypePrinter);
                WriteLine($"{property.Name} = new {propertyArrayElementType}[{size}];");
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{property.Name}[i] = {parentClass.WrappedStructFieldName}.{property.Field.Name}[i];");
                UnindentAndWriteCloseBrace();
            }
            else if (arrayType.Declaration is Enumeration)
            {
                WriteLine($"{property.Name} = new {arrayType.Declaration.Name}[{size}];");
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                var typeName = arrayType.Declaration.Name;
                WriteLine($"{property.Name}[i] = ({typeName})({parentClass.WrappedStructFieldName}.{property.Field.Name}[i]);");
                UnindentAndWriteCloseBrace();
            }
            else //const array of structs
            {
                TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
                arrayType.ElementType.Declaration = decl;
                var propertyArrayElementType = arrayType.ElementType.Visit(TypePrinter);
                arrayType.ElementType.Declaration = null;
                TypePrinter.PopMarshalType();

                WriteLine($"{property.Name} = new {propertyArrayElementType}[{size}];");
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                var typeName = propertyTypeName.Type.Replace("[]", "", StringComparison.OrdinalIgnoreCase);
                WriteLine($"{property.Name}[i] = new {typeName}({parentClass.WrappedStructFieldName}.{property.Field.Name}[i]);");
                UnindentAndWriteCloseBrace();
            }
        }

        private void WritePointerToStructGetter(Property property, Class parentClass, Class decl, string propertyTypeName)
        {
            var structFieldPath = $"{parentClass.WrappedStructFieldName}.{property.Field.Name}";

            var pointer = property.Field.Type as PointerType;
            if (pointer == null)
            {
                throw new ArgumentException($"Binding type should be pointer to struct, but type is {property.Field.Type}");
            }

            TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
            var fieldTypeName = property.Field.Type.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            var fullTypeName = fieldTypeName;
            if (decl != null && 
                !decl.IsSimpleType && 
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
                WriteLine($"{property.Name} = new {propertyTypeName}(*{structFieldPath});");
                WriteFreeMemory(structFieldPath);
            }
        }

        private void WritePointerToArraySetter(Property property, TypePrinterResult pairedFieldType, int index)
        {
            var decl = property.Field.Type.Declaration as Class;
            var poinerType = property.Type as PointerType;
            var array = poinerType.Pointee as ArrayType;
            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
            array.ElementType.Declaration = property.Field.Type.Declaration;
            var arrayTypeName = array.ElementType.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            var tmpArrayName = $"tmpArray{index}";
            var size = $"{property.Name}.Length";

            if (decl == null || decl.IsSimpleType)
            {
                WriteLine($"var {tmpArrayName} = new {arrayTypeName}[{size}];");
            }
            else if (!decl.IsSimpleType)
            {
                var interop = InteropNamespace;
                var structName = decl.Name;
                if (decl.InnerStruct != null)
                {
                    interop = decl.InnerStruct.Namespace;
                    structName = decl.InnerStruct.Name;
                }
                WriteLine($"var {tmpArrayName} = new {interop}.{structName}[{size}];");
            }

            WriteLine($"for (int i = 0; i < {property.Name}.Length; ++i)");
            WriteOpenBraceAndIndent();
            if (property.Type.IsEnum() || property.Type.IsPointerToArrayOfEnums())
            {
                var @enum = property.Field.Type.Declaration as Enumeration;
                WriteLine($"{tmpArrayName}[i] = ({@enum.InheritanceType}){property.Name}[i];");
            }
            else if (decl != null && (decl.ClassType == ClassType.Class || decl.IsSimpleType)
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
                    
                    WriteLine($"{tmpArrayName}[i] = {conversionName}({property.Name}[i]);");
                }
                else
                {
                    WriteLine($"{tmpArrayName}[i] = {property.Name}[i];");
                }
                
            }
            else
            {
                WriteLine($"{tmpArrayName}[i] = {property.Name}[i].{ConversionMethodName};");
            }
            UnindentAndWriteCloseBrace();
            WriteLine($"{property.PairedField.Name} = new {pairedFieldType.Type}({tmpArrayName});");
        }

        private void ConstArrayConversionSetter(Property property, Class parentClass, Class decl, int index)
        {
            var arrayType = property.Field.Type as ArrayType;
            if (arrayType == null)
            {
                throw new ArgumentException("Type for array is not an ArrayType");
            }

            var size = arrayType.Size;
            arrayType.ElementType.Declaration = arrayType.Declaration;
            
            WriteLine($"if ({property.Name}.Length > {size})");
            PushIndent();
            WriteLine($"throw new System.ArgumentOutOfRangeException(nameof({property.Name}), \"Array is out of bounds. Size should not be more than {size}\");");
            PopIndent();
            NewLine();
            
            var fieldName = $"{parentClass.WrappedStructFieldName}.{property.Field.Name}";
            if (arrayType.ElementType.CanConvertToFixedArray())
            {
                if (arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                    arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                {
                    var isUnicode = arrayType.ElementType.IsUnicodeString().ToString().ToLower();
                    WriteLine(
                        $"{TextGenerator.NativeUtilsStringToFixedArray}({fieldName}, {size}, {property.Name}, {isUnicode});");
                }
                else
                {
                    WriteLine(
                        $"{TextGenerator.NativeUtilsPrimitiveToFixedArray}({fieldName}, {size}, {property.Name});");
                }
            }
            else if (arrayType.ElementType.IsPurePointer())
            {
                WriteLine($"for (int i = 0; i < {size}; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{parentClass.WrappedStructFieldName}.{property.Field.Name}[i] = {property.Name}[i];");
                UnindentAndWriteCloseBrace();
            }
            else if (arrayType.Declaration is Enumeration @enum)
            {
                WriteLine($"for (int i = 0; i < {property.Name}.Length; ++i)");
                WriteOpenBraceAndIndent();
                WriteLine($"{parentClass.WrappedStructFieldName}.{property.Field.Name}[i] = {property.Name}[i];");
                UnindentAndWriteCloseBrace();
            }
            else
            {
                TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
                var result = arrayType.ElementType.Visit(TypePrinter);
                TypePrinter.PopMarshalType();
                TypePrinter.PushMarshalType(MarshalTypes.Property);
                var propType = arrayType.ElementType.Visit(TypePrinter);
                TypePrinter.PopMarshalType();

                if (decl != null && !string.IsNullOrEmpty(decl.Namespace) &&
                    decl.Name == propType.Type)
                {
                    result = $"{InteropNamespace}.{propType}";
                }

                WriteLine($"for (int i = 0; i < {property.Name}.Length; ++i)");
                WriteOpenBraceAndIndent();
                if ((propType.Type == CSharpTypePrinter.ObjectType)
                    || (decl != null && (decl.ClassType == ClassType.Class || decl.ConnectedTo is { ClassType: ClassType.Class })))
                {
                    WriteLine($"{parentClass.WrappedStructFieldName}.{property.Field.Name}[i] = ({result}){property.Name}[i];");
                }
                else
                {
                    WriteLine($"{parentClass.WrappedStructFieldName}.{property.Field.Name}[i] = {property.Name}[i].{ConversionMethodName};");
                }
                UnindentAndWriteCloseBrace();
            }
        }

        private void WritePointerToStructSetter(Property property, string pairedFieldType, int index)
        {
            var decl = property.Field.Type.Declaration as Class;
            var propDecl = property.Type.Declaration as Class;
            if (decl == null || decl.IsSimpleType || property.Type.IsPointerToBuiltInType(out var type))
            {
                WriteLine($"{property.PairedField.Name} = new {pairedFieldType}({property.Name}.Value);");
            }
            else if (!decl.IsSimpleType)
            {
                var varName = $"struct{index}";
                if (decl.ClassType is ClassType.Struct or ClassType.Union)
                {
                    WriteLine($"var {varName} = {property.Name}.{ConversionMethodName};");
                }
                else
                {
                    if (propDecl != null)
                    {
                        if (propDecl.Name == decl.Name && decl.ClassType != ClassType.Class)
                        {
                            WriteLine($"{decl.InnerStruct.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.Name} {varName} = {property.Name};");
                        }
                        else if (decl.ClassType == ClassType.Class && decl.InnerStruct != null)
                        {
                            WriteLine($"{decl.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.InnerStruct.Name} {varName} = {property.Name};");
                        }
                        else
                        {
                            WriteLine($"{decl.Owner.FullNamespace}.{InteropNamespaceExtension}.{decl.Name} {varName} = {property.Name};");
                        }
                    }
                }

                WriteLine($"{property.PairedField.Name} = new {pairedFieldType}({varName});");
            }
        }

        private void GenerateDisposePattern(Class @class)
        {
            if (!@class.IsDisposable || string.IsNullOrEmpty(@class.DisposeBody) || string.IsNullOrEmpty(@class.DisposableBaseClass))
            {
                return;
            }

            PushBlock(CodeBlockKind.Disposable);
            NewLine();
            WriteLine("protected override void UnmanagedDisposeOverride()");
            WriteOpenBraceAndIndent();
            WriteLine(@class.DisposeBody);
            UnindentAndWriteCloseBrace();
            NewLine();
            PopBlock();
        }
    }
}
