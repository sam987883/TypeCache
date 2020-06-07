// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Common
{
	public class Expression
	{
		public string Field { get; set; } = string.Empty;
		public ComparisonOperator Operator { get; set; }
		public object? Value { get; set; }
	}
}
