// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectEnumType<T> : EnumerationGraphType where T : class
	{
		public GraphObjectEnumType()
		{
			var graphAttribute = TypeOf<T>.Attributes.First<Attribute, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? $"{TypeOf<T>.Name}Fields";

			foreach (var property in TypeOf<T>.Properties.Values)
			{
				graphAttribute = property.Attributes.First<Attribute, GraphAttribute>();
				if (graphAttribute?.Ignore == true)
					continue;

				var name = graphAttribute?.Name ?? property.Name;
				var description = graphAttribute?.Description;
				var deprecationReason = property.Attributes.First<Attribute, ObsoleteAttribute>()?.Message;

				this.AddValue(name, description, name, deprecationReason);
			}
		}
	}
}
