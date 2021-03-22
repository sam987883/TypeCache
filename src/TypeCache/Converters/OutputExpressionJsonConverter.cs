// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Data;

namespace TypeCache.Converters
{
	public class OutputExpressionJsonConverter : JsonConverter<OutputExpression>
	{
		public override OutputExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? alias = null;
			string? expression = null;

			if (reader.TokenType == JsonTokenType.StartObject && reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				alias = reader.GetString();
				reader.Read();
				expression = reader.GetString();
				reader.Read();
			}

			return new OutputExpression(alias!, expression!);
		}

		public override void Write(Utf8JsonWriter writer, OutputExpression outputExpression, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(outputExpression.As, outputExpression.Expression);
			writer.WriteEndObject();
		}
	}
}
