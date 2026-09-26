# QuantumBinding

QuantumBinding generates C# bindings from C headers. It parses the headers with libclang and writes the
interop layer: structs, enums, delegates and function imports. It can also write managed wrapper classes over the
native structs. You drive it from a small console program, so you regenerate the bindings with `dotnet run`
whenever the headers change.

QuantumBinding generates its own libclang bindings (`QuantumBinding.Clang`). It also generates the Vulkan, Slang and
SPIR-V bindings of [AdamantiumVulkan](https://github.com/QuantumDeveloper/AdamantiumVulkan).

## Packages

| Package | What it is | Reference it from |
|---|---|---|
| `QuantumBinding.Generator` | The generator. | Your generator program. |
| `QuantumBinding.Clang` | C# bindings to the libclang C API. The generator parses headers through them. | Nowhere: it comes with the generator. |
| `QuantumBinding.Utils` | Runtime helpers the generated code calls: marshaling contexts and native memory. Targets `net10.0` and `netstandard2.0`. | The project that holds the generated bindings. |

## Quick start

### 1. A generator program

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>$(NETCoreSdkRuntimeIdentifier)</RuntimeIdentifier>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="QuantumBinding.Generator" Version="3.0.0" />
  </ItemGroup>

</Project>
```

Keep the `RuntimeIdentifier` line. The native libclang comes from the `libclang` runtime packages, one per
platform, and a restore picks the one for your machine only through the runtime identifier. Without the line
there is no libclang next to the generator and it cannot parse anything. Supported: `win-x64`, `win-arm64`,
`linux-x64`, `linux-arm64`, `osx-arm64`.

The package also brings clang's builtin headers (`stddef.h`, `stdint.h`, `stdbool.h` and the rest) and copies them
to the output, so `#include <stdint.h>` resolves without a C toolchain installed.

### 2. Describe the library

```csharp
using QuantumBinding.Generator;
using QuantumBinding.Generator.Processors;
using QuantumBinding.Generator.Utils;

public class MyLibGenerator : QuantumBindingGenerator
{
    private Module module;

    public override void OnSetup(BindingOptions options)
    {
        // The name is the native library the bindings load: mylib.dll, libmylib.so, libmylib.dylib.
        module = Module.Create("mylib");
        module.Name = "MyLib";
        module.Files.Add("native/include/mylib.h");
        module.IncludeDirs.Add("native/include");
        module.OutputPath = "../MyLib/Generated";
        module.OutputFileName = "MyLib";
        module.OutputNamespace = "MyLib";
        module.InteropSubNamespace = "Interop";
        module.InteropClassName = "MyLibInterop";
        module.MethodClassName = "MyLibNative";
        module.EachTypeInSeparateFile = true;

        options.AddModule(module);
    }

    public override void OnSetupPostProcessing(ProcessingContext context)
    {
        // Functions over a handle become its methods, the rest static methods of MyLibNative.
        context.AddPreGeneratorPass(new FunctionToInstanceMethodPass(), ExecutionPassKind.PerTranslationUnit, module);
        // mylib_release(context) reads context.Release().
        context.AddPreGeneratorPass(
            new RegexRenamePass("^mylib_", "", RenameTargets.Method, true),
            ExecutionPassKind.PerTranslationUnit,
            module);
        // Every name but the imported functions' in PascalCase: mylib_context becomes MylibContext.
        context.AddPreGeneratorPass(
            new CaseRenamePass(RenameTargetsUtils.AnyExcept(RenameTargets.Function), CasePattern.PascalCase),
            ExecutionPassKind.PerTranslationUnit,
            module);
    }
}
```

```csharp
new MyLibGenerator().Run();
```

A module needs six names, and the run stops with an `ArgumentNullException` when one is missing: the library (the
argument of `Module.Create`), `OutputFileName`, `OutputNamespace`, `InteropSubNamespace`, `InteropClassName` (the
class of the raw imports) and `MethodClassName` (the static class of the managed methods over the functions that
stay global).

A run writes the bindings to `OutputPath`. It removes the files a previous run wrote that this run no longer
produces. If a header does not parse, the run throws and leaves the previous output in place.

### 3. The bindings project

The project that holds the generated code references `QuantumBinding.Utils` and allows unsafe code:

```xml
<PropertyGroup>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="QuantumBinding.Utils" Version="3.0.1" />
</ItemGroup>
```

It also ships the native library next to the application, as any P/Invoke library does.

## Shaping the output

The setup methods of `QuantumBindingGenerator` run in order:

- `OnSetup` adds the modules.
- `OnBeforeSetupPasses` fixes what the header cannot say.
- `OnSetupPostProcessing` adds the passes that rename and reshape the result.

Useful passes:

- `CaseRenamePass`, `RegexRenamePass`, `SequentialRegexRenamePass`: names in C# style, prefixes stripped or replaced.
- `EnumItemsCleanupPass`, `EnumItemsRenamePass`, `CheckFlagsEnumsPass`: enum items without the enum's prefix,
  `[Flags]` where the values are bits.
- `FunctionToInstanceMethodPass`: a function whose first parameter is a handle becomes a method of that handle's
  class. The functions left global become static methods of the `MethodClassName` class. With a rename pass
  stripping the `mylib_` prefix, `mylib_release(context)` reads `context.Release()` and
  `mylib_create(&settings, &context)` reads `MyLibNative.Create(settings, out context)`.

Some things a header cannot say: that a `void*` is caller data, that a pointer is an array or an out parameter. The
fluent `PostProcessingApi` states them:

```csharp
// using QuantumBinding.Generator.ProcessingFluentApi;
public override void OnBeforeSetupPasses(ProcessingContext context)
{
    var api = new PostProcessingApi();
    api.Function("mylib_watch")
        .WithParameterName("userData")
        .InterpretAsPointerToVoid()
        .SetParameterKind(ParameterKind.In);
    context.AddPreGeneratorPass(new PostProcessingApiPass(api), ExecutionPassKind.PerTranslationUnit);
}
```

`module.TargetRuntime` picks what the generated code may use. `TargetRuntime.Net8Plus`, the default, passes arrays
and strings as spans and memory. `TargetRuntime.NetStandard20` passes them as arrays.

## C++ and COM libraries

QuantumBinding reads C. For a C++ or COM library, write a flat C facade: `extern "C"` functions over opaque
handles, built as a shared library of its own. Then generate from the facade's header. The Slang and Microsoft
GameInput bindings in the Adamantium projects are made this way.

## Building from source

```
dotnet build QuantumBinding.sln
```

`QuantumBinding.Demo`
(`QuantumBinding.ClangGenerator`) is the generator program for the libclang bindings, a complete real setup. It
writes them to `QuantumBinding.ClangSandbox`.

## License

Apache-2.0. Copyright © 2019-2026 Adamantium Studio.
