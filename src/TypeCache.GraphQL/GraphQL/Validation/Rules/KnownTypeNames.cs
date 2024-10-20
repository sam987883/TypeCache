using GraphQL.Utilities;
using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Known type names:
///
/// A GraphQL document is only valid if referenced types (specifically
/// variable definitions and fragment conditions) are defined by the type schema.
/// </summary>
public class KnownTypeNames : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="KnownTypeNamesError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new MatchingNodeVisitor<GraphQLNamedType>(leave: (node, context) =>
	{
		var type = context.Schema.AllTypes[node.Name];
		if (type is null)
		{
			var typeNames = context.Schema.AllTypes.Dictionary.Values.Select(_ => _.Name).ToArray();
			var suggestionList = StringUtils.SuggestionList(node.Name.StringValue, typeNames); //ISSUE:allocation
			context.ReportError(new KnownTypeNamesError(context, node, suggestionList));
		}
	});
}
