// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Data;

namespace TypeCache.Converters
{
	public class ParameterJsonConverter : JsonConverter<Parameter>
	{
		public override Parameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? name = null;
			string? value = null;

			if (reader.TokenType == JsonTokenType.StartObject && reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				name = reader.GetString();
				reader.Read();
				value = reader.GetString();
				reader.Read();
			}

			return new Parameter(name!, value!);
		}

		public override void Write(Utf8JsonWriter writer, Parameter parameter, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(parameter.Name, parameter.Value.ToString());
			writer.WriteEndObject();
		}
	}
}
