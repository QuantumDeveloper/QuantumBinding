using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public class PrepareStructsBeforeWrappingPass : PreGeneratorPass
    {
        private readonly Dictionary<string, StructurePredefinedInput> predefinedValues;
        
        public PrepareStructsBeforeWrappingPass(IEnumerable<StructurePredefinedInput> predefinedValues)
        {
            Options.VisitClasses = true;
            this.predefinedValues = predefinedValues.ToDictionary(x=>x.StructType);
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class) || @class.ClassType != ClassType.Struct)
            {
                return false;
            }

            if (predefinedValues == null) return false;

            if (!predefinedValues.TryGetValue(@class.Name, out var values))
            {
                return true;
            }

            foreach (var field in @class.Fields)
            {
                if (values.FieldValues.TryGetValue(field.Name, out var value))
                {
                    field.PredefinedValue = value.Value;
                    field.IsPredefinedValueReadOnly = value.IsReadOnly;
                }
            }

            return true;
        }
    }
}