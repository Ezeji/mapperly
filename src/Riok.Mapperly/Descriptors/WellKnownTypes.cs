using Microsoft.CodeAnalysis;
using Riok.Mapperly.Helpers;

namespace Riok.Mapperly.Descriptors;

public class WellKnownTypes
{
    private readonly Compilation _compilation;
    private readonly Dictionary<string, INamedTypeSymbol?> _cachedTypes = new();

    internal WellKnownTypes(Compilation compilation)
    {
        _compilation = compilation;
    }

    // use string type name as they are not available in netstandard2.0
    public INamedTypeSymbol? DateOnly => TryGet("System.DateOnly");

    public INamedTypeSymbol? TimeOnly => TryGet("System.TimeOnly");

    public ITypeSymbol GetArrayType(ITypeSymbol type) =>
        _compilation.CreateArrayTypeSymbol(type, elementNullableAnnotation: type.NullableAnnotation).NonNullable();

    public INamedTypeSymbol Get<T>() => Get(typeof(T));

    public INamedTypeSymbol Get(Type type)
    {
        if (type.IsConstructedGenericType)
        {
            type = type.GetGenericTypeDefinition();
        }
        return Get(type.FullName ?? throw new InvalidOperationException("Could not get name of type " + type));
    }

    public INamedTypeSymbol Get(string typeFullName) =>
        TryGet(typeFullName) ?? throw new InvalidOperationException("Could not get type " + typeFullName);

    public INamedTypeSymbol? TryGet(string typeFullName)
    {
        if (_cachedTypes.TryGetValue(typeFullName, out var typeSymbol))
        {
            return typeSymbol;
        }

        typeSymbol = _compilation.GetTypeByMetadataName(typeFullName);
        _cachedTypes.Add(typeFullName, typeSymbol);

        return typeSymbol;
    }
}
