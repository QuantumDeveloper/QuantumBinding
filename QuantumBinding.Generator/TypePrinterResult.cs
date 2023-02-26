using System;

namespace QuantumBinding.Generator
{
    public class TypePrinterResult
    {
        public TypePrinterResult()
        {
            Attribute = String.Empty;
            ParameterModifier = String.Empty;
            Type = String.Empty;
            TypeSuffix = String.Empty;
            ParameterSuffix = String.Empty;
        }
        public string ParameterModifier { get; set; }
        public string Type { get; set; }

        public string TypeSuffix { get; set; }
        
        public string Attribute { get; set; }
        
        public string ParameterSuffix { get; set; }

        public static implicit operator TypePrinterResult(string type)
        {
            return new TypePrinterResult { Type = type };
        }

        public string MergeResult()
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(ParameterModifier))
            {
                result = $"{ParameterModifier} ";
            }
            
            if (!string.IsNullOrEmpty(Attribute))
            {
                result += $"{Attribute} ";
            }

            result += Type;

            if (!string.IsNullOrEmpty(TypeSuffix))
            {
                result += $"{TypeSuffix}";
            }

            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(ParameterModifier))
            {
                result = $"{ParameterModifier} ";
            }

            result += $"{Type}{TypeSuffix}";

            return result;
        } 
    }
}