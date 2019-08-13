using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.CodeGeneration
{
    public abstract class CodeBlockGenerator : ITextGenerator
    {
        public CodeBlock RootBlock { get; set; }
        public CodeBlock ActiveBlock { get; set; }

        protected CodeBlockGenerator()
        {
            RootBlock = new CodeBlock();
            ActiveBlock = RootBlock;
        }

        public void PushBlock(CodeBlock block)
        {
            ActiveBlock.AddBlock(block);
        }

        public void PushBlock(CodeBlockKind blockKind, object codeObject = null)
        {
            var block = new CodeBlock(blockKind);
            block.CodeObject = codeObject;
            ActiveBlock.AddBlock(block);
            ActiveBlock = block;
        }

        public CodeBlock PopBlock(NewLineStrategy newLineStrategy = NewLineStrategy.NoShift)
        {
            var block = ActiveBlock;
            block.NewLineStrategy = newLineStrategy;
            ActiveBlock = block.Parent;
            return block;
        }

        public IEnumerable<CodeBlock> FindBlocks(CodeBlockKind blockKind)
        {
            return RootBlock.FindBlocks(blockKind);
        }

        public CodeBlock FindBlock(CodeBlockKind blockKind)
        {
            return RootBlock.FindBlocks(blockKind).FirstOrDefault();
        }

        public virtual string Generate()
        {
            return RootBlock.Generate();
        }

        public uint Indent => ActiveBlock.Indent;

        public void Write(string text)
        {
            ActiveBlock.Write(text);
        }

        public void WriteLine(string text)
        {
            ActiveBlock.WriteLine(text);
        }

        public void WriteLineWithIndent(string text)
        {
            ActiveBlock.WriteLineWithIndent(text);
        }

        public void WriteOpenBraceAndIndent()
        {
            ActiveBlock.WriteOpenBraceAndIndent();
        }

        public void UnindentAndWriteCloseBrace()
        {
            ActiveBlock.UnindentAndWriteCloseBrace();
        }

        public void NewLine()
        {
            ActiveBlock.NewLine();
        }

        public void PushIndent(uint indentation = TextGenerator.DefaultIndentation)
        {
            ActiveBlock.PushIndent(indentation);
        }

        public void PopIndent()
        {
            ActiveBlock.PopIndent();
        }

        public void WriteLineWithTrim(string text)
        {
            ActiveBlock.WriteLineWithTrim(text);
        }
    }
}