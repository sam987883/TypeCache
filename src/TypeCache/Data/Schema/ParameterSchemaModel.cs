// Copyright (c) 2021 Samuel Abraham

using System.Data;

namespace TypeCache.Data.Schema;

public class ParameterSchemaModel
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public SqlDbType Type { get; set; }

	public bool Output { get; set; }
}
