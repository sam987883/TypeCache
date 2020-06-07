// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection;

namespace sam987883.Database
{
	public class Select
	{
		public (string Name, object Value)[] Parameters { get; set; } = new (string, object)[0];

		public string[] Columns { get; set; } = new string[0];

		public string From { get; set; } = string.Empty;

		public ExpressionSet? Where { get; set; }

		public ExpressionSet? Having { get; set; }

		public (string Column, Sort Sort)[] OrderBy { get; set; } = new (string, Sort)[0];

		public object?[][] Rows { get; set; } = new object[0][];
	}

	public class Select<T> where T : class
	{
		public Select(IPropertyCache<T> propertyCache) =>
			this.PropertyCache = propertyCache;

		internal IPropertyCache<T> PropertyCache { get; set; }

		public (string Name, object Value)[] Parameters { get; set; } = new (string, object)[0];

		public string[] Columns { get; set; } = new string[0];

		public string From { get; set; } = string.Empty;

		public ExpressionSet? Where { get; set; }

		public ExpressionSet? Having { get; set; }

		public (string Column, Sort Sort)[] OrderBy { get; set; } = new (string, Sort)[0];

		public T[] Rows { get; set; } = new T[0];
	}
}