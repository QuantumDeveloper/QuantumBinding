using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.AST;

public abstract class DeclarationUnit : Declaration
{
    protected DeclarationUnit()
    {
        _declarations = new List<Declaration>();
        Namespaces = new List<Namespace>();
    }

    static DeclarationUnit()
    {
        DummyTypes = new Dictionary<string, string>();
    }

    private List<Declaration> _declarations;

    public List<Namespace> Namespaces { get; private set; }
    
    public IReadOnlyList<GlobalUsings> GlobalUsings => _declarations.Where(x => x is GlobalUsings).OfType<GlobalUsings>().ToList();
    
    public IReadOnlyList<DispatchTable> DispatchTables => _declarations.Where(x => x is DispatchTable).OfType<DispatchTable>().ToList();

    public IReadOnlyList<Macro> Macros => _declarations.Where(x => x is Macro).OfType<Macro>().ToList();

    public IReadOnlyList<Enumeration> Enums =>
        _declarations.Where(x => x is Enumeration).OfType<Enumeration>().ToList();

    public IReadOnlyList<Class> AllClasses => _declarations.Where(x => x is Class).OfType<Class>().ToList();

    public IReadOnlyList<Class> Classes => AllClasses.Where(x => x.ClassType == ClassType.Class).ToList();

    public IReadOnlyList<Class> Structs => AllClasses.Where(x => x.ClassType == ClassType.Struct).ToList();

    public IReadOnlyList<Class> Unions => AllClasses.Where(x => x.ClassType == ClassType.Union).ToList();

    public IReadOnlyList<Class> Wrappers => AllClasses
        .Where(x => x.ClassType is ClassType.StructWrapper or ClassType.UnionWrapper).ToList();

    public IReadOnlyList<Class> StructWrappers =>
        AllClasses.Where(x => x.ClassType == ClassType.StructWrapper).ToList();

    public IReadOnlyList<Class> UnionWrappers =>
        AllClasses.Where(x => x.ClassType == ClassType.UnionWrapper).ToList();

    public IReadOnlyList<Class> ExtensionClasses => TranslationUnitsPool.SelectMany(x => x.Classes)
        .Where(x => x.HasExtensions && x.ExtensionMethods.Any(z => z.Owner == this)).ToList();

    public IReadOnlyList<Delegate> Delegates => _declarations.Where(x => x is Delegate).OfType<Delegate>().ToList();

    public IReadOnlyList<Function> Functions =>
        _declarations.Where(x => x is Function && !(x is Method)).OfType<Function>().ToList();

    public IReadOnlyList<Method> StaticMethods => _declarations.Where(x => x is Method).OfType<Method>().ToList();

    public IReadOnlyList<Declaration> Declarations => _declarations;

    public IReadOnlyList<Declaration> IgnoredDeclarations =>
        _declarations.Where(x => x.IsIgnored).ToList();

    public static Dictionary<string, string> DummyTypes { get; }

    public static List<TranslationUnit> TranslationUnitsPool { get; set; }

    public bool IsEmpty => IsUnitEmpty();

    private bool IsUnitEmpty()
    {
        return (_declarations.Count == 0 || _declarations.Except(IgnoredDeclarations).ToList().Count == 0) &&
               ExtensionClasses.Count == 0;
    }

    public bool IsSpecializationsAvailable(GeneratorSpecializations specializations)
    {
        var specs = specializations.GetFlags();
        bool isAvailable = false;
        foreach (var spec in specs)
        {
            switch (spec)
            {
                case GeneratorSpecializations.Classes:
                    isAvailable |= Classes.Count > 0 ||
                                   ExtensionClasses.Where(x => x.ClassType == ClassType.Class).ToList().Count > 0 ||
                                   StaticMethods.Count > 0;
                    break;
                case GeneratorSpecializations.Structs:
                    isAvailable |= Structs.Count > 0 ||
                                   ExtensionClasses.Where(x => x.ClassType == ClassType.Struct).ToList().Count > 0;
                    break;
                case GeneratorSpecializations.Unions:
                    isAvailable |= Unions.Count > 0;
                    break;
                case GeneratorSpecializations.StructWrappers:
                    isAvailable |= StructWrappers.Count > 0 ||
                                   ExtensionClasses.Where(x => x.ClassType == ClassType.Struct).ToList().Count > 0;
                    break;
                case GeneratorSpecializations.UnionWrappers:
                    isAvailable |= UnionWrappers.Count > 0;
                    break;
                case GeneratorSpecializations.Enums:
                    isAvailable |= Enums.Count > 0;
                    break;
                case GeneratorSpecializations.Functions:
                    isAvailable |= Functions.Count > 0;
                    break;
                case GeneratorSpecializations.Delegates:
                    isAvailable |= Delegates.Count > 0;
                    break;
                case GeneratorSpecializations.Macros:
                    isAvailable |= Macros.Count > 0;
                    break;
                case GeneratorSpecializations.ExtensionMethods:
                    isAvailable |= ExtensionClasses.Count > 0;
                    break;
                case GeneratorSpecializations.GlobalUsings:
                    isAvailable |= GlobalUsings.Count > 0;
                    break;
                case GeneratorSpecializations.DispatchTables:
                    isAvailable |= DispatchTables.Count > 0;
                    break;
            }
        }

        return isAvailable;
    }

