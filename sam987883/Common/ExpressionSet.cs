// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Common
{
	public class ExpressionSet
	{
		public LogicalOperator Operator { get; set; }
		public Expression[] Expressions { get; set; } = new Expression[0];
		public ExpressionSet[] ExpressionSets { get; set; } = new ExpressionSet[0];
	}
}
