using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBinding.Generator.Processors
{
    /// <summary>
    /// This class detecs and move methods from global scope to classes local scope.
    /// </summary>
    /// <remarks>Works ONLY if first parameter is StructWrapper or UnionWrapper. 
    /// It will be applied automatically if options WrapInteopObjects will be set to TRUE</remarks>
    public class GlobalScopeToClassMethod : PreGeneratorPass
    {
        public GlobalScopeToClassMethod()
        {
            Options.VisitMethods = true;
        }

        public override bool VisitMethod(Method method)
        {
            if (method.Name == "Clang_sortCodeCompletionResults")
            {

            }

            if (method.Class != null) return false;

            if (method.Parameters.Count == 0 || method.IsExtensionMethod) return false;

            if (method.Parameters[0].ParameterKind != ParameterKind.In) return false;

            var parameter = method.Parameters[0];
            if (parameter.Type.IsArray() || parameter.Type.IsPointerToArray()) return false;

            if (GetMethodParameter(method.Parameters[0], out var @class))
            {
                CurrentNamespace.RemoveDeclaration(method);
                method.Parameters.RemoveAt(0);
                foreach (var param in method.Parameters)
                {
                    param.Index--;
                }

                method.IsStatic = false;
                method.Class = @class;
                @class.AddMethod(method);
            }

            return true;
        }

        private bool GetMethodParameter(Parameter parameter, out Class @class)
        {
            @class = parameter.Type.Declaration as Class;
            if (@class == null || @class.ClassType != ClassType.StructWrapper || @class.ClassType == ClassType.UnionWrapper)
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
