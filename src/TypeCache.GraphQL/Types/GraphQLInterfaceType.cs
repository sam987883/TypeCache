// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInterfaceType<T> : InterfaceGraphType<T>
	where T : class
{
	public GraphQLInterfaceType()
	{
		typeof(T).GetKind().AssertEquals(Kind.Interface);

		this.Name = typeof(T).GraphQLName();

		typeof(T).GetInstanceProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.Select(propertyInfo => this.AddField(propertyInfo.ToFieldType<T>()))
			.ToArray();
	}
}
