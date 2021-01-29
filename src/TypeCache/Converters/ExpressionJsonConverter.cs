﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class ExpressionJsonConverter : JsonConverter<ComparisonExpression>
	{
		public override ComparisonExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var expression = new ComparisonExpression();

			string field = string.Empty;
			var comparisonOperator = ComparisonOperator.Equal;
			object value = null!;
			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString();

					if (reader.Read())
					{
						if (name.Is(nameof(expression.Field)))
							field = reader.GetString()!;
						else if (name!.Is(nameof(expression.Operator)))
							comparisonOperator = reader.GetString().Enum<ComparisonOperator>()!.Value;
						else
						{
							if (!name.Is(nameof(expression.Value)))
								comparisonOperator = name.Enum<ComparisonOperator>()!.Value;

							value = JsonSerializer.Deserialize<object>(ref reader, options)!;
						}
					}
				}
			}

			return new ComparisonExpression
			{
				Field = field,
				Operator = comparisonOperator,
				Value = value
			};
		}

		public override void Write(Utf8JsonWriter writer, ComparisonExpression expression, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString(nameof(expression.Field), expression.Field);
			writer.WritePropertyName(expression.Operator.Name());
			if (expression.Value != null)
				JsonSerializer.Serialize(writer, expression.Value, expression.Value.GetType());
			else
				writer.WriteNullValue();
			writer.WriteEndObject();
		}
	}
}
