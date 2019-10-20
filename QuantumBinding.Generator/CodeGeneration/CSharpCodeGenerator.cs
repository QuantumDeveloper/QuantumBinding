using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class CSharpCodeGenerator: CSharpCodeGeneratorBase
    {
        public CSharpCodeGenerator(
            ProcessingContext context,
            IEnumerable<TranslationUnit> units,
            GeneratorSpecializations specializations)
            : base(context, units, specializations)
        {
            TypePrinter = new CSharpTypePrinter(context.Options);
        }

        public CSharpCodeGenerator(
            ProcessingContext context,
            TranslationUnit unit,
            GeneratorSpecializations specializations)
            : this(context, new List<TranslationUnit>() {unit}, specializations)
        {

        }

        public override void Run()
        {
            PushBlock(CodeBlockKind.Root);
            GenerateFileHeader();
            foreach (var unit in TranslationUnits)
            {
                TypePrinter.PushModule(unit.Module);
                SetAlternativeNamespace(unit);
                GenerateTranslationUnit(unit);
            }
            PopBlock();
        }

        protected virtual void GenerateTranslationUnit(TranslationUnit unit)
        {
            CurrentTranslationUnit = unit;
            GenerateNamespace(unit);
        }

        protected virtual void GenerateNamespace(Namespace @namespace)
        {
            var dict = new Dictionary<GeneratorSpecializations, Action>
            {
                { GeneratorSpecializations.Constants, GenerateMacros },
                { GeneratorSpecializations.Enums, GenerateEnums },
                { GeneratorSpecializations.Structs, () => GenerateClasses(ClassType.Struct) },
                { GeneratorSpecializations.Unions, () => GenerateClasses(ClassType.Union) },
                { GeneratorSpecializations.Classes, () => GenerateClasses(ClassType.Class) },
                { GeneratorSpecializations.Delegates, GenerateDelegates },
                { GeneratorSpecializations.Functions, GenerateFunctions }
            };

            var parts = Specializations.GetFlags();
            PushBlock(CodeBlockKind.Namespace);
            WriteCurrentNamespace(@namespace);
            WriteOpenBraceAndIndent();

            GenerateUsings();

            NewLine();

            if (Specializations == GeneratorSpecializations.None)
            {
                foreach (var action in dict)
                {
                    action.Value.Invoke();
                    NewLine();
                }
            }
            else
            {
                foreach (var part in parts)
                {
                    if (part == GeneratorSpecializations.All || !dict.ContainsKey(part))
                        continue;

                    dict[part].Invoke();
                    NewLine();
                }
            }

            foreach (var childNamespace in @namespace.Namespaces)
            {
                GenerateNamespace(childNamespace);
            }

            UnindentAndWriteCloseBrace();
            PopBlock(NewLineStrategy.NewLineBeforeNextBlock);
        }

        private void GenerateEnums()
        {
            foreach (var @enum in CurrentTranslationUnit.Enums)
            {
                GenerateEnumItems(@enum);
            }
        }

        private void GenerateClasses(ClassType classType)
        {
            switch (classType)
            {
                case ClassType.Struct:
                    var structs = CurrentTranslationUnit.Structs.Where(x => !x.IsIgnored);

                    if (Options.ConvertRules.PodTypesAsSimpleTypes && structs.Where(x=>!x.IsSimpleType).ToList().Count == 0 && Specializations == GeneratorSpecializations.Structs)
                    {
                        IsGeneratorEmpty = true;
                    }

                    foreach (var @struct in structs)
                    {
                        GenerateClass(@struct);
                    }

                    break;
                case ClassType.Union:
                    var unions = CurrentTranslationUnit.Unions.Where(x => !x.IsIgnored);
                    if (unions.Count() == 0 && Specializations == GeneratorSpecializations.Unions)
                    {
                        IsGeneratorEmpty = true;
                    }

                    foreach (var union in unions)
                    {
                        GenerateClass(union);
                    }
                    break;
                case ClassType.Class:
                    var classes = CurrentTranslationUnit.Classes.Where(x => !x.IsIgnored);

                    foreach (var @class in classes)
                    {
                        GenerateClass(@class);
                    }

                    GenerateCommonMethods();

                    var classExtensions = CurrentTranslationUnit.ExtensionClasses.Where(x => !x.IsIgnored);
                    foreach (var ext in classExtensions)
                    {
                        GenerateClassExtension(ext);
                    }

                    if (classes.Count() == 0 && classExtensions.Count() == 0 && Specializations == GeneratorSpecializations.Classes)
                    {
                        IsGeneratorEmpty = true;
                    }

                    break;
            }
        }

        private void GenerateMacros()
        {
            var macros = CurrentTranslationUnit.Macros.Where(x => !x.IsIgnored).ToList();
            if (macros.Count == 0 && Specializations == GeneratorSpecializations.Constants)
            {
                IsGeneratorEmpty = true;
            }

            PushBlock(CodeBlockKind.Class);
            WriteLine("public static class Constants");
            WriteOpenBraceAndIndent();
            foreach (var macro in CurrentTranslationUnit.Macros)
            {
                if (macro.IsIgnored)
                    continue;

                GenerateMacro(macro);
            }
            UnindentAndWriteCloseBrace();
            PopBlock();
        }

        protected override void GenerateMacro(Macro macro)
        {
            if (macro.IsIgnored)
                return;

            PushBlock(CodeBlockKind.Macro);
            GenerateCommentIfNotEmpty(macro.Comment);

            WriteLocation(macro);
            if (!macro.IsFunctionLike)
            {
                WriteLine($"public static {macro.Type.Visit(TypePrinter)} {macro.Name} => {macro.Value};");
            }
            else
            {
                var @params = TypePrinter.VisitParameters(macro.Parameters, MarshalTypes.MethodParameter);
                WriteLine($"public static {macro.Type.Visit(TypePrinter)} {macro.Name}({@params})");
                WriteOpenBraceAndIndent();
                WriteLine(macro.Value);
                UnindentAndWriteCloseBrace();
            }
            NewLine();
            PopBlock();
        }

        protected override void GenerateEnumItems(Enumeration @enum)
        {
            if (@enum.IsIgnored)
                return;

            PushBlock(CodeBlockKind.Enum, @enum);
            WriteLocation(@enum);

            GenerateCommentIfNotEmpty(@enum.Comment);

            if (@enum.IsFlagEnum)
            {
                WriteLine("[Flags]");
            }
            WriteLine($"{@enum.AccessSpecifier.ToString().ToLower()} enum {@enum.Name} : {@enum.InheritanceType}");
            WriteOpenBraceAndIndent();
            foreach (var item in @enum.Items)
            {
                if (item.IsIgnored) continue;
                PushBlock(CodeBlockKind.EnumItem, item);
                GenerateCommentIfNotEmpty(item.Comment);
                WriteLine($"{item.Name} = {item.Value},");
                NewLine();
                PopBlock();
            }
            UnindentAndWriteCloseBrace();

            NewLine();

            PopBlock();
        }

        private void GenerateCommonMethods()
        {
            if (CurrentTranslationUnit.Methods.Count == 0)
            {
                return;
            }

            PushBlock(CodeBlockKind.Class);
            WriteLine($"public static class {CurrentTranslationUnit.Module.MethodClassName}");
            WriteOpenBraceAndIndent();

            TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
            foreach (var method in CurrentTranslationUnit.Methods)
            {
                GenerateMethod(method);
            }
            TypePrinter.PopMarshalType();

            UnindentAndWriteCloseBrace();

            PopBlock();
        }

        protected override void GenerateClass(Class @class)
        {
            if (@class.IsSimpleType && CurrentTranslationUnit.Module.SkipPodTypesGeneration)
            {
                return;
            }

            switch (@class.ClassType)
            {
                case ClassType.Class:
                    PushBlock(CodeBlockKind.Class);
                    break;
                case ClassType.Struct:
                    PushBlock(CodeBlockKind.Struct);
                    break;
                case ClassType.Union:
                    PushBlock(CodeBlockKind.Union);
                    break;
            }

            WriteLocation(@class);

            GenerateCommentIfNotEmpty(@class.Comment);

            if (Options.GenerateSequentialLayout && @class.ClassType == ClassType.Struct)
            {
                PushBlock(CodeBlockKind.Attribute);
                WriteLine("[StructLayout(LayoutKind.Sequential)]");
                PopBlock();
            }
            else if (Options.GenerateSequentialLayout && @class.ClassType == ClassType.Union)
            {
                PushBlock(CodeBlockKind.Attribute);
                WriteLine("[StructLayout(LayoutKind.Explicit)]");
                PopBlock();
            }

            WriteLine(TypePrinter.VisitClass(@class).ToString());
            WriteOpenBraceAndIndent();

            GenerateFields(@class);

            GenerateConstructors(@class);

            GenerateProperties(@class);

            GenerateMethods(@class);

            GenerateOverloads(@class);

            UnindentAndWriteCloseBrace();

            NewLine();

            PopBlock();
        }

        private void GenerateClassExtension(Class extension)
        {
            switch (extension.ClassType)
            {
                case ClassType.Class:
                    PushBlock(CodeBlockKind.Class);
                    break;
                case ClassType.Struct:
                    PushBlock(CodeBlockKind.Struct);
                    break;
                case ClassType.Union:
                    PushBlock(CodeBlockKind.Union);
                    break;
            }

            WriteLocation(extension);

            WriteLine($"{TypePrinter.GetAccessSpecifier(extension.AccessSpecifier)} static partial class {extension.Name}Extension");
            WriteOpenBraceAndIndent();

            GenerateExtensionMethods(extension);

            UnindentAndWriteCloseBrace();

            NewLine();

            PopBlock();
        }

        private void GenerateProperties(Class @class)
        {
            foreach (var property in @class.Properties)
            {
                PushBlock(CodeBlockKind.Property);
                GenerateCommentIfNotEmpty(property.Comment);
                TypePrinter.PushMarshalType(MarshalTypes.NativeField);
                var propertyStr = TypePrinter.VisitProperty(property);
                TypePrinter.PopMarshalType();

                PushBlock(CodeBlockKind.AccessSpecifier);
                Write($"{TypePrinter.GetAccessSpecifier(property.AccessSpecifier)}");
                PopBlock(NewLineStrategy.SpaceBeforeNextBlock);
                WriteLine($"{propertyStr}");
                WriteOpenBraceAndIndent();

                var getterAccessSpecifier = TypePrinter.GetAccessSpecifier(property.Getter?.AccessSpecifier);
                var setterAccessSpecifier = TypePrinter.GetAccessSpecifier(property.Setter?.AccessSpecifier);
                if (property.IsAutoProperty)
                {
                    if (property.Getter.AccessSpecifier != AccessSpecifier.Public)
                    {
                        Write($"{getterAccessSpecifier} ");
                    }

                    Write("get;");

                    if (property.Setter.AccessSpecifier != AccessSpecifier.Public)
                    {
                        Write($"{setterAccessSpecifier} ");
                    }

                    Write("set;");
                }
                else
                {
                    if (property.Getter != null)
                    {
                        if (property.Getter.AccessSpecifier != AccessSpecifier.Public)
                        {
                            Write($"{getterAccessSpecifier} ");
                        }

                        Write($"get => {property.Field.Name};");
                        NewLine();
                    }

                    if (property.Setter != null)
                    {
                        if (property.Setter.AccessSpecifier != AccessSpecifier.Public)
                        {
                            Write($"{setterAccessSpecifier} ");
                        }

                        Write($"set => {property.Field.Name} = value;");
                        NewLine();
                    }
                }

                UnindentAndWriteCloseBrace();

                NewLine();

                PopBlock();
            }
        }

        private void GenerateMethods(Class @class)
        {
            foreach (var method in @class.Methods.OrderBy(x=>x.Name))
            {
                GenerateMethod(method);
            }
        }

        private void GenerateExtensionMethods(Class @class)
        {
            foreach (var method in @class.ExtensionMethods)
            {
                //If Current translation unit is not the same file as extension method, skip method generation
                if (CurrentTranslationUnit.FileName != method.Owner.FileName) continue;
                GenerateMethod(method);
            }
        }

        private void GenerateDelegates()
        {
            foreach (var @delegate in CurrentTranslationUnit.Delegates)
            {
                GenerateDelegate(@delegate);
            }
        }

        private void GenerateFunctions()
        {
            PushBlock(CodeBlockKind.Class);
            WriteLine($"public static class {CurrentTranslationUnit.Module.InteropClassName}");
            WriteOpenBraceAndIndent();
            string libraryPath = "LibraryPath";
            WriteLine($"public const string {libraryPath} = \"{CurrentTranslationUnit.Module.LibraryName}\";");
            NewLine();

            TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);

            foreach (var function in CurrentTranslationUnit.Functions.OrderBy(x=>x.Name))
            {
                function.DllName = libraryPath;
                GenerateFunction(function);
                NewLine();
            }
            TypePrinter.PopMarshalType();

            UnindentAndWriteCloseBrace();
            PopBlock();
            NewLine();
        }

        protected override void GenerateDelegate(Delegate @delegate)
        {
            if (@delegate.IsIgnored)
                return;

            PushBlock(CodeBlockKind.Delegate, @delegate);

            WriteLocation(@delegate);

            GenerateCommentIfNotEmpty(@delegate.Comment);

            if (@delegate.SuppressUnmanagedCodeSecurity)
            {
                PushBlock(CodeBlockKind.Attribute);
                WriteLine("[SuppressUnmanagedCodeSecurity]");
                PopBlock();
            }

            PushBlock(CodeBlockKind.Attribute);
            WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{@delegate.CallingConvention})]");
            PopBlock();

            if (@delegate.ReturnType.IsAnsiString())
            {
                PushBlock(CodeBlockKind.Attribute);
                WriteLine($"[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]");
                PopBlock();
            }

            PushBlock(CodeBlockKind.AccessSpecifier);
            Write($"{TypePrinter.GetAccessSpecifier(@delegate.AccessSpecifier)} delegate");
            PopBlock(NewLineStrategy.SpaceBeforeNextBlock);

            var returnType = @delegate.ReturnType.Visit(TypePrinter);
            Write($"{returnType} {@delegate.Name}(");
            if (@delegate.Name == "PFN_vkCreateInstance")
            {

            }
            CheckParameters(@delegate.Parameters);
            var @params = TypePrinter.VisitParameters(@delegate.Parameters, MarshalTypes.DelegateParameter);
            Write(@params.ToString());
            Write(");");

            NewLine();

            PopBlock();
        }

        protected override void GenerateFunction(Function function)
        {
            if (function.IsIgnored)
                return;

            PushBlock(CodeBlockKind.Function, function);

            WriteLocation(function);

            GenerateCommentIfNotEmpty(function.Comment);

            if (function.SuppressUnmanagedCodeSecurity)
            {
                PushBlock(CodeBlockKind.Attribute);
                WriteLine("[SuppressUnmanagedCodeSecurity]");
                PopBlock();
            }

            PushBlock(CodeBlockKind.Attribute);
            WriteLine($"[DllImport({function.DllName}, EntryPoint = \"{function.EntryPoint}\", CallingConvention = CallingConvention.{function.CallingConvention})]");
            PopBlock();

            if (function.ReturnType.IsAnsiString())
            {
                PushBlock(CodeBlockKind.Attribute);
                WriteLine($"[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]");
                PopBlock();
            }

            PushBlock(CodeBlockKind.AccessSpecifier);
            Write($"{TypePrinter.GetAccessSpecifier(function.AccessSpecifier)} static extern");
            PopBlock(NewLineStrategy.SpaceBeforeNextBlock);

            var returnType = function.ReturnType.Visit(TypePrinter);
            Write($"{returnType} {function.Name}(");
            CheckParameters(function.Parameters);
            if (function.Name == "spvc_compiler_options_set_bool")
            {

            }
            var @params = TypePrinter.VisitParameters(function.Parameters, MarshalTypes.NativeParameter);
            Write(@params.ToString());
            Write(");");

            NewLine();

            PopBlock();
        }

        private void GenerateMethod(Method method)
        {
            if (method.IsIgnored)
                return;

            PushBlock(CodeBlockKind.Method, method);
            GenerateCommentIfNotEmpty(method.Function.Comment);
            CheckParameters(method.Parameters);
            var methodResult = TypePrinter.VisitMethod(method);
            WriteLine(methodResult.ToString());
            WriteOpenBraceAndIndent();
            var nativeFunctionCallResult = VisitNativeFunctionCall(method);
            PushBlock(CodeBlockKind.MethodBody, nativeFunctionCallResult);
            WriteLine(nativeFunctionCallResult.ToString());
            PopBlock();
            UnindentAndWriteCloseBrace();

            NewLine();

            PopBlock();
        }

        public TypePrinterResult VisitNativeFunctionCall(Method method)
        {
            TextGenerator textGenerator = new TextGenerator();
            List<Parameter> nativeParams = new List<Parameter>();
            List<Action> actions = new List<Action>();

            bool isVoid = method.ReturnType.IsPrimitiveTypeEquals(PrimitiveType.Void);
            int index = 0;
            var wrapInteropObjects = CurrentTranslationUnit.Module.WrapInteropObjects;

            if (method.Name == "loadDiagnostics" || method.Name == "CmdBeginTransformFeedbackEXT")
            {

            }

            for (var i = 0; i < method.Function.Parameters.Count; i++)
            {
                if (method.Name == "UnregisterObjectsNVX")
                {

                }
                var parameter = method.Function.Parameters[i];
                var classDecl = parameter.Type.Declaration as Class;
                if (wrapInteropObjects)
                {
                    if (classDecl != null && (classDecl.ClassType == ClassType.Struct || classDecl.ClassType == ClassType.Union))
                    {
                        parameter = method.Parameters.FirstOrDefault(x=>x.Id == parameter.Id);
                        classDecl = parameter.Type.Declaration as Class;
                    }
                }

                ArrayType currentArray = null;
                string arrayLength = string.Empty;
                if (classDecl == null)
                {
                    if (parameter.Type.IsConstArray(out int arraySize))
                    {
                        textGenerator.WriteLine(
                            $"if ({parameter.Name} == null || {parameter.Name}.Length != {arraySize})");
                        textGenerator.WriteOpenBraceAndIndent();
                        textGenerator.WriteLine(
                            $"throw new ArgumentOutOfRangeException(\"{parameter.Name}\", \"The dimensions of the provided array don't match the required size. Size should be = {arraySize}\");");
                        textGenerator.UnindentAndWriteCloseBrace();
                    }

                    if (parameter.Type.Declaration == null || !parameter.Type.IsPointer())
                    {
                        nativeParams.Add(parameter);
                        continue;
                    }
                }

                var argumentName = $"arg{index++}";
                if (classDecl != null)
                {
                    if (classDecl.ClassType == ClassType.Class && !parameter.Type.IsPointerToArray())
                    {
                        WriteClass();
                    }
                    else //arrays of structs, structs, unions, PODs (plain old data => primitive types)
                    {
                        var type = parameter.Type;
                        if (type.IsPointer())
                        {
                            if (type.IsPointerToArray())
                            {
                                WritePointerToArray();
                            }
                            else if (!classDecl.IsSimpleType)
                            {
                                WritePointerToStruct();
                            }
                            else //Pointer to simple type like int, long, float, etc.
                            {
                                WritePointerToSimpleType();
                            }
                        }
                        else // structs without pointers, simple types
                        {
                            if (parameter.ParameterKind == ParameterKind.Out &&
                                CurrentTranslationUnit.Module.WrapInteropObjects && !classDecl.IsSimpleType)
                            {
                                TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                                var t = parameter.Type.Visit(TypePrinter);
                                TypePrinter.PopMarshalType();
                                textGenerator.WriteLine(
                                    $"{parameter.Type.Declaration.AlternativeNamespace}.{t} {argumentName};");

                                actions.Add(ConvertOutStructToClass);
                            }
                            else
                            {
                                argumentName = parameter.Name;
                            }
                        }
                    }

                    nativeParams.Add(new Parameter() { Name = argumentName, ParameterKind = parameter.ParameterKind, Type = parameter.Type });
                }
                else if (parameter.Type.Declaration is Enumeration enumeration)
                {
                    if (parameter.Type.IsPointer() && !parameter.Type.IsPointerToArray())
                    {
                        if (parameter.ParameterKind == ParameterKind.In || parameter.ParameterKind == ParameterKind.Readonly)
                        {
                            textGenerator.WriteLine($"var {argumentName} = new GCHandleReference(({enumeration.InheritanceType}){parameter.Name});");
                            nativeParams.Add(new Parameter() { Name = $"{argumentName}.Handle", ParameterKind = parameter.ParameterKind, Type = parameter.Type });
                            actions.Add(FreeReference);
                        }
                        else if (parameter.ParameterKind == ParameterKind.InOut)
                        {
                            textGenerator.WriteLine($"var {argumentName} = ({enumeration.InheritanceType}){parameter.Name};");
                            nativeParams.Add(new Parameter() { Name = $"{argumentName}", ParameterKind = parameter.ParameterKind, Type = parameter.Type });
                            actions.Add(ConvertRefResultBackToEnum);
                        }
                        else // ParameterKind == ParameterKind.Out
                        {
                            nativeParams.Add(parameter);
                        }
                    }
                    else if (parameter.Type.IsPointerToArray())
                    {
                        var pointer = parameter.Type as PointerType;
                        currentArray = pointer.Pointee as ArrayType;
                        arrayLength = currentArray.ArraySizeSource;
                        if (string.IsNullOrEmpty(arrayLength))
                        {
                            arrayLength = $"{parameter.Name}.Length";
                        }

                        textGenerator.WriteLine($"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {enumeration.InheritanceType}[{parameter.Name}.Length];");
                        textGenerator.WriteLine($"if (!ReferenceEquals({parameter.Name}, null))");
                        textGenerator.WriteOpenBraceAndIndent();
                        textGenerator.WriteLine($"for (int i = 0; i < {arrayLength}; ++i)");
                        textGenerator.WriteOpenBraceAndIndent();
                        textGenerator.WriteLine($"{argumentName}[i] = ({enumeration.InheritanceType}){parameter.Name}[i];");
                        textGenerator.UnindentAndWriteCloseBrace();
                        textGenerator.UnindentAndWriteCloseBrace();

                        nativeParams.Add(new Parameter() { Name = $"{argumentName}", ParameterKind = parameter.ParameterKind, Type = parameter.Type });
                    }
                }

                void WriteClass()
                {
                    if (parameter.ParameterKind != ParameterKind.Out && !parameter.Type.IsPointerToArray())
                    {
                        if (classDecl == method.Class && parameter.Index == 0)
                        {
                            if (method.IsStatic)
                            {
                                textGenerator.WriteLine($"var {argumentName} = {parameter.Name};");
                            }
                            else
                            {
                                argumentName = "this"; // pass this as first parameter tp avoid additional copying of memory
                            }
                        }
                        else
                        {
                            if (!parameter.Type.IsPointer())
                            {
                                textGenerator.WriteLine(
                                    $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? new {parameter.Type.Visit(TypePrinter)}() : ({classDecl.InnerStruct.Name}){parameter.Name};");
                            }
                            else
                            {
                                textGenerator.WriteLine(
                                    $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr(({classDecl.InnerStruct.Name}){parameter.Name});");
                                if (parameter.ParameterKind == ParameterKind.InOut)
                                {
                                    actions.Add(ConvertPointerToClassOrStruct);
                                }
                                actions.Add(FrePtr);
                            }
                        }
                    }
                    else if (parameter.Type.IsPointerToArray())
                    {
                        var pointer = parameter.Type as PointerType;
                        currentArray = pointer.Pointee as ArrayType;
                        arrayLength = currentArray.ArraySizeSource;
                        if (string.IsNullOrEmpty(arrayLength))
                        {
                            arrayLength = $"{parameter.Name}.Length";
                        }

                        if (parameter.ParameterKind != ParameterKind.Out)
                        {
                            textGenerator.WriteLine(
                                $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[{arrayLength}];");

                            if (parameter.ParameterKind == ParameterKind.InOut)
                            {
                                actions.Add(ImplicitTwoWayArrayTypeConversion);
                            }
                            else if (parameter.ParameterKind == ParameterKind.In)
                            {
                                ImplicitTwoWayArrayTypeConversion();
                            }
                        }
                        else if (parameter.ParameterKind == ParameterKind.Out)
                        {
                            if (!string.IsNullOrEmpty(currentArray.ArraySizeSource)
                                && CurrentTranslationUnit.Module.TreatOutputArraysAsPointers)
                            {
                                textGenerator.WriteLine($"IntPtr {argumentName} = IntPtr.Zero;");
                                actions.Add(ConvertPtrToWrappedStructArray);
                            }
                            else if (!string.IsNullOrEmpty(currentArray.ArraySizeSource)
                                     && !CurrentTranslationUnit.Module.TreatOutputArraysAsPointers)
                            {
                                textGenerator.WriteLine(
                                    $"var {argumentName} = new {classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[{arrayLength}];");
                                actions.Add(ImplicitTwoWayArrayTypeConversion);
                            }
                            else
                            {
                                argumentName = parameter.Name;
                            }
                        }
                    }
                    else
                    {
                        textGenerator.WriteLine($"{classDecl.InnerStruct.Name} {argumentName};");
                        actions.Add(ConvertOutStructToClass);
                    }
                }

                void WritePointerToStruct()
                {
                    var pointer = (PointerType)parameter.Type;
 
                    TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                    var interopType = parameter.Type.Visit(TypePrinter).Type;
                    string wrappedType = string.Empty;
                    if (wrapInteropObjects && parameter.WrappedType != null)
                    {
                        wrappedType = parameter.WrappedType.Visit(TypePrinter).Type;
                        if (interopType == wrappedType)
                        {
                            interopType = $"{classDecl.Owner.FullNamespace}.{interopType}";
                            wrappedType = $"{classDecl.AlternativeNamespace}.{interopType}";
                        }
                    }
                    TypePrinter.PopMarshalType();

                    if (parameter.ParameterKind == ParameterKind.In || 
                        parameter.ParameterKind == ParameterKind.Readonly || 
                        parameter.ParameterKind == ParameterKind.InOut)
                    {
                        // Input parameter is IntPtr, so we need to just pass it as is without any conversion
                        if (parameter.Type.IsPointerToIntPtr() || parameter.Type.IsPurePointer())
                        {
                            argumentName = parameter.Name;
                        }
                        else
                        {
                            switch (classDecl.ClassType)
                            {
                                case ClassType.Struct:
                                case ClassType.Union:
                                    if (pointer.IsNullable)
                                    {
                                        textGenerator.WriteLine(
                                            $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr({parameter.Name}.Value);");
                                    }
                                    else
                                    {
                                        textGenerator.WriteLine(
                                            $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr({parameter.Name});");
                                    }
                                    break;
                                case ClassType.StructWrapper:
                                case ClassType.UnionWrapper:
                                    textGenerator.WriteLine(
                                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr({parameter.Name}.{ConversionMethodName});");
                                    actions.Add(DisposeWrapper);
                                    break;
                            }
                            if (parameter.ParameterKind == ParameterKind.InOut)
                            {
                                actions.Add(ConvertPointerToClassOrStruct);
                            }
                            actions.Add(FrePtr);
                        }
                    }
                    else if (parameter.ParameterKind == ParameterKind.Out)
                    {
                        if (wrapInteropObjects)
                        {
                            wrappedType = wrappedType.TrimEnd('?');
                            textGenerator.WriteLine($"{wrappedType} {argumentName};");
                            actions.Add(ConvertOutStructToClass);
                        }
                        else
                        {
                            argumentName = parameter.Name;
                        }
                    }
                }

                void WritePointerToSimpleType()
                {
                    var pointer = (PointerType)parameter.Type;

                    TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                    var interopType = parameter.Type.Visit(TypePrinter).Type;
                    string wrappedType = string.Empty;
                    if (wrapInteropObjects && parameter.WrappedType != null)
                    {
                        wrappedType = parameter.WrappedType.Visit(TypePrinter).Type;
                        if (interopType == wrappedType)
                        {
                            interopType = $"{classDecl.Owner.FullNamespace}.{interopType}";
                            wrappedType = $"{classDecl.AlternativeNamespace}.{interopType}";
                        }
                    }
                    TypePrinter.PopMarshalType();

                    if (parameter.ParameterKind == ParameterKind.In ||
                        parameter.ParameterKind == ParameterKind.Readonly ||
                        parameter.ParameterKind == ParameterKind.InOut)
                    {
                        // Input parameter is just ref to simple type like int, long or another, so we need to just pass it as is without any conversion
                        if (classDecl.IsSimpleType && 
                            Options.ConvertRules.PodTypesAsSimpleTypes && 
                            parameter.ParameterKind == ParameterKind.InOut)
                        {
                            argumentName = parameter.Name;
                        }
                        else
                        {
                            switch (classDecl.ClassType)
                            {
                                case ClassType.Struct:
                                case ClassType.Union:
                                    if (pointer.IsNullable)
                                    {
                                        textGenerator.WriteLine(
                                            $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr({parameter.Name}.Value);");
                                    }
                                    else
                                    {
                                        textGenerator.WriteLine(
                                            $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr({parameter.Name});");
                                    }
                                    break;
                                case ClassType.StructWrapper:
                                case ClassType.UnionWrapper:
                                    textGenerator.WriteLine(
                                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {IntPtrZero} : MarshalUtils.MarshalStructToPtr({parameter.Name}.{ConversionMethodName});");
                                    actions.Add(DisposeWrapper);
                                    break;
                            }
                            if (parameter.ParameterKind == ParameterKind.InOut && !classDecl.IsSimpleType)
                            {
                                actions.Add(ConvertPointerToClassOrStruct);
                            }
                            actions.Add(FrePtr);
                        }
                    }
                    else if (parameter.ParameterKind == ParameterKind.Out)
                    {
                        if (wrapInteropObjects)
                        {
                            wrappedType = wrappedType.TrimEnd('?');
                            textGenerator.WriteLine($"{wrappedType} {argumentName};");
                            actions.Add(ConvertOutStructToClass);
                        }
                        else
                        {
                            argumentName = parameter.Name;
                        }
                    }
                }

                void WritePointerToArray()
                {
                    var pointer = parameter.Type as PointerType;
                    currentArray = pointer.Pointee as ArrayType;
                    var arraySizeSource = currentArray.ArraySizeSource;
                    arrayLength = $"{parameter.Name}.Length";
                    if (!string.IsNullOrEmpty(arraySizeSource))
                    {
                        arrayLength = arraySizeSource;
                    }
                    if (!classDecl.IsSimpleType)
                    {
                        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                        var typeStrResult = parameter.Type.Visit(TypePrinter);
                        if (wrapInteropObjects && parameter.WrappedType != null)
                        {
                            typeStrResult = parameter.WrappedType.Visit(TypePrinter);
                        }
                        TypePrinter.PopMarshalType();
                        if (parameter.ParameterKind == ParameterKind.In)
                        {
                            if (classDecl.ClassType == ClassType.Class)
                            {
                                textGenerator.WriteLine($"{classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[] {argumentName} = null;");
                            }
                            else
                            {
                                textGenerator.WriteLine($"{classDecl.AlternativeNamespace}.{typeStrResult}[] {argumentName} = null;");
                            }

                            if (classDecl.ClassType == ClassType.Class)
                            {
                                textGenerator.WriteLine($"{argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[{arrayLength}];");
                            }
                            else
                            {
                                textGenerator.WriteLine($"{argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {classDecl.AlternativeNamespace}.{typeStrResult}[{arrayLength}];");
                            }
                            ImplicitTwoWayArrayTypeConversion();
                        }
                        else if (parameter.ParameterKind == ParameterKind.InOut)
                        {
                            if (classDecl.ClassType == ClassType.Class)
                            {
                                textGenerator.WriteLine($"{classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[] {argumentName} = null;");
                            }
                            else
                            {
                                textGenerator.WriteLine($"{classDecl.AlternativeNamespace}.{typeStrResult}[] {argumentName} = null;");
                            }

                            if (classDecl.ClassType == ClassType.Class)
                            {
                                textGenerator.WriteLine($"{argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[{arrayLength}];");
                            }
                            else
                            {
                                textGenerator.WriteLine($"{argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {classDecl.AlternativeNamespace}.{typeStrResult}[{arrayLength}];");
                            }

                            actions.Add(ImplicitTwoWayArrayTypeConversion);
                        }
                        else if (parameter.ParameterKind == ParameterKind.Out)
                        {
                            if (CurrentTranslationUnit.Module.TreatOutputArraysAsPointers)
                            {
                                textGenerator.WriteLine($"var {argumentName} = {IntPtrZero};");
                                actions.Add(ConvertPtrToWrappedStructArray);
                            }
                            else if (!string.IsNullOrEmpty(arraySizeSource))
                            {
                                if (classDecl.ClassType == ClassType.Class)
                                {
                                    textGenerator.WriteLine($"var {argumentName} = new {classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}[{arraySizeSource}];");
                                }
                                else
                                {
                                    textGenerator.WriteLine($"var {argumentName} = new {classDecl.AlternativeNamespace}.{classDecl.Name}[{arraySizeSource}];");
                                }
                                actions.Add(ImplicitTwoWayArrayTypeConversion);
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(arraySizeSource) &&
                             parameter.ParameterKind == ParameterKind.Out &&
                             CurrentTranslationUnit.Module.TreatOutputArraysAsPointers)
                    {
                        textGenerator.WriteLine($"var {argumentName} = {IntPtrZero};");
                        actions.Add(ConvertIntPtrToArray);
                    }
                    else
                    {
                        argumentName = parameter.Name;
                    }
                }

                void FrePtr()
                {
                    textGenerator.WriteLine($"Marshal.FreeHGlobal({argumentName});");
                }

                void FreeReference()
                {
                    textGenerator.WriteLine($"{argumentName}?.Dispose();");
                }

                void ConvertRefResultBackToEnum()
                {
                    textGenerator.WriteLine($"{parameter.Name} = ({parameter.Type.Declaration.Name}){argumentName};");
                }

                void DisposeWrapper()
                {
                    if ((classDecl.ClassType == ClassType.StructWrapper ||
                         classDecl.ClassType == ClassType.UnionWrapper) && classDecl.IsDisposable)
                    {
                        textGenerator.WriteLine($"{parameter.Name}?.Dispose();");
                    }
                }

                void DisposeWrappedArray()
                {
                    if ((classDecl.ClassType == ClassType.StructWrapper ||
                         classDecl.ClassType == ClassType.UnionWrapper) && classDecl.IsDisposable)
                    {
                        textGenerator.WriteLine($"if (!ReferenceEquals({parameter.Name}, null))");
                        textGenerator.WriteOpenBraceAndIndent();
                        textGenerator.WriteLine($"for (int i = 0; i < {arrayLength}; ++i)");
                        textGenerator.WriteOpenBraceAndIndent();
                        if (parameter.ParameterKind == ParameterKind.In)
                        {
                            textGenerator.WriteLine($"{parameter.Name}[i]?.Dispose();");
                        }
                        textGenerator.UnindentAndWriteCloseBrace();
                        textGenerator.UnindentAndWriteCloseBrace();
                    }
                }

                void ConvertPointerToClassOrStruct()
                {
                    var tmp = $"temp{argumentName}";
                    if (classDecl.ClassType == ClassType.Class)
                    {
                        textGenerator.WriteLine(
                            $"var {tmp} =  Marshal.PtrToStructure<{classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}>({argumentName});");
                        textGenerator.WriteLine($"{parameter.Name} = {tmp};");
                    }
                    else if (classDecl.ClassType == ClassType.StructWrapper || classDecl.ClassType == ClassType.UnionWrapper)
                    {
                        textGenerator.WriteLine(
                            $"var {tmp} =  Marshal.PtrToStructure<{classDecl.WrappedStruct.AlternativeNamespace}.{classDecl.WrappedStruct.Name}>({argumentName});");
                        textGenerator.WriteLine($"{parameter.Name} = new {classDecl.Owner.FullNamespace}.{classDecl.Name}({tmp});");
                    }
                    else
                    {
                        textGenerator.WriteLine($"{parameter.Name} =  Marshal.PtrToStructure<{classDecl.AlternativeNamespace}.{classDecl.Name}>({argumentName});");
                    }
                }

                void ConvertOutStructToClass()
                {
                    if (wrapInteropObjects)
                    {
                        var wrapperType = GetTypeNameFromParameter(parameter, classDecl);
                        textGenerator.WriteLine($"{parameter.Name} = new {wrapperType}({argumentName});");
                    }
                    else
                    {
                        textGenerator.WriteLine($"{parameter.Name} = {argumentName};");
                    }
                }

                void ImplicitTwoWayArrayTypeConversion()
                {
                    if (parameter.ParameterKind != ParameterKind.Out)
                    {
                        textGenerator.WriteLine($"if (!ReferenceEquals({parameter.Name}, null))");
                        textGenerator.WriteOpenBraceAndIndent();
                        InnerArrayCopy();
                        textGenerator.UnindentAndWriteCloseBrace();
                    }
                    else
                    {
                        textGenerator.WriteLine($"{parameter.Name} = new {classDecl.Name}[{arrayLength}];");
                        InnerArrayCopy();
                    }

                    void InnerArrayCopy()
                    {
                        textGenerator.WriteLine($"for (int i = 0; i < {arrayLength}; ++i)");
                        textGenerator.WriteOpenBraceAndIndent();
                        if (parameter.ParameterKind != ParameterKind.In)
                        {
                            if (wrapInteropObjects)
                            {
                                textGenerator.WriteLine($"{parameter.Name}[i] = new {classDecl.Name}({argumentName}[i]);");
                            }
                            else
                            {
                                textGenerator.WriteLine($"{parameter.Name}[i] = {argumentName}[i];");
                            }
                        }
                        else
                        {
                            if (wrapInteropObjects && classDecl.ClassType != ClassType.Class)
                            {
                                textGenerator.WriteLine($"{argumentName}[i] = {parameter.Name}[i].{ConversionMethodName};");
                                actions.Add(DisposeWrappedArray);
                            }
                            else
                            {
                                textGenerator.WriteLine($"{argumentName}[i] = {parameter.Name}[i];");
                            }
                        }

                        textGenerator.UnindentAndWriteCloseBrace();
                    }
                }

                void ConvertIntPtrToArray()
                {
                    TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                    var type = parameter.Type.Visit(TypePrinter);
                    TypePrinter.PopMarshalType();

                    ClassType classType = ClassType.Struct;
                    if (classDecl != null && !classDecl.IsSimpleType &&
                        !string.IsNullOrEmpty(classDecl.AlternativeNamespace))
                    {
                        if (classDecl.ClassType == ClassType.Class)
                        {
                            classType = ClassType.Class;
                            type = classDecl.InnerStruct.Name;
                        }

                        type = $"{classDecl.AlternativeNamespace}.{type}";
                    }

                    textGenerator.WriteLine($"{parameter.Name} = new {type}[{currentArray.ArraySizeSource}];");
                    textGenerator.WriteLine(
                        $"MarshalUtils.IntPtrToManagedArray<{type}>({argumentName}, {parameter.Name});");
                    if ((CurrentTranslationUnit.Module.WrapInteropObjects && classType == ClassType.Struct) ||
                        classType == ClassType.Class)
                    {
                        textGenerator.WriteLine($"for (int i = 0; i< {arrayLength}; ++i)");
                        textGenerator.WriteOpenBraceAndIndent();
                        if (wrapInteropObjects && classType != ClassType.Class)
                        {
                            textGenerator.WriteLine($"{parameter.Name}[i] = new {type}({argumentName}[i]);");
                        }
                        else
                        {
                            textGenerator.WriteLine($"{parameter.Name}[i] = {argumentName}[i];");
                        }
                        
                        textGenerator.UnindentAndWriteCloseBrace();
                    }

                    textGenerator.WriteLine($"Marshal.FreeHGlobal({argumentName});");
                }

                void ConvertPtrToWrappedStructArray()
                {
                    TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                    var typeStrResult = parameter.Type.Visit(TypePrinter);
                    TypePrinter.PopMarshalType();
                    string interopNamespace;
                    if (classDecl.ClassType == ClassType.Class)
                    {
                        interopNamespace = $"{classDecl.InnerStruct.AlternativeNamespace}.{classDecl.InnerStruct.Name}";
                    }
                    else
                    {
                        interopNamespace = $"{classDecl.AlternativeNamespace}.{typeStrResult}";
                    }

                    textGenerator.WriteLine($"var _{parameter.Name} = new {interopNamespace}[{currentArray.ArraySizeSource}];");
                    textGenerator.WriteLine($"MarshalUtils.IntPtrToManagedArray<{interopNamespace}>({argumentName}, _{parameter.Name});");
                    textGenerator.WriteLine($"Marshal.FreeHGlobal({argumentName});");
                    textGenerator.WriteLine($"{parameter.Name} = new {typeStrResult}[{currentArray.ArraySizeSource}];");
                    textGenerator.WriteLine($"for (int i = 0; i< {arrayLength}; ++i)");
                    textGenerator.WriteOpenBraceAndIndent();
                    textGenerator.WriteLine($"{parameter.Name}[i] = _{parameter.Name}[i];");
                    textGenerator.UnindentAndWriteCloseBrace();
                }
            }

            if (!isVoid && actions.Count == 0)
            {
                textGenerator.Write("return ");
            }
            else if (!isVoid && actions.Count > 0)
            {
                textGenerator.Write("var result = ");
            }

            var @namespace = method.Function.AlternativeNamespace;

            var functionCall = $"{@namespace}.{CurrentTranslationUnit.Module.InteropClassName}.{method.Function.Name}({TypePrinter.VisitParameters(nativeParams, MarshalTypes.SkipParamTypes)});";
            textGenerator.WriteLine(functionCall);

            foreach (var action in actions)
            {
                action?.Invoke();
            }

            if (actions.Count > 0 && !isVoid)
            {
                textGenerator.WriteLine("return result;");
            }

            return textGenerator.ToString();
        }

        private string GetTypeNameFromParameter(Parameter parameter, Class classDecl)
        {
            TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
            var interopType = parameter.Type.Visit(TypePrinter).Type;
            if (parameter.WrappedType != null)
            {
                var wrappedType = parameter.WrappedType.Visit(TypePrinter).Type;
                if (interopType == wrappedType)
                {
                    interopType = $"{classDecl.Owner.FullNamespace}.{interopType}";
                }
            }
            TypePrinter.PopMarshalType();
            return interopType;
        }

        private void CheckParameters(IEnumerable<Parameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                AddUsingIfNeeded(parameter.Type);
            }
        }

        private void WriteLocation(Declaration declaration)
        {
            if (Options.DebugMode)
            {
                PushBlock(CodeBlockKind.DebugInfo);
                WriteLine($"// {declaration.Location}");
                PopBlock();
            }
        }
    }
}