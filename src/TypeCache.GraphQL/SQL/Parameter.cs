﻿// Copyright (c) 2021 Samuel Abraham

using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.SQL;

public class Parameter
{
	[GraphType(ScalarType.NonNullableString)]
	public string Name { get; set; } = string.Empty;

	public object? Value { get; set; }
}
