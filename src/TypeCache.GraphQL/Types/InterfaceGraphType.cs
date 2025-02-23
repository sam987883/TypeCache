// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;

namespace TypeCache.GraphQL.Types;

public sealed class InterfaceGraphType<T> : global::GraphQL.Types.InterfaceGraphType<T>
	where T : class
{
	public InterfaceGraphType()
	{
		typeof(T).IsInterface.ThrowIfFalse();

		this.Name = typeof(T).GraphQLName();

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.AddField(propertyInfo.ToFieldType()));
	}
}
