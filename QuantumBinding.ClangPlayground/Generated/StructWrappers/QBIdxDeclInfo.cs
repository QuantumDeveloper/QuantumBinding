
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxDeclInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _entityInfo;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo> _semanticContainer;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo> _lexicalContainer;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo> _declAsContainer;

    private NativeStructArray<QuantumBinding.Clang.Interop.CXIdxAttrInfo> _attributes;

    public QBIdxDeclInfo()
    {
    }

    public QBIdxDeclInfo(QuantumBinding.Clang.Interop.CXIdxDeclInfo _internal)
    {
        EntityInfo = new QBIdxEntityInfo(*_internal.entityInfo);
        NativeUtils.Free(_internal.entityInfo);
        Cursor = new QBCursor(_internal.cursor);
        Loc = new QBIdxLoc(_internal.loc);
        SemanticContainer = new QBIdxContainerInfo(*_internal.semanticContainer);
        NativeUtils.Free(_internal.semanticContainer);
        LexicalContainer = new QBIdxContainerInfo(*_internal.lexicalContainer);
        NativeUtils.Free(_internal.lexicalContainer);
        IsRedeclaration = _internal.isRedeclaration;
        IsDefinition = _internal.isDefinition;
        IsContainer = _internal.isContainer;
        DeclAsContainer = new QBIdxContainerInfo(*_internal.declAsContainer);
        NativeUtils.Free(_internal.declAsContainer);
        IsImplicit = _internal.isImplicit;
        Attributes = new QBIdxAttrInfo[_internal.numAttributes];
        var nativeTmpArray0 = NativeUtils.PointerToManagedArray(_internal.attributes, _internal.numAttributes);
        for (int i = 0; i < nativeTmpArray0.Length; ++i)
        {
            Attributes[i] = new QBIdxAttrInfo(nativeTmpArray0[i]);
        }
        NativeUtils.Free(_internal.attributes);
        NumAttributes = _internal.numAttributes;
        Flags = _internal.flags;
    }

    public QBIdxEntityInfo EntityInfo { get; set; }
    public QBCursor Cursor { get; set; }
    public QBIdxLoc Loc { get; set; }
    public QBIdxContainerInfo SemanticContainer { get; set; }
    public QBIdxContainerInfo LexicalContainer { get; set; }
    public int IsRedeclaration { get; set; }
    public int IsDefinition { get; set; }
    public int IsContainer { get; set; }
    public QBIdxContainerInfo DeclAsContainer { get; set; }
    public int IsImplicit { get; set; }
    public QBIdxAttrInfo[] Attributes { get; set; }
    public uint NumAttributes { get; set; }
    public uint Flags { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxDeclInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxDeclInfo();
        _entityInfo.Dispose();
        if (EntityInfo != null)
        {
            var struct0 = EntityInfo.ToNative();
            _entityInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct0);
            _internal.entityInfo = _entityInfo.Handle;
        }
        if (Cursor != null)
        {
            _internal.cursor = Cursor.ToNative();
        }
        if (Loc != null)
        {
            _internal.loc = Loc.ToNative();
        }
        _semanticContainer.Dispose();
        if (SemanticContainer != null)
        {
            var struct1 = SemanticContainer.ToNative();
            _semanticContainer = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo>(struct1);
            _internal.semanticContainer = _semanticContainer.Handle;
        }
        _lexicalContainer.Dispose();
        if (LexicalContainer != null)
        {
            var struct2 = LexicalContainer.ToNative();
            _lexicalContainer = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo>(struct2);
            _internal.lexicalContainer = _lexicalContainer.Handle;
        }
        _internal.isRedeclaration = IsRedeclaration;
        _internal.isDefinition = IsDefinition;
        _internal.isContainer = IsContainer;
        _declAsContainer.Dispose();
        if (DeclAsContainer != null)
        {
            var struct3 = DeclAsContainer.ToNative();
            _declAsContainer = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo>(struct3);
            _internal.declAsContainer = _declAsContainer.Handle;
        }
        _internal.isImplicit = IsImplicit;
        _attributes.Dispose();
        if (Attributes != null)
        {
            var tmpArray0 = new QuantumBinding.Clang.Interop.CXIdxAttrInfo[Attributes.Length];
            for (int i = 0; i < Attributes.Length; ++i)
            {
                tmpArray0[i] = Attributes[i].ToNative();
            }
            _attributes = new NativeStructArray<QuantumBinding.Clang.Interop.CXIdxAttrInfo>(tmpArray0);
            _internal.attributes = _attributes.Handle;
        }
        _internal.numAttributes = NumAttributes;
        _internal.flags = Flags;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _entityInfo.Dispose();
        _semanticContainer.Dispose();
        _lexicalContainer.Dispose();
        _declAsContainer.Dispose();
        _attributes.Dispose();
    }


    public static implicit operator QBIdxDeclInfo(QuantumBinding.Clang.Interop.CXIdxDeclInfo q)
    {
        return new QBIdxDeclInfo(q);
    }

}



