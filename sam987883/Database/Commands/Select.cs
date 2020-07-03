// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection;

namespace sam987883.Database.Commands
{
	public class Select
	{
		public (string Name, object Value)[] Parameters { get; set; } = new (string, object)[0];

		public string From { get; set; } = string.Empty;

		public ExpressionSet? Where { get; set; }

		public ExpressionSet? Having { get; set; }

		public (string Column, Sort Sort)[] OrderBy { get; set; } = new (string, Sort)[0];

		public RowSet Output { get; set; } = new RowSet();
	}

	public class Select<T> where T : class, new()
	{
		public Select(IPropertyCache<T> propertyCache) =>
			this.PropertyCache = propertyCache;

		internal IPropertyCache<T> PropertyCache { get; set; }

		public (string Name, object Value)[] Parameters { get; set; } = new (string, object)[0];

		public string From { get; set; } = string.Empty;

		public ExpressionSet? Where { get; set; }

		public ExpressionSet? Having { get; set; }

		public (string Column, Sort Sort)[] OrderBy { get; set; } = new (string, Sort)[0];

		public RowSet<T> Output { get; set; } = new RowSet<T>();
	}
}