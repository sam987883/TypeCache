using System.Threading.Tasks;
using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Fragments on composite type:
///
/// Fragments use a type condition to determine if they apply, since fragments
/// can only be spread into a composite type (object, interface, or union), the
/// type condition must also be a composite type.
/// </summary>
public class FragmentsOnCompositeTypes : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="FragmentsOnCompositeTypesError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new NodeVisitors(
		new MatchingNodeVisitor<GraphQLInlineFragment>((node, context) =>
		{
			var type = context.TypeInfo.GetLastType();
			if (node.TypeCondition?.Type is not null && type is not null && !type.IsCompositeType())
				context.ReportError(new FragmentsOnCompositeTypesError(context, node));
		}),
		new MatchingNodeVisitor<GraphQLFragmentDefinition>((node, context) =>
		{
			var type = context.TypeInfo.GetLastType();
			if (type is not null && !type.IsCompositeType())
				context.ReportError(new FragmentsOnCompositeTypesError(context, node));
		})
	);
}
