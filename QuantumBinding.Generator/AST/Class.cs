using System;
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
            fields = new List<Field>();
            methods = new List<Method>();
            properties = new List<Property>();
            Constructors = new List<Constructor>();
            Operators = new List<Operator>();
        }


        private List<Field> fields;
        private List<Method> methods;
        private List<Property> properties;

        public bool IsWrapper => ClassType is ClassType.StructWrapper or ClassType.UnionWrapper && WrappedStruct != null;
        
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

        public IReadOnlyList<Field> Fields => fields.AsReadOnly();

        public IReadOnlyList<Property> Properties => properties.AsReadOnly();

        public List<Constructor> Constructors { get; }

        public List<Operator> Operators { get; }

        public Class InnerStruct { get; set; }

        public Class ExtendedFrom { get; set; }

        public Class WrappedStruct { get; set; }

        public string WrappedStructFieldName { get; set; }

        public new List<Method> Methods => methods.Where(x => !x.IsExtensionMethod).ToList();

        public List<Method> ExtensionMethods => methods.Where(x=>x.IsExtensionMethod).ToList();

        public IReadOnlyCollection<Method> AllMethods => methods.AsReadOnly();

        public void AddConstructor(Constructor ctor)
        {
            Constructors.Add(ctor);
        }

        public bool AddField(Field field)
        {
            var f = Fields.FirstOrDefault(x => x.Name == field.Name);
            if (f == null)
            {
                field.Class = this;
                fields.Add(field);
            }

            return f == null;
        }

        public void RemoveField(Field field)
        {
            field.Class = null;
            fields.Remove(field);
        }

        public bool AddProperty(Property prop)
        {
            var property = properties.FirstOrDefault(x => x.Name == prop.Name);
            if (property == null)
            {
                properties.Add(prop);
            }

            return property == null;
        }

        public void RemoveProperty(string name)
        {
            var property = properties.FirstOrDefault(x => x.Name == name);
            if (property != null)
            {
                properties.Remove(property);
            }
        }

        public void AddMethod(Method method)
        {
            if (method == null) return;
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
                OriginalName = OriginalName,
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
            };
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitClass(this);
        }
    }
}