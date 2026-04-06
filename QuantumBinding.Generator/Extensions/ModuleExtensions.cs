using QuantumBinding.Clang;
using QuantumBinding.Generator.Parser.Clang;

namespace QuantumBinding.Generator.Extensions;

public static class ModuleExtensions
{
    public static Module WithDefaultParser(this Module module)
    {
        var clangIndex = ClangNative.CreateIndex(0, 0);
        module.MetadataProvider = new ClangParser(clangIndex);
        return module;
    }
}