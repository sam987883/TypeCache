// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection;

namespace sam987883.Database.Requests
{
	public class UpdateRequest
	{
		/// <summary>
		/// The columns of DELETED data to return, that is, the original data before being updated.
		/// </summary>
		public string[] OutputDeleted { get; set; } = new string[0];

		/// <summary>
		/// The columns of INSERTED data to return, that is, the new data after being updated.
		/// </summary>
		public string[] OutputInserted { get; set; } = new string[0];

		/// <summary>
		/// The target table to perform SQL UPDATE operation on.
		/// </summary>
		public string Table { get; set; } = string.Empty;

		/// <summary>
		/// The columns to UPDATE along with their values to be SET.
		/// </summary>
		public (string Column, object Value)[] Set { get; set; } = new (string, object)[0];

		/// <summary>
		/// The WHERE clause that indicate which records will be updated.
		/// </summary>
		public ExpressionSet Where { get; set; } = new ExpressionSet();
	}
}
