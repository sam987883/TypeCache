// Copyright (c) 2020 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using sam987883.Common.Extensions;
using sam987883.Common.Models;

namespace sam987883.Common.Converters
{
    public class ExpressionJsonConverter : JsonConverter<Expression>
    {
        public override Expression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var expression = new Expression();

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString();

                    if (reader.Read())
                    {
                        if (name.Is(nameof(expression.Field)))
                            expression.Field = reader.GetString();
                        else if (name.Is(nameof(expression.Operator)))
                            expression.Operator = Enum.Parse<ComparisonOperator>(reader.GetString());
                        else
                        {
                            if (!name.Is(nameof(expression.Value)))
                                expression.Operator = Enum.Parse<ComparisonOperator>(name);

                            expression.Value = JsonSerializer.Deserialize<object>(ref reader, options);
                        }
                    }
                }
            }

            return expression;
        }

        public override void Write(Utf8JsonWriter writer, Expression expression, JsonSerializerOptions options)
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
