// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace sam987883.Database.Models
{
	public class ObjectSchema
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public ObjectType Type { get; set; }

		public string ObjectName { get; set; } = string.Empty;

		public string SchemaName { get; set; } = string.Empty;

		public string DatabaseName { get; set; } = string.Empty;

		public bool Inline { get; set; }

		public IImmutableList<ColumnSchema> Columns { get; set; } = ImmutableArray<ColumnSchema>.Empty;

		public IImmutableList<ParameterSchema> Parameters { get; set; } = ImmutableArray<ParameterSchema>.Empty;
	}
}
