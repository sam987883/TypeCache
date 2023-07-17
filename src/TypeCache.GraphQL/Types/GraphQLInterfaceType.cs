// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInterfaceType<T> : InterfaceGraphType<T>
	where T : class
{
	public GraphQLInterfaceType()
	{
		typeof(T).IsInterface.AssertTrue();

		this.Name = typeof(T).GraphQLName();

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.AddField(propertyInfo, new PropertyFieldResolver<T>(propertyInfo)));
	}
}
