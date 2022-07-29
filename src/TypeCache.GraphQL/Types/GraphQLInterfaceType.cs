// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types;

public class GraphQLInterfaceType<T> : InterfaceGraphType<T>
	where T : class
{
	public GraphQLInterfaceType()
	{
		TypeOf<T>.Kind.AssertEquals(Kind.Interface);

		this.Name = TypeOf<T>.Member.GraphQLName();

		TypeOf<T>.Properties
			.If(property => property.Getter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(property.ToFieldType<T>()));
	}
}
