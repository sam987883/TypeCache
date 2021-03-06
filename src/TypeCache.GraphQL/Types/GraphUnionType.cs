﻿// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Collections.Extensions;

namespace TypeCache.GraphQL.Types
{
	/// <summary>
	/// <see cref="GraphUnionType"/> works with 2 or more types.
	/// </summary>
	public class GraphUnionType : UnionGraphType
	{
		public GraphUnionType(Type[] types)
		{
			if (types.Length < 2)
				throw new ArgumentOutOfRangeException(nameof(GraphUnionType), $"2 or more types are required.");

			var graphObjectType = typeof(GraphObjectType<>);
			types.Do(type => this.Type(graphObjectType.MakeGenericType(type)));
		}
	}
}
