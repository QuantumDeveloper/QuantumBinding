using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public interface IPreGeneratorPass : IDeclarationVisitor<bool>
    {
        PreGeneratorOptions Options { get; }

        ProcessingContext ProcessingContext { get; set; }

        ASTContext AstContext { get; }

        void Run();
    }
}