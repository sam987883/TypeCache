using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Argument values of correct type:
///
/// A GraphQL document is only valid if all field argument literal values are
/// of the type expected by their position.
/// </summary>
public class ArgumentsOfCorrectType : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="ArgumentsOfCorrectTypeError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new MatchingNodeVisitor<GraphQLArgument>((argAst, context) =>
	{
		var type = context.TypeInfo.GetArgument()?.ResolvedType;
		if (type is null)
			return;

		var errors = context.IsValidLiteralValue(type, argAst.Value);
		if (errors is not null)
			context.ReportError(new ArgumentsOfCorrectTypeError(context, argAst, errors));
	});
}
