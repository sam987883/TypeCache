// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL;
using GraphQL.Types;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLObjectType<T> : ObjectGraphType<T>
{
	public GraphQLObjectType()
	{
		this.Name = TypeOf<T>.Member.GraphQLName();
		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.GraphQLDeprecationReason();

		var fields = TypeOf<T>.Properties
			.Where(property => property.Getter is not null && !property.GraphQLIgnore())
			.Select(property => property.ToFieldType<T>());
		foreach (var field in fields)
			this.AddField(field);

		var nonGenericInterfaces = TypeOf<T>.InterfaceTypes
			.Where(type => type.ElementType is null && !type.GenericHandle.HasValue);
		foreach (var nonGenericInterface in nonGenericInterfaces)
			this.Interfaces.Add(nonGenericInterface);
	}
}
