// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;
using static TypeCache.Default;

namespace TypeCache.Data.Extensions;

public static class SqlExtensions
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string EscapeIdentifier([NotNull] this string @this)
		=> Invariant($"[{@this.EscapeValue().Replace("]", "]]")}]");

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string EscapeLikeValue([NotNull] this string @this)
		=> @this.EscapeValue().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string EscapeValue([NotNull] this string @this)
		=> @this.Replace("'", "''");

	public static string ToSQL(this object? @this) => @this switch
	{
		null or DBNull => "NULL",
		bool boolean => boolean ? "1" : "0",
		char text => text.Equals('\'') ? "N''''" : Invariant($"N'{text}'"),
		string text => Invariant($"N'{text.EscapeValue()}'"),
		DateTime dateTime => Invariant($"'{dateTime:o}'"),
		DateTimeOffset dateTimeOffset => Invariant($"'{dateTimeOffset:o}'"),
		TimeSpan time => Invariant($"'{time:c}'"),
		Guid guid => Invariant($"'{guid:D}'"),
		LogicalOperator.And => "AND",
		LogicalOperator.Or => "OR",
		LogicalOperator _ => throw new NotImplementedException(Invariant($"{nameof(LogicalOperator)}.{@this} is not implemented for SQL.")),
		Sort.Ascending => "ASC",
		Sort.Descending => "DESC",
		Sort _ => string.Empty,
		Enum token => token.ToString("D"),
		Index index => index.Value.ToString(),
		JsonArray json => json.ToJsonString().EscapeValue(),
		JsonObject json => json.ToJsonString().EscapeValue(),
		JsonNode json => json.ToString().Replace("\"", string.Empty).EscapeValue(),
		JsonElement json => json.ValueKind switch
		{
			JsonValueKind.String => Invariant($"N'{json.GetString()!.EscapeValue()}'"),
			JsonValueKind.Number => json.ToString()!,
			JsonValueKind.True => "1",
			JsonValueKind.False => "0",
			JsonValueKind.Null => "NULL",
			_ => Invariant($"N'{json.ToString()!.EscapeValue()}'")
		},
		Range range => Invariant($"'{range}'"),
		Uri uri => Invariant($"'{uri.ToString().EscapeValue()}'"),
		byte[] binary => Invariant($"0x{binary.ToHex()}"),
		IEnumerable enumerable => Invariant($"({enumerable.As<object>().Map(_ => _.ToSQL()).Join(", ")})"),
		_ => @this.ToString() ?? "NULL"
	};
}
