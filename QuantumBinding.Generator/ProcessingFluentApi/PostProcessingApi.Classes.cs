﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public partial class PostProcessingApi : IClassParameters, ISetField, ISetProperty
    {
        private enum LastOperation
        {
            AddClass,
            AddEnum,
            AddClasses,
            AddFunction,
            AddFunctions
        }

        private readonly Dictionary<string, ClassExtension> classes;
        private ClassExtension _currentClass;
        private FieldExtension _currentField;
        private List<ClassExtension> _currentClasses;
        private PropertyExtension _currentProperty;

        private LastOperation lastOperation;

        public IClassParameters Class(string className)
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentNullException(nameof(className));
            }

            if (!classes.TryGetValue(className, out _currentClass))
            {
                var @class = new ClassExtension();

                @class.Name = className;
                _currentClass = @class;
                classes.Add(className, @class);
            }

            lastOperation = LastOperation.AddClass;

            return this;
        }

        public IClassParameters Classes(IEnumerable<string> classesNames)
        {
            var classesNamesList = classesNames.ToList();
            if (classesNames == null)
            {
                throw new ArgumentNullException(nameof(classesNames));
            }

            if (classesNamesList.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(classesNames), "Collection should contain at least one class name");
            }

            _currentClasses = new List<ClassExtension>();
            foreach (var className in classesNamesList)
            {
                if (!classes.TryGetValue(className, out var @class))
                {
                    var classExtension = new ClassExtension(className);
                    _currentClasses.Add(classExtension);
                    classes.Add(className, classExtension);
                }
                else
                {
                    _currentClasses.Add(@class);
                }
            }

            lastOperation = LastOperation.AddClasses;

            return this;
        }

        ISetField IClassParameters.WithField(string fieldName)
        {
            ProcessField(fieldName);
            return this;
        }

        ISetField IClassParameters.AddField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (lastOperation == LastOperation.AddClass)
            {
                var field = _currentClass.FieldsToAdd.FirstOrDefault(x => x.Name == fieldName);
                if (field != null)
                {
                    throw new ArgumentException($"Parameter with name {field} already added for {_currentClass.Name}");
                }

                field = new FieldExtension(fieldName);
                _currentClass.FieldsToAdd.Add(field);
                _currentField = field;
            }
            else if (lastOperation == LastOperation.AddClasses)
            {
                var field = new FieldExtension(fieldName);
                foreach (var @class in _currentClasses)
                {
                    var f = @class.FieldsToAdd.FirstOrDefault(x => x == field);
                    if (f != null)
                    {
                        throw new ArgumentException($"Parameter with name {field} already added for {_currentClass.Name}");
                    }

                    @class.FieldsToAdd.Add(field);
                    _currentField = field;
                }
            }
            return this;
        }

        ISetField ISetField.SetType(BindingType type)
        {
            _currentField.Type = type ?? throw new ArgumentNullException(nameof(type));
            _currentField.ReplaceDeclaration = false;

            return this;
        }

        ISetField ISetField.ChangeType()
        {
            _currentField.ReplaceDeclaration = true;

            return this;
        }
        
        ISetField ISetField.ChangeType<T>()
        {
            _currentField.ReplaceDeclaration = true;
            _currentField.DeclarationType = typeof(T);

            return this;
        }

        ISetField ISetField.AddAttribute(string attribute)
        {
            if (string.IsNullOrEmpty(attribute))
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            _currentField.Attributes.Add(attribute);

            return this;
        }

        ISetField ISetField.RemoveExistingAttributes()
        {
            _currentField.RemoveExistingAttributes = true;
            return this;
        }

        ISetField IClassParameters.SetClassType(ClassType classType)
        {
            _currentClass.ClassType = classType;
            return this;
        }

        ISetProperty IClassParameters.AddProperty(string propertyName)
        {
            _currentProperty = new PropertyExtension(propertyName);
            if (lastOperation == LastOperation.AddClass)
            {
                var prop = _currentClass.PropertiesToAdd.FirstOrDefault(x => x.Name == propertyName);
                if (prop == null)
                {
                    _currentClass.PropertiesToAdd.Add(_currentProperty);
                }
            }
            else if (lastOperation == LastOperation.AddClasses)
            {
                foreach (var currentClass in _currentClasses)
                {
                    var prop = currentClass.PropertiesToAdd.FirstOrDefault(x => x.Name == propertyName);
                    if (prop == null)
                    {
                        currentClass.PropertiesToAdd.Add(_currentProperty);
                    }
                }
            }
            return this;
        }

        ISetProperty ISetProperty.SetType(BindingType type)
        {
            _currentProperty.Type = type;
            return this;
        }

        ISetProperty ISetProperty.SetField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            var field = new FieldExtension(fieldName);
            _currentProperty.Field = field;

            return this;
        }

        ISetProperty ISetProperty.SetSetter(Method setter)
        {
            _currentProperty.Setter = setter;
            return this;
        }

        ISetProperty ISetProperty.SetGetter(Method getter)
        {
            _currentProperty.Getter = getter;
            return this;
        }

        ISetProperty ISetProperty.IsAutoProperty(bool value)
        {
            _currentProperty.IsAutoProperty = value;
            return this;
        }

        void IClassParameters.SetUnderlyingType(BindingType type)
        {
            if (lastOperation == LastOperation.AddClass)
            {
                _currentClass.UnderlyingNativeType = type;
            }
            else if (lastOperation == LastOperation.AddClasses)
            {
                foreach (var currentClass in _currentClasses)
                {
                    currentClass.UnderlyingNativeType = type;
                }
            }
        }

        public IClassParameters Ignore()
        {
            _currentClass.IsIgnored = true;
            return this;
        }

        public IClassParameters CleanObject()
        {
            _currentClass.CleanObject = true;
            return this;
        }

        public IClassParameters CopyFieldsFromLinkedObject()
        {
            _currentClass.CopyFieldsFromLinkedObject = true;
            return this;
        }

        public bool TryGetClass(string className, bool matchCase, out ClassExtension @class)
        {
            if (matchCase)
            {
                return classes.TryGetValue(className, out @class);
            }

            var key = classes.Keys.FirstOrDefault(x => x.Equals(className, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(key))
            {
                return classes.TryGetValue(key, out @class);
            }

            @class = null;
            return false;
        }

        ISetField ISetField.InterpretAsPointerToArray(BindingType elementType, bool isNullable, string arraySizeSource, uint pointerDepth)
        {
            var pointer = new PointerType();
            pointer.IsNullable = isNullable;
            _currentField.Type = pointer;
            for (int i = 1; i < pointerDepth; i++)
            {
                var ptr = new PointerType();
                pointer.Pointee = ptr;
                pointer = ptr;
            }
            
            var arrayType = new ArrayType();
            arrayType.ArraySizeSource = arraySizeSource;
            arrayType.SizeType = ArraySizeType.Incomplete;
            arrayType.ElementType = elementType;
            pointer.Pointee = arrayType;
            
            return this;
        }

        public ISetField InterpretAsPointerToPrimitiveType(PrimitiveType primitiveType, uint pointerDepth = 1)
        {
            var pointer = new PointerType();
            _currentField.Type = pointer;
            for (int i = 1; i < pointerDepth; i++)
            {
                var ptr = new PointerType();
                pointer.Pointee = ptr;
                pointer = ptr;
            }

            var builtin = new BuiltinType(PrimitiveType.Void);
            pointer.Pointee = builtin;

            return this;
        }

        ISetField ISetField.InterpretAsArray(BindingType elementType, ArraySizeType sizeType, int size)
        {
            var arrayType = new ArrayType();
            arrayType.ElementType = elementType;
            arrayType.ElementSize = size;
            arrayType.SizeType = sizeType;
            _currentField.Type = arrayType;

            return this;
        }

        ISetField ISetField.InterpretAsPointerType(BindingType pointeeType)
        {
            _currentField.Type = new PointerType() { Pointee = pointeeType };
            return this;
        }

        ISetField ISetField.InterpretAsIs()
        {
            return this;
        }

        public ISetField InterpretAs(BindingType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            _currentField.Type = type;
            return this;
        }

        public ISetField InterpretAsCustomType(string typeName)
        {
            _currentField.Type = new CustomType(typeName);
            return this;
        }

        public ISetField InterpretAsDelegateType(IEnumerable<Parameter> parameters, string name)
        {
            _currentField.Type = new DelegateType() { Name = name, Parameters = new List<Parameter>(parameters)};
            return this;
        }

        ISetField ISetField.WithField(string fieldName)
        {
            ProcessField(fieldName);
            return this;
        }

        private void ProcessField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (lastOperation == LastOperation.AddClass)
            {
                var field = _currentClass.Fields.FirstOrDefault(x => x.Name == fieldName);
                if (field != null)
                {
                    throw new ArgumentException($"Parameter with name {field} already added for {_currentClass.Name}");
                }

                field = new FieldExtension(fieldName);
                _currentClass.Fields.Add(field);
                _currentField = field;
            }
            else if (lastOperation == LastOperation.AddClasses)
            {
                var field = new FieldExtension(fieldName);
                foreach (var @class in _currentClasses)
                {
                    field = @class.Fields.FirstOrDefault(x => x.Name == fieldName);
                    if (field != null)
                    {
                        throw new ArgumentException($"Parameter with name {field} already added for {_currentClass.Name}");
                    }

                    @class.Fields.Add(field);
                }
                _currentField = field;
            }
        }
    }
}
