#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public unsafe struct MarshaledString
{
    private readonly bool isInitialized;
    private readonly void* pointer;
    private readonly bool isUnicode;
    private GCHandle handle;

    public MarshaledString(string str, bool isUnicode)
    {
        this.isUnicode = isUnicode;

        str += '\0';
        handle = GCHandle.Alloc(str, GCHandleType.Pinned);
        pointer = (void*)handle.AddrOfPinnedObject();
        
        isInitialized = true;
    }

    public bool IsUnicode => isUnicode;

    public void* Pointer => pointer;
    
    public string GetString()
    {
        return isUnicode ? new string((char*)pointer) : new string((sbyte*)pointer);
    }

    public void Dispose()
    {
        if (isInitialized)
        {
            handle.Free();
        }
    }
    
    public static implicit operator sbyte*(in MarshaledString value) => (sbyte*)value.pointer;
    
    public static implicit operator char*(in MarshaledString value) => (char*)value.pointer;
}
#endif