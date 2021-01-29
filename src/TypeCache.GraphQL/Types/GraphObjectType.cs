// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectType<T> : ObjectGraphType<T>
		where T : class
	{
		public GraphObjectType()
		{
			var graphAttribute = TypeOf<T>.Attributes.First<Attribute, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? TypeOf<T>.Name;

			TypeOf<T>.Properties.Values
				.If(property => property!.Getter != null && property.Attributes.First<Attribute, GraphAttribute>()?.Ignore != true)
				.Do(property => this.AddField(property!.CreateFieldType(false)));
		}
	}
}
