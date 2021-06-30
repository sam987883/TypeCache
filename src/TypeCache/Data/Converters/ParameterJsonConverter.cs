// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data.Converters
{
	public class ParameterJsonConverter : JsonConverter<IDictionary<string, object?>>
	{
		public override IDictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
						parameters.Add(name, reader.GetString());
				}
			}

			return parameters;
		}

		public override void Write(Utf8JsonWriter writer, IDictionary<string, object?> parameters, JsonSerializerOptions options)
		{
			if (parameters.Any())
			{
				writer.WriteStartObject();
				parameters.Do(_ => writer.WriteString(_.Key, _.Value?.ToString()));
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
