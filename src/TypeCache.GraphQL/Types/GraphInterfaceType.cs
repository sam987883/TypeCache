// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types
{
	public class GraphInterfaceType<T> : InterfaceGraphType<T>
		where T : class
	{
		public GraphInterfaceType()
		{
			TypeOf<T>.Kind.Assert(TypeOf<T>.Name, Kind.Interface);

			var graphAttribute = TypeOf<T>.Attributes.First<GraphAttribute>();
			this.Name = graphAttribute?.Name ?? TypeOf<T>.Name;

			TypeOf<T>.Properties.Values
				.If(property => property!.Getter is not null
					&& !property.Attributes.Any<GraphIgnoreAttribute>()
					&& !property.Attributes.Any<GraphCursorAttribute>())
				.Do(this.AddField!);
		}
	}
}
