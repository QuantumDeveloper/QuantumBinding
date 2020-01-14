using System;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.Processors
{
    public class UpdateWrappedMethodParametersPass : PreGeneratorPass
    {
        public UpdateWrappedMethodParametersPass(GeneratorSpecializations specializations)
        {
            Specializations = specializations;
            Options.VisitClasses = true;
            Options.VisitMethods = true;
        }

        public GeneratorSpecializations Specializations { get; }

        public override bool VisitMethod(Method method)
        {
            if (IsVisited(method))
            {
                return false;
            }

            UpdateReturnType(method);
            UpdateMethodParameters(method);

            return true;
        }


        private void UpdateReturnType(Method method)
        {
            if (method.ReturnType.Declaration is Class decl)
            {
                if (decl.ClassType != ClassType.Struct && decl.ClassType != ClassType.Union) return;

                var declaration = FindDeclaration(method.ReturnType);

                var returnType = (BindingType)method.ReturnType.Clone();
                returnType.Declaration = declaration;
                method.ReturnType = returnType;
            }
        }

        private void UpdateMethodParameters(Method method)
        {
            foreach (var parameter in method.Parameters)
            {
                var declaration = FindDeclaration(parameter.Type);
                if (declaration != null)
                {
                    var wrappedType = parameter.Type;
                    parameter.WrappedType = wrappedType;
                    parameter.Type = (BindingType)wrappedType.Clone();
                    parameter.Type.Declaration = declaration;
                }
            }
        }

        private Declaration FindDeclaration(BindingType type)
        {
            var decl = type.Declaration as Class;
            if (decl == null) return null;
            if (decl.ClassType != ClassType.Struct && decl.ClassType != ClassType.Union) return null;

            if (decl.IsSimpleType)
            {
                return decl;
            }

            Class declaration = null;
            foreach (var unit in AstContext.TranslationUnits)
            {
                if (decl.ClassType == ClassType.Struct)
                {
                    declaration = unit.StructWrappers.FirstOrDefault(x => x.Id == decl.Id);

                    if (declaration == null)
                    {
                        declaration = unit.Classes.FirstOrDefault(x => x.Id == decl.Id);
                    }
                }
                else if (decl.ClassType == ClassType.Union)
                {
                    declaration = unit.UnionWrappers.FirstOrDefault(x => x.Id == decl.Id);
                }

                if (declaration != null) break;
            }

            return declaration;
        }
    }
}
