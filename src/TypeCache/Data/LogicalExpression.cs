// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;
using TypeCache.Collections;
using TypeCache.Converters;

namespace TypeCache.Data
{
	[JsonConverter(typeof(LogicalExpressionJsonConverter))]
	public sealed class LogicalExpression
	{
		public LogicalOperator Operator { get; set; }

		public LogicalExpression[] LogicalExpressions { get; set; } = Array<LogicalExpression>.Empty;

		public ComparisonExpression[] ComparisonExpressions { get; set; } = Array<ComparisonExpression>.Empty;
	}
}
