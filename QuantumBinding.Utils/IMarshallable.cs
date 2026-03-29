using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

public interface IMarshallable<TNative> where TNative : unmanaged
{
    int GetSize();
    void MarshalTo(ref MarshallingContext<TNative> context);
    void MarshalFrom(in TNative native);
}

public interface IUnmanagedWrapper<TNative> where TNative : unmanaged
{
    TNative GetNativeValue();
}

#if NET9_0_OR_GREATER
public interface IMarshallableObject
{
    int GetSize();
    unsafe void* GetNativePointer<TContext>(ref TContext context) where TContext : IMarshallingContext, allows ref struct;
}
#endif

public interface IMarshallingContext
{
    Span<byte> GetDataCursor();
    
    void SetDataCursor(Span<byte> cursor);
    
    Span<T> AllocateNative<T>(int count) where T : unmanaged;
    
    Span<byte> AllocateData(int size);
}

public ref struct MarshallingContext<TNative>: IMarshallingContext where TNative : unmanaged
{
    public Span<TNative> Destination;

    public Span<byte> DataCursor;

    public MarshallingContext(Span<TNative> destination, Span<byte> dataCursor)
    {
        Destination = destination;
        DataCursor = dataCursor;
    }

    public Span<byte> GetDataCursor()
    {
        return DataCursor;
    }

    public void SetDataCursor(Span<byte> cursor)
    {
        DataCursor = cursor;
    }

    public Span<T> AllocateNative<T>(int count) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>() * count;
        var rawSlice = AllocateData(size);
        return MemoryMarshal.Cast<byte, T>(rawSlice);
    }

    public Span<byte> AllocateData(int size)
    {
        var slice = DataCursor.Slice(0, size);
        DataCursor = DataCursor.Slice(size);
        return slice;
    }
}

public static unsafe class MarshallingContext
{
    public static MarshallingContext<TNative> Create<TNative>(Span<byte> mainBuffer)
        where TNative : unmanaged
    {
        var primaryStructSize = sizeof(TNative);

        var destinationSpan = MemoryMarshal.Cast<byte, TNative>(
            mainBuffer.Slice(0, primaryStructSize));

        var dataCursorSpan = mainBuffer.Slice(primaryStructSize);

        return new MarshallingContext<TNative>(destinationSpan, dataCursorSpan);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNative MarshalToNative<TNative>(this IMarshallable<TNative> managed, NativeContext context)
        where TNative : unmanaged
    {
        var marshallingContext = Create<TNative>(context.Data);
        
        managed.MarshalTo(ref marshallingContext);
        
        return marshallingContext.Destination[0];
    }
    
    public static NativeContext MarshalArray<T, TNative>(ReadOnlySpan<T> source, out TNative* pNativeArray)
        where T : class, IMarshallable<TNative>
        where TNative : unmanaged
    {
        int totalSize = 0;
        foreach (var item in source) totalSize += item.GetSize();

        var ctx = new NativeContext(totalSize, Span<byte>.Empty);
    
        Span<byte> cursor = ctx.Data;
        pNativeArray = MarshalContextUtils.MarshalArrayOfWrappers<T, TNative>(source, ref cursor);
        return ctx;
    }
}

public unsafe ref struct NativeContext
{
    private byte[] _rentedArray;
    public Span<byte> Data { get; }

    public NativeContext(int size, Span<byte> stackBuffer = default)
    {
        if (size <= stackBuffer.Length)
        {
            _rentedArray = null;
            Data = stackBuffer.Slice(0, size);
        }
        else
        {
            _rentedArray = ArrayPool<byte>.Shared.Rent(size);
            Data = _rentedArray.AsSpan(0, size);
        }
    }

    public void Dispose()
    {
        if (_rentedArray != null)
        {
            ArrayPool<byte>.Shared.Return(_rentedArray);
            _rentedArray = null;
        }
    }
}