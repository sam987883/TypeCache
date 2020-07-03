// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection;

namespace sam987883.Database.Commands
{
	public sealed class BatchInsert : Batch
	{
		public string[] InsertColumns { get; set; } = new string[0];

		public RowSet Output { get; set; } = new RowSet();
	}

	public sealed class BatchInsert<T> : Batch<T> where T : class, new()
	{
		public BatchInsert(IPropertyCache<T> propertyCache) : base(propertyCache) { }

		public string[] InsertColumns { get; set; } = new string[0];

		public RowSet<T> Output { get; set; } = new RowSet<T>();
	}
}
