using System.Collections.Generic;
using System.Text;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator
{
    public class CSharpTypePrinter : TypePrinter
    {
        public const string IntPtr = "IntPtr";
        public const string VoidPointer = "void*";
        public const string NullPointer = "null";
        public const string ObjectType = "object";
        public const string PointerOperator = "*";
        public const string DoublePointerOperator = "**";
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
                if (array.ElementType.TryGetClass(out Class @class))
                {
                    if (@class.IsSimpleType)
                    {
                        return Result($"{@class.UnderlyingNativeType}", "[]", attribute);
                    }
                }

                if (array.ElementType.IsPointer())
                {
                    var pointer = array.ElementType as PointerType;
                    if (pointer.TryGetClass(out Class ptr))
                    {
                        return Result($"{ptr.Name}", "[]", attribute);
                    }

                    if (pointer.IsPointerToCustomType(out var custom))
                    {
                        var decl = array.Declaration as Class;
                        if (MarshalType is MarshalTypes.Property or MarshalTypes.WrappedProperty && decl?.ConnectedTo != null)
                        {
                            return Result($"{decl.ConnectedTo.Name}", "[]", attribute);
                        }

                        return Result($"{custom.Name}", "[]", attribute);
                    }
                }

                array.ElementType.Declaration = array.Declaration;
                var result = array.ElementType.Visit(this);

                if ((array.ElementType.CanConvertToFixedArray() || array.ElementType.IsEnum()) && 
                    MarshalType == MarshalTypes.NativeField)
                {
                    return Result($"unsafe fixed {result}", parameterSuffix: $"[{array.Size}]");
                }

                //If parameter type could be converted to string and we are generating type for Wrapper Property or Method parameter
                // then it should be mapped to string
                if (array.ElementType.CanConvertToString() && 
                    MarshalType is MarshalTypes.WrappedProperty or MarshalTypes.MethodParameter)
                {
                    return Result($"string", "", attribute);
                }

                return Result($"{result}", "[]", attribute);
            }

            if (array.SizeType == ArraySizeType.Incomplete && array.IsConst)
            {
                if (MarshalType is MarshalTypes.NativeParameter or MarshalTypes.NativeField)
                {
                    if (array.ElementType.IsAnsiString())
                    {
                        return Result("sbyte");
                    }
                    if (array.ElementType.IsUnicodeString())
                    {
                        return Result("char");
                    }
                }
                return Result("string", "", "");
            }

            if (!array.ElementType.IsPrimitiveType && 
                MarshalType is MarshalTypes.MethodParameter or MarshalTypes.WrappedProperty)
            {
                return Result($"{array.Declaration.Name}", "[]");
            }

            array.ElementType.Declaration = array.Declaration;
            var visitResult = array.ElementType.Visit(this).ToString();
            return Result($"{visitResult}", "[]");
        }

        public override TypePrinterResult VisitPointerType(PointerType pointer)
        {
            var pointerDepth = pointer.GetDepth();
            
            if (MarshalType is 
                MarshalTypes.NativeField or 
                MarshalTypes.NativeParameter or 
                MarshalTypes.NativeReturnType or
                MarshalTypes.DelegateType or 
                MarshalTypes.DelegateParameter)
            {
                // TODO experimantal change if to switch
                // switch (pointer)
                // {
                //     case not null when pointer.IsAnsiString():
                //         break;
                // }
                if (pointer.IsAnsiString())
                {
                    return Result("sbyte", TextGenerator.GetPointerString(pointerDepth));
                }

                if (pointer.IsUnicodeString())
                {
                    return Result("char", TextGenerator.GetPointerString(pointerDepth));
                }

                if (pointer.IsStringArray(out var isUnicode))
                {
                    if (isUnicode)
                    {
                        //return Result("char", TextGenerator.GetPointerString(pointerDepth));
                        return Result("char", DoublePointerOperator);
                    }

                    //return Result("sbyte", TextGenerator.GetPointerString(pointerDepth));
                    return Result("sbyte", DoublePointerOperator);
                }

                if (pointer.IsPointerToVoid())
                {
                    var depth = pointerDepth;
                    if (pointerDepth > 1 && Parameter is { ParameterKind: ParameterKind.Out })
                    {
                        depth--;
                    }
                    return Result("void", TextGenerator.GetPointerString(depth));
                }

                if (pointer.IsPointerToIntPtr())
                {
                    return Result(IntPtr);
                }
                
                if (pointer.IsDoublePointer())
                {
                    var pointee = pointer.Pointee as PointerType;
                    pointee.Declaration = pointer.Declaration;
                    var result = pointee.Visit(this);
                    
                    if (Parameter is { ParameterKind: ParameterKind.Out })
                    {
                        return Result(result.Type, PointerOperator);
                    }
                    else
                    {
                        return Result(result.Type, TextGenerator.GetPointerString(pointerDepth));
                    }
                }

                if (pointer.IsPointerToArray())
                {
                    pointer.Pointee.Declaration = pointer.Declaration;
                    var result = pointer.Pointee.Visit(this);
                    return Result($"{result.Type}", PointerOperator);
                }

                if (pointer.IsPointerToStructOrUnion() || pointer.IsPointerToBuiltInType(out var primitive))
                {
                    pointer.Pointee.Declaration = pointer.Declaration; // To correctly visit pointee, we need to copy Declaration from pointer 
                    var result = pointer.Pointee.Visit(this);
                    if (Parameter is { ParameterKind: ParameterKind.Out })
                    {
                        return Result(result.ToString());
                    }
                    else
                    {
                        return Result(result.ToString(), PointerOperator);
                    }
                }

                // Case when current object points to a Class declaration (ClassType == Class/Struct/Union/Wrapper)
                if (pointer.TryGetClass(out var @class))
                {
                    pointer.Pointee.Declaration = pointer.Declaration;
                    if (pointer.IsSimpleType())
                    {
                        if (Options.PodTypesAsSimpleTypes)
                        {
                            var result = @class.UnderlyingNativeType.Visit(this);
                            return Result(result.Type, PointerOperator);
                        }
                    }

                    if (@class.ClassType == ClassType.Class && 
                        @class.InnerStruct != null)
                    {
                        if (Parameter is { ParameterKind: ParameterKind.Out })
                        {
                            return Result(@class.InnerStruct.Name);
                        }
                        else
                        {
                            return Result(@class.InnerStruct.Name, PointerOperator);
                        }
                    }
                }
                // Case when current object points to the array of enums
                else if (pointer.TryGetEnum(out var @enum))
                {
                    if (pointer.IsPointerToArray() || pointer.IsPointerToEnum() && 
                        MarshalType is MarshalTypes.NativeParameter or MarshalTypes.NativeField)
                    {
                        return Result($"{@enum.Name}", PointerOperator);
                    }
                }
            }
            else if (MarshalType is MarshalTypes.MethodParameter or MarshalTypes.Property or MarshalTypes.WrappedProperty)
            {
                if (pointer.IsAnsiString() || pointer.IsUnicodeString())
                {
                    return Result("string");
                }

                if (pointer.IsStringArray())
                {
                    return Result("string", "[]");
                }
                
                if (pointer.IsPointerToVoid() )
                {
                    return Result("void", TextGenerator.GetPointerString(pointerDepth));
                }
                
                if (pointer.IsPointerToIntPtr())
                {
                    return Result(IntPtr);
                }
                
                if (pointer.IsDoublePointer())
                {
                    var pointee = pointer.Pointee as PointerType;
                    pointee.Declaration = pointer.Declaration;
                    var result = pointer.Pointee.Visit(this);

                    if (Parameter is { ParameterKind: ParameterKind.Out })
                    {
                        if (pointee.IsPointerToArray())
                        {
                            return result;
                        }
                        return Result(result.Type, PointerOperator);
                    }
                    else
                    {
                        if (pointee.IsPointerToPrimitiveType(out var primitive))
                        {
                            return Result(result.Type, TextGenerator.GetPointerString(pointerDepth));
                        }
                        return Result(result.Type);
                    }
                }
                
                if (pointer.IsPointerToArray())
                {
                    pointer.Pointee.Declaration = pointer.Declaration;
                    var result = pointer.Pointee.Visit(this);
                    return result;
                }

                if (pointer.IsPointerToPrimitiveType(out var primitiveType) || 
                    pointer.IsPointerToSimpleType() || 
                    pointer.IsPointerToEnum())
                {
                    pointer.Pointee.Declaration = pointer.Declaration;
                    var result = pointer.Pointee.Visit(this);
                    if (MarshalType is MarshalTypes.WrappedProperty)
                    {
                        result.TypeSuffix = NullableOperator;
                    }

                    return result;
                }

                if (pointer.IsPointerToCustomType(out var customType))
                {
                    customType.Declaration = pointer.Declaration;
                    var result = customType.Visit(this);
                    return result;
                }
            }
            
            var res = pointer.Pointee.Visit(this);
            if (MarshalType is MarshalTypes.NativeField or MarshalTypes.NativeParameter)
            {
                return Result(res.Type, PointerOperator);
            }

            return res;
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
                        MarshalType is MarshalTypes.NativeParameter or MarshalTypes.MethodParameter or MarshalTypes.DelegateType or MarshalTypes.DelegateParameter)
                    {
                        return "bool";
                    }
                    return "byte";
                case PrimitiveType.Byte:
                    return "byte";
                case PrimitiveType.SChar:
                    if (Module.CharAsBoolForMethods &&
                        MarshalType is MarshalTypes.NativeParameter or MarshalTypes.MethodParameter or MarshalTypes.DelegateType or MarshalTypes.DelegateParameter)
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
                    return "System.IntPtr";
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
                if (@class.IsSimpleType)
                {
                    return Options.PodTypesAsSimpleTypes ? @class.UnderlyingNativeType.Visit(this) : Result(@class.Name);
                }
                
                if (MarshalType is MarshalTypes.NativeField or
                        MarshalTypes.NativeParameter or
                        MarshalTypes.DelegateType or
                        MarshalTypes.DelegateParameter or
                        MarshalTypes.NativeReturnType)
                {
                    if (@class.ClassType is ClassType.Struct or ClassType.Union)
                    {
                        return Result(@class.Name);
                    }

                    if (@class.InnerStruct != null)
                    {
                        return Result(@class.InnerStruct.Name);
                    }
                }

                if (MarshalType is MarshalTypes.WrappedProperty)
                {
                    if (@class.ConnectedTo != null)
                    {
                        return Result(@class.ConnectedTo.Name);
                    }
                }
                
                return Result(@class.Name);

            }

            if (customType.TryGetDelegate(out Delegate @delegate))
            {
                return Result("void", PointerOperator); // we must return delegates to C++ as void* because in other way we will corrupt memory
            }

            if (customType.TryGetEnum(out Enumeration @enum))
            {
                // if (MarshalType == MarshalTypes.NativeField && Module.GeneratorMode == GeneratorMode.Compatible)
                // {
                //     return @enum.InheritanceType;
                // }
                return @enum.Name;
            }

            return customType.IsInSystemHeader switch
            {
                true when MarshalType == MarshalTypes.WrappedProperty => Result("void*"),
                true when MarshalType == MarshalTypes.NativeField => Result("void"),
                _ => customType.Name
            };
        }

        public override TypePrinterResult VisitDelegateType(DelegateType delegateType)
        {
            TypePrinterResult result;
            if (delegateType.Declaration is Delegate @delegate)
            {
                PushMarshalType(MarshalTypes.DelegateType);
                result = VisitParameters(@delegate.Parameters);
                var returnType = @delegate.ReturnType.Visit(this);
                PopMarshalType();
                result.Type = $"{result.Type}, {returnType.Type}";
            }
            else
            {
                PushMarshalType(MarshalTypes.DelegateType);
                result = VisitParameters(delegateType.Parameters);
                PopMarshalType();
            }

            if (Module.GeneratorMode == GeneratorMode.Compatible)
            {
                result.Type = $"void*";
            }
            else
            {
                result.Type = $"delegate* unmanaged<{result.Type}>";
            }
            
            return result;
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
                var result = parameter.Visit(this);

                if (parameter.Index == 0 && isExtensionMethod)
                {
                    result = $"this {result}";
                }

                paramsList.Add(result.ToString());
                PopParameter();
            }

            return string.Join(", ", paramsList);
        }

        public override TypePrinterResult VisitParameter(Parameter parameter)
        {
            var hasModifier = ContainsModifiers(parameter, out var attribute, out var modifier);
            TypePrinterResult result;
            if (MarshalType is MarshalTypes.SkipParamTypesAndModifiers)
            {
                return parameter.Name;
            }

            if (MarshalType is MarshalTypes.SkipParamTypes or MarshalTypes.NativeFunctionCall)
            {
                result = parameter.Name;
                attribute = string.Empty;
            }
            else
            {
                result = parameter.Type.Visit(this);

                if (parameter.Type.Declaration is Class decl)
                {
                    // Will always write full namespaces for structs and classes to avoid collisions with native .Net types
                    if (Module.WrapInteropObjects)
                    {
                        if (!decl.IsSimpleType &&
                            decl.ClassType is ClassType.Struct or ClassType.Union &&
                            parameter.Type.IsPointerToStructOrUnion())
                        {
                            if (MarshalType is MarshalTypes.NativeParameter or MarshalTypes.MethodParameter)
                            {
                                result.Type = $"{decl.Namespace}.{result.Type}";
                            }
                        }
                        else if (decl.ClassType == ClassType.Class)
                        {
                            if (MarshalType == MarshalTypes.MethodParameter)
                            {
                                result.Type = $"{decl.Namespace}.{result.Type}";
                            }
                            else if (MarshalType is MarshalTypes.NativeParameter)
                            {
                                if (!string.IsNullOrEmpty(decl.InnerStruct.Namespace))
                                {
                                    result.Type = $"{decl.InnerStruct.Namespace}.{result.Type}";
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Type = $"{decl.Namespace}.{result.Type}";
                    }
                }
                
                if (parameter.ParameterKind == ParameterKind.Out && !parameter.Type.IsPointerToArray() && !parameter.Type.IsDoublePointer())
                {
                    result.TypeSuffix = string.Empty;
                }

                if (MarshalType == MarshalTypes.DelegateType)
                {
                    result.Type = result.ToString();
                }
                else if (MarshalType != MarshalTypes.DelegateType &&
                    parameter.ParameterKind == ParameterKind.InOut 
                    && !parameter.Type.IsPointer())
                {
                    result.Type += $" {parameter.Name}";
                }
                else
                {
                    result.Type += $"{result.TypeSuffix} {parameter.Name}";
                }

                result.TypeSuffix = string.Empty;
            }

            if (parameter.HasDefaultValue && MarshalType == MarshalTypes.MethodParameter)
            {
                result.Type += $" = {parameter.DefaultValue}";
            }

            switch (MarshalType)
            {
                // Do not write "ref" keyword for methods if parameter type is Array
                case MarshalTypes.MethodParameter when 
                    parameter.ParameterKind == ParameterKind.InOut && 
                    (parameter.Type.IsPurePointer() ||
                     parameter.Type.Declaration is Class { IsWrapper: true }):
                case MarshalTypes.SkipParamTypes when
                    parameter.ParameterKind != ParameterKind.InOut &&
                    !parameter.Type.IsPointer():
                case MarshalTypes.NativeFunctionCall when
                    parameter.ParameterKind is ParameterKind.InOut &&
                    (parameter.Type.IsPointerToArray() ||
                    parameter.IsOverload ||
                    (parameter.Type.Declaration is Class { IsSimpleType: false } &&
                    Options.PodTypesAsSimpleTypes)):
                    hasModifier = false;
                    break;
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

            builder.Append($"{GetAccessSpecifier(@class.AccessSpecifier)} unsafe ");
            builder.Append("partial ");
            builder.Append(@class.ClassType is ClassType.Union or ClassType.Struct ? "struct " : "class ");
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

            builder.Append($"{fieldResult} ");
            builder.Append($"{field.Name}");
            if (!string.IsNullOrEmpty(fieldResult.ParameterSuffix))
            {
                builder.Append(fieldResult.ParameterSuffix);
            }

            return Result(builder.ToString(), "", fieldResult.Attribute);
        }

        public override TypePrinterResult VisitProperty(Property property)
        {
            StringBuilder builder = new StringBuilder();
            var type = property.Type.Visit(this);
            builder.Append($"{type} {property.Name}");
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
            return accessSpecifier == null ? AccessSpecifier.Public.ToString().ToLower() : accessSpecifier.Value.ToString().ToLower();
        }

        private bool ContainsModifiers(Parameter param, out string attribute, out string modifier)
        {
            modifier = string.Empty;
            attribute = string.Empty;

            if (param.ParameterKind == ParameterKind.Readonly &&
                MarshalType is MarshalTypes.NativeParameter or MarshalTypes.SkipParamTypes)
            {
                return false;
            }

            if (MarshalType == MarshalTypes.MethodParameter && param.ParameterKind == ParameterKind.In)
            {
                return false;
            }

            if (MarshalType is MarshalTypes.SkipParamTypes or MarshalTypes.DelegateType or MarshalTypes.DelegateParameter)
            {
                if (param.ParameterKind == ParameterKind.InOut && !(param.Type.IsPurePointer()))
                {
                    modifier = "ref";
                    return true;
                }

                if (param.ParameterKind == ParameterKind.Out && !param.Type.IsPointerToArray())
                {
                    modifier = "out";
                    return true;
                }

                return false;
            }
            
            if (MarshalType == MarshalTypes.MethodParameter && param.Type.IsPointerToArray() && param.ParameterKind == ParameterKind.InOut)
            {
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
            else if (MarshalType is MarshalTypes.NativeParameter or MarshalTypes.NativeFunctionCall)
            {
                switch (param.ParameterKind)
                {
                    case ParameterKind.InOut:
                        if (param.Type.IsPointerToIntPtr()
                            /*param.Type.IsArray() || 
                            param.Type.IsPointerToArray() ||
                            param.Type.IsPurePointer()*/)
                        {
                            attribute = "[In, Out]";
                        }
                        break;
                    case ParameterKind.Out:
                        if (!param.Type.IsPointerToArray())
                        {
                            modifier = "out";
                        }
                        break;
                }
            }

            return true;
        }

        private TypePrinterResult Result(string type, string typeSuffix = "", string attribute = "", string parameterSuffix = "")
        {
            return new TypePrinterResult()
            {
                Attribute = attribute,
                Type = type,
                TypeSuffix = typeSuffix,
                ParameterSuffix = parameterSuffix
            };
        }
    }
}