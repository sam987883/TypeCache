// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TypeCache.Extensions;

public static class JsonExtensions
{
	public static JsonElement[] GetArrayElements(this JsonElement @this)
	{
		@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Array);

		return @this.EnumerateArrayValues().ToArray();
	}

	public static object?[] GetArrayValues(this JsonElement @this)
	{
		@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Array);

		return @this.EnumerateArrayValues().Select(jsonElement => jsonElement.GetValue()).ToArray();
	}

	public static IDictionary<string, JsonElement> GetObjectElements(this JsonElement @this)
	{
		@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Object);

		var properties = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
		using var enumerator = @this.EnumerateObject();
		while (enumerator.MoveNext())
		{
			properties.Add(enumerator.Current.Name, enumerator.Current.Value);
		}

		return properties;
	}

	public static IDictionary<string, object?> GetObjectValues(this JsonElement @this)
	{
		@this.ValueKind.ThrowIfNotEqual(JsonValueKind.Object);

		var properties = new Dictionary<string, object?>(StringComparer.Ordinal);
		using var enumerator = @this.EnumerateObject();
		while (enumerator.MoveNext())
		{
			properties.Add(enumerator.Current.Name, enumerator.Current.Value.GetValue());
		}

		return properties;
	}

	public static object? GetValue(this JsonElement @this)
		=> @this.ValueKind switch
		{
			JsonValueKind.Undefined => throw new UnreachableException(Invariant($"{nameof(@this.ValueKind)} is {JsonValueKind.Undefined.Name()}")),
			JsonValueKind.True => true,
			JsonValueKind.False => false,
			JsonValueKind.Number when @this.TryGetInt32(out var value) => value,
			JsonValueKind.Number when @this.TryGetInt64(out var value) => value,
			JsonValueKind.Number => @this.GetDecimal(),
			JsonValueKind.String when @this.TryGetDateTime(out var value) => value,
			JsonValueKind.String when @this.TryGetDateTimeOffset(out var value) => value,
			JsonValueKind.String when @this.TryGetGuid(out var value) => value,
			JsonValueKind.String => @this.GetString()!,
			JsonValueKind.Array => @this.GetArrayValues(),
			JsonValueKind.Object => @this.GetObjectValues(),
			_ => null
		};

	public static object? GetValue(this ref Utf8JsonReader @this)
		=> @this.TokenType switch
		{
			JsonTokenType.True => true,
			JsonTokenType.False => false,
			JsonTokenType.Number when @this.TryGetInt32(out var value) => value,
			JsonTokenType.Number when @this.TryGetInt64(out var value) => value,
			JsonTokenType.Number => @this.GetDecimal(),
			JsonTokenType.String when @this.TryGetDateTime(out var value) => value,
			JsonTokenType.String when @this.TryGetDateTimeOffset(out var value) => value,
			JsonTokenType.String when @this.TryGetGuid(out var value) => value,
			JsonTokenType.String => @this.GetString(),
			_ => null
		};

	public static void ReadToEnd(this ref Utf8JsonReader @this)
	{
		while (@this.Read()) { }
	}

	public static bool ReadUntil(this ref Utf8JsonReader @this, JsonTokenType tokenType)
	{
		while (@this.Read())
			if (@this.TokenType == tokenType)
				return true;

		return false;
	}

	public static T[]? ToArray<T>(this JsonArray @this, JsonSerializerOptions? options = null)
		=> JsonSerializer.Deserialize<T[]?>(@this, options);

	public static void WriteValue(this Utf8JsonWriter @this, object? value, JsonSerializerOptions options)
	{
		Action writeValue = value switch
		{
			null or DBNull => @this.WriteNullValue,
			bool success => () => @this.WriteBooleanValue(success),
			sbyte number => () => @this.WriteNumberValue((int)number),
			short number => () => @this.WriteNumberValue((int)number),
			int number => () => @this.WriteNumberValue(number),
			long number => () => @this.WriteNumberValue(number),
			Int128 number => () => @this.WriteNumberValue((decimal)number),
			BigInteger number => () => @this.WriteNumberValue((decimal)number),
			nint number => () => @this.WriteNumberValue((long)number),
			byte number => () => @this.WriteNumberValue((uint)number),
			ushort number => () => @this.WriteNumberValue((uint)number),
			uint number => () => @this.WriteNumberValue(number),
			ulong number => () => @this.WriteNumberValue(number),
			UInt128 number => () => @this.WriteNumberValue((decimal)number),
			nuint number => () => @this.WriteNumberValue((ulong)number),
			Half number => () => @this.WriteNumberValue((float)number),
			float number => () => @this.WriteNumberValue(number),
			double number => () => @this.WriteNumberValue(number),
			decimal number => () => @this.WriteNumberValue(number),
			DateOnly dateOnly => () => @this.WriteStringValue(dateOnly.ToISO8601()),
			DateTime dateTime => () => @this.WriteStringValue(dateTime.ToISO8601()),
			DateTimeOffset dateTimeOffset => () => @this.WriteStringValue(dateTimeOffset.ToISO8601()),
			TimeOnly timeOnly => () => @this.WriteStringValue(timeOnly.ToISO8601()),
			TimeSpan timeSpan => () => @this.WriteStringValue(timeSpan.ToText()),
			Guid id => () => @this.WriteStringValue(id),
			char or Index or Range or Uri => () => @this.WriteStringValue(value.ToString()),
			string text => () => @this.WriteStringValue(text),
			_ => () => JsonSerializer.Serialize(@this, value, options)
		};
		writeValue();
	}

	private static IEnumerable<JsonElement> EnumerateArrayValues(this JsonElement @this)
	{
		using var enumerator = @this.EnumerateArray();
		while (enumerator.MoveNext())
			yield return enumerator.Current;
	}
}
