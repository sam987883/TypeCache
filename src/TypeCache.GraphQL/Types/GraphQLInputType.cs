﻿// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInputType<T> : InputObjectGraphType<T>
	where T : notnull
{
	public GraphQLInputType()
	{
		this.Name = typeof(T).GraphQLInputName();
		this.Description = typeof(T).GraphQLDescription();
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && propertyInfo.CanWrite && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.AddField(new()
			{
				Type = propertyInfo.ToGraphQLType(true),
				Name = propertyInfo.GraphQLName(),
				Description = propertyInfo.GraphQLDescription(),
				DeprecationReason = propertyInfo.GraphQLDeprecationReason()
			}));
	}
}
