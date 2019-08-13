using QuantumBinding.ClangGenerator;
using QuantumBinding.Generator;

namespace QuantumBinding.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            QuantumBindingGenerator generator = new ClangBindingGenerator();
            generator.Run();
        }
    }
}
