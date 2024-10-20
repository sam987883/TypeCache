using GraphQL.Types;
using GraphQL.Validation.Errors;
using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Possible fragment spread:
///
/// A fragment spread is only valid if the type condition could ever possibly
/// be <see langword="true"/>: if there is a non-empty intersection of the
/// possible parent types, and possible types which pass the type condition.
/// </summary>
public class PossibleFragmentSpreads : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="PossibleFragmentSpreadsError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context)
		=> new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new NodeVisitors(
		new MatchingNodeVisitor<GraphQLInlineFragment>((node, context) =>
		{
			var fragType = context.TypeInfo.GetLastType();
			var parentType = context.TypeInfo.GetParentType()?.GetNamedGraphType();

			if (fragType is not null && parentType is not null && !GraphQLExtensions.DoTypesOverlap(fragType, parentType))
				context.ReportError(new PossibleFragmentSpreadsError(context, node, parentType, fragType));
		}),
		new MatchingNodeVisitor<GraphQLFragmentSpread>((node, context) =>
		{
			var fragName = node.FragmentName.Name;
			var fragDef = context.Document.FindFragmentDefinition(fragName);
			var fragType = fragDef?.TypeCondition.Type.GraphTypeFromType(context.Schema);
			var parentType = context.TypeInfo.GetParentType()?.GetNamedGraphType();

			if (fragType is not null && parentType is not null && !GraphQLExtensions.DoTypesOverlap(fragType, parentType))
				context.ReportError(new PossibleFragmentSpreadsError(context, node, parentType, fragType));
		})
	);
}
