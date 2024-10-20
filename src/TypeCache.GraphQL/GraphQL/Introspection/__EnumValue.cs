using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// The <see cref="__EnumValue"/> introspection type represents one of possible values of an enum.
/// </summary>
public class __EnumValue : ObjectGraphType<GraphQLEnumType.EnumValue>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="__EnumValue"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	public __EnumValue(bool allowAppliedDirectives = false)
	{
		this.SetName(nameof(__EnumValue), validate: false);
		this.Description = "One possible value for a given Enum. Enum values are unique values, not a placeholder for a string or numeric value. However an Enum value is returned in a JSON response as a string.";

		this.Field<NonNullGraphType<GraphQLStringType>>("name").Resolve(context => context.Source!.Name);
		this.Field<GraphQLStringType>("description").Resolve(context => context.Source!.Description);

		this.Field<NonNullGraphType<GraphQLBooleanType>>("isDeprecated").Resolve(context => context.Source!.DeprecationReason.IsNotBlank().Box());
		this.Field<GraphQLStringType>("deprecationReason").Resolve(context => context.Source!.DeprecationReason);

		if (allowAppliedDirectives)
			this.AddAppliedDirectivesField("enum value");
	}
}
