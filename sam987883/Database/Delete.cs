// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection;

namespace sam987883.Database
{
	public class Delete
	{
		public string Table { get; set; } = string.Empty;

		public ExpressionSet Where { get; set; } = new ExpressionSet();

		public RowSet Output { get; set; } = new RowSet();
	}

	public class Delete<T> where T : class, new()
	{
		public Delete(IPropertyCache<T> propertyCache) =>
			this.PropertyCache = propertyCache;

		internal IPropertyCache<T> PropertyCache { get; set; }

		public string Table { get; set; } = string.Empty;

		public ExpressionSet Where { get; set; } = new ExpressionSet();

		public RowSet<T> Output { get; set; } = new RowSet<T>();
	}
}
