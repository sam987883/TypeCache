// Copyright (c) 2021 Samuel Abraham

using TypeCache.Converters;
using System.Text.Json.Serialization;

namespace TypeCache.Data
{
	[JsonConverter(typeof(ExpressionJsonConverter))]
	public readonly struct ComparisonExpression
	{
		public string Field { get; init; }

		public ComparisonOperator Operator { get; init; }

		public object? Value { get; init; }
	}
}
