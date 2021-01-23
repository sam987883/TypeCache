// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types.Relay;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Types
{
	public class GraphConnectionType<T> : ConnectionType<GraphObjectType<T>, GraphEdgeType<T>>
        where T : class
    {
        public GraphConnectionType()
        {
            var name = Class<T>.Attributes.First<Attribute, GraphAttribute>()?.Name ?? Class<T>.Name;
            Name = $"{name}Connection";
            Description = $"A connection from an object to a list of objects of type `{name}`.";
        }
    }
}
