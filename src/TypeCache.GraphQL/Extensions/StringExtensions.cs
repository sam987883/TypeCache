using System.Text;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Extensions;

/// <summary>
/// Provides extension methods for strings.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Returns a camel case version of the string.
	/// </summary>
	/// <param name="this">The source string.</param>
	/// <returns>System.String.</returns>
	public static string ToCamelCase(this string @this)
	{
		if (@this.IsBlank())
			return string.Empty;

		var firstLetter = @this[0].ToLowerInvariant();
		if (firstLetter == @this[0])
			return @this;

		Span<char> buffer = stackalloc char[@this.Length];
		buffer[0] = firstLetter;
		@this.AsSpan().Slice(1).CopyTo(buffer.Slice(1));

		return buffer.ToString();
	}

	/// <summary>
	/// Returns a pascal case version of the string.
	/// </summary>
	/// <param name="this">The source string.</param>
	/// <returns>System.String.</returns>
	public static string ToPascalCase(this string @this)
	{
		if (@this.IsBlank())
			return string.Empty;

		var firstLetter = @this[0].ToUpperInvariant();
		if (firstLetter == @this[0])
			return @this;

		Span<char> buffer = stackalloc char[@this.Length];
		buffer[0] = firstLetter;
		@this.AsSpan().Slice(1).CopyTo(buffer.Slice(1));

		return buffer.ToString();
	}

	/// <summary>
	/// Returns a constant case version of this string. For example, converts 'StringError' into 'STRING_ERROR'.
	/// </summary>
	public static string ToConstantCase(this string @this)
	{
		var textBuilder = new StringBuilder(@this);

		// Iterate through each character in the string, stopping a character short of the end.
		for (var i = 0; i < textBuilder.Length - 1; ++i)
		{
			var (current, next) = (textBuilder[i], textBuilder[i + 1]);

			if (current.IsLower() && next.IsUpper()      // ([a-z], [A-Z])
				|| current.IsDigit() && next.IsLetter()  // ([0-9], [A-Za-z])
				|| current.IsLetter() && next.IsDigit()) // ([A-Za-z], [0-9])
			{
				textBuilder.Insert(++i, '_');
				continue;
			}
		}

		return textBuilder.ToString().ToUpperInvariant();
	}
}
