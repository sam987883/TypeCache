// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types.Relay;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Types
{
	public class GraphConnectionType<T> : ConnectionType<GraphObjectType<T>, GraphEdgeType<T>>
        where T : class
    {
        public GraphConnectionType()
        {
            var name = TypeOf<T>.Attributes.First<Attribute, GraphAttribute>()?.Name ?? TypeOf<T>.Name;
            Name = $"{name}Connection";
            Description = $"A connection from an object to a list of objects of type `{name}`.";
        }
    }
}
