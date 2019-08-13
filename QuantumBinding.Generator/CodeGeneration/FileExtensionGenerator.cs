using QuantumBinding.Generator.AST;
using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class FileExtensionGenerator : CSharpCodeGeneratorBase
    {
        public FileExtensionGenerator(ProcessingContext context, TranslationUnit unit, FileExtensionKind extensionKind)
            : this(context, new List<TranslationUnit> { unit }, extensionKind)
        {
        }

        public FileExtensionGenerator(ProcessingContext context, IEnumerable<TranslationUnit> units, FileExtensionKind extensionKind)
            : base(context, units, GeneratorSpecializations.None)
        {
            this.extensionKind = extensionKind;
        }

        private FileExtensionKind extensionKind;
        private string disposableClassName = "DisposableObject";

        public override void Run()
        {
            PushBlock(CodeBlockKind.Root);
            GenerateFileHeader();
            GenerateNamespace();
            PopBlock();
        }

        protected virtual void GenerateNamespace()
        {
            switch (extensionKind)
            {
                case FileExtensionKind.Disposable:
                    GenerateDisposeExtension();
                    break;
                case FileExtensionKind.Utils:
                    GenerateUtilsClass();
                    break;
            }
        }

        private void GenerateDisposeExtension()
        {
            int classesCount = 0;
            foreach (var unit in TranslationUnits)
            {
                var classes = unit.Classes.Where(x => !x.IsIgnored && x.ClassType == ClassType.Class && x.IsDisposable && !x.IsExtension).ToList();
                if (classes.Count == 0)
                {
                    continue;
                }

                classesCount += classes.Count;

                PushBlock(CodeBlockKind.Namespace);
                WriteCurrentNamespace(unit);
                WriteOpenBraceAndIndent();

                GenerateUsings();

                NewLine();

                TypePrinter.PushModule(unit.Module);
                GenerateDisposedClasses(classes);

                UnindentAndWriteCloseBrace();
                PopBlock();
            }

            if (classesCount == 0)
            {
                IsGeneratorEmpty = true;
            }
        }

        private void GenerateUtilsClass()
        {
            PushBlock(CodeBlockKind.Namespace);
            CurrentNamespace = AstContext.Module.OutputNamespace;
            WriteLine($"namespace {AstContext.Module.OutputNamespace}");
            WriteOpenBraceAndIndent();

            GenerateUsings();

            NewLine();

            PushBlock(CodeBlockKind.Utils);
            //---------MarshalUtils
            PushBlock(CodeBlockKind.Class, "MarshalUtils");

            string marshaUtils =
@"public static class MarshalUtils
{
    public static void IntPtrToManagedArray<T>(IntPtr unmanagedArray, T[] managedArray)
    {
        var size = Marshal.SizeOf(typeof(T));
        for (int i = 0; i < managedArray.Length; i++)
        {
            IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
            managedArray[i] = Marshal.PtrToStructure<T>(ins);
        }
    }

    public static string[] IntPtrToStringArray(IntPtr unmanagedArray, uint count, bool isUnicode = false)
    {
        var array = new string[count];
        for (int i = 0; i < count; ++i)
        {
            IntPtr strPtr = new IntPtr(unmanagedArray.ToInt64() + i * IntPtr.Size);
            if (isUnicode)
            {
                array[i] = Marshal.PtrToStringUni(strPtr);
            }
            else
            {
                array[i] = Marshal.PtrToStringAnsi(strPtr);
            }
        }
        return array;
    }

    public static IntPtr MarshalStructToPtr<T>(T @struct)
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(@struct));
        Marshal.StructureToPtr(@struct, ptr, false);
        return ptr;
    }
}";

            WriteLine(marshaUtils);
            PopBlock();
            //---------MarshalUtils

            NewLine();

            //---------ConstCharPtrMarshaler
            PushBlock(CodeBlockKind.Class, "ConstCharPtrMarshaler");

            string customMarshaler =
