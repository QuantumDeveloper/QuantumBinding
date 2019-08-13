using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface ISetField
    {
        ISetField WithField(string fieldName);
        ISetField SetType(BindingType type);
        ISetField AddAttribute(string attribute);
        ISetField RemoveExistingAttributes();
        ISetField TreatAsPointerToArray(BindingType elementType, bool isNullable = true, string arraySizeSource = "");
        ISetField TreatAsArray(BindingType elementType, ArraySizeType sizeType, int size = 0);
        ISetField TreatAsPointerType(BindingType pointeeType);
        ISetField TreatAsIs();
    }
}