using System;
using System.Buffers;

namespace QuantumBinding.Utils;

public readonly ref struct NativeArray<T> : IDisposable
{
    private readonly T[] _rentedArray;
    public readonly Span<T> Span { get; }

    public NativeArray(int size)
    {
        
        _rentedArray = ArrayPool<T>.Shared.Rent(size);
        Span = _rentedArray.AsSpan(0, size);
    }

    public void Dispose()
    {
        if (_rentedArray != null)
        {
            ArrayPool<T>.Shared.Return(_rentedArray);
        }
    }
}