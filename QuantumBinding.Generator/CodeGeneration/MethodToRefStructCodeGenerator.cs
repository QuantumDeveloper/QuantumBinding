using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.CodeGeneration;

public class MethodToRefStructCodeGenerator : TextGenerator
{
    private Method method;

    public TypePrinter TypePrinter { get; }

    public TranslationUnit CurrentTranslationUnit { get; }

    protected string ConversionMethodName { get; set; }

    protected bool WrapInteropObjects { get; set; }
    
    protected  BindingOptions Options { get; set; }
    
    private int currentParameterIndex;
    private List<Parameter> nativeParams;

    public MethodToRefStructCodeGenerator(
        BindingOptions options, 
        TranslationUnit translationUnit,
        string conversionMethodName)
    {
        Options = options;
        TypePrinter = new CSharpTypePrinter(options);
        TypePrinter.PushModule(translationUnit.Module);
        CurrentTranslationUnit = translationUnit;
        ConversionMethodName = conversionMethodName;
        WrapInteropObjects = CurrentTranslationUnit.Module.WrapInteropObjects;
    }
    
    public TargetRuntime TargetRuntime => CurrentTranslationUnit.Module.TargetRuntime;

    public TypePrinterResult GenerateMarshalContext(Method method)
    {
        this.method = method;
        
        Clear();

        if (method.GenerateMarshalContext)
        {
            GenerateContextCall(method);
        }
        else
        {
            GenerateSimpleCall(method);
        }

        return ToString();

        void GenerateSimpleCall(Method method)
        {
            var simpleMethodsGenerator = new MethodToFunctionCodeGenerator(Options, CurrentTranslationUnit, ConversionMethodName);
            var result = simpleMethodsGenerator.GenerateMarshalBody(method);
            WriteLine(result.ToString());
        }
        
        void GenerateCalculateSizeMethod(Method method)
        {
            var parameters = method.Parameters
                .Where(x => x.IsAvailableForContextGeneration())
                .ToList();
            
            if (method.Name == "EqualCursors")
            {
                int x = 0;
            }
            
            if (method.IsInstanceMethod && method.Class.IsWrapper)
            {
                var firstParameter = (Parameter)method.Function.Parameters.First().Clone();
                firstParameter.Type.Declaration = method.Class;
                parameters.Insert(0, firstParameter);
            }
            
            var parametersResult =
                TypePrinter.VisitParameters(parameters, MarshalTypes.SkipParamModifiers, method.IsExtensionMethod);

            string totalSizeName = "totalSize";
            WriteLine($"int CalculateSize({parametersResult})");
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
                        WriteLine($"{totalSizeName} += {MarshalContextCalculateSizeForStringArray}({parameter.Name});");
                    }
                    else
                    {
                        WriteLine($"if (!string.IsNullOrEmpty({parameter.Name}))");
                        PushIndent();
                        WriteLine($"{totalSizeName} += {parameter.Name}.Length * sizeof(byte) + 1;");
                        PopIndent();
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
                    var declaration = parameter.Type.Declaration as Class;
                    WriteForLoop($"{parameter.Name}.Length",
                        () =>
                        {
                            if (declaration != null && declaration.IsWrapper)
                            {
                                WriteLine($"if({parameter.Name}[(int)i] == null)");
                                PushIndent();
                                WriteLine($"{totalSizeName} += Marshal.SizeOf<{declaration.NativeStruct.FullName}>();");
                                PopIndent();
                                WriteLine($"else");
                                PushIndent();
                                WriteLine($"{totalSizeName} += {parameter.Name}[(int)i].GetSize();");
                                PopIndent();
                            }
                        });
                }
                else if (parameter.Type.IsWrapper())
                {
                    WriteLine($"if ({parameter.Name} != null)");
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

        void GenerateContextCall(Method method)
        {
            var parameters = method.Parameters
                .Where(x => x.IsAvailableForContextGeneration())
                .ToList();
            var parametersResult = TypePrinter.VisitParameters(parameters, MarshalTypes.SkipParamTypesAndModifiers);
            TypePrinter.PopMarshalType();

            GenerateCalculateSizeMethod(method);
            NewLine();

            if (method.IsInstanceMethod && method.Class.IsWrapper)
            {
                parametersResult.Type = $"this, {parametersResult.Type}";
            }
            
            WriteLine($"var totalSize = CalculateSize({parametersResult});");

            parameters = method.Parameters.ToList();
            if (method.Class != null)
            {
                var instanceParameter = (Parameter)method.Function.Parameters[0].Clone();
                instanceParameter.Type.Declaration = method.Class;
                instanceParameter.Name = "this";
                parameters.Insert(0, instanceParameter);
            }
            
            WriteTryFinallyBlock(() =>
            {
                var contextGenerator = new MarshalContextToFunctionCodeGenerator(Options, CurrentTranslationUnit, ConversionMethodName);
                var result = contextGenerator.GenerateMarshalBody(method);
                WriteLine(result.ToString());
            });
        }
    }

    private void WriteTryFinallyBlock(Action action)
    {
        WriteLine($"byte[] rentedArray = null;");
        WriteLine($"var mainBuffer = totalSize <= {StackAllocThresholdPropertyName} ? stackalloc byte[totalSize] : (rentedArray = System.Buffers.ArrayPool<byte>.Shared.Rent(totalSize)).AsSpan(0, totalSize);");
        WriteLine($"try");
        WriteOpenBraceAndIndent();
        action?.Invoke();
        UnindentAndWriteCloseBrace();
        WriteLine($"finally");
        WriteOpenBraceAndIndent();
        WriteLine($"if (rentedArray != null)");
        PushIndent();
        WriteLine($"System.Buffers.ArrayPool<byte>.Shared.Return(rentedArray);");
        PopIndent();
        UnindentAndWriteCloseBrace();
    }
}