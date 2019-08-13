using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface ISetProperty
    {
        ISetProperty SetType(BindingType type);
        ISetProperty SetField(string fieldName);
        ISetProperty SetSetter(Method setter);
        ISetProperty SetGetter(Method getter);
        ISetProperty IsAutoProperty(bool value);
    }
}