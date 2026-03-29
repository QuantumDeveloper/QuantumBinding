using System.ComponentModel.DataAnnotations;

namespace QuantumBinding.Generator
{
    public enum PrimitiveType
    {
        [Display(Name = "(none)")]
        None = 0,
        [Display(Name = "null")]
        Null = 1,
        [Display(Name = "void")]
        Void = 2,
        [Display(Name = "System.IntPtr")]
        IntPtr = 3,
        [Display(Name = "System.UintPtr")]
        UintPtr = 4,
        [Display(Name = "System.Char")]
        Char = 5,
        [Display(Name = "System.Char")]
        UChar = 6,
        [Display(Name = "System.Char")]
        SChar = 7,
        [Display(Name = "System.Char")]
        WideChar = 8,
        [Display(Name = "System.Byte")]
        Byte = 9,
        [Display(Name = "System.Sbyte")]
        Sbyte = 10,
        [Display(Name = "System.Boolean")]
        Bool = 11,
        [Display(Name = "System.Boolean")]
        Bool32 = 12,
        [Display(Name = "System.Int16")]
        Int16 = 13,
        [Display(Name = "System.Int32")]
        Int32 = 14,
        [Display(Name = "System.Int64")]
        Int64 = 15,
        [Display(Name = "System.UInt64")]
        UInt16 = 16,
        [Display(Name = "System.UInt32")]
        UInt32 = 17,
        [Display(Name = "System.UInt64")]
        UInt64 = 18,
        [Display(Name = "System.Single")]
        Float = 19,
        [Display(Name = "System.Double")]
        Double = 20,
        [Display(Name = "System.Decimal")]
        Decimal = 21,
        [Display(Name = "System.String")]
        String = 22,
        [Display(Name = "object")]
        Object = 23,
        [Display(Name = "(unknown)")]
        Unknown = 24,
        
    }
}