// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Data;

namespace TypeCache.Converters
{
	public class ParameterArrayJsonConverter : JsonConverter<Parameter[]>
	{
		public override Parameter[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var parameters = new List<Parameter>();

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					parameters.Add(new Parameter
					{
						Name = reader.GetString()!,
						Value = reader.Read() ? JsonSerializer.Deserialize<object>(ref reader) : null
					});
				}
			}

			while (reader.TokenType != JsonTokenType.EndObject && reader.Read()) { }

			return parameters.ToArray();
		}

		public override void Write(Utf8JsonWriter writer, Parameter[] parameters, JsonSerializerOptions options)
		{
			if (parameters != null)
			{
				writer.WriteStartArray();
				writer.WriteStartObject();
				parameters.Do(parameter =>
				{
					writer.WritePropertyName(parameter.Name);
					if (parameter.Value != null)
						JsonSerializer.Serialize(writer, parameter.Value);
					else
						writer.WriteNullValue();
				});
				writer.WriteEndObject();
				writer.WriteEndArray();
			}
			else
				writer.WriteNullValue();
		}
	}
}
