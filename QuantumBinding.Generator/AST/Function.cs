using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Function : DeclarationUnit
    {
        public Function()
        {
            AccessSpecifier = AccessSpecifier.Internal;
            Parameters = new List<Parameter>();
        }

        public string DllName { get; set; }

        public string EntryPoint { get; set; }

        public CallingConvention CallingConvention { get; set; }

        public bool SuppressUnmanagedCodeSecurity { get; set; }

        public AccessSpecifier AccessSpecifier { get; set; }

        public BindingType ReturnType { get; set; }

        public bool IsPtrToConstChar { get; set; }

        public List<Parameter> Parameters { get; }

        public Class Class { get; set; }

        public override string ToString()
        {
            return $"{AccessSpecifier} {ReturnType} {Name}";
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }

        public override object Clone()
        {
            var func = new Function
            {
                AccessSpecifier = AccessSpecifier,
                DllName = DllName,
                EntryPoint = EntryPoint,
                CallingConvention = CallingConvention,
                SuppressUnmanagedCodeSecurity = SuppressUnmanagedCodeSecurity,
                ReturnType = ReturnType,
                IsPtrToConstChar = IsPtrToConstChar,
                Class = Class,
                Name = Name,
                Location = Location,
                Owner = Owner,
                AlternativeNamespace = AlternativeNamespace,

                IsIgnored = IsIgnored,
                Id = Id
            };
            func.Parameters.AddRange(Parameters);
            if (Comment != null)
            {
                func.Comment = (Comment) Comment.Clone();
            }

            return func;
        }
    }
}