using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace QuantumBinding.Utils;

public static unsafe class MarshalingUtils
{
    public static uint StackAllocThreshold { get; set; } = 4096;

    #region Managed -> Native

    public static TNative* MarshalArrayToPointer<TManaged, TNative, TParent>(
        ReadOnlyMemory<TManaged> source,
        ref MarshallingContext<TParent> context)
        where TManaged : IMarshallable<TNative> // Managed type
        where TNative : unmanaged // Unmanaged type (target type for array)
        where TParent : unmanaged // Unmanaged type (for context)
    {
        if (source.IsEmpty)
        {
            return null;
        }

        int count = source.Length;

        var sizeInBytes = sizeof(TNative) * count;
        var byteSpan = context.AllocateData(sizeInBytes);

        var destinationArraySpan = MemoryMarshal.Cast<byte, TNative>(byteSpan);

        var pDestinationArray = (TNative*)Unsafe.AsPointer(
            ref MemoryMarshal.GetReference(destinationArraySpan));

        var sourceSpan = source.Span;

        for (int i = 0; i < count; ++i)
        {
            var itemDestinationSpan = destinationArraySpan.Slice(i, 1);

            var itemContext = new MarshallingContext<TNative>(
                itemDestinationSpan,
                context.DataCursor
            );

            sourceSpan[i].MarshalTo(ref itemContext);

            context.DataCursor = itemContext.DataCursor;
        }

        return pDestinationArray;
    }

