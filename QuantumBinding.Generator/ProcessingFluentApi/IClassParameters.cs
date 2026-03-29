using System;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface IClassParameters
    {
        ISetField WithField(string fieldName);

        ISetField AddField(string fieldName);

        ISetProperty AddProperty(string propertyName);

        ISetField SetClassType(ClassType classType);

        IClassParameters SetUnderlyingType(BindingType type);

        IClassParameters Ignore();

        IClassParameters CleanObject();

        IClassParameters CopyFieldsFromLinkedObject();
        
        IClassParameters UpdateLinkedClass(string linkedClassName);
        
        IClassParameters AddOverloadOperator(
            OperatorKind operatorKind, 
            TransformationKind transformationKind, 
            string fieldName,
            bool passValueToConstructor);

        IClassParameters AddDefaultConstructor();

        IClassParameters AddConstructorWithParameters<T>(string paramName, ParameterKind parameterKind,
            BindingType type) where T : Declaration;
    }
}