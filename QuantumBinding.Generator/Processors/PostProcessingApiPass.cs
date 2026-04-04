using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.ProcessingFluentApi;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors;

public class PostProcessingApiPass : PreGeneratorPass
{
    private readonly PostProcessingApi fixApi;
    private ClassExtension[] _newClasses = null;

    public PostProcessingApiPass(PostProcessingApi fixApi)
    {
        this.fixApi = fixApi;
        _newClasses = fixApi.GetNewClasses();
        Options.VisitFunctions = true;
        Options.VisitParameters = true;
        Options.VisitDelegates = true;
        Options.VisitClasses = true;
        Options.VisitEnums = true;
    }

    public override void OnBeforeUnitRun(TranslationUnit unit)
    {
        base.OnBeforeUnitRun(unit);
        var classes = _newClasses.Where(c => c.TranslationUnitFileName == unit.FileName).ToArray();

        if (classes.Any())
        {
            var classesToAdd = new List<Class>();
            foreach (var classExtension in classes)
            {
                var nativeStruct = unit.AllClasses.FirstOrDefault(x => x.Name == classExtension.NativeStructName);
                if (nativeStruct == null && classExtension.ClassType == ClassType.Class)
                    continue;

                var @class = new Class();
                @class.Name = classExtension.Name;
                @class.ClassType = classExtension.ClassType;
                @class.NativeStruct = nativeStruct;
                @class.Owner = unit;
                classesToAdd.Add(@class);

                var functions = unit.Functions.Where(x =>
                        x.Parameters.Any(p => p.Type.Declaration is Class decl && decl.Name == nativeStruct.Name) ||
                        x.ReturnType.Declaration is Class decl && decl.Name == nativeStruct.Name)
                    .ToList();

                foreach (var function in functions)
                {
                    foreach (var parameter in function.Parameters)
                    {
                        if (parameter.Type.IsCustomType(out CustomType customType))
                        {
                            if (DeclarationUnit.DummyTypes.TryGetValue(customType.Name, out string realTypeName))
                            {
                                customType.Name = realTypeName;
                            }
                        }

                        var decl = GetDeclarationFromCustomType(parameter.Type);

                        if (decl is Class { ClassType: ClassType.Struct } @classDecl &&
                            @classDecl.Name == nativeStruct.Name)
                        {
                            parameter.Type.Declaration = @class;
                        }
                    }

                    var returnDecl = GetDeclarationFromCustomType(function.ReturnType);
                    if (returnDecl is Class { ClassType: ClassType.Struct } @classDecl1 &&
                        @classDecl1.Name == nativeStruct.Name)
                    {
                        function.ReturnType.Declaration = @class;
                    }
                }
            }

            unit.AddDeclarations(classesToAdd);
        }

        var allWrappers = fixApi.GetAllWrappers();

        if (allWrappers == null) return;

        var wrappers = unit.Structs.ToList();
        foreach (var field in allWrappers.Fields)
        {
            var selectedWrappers = wrappers.Where(x => x.Fields.Any(y => y.Name == field.Name)).ToList();
            foreach (var wrapper in selectedWrappers)
            {
                var wrapperField = wrapper.Fields.FirstOrDefault(x => x.Name == field.Name);
                UpdateFieldDeclaration(wrapperField, field);
            }
        }
    }

    private Declaration GetDeclarationFromCustomType(BindingType type)
    {
        type.IsCustomType(out CustomType custom);
        Declaration decl = null;
        if (custom != null)
        {
            foreach (var unit in AstContext.TranslationUnits)
            {
                decl = unit.Declarations.FirstOrDefault(x => x.Name == custom.Name);

                if (decl != null)
                    break;
            }
        }

        return decl;
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
            var param = function.Parameters.FirstOrDefault(x => x.Name == parameter.Name);
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

        if (!fixApi.TryGetDelegate(@delegate.Name, false, out var delegateFix))
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

        if (!string.IsNullOrEmpty(classFix.LinkedClassName))
        {
            @class.LinkedTo = CurrentNamespace.Classes.FirstOrDefault(x => x.Name == classFix.LinkedClassName);
        }

        @class.IsIgnored = classFix.IsIgnored;

        if (classFix.CleanObject)
        {
            @class.ClearConstructors();
            @class.ClearProperties();
            @class.ClearOperators();
            @class.ClearMethods();
            @class.ClearFields();
        }

        if (classFix.CopyFieldsFromLinkedObject && @class.NativeStruct != null)
        {
            foreach (var field in @class.NativeStruct.Fields)
            {
                @class.AddField(field);
            }
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

            UpdateFieldDeclaration(field, fieldFix);
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

            UpdateFieldDeclaration(field, fieldFix);

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

        foreach (var fixOperator in classFix.Operators)
        {
            var field = @class.Fields.FirstOrDefault(x => x.Name == fixOperator.FieldName);

            if (field == null)
                continue;

            var op = new Operator();
            op.Class = @class;
            op.FieldName = fixOperator.FieldName;
            op.Type = field.Type;
            op.TransformationKind = fixOperator.TransformationKind;
            op.OperatorKind = fixOperator.OperatorKind;
            @class.Operators.Add(op);
        }

        foreach (var constructor in classFix.Constructors)
        {
            var ctor = new Constructor() { Class = @class, IsDefault = constructor.IsDefault };
            foreach (var parameter in constructor.Parameters)
            {
                var inputParameter = new Parameter();
                inputParameter.Name = parameter.Name;
                inputParameter.Type = parameter.Type;
                inputParameter.ParameterKind = parameter.ParameterKind;
                inputParameter.DefaultValue = parameter.DefaultValue;

                UpdateParameterDeclaration(inputParameter, parameter);

                ctor.InputParameters.Add(inputParameter);
            }

            @class.Constructors.Add(ctor);
        }

        return true;
    }

    private void UpdateFieldDeclaration(Field field, FieldExtension fieldFix)
    {
        if (field.Type != null)
        {
            var decl = field.Type.Declaration;
            field.Type = fieldFix.Type;
            if (fieldFix.ReplaceDeclaration)
            {
                foreach (var translationUnit in AstContext.TranslationUnits)
                {
                    var decls = translationUnit.Declarations.Where(x =>
                        x.Name == fieldFix.Type.ToString()).ToList();
                    decl = decls.Count > 1
                        ? decls.FirstOrDefault(x => x.GetType() == fieldFix.DeclarationType)
                        : decls.FirstOrDefault();

                    if (decl != null) break;
                }
            }

            field.Type.Declaration = decl;
        }
    }

    private void UpdateParameterDeclaration(Parameter parameter, ParameterExtension parameterFix)
    {
        if (parameter.Type != null)
        {
            var decl = parameter.Type.Declaration;
            parameter.Type = parameterFix.Type;
            if (parameterFix.ReplaceDeclaration)
            {
                foreach (var translationUnit in AstContext.TranslationUnits)
                {
                    var decls = translationUnit.Declarations.Where(x =>
                        x.Name == parameterFix.Type.ToString()).ToList();
                    decl = decls.Count > 1
                        ? decls.FirstOrDefault(x => x.GetType() == parameterFix.DeclarationType)
                        : decls.FirstOrDefault();

                    if (decl != null) break;
                }
            }

            parameter.Type.Declaration = decl;
        }
    }

    public override bool VisitEnum(Enumeration enumeration)
    {
        if (IsVisited(enumeration))
        {
            return false;
        }

        if (!fixApi.TryGetEnum(enumeration.Name, false, out EnumExtension enumExtension))
        {
            return false;
        }

        enumeration.IsFlagEnum = enumExtension.IsFlagsEnum;

        return true;
    }
}