    public static TManaged* MarshalBlittableArrayToPointer<TManaged, TParent>(
        ReadOnlySpan<TManaged> source,
        ref MarshallingContext<TParent> context)
        where TManaged : unmanaged
        where TParent : unmanaged
    {
        if (source.IsEmpty)
        {
            return null;
        }

        var sourceByteSpan = MemoryMarshal.AsBytes(source);

        var destinationByteSpan = context.AllocateData(sourceByteSpan.Length);

        sourceByteSpan.CopyTo(destinationByteSpan);

        return (TManaged*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destinationByteSpan));
    }

    public static void MarshalFixedArrayToPointer<T>(
        ReadOnlySpan<T> source,
        T* destinationPtr,
        int destinationCapacity)
        where T : unmanaged
    {
        var destSpan = new Span<T>(destinationPtr, destinationCapacity);

        int lengthToCopy = Math.Min(source.Length, destSpan.Length);

        if (source.IsEmpty)
        {
            return;
        }

        var spanToCopy = source.Slice(0, lengthToCopy);

        spanToCopy.CopyTo(destSpan);

        if (lengthToCopy < destSpan.Length)
        {
            destSpan.Slice(lengthToCopy).Clear();
        }
    }

    public static void MarshalArrayOfWrappersToFixedBuffer<TManaged, TNative>(
        ReadOnlySpan<TManaged> source,
        TNative* destinationPtr,
        int destinationCapacity,
        ref Span<byte> dataCursor)
        where TManaged : class, IMarshallable<TNative>
        where TNative : unmanaged
    {
        var destSpan = new Span<TNative>(destinationPtr, destinationCapacity);

        int lengthToCopy = Math.Min(source.Length, destSpan.Length);

        if (source.IsEmpty)
        {
            destSpan.Clear();
            return;
        }

        for (int i = 0; i < lengthToCopy; i++)
        {
            TManaged managedItem = source[i];

            ref TNative nativeSlot = ref destSpan[i];

            if (managedItem == null)
            {
                nativeSlot = default;
            }
            else
            {
#if NET6_0_OR_GREATER
                var itemDestSpan = new Span<TNative>(ref nativeSlot);
#else
                var itemDestSpan = new Span<TNative>(Unsafe.AsPointer(ref nativeSlot), 1);
#endif

                var itemContext = new MarshallingContext<TNative>(itemDestSpan, dataCursor);
                managedItem.MarshalTo(ref itemContext);
                dataCursor = itemContext.DataCursor;
            }
        }

        if (lengthToCopy < destSpan.Length)
        {
            destSpan.Slice(lengthToCopy).Clear();
        }
    }

    public static void MarshalArrayOfHandleWrappersToFixedBuffer<TManaged, TNative>(
        ReadOnlySpan<TManaged> source,
        TNative* destinationPtr,
        int destinationCapacity)
        where TManaged : class, IUnmanagedWrapper<TNative>
        where TNative : unmanaged
    {
        var destSpan = new Span<TNative>(destinationPtr, destinationCapacity);

        int lengthToCopy = Math.Min(source.Length, destSpan.Length);

        if (source.IsEmpty)
        {
            destSpan.Clear();
            return;
        }

        for (int i = 0; i < lengthToCopy; i++)
        {
            TManaged managedItem = source[i];

            if (managedItem == null)
            {
                destSpan[i] = default;
            }
            else
            {
                destSpan[i] = managedItem.GetNativeValue();
            }
        }

        if (lengthToCopy < destSpan.Length)
        {
            destSpan.Slice(lengthToCopy).Clear();
        }
    }

    public static void MarshalStringToFixedUtf8Buffer(string source, Span<byte> destination)
    {
        if (destination.Length == 0)
        {
            return;
        }

        if (string.IsNullOrEmpty(source))
        {
            destination.Clear();
            return;
        }

#if NET6_0_OR_GREATER

        Span<byte> bufferForChars = destination.Slice(0, destination.Length - 1);

        int bytesUsed = Encoding.UTF8.GetBytes(source.AsSpan(), bufferForChars);

        destination[bytesUsed] = (byte)'\0';

        if (bytesUsed < destination.Length - 1)
        {
            destination.Slice(bytesUsed + 1).Clear();
        }
#else
        int maxByteCount = destination.Length - 1;
        
        if (maxByteCount <= 0) 
        {
            destination[0] = (byte)'\0';
            return;
        }

        int bytesUsed;
        
        fixed (char* pSource = source)
        fixed (byte* pDest = destination)
        {
            bytesUsed = Encoding.UTF8.GetBytes(
                pSource,        // char*
                source.Length,  // int charCount
                pDest,          // byte*
                maxByteCount    // int byteCount
            );
        }
        
        destination[bytesUsed] = (byte)'\0';
        
        if (bytesUsed < destination.Length - 1)
        {
            destination.Slice(bytesUsed + 1).Clear();
        }
#endif
    }

    public static sbyte** MarshalStringArrayToUtf8Pointer<TParent>(
        ReadOnlySpan<string> source,
        ref MarshallingContext<TParent> context)
        where TParent : unmanaged
    {
        if (source.IsEmpty)
        {
            return null;
        }

        int count = source.Length;

        var pointerArrayByteSpan = context.AllocateData(count * sizeof(nint));
        var pointerArraySpan = MemoryMarshal.Cast<byte, nint>(pointerArrayByteSpan);

        for (int i = 0; i < count; i++)
        {
            string str = source[i];

            if (str is null)
            {
                pointerArraySpan[i] = 0;
                continue;
            }

            int byteCount = Encoding.UTF8.GetByteCount(str);

            var stringByteSpan = context.AllocateData(byteCount + 1);
            int bytesWritten;

#if NET6_0_OR_GREATER

            bytesWritten = Encoding.UTF8.GetBytes(str.AsSpan(), stringByteSpan);

#else
            fixed (char* pSource = str)
            {
                byte* pDest = (byte*)Unsafe.AsPointer(
                    ref MemoryMarshal.GetReference(stringByteSpan));

                bytesWritten = Encoding.UTF8.GetBytes(
                    pSource,
                    str.Length,
                    pDest,
                    byteCount
                );
            }
#endif

            stringByteSpan[bytesWritten] = (byte)'\0';
            var pString = (sbyte*)Unsafe.AsPointer(
                ref MemoryMarshal.GetReference(stringByteSpan));
            pointerArraySpan[i] = (nint)pString;
        }

        return (sbyte**)Unsafe.AsPointer(
            ref MemoryMarshal.GetReference(pointerArrayByteSpan));
    }

    public static TManaged* MarshalStructToPointer<TManaged, TParent>(
        TManaged source,
        ref MarshallingContext<TParent> context)
        where TManaged : unmanaged
        where TParent : unmanaged
    {
        var byteSpan = context.AllocateData(sizeof(TManaged));

        var structSpan = MemoryMarshal.Cast<byte, TManaged>(byteSpan);

        structSpan[0] = source;

        var pValue = (TManaged*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(structSpan));

        return pValue;
    }

    #endregion

    #region Native -> Managed

    public static string[] MarshalPointerToStringArray(char** sourcePtr, uint count)
    {
        if (sourcePtr == null) return null;

        var array = new string[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = new string(sourcePtr[i]);
        }

        return array;
    }

    public static string[] MarshalPointerToStringArray(sbyte** sourcePtr, uint count)
    {
        if (sourcePtr == null) return null;

        var array = new string[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = new string(sourcePtr[i]);
        }

        return array;
    }

    public static void MarshalFromPointerToArray<T>(
        T* sourcePtr,
        ulong sourceCapacity,
        Span<T> destination)
        where T : unmanaged
    {
        if (sourcePtr == null || sourceCapacity == 0)
        {
            return;
        }

        var sourceSpan = new ReadOnlySpan<T>(sourcePtr, (int)sourceCapacity);

        int toCopy = Math.Min(sourceSpan.Length, destination.Length);
        sourceSpan.Slice(0, toCopy).CopyTo(destination);
    }

    public static TManaged[] MarshalFromPointerToArrayOfStructs<TManaged, TNative>(
        TNative* sourcePtr,
        uint sourceCount,
        int sourceCapacity)
        where TManaged : IMarshallable<TNative>, new()
        where TNative : unmanaged
    {
        if (sourcePtr == null || sourceCount == 0)
        {
            return [];
        }

        int count = (int)Math.Min((uint)sourceCapacity, sourceCount);
        if (count == 0)
        {
            return [];
        }

        var sourceSpan = new ReadOnlySpan<TNative>(sourcePtr, count);

        var destinationArray = new TManaged[count];

        for (int i = 0; i < count; i++)
        {
            ref readonly TNative nativeItem = ref sourceSpan[i];

            var managedItem = new TManaged();

            managedItem.MarshalFrom(in nativeItem);

            destinationArray[i] = managedItem;
        }

        return destinationArray;
    }

    public static string MarshalFixedByteArrayToString(sbyte* pSource, int maxCount)
    {
        if (pSource == null)
        {
            return null;
        }

        int length = 0;
        while (length < maxCount && pSource[length] != 0)
        {
            length++;
        }

        if (length == 0)
        {
            return string.Empty;
        }

        return new string(pSource, 0, length, Encoding.UTF8);
    }

    public static string MarshalFixedCharArrayToString(char* pSource, int maxCount)
    {
        if (pSource == null)
        {
            return null;
        }

        int length = 0;
        while (length < maxCount && pSource[length] != '\0')
        {
            length++;
        }

        if (length == 0)
        {
            return string.Empty;
        }

        return new string(pSource, 0, length);
    }

    public static int GetTotalSize<T, TNative>(ReadOnlySpan<T> items) where T : IMarshallable<TNative> where TNative : unmanaged
    {
        int size = 0;
        for (int i = 0; i < items.Length; i++) size += items[i].GetSize();
        return size;
    }

    #endregion
}