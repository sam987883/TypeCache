// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.Data.Schema;

public class ObjectSchemaModel
{
	public int Id { get; set; }

	public ObjectType Type { get; set; }

	public string DatabaseName { get; set; } = string.Empty;

	public string SchemaName { get; set; } = string.Empty;

	public string ObjectName { get; set; } = string.Empty;

	public ColumnSchemaModel[] Columns { get; set; } = Array<ColumnSchemaModel>.Empty;

	public ParameterSchemaModel[] Parameters { get; set; } = Array<ParameterSchemaModel>.Empty;
}
