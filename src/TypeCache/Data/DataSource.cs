// Copyright(c) 2020 Samuel Abraham

namespace TypeCache.Data
{
	public readonly struct DataSource
	{
		public string Name { get; init; }

		public string DatabaseProvider { get; init; }

		public string ConnectionString { get; init; }
	}
}
