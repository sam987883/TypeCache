using GraphQL.Types;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace GraphQL.Introspection;

/// <summary>
/// The <see cref="__DirectiveArgument"/> introspection type represents an argument of
/// a directive applied to a schema element - type, field, argument, etc.
/// <br/><br/>
/// Note that this class describes only explicitly specified arguments. If the argument in the directive
/// definition has default value and this argument was not specified when applying the directive to schema
/// element, then such an argument with default value will not be returned.
/// </summary>
public class __DirectiveArgument : ObjectGraphType<DirectiveArgument>
{
	/// <summary>
	/// Initializes a new instance of this graph type.
	/// </summary>
	public __DirectiveArgument()
	{
		this.SetName(nameof(__DirectiveArgument), validate: false);
		this.Description = "Value of an argument provided to directive";

		this.AddField(new()
		{
			Name = "name",
			Type = typeof(NonNullGraphType<GraphQLStringType>),
			Description = "Argument name",
			Resolver = new CustomFieldResolver(context => ValueTask.FromResult<object>(((DirectiveArgument)context.Source!).Name)!)
		});

		//this.Field<NonNullGraphType<GraphQLStringType>>("name")
		//		.Description("Argument name")
		//		.Resolve(context => context.Source!.Name);

		this.Field<NonNullGraphType<GraphQLStringType>>("value")
			.Description("A GraphQL-formatted string representing the value for argument.")
			.Resolve(context =>
			{
				var argument = context.Source!;
				if (argument.Value is null)
					return "null";

				var grandParent = context.Parent!.Parent!;
				int index = (int)grandParent.Path.Last();
				var appliedDirective = ((IList<AppliedDirective>)grandParent.Source!)[index];
				var directiveDefinition = context.Schema.Directives[appliedDirective.Name];
				var argumentDefinition = directiveDefinition![argument.Name];

				return argumentDefinition!.ResolvedType!.Print(argument.Value);
			});
	}
}
