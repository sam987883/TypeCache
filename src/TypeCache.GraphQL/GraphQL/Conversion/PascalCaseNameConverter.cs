using GraphQL.Types;
using TypeCache.GraphQL.Extensions;

namespace GraphQL.Conversion;

/// <summary>
/// PascalCase name converter.
/// </summary>
public class PascalCaseNameConverter : INameConverter
{
	/// <summary>
	/// Static instance of <see cref="PascalCaseNameConverter"/> that can be reused instead of creating new.
	/// </summary>
	public static readonly PascalCaseNameConverter Instance = new();

	/// <summary>
	/// Returns the field name converted to PascalCase.
	/// </summary>
	public string NameForField(string fieldName, IObjectGraphType parentGraphType) => fieldName.ToPascalCase();

	/// <summary>
	/// Returns the argument name converted to PascalCase.
	/// </summary>
	public string NameForArgument(string argumentName, IObjectGraphType parentGraphType, IFieldType field) => argumentName.ToPascalCase();
}
