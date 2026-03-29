using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class TextGenerator: ITextGenerator
    {
        public static string NullPointer => "null";
        
        public static string StackAlloc => "stackalloc";
        
        public static string StackAllocThresholdPropertyName => "QuantumBinding.Utils.MarshalingUtils.StackAllocThreshold";
        
        public static string MarshalContextUtilsStructToPointer => "QuantumBinding.Utils.MarshalContextUtils.MarshalStructToPointer";
        public static string MarshalContextUtilsStructToNative => "QuantumBinding.Utils.MarshalContextUtils.MarshalStructToNative";
        public static string MarshalContextUtilsStructToDoublePointer => "QuantumBinding.Utils.MarshalContextUtils.MarshalStructToPointerArray";
        public static string MarshalContextUtilsWrapperArrayToPointer => "QuantumBinding.Utils.MarshalContextUtils.MarshalArrayOfWrappers";
        public static string MarshalContextUtilsArrayOfHandleWrappers => "QuantumBinding.Utils.MarshalContextUtils.MarshalArrayOfHandleWrappers";
        public static string MarshalContextUtilsBlittableArray => "QuantumBinding.Utils.MarshalContextUtils.MarshalBlittableArray";
        public static string MarshalContextUtilsUnmarshalBlittableArray => "QuantumBinding.Utils.MarshalContextUtils.UnmarshalBlittableArray";
        public static string MarshalContextUtilsAllocatePointerForArray => "QuantumBinding.Utils.MarshalContextUtils.AllocatePointerArray";
        public static string MarshalContextCalculateSizeForStringArray => "QuantumBinding.Utils.MarshalContextUtils.CalculateRequiredSizeForStringArray";
        public static string MarshalContextStringToPointer => "QuantumBinding.Utils.MarshalContextUtils.MarshalString";
        public static string MarshalContextStringArrayToDoublePointer => "QuantumBinding.Utils.MarshalContextUtils.MarshalStringArray";
        
        public static string SpanClassName => "System.Span";
        public static string ReadonlySpanClassName => "System.ReadOnlySpan";
        public static string UnsafeClassName => "System.Runtime.CompilerServices.Unsafe";
        public static string MemoryMarshalClassName => "System.Runtime.InteropServices.MemoryMarshal";
        
        public static string MarshalingUtilsClassName => "QuantumBinding.Utils.MarshallingUtils";
        public static string MarshalFixedArrayToPointer => "QuantumBinding.Utils.MarshalingUtils.MarshalFixedArrayToPointer";
        public static string MarshalArrayOfWrappersToFixedBuffer => "QuantumBinding.Utils.MarshalingUtils.MarshalArrayOfWrappersToFixedBuffer";
        public static string MarshalArrayOfHandleWrappersToFixedBuffer => "QuantumBinding.Utils.MarshalingUtils.MarshalArrayOfHandleWrappersToFixedBuffer";
        
        public static string MarshalStringToFixedUtf8Buffer => "QuantumBinding.Utils.MarshalingUtils.MarshalStringToFixedUtf8Buffer";
        public static string MarshalStringArrayToUtf8Buffer => "QuantumBinding.Utils.MarshalingUtils.MarshalStringArrayToUtf8Pointer";
        public static string MarshalArrayToPointer => "QuantumBinding.Utils.MarshalingUtils.MarshalArrayToPointer";
        public static string MarshalBlittableArrayToPointer => "QuantumBinding.Utils.MarshalingUtils.MarshalBlittableArrayToPointer";
        public static string MarshalPointerToStringArray => "QuantumBinding.Utils.MarshalingUtils.MarshalPointerToStringArray";
        public static string MarshalFromPointerToArray => "QuantumBinding.Utils.MarshalingUtils.MarshalFromPointerToArray";
        public static string MarshalFixedByteArrayToString => "QuantumBinding.Utils.MarshalingUtils.MarshalFixedByteArrayToString";
        public static string MarshalFixedCharArrayToString => "QuantumBinding.Utils.MarshalingUtils.MarshalFixedCharArrayToString";
        public static string MarshalStructToPointer => "QuantumBinding.Utils.MarshalingUtils.MarshalStructToPointer";
        public static string MarshalFromPointerToArrayOfStructs => "QuantumBinding.Utils.MarshalingUtils.MarshalFromPointerToArrayOfStructs";
        
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

        public static string GetPointerString(uint pointerDepth)
        {
            var pointers = string.Empty;
            for (int i = 0; i < pointerDepth; i++)
            {
                pointers += "*";
            }

            return pointers;
        }
    }
}