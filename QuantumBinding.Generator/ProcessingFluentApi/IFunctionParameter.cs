using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface IFunctionParameter
    {
        IInterpretFunctionParameterByName WithParameterName(string paramName);

        IInterpretFunctionParameterByName RenameTo(string newName);

        void WithReturnType(BindingType returnType);
    }

    public interface ICommonFunctionParameter
    {
        IInterpretFunctionParameter WithParameterType(string typeName);
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

    public interface IInterpretFunctionParameter
    {
        IFunctionParameterType InterpretAsIs();

        IFunctionParameterType InterpretAsBuiltinType();

        IFunctionParameterType InterpretAsPointerType();

        IFunctionParameterType InterpretAsPointerToArray(ArraySizeType sizeType, long size = 0);

        IFunctionParameterType InterpretAsArray(ArraySizeType sizeType, long size = 0);

        IFunctionParameterType InterpretAsCustomType();
    }

    public interface IFunctionParameterName
    {
        IFunctionParameterName SetParameterKind(ParameterKind parameterKind);

        IFunctionParameterName SetDefaultValue(string value);

        //IFunctionParameterName SetNullable(bool value);

        IInterpretFunctionParameterByName WithParameterName(string paramName);
    }

    public interface IInterpretFunctionParameterByName
    {
        IFunctionParameterName InterpretAsPointerToArray(BindingType elementType, bool isNullable = true, string arraySizeSource = "");

        IFunctionParameterName InterpretAsArray(BindingType elementType, ArraySizeType sizeType, int size = 0);

        IFunctionParameterName InterpretAsPointerType(BindingType pointeeType);

        IFunctionParameterName InterpretAsIs();
    }
}