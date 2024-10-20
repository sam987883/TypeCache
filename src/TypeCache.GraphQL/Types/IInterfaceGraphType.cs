using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// Represents a GraphQL interface graph type.
/// </summary>
public interface IInterfaceGraphType : IAbstractGraphType
{
	FieldType? this[string field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field));

	FieldType? this[GraphQLName field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field.StringValue));

	/// <summary>
	/// The <see cref="RuntimeTypeHandle"/> of the type represented by this object graph.
	/// </summary>
	RuntimeTypeHandle TypeHandle { get; }

	ISet<FieldType> Fields { get; }
}
