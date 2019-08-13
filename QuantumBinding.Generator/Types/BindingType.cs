using System;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Types
{
    public abstract class BindingType: ICloneable
    {
        protected BindingType()
        {
        }

        protected BindingType(BindingType type)
        {
            Qualifiers = type.Qualifiers;
            Declaration = type.Declaration;
        }

        public TypeQualifiers Qualifiers;

        public bool IsConst => Qualifiers.IsConst;

        public bool IsVolatile => Qualifiers.IsVolatile;

        public bool IsRestrict => Qualifiers.IsRestrict;

        public Declaration Declaration { get; set; }

        public virtual bool IsPrimitiveType => false;

        public abstract T Visit<T>(ITypeVisitor<T> typeVisitor);

        public abstract object Clone();
    }
}