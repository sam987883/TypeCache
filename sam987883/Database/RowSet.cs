// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Database
{
	public struct RowSet
	{
		public string[] Columns { get; set; }

		public object?[][] Rows { get; set; }
	}
}
