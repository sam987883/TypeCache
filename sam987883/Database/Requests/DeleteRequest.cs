// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;

namespace sam987883.Database.Requests
{
	public class DeleteRequest
	{
		/// <summary>
		/// Target table to receive the DELETE operation.
		/// </summary>
		public string From { get; set; } = string.Empty;

		/// <summary>
		/// The WHERE clause expression that indicates which records will get deleted.
		/// </summary>
		public ExpressionSet Where { get; set; } = new ExpressionSet();

		/// <summary>
		/// The columns of data to return.
		/// </summary>
		public string[] Output { get; set; } = new string[0];
	}
}
