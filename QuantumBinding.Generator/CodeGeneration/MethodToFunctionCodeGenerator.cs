using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.CodeGeneration;

public class MethodToFunctionCodeGenerator : TextGenerator
{
    private Method method;
    List<Action> postActions;

    public TypePrinter TypePrinter { get; }

    public TranslationUnit CurrentTranslationUnit { get; }

    protected string ConversionMethodName { get; set; }

    protected bool WrapInteropObjects { get; set; }
    
    protected  BindingOptions Options { get; set; }

    private int currentParameterIndex;
    private List<Parameter> nativeParams;

    public MethodToFunctionCodeGenerator(
        BindingOptions options, 
        TranslationUnit translationUnit,
        string conversionMethodName)
    {
        postActions = new List<Action>();
        Options = options;
        TypePrinter = new CSharpTypePrinter(options);
        CurrentTranslationUnit = translationUnit;
        ConversionMethodName = conversionMethodName;
        WrapInteropObjects = CurrentTranslationUnit.Module.WrapInteropObjects;
    }

    public TypePrinterResult GenerateMethodBody(Method method)
    {
        this.method = method;

        if (this.method.Name.Contains("parseTranslationUnit2"))
        {
            int bug = 0;
        }
        
        Clear();

        nativeParams = new List<Parameter>();

        bool isVoid = method.ReturnType.IsPrimitiveTypeEquals(PrimitiveType.Void);
        int index = 0;
        var wrapInteropObjects = CurrentTranslationUnit.Module.WrapInteropObjects;
        var isInstanceMethod = method.Class != null;

        for (var paramIndex = 0; paramIndex < method.Function.Parameters.Count; paramIndex++)
        {
            currentParameterIndex = paramIndex;
            var parameter = method.Function.Parameters[currentParameterIndex];
            var classDecl = parameter.Type.Declaration as Class;
            if (wrapInteropObjects)
            {
                if (classDecl is { ClassType: ClassType.Struct or ClassType.Union })
                {
                    if (currentParameterIndex == 0 && isInstanceMethod)
                    {
                        classDecl = method.Class;
                    }
                    else
                    {
                        var p = method.Parameters.FirstOrDefault(x => x.Id == parameter.Id);
                        classDecl = p.Type.Declaration as Class;
                    }
                }
            }

            if (parameter.Type.IsPointerToBuiltInType(out var primType))
            {
                classDecl = null;
            }
            
            var argumentName = $"arg{index++}";
            if (classDecl == null)
            {
                if (parameter.Type.IsConstArray(out int arraySize))
                {
                    WriteLine(
                        $"if ({parameter.Name} == null || {parameter.Name}.Length != {arraySize})");
                    WriteOpenBraceAndIndent();
                    WriteLine(
                        $"throw new ArgumentOutOfRangeException(\"{parameter.Name}\", \"The dimensions of the provided array don't match the required size. Size should be = {arraySize}\");");
                    UnindentAndWriteCloseBrace();
                }

                if (parameter.Type.IsPointerToString())
                {
                    if (parameter.Type.IsStringArray())
                    {
                        WritePointerToStringArray(parameter, argumentName);
                    }
                    else
                    {
                        WritePointerToString(parameter, argumentName);
                    }
                    
                    nativeParams.Add(new Parameter()
                    {
                        Name = $"{argumentName}", ParameterKind = parameter.ParameterKind, Type = parameter.Type
                    });
                }
                else if (parameter.Type.IsPointerToBuiltInType(out var prim) && !parameter.Type.IsPurePointer())
                {
                    WritePointerToPrimitiveType(parameter, argumentName);
                    var nativeParam = new Parameter()
                    {
                        ParameterKind = parameter.ParameterKind, Type = parameter.Type
                    };
                    switch (parameter.ParameterKind)
                    {
                        case ParameterKind.Out:
                            nativeParam.Name = parameter.Name;
                            break;
                        default:
                            nativeParam.Name = argumentName;
                            break;
                    }
                    nativeParams.Add(nativeParam);

                    continue;
                }
                // Input parameter is void*, so we need to just pass it as is without any conversion
                else if (parameter.Type.IsPointerToIntPtr() || parameter.Type.IsPurePointer())
                {
                    nativeParams.Add(parameter);
                    continue;
                }
                else if (parameter.Type.Declaration == null || !parameter.Type.IsPointer())
                {
                    nativeParams.Add(parameter);
                    continue;
                }
            }
            
            if (classDecl != null)
            {
                if (classDecl.ClassType == ClassType.Class && !parameter.Type.IsPointerToArray())
                {
                    if (classDecl == method.Class && parameter.Index == 0)
                    {
                        if (!method.IsStatic)
                        {
                            argumentName = "this"; // pass this as first parameter to avoid additional copying of memory
                        }
                    }
                    WriteClass(parameter, argumentName, classDecl);
                }
                else //arrays of structs, structs, unions, PODs (plain old data => primitive types)
                {
                    var type = parameter.Type;
                    if (type.IsPointer())
                    {
                        if (type.IsPointerToArray(out var arrayType, out var depth))
                        {
                            WritePointerToArray(parameter, argumentName, classDecl, arrayType, depth);
                        }
                        else if (type.IsPointerToStructOrUnion() && !classDecl.IsSimpleType)
                        {
                            WritePointerToStruct(parameter, argumentName, classDecl);
                        }
                        else //Pointer to simple type like int, long, float, etc.
                        {
                            WritePointerToSimpleType(parameter, argumentName, classDecl);
                        }
                    }
                    else if (classDecl.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper)
                    {
                        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly &&
                            currentParameterIndex == 0 &&
                            isInstanceMethod)
                        {
                            argumentName = $"{ConversionMethodName}";
                            CreateNativeParameter(parameter, argumentName, classDecl);
                        }
                        else
                        {
                            WriteWrappedStruct(parameter, argumentName, classDecl);
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
                            WriteLine(
                                $"{parameter.Type.Declaration.Namespace}.{t} {argumentName};");

                            postActions.Add(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
                        }
                        else
                        {
                            argumentName = parameter.Name;
                        }

                        CreateNativeParameter(parameter, argumentName, classDecl);
                    }
                }
            }
            else if (parameter.Type.Declaration is Enumeration enumeration)
            {
                WriteEnumeration(parameter, argumentName, enumeration);
            }
        }

        switch (isVoid)
        {
            case false when postActions.Count == 0 && method.ReturnType.IsString():
                Write("var result = ");
                break;
            case false when method.ReturnType.IsPointerToStructOrUnion():
                Write("var result = ");
                postActions.Add(ConvertReturnType);
                break;
            case false when postActions.Count == 0:
                Write("return ");
                break;
            case false when postActions.Count > 0:
                Write("var result = ");
                break;
        }

        var @namespace = method.Function.Namespace;

        var functionCall =
            $"{@namespace}.{CurrentTranslationUnit.Module.InteropClassName}.{method.Function.Name}({TypePrinter.VisitParameters(nativeParams, MarshalTypes.NativeFunctionCall)});";
        WriteLine(functionCall);

        foreach (var action in postActions)
        {
            action?.Invoke();
        }

        if (postActions.Count > 0 && !isVoid && !method.ReturnType.IsPointerToStructOrUnion())
        {
            if (method.ReturnType.IsString())
            {
                WriteLine($"return new string(result);");
            }
            else
            {
                WriteLine("return result;");
            }
        }
        else if (method.ReturnType.IsString())
        {
            WriteLine($"return new string(result);");
        }

        return ToString();
    }

