namespace QuantumBinding.Generator
{
    public class TypePrinterResult
    {
        public string Type { get; set; }

        public string TypeSuffix { get; set; }
        
        public string ParameterSuffix { get; set; }

        public string Attribute { get; set; }

        public static implicit operator TypePrinterResult(string type)
        {
            return new TypePrinterResult { Type = type };
        }

        public string MergeResult()
        {
            string result = "";
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

        public override string ToString() => $"{Type}{TypeSuffix}";
    }
}