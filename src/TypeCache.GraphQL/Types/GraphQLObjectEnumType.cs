// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLObjectEnumType<T> : EnumerationGraphType where T : class
{
	public GraphQLObjectEnumType()
	{
		var graphName = TypeOf<T>.Attributes.First<GraphQLNameAttribute>()?.Name;
		this.Name = graphName ?? $"{TypeOf<T>.Name}Fields";
		this.Description = $"Fields of type `{graphName ?? TypeOf<T>.Name}`.";

		foreach (var property in TypeOf<T>.Properties.Values.If(property => !property.GraphIgnore()))
		{
			var name = property.GraphName() ?? property.Name;
			var description = property.GraphDescription();
			var deprecationReason = property.ObsoleteMessage();

			this.AddValue(name, description, name, deprecationReason);
		}
	}
}
