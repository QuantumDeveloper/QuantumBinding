using System;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface IClassParameters
    {
        ISetField WithField(string fieldName);

        ISetField AddField(string fieldName);

        ISetProperty AddProperty(string propertyName);

        ISetField SetClassType(ClassType classType);

        void SetUnderlyingType(BindingType type);

        IClassParameters Ignore();

        IClassParameters CleanObject();

        IClassParameters CopyFieldsFromLinkedObject();
    }
}