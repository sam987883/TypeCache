﻿// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SqlApiAttribute : Attribute
{
	public SqlApiAction Actions { get; } = SqlApiAction.All;
}
