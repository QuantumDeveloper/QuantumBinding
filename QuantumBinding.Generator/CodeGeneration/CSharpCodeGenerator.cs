using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using QuantumBinding.Generator.Utils;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.CodeGeneration;

public class CSharpCodeGenerator: CSharpCodeGeneratorBase
{
    protected const string DelegateInvokeFuncName = "InvokeFunc";
    protected const string DelegateNativePointerName = "NativePointer";
    protected string CounterProperty => "Length";
    protected const string UnmanagedWrapperInterfaceName = "IUnmanagedWrapper";
        
    public CSharpCodeGenerator(
        ProcessingContext context,
        IEnumerable<TranslationUnit> units,
        GeneratorCategory category)
        : base(context, units, category)
    {
        TypePrinter = new CSharpTypePrinter(context.Options);
        IsEmpty = false; 
    }

    public CSharpCodeGenerator(
        ProcessingContext context,
        TranslationUnit unit,
        GeneratorCategory category)
        : this(context, new List<TranslationUnit>() {unit}, category)
    {

    }

    public override string FolderName => Category.ToString();

    public override void Run()
    {
        if (Category == GeneratorCategory.Undefined)
        {
            IsEmpty = true;
            return;
        }
            
        PushBlock(CodeBlockKind.Root);
        Name = Category.ToString();
        GenerateFileHeader();
        foreach (var unit in TranslationUnits)
        {
            TypePrinter.PushModule(unit.Module);
            GenerateTranslationUnit(unit);
        }
        PopBlock();
    }

    public override void Run(Declaration declaration)
    {
        if (declaration == null || 
            Category == GeneratorCategory.Undefined || 
            declaration.IsIgnored)
        {
            IsEmpty = true;
            return;
        }

        Name = declaration.Name;
            
        PushBlock(CodeBlockKind.Root);
        GenerateFileHeader();
        var translationUnit = TranslationUnits.First();
        TypePrinter.PushModule(translationUnit.Module);
        GenerateTranslationUnitForDeclaration(translationUnit, declaration);
        PopBlock();
    }

    protected virtual void GenerateTranslationUnit(TranslationUnit unit)
    {
        CurrentTranslationUnit = unit;
        GenerateNamespace(unit);
    }

    protected virtual void GenerateTranslationUnitForDeclaration(TranslationUnit unit, Declaration declaration)
    {
        CurrentTranslationUnit = unit;
        GenerateNamespace(unit, declaration);
    }
        
