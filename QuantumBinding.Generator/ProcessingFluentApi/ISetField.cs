using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface ISetField
    {
        ISetField WithField(string fieldName);
        ISetField SetType(BindingType type);
        ISetField AddAttribute(string attribute);
        ISetField RemoveExistingAttributes();
        ISetField InterpretAsPointerToArray(BindingType elementType, bool isNullable = true, string arraySizeSource = "");
        ISetField InterpretAsArray(BindingType elementType, ArraySizeType sizeType, int size = 0);
        ISetField InterpretAsPointerType(BindingType pointeeType);
        ISetField InterpretAsIs();
    }
}