namespace QuantumBinding.Generator
{
    public enum MarshalTypes
    {
        Undefined,
        NativeField,
        MethodParameter,
        NativeParameter,
        NativeReturnType,
        DelegateType,
        DelegateParameter,
        SkipParamTypes,
        SkipParamTypesAndModifiers,
        Property,
        WrappedProperty,
        NativeFunctionCall
    }
}