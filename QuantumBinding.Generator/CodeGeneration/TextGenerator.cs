using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class TextGenerator: ITextGenerator
    {
        public TextGenerator()
        {
            indentationStack = new Stack<uint>();
            stringBuilder = new StringBuilder();
        }

        public TextGenerator(TextGenerator copy)
        {
            stringBuilder = new StringBuilder(copy);
            indentationStack = new Stack<uint>(copy.indentationStack);
        }

        public const uint DefaultIndentation = 4;

        public Stack<uint> indentationStack;

        private readonly StringBuilder stringBuilder;
        protected bool StartNewLine;
        protected bool TrimLines;

        public bool HasText => stringBuilder.Length > 0;

        public uint Indent => (uint)indentationStack.Sum(x=>x);

        public void Clear()
        {
            stringBuilder.Clear();
        }

        public TextGenerator Clone()
        {
            return new TextGenerator(this);
        }

        public bool Contains(string text, StringComparison stringComparison)
        {
            return stringBuilder.ToString().Contains(text, stringComparison);
        }

        public void Write(string text)
        {
            var lines = text.Split(Environment.NewLine);
            if (string.IsNullOrEmpty(lines.Last()))
            {
                lines = lines[..^1];
            }
            int lineIndex = 0;
            foreach (var line in lines)
            {
                var currentLine = line;
                if (TrimLines)
                {
                    currentLine = line.TrimStart();
                }

                if (StartNewLine && !string.IsNullOrEmpty(currentLine))
                {
                    stringBuilder.Append(' ', (int)Indent);
                }

                stringBuilder.Append(currentLine);

                if (lineIndex < lines.Length - 1)
                {
                    NewLine();
                }
                else
                {
                    StartNewLine = false;
                }

                lineIndex++;
            }

            TrimLines = false;
        }

        public void WriteLine(string text)
        {
            StartNewLine = true;
            Write(text);
            NewLine();
        }

        public void WriteLineWithTrim(string text)
        {
            TrimLines = true;
            WriteLine(text);
        }

        public void WriteLineWithIndent(string text)
        {
            WriteLine(text);
            PushIndent();
        }

        public void WriteOpenBraceAndIndent()
        {
            WriteLine("{");
            PushIndent();
        }

        public void UnindentAndWriteCloseBrace()
        {
            PopIndent();
            WriteLine("}");
        }

        public void NewLine()
        {
            stringBuilder.Append(Environment.NewLine);
            StartNewLine = true;
        }

        public void PushIndent(uint indentation = DefaultIndentation)
        {
            indentationStack.Push(indentation);
        }

        public void PopIndent()
        {
            indentationStack.Pop();
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }

        public static implicit operator string(TextGenerator text)
        {
            return text.ToString();
        }
    }
}