    protected virtual void GenerateNamespace(Namespace @namespace, Declaration declaration)
    {
        try
        {
            PushBlock(CodeBlockKind.Namespace);
            GenerateUsings();
                
            NewLine();
                
            WriteCurrentNamespace(@namespace);

            NewLine();
                
            GenerateDeclaration(declaration);
            NewLine();
            
            PopBlock(NewLineStrategy.NewLineBeforeNextBlock);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected virtual bool IsDeclarationEqualsSpec(Declaration decl, GeneratorSpecializations spec)
    {
        switch (spec)
        {
            case GeneratorSpecializations.Classes when decl is Class @class && @class.ClassType == ClassType.Class:
            case GeneratorSpecializations.Structs when decl is Class @struct && @struct.ClassType == ClassType.Struct:
            case GeneratorSpecializations.Unions when decl is Class union && union.ClassType == ClassType.Union:
            case GeneratorSpecializations.Macros when decl is Macro:
            case GeneratorSpecializations.Enums when decl is Enumeration:
            case GeneratorSpecializations.Delegates when decl is Delegate:
                return true;
            default:
                return false;
        }
    }

    protected virtual void GenerateDeclaration(Declaration declaration)
    {
        switch (declaration)
        {
            case Enumeration @enum:
                GenerateEnum(@enum);
                break;
            case Class { ClassType: not (ClassType.StructWrapper and ClassType.UnionWrapper) } @class:
                if (Category == GeneratorCategory.ExtensionMethods)
                {
                    GenerateClassExtension(@class);
                }
                else
                {
                    GenerateClass(@class);
                }
                break;
            case Delegate @delegate:
                GenerateDelegate(@delegate);
                break;
            case Function function:
                GenerateFunction(function);
                break;
        }
    }

    protected virtual void GenerateNamespace(Namespace @namespace)
    {
        var dict = new Dictionary<GeneratorCategory, Action>
        {
            { GeneratorCategory.Macros, GenerateMacros },
            { GeneratorCategory.Enums, GenerateEnums },
            { GeneratorCategory.Structs, () => GenerateClasses(ClassType.Struct) },
            { GeneratorCategory.Unions, () => GenerateClasses(ClassType.Union) },
            { GeneratorCategory.Classes, () => GenerateClasses(ClassType.Class) },
            { GeneratorCategory.Delegates, GenerateDelegates },
            { GeneratorCategory.OldFashionDelegates, GenerateOldFashionDelegates },
            { GeneratorCategory.Functions, GenerateFunctions },
            { GeneratorCategory.StaticMethods, GenerateStaticMethods }
        };
            
        GenerateUsings();
            
        NewLine();

        PushBlock(CodeBlockKind.Namespace);
        WriteCurrentNamespace(@namespace);

        NewLine();
            
        dict[Category].Invoke();
        NewLine();

        foreach (var childNamespace in @namespace.Namespaces)
        {
            GenerateNamespace(childNamespace);
        }
            
        PopBlock(NewLineStrategy.NewLineBeforeNextBlock);
    }

    private void GenerateEnums()
    {
        foreach (var @enum in CurrentTranslationUnit.Enums)
        {
            GenerateEnum(@enum);
        }
    }

    private void GenerateClasses(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Struct:
                var structs = CurrentTranslationUnit.Structs.Where(x => !x.IsIgnored).ToArray();

                if (Options.PodTypesAsSimpleTypes && structs.Where(x=>!x.IsSimpleType).ToList().Count == 0 && Category == GeneratorCategory.Structs)
                {
                    IsEmpty = true;
                }

                foreach (var @struct in structs)
                {
                    GenerateClass(@struct);
                }

                break;
            case ClassType.Union:
                var unions = CurrentTranslationUnit.Unions.Where(x => !x.IsIgnored).ToArray();
                if (!unions.Any() && Category == GeneratorCategory.Unions)
                {
                    IsEmpty = true;
                }

                foreach (var union in unions)
                {
                    GenerateClass(union);
                }
                break;
            case ClassType.Class:
                var classes = CurrentTranslationUnit.Classes.Where(x => !x.IsIgnored).ToArray();

                foreach (var @class in classes)
                {
                    GenerateClass(@class);
                }

                var classExtensions = CurrentTranslationUnit.ExtensionClasses.Where(x => !x.IsIgnored).ToArray();
                foreach (var ext in classExtensions)
                {
                    GenerateClassExtension(ext);
                }

                if (classes.Length == 0 && CurrentTranslationUnit.StaticMethods.Count == 0 && classExtensions.Length == 0 && Category == GeneratorCategory.Classes)
                {
                    IsEmpty = true;
                }

                break;
        }
    }

    private void GenerateMacros()
    {
        var macros = CurrentTranslationUnit.Macros.Where(x => !x.IsIgnored).ToList();
        if (macros.Count == 0 && Category == GeneratorCategory.Macros)
        {
            IsEmpty = true;
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

    protected override void GenerateEnum(Enumeration @enum)
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

    private void GenerateStaticMethods()
    {
        if (CurrentTranslationUnit.Module.EachTypeInSeparateFile && CurrentTranslationUnit.StaticMethods.Count == 0)
        {
            IsEmpty = true;
            return;
        }
            
        PushBlock(CodeBlockKind.Class);
        WriteLine($"public unsafe static class {CurrentTranslationUnit.Module.MethodClassName}");
        WriteOpenBraceAndIndent();

        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
        foreach (var method in CurrentTranslationUnit.StaticMethods)
        {
            GenerateMethod(method);
        }
        TypePrinter.PopMarshalType();
            
        UnindentAndWriteCloseBrace();
            
        PopBlock();
    }

    protected override void GenerateClass(Class @class)
    {
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

        var classIdentifier = TypePrinter.VisitClass(@class).ToString();
        if (@class.ClassType == ClassType.Class)
        {
            WriteLine($"{classIdentifier} : {UnmanagedWrapperInterfaceName}<{@class.NativeStruct.FullName}>");
        }
        else
        {
            WriteLine($"{classIdentifier}");
        }

        WriteOpenBraceAndIndent();

        GenerateFields(@class);

        foreach (var fixedStructInfo in FixedStructInfos)
        {
            NewLine();
            GenerateConstArrayFixedWrapper(fixedStructInfo);
        }
        FixedStructInfos.Clear();
        FixedTypesHash.Clear();

        GenerateConstructors(@class);
            
        GenerateUnmanagedWrapperInterface(@class);

        GenerateProperties(@class);

        GenerateMethods(@class);
            
        GeneratePinnableReference(@class);

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

        WriteLine($"{TypePrinter.GetAccessSpecifier(extension.AccessSpecifier)} static unsafe partial class {extension.Name}Extension");
        WriteOpenBraceAndIndent();

        GenerateExtensionMethods(extension);

        UnindentAndWriteCloseBrace();

        NewLine();

        PopBlock();
    }

    private void GenerateUnmanagedWrapperInterface(Class @class)
    {
        if (@class.ClassType != ClassType.Class) 
            return;
            
        // We are assuming that the first field is always the native pointer to the inner struct
        WriteLine($"public {@class.NativeStruct.FullName} GetNativeValue() => {@class.Fields[0].Name};");
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
            if (property.HasSimpleGetterOnly)
            {
                WriteLine($"{propertyStr} => {property.Field.Name};");
            }
            else
            {
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
            }
                
            NewLine();

            PopBlock();
        }
    }

    protected void GenerateMethods(Class @class)
    {
        foreach (var method in @class.Methods.OrderBy(x=>x.Name))
        {
            GenerateMethod(method);
        }
    }

    protected void GeneratePinnableReference(Class @class)
    {
        if (@class.ClassType != ClassType.Class) return;
            
        WriteLine($"public ref readonly {@class.Fields[0].Type} GetPinnableReference() => ref {@class.Fields[0].Name};");
    }

    protected void GenerateExtensionMethods(Class @class)
    {
        foreach (var method in @class.ExtensionMethods)
        {
            // If the current translation unit is not the same file as the extension method, skip method generation
            if (CurrentTranslationUnit.FileName != method.Owner.FileName) continue;
            GenerateMethod(method);
        }
    }

    private string GetAccessSpecifierString(AccessSpecifier accessSpecifier)
    {
        return accessSpecifier.ToString().ToLower();
    }

    private void GenerateFunctions()
    {
        PushBlock(CodeBlockKind.Class);
        WriteLine($"{GetAccessSpecifierString(CurrentTranslationUnit.Module.InteropClassAccessSpecifier)} static unsafe partial class {CurrentTranslationUnit.Module.InteropClassName}");
        WriteOpenBraceAndIndent();
        string libraryPath = "LibraryPath";
            
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        var functions = CurrentTranslationUnit.Functions.OrderBy(x => x.Name).ToArray();

        if (CurrentTranslationUnit.Module.EachTypeInSeparateFile && functions.Length == 0)
        {
            IsEmpty = true;
            return;
        }
            
        WriteLine($"public const string {libraryPath} = \"{CurrentTranslationUnit.Module.LibraryName}\";");
        NewLine();

        foreach (var function in functions)
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
        
    protected override void GenerateFunction(Function function)
    {
        if (function.IsIgnored)
            return;

        PushBlock(CodeBlockKind.Function, function);

        WriteLocation(function);

        GenerateCommentIfNotEmpty(function.Comment);

        PushBlock(CodeBlockKind.Attribute);
        var dllImportString =
            $"[DllImport({function.DllName}, EntryPoint = \"{function.EntryPoint}\", ExactSpelling = true";
        if (CurrentTranslationUnit.Module.ForceCallingConvention)
        {
            dllImportString = $"{dllImportString}, CallingConvention = CallingConvention.{function.CallingConvention}";
        }
        dllImportString = $"{dllImportString})]";
            
        WriteLine(dllImportString);
        PopBlock();

        PushBlock(CodeBlockKind.AccessSpecifier);
        Write($"{TypePrinter.GetAccessSpecifier(function.AccessSpecifier)} static extern");
        PopBlock(NewLineStrategy.SpaceBeforeNextBlock);

        var returnType = function.ReturnType.Visit(TypePrinter);
        Write($"{returnType} {function.Name}(");
        CheckParameters(function.Parameters);

        var @params = TypePrinter.VisitParameters(function.Parameters, MarshalTypes.NativeParameter);
        Write(@params.ToString());
        Write(");");

        NewLine();

        PopBlock();
    }
        
    private void GenerateOldFashionDelegates()
    {
        WriteLine("public static unsafe class Delegates");
        WriteOpenBraceAndIndent();
        foreach (var @delegate in CurrentTranslationUnit.Delegates)
        {
            GenerateOldFashionDelegate(@delegate);
        }
        UnindentAndWriteCloseBrace();
    }
        
    protected override void GenerateOldFashionDelegate(Delegate @delegate)
    {
        if (@delegate.IsIgnored)
            return;

        PushBlock(CodeBlockKind.Delegate, @delegate);

        WriteLocation(@delegate);

        GenerateCommentIfNotEmpty(@delegate.Comment);

        PushBlock(CodeBlockKind.Attribute);
        WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{@delegate.CallingConvention})]");
        PopBlock();

        PushBlock(CodeBlockKind.AccessSpecifier);
        Write($"{TypePrinter.GetAccessSpecifier(@delegate.AccessSpecifier)} unsafe delegate");
        PopBlock(NewLineStrategy.SpaceBeforeNextBlock);

        TypePrinter.PushMarshalType(MarshalTypes.NativeReturnType);
        var returnType = @delegate.ReturnType.Visit(TypePrinter);
        Write($"{returnType} {@delegate.Name}(");
        TypePrinter.PopMarshalType();
        CheckParameters(@delegate.Parameters);

        var @params = TypePrinter.VisitParameters(@delegate.Parameters, MarshalTypes.DelegateParameter);
        Write(@params.ToString());
        Write(");");
        NewLine();

        PopBlock();
    }
        
    private void GenerateDelegates()
    {
        foreach (var @delegate in CurrentTranslationUnit.Delegates)
        {
            GenerateDelegate(@delegate);
        }
    }

    protected override void GenerateDelegate(Delegate @delegate)
    {
        if (@delegate.IsIgnored)
            return;

        PushBlock(CodeBlockKind.Delegate, @delegate);

        WriteLocation(@delegate);

        GenerateCommentIfNotEmpty(@delegate.Comment);

        string pointerArg = "ptr";
        TypePrinter.PushMarshalType(MarshalTypes.NativeReturnType);
        var returnType = @delegate.ReturnType.Visit(TypePrinter);
        TypePrinter.PopMarshalType();
        CheckParameters(@delegate.Parameters);
        var types = TypePrinter.VisitParameters(@delegate.Parameters, MarshalTypes.DelegateType);
        var nativeParams = TypePrinter.VisitParameters(@delegate.Parameters, MarshalTypes.DelegateParameter);
        var paramsWithoutTypes = TypePrinter.VisitParameters(@delegate.Parameters, MarshalTypes.SkipParamTypes);
            
        WriteLine($"{TypePrinter.GetAccessSpecifier(@delegate.AccessSpecifier)} unsafe struct {@delegate.Name}");
        WriteOpenBraceAndIndent();
            
        string delegateParams = ValidateDelegateParameters(types, returnType);

        if (CurrentTranslationUnit.Module.GeneratorMode == GeneratorMode.Compatible)
        {
            WriteDelegateWrapperConstructorCompatible(@delegate, pointerArg, delegateParams);
            NewLine();
            WriteInvokeFieldDelegateCompatible(delegateParams, "Stdcall");
            NewLine();
            WriteInvokeFieldDelegateCompatible(delegateParams, "Cdecl");
            NewLine();
            WriteNativePointerProperty();
            NewLine();
            WriteInvokeMethodDelegateCompatible(@delegate, @nativeParams, paramsWithoutTypes, returnType);
            NewLine();
            WriteStaticInvokeMethodDelegateCompatible(@delegate, @delegateParams, nativeParams, paramsWithoutTypes,
                returnType);
            WriteStaticInvokeMethodDelegateForNuintCompatible(@delegate, @delegateParams, nativeParams,
                paramsWithoutTypes,
                returnType);
        }
        else
        {
            WriteDelegateWrapperConstructorPreview(@delegate, pointerArg, delegateParams);
            NewLine();
            WriteInvokeFieldDelegatePreview(delegateParams);
            NewLine();
            WriteNativePointerProperty();
            NewLine();
            WriteInvokeMethodDelegatePreview(@delegate, @nativeParams, paramsWithoutTypes, returnType);
            WriteStaticInvokeMethodDelegatePreview(@delegate, @delegateParams, nativeParams, paramsWithoutTypes,
                returnType);
            WriteStaticInvokeMethodDelegateForNuintPreview(@delegate, @delegateParams, nativeParams, paramsWithoutTypes,
                returnType);
        }

        NewLine();
        WriteImplicitDelegateConversion(@delegate, pointerArg);
        UnindentAndWriteCloseBrace();
        PopBlock();
    }

    private void WriteDelegateWrapperConstructorCompatible(Delegate @delegate, string pointerArg, string delegateParams)
    {
        WriteLine($"public {@delegate.Name}(nuint {pointerArg}) : this((void*) {pointerArg}) {{ }}");
        NewLine();
        
        WriteLine($"public {@delegate.Name}(void* {pointerArg})");
        WriteOpenBraceAndIndent();
            
        WriteLine($"{DelegateNativePointerName} = {pointerArg};");
            
        WriteCheckForWindowsRuntimeString();
        WriteOpenBraceAndIndent();
        WriteLine($"InvokeStdcall = (delegate* unmanaged[Stdcall]<{delegateParams}>){pointerArg};");
        WriteLine($"InvokeCdecl = default;");
        UnindentAndWriteCloseBrace();
        WriteLine("else");
        WriteOpenBraceAndIndent();
        WriteLine($"InvokeCdecl = (delegate* unmanaged[Cdecl]<{delegateParams}>){pointerArg};");
        WriteLine($"InvokeStdcall = default;");
        UnindentAndWriteCloseBrace();
        UnindentAndWriteCloseBrace();
    }
        
    private void WriteDelegateWrapperConstructorPreview(Delegate @delegate, string pointerArg, string delegateParams)
    {
        WriteLine($"public {@delegate.Name}(nuint {pointerArg}) : this((void*) {pointerArg}) {{ }}");
        NewLine();
        WriteLine($"public {@delegate.Name}(void* {pointerArg})");
        WriteOpenBraceAndIndent();
        WriteLine($"{DelegateNativePointerName} = {pointerArg};");
        WriteLine($"{DelegateInvokeFuncName} = (delegate* unmanaged<{delegateParams}>){pointerArg};");
        UnindentAndWriteCloseBrace();
    }

    private void WriteInvokeFieldDelegateCompatible(string delegateParams, string callConv)
    {
        WriteLine($"private delegate* unmanaged[{callConv}]<{delegateParams}> Invoke{callConv};");
    }
        
    private void WriteInvokeFieldDelegatePreview(string delegateParams)
    {
        WriteLine($"private delegate* unmanaged<{delegateParams}> {DelegateInvokeFuncName};");
    }

    private void WriteNativePointerProperty()
    {
        WriteLine($"public void* {DelegateNativePointerName} {{ get; }}");
    }

    private void WriteInvokeMethodDelegateCompatible(Delegate @delegate, TypePrinterResult @params, TypePrinterResult argumentsOnly, TypePrinterResult returnType)
    {
        WriteLine($"public {returnType} Invoke({@params})");
        WriteOpenBraceAndIndent();
        WriteCheckForWindowsRuntimeString();
        WriteOpenBraceAndIndent();
        string returnKeyword = "return";
        if (@delegate.ReturnType.IsPrimitiveType(out var t) && t == PrimitiveType.Void)
        {
            returnKeyword = string.Empty;
        }
        WriteLine($"{returnKeyword} InvokeStdcall({argumentsOnly});");
        UnindentAndWriteCloseBrace();
        WriteLine("else");
        WriteOpenBraceAndIndent();
        WriteLine($"{returnKeyword} InvokeCdecl({argumentsOnly});");
        UnindentAndWriteCloseBrace();
        UnindentAndWriteCloseBrace();
    }
        
    private void WriteStaticInvokeMethodDelegateCompatible(Delegate @delegate, TypePrinterResult @delegateParams, TypePrinterResult @params, TypePrinterResult argumentsOnly, TypePrinterResult returnType)
    {
        TypePrinterResult localParams = @params.Type;
        localParams.Type = string.IsNullOrEmpty(localParams.Type) ? "void* ptr" : $"void* ptr, {localParams}";
        WriteLine($"public static {returnType} Invoke({localParams})");
        WriteOpenBraceAndIndent();
        string returnKeyword = "return";
        if (@delegate.ReturnType.IsPrimitiveType(out var t) && t == PrimitiveType.Void)
        {
            returnKeyword = string.Empty;
        }
        WriteCheckForWindowsRuntimeString();
        WriteOpenBraceAndIndent();
        WriteLine($"{returnKeyword} ((delegate* unmanaged[Stdcall]<{delegateParams}>)ptr)({argumentsOnly});");
        UnindentAndWriteCloseBrace();
        WriteLine("else");
        WriteOpenBraceAndIndent();
        WriteLine($"{returnKeyword} ((delegate* unmanaged[Cdecl]<{delegateParams}>)ptr)({argumentsOnly});");
        UnindentAndWriteCloseBrace();
        UnindentAndWriteCloseBrace();
    }
    
    private void WriteStaticInvokeMethodDelegateForNuintCompatible(Delegate @delegate, TypePrinterResult @delegateParams, TypePrinterResult @params, TypePrinterResult argumentsOnly, TypePrinterResult returnType)
    {
        TypePrinterResult localParams = @params.Type;
        localParams.Type = string.IsNullOrEmpty(@params.Type) ? "nuint ptr" : $"nuint ptr, {localParams}";
        WriteLine($"public static {returnType} Invoke({localParams})");
        WriteOpenBraceAndIndent();
        string returnKeyword = "return";
        if (@delegate.ReturnType.IsPrimitiveType(out var t) && t == PrimitiveType.Void)
        {
            returnKeyword = string.Empty;
        }
        WriteCheckForWindowsRuntimeString();
        WriteOpenBraceAndIndent();
        WriteLine($"{returnKeyword} ((delegate* unmanaged[Stdcall]<{delegateParams}>)(void*)ptr)({argumentsOnly});");
        UnindentAndWriteCloseBrace();
        WriteLine("else");
        WriteOpenBraceAndIndent();
        WriteLine($"{returnKeyword} ((delegate* unmanaged[Cdecl]<{delegateParams}>)(void*)ptr)({argumentsOnly});");
        UnindentAndWriteCloseBrace();
        UnindentAndWriteCloseBrace();
    }
        
    private void WriteInvokeMethodDelegatePreview(Delegate @delegate, TypePrinterResult @params, TypePrinterResult argumentsOnly, TypePrinterResult returnType)
    {
        WriteLine($"public {returnType} Invoke({@params})");
        WriteOpenBraceAndIndent();
        string returnKeyword = "return";
        if (@delegate.ReturnType.IsPrimitiveType(out var t) && t == PrimitiveType.Void)
        {
            returnKeyword = string.Empty;
        }
        WriteLine($"{returnKeyword} {DelegateInvokeFuncName}({argumentsOnly});");
        UnindentAndWriteCloseBrace();
    }
        
    private void WriteStaticInvokeMethodDelegatePreview(Delegate @delegate, TypePrinterResult @delegateParams, TypePrinterResult @params, TypePrinterResult argumentsOnly, TypePrinterResult returnType)
    {
        TypePrinterResult localParams = @params.Type;
        localParams.Type = string.IsNullOrEmpty(@params.Type) ? "void* ptr" : $"void* ptr, {localParams}";
        WriteLine($"public static {returnType} Invoke({localParams})");
        WriteOpenBraceAndIndent();
        string returnKeyword = "return";
        if (@delegate.ReturnType.IsPrimitiveType(out var t) && t == PrimitiveType.Void)
        {
            returnKeyword = string.Empty;
        }
        WriteLine($"{returnKeyword} ((delegate* unmanaged<{delegateParams}>)ptr)({argumentsOnly});");
        UnindentAndWriteCloseBrace();
    }
    
    private void WriteStaticInvokeMethodDelegateForNuintPreview(Delegate @delegate, TypePrinterResult @delegateParams, TypePrinterResult @params, TypePrinterResult argumentsOnly, TypePrinterResult returnType)
    {
        TypePrinterResult localParams = @params.Type;
        localParams.Type = string.IsNullOrEmpty(@params.Type) ? "nuint ptr" : $"nuint ptr, {localParams}";
        WriteLine($"public static {returnType} Invoke({localParams})");
        WriteOpenBraceAndIndent();
        string returnKeyword = "return";
        if (@delegate.ReturnType.IsPrimitiveType(out var t) && t == PrimitiveType.Void)
        {
            returnKeyword = string.Empty;
        }
        WriteLine($"{returnKeyword} ((delegate* unmanaged<{delegateParams}>)(void*)ptr)({argumentsOnly});");
        UnindentAndWriteCloseBrace();
    }

    private void WriteImplicitDelegateConversion(Delegate @delegate, string pointerArg)
    {
        WriteLine($"public static explicit operator {@delegate.Name}(void* {pointerArg}) => new({pointerArg});");
        NewLine();
        WriteLine($"public static explicit operator {@delegate.Name}(nuint {pointerArg}) => new({pointerArg});");
    }

    private void WriteCheckForWindowsRuntimeString()
    {
        WriteLine("if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))");
    }

    private string ValidateDelegateParameters(TypePrinterResult types, TypePrinterResult returnType)
    {
        string delegateParams;
        if (string.IsNullOrEmpty(types.ToString()))
        {
            delegateParams = returnType.ToString();
        }
        else
        {
            delegateParams = $"{types}, {returnType}";
        }

        return delegateParams;
    }

    private void GenerateMethod(Method method)
    {
        if (method.IsIgnored)
            return;
            
        PushBlock(CodeBlockKind.Method, method);
        GenerateCommentIfNotEmpty(method.Function.Comment);
        CheckParameters(method.Parameters);
        AddUsingIfNeeded(method.ReturnType);
        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);

        var methodResult = TypePrinter.VisitMethod(method);
        TypePrinter.PopMarshalType();
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
        var methodToRefStruct =
            new MethodToRefStructCodeGenerator(Options, CurrentTranslationUnit, MarshalToMethodName);
        return methodToRefStruct.GenerateMarshalContext(method);
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
        if (!Options.DebugMode) return;
            
        PushBlock(CodeBlockKind.DebugInfo);
        WriteLine($"// {declaration.Location}");
        PopBlock();
    }
        
