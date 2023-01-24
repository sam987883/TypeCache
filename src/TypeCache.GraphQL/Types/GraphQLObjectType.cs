// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using static System.FormattableString;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

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
		this.AddFieldTypes(typeof(T).GetInstanceProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.Select(propertyInfo => propertyInfo.ToFieldType<T>()));

		typeof(T).GetInterfaces()
			.Where(_ => !_.HasElementType && !_.IsGenericType)
			.ToArray()
			.ForEach(this.Interfaces.Add);
	}

	/// <summary>
	/// Adds a field that is based on the result of the <paramref name="methodInfo"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="methodInfo"/>.
	/// </summary>
	public FieldType AddField(MethodInfo methodInfo)
	{
		var fieldType = methodInfo.ToFieldType();
		var type = methodInfo.ReturnType;
		if (type.IsGenericType && type.IsAny(typeof(Task<>), typeof(ValueTask<>)))
			type = type.GenericTypeArguments[0];
		var resolverType = typeof(ItemLoaderFieldResolver<>).MakeGenericType(type);
		fieldType.Resolver = (IFieldResolver)resolverType.Create(methodInfo)!;
		return this.AddField(fieldType);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="methodInfo"/>,
	/// whose result objects are matched back to the parent object by matching the value of the properties on both objects with the same <c><see cref="GraphQLMatchAttribute"/></c>.<br/>
	/// This subquery returns 0 or 1 child record for every 1 parent record of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads the child data for all parent property values.  This method can contain user input query parameters,
	/// in addition a parameter of type <c>IEnumerable&lt;<typeparamref name="MATCH"/>&gt;</c> or <typeparamref name="MATCH"/>[] for taking in
	/// the matched values to query all <c><typeparamref name="CHILD"/></c> objects.</param>
	/// <param name="getParentMatch">Gets the property value to match the parent object on.</param>
	/// <param name="getChildMatch">Gets the property value to match the child object on.</param>
	public FieldType AddQueryItem<CHILD, MATCH>(MethodInfo methodInfo, Func<T, MATCH> getParentMatch, Func<CHILD, MATCH> getChildMatch)
		where CHILD : notnull
	{
		methodInfo.ReturnType.IsAny<CHILD[], IEnumerable<CHILD>, Task<CHILD[]>, Task<IEnumerable<CHILD>>, ValueTask<CHILD[]>, ValueTask<IEnumerable<CHILD>>>().AssertTrue();

		var fieldType = new FieldType()
		{
			Arguments = new QueryArguments(methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
					&& !parameterInfo.ParameterType.IsAny<IResolveFieldContext, MATCH[], IEnumerable<MATCH>>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLType())
				{
					Name = parameterInfo.GraphQLName(),
					Description = parameterInfo.GraphQLDescription(),
				})),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new BatchLoaderFieldResolver<T, CHILD, MATCH>(methodInfo, getParentMatch, getChildMatch, false),
			Type = typeof(GraphQLObjectType<CHILD>)
		};
		return this.AddField(fieldType);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="methodInfo"/>,
	/// whose result objects are matched back to the parent object by matching the value of the properties on both objects with the same <c><see cref="GraphQLMatchAttribute"/></c>.<br/>
	/// This subquery returns 0 or more child records for every 1 parent record of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads the child data for all parent property values.  This method can contain user input query parameters,
	/// in addition a parameter of type <c>IEnumerable&lt;<typeparamref name="MATCH"/>&gt;</c> or <typeparamref name="MATCH"/>[] for taking in
	/// the matched values to query all <c><typeparamref name="CHILD"/></c> objects.</param>
	/// <param name="getParentMatch">Gets the property value to match the parent object on.</param>
	/// <param name="getChildMatch">Gets the property value to match the child object on.</param>
	public FieldType AddQueryCollection<CHILD, MATCH>(MethodInfo methodInfo, Func<T, MATCH> getParentMatch, Func<CHILD, MATCH> getChildMatch)
		where CHILD : notnull
	{
		methodInfo.ReturnType.IsAny<CHILD[], IEnumerable<CHILD>, Task<CHILD[]>, Task<IEnumerable<CHILD>>, ValueTask<CHILD[]>, ValueTask<IEnumerable<CHILD>>>().AssertTrue();

		var fieldType = new FieldType()
		{
			Arguments = new QueryArguments(methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
					&& !parameterInfo.ParameterType.IsAny<IResolveFieldContext, MATCH[], IEnumerable<MATCH>>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLType())
				{
					Name = parameterInfo.GraphQLName(),
					Description = parameterInfo.GraphQLDescription(),
				})),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new BatchLoaderFieldResolver<T, CHILD, MATCH>(methodInfo, getParentMatch, getChildMatch, true),
			Type = typeof(ListGraphType<GraphQLObjectType<CHILD>>)
		};
		return this.AddField(fieldType);
	}
}
