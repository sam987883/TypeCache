// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection;

namespace sam987883.Database
{
	public sealed class BatchDelete : Batch
	{
		public string[] OnColumns { get; set; } = new string[0];

		public RowSet Output { get; set; } = new RowSet();
	}

	public sealed class BatchDelete<T> : Batch<T> where T : class, new()
	{
		public BatchDelete(IPropertyCache<T> propertyCache) : base(propertyCache) { }

		public string[] OnColumns { get; set; } = new string[0];

		public RowSet<T> Output { get; set; } = new RowSet<T>();
	}
}