    private void ConvertReturnType()
    {
        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
        var wrappedType = method.ReturnType.Visit(TypePrinter);
        TypePrinter.PopMarshalType();
        WriteLine($"var wrappedResult = new {wrappedType.Type}(*result);");
        FreeNativePointer("result");
        WriteLine("return wrappedResult;");
    }

    private void CreateNativeParameter(Parameter parameter, string argumentName, Class classDecl)
    {
        var clonedType = (BindingType)parameter.Type.Clone();
        clonedType.Declaration = classDecl;
        
        nativeParams.Add(new Parameter()
            { Name = argumentName, IsOverload = parameter.IsOverload, ParameterKind = parameter.ParameterKind, Type = clonedType });
    }

    private void WritePointerToString(Parameter parameter, string argumentName)
    {
        var isUnicode = parameter.Type.IsUnicodeString();
        var isUnicodeString = isUnicode.ToString().ToLower();
        var ptr = parameter.Type as PointerType;
        var pointerString = GetPointerString(ptr.GetDepth());
        var conversionString = isUnicode ? $"char{pointerString}" : $"sbyte{pointerString}";
        WriteLine($"var {argumentName} = ({conversionString}){NativeUtilsStringToPointer}({parameter.Name}, {isUnicodeString});");

        postActions.Add( () => FreeNativePointer(argumentName));
    }
    
