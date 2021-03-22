// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphInputType<T> : InputObjectGraphType<T>
	{
		public GraphInputType()
		{
			var graphAttribute = typeof(T).GetCustomAttributes(true).First<GraphAttribute>();
			this.Name = graphAttribute?.Name ?? $"{typeof(T).GetName()}Input";

			TypeOf<T>.Properties.Values
				.If(property => property!.Getter is not null && property!.Setter is not null && property.Attributes.First<GraphAttribute>()?.Ignore is not true)
				.Do(property => this.AddField(property!.CreateFieldType(true)));
		}
	}
}
