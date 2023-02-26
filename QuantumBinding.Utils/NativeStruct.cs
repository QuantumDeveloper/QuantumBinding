namespace QuantumBinding.Utils;

public unsafe struct NativeStruct<T> where T : unmanaged
{
    public T* Handle;
    private bool isDisposed;

    public NativeStruct(T nativeStruct)
    {
        Handle = NativeUtils.StructOrEnumToPointer(nativeStruct);
        isDisposed = false;
    }

    public void Dispose()
    {
        if (!isDisposed && Handle != null)
        {
            NativeUtils.Free(Handle);
            Handle = null;
            isDisposed = true;
        }
    }
}
