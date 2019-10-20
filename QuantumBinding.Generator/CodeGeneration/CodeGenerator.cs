﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.CodeGeneration
{
    public abstract class CodeGenerator : CodeBlockGenerator
    {
        // Extracted from:
        // https://github.com/mono/mono/blob/master/mcs/class/System/Microsoft.CSharp/CSharpCodeGenerator.cs
        public static readonly string[] ReservedWords =
        {
            "abstract", "event", "new", "struct", "as", "explicit", "null", "switch",
            "base", "extern", "this", "false", "operator", "throw", "break", "finally",
            "out", "true", "fixed", "override", "try", "case", "params", "typeof",
            "catch", "for", "private", "foreach", "protected", "checked", "goto",
            "public", "unchecked", "class", "if", "readonly", "unsafe", "const",
            "implicit", "ref", "continue", "in", "return", "using", "virtual", "default",
            "interface", "sealed", "volatile", "delegate", "internal", "do", "is",
            "sizeof", "while", "lock", "stackalloc", "else", "static", "enum",
            "namespace", "object", "bool", "byte", "float", "uint", "char", "ulong",
            "ushort", "decimal", "int", "sbyte", "short", "double", "long", "string",
            "void", "partial", "yield", "where"
        };

        protected CodeGenerator(ProcessingContext context, TranslationUnit unit, GeneratorSpecializations specializations) : 
            this(context, new List<TranslationUnit>() { unit }, specializations)
        {
        }

        protected CodeGenerator(ProcessingContext context, IEnumerable<TranslationUnit> units, GeneratorSpecializations specializations)
        {
            Context = context;
            TranslationUnits = units.ToList();
            Specializations = specializations;
        }

        public bool IsGeneratorEmpty { get; protected set; }

        public GeneratorSpecializations Specializations { get; }

        public string GeneratorName => "QuantumBindingGenerator";

        public ProcessingContext Context { get; }

        public BindingOptions Options => Context.Options;

        public ASTContext AstContext => Context.AstContext;

        public List<TranslationUnit> TranslationUnits { get; }

        public virtual bool IsInteropGenerator
        {
            get 
            {
                if (Specializations.HasFlag(GeneratorSpecializations.Structs) ||
                    Specializations.HasFlag(GeneratorSpecializations.Unions) ||
                    Specializations.HasFlag(GeneratorSpecializations.Functions) ||
                    Specializations.HasFlag(GeneratorSpecializations.Delegates))
                {
                    return true;
                }

                return false;
            }
        }

        public abstract void Run();

        protected void GenerateFileHeader()
        {
            PushBlock(CodeBlockKind.Header);
            GenerateComment(CommentKind.CSharpShort);
            NewLine();
            PopBlock();
        }

        private void GenerateComment(CommentKind commentKind)
        {
            var comment = new List<string>
            {
                "----------------------------------------------------------------------------------------------",
                "<auto-generated>",
                $"This file was autogenerated by {GeneratorName}.",
                "Do not edit this file manually, because you will lose all your changes after next generation.",
                "</auto-generated>",
                "----------------------------------------------------------------------------------------------"
            };

            PushBlock(CodeBlockKind.AutoGenerated);
            GenerateMultilineComment(comment, commentKind);
            PopBlock();
        }

        protected void GenerateSummary(string summary)
        {
            var lines = summary.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            WriteLine("///<summary>");
            foreach (var line in lines)
            {
                WriteLine($"{GetMultilineCommentStart(CommentKind.CSharp)} {line}");
            }
            WriteLine("///</summary>");
        }

        public virtual void GenerateCommentIfNotEmpty(Comment comment)
        {
            if (comment != null && comment.Text != null)
            {
                PushBlock(CodeBlockKind.Comment);
                GenerateSummary(comment.BriefText);
                PopBlock();
            }
        }

        private void GenerateMultilineComment(List<string> comment, CommentKind commentKind)
        {
            var commentStart = GetCommentLineStart(commentKind);
            if (!string.IsNullOrEmpty(commentStart))
            {
                Write(commentStart);
            }

            var multilineComment = GetMultilineCommentStart(commentKind);
            foreach (var line in comment)
            {
                WriteLine($"{multilineComment} {line}");
            }

            var commentEnd = GetCommentLineEnd(commentKind);
            if (!string.IsNullOrEmpty(commentEnd))
            {
                WriteLine(commentEnd);
            }
        }

        private string GetCommentLineStart(CommentKind kind)
        {
            switch (kind)
            {
                case CommentKind.CSharp:
                case CommentKind.CSharpShort:
                    return "";
                default:
                    return "/*";
            }
        }

        private string GetMultilineCommentStart(CommentKind kind)
        {
            switch (kind)
            {
                case CommentKind.CSharp:
                    return "///";
                case CommentKind.CSharpShort:
                    return "//";
                default:
                    return "";
            }
        }

        private string GetCommentLineEnd(CommentKind kind)
        {
            switch (kind)
            {
                case CommentKind.CSharp:
                case CommentKind.CSharpShort:
                    return string.Empty;
                default:
                    return "*/";
            }
        }

        protected virtual void GenerateMacro(Macro macro) {}

        protected virtual void GenerateEnumItems(Enumeration @enum) {}

        protected virtual void GenerateClass(Class @class) {}

        protected virtual void GenerateDelegate(Delegate @delegate) { }

        protected virtual void GenerateFunction(Function function) {}
    }
}