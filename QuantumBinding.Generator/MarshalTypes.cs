namespace QuantumBinding.Generator;

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
    SkipParamModifiers,
    SkipParamTypesAndModifiers,
    Property,
    WrappedProperty,
    NativeFunctionCall
}