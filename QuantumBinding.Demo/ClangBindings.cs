using System;
using QuantumBinding.Generator;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.ClangGenerator;

public partial class ClangBindings
{
    public class MacroFunctions
    {
        public static MacroFunction CIndexVersionEncode(uint major, uint minor)
        {
            var function = new MacroFunction();
            var majorParameter = new Parameter()
                { Type = new BuiltinType(PrimitiveType.UInt32), Name = "major", ParameterKind = ParameterKind.In };
            var minorParameter = new Parameter()
                { Type = new BuiltinType(PrimitiveType.UInt32), Name = "minor", ParameterKind = ParameterKind.In };
            function.Parameters.Add(majorParameter);
            function.Parameters.Add(minorParameter);
            function.FunctionBody =
                $"var version = (({major})*10000) + (({minor})*1);{Environment.NewLine}return (uint)version;";
            function.ReturnType = new BuiltinType(PrimitiveType.UInt32);
            function.ApplyStrategy = MacroFunctionStrategy.ApplyAll;

            return function;
        }

        public static MacroFunction CIndexVersionStringize(uint major, uint minor)
        {
            var function = new MacroFunction();
            var majorParameter = new Parameter()
                { Type = new BuiltinType(PrimitiveType.UInt32), Name = "major", ParameterKind = ParameterKind.In };
            var minorParameter = new Parameter()
                { Type = new BuiltinType(PrimitiveType.UInt32), Name = "minor", ParameterKind = ParameterKind.In };
            function.Parameters.Add(majorParameter);
            function.Parameters.Add(minorParameter);
            function.FunctionBody = $"return \"{major}.{minor}\";";
            function.ApplyStrategy = MacroFunctionStrategy.ApplyAll;
            
            return function;
        }

        // public static MacroFunction CreateMakeProfileVersionFunction(byte major, byte minor, byte patch)
        // {
        //     var function = new MacroFunction();
        //     function.FunctionBody =
        //         $"({major} << 22 | {minor} << 12 | {patch})";
        //     function.ApplyStrategy = MacroFunctionStrategy.ApplyOnlyFunctionBody;
        //
        //     return function;
        // }

        // public static MacroFunction CreateApiVersion()
        // {
        //     var function = new MacroFunction();
        //     function.ReturnType = new BuiltinType(PrimitiveType.UInt32);
        //     function.ApplyStrategy = MacroFunctionStrategy.ApplyOnlyReturnType;
        //     return function;
        // }
        //
        // public static MacroFunction CreateSpirvMSLVersion()
        // {
        //     var function = new MacroFunction();
        //     var major = new Parameter()
        //         { Type = new BuiltinType(PrimitiveType.Byte), Name = "major", ParameterKind = ParameterKind.In };
        //     var minor = new Parameter()
        //         { Type = new BuiltinType(PrimitiveType.Byte), Name = "minor", ParameterKind = ParameterKind.In };
        //     var patch = new Parameter()
        //         { Type = new BuiltinType(PrimitiveType.Byte), Name = "patch", ParameterKind = ParameterKind.In };
        //     function.Parameters.Add(major);
        //     function.Parameters.Add(minor);
        //     function.Parameters.Add(patch);
        //     function.FunctionBody =
        //         $"var version = major * 10000 + minor * 100 + patch;{Environment.NewLine}return (uint)version;";
        //     function.ReturnType = new BuiltinType(PrimitiveType.UInt32);
        //     return function;
        // }
        //
        // public static MacroFunction CreateApiVersionFor(string paramName)
        // {
        //     var function = new MacroFunction();
        //     var param = new Parameter() { Type = new BuiltinType(PrimitiveType.Byte), Name = paramName };
        //     function.Parameters.Add(param);
        //     switch (paramName)
        //     {
        //         case "variant":
        //             function.FunctionBody = $"return (uint)({paramName}>>29);";
        //             break;
        //         case "major":
        //             function.FunctionBody = $"return ((uint)({paramName}>>22) & 0x7FU);";
        //             break;
        //         case "minor":
        //             function.FunctionBody = $"return ((uint)({paramName}>>12) & 0x3FFU);";
        //             break;
        //         case "patch":
        //             function.FunctionBody = $"return (uint)({paramName} & 0xFFFU);";
        //             break;
        //     }
        //
        //     function.FunctionBody = $"return (uint)({paramName}>>22);";
        //     function.ReturnType = new BuiltinType(PrimitiveType.UInt32);
        //
        //     return function;
        // }
        //
        // public static MacroFunction CreateHeaderVersion()
        // {
        //     var function = new MacroFunction();
        //     function.ReturnType = new BuiltinType(PrimitiveType.Byte);
        //     function.ApplyStrategy = MacroFunctionStrategy.ApplyOnlyReturnType;
        //     return function;
        // }
        //
        // public static MacroFunction CreateLodClampNone()
        // {
        //     var function = new MacroFunction();
        //     function.ReturnType = new BuiltinType(PrimitiveType.Float);
        //     function.ApplyStrategy = MacroFunctionStrategy.ApplyOnlyReturnType;
        //     return function;
        // }
        //
        // public static MacroFunction CreateStringReturnMacro()
        // {
        //     var func = new MacroFunction();
        //     func.ReturnType = new BuiltinType(PrimitiveType.String);
        //     func.ApplyStrategy = MacroFunctionStrategy.ApplyOnlyReturnType;
        //     return func;
        // }
    }
}