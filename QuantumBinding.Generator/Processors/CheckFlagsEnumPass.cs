using System;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Processors
{
    public class CheckFlagsEnumsPass : PreGeneratorPass
    {
        public CheckFlagsEnumsPass()
        {
            Options.VisitEnums = true;
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            foreach (var item in enumeration.Items)
            {
                try
                {
                    var value = Convert.ToInt64(item.Value);

                    if (value < 0)
                    {
                        return false;
                    }

                    if (!ClangUtils.IsPowerOfTwo((uint)value) && value != int.MaxValue)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            enumeration.IsFlagEnum = true;

            return true;
        }
    }
}