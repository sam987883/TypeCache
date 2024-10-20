﻿using GraphQL.Validation.Errors;
using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Unique fragment names:
///
/// A GraphQL document is only valid if all defined fragments have unique names.
/// </summary>
public class UniqueFragmentNames : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="UniqueFragmentNamesError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(context.Document.FragmentsCount() > 1 ? _nodeVisitor : null);

	private static readonly INodeVisitor _nodeVisitor = new MatchingNodeVisitor<GraphQLFragmentDefinition>((fragmentDefinition, context) =>
	{
		var knownFragments = context.TypeInfo.UniqueFragmentNames_KnownFragments ??= new();

		var fragmentName = fragmentDefinition.FragmentName.Name;
		if (!knownFragments.TryAdd(fragmentName, fragmentDefinition))
			context.ReportError(new UniqueFragmentNamesError(context, knownFragments[fragmentName], fragmentDefinition));
	});
}
