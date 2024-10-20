using TypeCache.GraphQL.Types;

namespace GraphQL.Types;

/// <summary>
/// Represents a placeholder for another GraphQL type, referenced by name. Must be replaced with a
/// reference to the actual GraphQL type before using the reference.
/// </summary>
[DebuggerDisplay("ref {TypeName,nq}")]
public sealed class GraphQLTypeReference : GraphQLInterfaceType<object>, IObjectGraphType
{
	/// <summary>
	/// Initializes a new instance for the specified graph type name.
	/// </summary>
	public GraphQLTypeReference(string typeName)
	{
		this.Name = "__GraphQLTypeReference";
		TypeName = typeName;
	}

	/// <summary>
	/// Returns the GraphQL type name that this reference is a placeholder for.
	/// </summary>
	public string TypeName { get; private set; }

	ISet<FieldType> IObjectGraphType.Fields => throw Invalid();

	ISet<RuntimeTypeHandle> IObjectGraphType.Interfaces => throw new NotImplementedException();

	ISet<IInterfaceGraphType> IObjectGraphType.ResolvedInterfaces => throw new NotImplementedException();

	FieldType? IObjectGraphType.this[string field] => throw Invalid();

	private InvalidOperationException Invalid() => new($"This is just a reference to '{TypeName}'. Resolve the real type first.");

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		return obj is GraphQLTypeReference other
			? TypeName == other.TypeName
			: base.Equals(obj);
	}

	/// <inheritdoc/>
	public override int GetHashCode() => TypeName?.GetHashCode() ?? 0;
}

/// <summary>
/// Represents a placeholder for another GraphQL Output type, referenced by CLR type. Must be replaced with a
/// reference to the actual GraphQL type before using the reference.
/// </summary>
public sealed class GraphQLClrOutputTypeReference<[NotAGraphType] T> : GraphQLInterfaceType<T>, IObjectGraphType
{
	private GraphQLClrOutputTypeReference()
	{
	}

	FieldType? IObjectGraphType.this[string field] => throw new NotImplementedException();

	/// <inheritdoc/>
	ISet<FieldType> IObjectGraphType.Fields => throw new NotImplementedException();

	ISet<RuntimeTypeHandle> IObjectGraphType.Interfaces => throw new NotImplementedException();

	ISet<IInterfaceGraphType> IObjectGraphType.ResolvedInterfaces => throw new NotImplementedException();
}

/// <summary>
/// Represents a placeholder for another GraphQL Input type, referenced by CLR type. Must be replaced with a
/// reference to the actual GraphQL type before using the reference.
/// </summary>
public sealed class GraphQLClrInputTypeReference<[NotAGraphType] T> : InputObjectGraphType
{
	private GraphQLClrInputTypeReference()
	{
	}
}
