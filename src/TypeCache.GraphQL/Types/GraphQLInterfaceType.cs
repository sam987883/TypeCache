// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInterfaceType<T> : InterfaceGraphType<T>
	where T : class
{
	public GraphQLInterfaceType()
	{
		TypeOf<T>.Kind.AssertEquals(Kind.Interface);

		this.Name = TypeOf<T>.Member.GraphQLName();

		var fields = TypeOf<T>.Properties
			.Where(property => property.Getter is not null && !property.GraphQLIgnore())
			.Select(property => property.ToFieldType<T>());
		foreach (var field in fields)
			this.AddField(field);
	}
}
