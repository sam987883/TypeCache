// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLObjectType<T> : ObjectGraphType<T>
{
	public GraphQLObjectType()
	{
		this.Name = TypeOf<T>.Member.GraphQLName();
		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.GraphQLDeprecationReason();

		TypeOf<T>.Properties
			.If(property => property.Getter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(property.ToFieldType<T>()));

		TypeOf<T>.InterfaceTypes
			.If(type => type.ElementType is null && !type.GenericHandle.HasValue)
			.Do(type => this.Interfaces.Add(type));
	}
}
