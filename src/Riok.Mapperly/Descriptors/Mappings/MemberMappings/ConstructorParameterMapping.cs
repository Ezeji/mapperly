using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Riok.Mapperly.Descriptors.Mappings.MemberMappings;

public class ConstructorParameterMapping
{
    private readonly bool _selfOrPreviousIsUnmappedOptional;

    public ConstructorParameterMapping(IParameterSymbol parameter, NullMemberMapping delegateMapping, bool selfOrPreviousIsUnmappedOptional)
    {
        DelegateMapping = delegateMapping;
        Parameter = parameter;
        _selfOrPreviousIsUnmappedOptional = selfOrPreviousIsUnmappedOptional;
    }

    public IParameterSymbol Parameter { get; }

    public NullMemberMapping DelegateMapping { get; }

    public ArgumentSyntax BuildArgument(TypeMappingBuildContext ctx)
    {
        var argumentExpression = DelegateMapping.Build(ctx);
        var arg = SyntaxFactory.Argument(argumentExpression);
        return _selfOrPreviousIsUnmappedOptional ? arg.WithNameColon(SyntaxFactory.NameColon(Parameter.Name)) : arg;
    }

    protected bool Equals(ConstructorParameterMapping other) =>
        Parameter.Equals(other.Parameter, SymbolEqualityComparer.Default)
        && DelegateMapping.Equals(other.DelegateMapping)
        && _selfOrPreviousIsUnmappedOptional == other._selfOrPreviousIsUnmappedOptional;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((ConstructorParameterMapping)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = SymbolEqualityComparer.Default.GetHashCode(Parameter);
            hashCode = (hashCode * 397) ^ DelegateMapping.GetHashCode();
            hashCode = (hashCode * 397) ^ _selfOrPreviousIsUnmappedOptional.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(ConstructorParameterMapping? left, ConstructorParameterMapping? right) => Equals(left, right);

    public static bool operator !=(ConstructorParameterMapping? left, ConstructorParameterMapping? right) => !Equals(left, right);
}
