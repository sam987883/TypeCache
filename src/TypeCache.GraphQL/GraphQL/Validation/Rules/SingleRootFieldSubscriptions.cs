using GraphQL.Validation.Errors;
using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Subscription operations must have exactly one root field.
/// </summary>
public class SingleRootFieldSubscriptions : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="SingleRootFieldSubscriptionsError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new MatchingNodeVisitor<GraphQLOperationDefinition>((operation, context) =>
	{
		if (operation.Operation is not OperationType.Subscription)
			return;

		var rootFields = operation.SelectionSet.Selections;
		if (rootFields.Count != 1)
			context.ReportError(new SingleRootFieldSubscriptionsError(context, operation, rootFields.Skip(1).ToArray()));

		if (rootFields[0] is GraphQLField field && isIntrospectionField(field))
			context.ReportError(new SingleRootFieldSubscriptionsError(context, operation, field));

		var fragment = operation.SelectionSet.Selections.Find(node => node is GraphQLFragmentSpread || node is GraphQLInlineFragment);
		if (fragment is null)
			return;

		if (fragment is GraphQLFragmentSpread fragmentSpread)
		{
			var fragmentDefinition = context.Document.FindFragmentDefinition(fragmentSpread.FragmentName.Name);
			if (fragmentDefinition is null)
				return;

			rootFields = fragmentDefinition.SelectionSet.Selections;
		}
		else if (fragment is GraphQLInlineFragment inlineFragment)
			rootFields = inlineFragment.SelectionSet.Selections;

		if (rootFields.Count != 1
			|| (rootFields[0] is GraphQLField field2 && isIntrospectionField(field2)))
			context.ReportError(new SingleRootFieldSubscriptionsError(context, operation, fragment));

		static bool isIntrospectionField(GraphQLField field)
			=> field.Name.Value.Length >= 2 && field.Name.Value.Span.StartsWith("__");
	});
}
