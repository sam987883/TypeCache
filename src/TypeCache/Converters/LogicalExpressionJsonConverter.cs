// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class LogicalExpressionJsonConverter : JsonConverter<LogicalExpression?>
	{
		public override LogicalExpression? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null)
				return null;
			else if (reader.TokenType == JsonTokenType.StartObject &&
				reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				var name = reader.GetString();
				if (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
				{
					var comparisonExpressions = new List<ComparisonExpression>();
					var logicalExpressions = new List<LogicalExpression>();

					while (reader.TokenType != JsonTokenType.EndArray)
					{
						if (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
						{
							var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
							if (json.TryGetProperty(LogicalOperator.And.Name(), out _) ||
								json.TryGetProperty(LogicalOperator.Or.Name(), out _))
								logicalExpressions.Add(JsonSerializer.Deserialize<LogicalExpression>(json.ToString()!, options)!);
							else
								comparisonExpressions.Add(JsonSerializer.Deserialize<ComparisonExpression>(json.ToString()!, options)!);
						}
					}

					reader.ReadUntil(JsonTokenType.EndObject);

					return new LogicalExpression
					{
						Operator = name.ToEnum<LogicalOperator>()!.Value,
						ComparisonExpressions = comparisonExpressions.ToArray(),
						LogicalExpressions = logicalExpressions.ToArray()
					};
				}
				else
					throw new JsonException($"[{nameof(LogicalExpression)}] element [{name}] must contain an array.");
			}
			else
				throw new JsonException($"[{nameof(LogicalExpression)}] must be a JSON object.");
		}

		public override void Write(Utf8JsonWriter writer, LogicalExpression? expressionSet, JsonSerializerOptions options)
		{
			if (expressionSet != null)
			{
				writer.WriteStartObject();
				writer.WritePropertyName(expressionSet.Operator.Name());
				writer.WriteStartArray();
				expressionSet.LogicalExpressions?.Do(_ => JsonSerializer.Serialize(writer, _, options));
				expressionSet.ComparisonExpressions?.Do(_ => JsonSerializer.Serialize(writer, _, options));
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
