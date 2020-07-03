// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection;

namespace sam987883.Database.Commands
{
	public sealed class BatchUpsert : Batch
	{
		public string[] InsertColumns { get; set; } = new string[0];

		public string[] OnColumns { get; set; } = new string[0];

		public string[] UpdateColumns { get; set; } = new string[0];

		public Output Output { get; set; } = new Output();
	}

	public sealed class BatchUpsert<T> : Batch<T> where T : class, new()
	{
		public BatchUpsert(IPropertyCache<T> propertyCache) : base(propertyCache) { }

		public string[] InsertColumns { get; set; } = new string[0];

		public string[] OnColumns { get; set; } = new string[0];

		public string[] UpdateColumns { get; set; } = new string[0];

		public Output<T> Output { get; set; } = new Output<T>();
	}
}
