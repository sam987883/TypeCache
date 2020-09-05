// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Converters;
using System.Text.Json.Serialization;

namespace Sam987883.Common.Models
{
	[JsonConverter(typeof(ExpressionSetJsonConverter))]
	public class ExpressionSet
	{
		public LogicalOperator Operator { get; set; }

		public Expression[] Expressions { get; set; } = new Expression[0];

		public ExpressionSet[] ExpressionSets { get; set; } = new ExpressionSet[0];
	}
}
