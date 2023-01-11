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

		this.Name = typeof(T).GraphQLName();

		var fields = TypeOf<T>.Properties
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.Select(propertyInfo => propertyInfo.ToFieldType<T>());
		foreach (var field in fields)
			this.AddField(field);
	}
}