@"public class ConstCharPtrMarshaler : ICustomMarshaler
{
    private static readonly ConstCharPtrMarshaler instance = new ConstCharPtrMarshaler();

    public static ICustomMarshaler GetInstance(string cookie)
    {
        return instance;
    }

    public object MarshalNativeToManaged(IntPtr pNativeData)
    {
        return Marshal.PtrToStringAnsi(pNativeData);
    }

    public IntPtr MarshalManagedToNative(object managedObj)
    {
        var str = (string)managedObj;
        return Marshal.StringToHGlobalAnsi(str);
    }

    public void CleanUpNativeData(IntPtr pNativeData)
    {
    }

    public void CleanUpManagedData(object managedObj)
    {
    }

    public int GetNativeDataSize()
    {
        return IntPtr.Size;
    }
}";

            WriteLine(customMarshaler);
            PopBlock();
            //---------ConstCharPtrMarshaler

            NewLine();

            GenerateDisposeClass();

            NewLine();

            GenerateGCHandleReferenceClass();

            NewLine();

            GenerateStringReferenceClass();

            NewLine();

            GenerateStringArrayReferenceClass();

            NewLine();

            GenerateStructReferenceClass();

            PopBlock();

            UnindentAndWriteCloseBrace();
            PopBlock();
        }

        private string GetDisposeClassString()
        {
            return
@"public class DisposableObject: IDisposable
{
    public bool IsDisposed { get; private set; }

    protected virtual void Dispose(bool disposeManaged)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposeManaged)
        {
            ManagedDisposeOverride();
        }

        UnmanagedDisposeOverride();

        IsDisposed = true;
    }

    protected virtual void ManagedDisposeOverride() { }

    protected virtual void UnmanagedDisposeOverride() { }

    ~DisposableObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}";
        }

        private void GenerateDisposedClasses(List<Class> classes)
        {
            foreach(var @class in classes)
            {
                GenerateClass(@class);
                NewLine();
            }
        }

        protected override void GenerateClass(Class @class)
        {
            PushBlock(CodeBlockKind.Class, @class);

            WriteLine($"{TypePrinter.VisitClass(@class)} : {disposableClassName}");

            WriteOpenBraceAndIndent();

            GenerateUnmanagedDisposePattern(@class.DisposeBody);

            UnindentAndWriteCloseBrace();

            PopBlock();
        }

        private void GenerateDisposeClass()
        {
            PushBlock(CodeBlockKind.Class, "DisposableObject");
            WriteLine(GetDisposeClassString());
            PopBlock();
        }

        private void GenerateGCHandleReferenceClass()
        {
            PushBlock(CodeBlockKind.Class, "GCHandleReference");
            var str =
@"public class GCHandleReference : DisposableObject
{
    bool isInitialized;
    GCHandle reference;

    public IntPtr Handle
    {
        get
        {
            if (reference.IsAllocated)
            {
                return reference.AddrOfPinnedObject();
            }
            return IntPtr.Zero;
        }
    }

    public GCHandleReference(object obj)
    {
        if (obj != null)
        {
            isInitialized = true;
            reference = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (isInitialized)
        {
            reference.Free();
        }
    }
}";
            WriteLine(str);
            PopBlock();
        }

        private void GenerateStringReferenceClass()
        {
            PushBlock(CodeBlockKind.Class, "StringReference");
            var str =
@"public class StringReference : DisposableObject
{
    bool isInitialized;
    IntPtr reference;

    public IntPtr Handle => reference;

    public StringReference(string str, bool isUnicode)
    {
        if (string.IsNullOrEmpty(str))
        {
            return;
        }

        if (!isUnicode)
        {
            reference = Marshal.StringToHGlobalAnsi(str);
        }
        else
        {
            reference = Marshal.StringToHGlobalUni(str);
        }
        isInitialized = true;
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (isInitialized)
        {
            Marshal.FreeHGlobal(reference);
        }
    }
}";
            WriteLine(str);
            PopBlock();
        }

        private void GenerateStringArrayReferenceClass()
        {
            PushBlock(CodeBlockKind.Class, "StringArrayReference");
            var str =
@"public class StringArrayReference : DisposableObject
{
    IntPtr[] stringReferences;

    GCHandle reference;

    public IntPtr Handle
    {
        get
        {
            if (reference.IsAllocated)
            {
                return reference.AddrOfPinnedObject();
            }
            return IntPtr.Zero;
        }
    }

    public StringArrayReference(in string[] strArray, bool isUnicode)
    {
        if (strArray != null && strArray.Length > 0)
        {
            stringReferences = new IntPtr[strArray.Length];
            int cnt = 0;
            foreach (var str in strArray)
            {
                if (!isUnicode)
                {
                    stringReferences[cnt++] = Marshal.StringToHGlobalAnsi(str);
                }
                else
                {
                    stringReferences[cnt++] = Marshal.StringToHGlobalUni(str);
                }
            }
            reference = GCHandle.Alloc(stringReferences, GCHandleType.Pinned);
        }
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (stringReferences != null)
        {
            foreach (var ptr in stringReferences)
            {
                Marshal.FreeHGlobal(ptr);
            }
            reference.Free();
        }
    }
}";
            WriteLine(str);
            PopBlock();
        }

        private void GenerateStructReferenceClass()
        {
            var str =
@"public class StructReference : DisposableObject
{
    bool isInitialized;
    IntPtr reference;

    public IntPtr Handle => reference;

    public StructReference(object obj)
    {
        if (obj != null)
        {
            isInitialized = true;
            reference = MarshalUtils.MarshalStructToPtr(obj);
        }
    }

    protected override void UnmanagedDisposeOverride()
    {
        base.UnmanagedDisposeOverride();
        if (isInitialized)
        {
            Marshal.FreeHGlobal(reference);
        }
    }
}";

            PushBlock(CodeBlockKind.Class, "StructReference");
            WriteLine(str);
            PopBlock();
        }

        private void GenerateUnmanagedDisposePattern(string disposeContent)
        {
            PushBlock(CodeBlockKind.Disposable);
            WriteLine("protected override void UnmanagedDisposeOverride()");
            WriteOpenBraceAndIndent();
            WriteLine(disposeContent);
            UnindentAndWriteCloseBrace();
            PopBlock();
        }
    }

    
}
