using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace QuantumBinding.Utils;

public static unsafe class MarshalContextUtils
{
    public static TNative* MarshalStructToPointer<TManaged, TNative>(
        TManaged source,
        ref Span<byte> dataCursor)
        where TManaged : class, IMarshallable<TNative>
        where TNative : unmanaged
    {
        if (source == null)
            return null;
        
        int primarySize = sizeof(TNative);
        Span<byte> destinationByteSpan = dataCursor.Slice(0, primarySize);

        // Same reason as in MarshalArrayOfWrappers: a marshaller writes a field only when it differs from the default,
        // so the destination must start out zeroed. It is not, whenever the buffer came from ArrayPool.Rent.
        destinationByteSpan.Clear();

        Span<TNative> destinationSpan = MemoryMarshal.Cast<byte, TNative>(destinationByteSpan);

        Span<byte> remainingBuffer = dataCursor.Slice(primarySize);

        var localContext = new MarshallingContext<TNative>(
            destinationSpan,
            remainingBuffer
        );

        source.MarshalTo(ref localContext);

        int totalBytesUsed = remainingBuffer.Length - localContext.DataCursor.Length + primarySize;

        dataCursor = dataCursor.Slice(totalBytesUsed);

        return (TNative*)Unsafe.AsPointer(ref destinationSpan.GetPinnableReference());
    }
    
    public static TNative MarshalStructToNative<TManaged, TNative>(
        TManaged source,
        ref Span<byte> dataCursor)
        where TManaged : class, IMarshallable<TNative>
        where TNative : unmanaged
    {
        if (source == null)
            return default;
        
        TNative* pNative = MarshalStructToPointer<TManaged, TNative>(source, ref dataCursor);
        
        return *pNative;
    }
    
