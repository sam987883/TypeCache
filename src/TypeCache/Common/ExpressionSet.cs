// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Common
{
	[JsonConverter(typeof(ExpressionSetJsonConverter))]
	public class ExpressionSet
	{
		public LogicalOperator Operator { get; set; }

		public ComparisonExpression[] Expressions { get; set; } = Array.Empty<ComparisonExpression>();

		public ExpressionSet[] ExpressionSets { get; set; } = Array.Empty<ExpressionSet>();
	}
}
