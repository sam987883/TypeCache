using GraphQL.Types;
using GraphQL.Utilities;
using GraphQL.Validation.Errors;
using GraphQLParser.AST;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Fields on correct type:
///
/// A GraphQL document is only valid if all fields selected are defined by the
/// parent type, or are an allowed meta field such as __typename.
/// </summary>
public class FieldsOnCorrectType : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="FieldsOnCorrectTypeError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context) => new(_nodeVisitor);

	private static readonly INodeVisitor _nodeVisitor = new MatchingNodeVisitor<GraphQLField>((node, context) =>
	{
		var type = context.TypeInfo.GetParentType()?.GetNamedGraphType();
		if (type is not null)
		{
			var fieldDef = context.TypeInfo.GetFieldDef();
			if (fieldDef is null)
			{
				// This field doesn't exist, lets look for suggestions.
				var fieldName = node.Name;

				// First determine if there are any suggested types to condition on.
				var suggestedTypeNames = GetSuggestedTypeNames(type, fieldName.StringValue).ToList(); //ISSUE:allocation

				// If there are no suggested types, then perhaps this was a typo?
				var suggestedFieldNames = suggestedTypeNames.Count > 0
					? Array.Empty<string>()
					: GetSuggestedFieldNames(type, fieldName.StringValue); //ISSUE:allocation

				// Report an error, including helpful suggestions.
				context.ReportError(new FieldsOnCorrectTypeError(context, node, type, suggestedTypeNames, suggestedFieldNames));
			}
		}
	});

	/// <summary>
	/// Go through all of the implementations of type, as well as the interfaces
	/// that they implement. If any of those types include the provided field,
	/// suggest them, sorted by how often the type is referenced,  starting
	/// with Interfaces.
	/// </summary>
	private static IEnumerable<string> GetSuggestedTypeNames(IGraphType type, string fieldName)
	{
		if (type is IAbstractGraphType absType)
		{
			var suggestedObjectTypes = new List<string>();
			var interfaceUsageCount = new Dictionary<string, int>();

			foreach (var possibleType in absType.PossibleTypes)
			{
				if (possibleType[fieldName] is not null)
				{
					// This object defines this field.
					suggestedObjectTypes.Add(possibleType.Name);

					if (possibleType is ObjectGraphType graphType)
					{
						foreach (var possibleInterface in graphType.ResolvedInterfaces)
						{
							if (possibleInterface[fieldName] is not null)
							{
								// This interface type defines this field.
								interfaceUsageCount[possibleInterface.Name] = interfaceUsageCount.TryGetValue(possibleInterface.Name, out int value) ? value + 1 : 1;
							}
						}
					}
				}
			}

			var suggestedInterfaceTypes = interfaceUsageCount.Keys.OrderBy(_ => interfaceUsageCount[_]);
			return suggestedInterfaceTypes.Concat(suggestedObjectTypes);
		}

		return Enumerable.Empty<string>();
	}

	/// <summary>
	/// For the field name provided, determine if there are any similar field names
	/// that may be the result of a typo.
	/// </summary>
	private static IEnumerable<string> GetSuggestedFieldNames(IGraphType type, string fieldName)
		=> type is IObjectGraphType complexType
			? StringUtils.SuggestionList(fieldName, complexType.Fields.Select(_ => _.Name))
			: Enumerable.Empty<string>();
}
