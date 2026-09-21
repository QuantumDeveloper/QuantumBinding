using System.Runtime.InteropServices;

namespace QuantumBinding.Utils;

/// <summary>
/// The marshalled size of <typeparamref name="T"/>, worked out once per process instead of on every call.
/// <para>The value is exactly what <see cref="Marshal.SizeOf{T}()"/> returns - this does not compute a size a different
/// way, it stops computing the same one again. A struct's size is a property of its TYPE: it is the same whether one
/// field was filled or all of them, so nothing here can go stale. Generic statics are per closed type, so two structs
/// never share a slot.</para>
/// <para>Deliberately NOT Unsafe.SizeOf: that reports the MANAGED layout, which differs for a type carrying a bool, a
/// char, a fixed buffer or a [MarshalAs]. Keeping Marshal's answer means this is safe for every binding the generator
/// serves, whatever its structs look like - the caller cannot get a different number than before.</para>
/// </summary>
public static class SizeOfCache<T>
{
    public static readonly int Size = Marshal.SizeOf<T>();
}
