using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors
{
    public class NormalizeParametersPass : PreGeneratorPass
    {
        public NormalizeParametersPass()
        {
            Options.VisitFunctions = true;
            Options.VisitDelegates = true;
            Options.VisitClasses = true;
            Options.VisitFields = true;
            Options.VisitProperties = true;
            Options.VisitParameters = true;
        }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            var decl = GetDeclarationFromCustomType(function.ReturnType);
            function.ReturnType.Declaration = decl;

            return true;
        }

        public override bool VisitMethod(Method method)
        {
            if (IsVisited(method))
            {
                return false;
            }

            var decl = GetDeclarationFromCustomType(method.ReturnType);
            method.ReturnType.Declaration = decl;

            return true;
        }

        public override bool VisitDelegate(Delegate @delegate)
        {
            if (IsVisited(@delegate))
            {
                return false;
            }

            Declaration decl = GetDeclarationFromCustomType(@delegate.ReturnType);
            @delegate.ReturnType.Declaration = decl;

            return true;
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
            {
                return false;
            }

            if (!CurrentNamespace.Module.AllowConvertStructToClass && @class.InnerStruct != null)
            {
                @class.InnerStruct.IsIgnored = true;
                return true;
            }

            if (@class.IsPointer && @class.InnerStruct != null && @class.InnerStruct.Fields.Count == 0)
            {
                var field = new Field("pointer");
                field.AccessSpecifier = AccessSpecifier.Public;
                field.Name = "pointer";
                field.Type = new PointerType() { Pointee = new BuiltinType(PrimitiveType.Void) };
                @class.InnerStruct.AddField(field);
            }

            return true;
        }

        public override bool VisitField(Field field)
        {
            if (IsVisited(field))
            {
                return false;
            }

            if (field.Type.IsCustomType(out CustomType customType))
            {
                if (DeclarationUnit.DummyTypes.TryGetValue(customType.Name, out string realTypeName))
                {
                    customType.Name = realTypeName;
                }

                Declaration decl = null;
                foreach (var unit in AstContext.TranslationUnits)
                {
                    decl = unit.Declarations.FirstOrDefault(x => x.Name == customType.Name);
                    if (decl != null) break;
                }

                if (decl is Delegate @delegate)
                {
                    field.Type = new DelegateType() { Name = customType.Name, Declaration = decl };
                }
                else
                {
                    field.Type.Declaration = decl;
                }
            }

            if (field.Type.IsPointer()) // if parameter is pointer to any type, then make it nullable for future possible manipulations
            {
                var pointerType = (PointerType)field.Type;
                pointerType.IsNullable = true;
            }

            if (CodeGenerator.ReservedWords.Contains(field.Name))
            {
                field.Name = "@" + field.Name;
            }

            if (field.Class.ClassType == ClassType.Union)
            {
                field.AddAttribute("[FieldOffset(0)]");
            }

            return true;
        }

        public override bool VisitProperty(Property property)
        {
            if (CodeGenerator.ReservedWords.Contains(property.Name))
            {
                property.Name = "@" + property.Name;
            }

            return true;
        }

        public override bool VisitParameter(Parameter parameter)
        {
            if (parameter.Type.IsCustomType(out CustomType customType))
            {
                if (DeclarationUnit.DummyTypes.TryGetValue(customType.Name, out string realTypeName))
                {
                    customType.Name = realTypeName;
                }
            }

            Declaration decl = GetDeclarationFromCustomType(parameter.Type);
            parameter.Type.Declaration = decl;

            Class classDecl = decl as Class;

            var type = parameter.Type;
            switch (type)
            {
                case ArrayType array when array.IsConst && !array.CanConvertToString():
                    parameter.ParameterKind = ParameterKind.Readonly;
                    break;
                case PointerType pointer when pointer.IsConst && !pointer.CanConvertToString():
                    parameter.ParameterKind = ParameterKind.Readonly;
                    break;
                case PointerType pointer when !pointer.CanConvertToString() && pointer.Pointee.IsPrimitiveType || classDecl?.IsSimpleType == true || decl is Enumeration:
                    parameter.ParameterKind = ParameterKind.Ref;
                    break;
                case PointerType pointer when !pointer.Pointee.IsPrimitiveType && !pointer.IsConst && !pointer.IsPointerToStructOrUnion():
                    {
                        parameter.ParameterKind = ParameterKind.Out;
                        break;
                    }
                default:
                    parameter.ParameterKind = ParameterKind.In;
                    break;
            }

            if (parameter.Name.StartsWith("out")) //small hack to set correct parameter kind in cases developers calling out parameters starting with outxxx or out_xxx
            {
                parameter.ParameterKind = ParameterKind.Out;
            }

            if (CodeGenerator.ReservedWords.Contains(parameter.Name))
            {
                parameter.Name = "@" + parameter.Name;
            }

            if (parameter.Type.IsPointer() && !parameter.Type.IsPointerToArray() && parameter.ParameterKind != ParameterKind.Out)
            {
                ((PointerType)parameter.Type).IsNullable = true;
            }

            return true;
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
                    if (decl != null) break;
                }
            }

            return decl;
        }
    }
}