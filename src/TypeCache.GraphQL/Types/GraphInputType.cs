// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphInputType<T> : InputObjectGraphType<T>
	{
		public GraphInputType()
		{
			var graphAttribute = typeof(T).GetCustomAttributes(true).First<object, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? $"{typeof(T).GetName()}Input";

			TypeOf<T>.Properties.Values
				.If(property => property!.Getter != null && property.Attributes.First<Attribute, GraphAttribute>()?.Ignore != true)
				.Do(property => this.AddField(property!.CreateFieldType(true)));
		}
	}
}
