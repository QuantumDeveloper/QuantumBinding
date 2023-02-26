namespace QuantumBinding.Generator.Processors
{
    public class SequentialRegexRenamePass : RegexRenamePass
    {
        private readonly RegexRenameRunItem[] regexRenameItems;

        public SequentialRegexRenamePass(params RegexRenameRunItem[] items)
        {
            regexRenameItems = items;
        }

        public override void OnInitialize()
        {
            RunAgain = false;
            if (RunIndex > regexRenameItems.Length - 1) return;
            var item = regexRenameItems[RunIndex];
            InitializeLocalValues(item.Pattern, item.ReplaceWith, item.RenameTargets, item.IgnoreCase);
        }

        public override void OnComplete()
        {
            if (RunIndex < regexRenameItems.Length)
            {
                RunAgain = true;
                CleanVisitedDeclarations();
            }
        }
    }
}