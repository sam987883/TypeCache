// Copyright (c) 2021 Samuel Abraham

using TypeCache.Converters;
using System.Text.Json.Serialization;

namespace TypeCache.Data
{
	[JsonConverter(typeof(ExpressionJsonConverter))]
	public record ComparisonExpression(string Field, ComparisonOperator Operator, object? Value);
}
