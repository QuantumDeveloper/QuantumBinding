using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface IFunctionParameter
    {
        ITreatFunctionParameterByName WithParameterName(string paramName);

        void WithReturnType(BindingType returnType);
    }

    public interface ICommonFunctionParameter
    {
        ITreatFunctionParameter WithParameterType(string typeName);
    }

    public interface IFunctionParameterType
    {
        IFunctionParameterType SetConst(bool value);

        IFunctionParameterType SetNullable(bool value);

        IFunctionParameterType SetDelegateNullable(bool value);

        IFunctionParameterType SetDefaultValue(string value);

        IFunctionParameterType SetParameterKind(ParameterKind parameterKind);

        IFunctionParameterType NextParameter(string paramType);
    }

    public interface ITreatFunctionParameter
    {
        IFunctionParameterType TreatAsIs();

        IFunctionParameterType TreatAsBuiltinType();

        IFunctionParameterType TreatAsPointerType();

        IFunctionParameterType TreatAsPointerToArray(ArraySizeType sizeType, long size = 0);

        IFunctionParameterType TreatAsArray(ArraySizeType sizeType, long size = 0);

        IFunctionParameterType TreatAsCustomType();
    }

    public interface IFunctionParameterName
    {
        IFunctionParameterName SetParameterKind(ParameterKind parameterKind);

        IFunctionParameterName SetDefaultValue(string value);

        //IFunctionParameterName SetNullable(bool value);

        ITreatFunctionParameterByName WithParameterName(string paramName);
    }

    public interface ITreatFunctionParameterByName
    {
        IFunctionParameterName TreatAsPointerToArray(BindingType elementType, bool isNullable = true, string arraySizeSource = "");

        IFunctionParameterName TreatAsArray(BindingType elementType, ArraySizeType sizeType, int size = 0);

        IFunctionParameterName TreatAsPointerType(BindingType pointeeType);

        IFunctionParameterName TreatAsIs();
    }
}