// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class SqlApiAttribute : Attribute
{
	public SqlApiAction Actions { get; } = SqlApiAction.All;
}
