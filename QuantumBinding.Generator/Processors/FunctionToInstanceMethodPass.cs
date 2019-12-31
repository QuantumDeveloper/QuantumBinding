using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors
{
    public class FunctionToInstanceMethodAction : PreGeneratorPass
    {
        public FunctionToInstanceMethodAction()
        {
            Options.VisitFunctions = true;
        }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            Class @class = null;
            // Create method in global scope
            if ((function.Parameters.Count > 0 && !GetFunctionParameter(function.Parameters[0], out @class)) ||
                (function.Parameters.Count == 0 && !GetFunctionReturnType(function.ReturnType, out @class)))
            {
                var globalMethod = new Method();
                globalMethod.Function = function;
                globalMethod.Name = ClangUtils.ChangeStringStyle(function.Name, NamingStyle.FirstLetterUpperCase);
                globalMethod.IsStatic = true;
                globalMethod.ReturnType = function.ReturnType;
                globalMethod.AccessSpecifier = AccessSpecifier.Public;
                globalMethod.Owner = function.Owner;

                foreach (var parameter1 in function.Parameters)
                {
                    // Copy parameters with creating new instance and decrement parameter indices because we removed the first one
                    var param = (Parameter)parameter1.Clone();
                    globalMethod.Parameters.Add(param);
                }

                CurrentNamespace.AddDeclaration(globalMethod);

                return true;
            }

            var method = new Method();
            bool isExtension = false;

            if (function.Parameters.Count == 0)
            {
                if (!GetFunctionReturnType(function.ReturnType, out @class))
                {
                    return false;
                }

                method.IsStatic = true;
            }
            else if (function.Parameters.Count > 0 && function.Parameters[0].ParameterKind != ParameterKind.In)
            {
                if (!GetFunctionParameter(function.Parameters[0], out @class))
                {
                    return false;
                }

                method.IsStatic = true;
            }

            if (@class.Owner != function.Owner)
            {
                isExtension = true;
                method.IsExtensionMethod = true;
            }

            method.Function = function;
            method.Name = ClangUtils.ChangeStringStyle(function.Name, NamingStyle.FirstLetterUpperCase);
            method.ReturnType = function.ReturnType;
            method.AccessSpecifier = AccessSpecifier.Public;
            method.Owner = function.Owner;

            if (isExtension)
            {
                var parameters = function.Parameters;
                foreach (var parameter1 in parameters)
                {
                    // Copy all parameters because it is extension method
                    var param = (Parameter)parameter1.Clone();
                    method.Parameters.Add(param);
                }

                method.IsStatic = true;
            }
            else
            {
                var parameters = function.Parameters.Count > 0 && function.Parameters[0].ParameterKind == ParameterKind.In ? function.Parameters.Skip(1) : function.Parameters;
                foreach (var parameter1 in parameters)
                {
                    // Copy parameters with creating new instance and decrement parameter indices because we removed the first one
                    var param = (Parameter)parameter1.Clone();
                    param.Index--;
                    method.Parameters.Add(param);
                }
            }

            method.Class = @class;
            @class.AddMethod(method);

            return true;
        }

        private bool GetFunctionReturnType(BindingType type, out Class @class)
        {
            @class = type.Declaration as Class;
            if (@class == null || @class.ClassType != ClassType.Class)
            {
                return false;
            }

            CheckExtensions(ref @class);

            return true;
        }

        private bool GetFunctionParameter(Parameter parameter, out Class @class)
        {
            @class = parameter.Type.Declaration as Class;
            if (@class == null || @class.ClassType != ClassType.Class)
            {
                return false;
            }

            CheckExtensions(ref @class);

            return true;
        }

        private void CheckExtensions(ref Class @class)
        {
            if (CurrentNamespace != @class.Owner && @class.HasExtensions)
            {
                if (CurrentNamespace.FindClass(@class.Name) is Class extension)
                {
                    @class = extension;
                }
            }
        }
    }
}