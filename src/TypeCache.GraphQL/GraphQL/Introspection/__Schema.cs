using GraphQL.Types;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// The <see cref="__Schema"/> introspection type allows querying the schema for available types and directives.
/// </summary>
public class __Schema : ObjectGraphType<ISchema>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="__Schema"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	public __Schema(bool allowAppliedDirectives = false)
	{
		this.SetName(nameof(__Schema), validate: false);

		this.Description = "A GraphQL Schema defines the capabilities of a GraphQL server. It exposes all available types and directives on the server, as well as the entry points for query, mutation, and subscription operations.";

		this.Field<GraphQLStringType>("description")
			.Resolve(context => context.Schema.Description);

		this.Field<NonNullGraphType<ListGraphType<NonNullGraphType<__Type>>>>("types")
			.Description("A list of all types supported by this server.")
			.Resolve(context => context.Schema.AllTypes.Dictionary
				.Where(_ => context.Schema.Filter.AllowType(_.Value))
				.Select(_ => _.Value)
				.OrderBy(_ => _.Name)
				.ToArray());

		this.Field<NonNullGraphType<__Type>>("queryType")
			.Description("The type that query operations will be rooted at.")
			.Resolve(context => context.Schema.Query);

		this.Field<__Type>("mutationType")
			.Description("If this server supports mutation, the type that mutation operations will be rooted at.")
			.Resolve(context => context.Schema.Mutation is not null && context.Schema.Filter.AllowType(context.Schema.Mutation) ? context.Schema.Mutation : null);

		this.Field<__Type>("subscriptionType")
			.Description("If this server supports subscription, the type that subscription operations will be rooted at.")
			.Resolve(context => context.Schema.Subscription is not null && context.Schema.Filter.AllowType(context.Schema.Subscription) ? context.Schema.Subscription : null);

		this.Field<NonNullGraphType<ListGraphType<NonNullGraphType<__Directive>>>>("directives")
			.Description("A list of all directives supported by this server.")
			.Resolve(context => context.Schema.Directives
				.Where(context.Schema.Filter.AllowDirective)
				.OrderBy(_ => _.Name)
				.ToArray());

		if (allowAppliedDirectives)
			this.AddAppliedDirectivesField("schema");
	}
}
