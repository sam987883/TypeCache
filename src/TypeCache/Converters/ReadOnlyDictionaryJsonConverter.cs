// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class ReadOnlyDictionaryJsonConverter : JsonConverter<IReadOnlyDictionary<string, object?>>
{
	public override IReadOnlyDictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

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

	public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, object?> dictionary, JsonSerializerOptions options)
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

public class ReadOnlyDictionaryJsonConverter<T> : JsonConverter<IReadOnlyDictionary<string, T>>
	where T : struct, Enum
{
	public override IReadOnlyDictionary<string, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var dictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

		if (reader.TokenType is JsonTokenType.StartObject)
		{
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				var name = reader.GetString()!;
				if (reader.Read())
				{
					var tokenName = reader.GetString()!;
					dictionary.Add(name, Enum.Parse<T>(tokenName, true));
				}
			}
		}

		return dictionary;
	}

	public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, T> dictionary, JsonSerializerOptions options)
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
