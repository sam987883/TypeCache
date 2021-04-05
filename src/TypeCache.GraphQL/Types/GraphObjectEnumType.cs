﻿// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectEnumType<T> : EnumerationGraphType where T : class
	{
		public GraphObjectEnumType()
		{
			var graphAttribute = TypeOf<T>.Attributes.First<GraphAttribute>();
			this.Name = graphAttribute?.Name ?? $"{TypeOf<T>.Name}Fields";

			foreach (var property in TypeOf<T>.Properties.Values.If(property => !property!.Attributes.Any<GraphIgnoreAttribute>()))
			{
				graphAttribute = property!.Attributes.First<GraphAttribute>();
				var name = graphAttribute?.Name ?? property.Name;
				var description = graphAttribute?.Description;
				var deprecationReason = property.Attributes.First<ObsoleteAttribute>()?.Message;

				this.AddValue(name, description, name, deprecationReason);
			}
		}
	}
}
