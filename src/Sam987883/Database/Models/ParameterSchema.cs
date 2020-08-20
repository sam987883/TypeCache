// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Database.Models
{
	public class ParameterSchema
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public bool Output { get; set; }

		public bool Return { get; set; }
	}
}
