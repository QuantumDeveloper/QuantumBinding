namespace QuantumBinding.Generator.Processors
{
    public class RegexRenameRunItem
    {
        public RegexRenameRunItem(string pattern, string replaceWith, RenameTargets renameTargets, bool ignoreCase)
        {
            Pattern = pattern;
            ReplaceWith = replaceWith;
            RenameTargets = renameTargets;
            IgnoreCase = ignoreCase;
        }

        public string Pattern { get; }
        public string ReplaceWith { get; }
        public RenameTargets RenameTargets { get; }
        public bool IgnoreCase { get; }
    }
}