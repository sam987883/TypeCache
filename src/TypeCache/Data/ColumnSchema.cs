// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Data;

public sealed record ColumnSchema(string Name, bool Nullable, bool PrimaryKey, bool ReadOnly, bool Unique, RuntimeTypeHandle DataTypeHandle)
{
}
