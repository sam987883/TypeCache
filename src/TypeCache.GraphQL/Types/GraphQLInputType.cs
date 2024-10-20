// Copyright (c) 2021 Samuel Abraham

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
			.Select(propertyInfo => new FieldType()
			{
				Type = propertyInfo.PropertyType,
				Name = propertyInfo.GraphQLName(),
				Description = propertyInfo.GraphQLDescription(),
				DeprecationReason = propertyInfo.GraphQLDeprecationReason()
			})
			.ToArray()
			.ForEach(fieldType => this.Fields.Add(fieldType));
	}
}
