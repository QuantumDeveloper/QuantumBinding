using System;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.CodeGeneration;

public class MethodToFunctionCodeGenerator : MarshalContextToFunctionCodeGenerator
{
    public MethodToFunctionCodeGenerator(
        BindingOptions options, 
        TranslationUnit translationUnit,
        string conversionMethodName) : base(options, translationUnit, conversionMethodName)
    {
    }

    public override TypePrinterResult GenerateMarshalBody(Method method)
    {
        IsSimpleMethod = true;
        return base.GenerateMarshalBody(method);
    }

    protected override void ProcessParameter(Parameter parameter, string argumentName)
    {
        if (parameter.Type.Declaration is Enumeration enumeration)
        {
            WriteEnumeration(parameter, argumentName, enumeration);
        }
        else if (parameter.Type.Declaration is Class classDecl)
        {
            if (CurrentParameterIndex == 0 && IsInstanceMethod)
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
                        argumentName = "this"; // pass this as the first parameter to avoid additional copying of memory
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
                    else if (type.IsPointerToSimpleType())
                    {
                        WritePointerToSimpleType(parameter, argumentName, classDecl);
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
                        if (classDecl is { IsWrapper: true })
                        {
                            WriteLine($"using var ctx = new NativeContext(GetSize(), stackalloc byte[(int){StackAllocThresholdPropertyName}]);");
                            WriteLine($"var native = this.MarshalToNative(ctx);");
                            argumentName = "native";
                        }
                        else
                        {
                            argumentName = parameter.Name;
                        }
                    }

                    CreateNativeParameter(parameter, argumentName, classDecl);
                }
            }
        }
        else
        {
            if (parameter.Type.IsTriplePointer())
            {
                if (parameter.Type.IsPointerToStringArray())
                {
                    TypePrinter.PushParameter(parameter);
                    TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
                    var t = parameter.Type.Visit(TypePrinter);
                    TypePrinter.PopMarshalType();
                    TypePrinter.PopParameter();
                    WriteLine($"{t} p{parameter.Name};");
                    CreateNativeParameter(parameter, $"p{parameter.Name}", null);

                    string arraySizeSource = string.Empty;
                    var pointerType = parameter.Type as PointerType;
                    var pointee = pointerType.GetPointee();
                    if (pointee is ArrayType arrayType)
                    {
                        arraySizeSource = arrayType.ArraySizeSource;
                    }

                    PostActions.Enqueue(() =>
                    {
                        WriteLine($"{parameter.Name} = new string[{arraySizeSource}];");
                        WriteLine($"for (int i = 0; i < {parameter.Name}.Length; i++)");
                        WriteOpenBraceAndIndent();
                        WriteLine($"{parameter.Name}[i] = new string(p{parameter.Name}[i]);");
                        UnindentAndWriteCloseBrace();
                    });
                }
            }
            else if (parameter.Type.IsPointerToArray(out var arrayType, out var depth))
            {
                WritePointerToArrayOfPrimitiveTypes(parameter, arrayType, argumentName);
                
                CreateNativeParameter(parameter, argumentName, null);
            }
            else if (parameter.Type.IsPointerToBuiltInType(out var prim) && !parameter.Type.IsPurePointer())
            {
                WritePointerToPrimitiveType(parameter, argumentName);
                
                CreateNativeParameter(parameter, argumentName, null);
            }
            // The input parameter is void*, so we need to just pass it as is without any conversion
            else if (parameter.Type.IsPointerToIntPtr() || parameter.Type.IsPurePointer())
            {
                TypePrinter.PushParameter(parameter);
                TypePrinter.PushMarshalType(MarshalTypes.NativeParameter);
                var pointerType = parameter.Type.Visit(TypePrinter);
                TypePrinter.PopMarshalType();
                TypePrinter.PopParameter();
                if (parameter.ParameterKind == ParameterKind.Out)
                {
                    WriteLine($"{pointerType} {argumentName} = null;");
                }
                else
                {
                    WriteLine($"var {argumentName} = ({pointerType}){parameter.Name};");
                }

                if (parameter.ParameterKind is ParameterKind.Out or ParameterKind.Ref)
                {
                    PostActions.Enqueue(() =>
                    {
                        WriteLine($"{parameter.Name} = ({PrimitiveType.Nuint.GetDisplayName()}){argumentName};");
                    });
                }

                CreateNativeParameter(parameter, argumentName, null);
            }
            else if (parameter.Type.Declaration == null || !parameter.Type.IsPointer())
            {
                NativeParameters.Add(parameter);
            }
        }
    }
}