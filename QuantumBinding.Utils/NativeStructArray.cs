namespace QuantumBinding.Utils;

// https://stackoverflow.com/questions/42754123/conditional-per-targetframework-in-msbuild-15/42754213#42754213
public unsafe struct NativeStructArray<T> where T : unmanaged
{
    public readonly T* Handle;
    private bool isDisposed;

    public NativeStructArray(T[] arr)
    {
        if (arr == null)
        {
            Handle = null;
        }
        else
        {
            var size = (nuint)arr.Length * (nuint)sizeof(T);
            Handle = NativeUtils.GetPointerToManagedArray<T>(arr.Length);
            fixed (T* t = arr)
            {
                System.Buffer.MemoryCopy(t, Handle, (int)size, (int)size);
            }
        }

        isDisposed = false;
    }

    public void Dispose()
    {
        if (!isDisposed)
        {
            NativeUtils.Free(Handle);
            isDisposed = true;
        }
    }
}
