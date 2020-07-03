// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection;

namespace sam987883.Database.Commands
{
	public sealed class BatchUpdate : Batch
	{
		public string[] OnColumns { get; set; } = new string[0];

		public Output Output { get; set; } = new Output();

		public string[] UpdateColumns { get; set; } = new string[0];
	}

	public sealed class BatchUpdate<T> : Batch<T> where T : class, new()
	{
		public BatchUpdate(IPropertyCache<T> propertyCache) : base(propertyCache) { }

		public string[] OnColumns { get; set; } = new string[0];

		public Output<T> Output { get; set; } = new Output<T>();

		public string[] UpdateColumns { get; set; } = new string[0];
	}
}
