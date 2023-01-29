
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxEntityInfo : QBDisposableObject
{
    private MarshaledString _name;

    private MarshaledString _uSR;

    private NativeStructArray<QuantumBinding.Clang.Interop.CXIdxAttrInfo> _attributes;

    public QBIdxEntityInfo()
    {
    }

    public QBIdxEntityInfo(QuantumBinding.Clang.Interop.CXIdxEntityInfo _internal)
    {
        Kind = _internal.kind;
        TemplateKind = _internal.templateKind;
        Lang = _internal.lang;
        Name = new string(_internal.name);
        USR = new string(_internal.USR);
        Cursor = new QBCursor(_internal.cursor);
        Attributes = new QBIdxAttrInfo[_internal.numAttributes];
        var nativeTmpArray0 = NativeUtils.PointerToManagedArray(_internal.attributes, _internal.numAttributes);
        for (int i = 0; i < nativeTmpArray0.Length; ++i)
        {
            Attributes[i] = new QBIdxAttrInfo(nativeTmpArray0[i]);
        }
        NativeUtils.Free(_internal.attributes);
        NumAttributes = _internal.numAttributes;
    }

    public CXIdxEntityKind Kind { get; set; }
    public CXIdxEntityCXXTemplateKind TemplateKind { get; set; }
    public CXIdxEntityLanguage Lang { get; set; }
    public string Name { get; set; }
    public string USR { get; set; }
    public QBCursor Cursor { get; set; }
    public QBIdxAttrInfo[] Attributes { get; set; }
    public uint NumAttributes { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxEntityInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxEntityInfo();
        _internal.kind = Kind;
        _internal.templateKind = TemplateKind;
        _internal.lang = Lang;
        _name.Dispose();
        if (Name != null)
        {
            _name = new MarshaledString(Name, false);
            _internal.name = (sbyte*)_name;
        }
        _uSR.Dispose();
        if (USR != null)
        {
            _uSR = new MarshaledString(USR, false);
            _internal.USR = (sbyte*)_uSR;
        }
        if (Cursor != null)
        {
            _internal.cursor = Cursor.ToNative();
        }
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
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _name.Dispose();
        _uSR.Dispose();
        _attributes.Dispose();
    }


    public static implicit operator QBIdxEntityInfo(QuantumBinding.Clang.Interop.CXIdxEntityInfo q)
    {
        return new QBIdxEntityInfo(q);
    }

}



