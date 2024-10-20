using System.Threading.Tasks;
using GraphQL.Types;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// Provides the ability to filter the schema upon introspection to hide types, fields, arguments, enum values, and directives.
/// Can be used to hide information, such as graph types, from clients that have an inadequate permission level.
/// </summary>
public interface ISchemaFilter
{
	/// <summary>
	/// Returns a boolean indicating whether the specified graph type should be returned within the introspection query.
	/// </summary>
	/// <param name="type">The graph type to consider.</param>
	bool AllowType(IGraphType type);

	/// <summary>
	/// Returns a boolean indicating whether the specified field should be returned within the introspection query.
	/// </summary>
	/// <param name="parent">The parent type to which the field belongs.</param>
	/// <param name="field">The field to consider.</param>
	bool AllowField(IGraphType parent, IFieldType field);

	/// <summary>
	/// Returns a boolean indicating whether the specified argument should be returned within the introspection query.
	/// </summary>
	/// <param name="field">The field to which the argument belongs.</param>
	/// <param name="argument">The argument to consider.</param>
	bool AllowArgument(IFieldType field, QueryArgument argument);

	/// <summary>
	/// Returns a boolean indicating whether the specified enumeration value should be returned within the introspection query.
	/// </summary>
	/// <param name="parent">The enumeration to which the enumeration value belongs.</param>
	/// <param name="enumValue">The enumeration value to consider.</param>
	bool AllowEnumValue(GraphQLEnumType parent, GraphQLEnumType.EnumValue enumValue);

	/// <summary>
	/// Returns a boolean indicating whether the specified directive should be returned within the introspection query.
	/// </summary>
	/// <param name="directive">The directive to consider.</param>
	bool AllowDirective(Directive directive);
}
