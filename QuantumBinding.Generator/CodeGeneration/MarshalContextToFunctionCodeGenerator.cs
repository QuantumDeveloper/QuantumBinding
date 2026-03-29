using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.CodeGeneration;

public class MarshalContextToFunctionCodeGenerator : TextGenerator
{
    private int _currentParameterIndex;
    private string _spanBufferName = "mainBuffer";
    private string _spanBufferCursorName = "currentCursor";
    private bool _isInstanceMethod;
    
    public TypePrinter TypePrinter { get; }

    public TranslationUnit CurrentTranslationUnit { get; }

    protected string ConversionMethodName { get; set; }
    
    protected BindingOptions Options { get; set; }
    
    protected Method Method { get; set; }
    
    protected Queue<Action> PostActions { get; set; }
    
    protected List<Parameter> NativeParameters { get; set; }
    
    protected bool IsInstanceMethod => _isInstanceMethod;
    
    protected int CurrentParameterIndex => _currentParameterIndex;
    
    protected bool IsSimpleMethod { get; set; }
    
    protected TargetRuntime TargetRuntime => CurrentTranslationUnit.Module.TargetRuntime;
    
    public MarshalContextToFunctionCodeGenerator(
        BindingOptions options,
        TranslationUnit translationUnit,
        string conversionMethodName)
    {
        PostActions = new Queue<Action>();
        NativeParameters = new List<Parameter>();
        Options = options;
        TypePrinter = new CSharpTypePrinter(options);
        TypePrinter.PushModule(translationUnit.Module);
        CurrentTranslationUnit = translationUnit;
        ConversionMethodName = conversionMethodName;
    }

    public virtual TypePrinterResult GenerateMarshalBody(Method method)
    {
        Clear();
        
        PostActions.Clear();
        NativeParameters.Clear();
        Method = method;
        
        var parameters = method.Parameters.ToList();
        if (method.Class != null && method.Function.Parameters.Count > 0)
        {
            var instanceParameter = (Parameter)method.Function.Parameters[0].Clone();
            instanceParameter.Type.Declaration = method.Class;
            parameters.Insert(0, instanceParameter);
        }
            
        bool isVoid = method.ReturnType.IsPrimitiveTypeEquals(PrimitiveType.Void);

        var parametersResult = TypePrinter.VisitParameters(parameters, MarshalTypes.MethodParameter, method.IsExtensionMethod);
        var returnType = "void";
        if (!isVoid)
        {
            TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
            var wrappedType = method.ReturnType.Visit(TypePrinter);
            TypePrinter.PopMarshalType();
            returnType = $"{wrappedType}";
        }

        if (!IsSimpleMethod)
        {
            // WriteLine($"public static {returnType} Invoke(System.Span<byte> mainBuffer, {parametersResult})");
            // WriteOpenBraceAndIndent();
            WriteLine($"ref Span<byte> {_spanBufferCursorName} = ref {_spanBufferName};");
        }

        int index = 0;
        _isInstanceMethod = method.Class != null;

        for (var paramIndex = 0; paramIndex < method.Function.Parameters.Count; paramIndex++)
        {
            _currentParameterIndex = paramIndex;
            var parameter = method.Function.Parameters[_currentParameterIndex];
            var classDecl = parameter.Type.Declaration as Class;

            if (parameter.Type.IsPointerToBuiltInType(out var primType))
            {
                classDecl = null;
            }

            var argumentName = $"arg{index++}";
            ProcessParameter(parameter, argumentName);
        }
        
        switch (isVoid)
        {
            case false when PostActions.Count == 0 && method.ReturnType.IsString():
                Write("var result = ");
                break;
            case false when method.ReturnType.IsPointerToStructOrUnion():
                Write("var result = ");
                PostActions.Enqueue(ConvertReturnType);
                break;
            case false when method.ReturnType.IsPointerToClass(out var @class):
                Write("var result = ");
                PostActions.Enqueue(ConvertReturnType);
                break;
            case false when PostActions.Count == 0:
                Write("return ");
                break;
            case false when PostActions.Count > 0:
                Write("var result = ");
                break;
        }

        var @namespace = method.Function.Namespace;

        var functionCall =
            $"{@namespace}.{CurrentTranslationUnit.Module.InteropClassName}.{method.Function.Name}({TypePrinter.VisitParameters(NativeParameters, MarshalTypes.NativeFunctionCall)});";
        Write(functionCall);
        NewLine();

        var hasPostActions = PostActions.Count > 0;
        while (PostActions.Count > 0)
        {
            var action = PostActions.Dequeue();
            action?.Invoke();
        }

        if (hasPostActions && !isVoid && !method.ReturnType.IsPointerToStructOrUnion() && !method.ReturnType.IsPointerToClass(out _))
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

        // if (!IsSimpleMethod)
        // {
        //     UnindentAndWriteCloseBrace();
        // }

        return ToString();
    }

