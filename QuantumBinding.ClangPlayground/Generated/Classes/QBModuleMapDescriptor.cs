
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// Object encapsulating information about a module.map file.
///</summary>
public unsafe partial class QBModuleMapDescriptor
{
    internal CXModuleMapDescriptorImpl __Instance;
    public QBModuleMapDescriptor()
    {
    }

    public QBModuleMapDescriptor(QuantumBinding.Clang.Interop.CXModuleMapDescriptorImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Dispose a CXModuleMapDescriptor object.
    ///</summary>
    public void ModuleMapDescriptor_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_ModuleMapDescriptor_dispose(this);
    }

    ///<summary>
    /// Sets the framework module name that the module.map describes.
    ///</summary>
    public CXErrorCode ModuleMapDescriptor_setFrameworkModuleName(string name)
    {
        var arg1 = (sbyte*)NativeUtils.PointerToString(name, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_ModuleMapDescriptor_setFrameworkModuleName(this, arg1);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Sets the umbrella header name that the module.map describes.
    ///</summary>
    public CXErrorCode ModuleMapDescriptor_setUmbrellaHeader(string name)
    {
        var arg1 = (sbyte*)NativeUtils.PointerToString(name, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_ModuleMapDescriptor_setUmbrellaHeader(this, arg1);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Write out the CXModuleMapDescriptor object to a char buffer.
    ///</summary>
    public CXErrorCode ModuleMapDescriptor_writeToBuffer(uint options, out sbyte* out_buffer_ptr, out uint out_buffer_size)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ModuleMapDescriptor_writeToBuffer(this, options, out out_buffer_ptr, out out_buffer_size);
    }

    public ref readonly CXModuleMapDescriptorImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXModuleMapDescriptorImpl(QBModuleMapDescriptor q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXModuleMapDescriptorImpl();
    }

    public static implicit operator QBModuleMapDescriptor(QuantumBinding.Clang.Interop.CXModuleMapDescriptorImpl q)
    {
        return new QBModuleMapDescriptor(q);
    }

}



