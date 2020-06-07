// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;

namespace sam987883.Database
{
	public class TableSchema
	{
		public string Name { get; set; } = string.Empty;
		public string ObjectName { get; set; } = string.Empty;
		public string SchemaName { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = string.Empty;
		public int ObjectId { get; set; }
		public TableType Type { get; set; }
		public bool Inline { get; set; }
		public IReadOnlyList<ColumnSchema> Columns { get; set; } = new ColumnSchema[0];
	}
}
