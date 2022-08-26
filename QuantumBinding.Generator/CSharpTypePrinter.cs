using System;
using System.Collections.Generic;
using System.Text;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator
{
    public class CSharpTypePrinter : TypePrinter
    {
        public const string IntPtrType = "System.IntPtr";
        public const string ObjectType = "object";
        public const string NullableOperator = "?";

        public CSharpTypePrinter(BindingOptions options): base(options)
        {
            PushMarshalType(MarshalTypes.NativeParameter);
        }

        public override TypePrinterResult VisitArrayType(ArrayType array)
        {
            if (array.IsConstArray)
            {
                string attribute = "";
                if (MarshalType == MarshalTypes.NativeField)
                {
                    attribute = $"[MarshalAs(UnmanagedType.ByValArray, SizeConst = {array.Size})]";
                }
                else if (MarshalType == MarshalTypes.NativeParameter)
                {
                    attribute = $"[MarshalAs(UnmanagedType.LPArray, SizeConst = {array.Size})]";
                }

                if (array.CanConvertToString() && MarshalType != MarshalTypes.NativeField)
                {
                    return Result("string", "", $"[MarshalAs(UnmanagedType.ByValTStr, SizeConst = {array.Size})]");
                }

                if (array.ElementType.TryGetClass(out Class @class))
                {
                    if (@class.IsSimpleType)
                    {
                        return Result($"{@class.UnderlyingNativeType}[]", "", attribute);
                    }
                }

                if (array.ElementType.IsPointer())
                {
                    var pointer = array.ElementType as PointerType;
                    if (pointer.TryGetClass(out Class ptr))
                    {
                        return Result($"{ptr.Name}[]", "", attribute);
                    }

                    if (pointer.IsPointerToCustomType(out var custom))
                    {
                        var decl = array.Declaration as Class;
                        if ((MarshalType == MarshalTypes.Property || MarshalType == MarshalTypes.WrappedProperty) && decl?.ConnectedTo != null)
                        {
                            return Result($"{decl.ConnectedTo.Name}[]", "", attribute);
                        }

                        return Result($"{custom.Name}[]", "", attribute);
                    }
                }

                array.ElementType.Declaration = array.Declaration;
                var result = array.ElementType.Visit(this);

                if (array.ElementType.CanConvertToFixedArray() && MarshalType == MarshalTypes.NativeField)
                {
                    return Result($"unsafe fixed {result}", $"[{array.Size}]");
                }

                return Result($"{result}[]", "", attribute);
            }

            if (array.SizeType == ArraySizeType.Incomplete && 
                array.IsConst &&
                array.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar))
            {
                if (MarshalType == MarshalTypes.NativeParameter)
                {
                    return Result("string", "", "[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]");
                }

                return Result("string", "", "[MarshalAs(UnmanagedType.LPStr)]");
            }

            if (array.SizeType == ArraySizeType.Incomplete &&
                array.IsConst &&
                array.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
            {
                return Result("string", "", "[MarshalAs(UnmanagedType.LPWStr)]");
            }

            array.ElementType.Declaration = array.Declaration;
            var visitResult = array.ElementType.Visit(this).ToString();
            return Result($"{visitResult}[]");
        }

        public override TypePrinterResult VisitPointerType(PointerType pointer)
        {
            if ((pointer.IsAnsiString() || pointer.IsUnicodeString()) && (MarshalType == MarshalTypes.Property || MarshalType == MarshalTypes.WrappedProperty))
            {
                return Result("string");
            }

            if (pointer.IsAnsiString())
            {
                if (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.DelegateParameter)
                {
                    return Result("string", "", "[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]");
                }

                if (MarshalType == MarshalTypes.MethodParameter || MarshalType == MarshalTypes.NativeReturnType)
                {
                    return Result("string");
                }

                return Result(IntPtrType);
            }

            if (pointer.IsUnicodeString())
            {
                if (MarshalType != MarshalTypes.NativeField)
                {
                    return Result("string", "", "[MarshalAs(UnmanagedType.LPWStr)]");
                }

                return Result(IntPtrType);
            }

            if (pointer.IsStringArray())
            {
                if (MarshalType == MarshalTypes.NativeField)
                {
                    return Result(IntPtrType);
                }

                return Result("string[]");
            }

            if (MarshalType == MarshalTypes.NativeField || MarshalType == MarshalTypes.NativeReturnType) // pointers for struct fields are ALWAYS translated as IntPtr;
            {
                return Result(IntPtrType);
            }

            if (pointer.IsPointerToBuiltInType(out var builtIn) && MarshalType == MarshalTypes.WrappedProperty)
            {
                return $"{pointer.Pointee.Visit(this)}{NullableOperator}";
            }

            if (pointer.IsPurePointer()) // pure pointers also translated as IntPtr regardless of the usage
            {
                switch (MarshalType)
                {
                    case MarshalTypes.Property:
                    case MarshalTypes.WrappedProperty:
                    case MarshalTypes.NativeField:
                    case MarshalTypes.NativeParameter:
                    case MarshalTypes.MethodParameter:
                    case MarshalTypes.DelegateParameter:
                        return Result(IntPtrType);
                }
            }

            if (MarshalType == MarshalTypes.NativeParameter && 
                (Parameter?.ParameterKind == ParameterKind.In || Parameter?.ParameterKind == ParameterKind.Readonly))
            {
                if (pointer.IsPointerToPrimitiveType(out var prim) && !pointer.IsPointerToArray())
                {
                    return Result(pointer.Pointee.Visit(this).Type);
                }

                if (!pointer.IsPointerToArray())
                {
                    return Result(IntPtrType);
                }
            }

            if (pointer.TryGetClass(out Class @class))
            {
                if (pointer.IsPointerToArray())
                {
                    if (Parameter != null && 
                        Parameter.ParameterKind == ParameterKind.Out && 
                        MarshalType == MarshalTypes.NativeParameter)
                    {
                        return Result(IntPtrType);
                    }

                    if (@class.IsSimpleType && Options.PodTypesAsSimpleTypes)
                    {
                        return pointer.Pointee.Visit(this);
                    }

                    if (@class.ClassType == ClassType.Class && MarshalType == MarshalTypes.NativeParameter && @class.InnerStruct != null)
                    {
                        return Result(@class.InnerStruct.Name, "[]");
                    }

                    if (@class.ClassType == ClassType.StructWrapper && MarshalType == MarshalTypes.NativeParameter && @class.WrappedStruct != null)
                    {
                        return Result(@class.WrappedStruct.Name, "[]");
                    }

                    if (MarshalType == MarshalTypes.Property || MarshalType == MarshalTypes.WrappedProperty)
                    {
                        pointer.Pointee.Declaration = pointer.Declaration;
                        return pointer.Pointee.Visit(this);
                    }

                    return Result(@class.Name, "[]");
                }

                if (pointer.IsPointerToStruct() && !pointer.IsSimpleType())
                {
                    if (
                        (
                        @class.ClassType == ClassType.Struct ||
                        @class.ClassType == ClassType.Union ||
                        @class.ClassType == ClassType.StructWrapper ||
                        @class.ClassType == ClassType.UnionWrapper
                        ) &&
                        (
                        MarshalType == MarshalTypes.DelegateParameter ||
                        MarshalType == MarshalTypes.MethodParameter ||
                        MarshalType == MarshalTypes.Property ||
                        MarshalType == MarshalTypes.WrappedProperty
                        )
                        )
                    {
                        pointer.Pointee.Declaration = pointer.Declaration;
                        if (@class.ClassType == ClassType.Struct && MarshalType == MarshalTypes.DelegateParameter)
                        {
                            if (pointer.IsNullableForDelegate)
                            {
                                return Result(IntPtrType);
                            }
                            else
                            {
                                return $"{pointer.Pointee.Visit(this)}";
                            }
                        }

                        if (pointer.IsNullable && (@class.ClassType == ClassType.Struct || @class.ClassType == ClassType.Union))
                        {
                            if (Parameter?.ParameterKind == ParameterKind.Out)
                            {
                                return $"{pointer.Pointee.Visit(this)}";
                            }
                            return $"{pointer.Pointee.Visit(this)}{NullableOperator}";
                        }

                        if (@class.ConnectedTo != null && MarshalType == MarshalTypes.WrappedProperty)
                        {
                            return $"{@class.ConnectedTo.Name}";
                        }

                        return $"{pointer.Pointee.Visit(this)}";
                    }
                }

                if (pointer.IsSimpleType())
                {
                    if (MarshalType == MarshalTypes.MethodParameter && Parameter?.ParameterKind == ParameterKind.InOut)
                    {
                        if (Options.PodTypesAsSimpleTypes)
                        {
                            return $"{@class.UnderlyingNativeType.Visit(this)}";
                        }

                        return $"{@class.Name}{NullableOperator}";
                    }

                    if (pointer.IsPointerToPrimitiveType(out var primitive) && MarshalType == MarshalTypes.MethodParameter)
                    {
                        return pointer.Pointee.Visit(this);
                    }

                    if (pointer.IsNullable && (MarshalType == MarshalTypes.MethodParameter || MarshalType == MarshalTypes.WrappedProperty || MarshalType == MarshalTypes.Property))
                    {
                        if (Options.PodTypesAsSimpleTypes)
                        {
                            return $"{@class.UnderlyingNativeType.Visit(this)}{NullableOperator}";
                        }

                        return $"{@class.Name}{NullableOperator}";
                    }

                    if (!pointer.IsPointerToArray() && MarshalType == MarshalTypes.NativeParameter)
                    {
                        if (Options.PodTypesAsSimpleTypes)
                        {
                            return $"{@class.UnderlyingNativeType.Visit(this)}";
                        }

                        return $"{@class.Name}{NullableOperator}";
                    }
                }

                if (Parameter?.ParameterKind == ParameterKind.Out && MarshalType == MarshalTypes.NativeParameter && @class.ClassType == ClassType.Struct)
                {
                    return Result(@class.Name);
                }

                if (pointer.IsNullable && MarshalType == MarshalTypes.NativeParameter)
                {
                    return Result(IntPtrType);
                }

                if (@class.ClassType == ClassType.Class)
                {
                    switch (MarshalType)
                    {
                        case MarshalTypes.MethodParameter:
                            return Result(@class.Name);
                        case MarshalTypes.NativeField:
                        case MarshalTypes.NativeParameter:
                        case MarshalTypes.DelegateParameter:
                            return Result(@class.InnerStruct.Name);
                    }
                }

                if ((@class.ClassType == ClassType.Struct || @class.ClassType == ClassType.Union) 
                    && (MarshalType == MarshalTypes.NativeField || MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.WrappedProperty))
                {
                    return @class.Name;
                }

                if (@class.ClassType == ClassType.Struct && MarshalType == MarshalTypes.Property && @class.ConnectedTo != null)
                {
                    return @class.ConnectedTo.Name;
                }
            }
            else if (pointer.TryGetEnum(out var @enum))
            {
                if (pointer.IsPointerToArray() && MarshalType == MarshalTypes.NativeParameter)
                {
                    return Result($"{@enum.InheritanceType}[]");
                }

                if (!pointer.IsPointerToArray() && MarshalType == MarshalTypes.NativeParameter && Parameter?.ParameterKind == ParameterKind.InOut)
                {
                    return Result(@enum.InheritanceType);
                }
            }

            if (pointer.Declaration == null && pointer.Pointee is CustomType custom && custom.IsInSystemHeader && !custom.IsPrimitiveType)
            {
                return Result(ObjectType);
            }

            return pointer.Pointee.Visit(this);
        }

        public override TypePrinterResult VisitBuiltinType(BuiltinType builtin)
        {
            switch (builtin.Type)
            {
                case PrimitiveType.Bool:
                    if (MarshalType == MarshalTypes.NativeField)
                    {
                        return "byte";
                    }
                    return "bool";
                case PrimitiveType.Bool32:
                    if (MarshalType == MarshalTypes.NativeField)
                    {
                        return "uint";
                    }
                    return "bool";
                case PrimitiveType.Char:
                case PrimitiveType.UChar:
                    if (Module.CharAsBoolForMethods && 
                        (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.MethodParameter || MarshalType == MarshalTypes.DelegateParameter))
                    {
                        return "bool";
                    }
                    return "byte";
                case PrimitiveType.Byte:
                    return "byte";
                case PrimitiveType.SChar:
                    if (Module.CharAsBoolForMethods &&
                        (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.MethodParameter || MarshalType == MarshalTypes.DelegateParameter))
                    {
                        return "bool";
                    }
                    return "sbyte";
                case PrimitiveType.WideChar:
                    return "char";
                case PrimitiveType.UInt16:
                    return "ushort";
                case PrimitiveType.Int16:
                    return "short";
                case PrimitiveType.Float:
                    return "float";
                case PrimitiveType.Double:
                    return "double";
                case PrimitiveType.Int32:
                    return "int";
                case PrimitiveType.UInt32:
                    return "uint";
                case PrimitiveType.Decimal:
                    return "decimal";
                case PrimitiveType.Int64:
                    return "long";
                case PrimitiveType.UInt64:
                    return "ulong";
                case PrimitiveType.Null:
                case PrimitiveType.IntPtr:
                    return IntPtrType;
                case PrimitiveType.Void:
                    return "void";
                case PrimitiveType.String:
                    return "string";
                default:
                    return "Unknown";
            }
        }

        public override TypePrinterResult VisitCustomType(CustomType customType)
        {
            if (customType.TryGetClass(out Class @class))
            {
                if (@class.ClassType == ClassType.Class && (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.NativeField || MarshalType == MarshalTypes.DelegateParameter))
                {
                    if (@class.InnerStruct != null)
                    {
                        return @class.InnerStruct.Name;
                    }
                }

                if ((@class.ClassType == ClassType.StructWrapper || @class.ClassType == ClassType.UnionWrapper) &&
                    (MarshalType == MarshalTypes.WrappedProperty || MarshalType == MarshalTypes.MethodParameter))
                {
                    return @class.Name;
                }

                if ((@class.ClassType == ClassType.Struct && !@class.IsSimpleType) &&
                    MarshalType == MarshalTypes.WrappedProperty)
                {
                    return @class.Name;
                }

                if (@class.IsSimpleType && Options.PodTypesAsSimpleTypes)
                {
                    return @class.UnderlyingNativeType.Visit(this);
                }

                if (@class.ClassType == ClassType.Class && 
                    (
                    MarshalType == MarshalTypes.MethodParameter || 
                    MarshalType == MarshalTypes.WrappedProperty || 
                    MarshalType == MarshalTypes.Property))
                {
                    return @class.Name;
                }
            }

            if (customType.TryGetDelegate(out Delegate @delegate))
            {
                return Result(IntPtrType); // we must return delegates to C++ as IntPtr because in other way we will corrupt memory
            }

            if (customType.TryGetEnum(out Enumeration @enum))
            {
                if (MarshalType == MarshalTypes.NativeField)
                {
                    return @enum.InheritanceType;
                }
                return @enum.Name;
            }

            if (customType.IsInSystemHeader && MarshalType == MarshalTypes.Property)
            {
                return Result("object");
            }

            if (customType.IsInSystemHeader && MarshalType == MarshalTypes.NativeField)
            {
                return Result(IntPtrType);
            }

            return customType.Name;
        }

        public override TypePrinterResult VisitDependentNameType(DependentNameType dependentNameType)
        {
            return dependentNameType.Identifier;
        }

        public override TypePrinterResult VisitParameters(IEnumerable<Parameter> @params, MarshalTypes marshalType, bool isExtensionMethod = false)
        {
            PushMarshalType(marshalType);
            var paramsList = VisitParameters(@params, isExtensionMethod);
            PopMarshalType();
            return paramsList;
        }

        private TypePrinterResult VisitParameters(IEnumerable<Parameter> @params, bool isExtensionMethod = false)
        {
            List<string> paramsList = new List<string>();
            foreach (var parameter in @params)
            {
                PushParameter(parameter);
                try
                {
                    var result = parameter.Visit(this);

                    if (parameter.Index == 0 && isExtensionMethod)
                    {
                        result = $"this {result}";
                    }

                    paramsList.Add(result.ToString());
                }
                catch (Exception e)
                {

                }
                PopParameter();
            }

            return string.Join(", ", paramsList);
        }

        public override TypePrinterResult VisitParameter(Parameter parameter)
        {
            var hasModifier = ContainsModifiers(parameter, out string attribute, out string modifier);
            TypePrinterResult result;
            if (MarshalType == MarshalTypes.SkipParamTypes)
            {
                result = parameter.Name;
            }
            else
            {
                result = parameter.Type.Visit(this);

                if (parameter.Type.Declaration is Class decl)
                {
                    var originalNamespace = decl.Owner.FullNamespace;
                    if (!string.IsNullOrEmpty(originalNamespace))
                    {
                        originalNamespace += ".";
                    }
                    var alternativeNamespace = decl.AlternativeNamespace;
                    if (!string.IsNullOrEmpty(alternativeNamespace))
                    {
                        alternativeNamespace += ".";
                    }
                    // Will always write full namespaces for structs and classes to avoid interference with native .Net types
                    if (Module.WrapInteropObjects)
                    {
                        if (!decl.IsSimpleType &&
                            (decl.ClassType == ClassType.Struct || decl.ClassType == ClassType.Union) &&
                            parameter.Type.IsPointerToStruct() &&
                            result.Type != IntPtrType)
                        {
                            if ((MarshalType == MarshalTypes.NativeParameter && !((PointerType)parameter.Type).IsNullable) 
                                || MarshalType == MarshalTypes.MethodParameter || MarshalType == MarshalTypes.DelegateParameter)
                            {
                                result.Type = $"{alternativeNamespace}{result.Type}";
                            }
                        }
                        else if (decl.ClassType == ClassType.Class && result.Type != IntPtrType)
                        {
                            if (MarshalType == MarshalTypes.MethodParameter)
                            {
                                result.Type = $"{originalNamespace}{result.Type}";
                            }
                            else if (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.DelegateParameter)
                            {
                                if (!string.IsNullOrEmpty(decl.InnerStruct.AlternativeNamespace))
                                {
                                    result.Type = $"{decl.InnerStruct.AlternativeNamespace}.{result.Type}";
                                }
                            }
                        }
                    }
                    else if (result.Type != IntPtrType)
                    {
                        if (decl?.ClassType == ClassType.Class)
                        {
                            result.Type = $"{originalNamespace}{result.Type}";
                        }
                        else
                        {
                            result.Type = $"{alternativeNamespace}{result.Type}";
                        }
                    }
                }

                result.Type += $"{result.Suffix} {parameter.Name}";
                result.Suffix = string.Empty;
            }

            if (parameter.HasDefaultValue && MarshalType == MarshalTypes.MethodParameter)
            {
                result.Type += $" = {parameter.DefaultValue}";
            }

            if ((MarshalType == MarshalTypes.MethodParameter || MarshalType == MarshalTypes.DelegateParameter) && 
                parameter.ParameterKind == ParameterKind.InOut &&
                (parameter.Type.IsPointerToArray() || parameter.Type.IsArray())) // Do not write "ref" keyword for methods if parameter type is Array
            {
                hasModifier = false;
            }

            if (hasModifier)
            {
                if (!string.IsNullOrEmpty(attribute))
                {
                    string finalString = attribute;
                    if(!string.IsNullOrEmpty(result.Attribute))
                    {
                        finalString += $" {result.Attribute}";
                    }

                    if (!string.IsNullOrEmpty(modifier))
                    {
                        finalString += $" {modifier}";
                    }
                    return $"{finalString} {result.Type}";
                }

                if (!string.IsNullOrEmpty(modifier))
                {
                    return $"{modifier} {result.MergeResult()}";
                }

                return $"{result.MergeResult()}";
            }

            return result;
        }

        public override TypePrinterResult VisitEnum(Enumeration enumeration)
        {
            return $"{GetAccessSpecifier(enumeration.AccessSpecifier)} enum {enumeration.Name} : {enumeration.InheritanceType}";
        }

        public override TypePrinterResult VisitEnumItem(EnumerationItem item)
        {
            return $"{item.Name} = {item.Value}";
        }

        public override TypePrinterResult VisitClass(Class @class)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{GetAccessSpecifier(@class.AccessSpecifier)} ");
            builder.Append("partial ");

            if (@class.ClassType == ClassType.Union || @class.ClassType == ClassType.Struct)
            {
                builder.Append("struct ");
            }
            else
            {
                builder.Append("class ");
            }

            builder.Append($"{@class.Name}");

            return builder.ToString();
        }

        public TypePrinterResult VisitClassExtension(Class @class)
        {
            return $"extension {@class.Name}Extension";
        }

        public override TypePrinterResult VisitField(Field field)
        {
            if (field == null) return string.Empty;
            if (field.Type == null)
            {
                string result = string.Empty;
                if (MarshalType != MarshalTypes.MethodParameter)
                {
                    result = $"{GetAccessSpecifier(field.AccessSpecifier)} ";
                }
                return result += field.Name;
            }

            StringBuilder builder = new StringBuilder();
            PushField(field);
            var fieldResult = field.Type.Visit(this);
            PopField();
            if (MarshalType != MarshalTypes.MethodParameter)
            {
                builder.Append($"{GetAccessSpecifier(field.AccessSpecifier)} ");
            }

            builder.Append($"{fieldResult.Type} ");
            builder.Append($"{field.Name}");
            if (!string.IsNullOrEmpty(fieldResult.Suffix))
            {
                builder.Append(fieldResult.Suffix);
            }

            return Result(builder.ToString(), "", fieldResult.Attribute);
        }

        public override TypePrinterResult VisitProperty(Property property)
        {
            StringBuilder builder = new StringBuilder();
            var type = property.Type.Visit(this);
            builder.Append($"{type.Type} {property.Name}");
            return builder.ToString();
        }

        public override TypePrinterResult VisitFunction(Function function)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{function.ReturnType.Visit(this)} {function.Name}({VisitParameters(function.Parameters)})");
            return Result(builder.ToString());
        }

        public override TypePrinterResult VisitDelegate(Delegate @delegate)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{@delegate.ReturnType.Visit(this)} {@delegate.Name}({VisitParameters(@delegate.Parameters)})");
            return Result(builder.ToString());
        }

        public override TypePrinterResult VisitMethod(Method method)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{GetAccessSpecifier(method.AccessSpecifier)}");
            if (method.IsStatic)
            {
                builder.Append(" static");
            }

            return $"{builder} {method.ReturnType.Visit(this)} {method.Name}({VisitParameters(method.Parameters, MarshalTypes.MethodParameter, method.IsExtensionMethod)})";
        }

        public string GetAccessSpecifier(AccessSpecifier? accessSpecifier)
        {
            if (accessSpecifier == null)
            {
                return AccessSpecifier.Public.ToString().ToLower();
            }

            return accessSpecifier.ToString().ToLower();
        }

        private bool ContainsModifiers(Parameter param, out string attribute, out string modifier)
        {
            modifier = string.Empty;
            attribute = string.Empty;
            if (param.ParameterKind == ParameterKind.Readonly &&
                (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.SkipParamTypes))
            {
                return false;
            }

            if (MarshalType == MarshalTypes.MethodParameter && param.ParameterKind == ParameterKind.In)
            {
                return false;
            }

            if (MarshalType == MarshalTypes.SkipParamTypes)
            {
                if (param.ParameterKind == ParameterKind.InOut && !(param.Type.IsPurePointer() || param.Type.IsPointerToArray() || param.Type.IsArray()))
                {
                    modifier = "ref";
                    return true;
                }

                if (param.ParameterKind == ParameterKind.Out && !param.Type.IsPointerToArray())
                {
                    modifier = "out";
                    return true;
                }

                if (param.ParameterKind == ParameterKind.Out && param.Type.IsPointerToArray())
                {
                    modifier = "ref";
                    return true;
                }

                return false;
            }

            if (MarshalType == MarshalTypes.MethodParameter)
            {
                switch (param.ParameterKind)
                {
                    case ParameterKind.InOut:
                        modifier = "ref";
                        break;
                    case ParameterKind.Out:
                        modifier = "out";
                        break;
                    case ParameterKind.Readonly:
                        modifier = "in";
                        break;
                }
            }
            else if (MarshalType == MarshalTypes.NativeParameter || MarshalType == MarshalTypes.DelegateParameter)
            {
                switch (param.ParameterKind)
                {
                    case ParameterKind.In:
                        attribute = "[In]";
                        break;
                    case ParameterKind.InOut:
                        if (param.Type.IsArray() || 
                            param.Type.IsPointerToArray() ||
                            param.Type.IsPurePointer())
                        {
                            attribute = "[In, Out]";
                        }
                        else
                        {
                            modifier = "ref";
                        }
                        break;
                    case ParameterKind.Out:
                        if (param.Type.IsPointerToArray())
                        {
                            attribute = "[In, Out]";
                            modifier = "ref";
                        }
                        else
                        {
                            attribute = "[Out]";
                            modifier = "out";
                        }
                        break;
                }
            }

            return true;
        }

        private TypePrinterResult Result(string type, string suffix = "", string attribute = "")
        {
            return new TypePrinterResult()
            {
                Attribute = attribute,
                Type = type,
                Suffix = suffix
            };
        }
    }
}