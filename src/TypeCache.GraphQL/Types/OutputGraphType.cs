// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
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
public sealed class OutputGraphType<T> : ObjectGraphType<T>
	where T : notnull
{
	public OutputGraphType()
		: this(typeof(T).GraphQLName())
	{
	}

	public OutputGraphType(string name)
	{
		this.Name = name;
		this.Description = typeof(T).GraphQLDescription() ?? Invariant($"{typeof(T).Assembly.GetName().Name}: {typeof(T).Namespace}.{typeof(T).GetTypeName()}");
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.AddField(propertyInfo.ToFieldType()));

		typeof(T).GetInterfaces()
			.Where(_ => !_.HasElementType && !_.IsGenericType)
			.ToArray()
			.ForEach(this.Interfaces.Add);
	}

	public FieldType AddField(PropertyInfo propertyInfo)
		=> this.AddField(propertyInfo.ToFieldType());

	/// <summary>
	/// Adds a field that is based on the result of the <paramref name="methodInfo"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="methodInfo"/>.<br/>
	/// The <paramref name="methodInfo"/> parameters become the graph's query arguments.
	/// </summary>
	/// <remarks>The <paramref name="methodInfo"/> must be a static method.</remarks>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public FieldType AddField<ITEM>(MethodInfo methodInfo)
		where ITEM : notnull
	{
		var arguments = methodInfo.GetParameters()
			.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
				&& !parameterInfo.ParameterType.IsAssignableTo<IResolveFieldContext>()
				&& !parameterInfo.ParameterType.IsAssignableTo<CancellationToken>())
			.Select(parameterInfo => parameterInfo.ToQueryArgument());
		var fieldType = new FieldType()
		{
			Arguments = new(arguments),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new ItemFieldResolver<ITEM>(methodInfo),
			Type = typeof(OutputGraphType<ITEM>)
		};
		return this.AddField(fieldType);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="methodInfo"/>,
	/// whose result objects are reduced by <paramref name="getResult"/> based on state data from <see cref="IResolveFieldContext"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="methodInfo"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads data for all result items (<typeparamref name="ITEM"/>[]).  This method can contain user input query parameters.</param>
	/// <param name="getResult">Reduces the data returned by <paramref name="methodInfo"/>.</param>
	public FieldType AddField<ITEM>(MethodInfo methodInfo, Func<T, ITEM[], ITEM> getResult)
		where ITEM : notnull
	{
		var arguments = methodInfo.GetParameters()
			.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
				&& !parameterInfo.ParameterType.IsAssignableTo<IResolveFieldContext>()
				&& !parameterInfo.ParameterType.IsAssignableTo<CancellationToken>())
			.Select(parameterInfo => parameterInfo.ToQueryArgument());
		var fieldType = new FieldType()
		{
			Arguments = new(arguments),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new BatchItemFieldResolver<T, ITEM>(methodInfo, getResult),
			Type = typeof(OutputGraphType<ITEM>)
		};
		return this.AddField(fieldType);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="methodInfo"/>,
	/// whose result objects are reduced by <paramref name="getResult"/> based on state data from <see cref="IResolveFieldContext"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="methodInfo"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads data for all result items.  This method can contain user input query parameters.</param>
	/// <param name="getResult">Reduces the data returned by <paramref name="methodInfo"/>.</param>
	public FieldType AddField<ITEM>(MethodInfo methodInfo, Func<T, ITEM[], ITEM[]> getResult)
		where ITEM : notnull
	{
		var arguments = methodInfo.GetParameters()
			.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
				&& !parameterInfo.ParameterType.IsAssignableTo<IResolveFieldContext>()
				&& !parameterInfo.ParameterType.IsAssignableTo<CancellationToken>())
			.Select(parameterInfo => parameterInfo.ToQueryArgument());
		var fieldType = new FieldType()
		{
			Arguments = new(arguments),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new BatchCollectionFieldResolver<T, ITEM>(methodInfo, getResult),
			Type = typeof(OutputGraphType<ITEM>)
		};
		return this.AddField(fieldType);
	}
}