    protected virtual void ProcessParameter(Parameter parameter, string argumentName)
    {
        if (parameter.Type.Declaration == null)
        {
            MarshalSimpleType(parameter, argumentName);
        }
        else
        {
            MarshalComplexType(parameter, parameter.Type.Declaration, argumentName);
        }
    }

    protected void MarshalSimpleType(Parameter parameter, string argumentName)
    {
        if (parameter.Type.IsConstArray(out int arraySize))
        {
            WriteLine(
                $"if ({parameter.Name} == null || {parameter.Name}.Length != {arraySize})");
            WriteOpenBraceAndIndent();
            WriteLine(
                $"throw new ArgumentOutOfRangeException(\"{parameter.Name}\", \"The dimensions of the provided array don't match the required size. Size should be = {arraySize}\");");
            UnindentAndWriteCloseBrace();
            
            CreateNativeParameter(parameter, argumentName, parameter.Type.Declaration);
            
            WriteConstArray(parameter, argumentName);
        }
        else if (parameter.Type.IsPointerToString())
        {
            if (parameter.Type.IsStringArray())
            {
                WritePointerToStringArray(parameter, argumentName);
            }
            else
            {
                WritePointerToString(parameter, argumentName);
            }

            CreateNativeParameter(parameter, argumentName, parameter.Type.Declaration);
        }
        else if (parameter.Type.IsPointerToArray(out var arrayType, out var depth))
        {
            WritePointerToArrayForBuiltInType(parameter, argumentName, arrayType, depth);
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
            
            NativeParameters.Add(nativeParam);
        }
        // The input parameter is void*, so we need to just pass it as is without any conversion
        else if (parameter.Type.IsPointerToIntPtr() || parameter.Type.IsPurePointer())
        {
            NativeParameters.Add(parameter);
        }
        else if (parameter.Type.Declaration == null || !parameter.Type.IsPointer())
        {
            NativeParameters.Add(parameter);
        }
    }

