﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Class: DeclarationUnit
    {
        public Class()
        {
            AccessSpecifier = AccessSpecifier.Public;
            Fields = new List<Field>();
            methods = new List<Method>();
            Properties = new List<Property>();
            Constructors = new List<Constructor>();
            Operators = new List<Operator>();
        }

        private List<Method> methods { get; }

        public ClassType ClassType { get; set; }

        public BindingType UnderlyingNativeType { get; set; }

        public AccessSpecifier AccessSpecifier { get; set; }

        public AccessSpecifier WrapperMethodAccessSpecifier { get; set; }

        public Class ConnectedTo { get; set; }

        // True if the record is a POD (Plain Old Data) type.
        public bool IsSimpleType { get; set; }

        public bool IsTypedef { get; set; }

        public bool IsPointer { get; set; }

        public bool IsUnsafe { get; set; }

        public bool HasExtensions => ExtensionMethods.Any();

        public bool IsExtension { get; set; }

        public bool IsDisposable { get; set; }

        public string DisposableBaseClass { get; set; }

        public string DisposeBody { get; set; }

        public List<Field> Fields { get; }

        public List<Property> Properties { get; }

        public List<Constructor> Constructors { get; }

        public List<Operator> Operators { get; }

        public Class InnerStruct { get; set; }

        public Class ExtendedFrom { get; set; }

        public Class WrappedStruct { get; set; }

        public string WrappedStructFieldName { get; set; }

        public new List<Method> Methods => methods.Where(x => !x.IsExtensionMethod).ToList();

        public List<Method> ExtensionMethods => methods.Where(x=>x.IsExtensionMethod).ToList();

        public IReadOnlyCollection<Method> AllMethods => methods.AsReadOnly();

        public void AddField(Field field)
        {
            field.Class = this;
            Fields.Add(field);
        }

        public void RemoveField(Field field)
        {
            field.Class = null;
            Fields.Remove(field);
        }

        public void AddMethod(Method method)
        {
            methods.Add(method);
        }

        public void RemoveMethod(Method method)
        {
            methods.Remove(method);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool HasPointerFields => Fields.Any(x => x.IsPointer);

        public override object Clone()
        {
            return new Class()
            {
                Id = Id,
                AccessSpecifier = AccessSpecifier,
                ClassType = ClassType,
                Name = Name,
                IsPointer = IsPointer,
                InnerStruct = InnerStruct,
                IsTypedef = IsTypedef,
                UnderlyingNativeType = UnderlyingNativeType,
                Owner = Owner,
                Location = Location,
                IsIgnored = IsIgnored,
                IsSimpleType = IsSimpleType,
                WrappedStruct = WrappedStruct,
                WrappedStructFieldName = WrappedStructFieldName,
                ExtendedFrom = ExtendedFrom,
                AlternativeNamespace = AlternativeNamespace,
            };
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitClass(this);
        }
    }
}