    protected virtual void GenerateMethodMarshalContexts(Class @class)
    {
        if (@class.ClassType != ClassType.Class || @class.Methods.Count == 0) 
            return;
            
        var contextsToGenerate = @class.Methods.Where(x=>x.GenerateMarshalContext).ToList();

        foreach (var method in contextsToGenerate)
        {
            GenerateMethodContext(method);
        }
    }
        
    private void GenerateMethodContext(Method method)
    {
        WriteLine($"private unsafe ref struct {method.MarshalContextName}");
        WriteOpenBraceAndIndent();
            
        GenerateCalculateSizeMethod(method);
            
        NewLine();
            
        GenerateInvokeMethod(method);
            
        UnindentAndWriteCloseBrace();
    }

    void GenerateCalculateSizeMethod(Method method)
    {
        var parameters = method.Parameters
            .Where(x => x.IsAvailableForContextGeneration())
            .ToList();
        var parametersResult =
            TypePrinter.VisitParameters(parameters, MarshalTypes.SkipParamModifiers, method.IsExtensionMethod);

        string totalSizeName = "totalSize";
        WriteLine($"public static int CalculateSize({parametersResult})");
        WriteOpenBraceAndIndent();
        WriteLine($"int {totalSizeName} = 0;");
        foreach (var parameter in parameters)
        {
            if (parameter.Type.IsConstArray(out var size))
            {
                if (parameter.Type.IsConstArrayOfCustomTypes(out _) || parameter.Type.IsConstArrayOfEnums(out _))
                {
                    WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof({parameter.Type.Declaration.Name});");
                }
                else if (parameter.Type.IsConstArrayOfPrimitiveTypes(out _))
                {
                    var primitiveType = ((ArrayType)parameter.Type).ElementType as BuiltinType; 
                    WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof({primitiveType.Type.GetDisplayName()});");
                }
            }
            else if (parameter.Type.IsPointerToString())
            {
                if (parameter.Type.IsStringArray())
                {
                    WriteLine($"{totalSizeName} += {TextGenerator.MarshalContextCalculateSizeForStringArray}({parameter.Name});");
                }
                else
                {
                    WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof(byte) + 1;");
                }
            }
            else if (parameter.Type.IsPointerToArrayOfSimpleTypes(out var type))
            {
                WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof({type.Name});");
            }
            else if (parameter.Type.IsPointerToArrayOfPrimitiveTypes(out var elementType))
            {
                WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof({elementType.Type});");
            }
            else if (parameter.Type.IsPointerToArrayOfEnums())
            {
                var @enum = parameter.Type.Declaration as Enumeration; 
                WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof({@enum.InheritanceType});");
            }
            else if (parameter.Type.IsPointerToArray() || parameter.Type.IsArrayOfCustomTypes())
            {
                WriteForLoop($"{parameter.Name}.Length",
                    () => { WriteLine($"{totalSizeName} += {parameter.Name}[(int)i].GetSize();"); });
            }
            else if (parameter.Type.IsWrapper())
            {
                WriteLine($"if ({parameter.Name} != null);");
                PushIndent();
                WriteLine($"{totalSizeName} += {parameter.Name}.GetSize();");
                PopIndent();
            }
        }

        WriteLine($"return {totalSizeName};");
        UnindentAndWriteCloseBrace();
    }

    void WriteForLoop(string size, Action action)
    {
        WriteLine($"for (var i = 0U; i < {size}; i++)");
        WriteOpenBraceAndIndent();
        action();
        UnindentAndWriteCloseBrace();
    }

    void GenerateInvokeMethod(Method method)
    {
        var contextMarshaler = new MarshalContextToFunctionCodeGenerator(Options, CurrentTranslationUnit, MarshalToMethodName);
        var result = contextMarshaler.GenerateMarshalBody(method);
        WriteLine(result.ToString());
    }
}