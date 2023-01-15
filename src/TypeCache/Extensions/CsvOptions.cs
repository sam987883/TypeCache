// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.Extensions;

public readonly struct CsvOptions
{
	public CsvOptions() { }

	public string ByteFormatSpecifier { get; init; } = "X";

	public string DateOnlyFormatSpecifier { get; init; } = "O";

	public string DateTimeFormatSpecifier { get; init; } = "O";

	public string DateTimeOffsetFormatSpecifier { get; init; } = "O";

	public string DecimalFormatSpecifier { get; init; } = "D";

	public string EnumFormatSpecifier { get; init; } = "D";

	public string FalseText { get; init; } = bool.FalseString;

	public string IntegerFormatSpecifier { get; init; } = "D";

	public string GuidFormatSpecifier { get; init; } = "D";

	/// <summary>
	/// Property/Field name comparison.
	/// </summary>
	public StringComparison MemberNameComparison { get; init; } = StringComparison.Ordinal;

	/// <summary>
	/// Property/Field names to map.
	/// </summary>
	public string[] MemberNames { get; init; } = Array<string>.Empty;

	public string NullText { get; init; } = string.Empty;

	public string TimeOnlyFormatSpecifier { get; init; } = "O";

	public string TimeSpanFormatSpecifier { get; init; } = "c";

	public string TrueText { get; init; } = bool.TrueString;
}
