// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphObjectType<T> : ObjectGraphType<T>
	{
		public GraphObjectType()
		{
			var name = TypeOf<T>.Attributes.GraphName();
			var description = TypeOf<T>.Member.GraphDescription();

			if (TypeOf<T>.Is(typeof(Connection<>)))
			{
				name ??= $"{TypeOf<T>.EnclosedType!.Value.Name}Connection";
				description ??= $"A connection from an object to a list of objects of type `{TypeOf<T>.EnclosedType!.Value.Name}`.";
			}
			else if (TypeOf<T>.Is(typeof(Edge<>)))
			{
				name ??= $"{TypeOf<T>.EnclosedType!.Value.Name}Edge";
				description ??= $"An edge in a connection from an object to another object of type `{TypeOf<T>.EnclosedType!.Value.Name}`.";
			}
			else
			{
				name ??= TypeOf<T>.Name;

				TypeOf<T>.InterfaceTypes.Do(type => this.Interface(type));
			}

			this.Name = name;
			this.Description = description;
			this.DeprecationReason = TypeOf<T>.Member.ObsoleteMessage();

			TypeOf<T>.Properties.Values
				.If(property => property.Getter is not null && !property.GraphIgnore())
				.Do(property => this.AddField(property.ToFieldType(false)));
		}
	}
}
