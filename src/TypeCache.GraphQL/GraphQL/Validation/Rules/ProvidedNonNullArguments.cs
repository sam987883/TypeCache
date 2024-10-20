using GraphQL.Types;
using GraphQL.Validation.Errors;
using GraphQLParser.AST;
using TypeCache.Extensions;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Provided required arguments:
///
/// A field or directive is only valid if all required (non-null) field arguments
/// have been provided.
/// </summary>
public class ProvidedNonNullArguments : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="ProvidedNonNullArgumentsError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new NodeVisitors(
		new MatchingNodeVisitor<GraphQLField>(leave: (node, context) =>
		{
			var fieldDef = context.TypeInfo.GetFieldDef();
			if (fieldDef?.Arguments.Length > 0)
			{
				foreach (var arg in fieldDef.Arguments)
				{
					if (arg.DefaultValue is null &&
						arg.ResolvedType is NonNullGraphType &&
						node.Arguments?.ValueFor(arg.Name) is null)
					{
						context.ReportError(new ProvidedNonNullArgumentsError(context, node, arg));
					}
				}
			}
		}),
		new MatchingNodeVisitor<GraphQLDirective>(leave: (node, context) =>
		{
			var directive = context.TypeInfo.GetDirective();
			directive?.Arguments.ForEach(_ =>
			{
				var argAst = node.Arguments?.ValueFor(_.Name);
				if (argAst is null && _.ResolvedType is NonNullGraphType)
					context.ReportError(new ProvidedNonNullArgumentsError(context, node, _));
			});
		})
	);
}
