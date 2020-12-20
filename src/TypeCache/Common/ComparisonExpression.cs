// Copyright (c) 2021 Samuel Abraham

using TypeCache.Converters;
using System.Text.Json.Serialization;

namespace TypeCache.Common
{
	[JsonConverter(typeof(ExpressionJsonConverter))]
	public class ComparisonExpression
	{
		public string Field { get; set; } = string.Empty;

		public ComparisonOperator Operator { get; set; }

		public object? Value { get; set; }
	}
}
