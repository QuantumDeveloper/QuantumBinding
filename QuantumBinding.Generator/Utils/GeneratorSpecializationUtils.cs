using System.Collections.Generic;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator.Utils
{
    public static class GeneratorSpecializationUtils
    {
        public static GeneratorSpecializations All()
        {
            var specializations = GeneratorSpecializations.All;
            return specializations;
        }

        public static GeneratorSpecializations Except(this GeneratorSpecializations specializations, GeneratorSpecializations except)
        {
            return specializations & ~except;
        }

        public static GeneratorSpecializations AllExcept(GeneratorSpecializations except)
        {
            return GeneratorSpecializations.All & ~except;
        }

        public static List<GeneratorCategory> GetCategories(this GeneratorSpecializations specs)
        {
            var specsList = specs.GetFlags();
            var categories = new List<GeneratorCategory>();

            foreach (var spec in specsList)
            {
                categories.Add((GeneratorCategory)spec);
            }

            return categories;
        }
    }
}
