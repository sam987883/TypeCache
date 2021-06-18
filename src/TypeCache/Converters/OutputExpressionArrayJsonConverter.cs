// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Data;

namespace TypeCache.Converters
{
	public class OutputExpressionArrayJsonConverter : JsonConverter<OutputExpression[]>
	{
		public override OutputExpression[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var outputExpressions = new List<OutputExpression>();

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
						outputExpressions.Add(new OutputExpression(reader.GetString()!, name));
				}
			}

			return outputExpressions.ToArray();
		}

		public override void Write(Utf8JsonWriter writer, OutputExpression[] outputExpressions, JsonSerializerOptions options)
		{
			if (outputExpressions.Length > 0)
			{
				writer.WriteStartObject();
				outputExpressions.Do(outputExpression => writer.WriteString(outputExpression.As, outputExpression.Expression));
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
