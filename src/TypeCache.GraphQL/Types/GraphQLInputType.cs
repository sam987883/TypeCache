// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Types;
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

		var fields = TypeOf<T>.Properties
			.Where(propertyInfo => propertyInfo.CanRead && propertyInfo.CanWrite && !propertyInfo.GraphQLIgnore())
			.Select(propertyInfo => new FieldType()
			{
				Type = propertyInfo.GraphQLType(true),
				Name = propertyInfo.GraphQLName(),
				Description = propertyInfo.GraphQLDescription(),
				DeprecationReason = propertyInfo.GraphQLDeprecationReason(),
			});
		foreach (var field in fields)
			this.AddField(field);
	}
}
