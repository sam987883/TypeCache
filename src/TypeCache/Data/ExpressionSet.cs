// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json.Serialization;
using TypeCache.Converters;

namespace TypeCache.Data
{
	[JsonConverter(typeof(ExpressionSetJsonConverter))]
	public readonly struct ExpressionSet
	{
		public LogicalOperator Operator { get; init; }

		public ComparisonExpression[] Expressions { get; init; }

		public ExpressionSet[] ExpressionSets { get; init; }
	}
}
