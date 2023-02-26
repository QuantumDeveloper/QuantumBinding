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
            function.ReturnType = new BuiltinType(PrimitiveType.String);
            function.FunctionBody = $"return \"{major}.{minor}\";";
            function.ApplyStrategy = MacroFunctionStrategy.ApplyAll;
            
            return function;
        }

        public static MacroFunction CIndexVersionString()
        {
            var function = new MacroFunction();
            function.ReturnType = new BuiltinType(PrimitiveType.String);
            function.ApplyStrategy = MacroFunctionStrategy.ApplyOnlyReturnType;

            return function;
        }
    }
}