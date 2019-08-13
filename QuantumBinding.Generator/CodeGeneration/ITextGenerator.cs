namespace QuantumBinding.Generator.CodeGeneration
{
    public interface ITextGenerator
    {
        uint Indent { get; }

        void Write(string text);

        void WriteLine(string text);

        void WriteLineWithTrim(string text);

        void WriteLineWithIndent(string text);

        void WriteOpenBraceAndIndent();

        void UnindentAndWriteCloseBrace();

        void NewLine();

        void PushIndent(uint indentation = TextGenerator.DefaultIndentation);

        void PopIndent();
    }
}