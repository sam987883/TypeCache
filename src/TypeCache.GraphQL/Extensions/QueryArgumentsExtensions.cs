// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class QueryArgumentsExtensions
{
	public static void Add<T>(this QueryArguments @this, string name, object? defaultValue = null, string? description = null)
		where T : IGraphType
		=> @this.Add(new QueryArgument<T>
		{
			Name = name,
			DefaultValue = defaultValue,
			Description = description
		});

	public static void Add(this QueryArguments @this, string name, IGraphType resolvedType, object? defaultValue = null, string? description = null)
		=> @this.Add(new(resolvedType)
		{
			Name = name,
			DefaultValue = defaultValue,
			Description = description
		});
}
