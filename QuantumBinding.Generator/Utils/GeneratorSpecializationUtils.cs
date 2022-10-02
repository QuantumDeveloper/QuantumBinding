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
    }
}