    private void WritePointerToStringArray(Parameter parameter, string argumentName)
    {
        var isUnicode = parameter.Type.IsUnicodeString();
        var isUnicodeString = isUnicode.ToString().ToLower();
        var ptr = parameter.Type as PointerType;
        var pointerString = GetPointerString(ptr.GetDepth());
        var conversionString = isUnicode ? $"char{pointerString}" : $"sbyte{pointerString}"; 
        WriteLine($"var {argumentName} = ({conversionString}){NativeUtilsStringArrayToPointer}({parameter.Name}, {isUnicodeString});");
        
        postActions.Add( () => FreeNativePointer(argumentName));
    }

    private void WriteEnumeration(Parameter parameter, string argumentName, Enumeration enumeration)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        var nativeType = parameter.Type.Visit(TypePrinter);
        TypePrinter.PopMarshalType();
        if (parameter.Type.IsPointerToArray())
        {
            if (parameter.Type.IsDoublePointer())
            {
                WriteLine($"var {argumentName} = ({nativeType}){NativeUtilsArrayToPointer}({parameter.Name});");
            }
            else
            {
                WriteLine($"var {argumentName} = {NativeUtilsArrayToPointer}({parameter.Name});");
            }
            
            if (parameter.ParameterKind is ParameterKind.Ref or ParameterKind.Out)
            {
                postActions.Add(() => ConvertPointerToArrayOfEnums(parameter, argumentName));
            }

            postActions.Add(() => FreeNativePointer(argumentName));
            
            nativeParams.Add(new Parameter()
            {
                Name = $"{argumentName}", IsOverload = parameter.IsOverload, ParameterKind = parameter.ParameterKind, Type = parameter.Type
            });
        }
        else if (parameter.Type.IsPointer())
        {
            if (parameter.Type.IsDoublePointer())
            {
                WriteLine($"var {argumentName} = ({nativeType}){NativeUtilsStructOrEnumToPointer}({parameter.Name});");
            }
            else
            {
                if (parameter.ParameterKind is not ParameterKind.Out)
                {
                    WriteLine($"var {argumentName} = {NativeUtilsStructOrEnumToPointer}({parameter.Name});");
                }
                else
                {
                    argumentName = parameter.Name;
                }
            }
            
            if (parameter.ParameterKind is ParameterKind.Ref)
            {
                postActions.Add(() => ConvertPointerToEnum(parameter, argumentName));
            }

            if (parameter.ParameterKind is not ParameterKind.Out)
            {
                postActions.Add(() => FreeNativePointer(argumentName));
            }
            
            nativeParams.Add(new Parameter()
            {
                Name = $"{argumentName}", IsOverload = parameter.IsOverload, ParameterKind = parameter.ParameterKind, Type = parameter.Type
            });
        }
    }

    public void WriteClass(Parameter parameter, string argumentName, Class classDecl)
    {
        if (parameter.ParameterKind != ParameterKind.Out && !parameter.Type.IsPointerToArray())
        {
            if (classDecl == method.Class && parameter.Index == 0)
            {
                if (method.IsStatic)
                {
                    WriteLine($"var {argumentName} = {parameter.Name};");
                }
            }
            else
            {
                if (!parameter.Type.IsPointer())
                {
                    TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? new {parameter.Type.Visit(TypePrinter)}() : ({classDecl.InnerStruct.Name}){parameter.Name};");
                    TypePrinter.PopMarshalType();
                }
                else
                {
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : {NativeUtilsStructOrEnumToPointer}(({classDecl.InnerStruct.Name}){parameter.Name});");
                    if (parameter.ParameterKind == ParameterKind.Ref)
                    {
                        postActions.Add(() => ConvertPointerToClassOrStruct(parameter, argumentName, classDecl));
                    }

                    postActions.Add(() => FreeNativePointer(argumentName));
                }
            }
        }
        else if (parameter.Type.IsPointerToArray())
        {
            var pointer = parameter.Type as PointerType;
            var currentArray = pointer.Pointee as ArrayType;
            var arrayLength = currentArray.ArraySizeSource;
            if (string.IsNullOrEmpty(arrayLength))
            {
                arrayLength = $"{parameter.Name}.Length";
            }

            if (parameter.ParameterKind != ParameterKind.Out)
            {
                WriteLine(
                    $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : new {classDecl.InnerStruct.Namespace}.{classDecl.InnerStruct.Name}[{arrayLength}];");

                if (parameter.ParameterKind == ParameterKind.Ref)
                {
                    postActions.Add(
                        () => ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength));
                }
                else if (parameter.ParameterKind == ParameterKind.In)
                {
                    ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength);
                }
            }
            else if (parameter.ParameterKind == ParameterKind.Out)
            {
                if (!string.IsNullOrEmpty(currentArray.ArraySizeSource)
                    && CurrentTranslationUnit.Module.WrapInteropObjects)
                {
                    //TODO check this!
                    WriteLine($"IntPtr {argumentName} = IntPtr.Zero;");
                    postActions.Add(() => ConvertPtrToWrappedStructArray(parameter, argumentName, classDecl, currentArray));
                }
                else if (!string.IsNullOrEmpty(currentArray.ArraySizeSource)
                         && !CurrentTranslationUnit.Module.WrapInteropObjects)
                {
                    WriteLine(
                        $"var {argumentName} = new {classDecl.InnerStruct.Namespace}.{classDecl.InnerStruct.Name}[{arrayLength}];");
                    postActions.Add(
                        () => ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength));
                }
                else
                {
                    // TODO check this code for completeness
                    argumentName = parameter.Name;
                }
            }
        }
        else
        {
            TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
            TypePrinter.PushParameter(parameter);
            var argumentType = parameter.Type.Visit(TypePrinter);
            TypePrinter.PopParameter();
            TypePrinter.PopMarshalType();
            WriteLine($"{argumentType} {argumentName};");
            postActions.Add(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }

    void WriteWrappedStruct(Parameter parameter, string argumentName, Class classDecl)
    {
        string interopType = string.Empty;
        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
        try
        {
            interopType = parameter.WrappedType.Visit(TypePrinter).Type;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        TypePrinter.PopMarshalType();

        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly or ParameterKind.Ref)
        {
            WriteLine(
                $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? new {classDecl.WrappedStruct.Namespace}.{interopType}() : {parameter.Name}.{ConversionMethodName};");
            postActions.Add(() => DisposeWrapper(parameter, classDecl));
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            if (WrapInteropObjects)
            {
                WriteLine($"{parameter.WrappedType.Declaration.Name} {argumentName};");
                postActions.Add(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
            }
            else
            {
                argumentName = parameter.Name;
            }
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }
    
    void WritePointerToStruct(Parameter parameter, string argumentName, Class classDecl)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        TypePrinter.PushParameter(parameter);
        var interopType = parameter.Type.Visit(TypePrinter);
        interopType = $"{parameter.Type.Declaration.Namespace}.{interopType}";
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();

        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly or ParameterKind.Ref)
        {
            switch (classDecl.ClassType)
            {
                case ClassType.Struct:
                case ClassType.Union:
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : {NativeUtilsStructOrEnumToPointer}({parameter.Name});");

                    break;
                case ClassType.StructWrapper:
                case ClassType.UnionWrapper:
                    if (currentParameterIndex == 0 && method.IsInstanceMethod)
                    {
                        WriteLine(
                            $"var {argumentName} = {NativeUtilsStructOrEnumToPointer}({ConversionMethodName});");
                    }
                    else
                    {
                        if (parameter.Type.IsDoublePointer())
                        {
                            WriteLine(
                                $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : ({interopType}){NativeUtilsStructOrEnumToPointer}({parameter.Name}.{ConversionMethodName});");
                        }
                        else
                        {
                            WriteLine(
                                $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : {NativeUtilsStructOrEnumToPointer}({parameter.Name}.{ConversionMethodName});");
                        }
                        
                        postActions.Add(() => DisposeWrapper(parameter, classDecl));
                    }

                    break;
            }

            if (parameter.ParameterKind == ParameterKind.Ref)
            {
                postActions.Add(() => ConvertPointerToClassOrStruct(parameter, argumentName, classDecl));
            }

            postActions.Add(() => FreeNativePointer(argumentName));
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            if (WrapInteropObjects)
            {
                WriteLine($"{interopType} {argumentName};");
                postActions.Add(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
            }
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }

    void WritePointerToSimpleType(Parameter parameter, string argumentName, Class classDecl)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        var nativeType = parameter.Type.Visit(TypePrinter);
        TypePrinter.PopMarshalType();
        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly or ParameterKind.Ref)
        {
            switch (classDecl.ClassType)
            {
                case ClassType.Struct:
                case ClassType.Union:
                    WriteLine(
                        parameter.Type.IsDoublePointer()
                            ? $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : ({nativeType}){NativeUtilsStructOrEnumToPointer}({parameter.Name});"
                            : $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : {NativeUtilsStructOrEnumToPointer}({parameter.Name});");
                    break;
                case ClassType.StructWrapper:
                case ClassType.UnionWrapper:
                    WriteLine(
                        parameter.Type.IsDoublePointer()
                            ? $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : ({nativeType}){NativeUtilsStructOrEnumToPointer}({parameter.Name}.{ConversionMethodName});"
                            : $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? {NullPointer} : {NativeUtilsStructOrEnumToPointer}({parameter.Name}.{ConversionMethodName});");

                    postActions.Add(() => DisposeWrapper(parameter, classDecl));
                    break;
            }

            if (parameter.ParameterKind == ParameterKind.Ref)
            {
                postActions.Add(() => ConvertPointerToClassOrStruct(parameter, argumentName, classDecl));
            }

            postActions.Add(() => FreeNativePointer(argumentName));
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            if (WrapInteropObjects && classDecl.WrappedStruct != null)
            {
                WriteLine($"{classDecl.WrappedStruct.Name} {argumentName};");
                postActions.Add(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
            }
            else
            {
                argumentName = parameter.Name;
            }
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }

    void WritePointerToPrimitiveType(Parameter parameter, string argumentName)
    {
        if (parameter.ParameterKind is not ParameterKind.Out)
        {
            WriteLine($"var {argumentName} = {NativeUtilsStructOrEnumToPointer}({parameter.Name});");
        }

        if (parameter.ParameterKind is ParameterKind.Ref)
        {
            postActions.Add(() => ConvertOutPrimitiveTypePointerToValue(parameter, argumentName));
            postActions.Add(() => FreeNativePointer(argumentName));
        }
    }

    void ConvertOutPrimitiveTypePointerToValue(Parameter parameter, string argumentName)
    {
        WriteLine($"{parameter.Name} = *{argumentName};");
    }

    void WritePointerToArray(Parameter parameter, string argumentName, Class classDecl, ArrayType arrayType, uint pointerDepth)
    {
        var arraySizeSource = arrayType.ArraySizeSource;
        var arrayLength = $"{parameter.Name}.Length";
        if (!string.IsNullOrEmpty(arraySizeSource))
        {
            arrayLength = arraySizeSource;
        }

        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        TypePrinter.PushParameter(parameter);
        var typeStrResult = parameter.Type.Visit(TypePrinter);
        if (WrapInteropObjects && classDecl.WrappedStruct != null)
        {
            typeStrResult.Type = classDecl.WrappedStruct.Name;
        }
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();
        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly)
        {
            if (classDecl.ClassType == ClassType.Class)
            {
                WriteLine(
                    $" var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : {NativeUtilsGetPointerToArray}<{classDecl.InnerStruct.Namespace}.{classDecl.InnerStruct.Name}>({arrayLength});");
                ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength);
            }
            else
            {
                if (WrapInteropObjects && classDecl.IsWrapper && classDecl.WrappedStruct != null)
                {
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : {NativeUtilsGetPointerToArray}<{classDecl.WrappedStruct.Namespace}.{typeStrResult.Type}>({arrayLength});");
                    ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength);
                }
                else // assume that type is primitive or we are using structs/unions and conversion is not needed  
                {
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : {NativeUtilsArrayToPointer}({parameter.Name});");
                }
            }
        }
        else if (parameter.ParameterKind == ParameterKind.Ref)
        {
            if (classDecl.IsSimpleType)
            {
                WriteLine(
                    $"var {argumentName} = {NativeUtilsArrayToPointer}({parameter.Name});");
            }
            else if (classDecl.ClassType == ClassType.Class)
            {
                WriteLine(
                    $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : {NativeUtilsGetPointerToArray}<{classDecl.InnerStruct.Namespace}.{classDecl.InnerStruct.Name}>({arrayLength});");
            }
            else
            {
                if (classDecl.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper)
                {
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : {NativeUtilsGetPointerToArray}<{classDecl.WrappedStruct.Namespace}.{typeStrResult.Type}>({arrayLength});");
                }
                else
                {
                    WriteLine(
                        $"var {argumentName} = ReferenceEquals({parameter.Name}, null) ? null : {NativeUtilsGetPointerToArray}<{classDecl.Namespace}.{typeStrResult.Type}>({arrayLength});");
                }
            }

            postActions.Add(() => ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength));
            postActions.Add(() => FreeNativePointer(argumentName));
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            if (classDecl.IsWrapper)
            {
                WriteLine($"{classDecl.WrappedStruct.Namespace}.{typeStrResult} {argumentName} = {NullPointer};");
                postActions.Add(() => ConvertPtrToWrappedStructArray(parameter, argumentName, classDecl, arrayType));
            }
            else
            {
                WriteLine($"{typeStrResult} {argumentName} = {NativeUtilsGetPointerToArray}<{typeStrResult.Type}>({arrayLength});");
                postActions.Add(() => ConvertPointerToArray(parameter, argumentName, classDecl, arrayType));
            }
            
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }

    void ConvertPointerToArrayOfEnums(Parameter parameter, string argumentName)
    {
        WriteLine($"{NativeUtilsWritePointerToManagedArray}({argumentName}, {parameter.Name});");
    }


    void ConvertPointerToArray(Parameter parameter, string argumentName, Class classDecl, ArrayType arrayType)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        var type = parameter.Type.Visit(TypePrinter).Type;
        TypePrinter.PopMarshalType();
        
        var arrayLength = $"{parameter.Name}.Length";
        if (!string.IsNullOrEmpty(arrayType.ArraySizeSource))
        {
            arrayLength = arrayType.ArraySizeSource;
        }

        var classType = ClassType.Struct;
        if (classDecl is { IsSimpleType: false } &&
            !string.IsNullOrEmpty(classDecl.Namespace))
        {
            if (classDecl.ClassType == ClassType.Class)
            {
                classType = ClassType.Class;
            }
            type = $"{classDecl.FullName}";
        }
        

        WriteLine($"{parameter.Name} = new {type}[{arrayType.ArraySizeSource}];");
        
        if ((CurrentTranslationUnit.Module.WrapInteropObjects && classType == ClassType.Struct) ||
            classType == ClassType.Class)
        {
            WriteLine($"for (var i = 0U; i < {arrayLength}; ++i)");
            WriteOpenBraceAndIndent();
            if (WrapInteropObjects && classType != ClassType.Class)
            {
                WriteLine($"{parameter.Name}[i] = new {type}({argumentName}[i]);");
            }
            else
            {
                WriteLine($"{parameter.Name}[i] = {argumentName}[i];");
            }

            UnindentAndWriteCloseBrace();
        }

        FreeNativePointer(argumentName);
    }

    void FreeNativePointer(string argumentName)
    {
        WriteLine($"NativeUtils.Free({argumentName});");
    }

    void DisposeWrapper(Parameter parameter, Class classDecl)
    {
        if (classDecl.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper && classDecl.IsDisposable)
        {
            WriteLine($"{parameter.Name}?.Dispose();");
        }
    }

    void ImplicitTwoWayArrayTypeConversion(Parameter parameter, Class classDecl, string argumentName, string arrayLength)
    {
        if (parameter.ParameterKind != ParameterKind.Out)
        {
            WriteLine($"if (!ReferenceEquals({parameter.Name}, null))");
            WriteOpenBraceAndIndent();
            InnerArrayCopy();
            UnindentAndWriteCloseBrace();
        }
        else
        {
            WriteLine($"{parameter.Name} = new {classDecl.Name}[{arrayLength}];");
            InnerArrayCopy();
        }

        void InnerArrayCopy()
        {
            WriteLine($"for (var i = 0U; i < {arrayLength}; ++i)");
            WriteOpenBraceAndIndent();
            if (parameter.ParameterKind != ParameterKind.In && parameter.ParameterKind != ParameterKind.Readonly)
            {
                if (WrapInteropObjects && !classDecl.IsSimpleType)
                {
                    WriteLine($"{parameter.Name}[i] = new {classDecl.Name}({argumentName}[i]);");
                }
                else
                {
                    WriteLine($"{parameter.Name}[i] = {argumentName}[i];");
                }
            }
            else
            {
                if (WrapInteropObjects && classDecl.ClassType != ClassType.Class)
                {
                    WriteLine($"{argumentName}[i] = {parameter.Name}[i].{ConversionMethodName};");
                    postActions.Add(() => DisposeWrappedArray(parameter, classDecl, arrayLength));
                }
                else
                {
                    WriteLine($"{argumentName}[i] = {parameter.Name}[i];");
                }
            }

            UnindentAndWriteCloseBrace();
        }
    }

    void DisposeWrappedArray(Parameter parameter, Class classDecl, string arrayLength)
    {
        if (classDecl.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper && classDecl.IsDisposable)
        {
            if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly)
            {
                WriteLine($"if (!ReferenceEquals({parameter.Name}, null))");
                WriteOpenBraceAndIndent();
                WriteLine($"for (var i = 0U; i < {arrayLength}; ++i)");
                WriteOpenBraceAndIndent();

                WriteLine($"{parameter.Name}[i]?.Dispose();");

                UnindentAndWriteCloseBrace();
                UnindentAndWriteCloseBrace();
            }
        }
    }

    void ConvertPointerToClassOrStruct(Parameter parameter, string argumentName, Class classDecl)
    {
        WriteLine($"if ({argumentName} is not null)");
        WriteOpenBraceAndIndent();
        if (classDecl.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper)
        {
            WriteLine($"{parameter.Name} = new {classDecl.Owner.FullNamespace}.{classDecl.Name}(*{argumentName});");
        }
        else
        {
            WriteLine($"{parameter.Name} = *{argumentName};");
        }
        UnindentAndWriteCloseBrace();
        WriteDefaultOutParameterInitialization(parameter);
    }
    
    void ConvertPointerToEnum(Parameter parameter, string argumentName)
    {
        WriteLine($"if ({argumentName} is not null)");
        WriteOpenBraceAndIndent();
        WriteLine($"{parameter.Name} = *{argumentName};");
        UnindentAndWriteCloseBrace();
        WriteDefaultOutParameterInitialization(parameter);
    }

    void ConvertOutStructToClass(Parameter parameter, string argumentName, Class classDecl)
    {
        if (WrapInteropObjects)
        {
            var wrapperType = classDecl.Name;
            WriteLine($"{parameter.Name} = new {wrapperType}({argumentName});");
        }
        else
        {
            WriteLine($"{parameter.Name} = {argumentName};");
        }
    }

    void WriteDefaultOutParameterInitialization(Parameter parameter)
    {
        if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteLine("else");
            WriteOpenBraceAndIndent();
            WriteLine($"{parameter.Name} = default;");
            UnindentAndWriteCloseBrace();
        }
    }

    void ConvertPtrToWrappedStructArray(Parameter parameter, string argumentName, Class classDecl, ArrayType array)
    {
        var arrayLength = array.ArraySizeSource;
        if (string.IsNullOrEmpty(arrayLength))
        {
            arrayLength = $"{parameter.Name}.Length";
        }

        WriteLine($"var _{parameter.Name} = {NativeUtilsPointerToArray}({argumentName}, (long){arrayLength});");

        if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteLine($"{parameter.Name} = new {classDecl.FullName}[{arrayLength}];");
        }
        else if (parameter.ParameterKind == ParameterKind.Ref)
        {
            WriteLine($"if({parameter.Name} == null)");
            WriteOpenBraceAndIndent();
            WriteLine($"{parameter.Name} = new {classDecl.FullName}[{arrayLength}];");
            UnindentAndWriteCloseBrace();
        }

        WriteLine($"for (var i = 0U; i< {arrayLength}; ++i)");
        WriteOpenBraceAndIndent();
        WriteLine($"{parameter.Name}[i] = new {classDecl.FullName}(_{parameter.Name}[i]);");
        UnindentAndWriteCloseBrace();

        if (parameter.ParameterKind != ParameterKind.Out)
        {
            FreeNativePointer(argumentName);
        }
    }
}