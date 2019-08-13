using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public class GetterSetterToPropertyPass : PreGeneratorPass
    {
        public GetterSetterToPropertyPass()
        {
            Options.VisitClasses = true;
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
                return false;



            return true;
        }
    }
}