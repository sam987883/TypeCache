// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Converters;

public class ValueJsonConverter : JsonConverter<object?>
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetValue();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, value, options);

	private static IDictionary<string, object> GetObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		var dictionary = new Dictionary<string, object>(STRING_COMPARISON.ToStringComparer());
		while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
		{
			var name = reader.GetString();
			if (reader.Read())
			{
				var value = JsonSerializer.Deserialize<object>(ref reader, options);
				dictionary.Add(name!, value!);
			}
		}
		return dictionary;
	}

	private static object[] GetArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		var list = new List<object>();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			var value = JsonSerializer.Deserialize<object>(ref reader, options);
			list.Add(value!);
		}
		return list.ToArray();
	}
}
