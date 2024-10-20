// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using GraphQL;
using GraphQL.Introspection;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// Represents a GrahpQL class for all output object graph types.
/// </summary>
/// <typeparam name="TSourceType">The type of the object that this graph represents. More specifically, the .NET type of the source property within field resolvers for this graph.</typeparam>
public sealed class GraphQLObjectType<[NotAGraphType] T> : GraphQLObjectType
	where T : notnull
{
	public GraphQLObjectType()
		: this(typeof(T).GraphQLName())
	{
	}

	public GraphQLObjectType(string name)
	{
		if (typeof(IGraphType).IsAssignableFrom(typeof(T)) && this.GetType() != typeof(__Type))
			throw new InvalidOperationException($"Cannot use graph type '{typeof(T).Name}' as a model for graph type '{this.GetType().Name}'.");

		this.Name = name;
		this.Description = typeof(T).GraphQLDescription() ?? Invariant($"{typeof(T).Assembly.GetName().Name}: {typeof(T).Namespace}.{typeof(T).Name()}");
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();
		this.TypeHandle = typeof(T).TypeHandle;
		this.Interfaces.UnionWith(typeof(T).GetInterfaces().Where(_ => !_.HasElementType && !_.IsGenericType).Select(_ => _.TypeHandle));

		typeof(T).GetPublicProperties()
			.Where(propertyInfo => propertyInfo.CanRead && !propertyInfo.GraphQLIgnore())
			.ToArray()
			.ForEach(propertyInfo => this.AddField(propertyInfo));
	}

	public RuntimeTypeHandle TypeHandle { get; } = typeof(T).TypeHandle;

	public FieldType AddField(PropertyInfo propertyInfo)
	{
		var fieldType = new FieldType(propertyInfo);
		this.Fields.Add(fieldType);
		return fieldType;
	}

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
		var fieldType = new FieldType()
		{
			Arguments = methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.IsAssignableTo<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLName(), parameterInfo.ToGraphQLType())
				{
					Description = parameterInfo.GraphQLDescription(),
				})
				.ToArray(),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new ItemFieldResolver<ITEM>(methodInfo),
			Type = typeof(GraphQLObjectType<ITEM>)
		};
		this.Fields.Add(fieldType);
		return fieldType;
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="methodInfo"/>,
	/// whose result objects are reduced by <paramref name="getResult"/> based on state data from <see cref="IResolveFieldContext"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="methodInfo"/>.
	/// </summary>
	/// <param name="methodInfo">The method that loads data for all result items (<see cref="ITEM[]"/>).  This method can contain user input query parameters.</param>
	/// <param name="getResult">Reduces the data returned by <paramref name="methodInfo"/>.</param>
	public FieldType AddField<ITEM>(MethodInfo methodInfo, Func<T, ITEM[], ITEM> getResult)
		where ITEM : notnull
	{
		var fieldType = new FieldType()
		{
			Arguments = methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore()
					&& !parameterInfo.ParameterType.IsAssignableTo<IResolveFieldContext>()
					&& !parameterInfo.ParameterType.IsAssignableTo<CancellationToken>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLName(), parameterInfo.ToGraphQLType())
				{
					Description = parameterInfo.GraphQLDescription(),
				})
				.ToArray(),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new BatchItemFieldResolver<T, ITEM>(methodInfo, getResult),
			Type = typeof(GraphQLObjectType<ITEM>)
		};
		this.Fields.Add(fieldType);
		return fieldType;
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
		var fieldType = new FieldType()
		{
			Arguments = methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.IsAssignableTo<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.GraphQLName(), parameterInfo.ToGraphQLType())
				{
					Description = parameterInfo.GraphQLDescription(),
				})
				.ToArray(),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = new BatchCollectionFieldResolver<T, ITEM>(methodInfo, getResult),
			Type = typeof(GraphQLObjectType<ITEM>)
		};
		this.Fields.Add(fieldType);
		return fieldType;
	}
}

public class GraphQLObjectType : GraphType, IObjectGraphType
{
	public ISet<FieldType> Fields { get; } = new HashSet<FieldType>();

	public ISet<RuntimeTypeHandle> Interfaces { get; } = new HashSet<RuntimeTypeHandle>();

	public ISet<IInterfaceGraphType> ResolvedInterfaces { get; } = new HashSet<IInterfaceGraphType>();

	public RuntimeTypeHandle TypeHandle { get; }
}
