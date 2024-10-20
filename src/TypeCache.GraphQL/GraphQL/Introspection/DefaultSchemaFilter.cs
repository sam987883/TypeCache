using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// The default schema filter. By default nothing is hidden. Please note
/// that some features that are not in the official specification may be
/// hidden by default. These features can be unlocked using special
/// <see cref="ExperimentalIntrospectionFeaturesSchemaFilter"/> filter.
/// </summary>
public class DefaultSchemaFilter : ISchemaFilter
{
	/// <inheritdoc/>
	public virtual bool AllowType(IGraphType type) =>
		type is not __AppliedDirective && type is not __DirectiveArgument;

	/// <inheritdoc/>
	public virtual bool AllowField(IGraphType parent, IFieldType field)
		=> !(parent.IsIntrospectionType() && (field.Name == "appliedDirectives" || field.Name == "isRepeatable"));

	/// <inheritdoc/>
	public virtual bool AllowArgument(IFieldType field, QueryArgument argument)
		=> true;

	/// <inheritdoc/>
	public virtual bool AllowEnumValue(GraphQLEnumType parent, GraphQLEnumType.EnumValue enumValue)
		=> true;

	/// <inheritdoc/>
	public virtual bool AllowDirective(Directive directive)
	{
		if (directive.Introspectable.HasValue)
			return directive.Introspectable.Value;

		// If the directive has all its locations of type ExecutableDirectiveLocation,
		// only then it will be present in the introspection response.
		foreach (var location in directive.Locations)
		{
			if (!(location is DirectiveLocation.Query
				|| location is DirectiveLocation.Mutation
				|| location is DirectiveLocation.Subscription
				|| location is DirectiveLocation.Field
				|| location is DirectiveLocation.FragmentDefinition
				|| location is DirectiveLocation.FragmentSpread
				|| location is DirectiveLocation.InlineFragment
				|| location is DirectiveLocation.VariableDefinition))
			{
				return false;
			}
		}

		return true;
	}
}
