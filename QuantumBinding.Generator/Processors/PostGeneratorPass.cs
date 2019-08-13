using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public abstract class PostGeneratorPass : IPostGeneratorPass
    {
        protected PostGeneratorPass()
        {
        }

        public ProcessingContext ProcessingContext { get; set; }
        public ASTContext AstContext => ProcessingContext.AstContext;
        public void Run()
        {
            foreach (var generatorOutput in AstContext.GeneratorOutputs)
            {
                VisitGeneratorOutput(generatorOutput);
            }
        }

        public virtual void VisitGeneratorOutput(GeneratorOutput output)
        {

        }
    }
}