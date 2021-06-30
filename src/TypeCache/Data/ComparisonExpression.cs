// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	[JsonConverter(typeof(ComparisonExpressionJsonConverter))]
	public sealed class ComparisonExpression
	{
		public string Field { get; set; } = string.Empty;

		public ComparisonOperator Operator { get; set; }

		public object? Value { get; set; }
	}
}
