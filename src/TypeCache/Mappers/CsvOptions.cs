// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mappers;

public struct CsvOptions
{
	public string ByteFormatSpecifier { get; set; } = "X";

	public string DateOnlyFormatSpecifier { get; set; } = "o";

	public string DateTimeFormatSpecifier { get; set; } = "o";

	public string DateTimeOffsetFormatSpecifier { get; set; } = "o";

	public string DecimalFormatSpecifier { get; set; } = "D";

	public string EnumFormatSpecifier { get; set; } = "D";

	public string FalseText { get; set; } = bool.FalseString;

	public string IntegerFormatSpecifier { get; set; } = "D";

	public string GuidFormatSpecifier { get; set; } = "D";

	public string NullText { get; set; } = string.Empty;

	public string TimeOnlyFormatSpecifier { get; set; } = "o";

	public string TimeSpanFormatSpecifier { get; set; } = "c";

	public string TrueText { get; set; } = bool.TrueString;
}
