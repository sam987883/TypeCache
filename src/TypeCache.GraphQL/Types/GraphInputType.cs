// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphInputType<T> : InputObjectGraphType<T>
	{
		public GraphInputType()
		{
			var enclosedTypeName = TypeOf<T>.EnclosedType?.Attributes.GraphName() ?? TypeOf<T>.EnclosedType?.Name;
			this.Name = TypeOf<T>.Member.Attributes.GraphName()
				?? (enclosedTypeName is not null ? $"{TypeOf<T>.Name}Of{enclosedTypeName}Input" : $"{TypeOf<T>.Name}Input");
			this.Description = TypeOf<T>.Member.Attributes.GraphDescription() ?? $"An input object of type `{TypeOf<T>.Name}`.";

			TypeOf<T>.Properties.Values
				.If(property => property.Getter is not null && property.Setter is not null && !property.Attributes.GraphIgnore())
				.Do(property => this.AddField(property!.ToFieldType(true)));
		}
	}
}
