// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Converters;

public sealed class DictionaryJsonConverter : JsonConverter<IDictionary<string, object?>>
{
	public override IDictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var dictionary = new Dictionary<string, object?>(STRING_COMPARISON.ToStringComparer());

		if (reader.TokenType is JsonTokenType.StartObject)
		{
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				var name = reader.GetString()!;
				if (reader.Read())
					dictionary.Add(name, reader.GetValue());
			}
		}

		return dictionary;
	}

	public override void Write(Utf8JsonWriter writer, IDictionary<string, object?> dictionary, JsonSerializerOptions options)
	{
		if (dictionary.Any())
		{
			writer.WriteStartObject();
			foreach (var pair in dictionary)
			{
				writer.WritePropertyName(pair.Key);
				writer.WriteValue(pair.Value, options);
			}
			writer.WriteEndObject();
		}
		else
			writer.WriteNullValue();
	}
}

public class DictionaryJsonConverter<T> : JsonConverter<IDictionary<string, T>>
	where T : struct, Enum
{
	public override IDictionary<string, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var dictionary = new Dictionary<string, T>(STRING_COMPARISON.ToStringComparer());

		if (reader.TokenType is JsonTokenType.StartObject)
		{
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				var name = reader.GetString()!;
				if (reader.Read())
					dictionary.Add(name, EnumOf<T>.Member[reader.GetString()!]!.Value);
			}
		}

		return dictionary;
	}

	public override void Write(Utf8JsonWriter writer, IDictionary<string, T> dictionary, JsonSerializerOptions options)
	{
		if (dictionary.Any())
		{
			writer.WriteStartObject();
			foreach (var pair in dictionary)
			{
				writer.WritePropertyName(pair.Key);
				writer.WriteValue(pair.Value, options);
			}
			writer.WriteEndObject();
		}
		else
			writer.WriteNullValue();
	}
}
