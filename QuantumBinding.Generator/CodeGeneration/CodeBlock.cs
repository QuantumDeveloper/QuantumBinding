using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.CodeGeneration
{
    [DebuggerDisplay("{BlockKind} | {CodeObject}")]
    public class CodeBlock : ITextGenerator
    {
        public CodeBlock()
        {
            BlockKind = CodeBlockKind.Undefined;
            Blocks = new List<CodeBlock>();
            Text = new TextGenerator();
        }

        public CodeBlock(CodeBlockKind blockKind) : this()
        {
            BlockKind = blockKind;
        }

        private bool indentChanged;
        private bool isSubBlock;

        public TextGenerator Text { get; private set; }

        public CodeBlockKind BlockKind { get; set; }

        public object CodeObject { get; set; }
        
        public CodeBlock Parent { get; set; }

        public List<CodeBlock> Blocks { get; }

        public NewLineStrategy NewLineStrategy { get; set; }

        public void AddBlock(CodeBlock block)
        {
            if (Text.HasText || indentChanged)
            {
                indentChanged = false;
                var subBlock = new CodeBlock(){ Text = Text.Clone(), isSubBlock = true};
                Text.Clear();

                AddBlock(subBlock);
            }

            block.Parent = this;
            Blocks.Add(block);
        }

        public IEnumerable<CodeBlock> FindBlocks(CodeBlockKind blockKind)
        {
            foreach (var block in Blocks)
            {
                if (block.BlockKind == blockKind)
                {
                    yield return block;
                }

                foreach (var codeBlock in block.Blocks)
                {
                    if (codeBlock.BlockKind == blockKind)
                    {
                        yield return codeBlock;
                    }
                }
            }
        }

        public string Generate()
        {
            if (Blocks.Count == 0)
            {
                return Text.ToString();
            }

            var sb = new StringBuilder();

            var totalIndent = Text.Indent;

            foreach (var block in Blocks)
            {
                var text = block.Generate();
                if (block.NewLineStrategy == NewLineStrategy.OnNewLine || (block.NewLineStrategy == NewLineStrategy.IfNotEmpty && Text.HasText))
                {
                    sb.AppendLine();
                }

                if (block.isSubBlock)
                {
                    totalIndent = 0;
                }

                var lines = text.Split(Environment.NewLine);

                for (var index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    var isLast = index == lines.Length - 1;

                    if (!string.IsNullOrEmpty(line))
                    {
                        sb.Append(' ', (int) totalIndent);
                    }

                    sb.Append(line);

                    if (isLast)
                    {
                        if (!line.EndsWith(Environment.NewLine) && (block.NewLineStrategy != NewLineStrategy.NoShift && block.NewLineStrategy != NewLineStrategy.SpaceBeforeNextBlock))
                        {
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        if (!line.EndsWith(Environment.NewLine))
                        {
                            sb.AppendLine();
                        }
                    }
                }

                if (block.NewLineStrategy == NewLineStrategy.SpaceBeforeNextBlock)
                {
                    sb.Append(' ');
                }

                totalIndent += block.Text.Indent;

            }

            if (Text.HasText)
            {
                sb.Append(Text);
            }

            return sb.ToString();
        }

        public uint Indent { get; }

        public void Write(string text)
        {
            Text.Write(text);
        }

        public void WriteLine(string text)
        {
            Text.WriteLine(text);
        }

        public void WriteLineWithTrim(string text)
        {
            Text.WriteLineWithTrim(text);
        }

        public void WriteLineWithIndent(string text)
        {
            Text.WriteLineWithIndent(text);
        }

        public void WriteOpenBraceAndIndent()
        {
            Text.WriteOpenBraceAndIndent();
        }

        public void UnindentAndWriteCloseBrace()
        {
            Text.UnindentAndWriteCloseBrace();
        }

        public void NewLine()
        {
            Text.NewLine();
        }

        public void PushIndent(uint indentation = TextGenerator.DefaultIndentation)
        {
            indentChanged = true;
            Text.PushIndent(indentation);
        }

        public void PopIndent()
        {
            indentChanged = true;
            Text.PopIndent();
        }
    }
}