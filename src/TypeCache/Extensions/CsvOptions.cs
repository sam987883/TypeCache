// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections;

namespace TypeCache.Extensions;

public struct CsvOptions
{
	public CsvOptions() { }

	public string ByteFormatSpecifier { get; set; } = "X";

	public string DateOnlyFormatSpecifier { get; set; } = "O";

	public string DateTimeFormatSpecifier { get; set; } = "O";

	public string DateTimeOffsetFormatSpecifier { get; set; } = "O";

	public string DecimalFormatSpecifier { get; set; } = "D";

	public string EnumFormatSpecifier { get; set; } = "D";

	public string FalseText { get; set; } = bool.FalseString;

	public string IntegerFormatSpecifier { get; set; } = "D";

	public string GuidFormatSpecifier { get; set; } = "D";

	/// <summary>
	/// Property/Field name comparison.
	/// </summary>
	public StringComparison MemberNameComparison { get; set; } = StringComparison.Ordinal;

	/// <summary>
	/// Property/Field names to map.
	/// </summary>
	public string[] MemberNames { get; set; } = Array<string>.Empty;

	public string NullText { get; set; } = string.Empty;

	public string TimeOnlyFormatSpecifier { get; set; } = "O";

	public string TimeSpanFormatSpecifier { get; set; } = "c";

	public string TrueText { get; set; } = bool.TrueString;
}
