namespace GraphQL.Types;

/// <summary>
/// Provides a mapping from CLR types to graph types.
/// </summary>
public interface IGraphTypeMappingProvider
{
	/// <summary>
	/// Returns a graph type for a given CLR type, or <see langword="null"/> if no mapping is available.
	/// </summary>
	/// <param name="clrType">The CLR type to be mapped.</param>
	/// <param name="isInputType">Indicates whether the type is an input type.</param>
	Type? GetGraphTypeFromClrType(Type clrType, bool isInputType);
}
