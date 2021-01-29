// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	public readonly struct ParameterSchema
	{
		public int Id { get; init; }

		public string Name { get; init; }

		public bool Output { get; init; }

		public bool Return { get; init; }
	}
}
