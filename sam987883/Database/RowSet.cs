// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Database
{
	public sealed class RowSet
	{
		public string[] Columns { get; set; } = new string[0];

		public object?[][] Rows { get; set; } = new object[0][];
	}

	public sealed class RowSet<T> where T : class, new()
	{
		public string[] Columns { get; set; } = new string[0];

		public T[] Rows { get; set; } = new T[0];
	}
}
