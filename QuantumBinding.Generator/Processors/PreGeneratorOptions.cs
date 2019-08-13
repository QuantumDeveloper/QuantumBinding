namespace QuantumBinding.Generator.Processors
{
    public class PreGeneratorOptions
    {
        public bool VisitEnums { get; set; }
        public bool VisitEnumItems { get; set; }

        public bool VisitClasses { get; set; }
        public bool VisitFields { get; set; }
        public bool VisitProperties { get; set; }
        public bool VisitMethods { get; set; }

        public bool VisitFunctions { get; set; }
        public bool VisitDelegates { get; set; }
        public bool VisitParameters { get; set; }

        public bool VisitMacros { get; set; }
    }
}