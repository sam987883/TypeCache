using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// The <see cref="__Directive"/> introspection type represents a directive that a server supports.
/// </summary>
public class __Directive : ObjectGraphType<Directive>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="__Directive"/> introspection type.
	/// </summary>
	/// <param name="allowAppliedDirectives">Allows 'appliedDirectives' field for this type. It is an experimental feature.</param>
	/// <param name="allowRepeatable">Allows 'isRepeatable' field for this type. This feature is from a working draft of the specification.</param>
	public __Directive(bool allowAppliedDirectives = false, bool allowRepeatable = false)
	{
		this.SetName(nameof(__Directive), validate: false);

		this.Description = @"A Directive provides a way to describe alternate runtime execution and
			type validation behavior in a GraphQL document.

			In some cases, you need to provide options to alter GraphQL's
			execution behavior in ways field arguments will not suffice, such as
			conditionally including or skipping a field. Directives provide this by
			describing additional information to the executor.";

		this.Fields.Add(new()
		{
			Name = "name",
			Type = typeof(NonNullGraphType<GraphQLStringType>),
			Resolver = new CustomFieldResolver(context => (object)((Directive)context.Source!).Name)
		});

		//Field<NonNullGraphType<GraphQLStringType>>("name")
		//	.Resolve(context => context.Source!.Name);

		Field<GraphQLStringType>("description")
			.Resolve(context => context.Source!.Description);

		Field<NonNullGraphType<ListGraphType<NonNullGraphType<GraphQLEnumType<DirectiveLocation>>>>>("locations")
			.Resolve(context => context.Source!.Locations);

		Field<NonNullGraphType<ListGraphType<NonNullGraphType<__InputValue>>>>("args")
			.Resolve(context => context.Source!.Arguments);

		if (allowRepeatable)
			Field<NonNullGraphType<GraphQLBooleanType>>("isRepeatable")
				.Resolve(context => context.Source!.Repeatable);

		Field<NonNullGraphType<GraphQLBooleanType>>("onOperation").DeprecationReason("Use 'locations'.")
			.Resolve(context => context.Source!.Locations.Any(location =>
				location is DirectiveLocation.Query
				|| location is DirectiveLocation.Mutation
				|| location is DirectiveLocation.Subscription));

		Field<NonNullGraphType<GraphQLBooleanType>>("onFragment").DeprecationReason("Use 'locations'.")
			.Resolve(context => context.Source!.Locations.Any(location =>
				location is DirectiveLocation.FragmentSpread
				|| location is DirectiveLocation.InlineFragment
				|| location is DirectiveLocation.FragmentDefinition));

		Field<NonNullGraphType<GraphQLBooleanType>>("onField").DeprecationReason("Use 'locations'.")
			.Resolve(context => context.Source!.Locations.Any(location => location is DirectiveLocation.Field));

		if (allowAppliedDirectives)
			this.AddAppliedDirectivesField("directive");
	}
}
