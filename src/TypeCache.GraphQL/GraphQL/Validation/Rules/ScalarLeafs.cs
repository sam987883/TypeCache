using GraphQL.Types;
using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Scalar leafs:
///
/// A GraphQL document is valid only if all leaf fields (fields without
/// sub selections) are of scalar or enum types.
/// </summary>
public class ScalarLeafs : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="ScalarLeafsError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context)
		=> new(new MatchingNodeVisitor<GraphQLField>((field, context) => Field(context.TypeInfo.GetLastType(), field, context)));

	private static void Field(IGraphType? type, GraphQLField field, ValidationContext context)
	{
		if (type is null)
			return;

		if (type.IsLeafType())
		{
			if (field.SelectionSet?.Selections.Count > 0)
				context.ReportError(new ScalarLeafsError(context, field.SelectionSet, field, type));
		}
		else if (field.SelectionSet is null || field.SelectionSet.Selections.Count == 0)
			context.ReportError(new ScalarLeafsError(context, field, type));
	}
}
