using System;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.Processors;

[Flags]
public enum VisitOptions : uint
{
    None = 0,
    VisitFunctions = 1,
    VisitDelegates = 2,
    VisitClasses = 4,
    VisitFields = 8,
    VisitProperties = 16,
    VisitParameters = 32,
    All = VisitFunctions | VisitDelegates | VisitClasses | VisitFields | VisitProperties | VisitParameters
}
    
public class NormalizeParametersPass : PreGeneratorPass
{
    public NormalizeParametersPass(VisitOptions options = VisitOptions.All)
    {
        if (options.HasFlag(VisitOptions.VisitFunctions)) Options.VisitFunctions = true;
        if (options.HasFlag(VisitOptions.VisitDelegates)) Options.VisitDelegates = true;
        if (options.HasFlag(VisitOptions.VisitClasses)) Options.VisitClasses = true;
        if (options.HasFlag(VisitOptions.VisitFields)) Options.VisitFields = true;
        if (options.HasFlag(VisitOptions.VisitProperties)) Options.VisitProperties = true;
        if (options.HasFlag(VisitOptions.VisitParameters)) Options.VisitParameters = true;
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

        if (!CurrentNamespace.Module.AllowConvertStructToClass && @class.NativeStruct != null)
        {
            @class.NativeStruct.IsIgnored = true;
            return true;
        }

        if (@class.IsPointer && @class.NativeStruct != null && @class.NativeStruct.Fields.Count == 0)
        {
            var field = new Field("pointer");
            field.AccessSpecifier = AccessSpecifier.Public;
            field.Name = "pointer";
            field.Type = new PointerType() { Pointee = new BuiltinType(PrimitiveType.Void) };
            @class.NativeStruct.AddField(field);
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

        var decl = GetDeclarationFromCustomType(parameter.Type);
            
        if (decl is Class { ClassType: ClassType.Struct, LinkedTo: not null } @class)
        {
            decl = @class.LinkedTo;
        }
            
        parameter.Type.Declaration = decl;

        var classDecl = decl as Class;

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
            case PointerType pointer when pointer.GetDepth() >= 3:
            {
                parameter.ParameterKind = ParameterKind.Out;
                break;
            }
            default:
                parameter.ParameterKind = ParameterKind.In;
                break;
        }

        if (parameter.Name.StartsWith("out")) //small hack to set the correct parameter kind in cases developers calling out parameters starting with outxxx or out_xxx
        {
            parameter.ParameterKind = ParameterKind.Out;
        }

        if (CodeGenerator.ReservedWords.Contains(parameter.Name.ToLower()))
        {
            parameter.Name = "@" + parameter.Name.ToLower();
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

                if (decl != null) 
                    break;
            }
        }

        return decl;
    }
}