    protected virtual void MarshalComplexType(Parameter parameter, Declaration declaration, string argumentName)
    {
        if (declaration is Class classDecl)
        {
            if (_currentParameterIndex == 0 && IsInstanceMethod)
            {
                classDecl = Method.Class;
            }
            else
            {
                var p = Method.Parameters.FirstOrDefault(x => x.Id == parameter.Id);
                classDecl = p.Type.Declaration as Class;
            }
            
            if (classDecl.ClassType == ClassType.Class && !parameter.Type.IsPointerToArray())
            {
                if (classDecl == Method.Class && parameter.Index == 0)
                {
                    if (!Method.IsStatic)
                    {
                        //argumentName = parameter.Name;
                        argumentName = "this";
                    }
                }

                MarshalClass(parameter, argumentName, classDecl);
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
                        MarshalPointerToStruct(parameter, argumentName, classDecl);
                    }
                    else //Pointer to primitive type like int, long, float or very simple struct.
                    {
                        WritePointerToSimpleType(parameter, argumentName, classDecl);
                    }
                }
                else if (classDecl.IsWrapper)
                {
                    if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly &&
                        _currentParameterIndex == 0 &&
                        _isInstanceMethod)
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

                        PostActions.Enqueue(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
                    }
                    else
                    {
                        argumentName = parameter.Name;
                    }

                    CreateNativeParameter(parameter, argumentName, classDecl);
                }
            }
        }
        else if (declaration is Delegate @delegate)
        {
            argumentName = parameter.Name;
            CreateNativeParameter(parameter, argumentName, declaration);
        }
        else if (declaration is Enumeration enumeration)
        {
            WriteEnumeration(parameter, argumentName, enumeration);
        }
    }
    
    private void WritePointerToString(Parameter parameter, string argumentName)
    {
        var isUnicode = parameter.Type.IsUnicodeString();
        var isUnicodeString = isUnicode.ToString().ToLower();
        var ptr = parameter.Type as PointerType;
        var pointerString = GetPointerString(ptr.GetDepth());
        var conversionString = isUnicode ? $"char{pointerString}" : $"sbyte{pointerString}";
        WriteLine($"var {argumentName} = {MarshalContextStringToPointer}({parameter.Name}, ref {_spanBufferCursorName});");
    }
    
    private void WritePointerToStringArray(Parameter parameter, string argumentName)
    {
        var isUnicode = parameter.Type.IsUnicodeString();
        var isUnicodeString = isUnicode.ToString().ToLower();
        var ptr = parameter.Type as PointerType;
        var pointerString = GetPointerString(ptr.GetDepth());
        var conversionString = isUnicode ? $"char{pointerString}" : $"sbyte{pointerString}";
        string parameterName = TargetRuntime == TargetRuntime.Net8Plus ? $"{parameter.Name}.Span" : parameter.Name;
        WriteLine(
            $"var {argumentName} = {MarshalContextStringArrayToDoublePointer}({parameterName}, ref {_spanBufferCursorName});");
    }
    
    public void MarshalClass(Parameter parameter, string argumentName, Class classDecl)
    {
        if (parameter.ParameterKind != ParameterKind.Out && !parameter.Type.IsPointerToArray())
        {
            if (classDecl == Method.Class && parameter.Index == 0)
            {
                if (Method.IsStatic)
                {
                    WriteLine($"var {argumentName} = {parameter.Name};");
                }
                else if (parameter.Type.IsPointer())
                {
                    argumentName = $"nativeInstance";
                    WriteLine($"var {argumentName} = GetNativeValue();");

                    argumentName = $"&{argumentName}";
                }
            }
            else
            {
                if (parameter.Type.IsPointer())
                {
                    WriteLine($"{SpanClassName}<{classDecl.NativeStruct.FullName}> {argumentName}Span = {StackAlloc} {classDecl.NativeStruct.Name}[1];");
                    WriteLine($"{argumentName}Span[0] = {parameter.Name};");
                    WriteLine($"var {argumentName} = ({classDecl.NativeStruct.Name}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({argumentName}Span));");
                    if (parameter.ParameterKind == ParameterKind.Ref)
                    {
                        PostActions.Enqueue(() => ConvertPointerToClassOrStruct(parameter, argumentName, classDecl));
                    }
                }
                else
                {
                    TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
                    WriteLine(
                        $"var {argumentName} = {parameter.Name} == null ? new {parameter.Type.Visit(TypePrinter)}() : ({classDecl.NativeStruct.Name}){parameter.Name};");
                    TypePrinter.PopMarshalType();
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
                    $"var {argumentName} = {parameter.Name}.IsEmpty ? null : new {classDecl.NativeStruct.FullName}[{arrayLength}];");

                if (parameter.ParameterKind == ParameterKind.Ref)
                {
                    PostActions.Enqueue(
                        () => ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength));
                }
                else if (parameter.ParameterKind == ParameterKind.In)
                {
                    ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength);
                }
            }
            else if (parameter.ParameterKind == ParameterKind.Out)
            {
                if (!string.IsNullOrEmpty(arrayLength)
                    && CurrentTranslationUnit.Module.WrapInteropObjects)
                {
                    WriteLine($"IntPtr {argumentName} = IntPtr.Zero;");
                    PostActions.Enqueue(() => ConvertPointerToWrappedStructArray(parameter, argumentName, classDecl, currentArray));
                }
                else if (!string.IsNullOrEmpty(arrayLength)
                         && !CurrentTranslationUnit.Module.WrapInteropObjects)
                {
                    WriteLine(
                        $"var {argumentName} = new {classDecl.NativeStruct.Namespace}.{classDecl.NativeStruct.Name}[{arrayLength}];");
                    PostActions.Enqueue(
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
            PostActions.Enqueue(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }
    
    void ImplicitTwoWayArrayTypeConversion(Parameter parameter, Class classDecl, string argumentName, string arrayLength)
    {
        if (parameter.ParameterKind is ParameterKind.Out)
        {
            WriteLine($"{parameter.Name} = new {classDecl.Name}[{arrayLength}];");
        }
        
        if (parameter.ParameterKind is ParameterKind.Out or ParameterKind.Ref)
        {
            MarshalPointerToManagedArray();
        }
        else
        {
            WriteLine($"if (!{parameter.Name}.IsEmpty)");
            WriteOpenBraceAndIndent();
            MarshalArrayToNative();
            UnindentAndWriteCloseBrace();
        }

        void MarshalArrayToNative()
        {
            if (classDecl.IsSimpleType)
            {
                string nativeTypeName = classDecl.FullName;
                string totalBytes = $"{argumentName}Bytes";

                WriteLine($"int {totalBytes} = {parameter.Name}.Length * sizeof({nativeTypeName});");
                WriteLine($"{argumentName} = ({nativeTypeName}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({_spanBufferCursorName}.Slice(0, {totalBytes})));");
                WriteLine($"{_spanBufferCursorName} = {_spanBufferCursorName}.Slice({totalBytes});");
                WriteLine($"fixed (void* pSource = {parameter.Name})");
                WriteOpenBraceAndIndent();
                WriteLine($"{UnsafeClassName}.CopyBlock({argumentName}, pSource, (uint){totalBytes});");
                UnindentAndWriteCloseBrace();
            }
            else if (classDecl.IsWrapper)
            {
                WriteLine(
                    $"{argumentName} = {MarshalContextUtilsWrapperArrayToPointer}<{classDecl.FullName}, {classDecl.NativeStruct.FullName}>({parameter.Name}, ref {_spanBufferCursorName});");
            }
            else
            {
                WriteLine($"{argumentName}Span = {StackAlloc} {classDecl.NativeStruct.FullName}[(int){arrayLength}];");
                WriteLine(
                    $"{MarshalContextUtilsArrayOfHandleWrappers}<{classDecl.FullName}, {classDecl.NativeStruct.FullName}>({parameter.Name}, {argumentName}Span);");
                WriteLine(
                    $"{argumentName} = ({classDecl.NativeStruct.FullName}*){UnsafeClassName}.AsPointer(ref {MemoryMarshalClassName}.GetReference({argumentName}Span));");
            }
        }

        void MarshalPointerToManagedArray()
        {
            WriteLine($"for (var i = 0; i < {arrayLength}; ++i)");
            WriteOpenBraceAndIndent();
            if (classDecl.IsWrapper)
            {
                WriteLine($"{parameter.Name}[i] = new {classDecl.Name}({argumentName}[i]);");
            }
            else
            {
                WriteLine($"{parameter.Name}[i] = {argumentName}[i];");
            }
            UnindentAndWriteCloseBrace();
        }
    }

    protected void ConvertPointerToWrappedStructArray(Parameter parameter, string argumentName, Class classDecl,
        ArrayType array)
    {
        var arrayLength = array.ArraySizeSource;
        if (string.IsNullOrEmpty(arrayLength))
        {
            arrayLength = $"{parameter.Name}.Length";
        }

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

        WriteLine($"for (var i = 0U; i < {arrayLength}; ++i)");
        WriteOpenBraceAndIndent();
        WriteLine($"{parameter.Name}[i] = new {classDecl.FullName}({argumentName}[i]);");
        UnindentAndWriteCloseBrace();
    }
    
    protected void MarshalPointerToStruct(Parameter parameter, string argumentName, Class classDecl)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        TypePrinter.PushParameter(parameter);
        var interopType = parameter.Type.Visit(TypePrinter);
        var nativeType = $"{parameter.Type.Declaration.Namespace}.{interopType.Type}";
        interopType = $"{parameter.Type.Declaration.Namespace}.{interopType}";
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();

        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly or ParameterKind.Ref)
        {
            switch (classDecl.ClassType)
            {
                case ClassType.StructWrapper:
                case ClassType.UnionWrapper:
                    if (_currentParameterIndex == 0 && Method.IsInstanceMethod)
                    {
                        if (Method.GenerateMarshalContext)
                        {
                            WriteLine(
                                $"var {argumentName} = {MarshalContextUtilsStructToPointer}<{classDecl.FullName}, {nativeType}>({parameter.Name}, ref {_spanBufferCursorName});");
                        }
                        else
                        {
                            WriteLine($"{SpanClassName}<byte> {argumentName}Span = {StackAlloc} byte[GetSize()];");
                            var parameterName = CurrentParameterIndex == 0 ? "this" : parameter.Name;
                            WriteLine(
                                $"var {argumentName} = {MarshalContextUtilsStructToPointer}<{classDecl.FullName}, {nativeType}>({parameterName}, ref {argumentName}Span);");
                        }
                    }
                    else
                    {
                        if (parameter.Type.IsDoublePointer())
                        {
                            WriteLine(
                                $"var {argumentName} = {MarshalContextUtilsStructToDoublePointer}<{classDecl.FullName}, {nativeType}>({parameter.Name}, ref {_spanBufferCursorName});");
                        }
                        else
                        {
                            WriteLine(
                                $"var {argumentName} = {MarshalContextUtilsStructToPointer}<{classDecl.FullName}, {nativeType}>({parameter.Name}, ref {_spanBufferCursorName});");
                        }
                    }

                    break;
            }

            if (parameter.ParameterKind == ParameterKind.Ref)
            {
                PostActions.Enqueue(() => ConvertPointerToClassOrStruct(parameter, argumentName, classDecl));
            }
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteLine($"{interopType} {argumentName};");
            PostActions.Enqueue(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }
    
    protected void WritePointerToSimpleType(Parameter parameter, string argumentName, Class classDecl)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        TypePrinter.PushParameter(parameter);
        var nativeType = parameter.Type.Visit(TypePrinter);
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();
        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly or ParameterKind.Ref)
        {
            switch (classDecl.ClassType)
            {
                case ClassType.Struct:
                case ClassType.Union:
                    WritePointerToPrimitiveType(parameter, argumentName);
                    break;
            }

            if (parameter.ParameterKind == ParameterKind.Ref)
            {
                PostActions.Enqueue(() => ConvertPointerToClassOrStruct(parameter, argumentName, classDecl));
            }
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            if (classDecl.NativeStruct != null)
            {
                WriteLine($"{classDecl.NativeStruct.Name} {argumentName};");
                PostActions.Enqueue(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
            }
            else if (classDecl.IsSimpleType)
            {
                WriteLine($"{nativeType} {argumentName};");
                PostActions.Enqueue(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
            }
            else
            {
                argumentName = parameter.Name;
            }
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }
    
    protected void WritePointerToArray(Parameter parameter, string argumentName, Class classDecl, ArrayType arrayType, uint pointerDepth)
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
        if (classDecl.NativeStruct != null)
        {
            typeStrResult.Type = classDecl.NativeStruct.Name;
        }
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();
        
        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly)
        {
            if (classDecl.ClassType == ClassType.Class)
            {
                WriteLine($"{classDecl.NativeStruct.Namespace}.{typeStrResult} {argumentName} = null;");
                WriteLine($"{SpanClassName}<{classDecl.NativeStruct.FullName}> {argumentName}Span = default;");
                ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength);
            }
            else
            {
                if (classDecl.IsWrapper && classDecl.NativeStruct != null)
                {
                    WriteLine($"{classDecl.NativeStruct.Namespace}.{typeStrResult} {argumentName} = null;");
                    ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength);
                }
                else // assume that type is primitive (blittable), or we are using structs/unions and conversion is not needed  
                {
                    WriteLine(
                        $"var {argumentName} = {MarshalContextUtilsBlittableArray}<{typeStrResult.Type}>({parameter.Name}, ref {_spanBufferCursorName});");
                }
            }
        }
        else if (parameter.ParameterKind == ParameterKind.Ref)
        {
            arrayLength = $"{parameter.Name}.Length";
            if (classDecl.IsSimpleType)
            {
                WriteLine(
                    $"var {argumentName} = {MarshalContextUtilsBlittableArray}<{typeStrResult.Type}>({parameter.Name}, ref {_spanBufferCursorName});");
            }
            else if (classDecl.ClassType == ClassType.Class || classDecl.IsWrapper)
            {
                // if (Method.GenerateMarshalContext)
                // {
                //     WriteLine(
                //         $"var {argumentName} = {MarshalContextUtilsAllocatePointerForArray}<{classDecl.NativeStruct.FullName}>((int){arrayLength}, ref {_spanBufferCursorName});");
                // }
                // else
                // {
                //     
                // }
                WriteStackAlloc(argumentName, classDecl.NativeStruct.FullName, arrayLength);
                PostActions.Enqueue(() => ImplicitTwoWayArrayTypeConversion(parameter, classDecl, argumentName, arrayLength));
            }
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            if (Method.GenerateMarshalContext)
            {
                WriteStackAlloc(argumentName, classDecl.NativeStruct.FullName, arrayLength);
                // WriteLine(
                //     $"var {argumentName} = {MarshalContextUtilsAllocatePointerForArray}<{classDecl.NativeStruct.FullName}>((int){arrayLength}, ref {_spanBufferCursorName});");
            }
            else
            {
                if (classDecl.IsSimpleType)
                {
                    WriteLine($"{classDecl.FullName}* {argumentName} = {NullPointer};");
                }
                else
                {
                    WriteLine($"{classDecl.NativeStruct.FullName}* {argumentName} = {NullPointer};");
                }
                
            }
            
            if (classDecl.IsWrapper)
            {
                PostActions.Enqueue(() => ConvertPointerToWrappedStructArray(parameter, argumentName, classDecl, arrayType));
            }
            else
            {
                PostActions.Enqueue(() => ConvertPointerToArray(parameter, argumentName, classDecl, arrayType));
            }
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }

    protected void WriteStackAlloc(string argumentName, string type, string arrayLength)
    {
        WriteLine($"var {argumentName} = {StackAlloc} {type}[(int){arrayLength}];");
    }

    private void WriteConstArray(Parameter parameter, string argumentName)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        TypePrinter.PushParameter(parameter);
        var typeStrResult = parameter.Type.Visit(TypePrinter);
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();
        WriteLine(
            $"var {argumentName} = {MarshalContextUtilsBlittableArray}<{typeStrResult.Type}>({parameter.Name}, ref {_spanBufferCursorName});");
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
        
        if (classType is ClassType.Struct or ClassType.Class)
        {
            WriteLine($"for (var i = 0; i < (int){arrayLength}; ++i)");
            WriteOpenBraceAndIndent();
            if (classDecl.IsSimpleType)
            {
                WriteLine($"{parameter.Name}[i] = {argumentName}[i];");           
            }
            else
            {
                WriteLine(classType == ClassType.Class
                    ? $"{parameter.Name}[i] = {argumentName}[i];"
                    : $"{parameter.Name}[i] = new {type}({argumentName}[i]);");
            }

            UnindentAndWriteCloseBrace();
        }
    }
    
    void WriteWrappedStruct(Parameter parameter, string argumentName, Class classDecl)
    {
        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
        var interopType = parameter.WrappedType.Visit(TypePrinter).Type;
        TypePrinter.PopMarshalType();

        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly or ParameterKind.Ref)
        {
            WriteLine(
                $"var {argumentName} = {MarshalContextUtilsStructToNative}({parameter.Name}, ref {_spanBufferCursorName});");
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteLine($"{parameter.WrappedType.Declaration.Name} {argumentName};");
            PostActions.Enqueue(() => ConvertOutStructToClass(parameter, argumentName, classDecl));
        }
        
        CreateNativeParameter(parameter, argumentName, classDecl);
    }
    
    private void ConvertPointerToClassOrStruct(Parameter parameter, string argumentName, Class classDecl)
    {
        WriteLine($"if ({argumentName} is not null)");
        WriteOpenBraceAndIndent();
        if (classDecl.IsWrapper)
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
    
    private void WriteDefaultOutParameterInitialization(Parameter parameter)
    {
        if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteLine("else");
            PushIndent();
            WriteLine($"{parameter.Name} = default;");
            PopIndent();
        }
    }
    
    protected void WritePointerToArrayForBuiltInType(Parameter parameter, string argumentName, ArrayType arrayType, uint pointerDepth)
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
        
        TypePrinter.PopParameter();
        TypePrinter.PopMarshalType();
        if (parameter.ParameterKind is ParameterKind.In or ParameterKind.Readonly)
        {
            WriteLine(
                $"var {argumentName} = {MarshalContextUtilsBlittableArray}<{typeStrResult.Type}>({parameter.Name}, ref {_spanBufferCursorName});");
            
        }
        else if (parameter.ParameterKind == ParameterKind.Ref)
        {
            WriteStackAlloc(argumentName, typeStrResult.Type, arrayLength);

            PostActions.Enqueue(() => ConvertPointerToArrayOfNativeType(parameter, argumentName, typeStrResult.Type, arrayLength));
        }
        else if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteStackAlloc(argumentName, typeStrResult.Type, arrayLength);
            PostActions.Enqueue(() => ConvertPointerToArrayOfNativeType(parameter, argumentName, typeStrResult.Type, arrayLength));
        }
        
        CreateNativeParameter(parameter, argumentName, null);
    }
    
    protected void ConvertPointerToArrayOfNativeType(Parameter parameter, string argumentName, string typeStrResult, string arrayLength)
    {
        if (parameter.ParameterKind == ParameterKind.Out)
        {
            WriteLine($"{parameter.Name} = new {typeStrResult}[{arrayLength}];");
        }

        WriteLine($"{parameter.Name} = {MarshalContextUtilsUnmarshalBlittableArray}({argumentName}, {arrayLength});");
    }
    
    protected void WritePointerToPrimitiveType(Parameter parameter, string argumentName)
    {
        if (parameter.ParameterKind is not ParameterKind.Out)
        {
            TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
            TypePrinter.PushParameter(parameter);
            var interopType = parameter.Type.Visit(TypePrinter);
            TypePrinter.PopParameter();
            TypePrinter.PopMarshalType();
            WriteLine($"var {argumentName} = {StackAlloc} {interopType.Type}[1];");
            WriteLine($"*{argumentName} = {parameter.Name};");
        }

        if (parameter.ParameterKind is ParameterKind.Ref)
        {
            PostActions.Enqueue(() => ConvertOutPrimitiveTypePointerToValue(parameter, argumentName));
        }
    }
    
    void ConvertOutPrimitiveTypePointerToValue(Parameter parameter, string argumentName)
    {
        WriteLine($"{parameter.Name} = *{argumentName};");
    }
    
    protected void ConvertOutStructToClass(Parameter parameter, string argumentName, Class classDecl)
    {
        var wrapperType = classDecl.Name;
            
        var pointer = parameter.Type as PointerType;
        var depth = pointer.GetDepth();
        if (depth > 1)
        {
            WriteLine($"if ({argumentName} is not null)");
            WriteOpenBraceAndIndent();
            if (classDecl.IsWrapper)
            {
                WriteLine($"{parameter.Name} = new {wrapperType}(*{argumentName});");
            }
            else if (classDecl.IsSimpleType)
            {
                WriteLine($"{parameter.Name} = *{argumentName};");
            }

            UnindentAndWriteCloseBrace();
            WriteLine($"else");
            WriteOpenBraceAndIndent();
            WriteLine($"{parameter.Name} = (default);");
            UnindentAndWriteCloseBrace();
        }
        else
        {
            WriteLine($"{parameter.Name} = new {wrapperType}({argumentName});");
        }
    }
    
    protected void WriteEnumeration(Parameter parameter, string argumentName, Enumeration enumeration)
    {
        TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
        var nativeType = parameter.Type.Visit(TypePrinter);
        TypePrinter.PopMarshalType();
        if (parameter.Type.IsPointerToArray())
        {
            if (parameter.Type.IsDoublePointer())
            {
                WriteLine($"var pData{parameter.Name} = {MarshalContextUtilsBlittableArray}<{enumeration.FullName}>({parameter.Name}, ref {_spanBufferCursorName});");
                WriteLine($"var {argumentName} = stackalloc {enumeration.FullName}*[{parameter.Name}.Length];");
                WriteLine($"for (int i = 0; i < {parameter.Name}.Length; i++)");
                WriteOpenBraceAndIndent();
                WriteLine($"{argumentName}[i] = pData{parameter.Name} + i;");
                UnindentAndWriteCloseBrace();
            }
            else
            {
                WriteLine(
                    $"var {argumentName} = {MarshalContextUtilsBlittableArray}<{enumeration.FullName}>({parameter.Name}, ref {_spanBufferCursorName});");
            }
            
            var pointerType = (PointerType)parameter.Type;
            var arrayType = (ArrayType)pointerType.Pointee;
            var arraySizeSource = arrayType.ArraySizeSource;
            var arrayLength = $"{parameter.Name}.Length";
            if (!string.IsNullOrEmpty(arraySizeSource))
            {
                arrayLength = arraySizeSource;
            }
            
            if (parameter.ParameterKind is ParameterKind.Ref or ParameterKind.Out)
            {
                PostActions.Enqueue(() => ConvertPointerToArrayOfEnums(parameter, argumentName, enumeration, arrayLength));
            }

            CreateNativeParameter(parameter, argumentName, enumeration);
        }
        else if (parameter.Type.IsPointer())
        {
            if (parameter.Type.IsDoublePointer())
            {
                WriteLine($"var pValue{parameter.Name} = {StackAlloc} {enumeration.FullName}[1];");
                WriteLine($"*pValue{parameter.Name} = {parameter.Name};");
                WriteLine($"var {argumentName} = {StackAlloc} {enumeration.FullName}*[1];");
                WriteLine($"*{argumentName} = pValue{parameter.Name};");
            }
            else
            {
                if (parameter.ParameterKind is not ParameterKind.Out)
                {
                    WriteLine($"var {argumentName} = {StackAlloc} {enumeration.FullName}[1];");
                    WriteLine($"*{argumentName} = {parameter.Name};");
                }
                else
                {
                    argumentName = parameter.Name;
                }
            }
            
            if (parameter.ParameterKind is ParameterKind.Ref)
            {
                PostActions.Enqueue(() => ConvertPointerToEnum(parameter, argumentName));
            }

            CreateNativeParameter(parameter, argumentName, enumeration);
        }
        else if (parameter.Type.IsConstArray(out var arraySize))
        {
            WriteLine(
                $"if ({parameter.Name} == null || {parameter.Name}.Length != {arraySize})");
            WriteOpenBraceAndIndent();
            WriteLine(
                $"throw new ArgumentOutOfRangeException(\"{parameter.Name}\", \"The dimensions of the provided array don't match the required size. Size should be = {arraySize}\");");
            UnindentAndWriteCloseBrace();
            
            CreateNativeParameter(parameter, argumentName, parameter.Type.Declaration);
            
            WriteConstArray(parameter, argumentName);
        }
        else
        {
            CreateNativeParameter(parameter, parameter.Name, enumeration);
        }
    }
   
    void ConvertPointerToArrayOfEnums(Parameter parameter, string argumentName, Enumeration enumeration, string arrayLength)
    {
        WriteLine($"var {argumentName}SourceSpan = new {ReadonlySpanClassName}<{enumeration.FullName}>({argumentName}, (int){arrayLength});");
        if (parameter.ParameterKind is ParameterKind.Ref)
        {
            WriteLine($"{argumentName}SourceSpan.CopyTo({parameter.Name});");
        }
        else
        {
            WriteLine($"{parameter.Name} = {SpanClassName}<{enumeration.FullName}>({argumentName}, (int){arrayLength});");
        }
    }
    
    void ConvertPointerToEnum(Parameter parameter, string argumentName)
    {
        WriteLine($"if ({argumentName} is not null)");
        WriteOpenBraceAndIndent();
        WriteLine($"{parameter.Name} = *{argumentName};");
        UnindentAndWriteCloseBrace();
        WriteDefaultOutParameterInitialization(parameter);
    }
    
    protected void CreateNativeParameter(Parameter parameter, string argumentName, Declaration decl)
    {
        var clonedType = (BindingType)parameter.Type.Clone();
        clonedType.Declaration = decl;
        
        NativeParameters.Add(new Parameter()
            { Name = argumentName, IsOverload = parameter.IsOverload, ParameterKind = parameter.ParameterKind, Type = clonedType });
    }

    private void ConvertReturnType()
    {
        TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
        var wrappedType = Method.ReturnType.Visit(TypePrinter);
        TypePrinter.PopMarshalType();
        var classDecl = Method.ReturnType.Declaration as Class;
        if (classDecl is { IsWrapper: true })
        {
            WriteLine($"var wrappedResult = new {wrappedType.Type}(*result);");
            FreeNativePointer("result");
            WriteLine("return wrappedResult;");
        }
        else if (classDecl.ClassType == ClassType.Class)
        {
            WriteLine($"var classResult = *result;");
            FreeNativePointer("result");
            WriteLine("return classResult;");
        }
    }
    
    protected void FreeNativePointer(string argumentName)
    {
        WriteLine($"NativeUtils.Free({argumentName});");
    }
}