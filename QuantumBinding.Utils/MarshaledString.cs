using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public unsafe struct MarshaledString
{
    private bool isDisposed;
    private readonly bool isUnicode;

    public readonly void* Handle;

    public MarshaledString(string str, bool isUnicode)
    {
        this.isUnicode = isUnicode;

        Handle = NativeUtils.PointerToString(str, this.isUnicode);
        
        isDisposed = false;
    }

    public bool IsUnicode => isUnicode;

    public string GetString()
    {
        return isUnicode ? new string((char*)Handle) : new string((sbyte*)Handle);
    }

    public void Dispose()
    {
        if (!isDisposed && Handle != null)
        {
            NativeUtils.Free(Handle);
            isDisposed = true;
        }
    }
    
    public static implicit operator sbyte*(in MarshaledString value) => (sbyte*)value.Handle;
    
    public static implicit operator char*(in MarshaledString value) => (char*)value.Handle;
}