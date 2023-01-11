// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <inheritdoc cref="ObjectGraphType{TSourceType}"/>
/// </summary>
public sealed class GraphQLObjectType<T> : ObjectGraphType<T>
	where T : notnull
{
	public GraphQLObjectType()
	{
		this.Name = typeof(T).GraphQLName();
		this.Description = typeof(T).GraphQLDescription() ?? Invariant($"{typeof(T).Assembly.GetName().Name}: {typeof(T).Namespace}.{typeof(T).Name()}");
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		var fields = TypeOf<T>.Properties
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.Select(propertyInfo => propertyInfo.ToFieldType<T>());
		foreach (var field in fields)
			this.AddField(field);

		var nonGenericInterfaces = typeof(T).GetInterfaces()
			.Where(_ => !_.HasElementType && !_.IsGenericType);
		foreach (var nonGenericInterface in nonGenericInterfaces)
			this.Interfaces.Add(nonGenericInterface);
	}
}
