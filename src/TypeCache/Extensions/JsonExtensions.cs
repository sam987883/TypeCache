// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class JsonExtensions
{
	public static JsonElement[] GetArrayElements(this JsonElement @this)
	{
		@this.ValueKind.AssertEquals(JsonValueKind.Array);

		return @this.EnumerateArrayValues().ToArray();
	}

	public static object?[]? GetArrayValues(this JsonElement @this)
	{
		@this.ValueKind.AssertEquals(JsonValueKind.Array);

		return @this.EnumerateArrayValues().Map(jsonElement => jsonElement.GetValue()).ToArray();
	}

	public static IDictionary<string, JsonElement> GetObjectElements(this JsonElement @this)
	{
		@this.ValueKind.AssertEquals(JsonValueKind.Object);

		var properties = new Dictionary<string, JsonElement>(NAME_STRING_COMPARISON.ToStringComparer());
		using var enumerator = @this.EnumerateObject();
		while (enumerator.MoveNext())
		{
			properties.Add(enumerator.Current.Name, enumerator.Current.Value);
		}
		return properties;
	}

	public static IDictionary<string, object?> GetObjectValues(this JsonElement @this)
	{
		@this.ValueKind.AssertEquals(JsonValueKind.Object);

		var properties = new Dictionary<string, object?>(NAME_STRING_COMPARISON.ToStringComparer());
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
			JsonValueKind.Undefined => throw new NotImplementedException(),
			JsonValueKind.True => true,
			JsonValueKind.False => false,
			JsonValueKind.Number when @this.TryGetInt32(out var value) => value,
			JsonValueKind.Number when @this.TryGetInt64(out var value) => value,
			JsonValueKind.Number => @this.GetDecimal(),
			JsonValueKind.String when @this.TryGetDateTimeOffset(out var value) => value,
			JsonValueKind.String when @this.TryGetDateTime(out var value) => value,
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
			JsonTokenType.String when @this.TryGetDateTimeOffset(out var value) => value,
			JsonTokenType.String when @this.TryGetDateTime(out var value) => value,
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
		switch (value)
		{
			case null or DBNull:
				@this.WriteNullValue();
				break;
			case bool success:
				@this.WriteBooleanValue(success);
				break;
			case sbyte or byte or short or ushort or int or IntPtr:
				@this.WriteNumberValue(Convert.ToInt32(value));
				break;
			case byte or ushort or uint:
				@this.WriteNumberValue(Convert.ToUInt32(value));
				break;
			case long number:
				@this.WriteNumberValue(number);
				break;
			case ulong number:
				@this.WriteNumberValue(number);
				break;
			case float or Half or double:
				@this.WriteNumberValue(Convert.ToDouble(value));
				break;
			case decimal number:
				@this.WriteNumberValue(number);
				break;
			case DateOnly date:
				@this.WriteStringValue(date.ToShortDateString());
				break;
			case DateTime dateTime:
				@this.WriteStringValue(dateTime);
				break;
			case DateTimeOffset dateTimeOffset:
				@this.WriteStringValue(dateTimeOffset);
				break;
			case TimeOnly time:
				@this.WriteStringValue(time.ToShortTimeString());
				break;
			case TimeSpan timeSpan:
				@this.WriteStringValue(timeSpan.ToString("c"));
				break;
			case Guid id:
				@this.WriteStringValue(id);
				break;
			case char or Index or Range or Uri:
				@this.WriteStringValue(value.ToString());
				break;
			case string text:
				@this.WriteStringValue(text);
				break;
			default:
				JsonSerializer.Serialize(@this, value, options);
				break;
		}
	}

	private static IEnumerable<JsonElement> EnumerateArrayValues(this JsonElement @this)
	{
		using var enumerator = @this.EnumerateArray();
		while (enumerator.MoveNext())
			yield return enumerator.Current;
	}
}
