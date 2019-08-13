using System.Runtime.InteropServices;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public class ForceCallingConventionPass : PreGeneratorPass
    {
        public ForceCallingConventionPass(CallingConvention callingConvention)
        {
            Options.VisitFunctions = true;
            Options.VisitDelegates = true;
            CallingConvention = callingConvention;
        }

        public CallingConvention CallingConvention { get; }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            function.CallingConvention = CallingConvention;

            return true;
        }

        public override bool VisitDelegate(Delegate @delegate)
        {
            if (IsVisited(@delegate))
            {
                return false;
            }

            @delegate.CallingConvention = CallingConvention;

            return true;
        }
    }
}