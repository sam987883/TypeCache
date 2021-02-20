// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types.Relay;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Types
{
	public class GraphEdgeType<T> : EdgeType<GraphObjectType<T>>
        where T : class
    {
        public GraphEdgeType()
        {
			var name = TypeOf<T>.Attributes.First<GraphAttribute>()?.Name ?? TypeOf<T>.Name;
			Name = $"{name}Edge";
            Description = $"An edge in a connection from an object to another object of type `{name}`.";
        }
    }
}
