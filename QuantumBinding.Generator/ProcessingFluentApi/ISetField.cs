using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface ISetField
    {
        ISetField WithField(string fieldName);
        ISetField SetType(BindingType type);
        ISetField ChangeType();
        ISetField AddAttribute(string attribute);
        ISetField RemoveExistingAttributes();
        ISetField InterpretAsPointerToArray(BindingType elementType, bool isNullable = true, string arraySizeSource = "", uint pointerDepth = 1);
        ISetField InterpretAsPointerToPrimitiveType(PrimitiveType primitiveType, uint pointerDepth = 1);
        ISetField InterpretAsArray(BindingType elementType, ArraySizeType sizeType, int size = 0);
        ISetField InterpretAsPointerType(BindingType pointeeType);
        ISetField InterpretAsIs();
        
        ISetField InterpretAs(BindingType type);
        ISetField InterpretAsCustomType(string typeName);
        ISetField InterpretAsDelegateType(IEnumerable<Parameter> parameters, string name);
    }
}