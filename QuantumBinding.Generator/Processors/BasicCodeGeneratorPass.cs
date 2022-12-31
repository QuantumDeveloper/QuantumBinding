using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Processors;

public class BasicCodeGeneratorPass : CodeGeneratorPass
{
    public BasicCodeGeneratorPass()
    {
        GeneratorSpecializations =
            GeneratorSpecializationUtils.AllExcept(GeneratorSpecializations.StructWrappers |
                                                   GeneratorSpecializations.UnionWrappers |
                                                   GeneratorSpecializations.Extensions);
    }
    
    protected override CodeGenerator OnCreateGenerator(GeneratorCategory category, params TranslationUnit[] units)
    {
        return new CSharpCodeGenerator(ProcessingContext, units, category);
    }

    protected override List<CodeGenerator> ProcessPerTypeCodeGeneration(TranslationUnit unit, GeneratorSpecializations specializations)
    {
        
        switch (specializations)
        {
            case GeneratorSpecializations.Enums:
                return ProcessDeclarations(unit.Enums, unit);
            case GeneratorSpecializations.Delegates:
                return ProcessDeclarations(unit.Delegates, unit);
            case GeneratorSpecializations.Classes:
                return ProcessDeclarations(unit.Classes, unit);
            case GeneratorSpecializations.Structs:
                return ProcessDeclarations(unit.Structs, unit);
            case GeneratorSpecializations.Unions:
                return ProcessDeclarations(unit.Unions, unit);
            case GeneratorSpecializations.Macros:
                // Generate macros/constants separately to have all macros in one file
                var macrosGenerator = OnCreateGenerator(GeneratorCategory.Macros, unit);
                macrosGenerator.Run();
                return new List<CodeGenerator>() { macrosGenerator };
            case GeneratorSpecializations.Functions:
                // Generate macros/constants separately to have all macros in one file
                var functionsGenerator = OnCreateGenerator(GeneratorCategory.Functions, unit);
                functionsGenerator.Run();
                return new List<CodeGenerator>() { functionsGenerator };
            case GeneratorSpecializations.StaticMethods:
                var methodsGenerator = OnCreateGenerator(GeneratorCategory.StaticMethods, unit);
                methodsGenerator.Run();
                return new List<CodeGenerator>() { methodsGenerator };
            case GeneratorSpecializations.Extensions:
                return ProcessDeclarations(unit.ExtensionClasses, unit);
            default:
                return new List<CodeGenerator>();
        }
        
        // var types = unit.Declarations.Where(x => x is not Macro && x is not Method && x is not Function && x.IsAllowed(specializations));
        //
        //
        // var extensions = unit.ExtensionClasses.ToArray();
        // foreach (var extension in extensions)
        // {
        //     var extensionCategory = GeneratorCategory.ExtensionMethods;
        //     var generator = OnCreateGenerator(extensionCategory, unit);
        //     generator.Run(extension);
        //     codeGenerators.Add(generator);
        // }
        
        // var macrosGenerator = OnCreateGenerator(GeneratorCategory.Constants, unit);
        // macrosGenerator.Run();
        // codeGenerators.Add(macrosGenerator);
                    
        // var functionsGenerator = OnCreateGenerator(GeneratorCategory.Functions, unit);
        // functionsGenerator.Run();
        // codeGenerators.Add(functionsGenerator);
                    
        // var methodsGenerator = OnCreateGenerator(GeneratorCategory.StaticMethods, unit);
        // methodsGenerator.Run();
        // codeGenerators.Add(methodsGenerator);
        //
        // return codeGenerators;
    }
}