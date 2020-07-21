// Copyright (c) 2020 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using sam987883.Common;
using sam987883.Extensions;

namespace sam987883.Web.Middleware
{
	public class ExpressionJsonConverter : JsonConverter<Expression>
	{
        public override Expression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var expression = JsonSerializer.Deserialize<Expression>(ref reader, null);
			return expression.Value is JsonElement jsonElement ? new Expression
            {
                Field = expression.Field,
                Operator = expression.Operator,
                Value = jsonElement.ValueKind switch
                {
                    JsonValueKind.Undefined => throw new ArgumentException($"Invalid value for [{nameof(expression.Value)}] specified in [{nameof(Expression)}]: {expression.Value}.", expression.Field),
                    JsonValueKind.Object => jsonElement.ToString(),
                    JsonValueKind.Array => jsonElement.GetArrayValues(),
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.TryGetInt64(out var value) ? value : jsonElement.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => throw new NotImplementedException()
                }
            } : expression;
		}

		public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, null);
	}
}
