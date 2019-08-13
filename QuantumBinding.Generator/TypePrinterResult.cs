namespace QuantumBinding.Generator
{
    public class TypePrinterResult
    {
        public string Type { get; set; }

        public string Suffix { get; set; }

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

            if (!string.IsNullOrEmpty(Suffix))
            {
                result += $"{Suffix}";
            }

            return result;
        }

        public override string ToString() => Type;
    }
}