// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection;

namespace sam987883.Database.Commands
{
	public abstract class Batch
	{
		public RowSet Input { get; set; } = new RowSet();

		public string Table { get; set; } = string.Empty;
	}

	public abstract class Batch<T> where T : class, new()
	{
		internal protected Batch(IPropertyCache<T> propertyCache) =>
			this.PropertyCache = propertyCache;

		public RowSet<T> Input { get; set; } = new RowSet<T>();

		public string Table { get; set; } = string.Empty;

		internal IPropertyCache<T> PropertyCache { get; set; }
	}
}
