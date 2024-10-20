using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Variable default values of correct type:
///
/// A GraphQL document is only valid if all variable default values are of the
/// type expected by their definition.
/// </summary>
public class DefaultValuesOfCorrectType : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="DefaultValuesOfCorrectTypeError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new MatchingNodeVisitor<GraphQLVariableDefinition>((varDefAst, context) =>
	{
		var defaultValue = varDefAst.DefaultValue;
		var inputType = context.TypeInfo.GetInputType();

		if (inputType is not null && defaultValue is not null)
		{
			var errors = context.IsValidLiteralValue(inputType, defaultValue);
			if (errors is not null)
				context.ReportError(new DefaultValuesOfCorrectTypeError(context, varDefAst, inputType, errors));
		}
	});
}
