using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public interface IPostGeneratorPass
    {
        ProcessingContext ProcessingContext { get; set; }

        ASTContext AstContext { get; }

        void Run();
    }
}