﻿// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <inheritdoc cref="GraphType"/>
/// </summary>
public sealed class GraphQLObjectType<T> : ObjectGraphType<T>
	where T : notnull
{
	public GraphQLObjectType()
		: this(typeof(T).GraphQLName())
	{
	}

	public GraphQLObjectType(string name)
	{
		this.Name = name;
		this.Description = typeof(T).GraphQLDescription() ?? Invariant($"{typeof(T).Assembly.GetName().Name}: {typeof(T).Namespace}.{typeof(T).Name()}");
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.AddField(propertyInfo, new PropertyFieldResolver<T>(propertyInfo)));

		typeof(T).GetInterfaces()
			.Where(_ => !_.HasElementType && !_.IsGenericType)
			.ToArray()
			.ForEach(this.Interfaces.Add);
	}

	/// <summary>
	/// Adds a field that is based on the result of the <paramref name="methodInfo"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="methodInfo"/>.
	/// </summary>
	/// <remarks>The <paramref name="methodInfo"/> must be a static method.</remarks>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public FieldType AddField(MethodInfo methodInfo)
	{
		var type = methodInfo.ReturnType;
		if (type.IsGenericType && type.IsAny([typeof(Task<>), typeof(ValueTask<>)]))
			type = type.GenericTypeArguments[0];

		var resolverType = typeof(ItemLoaderFieldResolver<>).MakeGenericType(type);
		var resolver = (IFieldResolver)resolverType.Create([methodInfo])!;
		return this.AddField(methodInfo, resolver);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="methodInfo"/>,
	/// whose result objects are matched back to the parent object by matching the result of <paramref name="getParentMatch"/> and <paramref name="getChildMatch"/>.<br/>
	/// This subquery returns 0 or 1 child record for every 1 parent record of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads the child data for all parent property values.  This method can contain user input query parameters,
	/// in addition a parameter of type <c>IEnumerable&lt;<typeparamref name="MATCH"/>&gt;</c> or <typeparamref name="MATCH"/>[] for taking in
	/// the matched values to query all <c><typeparamref name="CHILD"/></c> objects.</param>
	/// <param name="getParentMatch">Gets the property value to match the parent object on.</param>
	/// <param name="getChildMatch">Gets the property value to match the child object on.</param>
	public FieldType AddQueryItem<CHILD, MATCH>(MethodInfo methodInfo, Func<T, MATCH> getParentMatch, Func<CHILD, MATCH> getChildMatch)
		where CHILD : notnull
		where MATCH : notnull
	{
		methodInfo.ReturnType.IsAny<CHILD[], IEnumerable<CHILD>, Task<CHILD[]>, Task<IEnumerable<CHILD>>, ValueTask<CHILD[]>, ValueTask<IEnumerable<CHILD>>>().ThrowIfFalse();

		var fieldType = new FieldType()
		{
			Arguments = new QueryArguments(methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
					&& !parameterInfo.ParameterType.IsAny<IResolveFieldContext, MATCH[], IEnumerable<MATCH>>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.ToGraphQLType())
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
	/// whose result objects are matched back to the parent object by matching the result of <paramref name="getParentMatch"/> and <paramref name="getChildMatch"/>.<br/>
	/// This subquery returns 0 or more child records for every 1 parent record of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads the child data for all parent property values.  This method can contain user input query parameters,
	/// in addition a parameter of type <c>IEnumerable&lt;<typeparamref name="MATCH"/>&gt;</c> or <typeparamref name="MATCH"/>[] for taking in
	/// the matched values to query all <c><typeparamref name="CHILD"/></c> objects.</param>
	/// <param name="getParentMatch">Gets the property value to match the parent object on.</param>
	/// <param name="getChildMatch">Gets the property value to match the child object on.</param>
	public FieldType AddQueryCollection<CHILD, MATCH>(MethodInfo methodInfo, Func<T, MATCH> getParentMatch, Func<CHILD, MATCH> getChildMatch)
		where CHILD : notnull
		where MATCH : notnull
	{
		methodInfo.ReturnType.IsAny<CHILD[], IEnumerable<CHILD>, Task<CHILD[]>, Task<IEnumerable<CHILD>>, ValueTask<CHILD[]>, ValueTask<IEnumerable<CHILD>>>().ThrowIfFalse();

		var fieldType = new FieldType()
		{
			Arguments = new QueryArguments(methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
					&& !parameterInfo.ParameterType.IsAny<IResolveFieldContext, MATCH[], IEnumerable<MATCH>>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.ToGraphQLType())
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
