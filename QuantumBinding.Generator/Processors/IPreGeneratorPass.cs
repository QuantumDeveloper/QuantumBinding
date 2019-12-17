using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public interface IPreGeneratorPass : IDeclarationVisitor<bool>
    {
        public bool RunAgain { get; set; }

        public uint RunIndex { get; }

        PreGeneratorOptions Options { get; }

        ProcessingContext ProcessingContext { get; set; }

        ASTContext AstContext { get; }

        void OnInitialize();

        void Run();

        void OnComplete();
    }
}