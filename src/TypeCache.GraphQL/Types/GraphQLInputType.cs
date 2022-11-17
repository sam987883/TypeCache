// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInputType<T> : InputObjectGraphType<T>
{
	public GraphQLInputType()
	{
		this.Name = TypeOf<T>.Member.GraphQLInputName();
		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.GraphQLDeprecationReason();

		TypeOf<T>.Properties
			.If(property => property.Getter is not null && property.Setter is not null && !property.GraphQLIgnore())
			.Do(property => this.AddField(new()
			{
				Type = property.GraphQLType(true),
				Name = property.GraphQLName(),
				Description = property.GraphQLDescription(),
				DeprecationReason = property.GraphQLDeprecationReason(),
			}));
	}
}
