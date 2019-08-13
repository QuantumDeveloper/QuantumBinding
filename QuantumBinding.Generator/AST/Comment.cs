namespace QuantumBinding.Generator.AST
{
    public class Comment : Declaration
    {
        public RawCommentKind Kind { get; set; }

        public string Text { get; set; }

        public string BriefText { get; set; }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitComment(this);
        }

        public override object Clone()
        {
            return new Comment()
            {
                Kind = Kind,
                Text = Text,
                BriefText = BriefText
            };
        }
    }
}