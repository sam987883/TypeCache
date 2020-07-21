// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;

namespace sam987883.Database.Requests
{
	public class SelectRequest
	{
		public string From { get; set; } = string.Empty;

		public ExpressionSet? Having { get; set; }

		public ColumnSort[] OrderBy { get; set; } = new ColumnSort[0];

		public ColumnOutput[] Output { get; set; } = new ColumnOutput[0];

		public Parameter[] Parameters { get; set; } = new Parameter[0];

		public ExpressionSet? Where { get; set; }
	}
}