    public void AddDummyType(string dummyType, string originalType)
    {
        DummyTypes.TryAdd(dummyType, originalType);
    }

    public DeclarationUnit FindNamespace(string @namespace)
    {
        return Namespaces.FirstOrDefault(x => x.Name == @namespace);
    }

    public void RemoveClasses(Class[] classes)
    {
        for (int i = 0; i < classes.Length; ++i)
        {
            _declarations.Remove(classes[i]);
        }
    }

    public IEnumerable<Declaration> FindDeclarationsBySourceLocation(string fileName, bool remove)
    {
        var declsOutput = new List<Declaration>();

        if (remove)
        {
            var decls = Declarations.Where(x => x.Location.FileNameWithoutExtension == fileName).ToList();
            declsOutput.AddRange(decls);
            _declarations = Declarations.Except(decls).ToList();
        }
        else
        {
            declsOutput.AddRange(Declarations.Where(x => x.Location.FileNameWithoutExtension == fileName));
        }

        return declsOutput;
    }

    public void AddDeclarations(IEnumerable<Declaration> decs)
    {
        foreach (var declaration in decs)
        {
            AddDeclaration(declaration);
        }
    }

    public void AddDeclaration(Declaration declaration)
    {
        if (declaration == null) return;

        Declaration decl = null;
        switch (declaration)
        {
            case Enumeration @enum:
                decl = Enums.FirstOrDefault(x => x.Name == declaration.Name);
                break;
            case Class @class:
                switch (@class.ClassType)
                {
                    case ClassType.Class:
                        decl = Classes.FirstOrDefault(x => x.Name == declaration.Name);
                        break;
                    case ClassType.Struct:
                        decl = Structs.FirstOrDefault(x => x.Name == declaration.Name);
                        break;
                    case ClassType.Union:
                        decl = Unions.FirstOrDefault(x => x.Name == declaration.Name);
                        break;
                    case ClassType.StructWrapper:
                        decl = StructWrappers.FirstOrDefault(x => x.Name == declaration.Name);
                        break;
                    case ClassType.UnionWrapper:
                        decl = UnionWrappers.FirstOrDefault(x => x.Name == declaration.Name);
                        break;
                }
                break;
            
            case GlobalUsings globalUsings:
                decl = GlobalUsings.FirstOrDefault(x => x.Name == declaration.Name);
                break;
            
            case DispatchTable dispatchTable:
                decl = DispatchTables.FirstOrDefault(x => x.Name == declaration.Name);
                break;
        }

        if (decl == null)
        {
            _declarations.Add(declaration);
        }
    }

    public void RemoveDeclaration(Declaration declaration)
    {
        _declarations.Remove(declaration);
    }

    public bool IsWrapperPresent(string declName, out Declaration declaration)
    {
        var wrapper = Wrappers.FirstOrDefault(x => x.Name == declName);
        declaration = wrapper;
        return wrapper != null;
    }

    public Declaration FindDeclaration(CustomType customType)
    {
        if (customType == null)
            return null;

        foreach (var unit in TranslationUnitsPool)
        {
            var decl = unit.Declarations.FirstOrDefault(x => x.Name == customType.Name);
            if (decl != null)
            {
                return decl;
            }
        }

        return null;
    }

    public DeclarationUnit FindClass(string className)
    {
        return AllClasses.FirstOrDefault(x => x.Name == className);
    }

    public DeclarationUnit FindEnum(string enumName)
    {
        return Enums.FirstOrDefault(x => x.Name == enumName);
    }

    public DeclarationUnit FindOrCreateEnum(string enumName, bool create = false)
    {
        var @enum = FindEnum(enumName) as Enumeration;
        if (@enum == null && create)
        {
            @enum = new Enumeration();
            @enum.Name = enumName;
        }

        return @enum;
    }

    public DeclarationUnit FindDelegate(string delgateName)
    {
        return Delegates.FirstOrDefault(x => x.Name.Equals(delgateName));
    }

    public DeclarationUnit FindFunction(string functionName)
    {
        return Functions.FirstOrDefault(x => x.Name.Equals(functionName));
    }

    public List<Function> FindFunctionsWithParameter(params string[] paramTypes)
    {
        var functions = Functions.Where(x => x.Parameters.Any(y => paramTypes.Contains(y.Type.ToString())))
            .ToList();
        return functions;
    }

    public void ReplaceFunctionsParameter(DependentNameType dependentType, Class @class)
    {
        var functions = FindFunctionsWithParameter(dependentType.PointsTo, @class.Name);
        foreach (var function in functions)
        {
            foreach (var parameter in function.Parameters)
            {
                var type = parameter.Type;

                if (!type.IsCustomType(out CustomType customType))
                {
                    continue;
                }

                if (customType.Name.Equals(dependentType.Identifier) ||
                    customType.Name.Equals(dependentType.PointsTo))
                {
                    customType.Name = @class.Name;
                    type.Declaration = @class;
                }
            }
        }
    }
}