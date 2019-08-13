using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator
{
    public interface IDeclarationVisitor<out T>
    {
        T VisitEnum(Enumeration enumeration);
        T VisitEnumItem(EnumerationItem item);
        T VisitFunction(Function function);
        T VisitDelegate(Delegate @delegate);
        T VisitParameter(Parameter parameter);
        T VisitComment(Comment comment);
        T VisitClass(Class @class);
        T VisitField(Field field);
        T VisitProperty(Property property);
        T VisitMethod(Method method);
        T VisitMacro(Macro macros);
        T VisitNamespace(Namespace @namespace);
        T VisitTranslationUnit(TranslationUnit translationUnit);
    }
}