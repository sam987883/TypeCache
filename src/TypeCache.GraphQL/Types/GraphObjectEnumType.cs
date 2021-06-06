// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

#nullable disable

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectEnumType<T> : EnumerationGraphType where T : class
	{
		public GraphObjectEnumType()
		{
			this.Name = TypeOf<T>.Attributes.First<GraphNameAttribute>()?.Name ?? $"{TypeOf<T>.Name}Fields";
			this.Description = $"Fields of type `{TypeOf<T>.Name}`.";

			foreach (var property in TypeOf<T>.Properties.Values.If(property => !property!.Attributes.Any<GraphIgnoreAttribute>()))
			{
				var name = property.GetGraphName();
				var description = property.GetGraphDescription();
				var deprecationReason = property.Attributes.First<ObsoleteAttribute>()?.Message;

				this.AddValue(name, description, name, deprecationReason);
			}
		}
	}
}
