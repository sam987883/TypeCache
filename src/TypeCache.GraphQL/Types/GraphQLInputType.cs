// Copyright (c) 2021 Samuel Abraham

using System.Linq;
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
		this.AddFieldTypes(typeof(T).GetInstanceProperties()
			.Where(propertyInfo => propertyInfo.CanRead && propertyInfo.CanWrite && !propertyInfo.GraphQLIgnore())
			.Select(propertyInfo => propertyInfo.ToInputFieldType()));
	}
}
