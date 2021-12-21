// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphObjectEnumType<T> : EnumerationGraphType where T : class
{
	public GraphObjectEnumType()
	{
		var graphName = TypeOf<T>.Attributes.First<GraphNameAttribute>()?.Name;
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
