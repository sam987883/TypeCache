// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	public class ParameterSchema
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public bool Output { get; set; }

		public bool Return { get; set; }
	}
}
