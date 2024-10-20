using TypeCache.Extensions;

namespace GraphQL.Utilities;

/// <summary>
/// Validator for GraphQL names.
/// </summary>
public static class NameValidator
{
	/// <summary>
	/// Validates a specified name.
	/// </summary>
	/// <param name="name">GraphQL name.</param>
	/// <param name="type">Type of element: field, type, argument, enum.</param>
	public static void ValidateName(string name, NamedElement type)
		=> GlobalSwitches.NameValidation(name, type);

	/// <summary>
	/// Validates a specified name during schema initialization.
	/// </summary>
	/// <param name="name">GraphQL name.</param>
	/// <param name="type">Type of element: field, type, argument, enum.</param>
	internal static void ValidateNameOnSchemaInitialize(string name, NamedElement type)
		=> ValidateDefault(name, type);

	/// <summary>
	/// Validates a specified name according to the GraphQL <see href="https://spec.graphql.org/October2021/#sec-Names">specification</see>.
	/// </summary>
	/// <param name="name">GraphQL name.</param>
	/// <param name="type">Type of element: field, type, argument, enum or directive.</param>
	public static void ValidateDefault(string name, NamedElement type)
	{
		name.ThrowIfBlank();

		(name.Length > 1 && name[0] is '_' && name[1] is '_').ThrowIfTrue();

		for (var i = 0; i < name.Length; ++i)
		{
			var c = name[i];
			(!c.IsLetterOrDigit() && c is not '_').ThrowIfTrue();
		}
	}
}