    public static TBlittable* MarshalBlittableArray<TBlittable>(
        ReadOnlySpan<TBlittable> source,
        ref Span<byte> dataCursor)
        where TBlittable : unmanaged
    {
        if (source.IsEmpty)
        {
            return null;
        }

        int sizeOfElement = Unsafe.SizeOf<TBlittable>();
        int sizeInBytes = source.Length * sizeOfElement;

        Span<byte> destinationByteSpan = dataCursor.Slice(0, sizeInBytes);

        ReadOnlySpan<byte> sourceByteSpan = MemoryMarshal.AsBytes(source);

        sourceByteSpan.CopyTo(destinationByteSpan);

        dataCursor = dataCursor.Slice(sizeInBytes);

        return (TBlittable*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destinationByteSpan));
    }
    
    public static TBlittable[] UnmarshalBlittableArray<TBlittable>(TBlittable* source, long count)
        where TBlittable : unmanaged
    {
        if (source == null || count <= 0)
            return Array.Empty<TBlittable>();

        TBlittable[] destination = new TBlittable[count];
        fixed (TBlittable* pDest = destination)
        {
            Unsafe.CopyBlock(pDest, source, (uint)(count * sizeof(TBlittable)));
        }
        return destination;
    }

    /// <summary>
    /// Copies <paramref name="count"/> native elements into a caller-owned <see cref="Span{T}"/> (clamped to the span's
    /// length). Used for two-call "query then fill" APIs whose managed parameter is a <c>Span&lt;T&gt;</c>: the span is
    /// passed by value, so reassigning it (as <see cref="UnmarshalBlittableArray{TBlittable}"/> did) never reaches the
    /// caller's buffer - the data must be copied INTO the span.
    /// </summary>
    public static void CopyNativeToSpan<TBlittable>(TBlittable* source, long count, Span<TBlittable> destination)
        where TBlittable : unmanaged
    {
        if (source == null || count <= 0 || destination.IsEmpty)
            return;

        var copyCount = (int)Math.Min(count, destination.Length);
        new ReadOnlySpan<TBlittable>(source, copyCount).CopyTo(destination);
    }
    
    public static TNative* AllocatePointerArray<TNative>(
        int count,
        ref Span<byte> dataCursor)
        where TNative : unmanaged
    {
        if (count <= 0)
        {
            return null;
        }

        int sizeOfElement = Unsafe.SizeOf<TNative>();
        int sizeInBytes = count * sizeOfElement;

        Span<byte> allocatedByteSpan = dataCursor.Slice(0, sizeInBytes);

        dataCursor = dataCursor.Slice(sizeInBytes);

        return (TNative*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(allocatedByteSpan));
    }

    public static TNative* MarshalArrayOfWrappers<TManaged, TNative>(
        ReadOnlySpan<TManaged> source,
        ref Span<byte> dataCursor)
        where TManaged : class, IMarshallable<TNative>
        where TNative : unmanaged
    {
        if (source.IsEmpty)
        {
            return null;
        }

        int count = source.Length;
        int sizeOfNativeStruct = sizeof(TNative);

        int sizeOfNativeArray = sizeOfNativeStruct * count;

        Span<byte> nativeArrayByteSpan = dataCursor.Slice(0, sizeOfNativeArray);

        // The generated marshallers SKIP writing a field whose value is the default - they rely on the destination
        // being zeroed. That holds for a small struct (the caller stack-allocates, and stackalloc is zeroed) but NOT for
        // an array: it comes from ArrayPool.Rent, which hands back whatever was in it. Every default-valued field then
        // kept a previous tenant's bytes - e.g. a VkBufferImageCopy with bufferOffset 0 reached the driver carrying a
        // stale POINTER as its offset, which Vulkan validation reported as a copy of ~10^14 bytes and the device did not
        // survive. Zero the slots so "skip the default" means what it says.
        nativeArrayByteSpan.Clear();

        TNative* pNativeArray = (TNative*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(nativeArrayByteSpan));

        Span<byte> remainingBuffer = dataCursor.Slice(sizeOfNativeArray);

        // A plain copy, NOT `ref remainingBuffer`: aliasing the two made the "how much did the nested data use" figure
        // below subtract a value from itself, so it was always zero and nested payloads were handed out twice.
        Span<byte> nestedDataCursor = remainingBuffer;

        for (int i = 0; i < count; i++)
        {
            TManaged managedItem = source[i];

            ref TNative nativeSlot = ref pNativeArray[i];

            if (managedItem == null)
            {
                nativeSlot = default;
            }
            else
            {
                Span<TNative> itemDestinationSpan;
#if NET6_0_OR_GREATER
                itemDestinationSpan = new Span<TNative>(ref nativeSlot);
#else
                itemDestinationSpan = new Span<TNative>(Unsafe.AsPointer(ref nativeSlot), 1);
#endif

                var localContext = new MarshallingContext<TNative>(
                    itemDestinationSpan,
                    nestedDataCursor
                );

                managedItem.MarshalTo(ref localContext);

                nestedDataCursor = localContext.DataCursor;
            }
        }

        int bytesUsedForNestedData = remainingBuffer.Length - nestedDataCursor.Length;
        int totalBytesConsumed = sizeOfNativeArray + bytesUsedForNestedData;

        dataCursor = dataCursor.Slice(totalBytesConsumed);

        return pNativeArray;
    }

    public static void MarshalArrayOfHandleWrappers<TManaged, TNative>(
        ReadOnlySpan<TManaged> source,
        Span<TNative> destination)
        where TManaged : class, IUnmanagedWrapper<TNative>
        where TNative : unmanaged
    {
        if (source.Length != destination.Length)
        {
            throw new ArgumentException("Source Span length must match Destination Span length.", nameof(destination));
        }
    
        int count = source.Length;

        for (int i = 0; i < count; i++)
        {
            TManaged wrapper = source[i];

            destination[i] = wrapper?.GetNativeValue() ?? default;
        }
    }

    public static TNative** MarshalStructToPointerArray<TManaged, TNative>(
        TManaged source,
        ref Span<byte> dataCursor)
        where TManaged : class, IMarshallable<TNative>
        where TNative : unmanaged
    {
        if (source == null)
        {
            return null;
        }

        int sizeOfPointerArray = sizeof(nint);

        Span<byte> pointerArrayByteSpan = dataCursor.Slice(0, sizeOfPointerArray);

        Span<nint> pointerArraySpan;
        TNative** ppNativeStructArray;

#if NET6_0_OR_GREATER
        pointerArraySpan = MemoryMarshal.Cast<byte, nint>(pointerArrayByteSpan);
        
        ref nint firstElementRef = ref MemoryMarshal.GetReference(pointerArraySpan);
        ppNativeStructArray = (TNative**)Unsafe.AsPointer(ref firstElementRef);
#else
        fixed (byte* pByte = &MemoryMarshal.GetReference(pointerArrayByteSpan))
        {
            nint* pNativeNintArray = (nint*)pByte;
            pointerArraySpan = new Span<nint>(pNativeNintArray, 1);

            ppNativeStructArray = (TNative**)pByte;
        }
#endif
        Span<byte> currentDataCursor = dataCursor.Slice(sizeOfPointerArray);

        TNative* pNativeStruct = MarshalStructToPointer<TManaged, TNative>(
            source,
            ref currentDataCursor
        );

        pointerArraySpan[0] = (nint)pNativeStruct;

        int totalBytesConsumed = dataCursor.Length - currentDataCursor.Length;
        dataCursor = dataCursor.Slice(totalBytesConsumed);

        return ppNativeStructArray;
    }

    public static sbyte** MarshalStringArrayToUtf8Pointer(
        ReadOnlySpan<string> source,
        Span<byte> pointerArrayBuffer,
        Span<byte> stringsDataBuffer)
    {
        if (source.IsEmpty)
        {
            return null;
        }

        int count = source.Length;
        int currentOffset = 0;

        var pointerArraySpan = MemoryMarshal.Cast<byte, nint>(pointerArrayBuffer);

        if (pointerArraySpan.Length < count)
            throw new ArgumentException("Pointer array buffer size is insufficient.");

        for (int i = 0; i < count; i++)
        {
            string str = source[i];

            if (str is null)
            {
                pointerArraySpan[i] = 0;
                continue;
            }

            int byteCount = System.Text.Encoding.UTF8.GetByteCount(str);

            if (currentOffset + byteCount + 1 > stringsDataBuffer.Length)
                throw new ArgumentException("Strings data buffer size is insufficient.");

            var currentStringByteSpan = stringsDataBuffer.Slice(currentOffset, byteCount + 1);
            int bytesWritten;

#if NET6_0_OR_GREATER
        bytesWritten = System.Text.Encoding.UTF8.GetBytes(str.AsSpan(), currentStringByteSpan);
#else
            fixed (char* pSource = str)
            {
                byte* pDest = (byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(
                    ref System.Runtime.InteropServices.MemoryMarshal.GetReference(currentStringByteSpan));

                bytesWritten = System.Text.Encoding.UTF8.GetBytes(
                    pSource,
                    str.Length,
                    pDest,
                    byteCount
                );
            }
#endif

            currentStringByteSpan[bytesWritten] = (byte)'\0';

            var pString = (sbyte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(currentStringByteSpan));
            pointerArraySpan[i] = (nint)pString;

            currentOffset += bytesWritten + 1;
        }


        return (sbyte**)Unsafe.AsPointer(ref MemoryMarshal.GetReference(pointerArrayBuffer));
    }
    
    public static sbyte** MarshalStringArray(
        ReadOnlySpan<string> source,
        ref Span<byte> dataCursor)
    {
        if (source.IsEmpty)
        {
            return null;
        }
    
        int count = source.Length;
        int pointerArraySize = count * sizeof(nint);
    
        int totalRequiredSize = CalculateRequiredSizeForStringArray(source);
        Span<byte> overallDestination = dataCursor.Slice(0, totalRequiredSize);
        Span<byte> pointerArrayBuffer = overallDestination.Slice(0, pointerArraySize);
        Span<byte> stringsDataBuffer = overallDestination.Slice(pointerArraySize);
        sbyte** ppUtf8Strings = MarshalStringArrayToUtf8Pointer(
            source, 
            pointerArrayBuffer, 
            stringsDataBuffer);

        dataCursor = dataCursor.Slice(totalRequiredSize);

        return ppUtf8Strings;
    }
    
    public static sbyte* MarshalString(
        string source,
        ref Span<byte> dataCursor)
    {
        if (string.IsNullOrEmpty(source))
        {
            return null; 
        }

        int byteCount = Encoding.UTF8.GetByteCount(source);
        int totalBytesToConsume = byteCount + 1;

        Span<byte> destinationByteSpan = dataCursor.Slice(0, totalBytesToConsume);
    
        int bytesWritten;

#if NET6_0_OR_GREATER
        bytesWritten = Encoding.UTF8.GetBytes(source.AsSpan(), destinationByteSpan);
#else
    fixed (char* pSource = source)
    {
        byte* pDest = (byte*)Unsafe.AsPointer(
            ref MemoryMarshal.GetReference(destinationByteSpan));

        bytesWritten = Encoding.UTF8.GetBytes(
            pSource,
            source.Length,
            pDest,
            byteCount
        );
    }
#endif
        destinationByteSpan[bytesWritten] = (byte)'\0';
        
        sbyte* pString = (sbyte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destinationByteSpan));
        
        dataCursor = dataCursor.Slice(totalBytesToConsume);

        return pString;
    }
    
    public static int CalculateRequiredSizeForStringArray(ReadOnlySpan<string> source)
    {
        if (source.IsEmpty) return 0;

        int pointerArraySize = source.Length * sizeof(nint);

        int stringsDataSize = 0;
        foreach (string str in source)
        {
            if (str is null) continue;
            
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(str);
            
            stringsDataSize += byteCount + 1;
        }

        return pointerArraySize + stringsDataSize;
    }
    
    /// <summary>
    /// For future use in case if we will need memory alignment
    /// </summary>
    /// <param name="cursor"></param>
    /// <param name="alignment"></param>
    public static void AlignCursor(ref Span<byte> cursor, int alignment = 8)
    {
        fixed (byte* ptr = cursor)
        {
            var address = (nuint)ptr;
            var remainder = address % (nuint)alignment;
            if (remainder != 0)
            {
                var padding = (int)((nuint)alignment - remainder);
                if (padding < cursor.Length)
                {
                    cursor = cursor.Slice(padding);
                }
            }
        }
    }
}