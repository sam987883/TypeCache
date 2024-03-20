// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Data;

public sealed class ColumnSchema(string name, bool nullable, bool primaryKey, bool readOnly, bool unique, RuntimeTypeHandle dataTypeHandle) : IEquatable<ColumnSchema>
{
	public string Name { get; } = name;

	public bool Nullable { get; } = nullable;

	public bool PrimaryKey { get; } = primaryKey;

	public bool ReadOnly { get; } = readOnly;

	public bool Unique { get; } = unique;

	public RuntimeTypeHandle DataTypeHandle { get; } = dataTypeHandle;

	public bool Equals(ColumnSchema? other)
		=> this.Name.EqualsIgnoreCase(other?.Name);

	public override bool Equals(object? other)
		=> this.Equals(other as ColumnSchema);

	public override int GetHashCode()
		=> this.Name.GetHashCode();
}
