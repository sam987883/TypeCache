﻿// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphInputType<T> : InputObjectGraphType<T>
	{
		public GraphInputType()
		{
			this.Name = TypeOf<T>.Member.Attributes.First<GraphNameAttribute>()?.Name
				?? (TypeOf<T>.EnclosedType != null ? $"{TypeOf<T>.Name}Of{TypeOf<T>.EnclosedType.Name}Input" : $"{TypeOf<T>.Name}Input");
			this.Description = TypeOf<T>.Member.GetGraphDescription() ?? $"An input object of type `{TypeOf<T>.Name}`.";

			TypeOf<T>.Properties.Values
				.If(property => property!.Getter is not null && property!.Setter is not null
					&& !property.Attributes.Any<GraphIgnoreAttribute>())
				.Do(property => this.AddField(property!.ToFieldType(true)));
		}
	}
}
