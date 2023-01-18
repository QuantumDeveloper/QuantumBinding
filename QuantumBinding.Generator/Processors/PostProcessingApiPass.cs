using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.ProcessingFluentApi;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors
{
    public class PostProcessingApiPass : PreGeneratorPass
    {
        private readonly PostProcessingApi fixApi;

        public PostProcessingApiPass(PostProcessingApi fixApi)
        {
            this.fixApi = fixApi;
            Options.VisitFunctions = true;
            Options.VisitParameters = true;
            Options.VisitDelegates = true;
            Options.VisitClasses = true;
        }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            if (!fixApi.TryGetFunction(function.Name, false, out FunctionExtension func))
            {
                return false;
            }

            if (func.ReturnType != null)
            {
                function.ReturnType = func.ReturnType;
            }

            function.Name = func.DecoratedName;

            if (!string.IsNullOrEmpty(func.EntryPointName))
            {
                function.EntryPoint = func.EntryPointName;
            }

            foreach (var parameter in func.Parameters)
            {
                var param = function.Parameters.FirstOrDefault(x=> x.Name == parameter.Name);
                if (param == null)
                {
                    continue;
                }

                if (parameter.Type != null)
                {
                    var decl = param.Type.Declaration;
                    param.Type = parameter.Type;
                    if (parameter.ReplaceDeclaration)
                    {
                        foreach (var unit in AstContext.TranslationUnits)
                        {
                            decl = unit.Declarations.FirstOrDefault(x => x.Name == parameter.Type.ToString());
                            if (decl != null) break;
                        }
                    }
                    param.Type.Declaration = decl;
                }

                if (parameter.HasDefaultValue)
                {
                    param.DefaultValue = parameter.DefaultValue;
                }

                if (parameter.ParameterKind != ParameterKind.Unknown)
                {
                    param.ParameterKind = parameter.ParameterKind;
                }
            }

            return true;
        }

        public override bool VisitParameter(Parameter parameter)
        {
            if (!fixApi.TryGetCommonParameterType(parameter.Type, out var parameterToApply))
            {
                return false;
            }

            if (parameterToApply.ParameterKind != ParameterKind.Unknown)
            {
                parameter.ParameterKind = parameterToApply.ParameterKind;
            }

            if (parameterToApply.HasDefaultValue)
            {
                parameter.DefaultValue = parameterToApply.DefaultValue;
            }

            if (parameterToApply.Type != null)
            {
                var decl = parameter.Type.Declaration;
                parameter.Type = parameterToApply.Type;
                parameter.Type.Declaration = decl;
            }

            return true;
        }

        public override bool VisitDelegate(Delegate @delegate)
        {
            if (IsVisited(@delegate))
            {
                return false;
            }

            if (!fixApi.TryGetDelegate(@delegate.Name, false, out DelegateExtension delegateFix))
            {
                return false;
            }

            if (delegateFix.ReturnType != null)
            {
                @delegate.ReturnType = delegateFix.ReturnType;
            }

            foreach (var parameter in delegateFix.Parameters)
            {
                var param = @delegate.Parameters.FirstOrDefault(x => x.Name == parameter.Name);
                if (param == null)
                {
                    continue;
                }

                if (parameter.Type != null)
                {
                    var decl = param.Type.Declaration;
                    param.Type = parameter.Type;
                    if (parameter.ReplaceDeclaration)
                    {
                        foreach (var unit in AstContext.TranslationUnits)
                        {
                            decl = unit.Declarations.FirstOrDefault(x => x.Name == parameter.Type.ToString());
                            if (decl != null) break;
                        }
                    }
                    param.Type.Declaration = decl;
                }

                if (parameter.ParameterKind != ParameterKind.Unknown)
                {
                    param.ParameterKind = parameter.ParameterKind;
                }
            }

            return true;
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
            {
                return false;
            }

            if (!fixApi.TryGetClass(@class.Name, false, out ClassExtension classFix))
            {
                return false;
            }

            if (@class.Name == "VkSubmitInfo")
            {
                int bug = 0;
            }
            
            if (classFix.IsDisposable)
            {
                @class.IsDisposable = classFix.IsDisposable;
                @class.DisposeBody = classFix.DisposeBody;
            }

            if (classFix.ClassType != ClassType.Unknown)
            {
                @class.ClassType = classFix.ClassType;
            }

            if (classFix.UnderlyingNativeType != null)
            {
                @class.UnderlyingNativeType = classFix.UnderlyingNativeType;
            }

            foreach (var fieldFix in classFix.Fields)
            {
                var field = @class.Fields.FirstOrDefault(x => x.Name == fieldFix.Name);
                if (field == null)
                {
                    continue;
                }

                if (fieldFix.RemoveExistingAttributes)
                {
                    field.Attributes.Clear();
                }

                foreach (var attribute in fieldFix.Attributes)
                {
                    field.AddAttribute(attribute);
                }

                if (field.Type != null)
                {
                    var decl = field.Type.Declaration;
                    field.Type = fieldFix.Type;
                    if (fieldFix.ReplaceDeclaration)
                    {
                        foreach (var translationUnit in AstContext.TranslationUnits)
                        {
                            decl = translationUnit.Declarations.FirstOrDefault(x => x.Name == fieldFix.Type.ToString());
                            if (decl != null) break;
                        }
                    }
                    field.Type.Declaration = decl;
                }
            }

            foreach (var fieldFix in classFix.FieldsToAdd)
            {
                var field = @class.Fields.FirstOrDefault(x => x.Name == fieldFix.Name);
                if (field != null)
                {
                    continue;
                }

                field = new Field();
                field.Name = fieldFix.Name;
                foreach (var attribute in fieldFix.Attributes)
                {
                    field.AddAttribute(attribute);
                }

                field.Type = fieldFix.Type;

                @class.AddField(field);
            }

            foreach (var propertyFix in classFix.PropertiesToAdd)
            {
                var property = @class.Properties.FirstOrDefault(x => x.Name == propertyFix.Name);
                if (property != null)
                {
                    continue;
                }

                property = new Property();
                property.Name = propertyFix.Name;
                property.Type = propertyFix.Type;
                property.Getter = propertyFix.Getter;
                property.Setter = propertyFix.Setter;
                property.Field = new Field(propertyFix.Field.Name);

                @class.AddProperty(property);
            }

            return true;
        }
    }
}