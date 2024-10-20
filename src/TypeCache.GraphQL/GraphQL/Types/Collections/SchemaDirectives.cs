using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace GraphQL.Types;

/// <summary>
/// A class that represents a list of directives supported by the schema.
/// </summary>
public sealed class SchemaDirectives : HashSet<Directive>
{
	/// <summary>
	/// Returns an instance of the predefined 'include' directive.
	/// </summary>
	public Directive Include { get; } = new("include", [DirectiveLocation.Field, DirectiveLocation.FragmentSpread, DirectiveLocation.InlineFragment])
	{
		Description = "Directs the executor to include this field or fragment only when the 'if' argument is true.",
		Arguments = [new QueryArgument("if", typeof(bool).ToGraphQLType(false).ToNonNullGraphType()) { Description = "Included when true." }]
	};

	/// <summary>
	/// Returns an instance of the predefined 'skip' directive.
	/// </summary>
	public Directive Skip { get; } = new("skip", [DirectiveLocation.Field, DirectiveLocation.FragmentSpread, DirectiveLocation.InlineFragment])
	{
		Description = "Directs the executor to skip this field or fragment when the 'if' argument is true.",
		Arguments = [new QueryArgument("if", typeof(bool).ToGraphQLType(false).ToNonNullGraphType()) { Description = "Skipped when true." }]
	};

	/// <summary>
	/// Returns an instance of the predefined 'deprecated' directive.
	/// </summary>
	public Directive Deprecated { get; } = new("deprecated", [DirectiveLocation.FieldDefinition, DirectiveLocation.EnumValue])
	{
		Description = "Marks an element of a GraphQL schema as no longer supported.",
		Arguments =
		[
			new QueryArgument("reason", typeof(string).ToGraphQLType(false), "No longer supported")
			{
				Description = "Explains why this element was deprecated, usually also including a suggestion for how to access supported similar data. Formatted in [Markdown](https://daringfireball.net/projects/markdown/)."
			}
		]
	};

	public Directive? this[string name]
		=> this.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(name));
}
