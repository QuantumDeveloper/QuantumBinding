using QuantumBinding.Generator;

namespace QuantumBinding.ClangGenerator;

class Program
{
    static void Main(string[] args)
    {
        QuantumBindingGenerator generator = new ClangBindingGenerator();
        generator.Run();
    }
}