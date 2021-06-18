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
					var name = reader.GetString()!;
					if (reader.Read())
						parameters.Add(new Parameter(name, reader.GetString()!));
				}
			}

			return parameters.ToArray();
		}

		public override void Write(Utf8JsonWriter writer, Parameter[] parameters, JsonSerializerOptions options)
		{
			if (parameters.Length > 0)
			{
				writer.WriteStartObject();
				parameters.Do(parameter => writer.WriteString(parameter.Name, parameter.Value?.ToString()));
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
