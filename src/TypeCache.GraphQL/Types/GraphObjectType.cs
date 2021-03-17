// Copyright (c) 2021 Samuel Abraham

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
			var graphAttribute = TypeOf<T>.Attributes.First<GraphAttribute>();
			this.Name = graphAttribute?.Name ?? TypeOf<T>.Name;

			TypeOf<T>.Properties.Values
				.If(property => property!.Getter is not null && property.Attributes.First<GraphAttribute>()?.Ignore != true)
				.Do(property => this.AddField(property!.CreateFieldType(false)));
		}
	}
}
