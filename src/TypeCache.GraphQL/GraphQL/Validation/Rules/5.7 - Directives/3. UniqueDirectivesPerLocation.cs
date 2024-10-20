using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Unique directive names per location:
///
/// A GraphQL document is only valid if all not repeatable directives
/// at a given location are uniquely named.
/// </summary>
public class UniqueDirectivesPerLocation : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="UniqueDirectivesPerLocationError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new NodeVisitors(
		new MatchingNodeVisitor<GraphQLOperationDefinition>((_, context) => CheckDuplicates(context, _.Directives)),
		new MatchingNodeVisitor<GraphQLField>((_, context) => CheckDuplicates(context, _.Directives)),
		new MatchingNodeVisitor<GraphQLFragmentDefinition>((_, context) => CheckDuplicates(context, _.Directives)),
		new MatchingNodeVisitor<GraphQLFragmentSpread>((_, context) => CheckDuplicates(context, _.Directives)),
		new MatchingNodeVisitor<GraphQLInlineFragment>((_, context) => CheckDuplicates(context, _.Directives)),
		new MatchingNodeVisitor<GraphQLVariableDefinition>((_, context) => CheckDuplicates(context, _.Directives))
	);

	private static void CheckDuplicates(ValidationContext context, GraphQLDirectives? directives)
	{
		if (directives?.Count > 0)
		{
			foreach (var directive in directives)
			{
				var directiveDef = context.Schema.Directives[directive.Name.StringValue];
				if (directiveDef is not null && !directiveDef.Repeatable && directives.Count(_ => _.Name == directive.Name) > 1)
					context.ReportError(new UniqueDirectivesPerLocationError(context, directive));
			}
		}
	}
}
