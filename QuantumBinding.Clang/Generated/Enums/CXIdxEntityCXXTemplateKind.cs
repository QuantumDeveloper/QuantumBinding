
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Extra C++ template information for an entity. This can apply to: CXIdxEntity_Function CXIdxEntity_CXXClass CXIdxEntity_CXXStaticMethod CXIdxEntity_CXXInstanceMethod CXIdxEntity_CXXConstructor CXIdxEntity_CXXConversionFunction CXIdxEntity_CXXTypeAlias
///</summary>
public enum CXIdxEntityCXXTemplateKind : uint
{
    CXIdxEntity_NonTemplate = 0,

    CXIdxEntity_Template = 1,

    CXIdxEntity_TemplatePartialSpecialization = 2,

    CXIdxEntity_TemplateSpecialization = 3,

}



