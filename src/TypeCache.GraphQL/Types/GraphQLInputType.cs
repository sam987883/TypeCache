// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Types;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLInputType<T> : InputObjectGraphType<T>
{
	public GraphQLInputType()
	{
		this.Name = TypeOf<T>.Member.GraphQLInputName();
		this.Description = TypeOf<T>.Member.GraphQLDescription();
		this.DeprecationReason = TypeOf<T>.Member.GraphQLDeprecationReason();

		var fields = TypeOf<T>.Properties
			.Where(property => property.Public && property.Getter is not null && property.Setter is not null && !property.GraphQLIgnore())
			.Select(property => new FieldType()
			{
				Type = property.GraphQLType(true),
				Name = property.GraphQLName(),
				Description = property.GraphQLDescription(),
				DeprecationReason = property.GraphQLDeprecationReason(),
			});
		foreach (var field in fields)
			this.AddField(field);
	}
}
