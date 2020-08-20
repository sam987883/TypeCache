// Copyright (c) 2020 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using Sam987883.Common.Extensions;
using Sam987883.Common.Models;
using System.Collections.Generic;

namespace Sam987883.Common.Converters
{
	public class ExpressionSetJsonConverter : JsonConverter<ExpressionSet?>
	{
		public override ExpressionSet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null)
				return null;
			else if (reader.TokenType == JsonTokenType.StartObject &&
				reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				var name = reader.GetString();
				if (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
				{
					var expressions = new List<Expression>();
					var expressionSets = new List<ExpressionSet>();

					while (reader.TokenType != JsonTokenType.EndArray)
					{
						if (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
						{
							var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
							if (json.TryGetProperty(LogicalOperator.And.Name(), out _) ||
								json.TryGetProperty(LogicalOperator.Or.Name(), out _))
								expressionSets.Add(JsonSerializer.Deserialize<ExpressionSet>(json.ToString(), options));
							else
								expressions.Add(JsonSerializer.Deserialize<Expression>(json.ToString(), options));
						}
					}

					while (reader.TokenType != JsonTokenType.EndObject && reader.Read()) { }

					return new ExpressionSet
					{
						Operator = Enum.Parse<LogicalOperator>(name),
						Expressions = expressions.ToArray(),
						ExpressionSets = expressionSets.ToArray()
					};
				}
				else
					throw new JsonException($"[{nameof(ExpressionSet)}] element [{name}] must contain an array.");
			}
			else
				throw new JsonException($"[{nameof(ExpressionSet)}] must be a JSON object.");
		}

		public override void Write(Utf8JsonWriter writer, ExpressionSet? expressionSet, JsonSerializerOptions options)
		{
			if (expressionSet != null)
			{
				writer.WriteStartObject();
				writer.WritePropertyName(expressionSet.Operator.Name());
				writer.WriteStartArray();
				expressionSet.ExpressionSets?.IfNotNull().Do(_ => JsonSerializer.Serialize(writer, _, options));
				expressionSet.Expressions?.IfNotNull().Do(_ => JsonSerializer.Serialize(writer, _, options));
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
