using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public abstract class DeclarationUnit : Declaration
    {
        protected DeclarationUnit()
        {
            declarations = new List<DeclarationUnit>();
            Namespaces = new List<Namespace>();
        }

        static DeclarationUnit()
        {
            DummyTypes = new Dictionary<string, string>();
        }

        private List<DeclarationUnit> declarations;

        public List<Namespace> Namespaces { get; private set; }

        public ReadOnlyCollection<Macro> Macros => declarations.Where(x => x is Macro).Cast<Macro>().ToList().AsReadOnly();

        public ReadOnlyCollection<Enumeration> Enums => declarations.Where(x => x is Enumeration).Cast<Enumeration>().ToList().AsReadOnly();

        public ReadOnlyCollection<Class> AllClasses => declarations.Where(x => x is Class).Cast<Class>().ToList().AsReadOnly();

        public ReadOnlyCollection<Class> Classes => AllClasses.Where(x => x.ClassType == ClassType.Class).ToList().AsReadOnly();

        public ReadOnlyCollection<Class> Structs => AllClasses.Where(x => x.ClassType == ClassType.Struct).ToList().AsReadOnly();

        public ReadOnlyCollection<Class> Unions => AllClasses.Where(x => x.ClassType == ClassType.Union).ToList().AsReadOnly();

        public ReadOnlyCollection<Class> StructWrappers => AllClasses.Where(x => x.ClassType == ClassType.StructWrapper).ToList().AsReadOnly();

        public ReadOnlyCollection<Class> UnionWrappers => AllClasses.Where(x => x.ClassType == ClassType.UnionWrapper).ToList().AsReadOnly();

        public ReadOnlyCollection<Class> ExtensionClasses => TranslationUnitsPool.SelectMany(x=>x.Classes).Where(x =>x.HasExtensions && x.ExtensionMethods.Any(z => z.Owner == this)).ToList().AsReadOnly();

        public ReadOnlyCollection<Delegate> Delegates => declarations.Where(x => x is Delegate).Cast<Delegate>().ToList().AsReadOnly();

        public ReadOnlyCollection<Function> Functions => declarations.Where(x => x is Function && !(x is Method)).Cast<Function>().ToList().AsReadOnly();

        public ReadOnlyCollection<Method> Methods => declarations.Where(x => x is Method).Cast<Method>().ToList().AsReadOnly();

        public ReadOnlyCollection<DeclarationUnit> Declarations => declarations.AsReadOnly();

        public ReadOnlyCollection<DeclarationUnit> IgnoredDeclarations =>
            declarations.Where(x => x.IsIgnored).ToList().AsReadOnly();

        public static Dictionary<string, string> DummyTypes { get; }

        public static List<TranslationUnit> TranslationUnitsPool { get; set; }

        public bool IsEmpty => IsUnitEmpty();

        private bool IsUnitEmpty()
        {
            return (declarations.Count == 0 || declarations.Except(IgnoredDeclarations).ToList().Count == 0) && ExtensionClasses.Count == 0;
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
                            Methods.Count > 0;
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
                    case GeneratorSpecializations.Constants:
                        isAvailable |= Macros.Count > 0;
                        break;
                }
            }

            return isAvailable;
        }

        public void AddDummyType(string dummyType, string originalType)
        {
            if (!DummyTypes.ContainsKey(dummyType))
            {
                DummyTypes.Add(dummyType, originalType);
            }
        }

        public DeclarationUnit FindNamespace(string @namespace)
        {
            return Namespaces.FirstOrDefault(x => x.Name == @namespace);
        }

        public void RemoveClasses(Class[] classes)
        {
            for (int i = 0; i < classes.Length; ++i)
            {
                declarations.Remove(classes[i]);
            }
        }

        public IEnumerable<DeclarationUnit> FindDeclarationsBySourceLocation(string fileName, bool remove)
        {
            var declsOutput = new List<DeclarationUnit>();

            if (remove)
            {
                var decls = Declarations.Where(x => x.Location.FileNameWithoutExtension == fileName).ToList();
                declsOutput.AddRange(decls);
                declarations = Declarations.Except(decls).ToList();
            }
            else
            {
                declsOutput.AddRange(Declarations.Where(x => x.Location.FileNameWithoutExtension == fileName));
            }

            return declsOutput;
        }

        public void AddDeclarations(IEnumerable<DeclarationUnit> decs)
        {
            foreach (var declaration in decs)
            {
                AddDeclaration(declaration);
            }
        }

        public void AddDeclaration(DeclarationUnit declaration)
        {
            declarations.Add(declaration);
        }

        private void RemoveDeclaration(DeclarationUnit declaration)
        {
            declarations.Remove(declaration);
        }

        public DeclarationUnit FindDeclaration(CustomType customType)
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
            var functions = Functions.Where(x => x.Parameters.Any(y => paramTypes.Contains(y.Type.ToString()))).ToList();
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

                    if (customType.Name.Equals(dependentType.Identifier) || customType.Name.Equals(dependentType.PointsTo))
                    {
                        customType.Name = @class.Name;
                        type.Declaration = @class;
                    }
                }
            }
        }
    }
}