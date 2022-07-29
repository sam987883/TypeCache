// Copyright (c) 2021 Samuel Abraham

using System.Data;

namespace TypeCache.Data.Schema;

public class ColumnSchemaModel
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public SqlDbType Type { get; set; }

	public bool Nullable { get; set; }

	public bool ReadOnly { get; set; }

	public bool Hidden { get; set; }

	public bool Identity { get; set; }

	public bool PrimaryKey { get; set; }

	public int Length { get; set; }
}
