using System;

namespace QuantumBinding.Clang;

public class QBDisposableObject: IDisposable
{
    public bool IsDisposed { get; private set; }

    protected virtual void Dispose(bool disposeManaged)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposeManaged)
        {
            ManagedDisposeOverride();
        }

        UnmanagedDisposeOverride();

        IsDisposed = true;
    }

    protected virtual void ManagedDisposeOverride() { }

    protected virtual void UnmanagedDisposeOverride() { }

    ~QBDisposableObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}