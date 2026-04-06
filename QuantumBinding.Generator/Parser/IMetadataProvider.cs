using System;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Parser;

public interface IMetadataProvider : IDisposable
{
    ParseResult Parse(TranslationUnit unit, string source);
}