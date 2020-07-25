// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Converters;
using System.Text.Json.Serialization;

namespace sam987883.Common.Models
{
	[JsonConverter(typeof(ExpressionJsonConverter))]
	public class Expression
	{
		public string Field { get; set; } = string.Empty;

		public ComparisonOperator Operator { get; set; }

		public object? Value { get; set; }
	}
}
