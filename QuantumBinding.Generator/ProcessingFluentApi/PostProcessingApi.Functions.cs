using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public partial class PostProcessingApi : IFunctionParameter, IFunctionParameterName, IInterpretFunctionParameterByName, ICommonFunctionParameter, IFunctionParameterType, IInterpretFunctionParameter
    {
        public PostProcessingApi()
        {
            functions = new Dictionary<string, FunctionExtension>();
            delegates = new Dictionary<string, DelegateExtension>();
            classes = new Dictionary<string, ClassExtension>();
            commonParameters = new List<ParameterExtension>();
        }

        private readonly Dictionary<string, FunctionExtension> functions;
        
        private FunctionExtension _currentFunction;
        private List<FunctionExtension> _currentFunctions;

        private ParameterExtension _currentParameter;
        
        private List<ParameterExtension> commonParameters;
        private ParameterExtension _currentCommonParameter;
        private string _currentCommonParameterTypeName;

        public IFunctionParameter Function(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                throw new ArgumentNullException(nameof(functionName));
            }

            if (!functions.TryGetValue(functionName, out _currentFunction))
            {
                var function = new FunctionExtension();
                function.DecoratedName = functionName;
                function.EntryPointName = functionName;
                _currentFunction = function;
                functions.Add(functionName, function);
            }

            lastOperation = LastOperation.AddFunction;
            return this;
        }

        public IFunctionParameter Functions(params string[] functionNames)
        {
            if (functionNames == null)
            {
                throw new ArgumentNullException(nameof(functionNames));
            }

            _currentFunctions = new List<FunctionExtension>();
            foreach (var functionName in functionNames)
            {
                if (!functions.TryGetValue(functionName, out _currentFunction))
                {
                    var function = new FunctionExtension();
                    function.EntryPointName = functionName;
                    function.DecoratedName = functionName;
                    _currentFunctions.Add(function);
                    functions.Add(functionName, function);
                }
            }

            lastOperation = LastOperation.AddFunctions;
            return this;
        }

        public ICommonFunctionParameter AllFunctions()
        {
            return this;
        }

        IInterpretFunctionParameterByName IFunctionParameter.WithParameterName(string paramName)
        {
            CreateParameter(paramName);
            return this;
        }
        
        public IInterpretFunctionParameterByName RenameTo(string newName)
        {
            _currentFunction.DecoratedName = newName;
            return this;
        }

        private void CreateParameter(string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (lastOperation == LastOperation.AddFunction)
            {
                var parameter = _currentFunction.Parameters.FirstOrDefault(x => x.Name == paramName);
                if (parameter != null)
                {
                    throw new ArgumentException(
                        $"Parameter with name {paramName} already added for {_currentFunction.DecoratedName}");
                }

                var param = new ParameterExtension();
                param.Name = paramName;
                _currentFunction.Parameters.Add(param);
                _currentParameter = param;
            }
            else if (lastOperation == LastOperation.AddFunctions)
            {
                var param = new ParameterExtension();
                param.Name = paramName;
                _currentParameter = param;

                foreach (var currentFunction in _currentFunctions)
                {
                    var parameter = currentFunction.Parameters.FirstOrDefault(x => x.Name == paramName);
                    if (parameter != null)
                    {
                        throw new ArgumentException(
                            $"Parameter with name {paramName} already added for {_currentFunction.DecoratedName}");
                    }

                    currentFunction.Parameters.Add(param);
                }
            }
        }

        IFunctionParameterName IFunctionParameterName.SetParameterKind(ParameterKind parameterKind)
        {
            _currentParameter.ParameterKind = parameterKind;
            return this;
        }

        IFunctionParameterName IFunctionParameterName.SetDefaultValue(string value)
        {
            _currentParameter.DefaultValue = value;
            return this;
        }

        IFunctionParameterName IInterpretFunctionParameterByName.InterpretAsPointerToArray(BindingType elementType, bool isNullable, string arraySizeSource)
        {
            var pointer = new PointerType();
            pointer.IsNullable = isNullable;
            var arrayType = new ArrayType();
            arrayType.ArraySizeSource = arraySizeSource;
            arrayType.SizeType = ArraySizeType.Incomplete;
            arrayType.ElementType = elementType;
            pointer.Pointee = arrayType;
            _currentParameter.Type = pointer;

            return this;
        }

        IFunctionParameterName IInterpretFunctionParameterByName.InterpretAsArray(BindingType elementType, ArraySizeType sizeType, int size)
        {
            var arrayType = new ArrayType();
            arrayType.ElementType = elementType;
            arrayType.ElementSize = size;
            arrayType.SizeType = sizeType;
            _currentParameter.Type = arrayType;

            return this;
        }

        IInterpretFunctionParameterByName IFunctionParameterName.WithParameterName(string paramName)
        {
            CreateParameter(paramName);
            return this;
        }

        IFunctionParameterName IInterpretFunctionParameterByName.InterpretAsPointerType(BindingType pointeeType)
        {
            _currentParameter.Type = new PointerType(){Pointee = pointeeType};
            return this;
        }

        IFunctionParameterName IInterpretFunctionParameterByName.InterpretAsIs()
        {
            return this;
        }

        public IFunctionParameterName InterpretAsBuiltinType(PrimitiveType type)
        {
            _currentParameter.Type = new BuiltinType(PrimitiveType.IntPtr);
            return this;
        }

        void IFunctionParameter.WithReturnType(BindingType returnType)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException(nameof(returnType));
            }

            if (lastOperation == LastOperation.AddFunction)
            {
                _currentFunction.ReturnType = returnType;
            }
            else if (lastOperation == LastOperation.AddFunctions)
            {
                foreach (var currentFunction in _currentFunctions)
                {
                    currentFunction.ReturnType = returnType;
                }
            }
        }

        public bool TryGetFunction(string functionName, bool matchCase, out FunctionExtension function)
        {
            if (matchCase)
            {
                return functions.TryGetValue(functionName, out function);
            }

            var key = functions.Keys.FirstOrDefault(x => x.Equals(functionName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(key))
            {
                return functions.TryGetValue(key, out function);
            }

            function = null;
            return false;
        }

        public bool TryGetCommonParameterType(BindingType inputType, out ParameterExtension parameter)
        {
            parameter = null;
            if (inputType == null)
            {
                return false;
            }

            foreach (var param in commonParameters)
            {
                var paramType = param.Type;

                if (inputType.IsPrimitiveType && paramType.IsPrimitiveType)
                {
                    parameter = param;
                }
                else if (inputType.IsPointerToCustomType(out var customType) && paramType.IsPointerToCustomType(out var paramCustomType))
                {
                    if (customType.Name == paramCustomType.Name)
                    {
                        parameter = param;
                    }
                }
                else if (inputType.IsPointerToArray() && paramType.IsPointerToArray())
                {
                    var inputArrayType = (ArrayType)((PointerType) (inputType)).Pointee;
                    var paramArrayType = (ArrayType)((PointerType) (paramType)).Pointee;

                    if (inputArrayType.ElementType == paramArrayType.ElementType)
                    {
                        parameter = param;
                    }
                }
                else if (inputType.IsArray() && paramType.IsArray())
                {
                    var inputArrayType = (ArrayType)inputType;
                    var paramArrayType = (ArrayType)paramType;

                    if (inputArrayType.ElementType == paramArrayType.ElementType)
                    {
                        parameter = param;
                    }
                }
            }

            return parameter != null;
        }

        IInterpretFunctionParameter ICommonFunctionParameter.WithParameterType(string typeName)
        {
            CreateCommonParameter(typeName);
            return this;
        }

        private void CreateCommonParameter(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            _currentCommonParameterTypeName = typeName;
            _currentCommonParameter = new ParameterExtension();
            commonParameters.Add(_currentCommonParameter);
        }

        IFunctionParameterType IFunctionParameterType.SetConst(bool value)
        {
            _currentCommonParameter.Type.Qualifiers.IsConst = value;

            return this;
        }

        IFunctionParameterType IFunctionParameterType.SetParameterKind(ParameterKind parameterKind)
        {
            _currentCommonParameter.ParameterKind = parameterKind;
            return this;
        }

        IFunctionParameterType IInterpretFunctionParameter.InterpretAsPointerType()
        {
            var type = new PointerType();
            var primitiveType = TypeUtil.GetPrimitiveTypeFromString(_currentCommonParameterTypeName);
            if (primitiveType != PrimitiveType.Unknown)
            {
                type.Pointee = new BuiltinType(primitiveType);
            }
            else
            {
                type.Pointee = new CustomType(_currentCommonParameterTypeName);
            }
            _currentCommonParameter.Type = type;
            return this;
        }

        IFunctionParameterType IInterpretFunctionParameter.InterpretAsPointerToArray(ArraySizeType sizeType, long size)
        {
            var type = new PointerType();
            var arrayType = new ArrayType();
            arrayType.SizeType = sizeType;
            arrayType.Size = size;
            var primitive = TypeUtil.GetPrimitiveTypeFromString(_currentCommonParameterTypeName);
            if (primitive != PrimitiveType.Unknown)
            {
                arrayType.ElementType = new BuiltinType(primitive);
            }
            else
            {
                arrayType.ElementType = new CustomType(_currentCommonParameterTypeName);
            }
            _currentCommonParameter.Type = type;
            return this;
        }

        IFunctionParameterType IInterpretFunctionParameter.InterpretAsArray(ArraySizeType sizeType, long size)
        {
            var arrayType = new ArrayType();
            arrayType.SizeType = sizeType;
            arrayType.Size = size;
            var primitive = TypeUtil.GetPrimitiveTypeFromString(_currentCommonParameterTypeName);
            if (primitive != PrimitiveType.Unknown)
            {
                arrayType.ElementType = new BuiltinType(primitive);
            }
            else
            {
                arrayType.ElementType = new CustomType(_currentCommonParameterTypeName);
            }

            _currentCommonParameter.Type = arrayType;
            return this;
        }

        IFunctionParameterType IInterpretFunctionParameter.InterpretAsCustomType()
        {
            _currentCommonParameter.Type = new CustomType(_currentCommonParameterTypeName);
            return this;
        }

        IFunctionParameterType IInterpretFunctionParameter.InterpretAsBuiltinType()
        {
            var primitive = TypeUtil.GetPrimitiveTypeFromString(_currentCommonParameterTypeName);
            if (primitive == PrimitiveType.Unknown)
            {
                throw new ArgumentException($"Type {_currentCommonParameterTypeName} is not a PrimitiveType");
            }

            _currentCommonParameter.Type = new BuiltinType(primitive);

            return this;
        }

        IFunctionParameterType IFunctionParameterType.SetNullable(bool value)
        {
            if (!_currentCommonParameter.Type.IsPointer())
            {
                throw new ArgumentException($"Nullable can be applied only to pointer types. {_currentCommonParameterTypeName} Is not a pointer type");
            }

            var type = (PointerType)_currentCommonParameter.Type;
            type.IsNullable = value;

            return this;
        }

        IFunctionParameterType IFunctionParameterType.SetDelegateNullable(bool value)
        {
            if (!_currentCommonParameter.Type.IsPointer())
            {
                throw new ArgumentException($"Nullable can be applied only to pointer types. {_currentCommonParameterTypeName} Is not a pointer type");
            }

            var type = (PointerType)_currentCommonParameter.Type;
            type.IsNullableForDelegate = value;

            return this;
        }

        IFunctionParameterType IInterpretFunctionParameter.InterpretAsIs()
        {
            return this;
        }

        IFunctionParameterType IFunctionParameterType.NextParameter(string paramType)
        {
            CreateCommonParameter(paramType);
            return this;
        }

        IFunctionParameterType IFunctionParameterType.SetDefaultValue(string value)
        {
            _currentCommonParameter.DefaultValue = value;
            return this;
        }
    }
}