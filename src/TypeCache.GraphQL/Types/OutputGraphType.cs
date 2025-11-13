// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.Reflection;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <inheritdoc cref="GraphType"/>
/// </summary>
public sealed class OutputGraphType<T> : ObjectGraphType<T>
	where T : notnull
{
	public OutputGraphType()
		: this(Type<T>.Attributes.GraphQLName ?? typeof(T).GraphQLName)
	{
	}

	public OutputGraphType(string name)
	{
		this.Name = name;
		this.Description = Type<T>.Attributes.GraphQLDescription
			?? Invariant($"{Type<T>.AssemblyName}: {Type<T>.Namespace}.{Type<T>.CodeName}");
		this.DeprecationReason = Type<T>.Attributes.GraphQLDeprecationReason;

		Type<T>.Properties.Values.Where(_ => _.CanRead && !_.Attributes.GraphQLIgnore).ForEach(_ => this.AddField(_.ToFieldType()));
		Type<T>.Interfaces.Select(_ => _.ToType()).Where(_ => !_.IsGenericType).ForEach(this.Interfaces.Add);
	}

	public FieldType AddField(PropertyEntity property)
		=> this.AddField(property.ToFieldType());

	/// <summary>
	/// Adds a field that is based on the result of the <paramref name="method"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="method"/>.<br/>
	/// The <paramref name="method"/> parameters become the graph's query arguments.
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public FieldType AddField<ITEM>(StaticMethodEntity method)
		where ITEM : notnull
	{
		var arguments = method.Parameters
			.Where(_ => !_.Attributes.GraphQLIgnore
				&& !_.ParameterType.IsAssignableTo<IResolveFieldContext>()
				&& !_.ParameterType.IsAssignableTo<CancellationToken>())
			.Select(_ => new QueryArgument(_.ToGraphQLType())
			{
				Name = _.Attributes.GraphQLName ?? _.Name,
				Description = _.Attributes.GraphQLDescription,
			});
		var fieldType = new FieldType()
		{
			Arguments = new(arguments),
			Name = method.Attributes.GraphQLName ?? method.Name,
			Description = method.Attributes.GraphQLDescription,
			DeprecationReason = method.Attributes.GraphQLDeprecationReason,
			Resolver = new ItemFieldResolver<ITEM>(method),
			Type = typeof(OutputGraphType<ITEM>)
		};
		return this.AddField(fieldType);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="method"/>,
	/// whose result objects are reduced by <paramref name="getResult"/> based on state data from <see cref="IResolveFieldContext"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="method"/>.
	/// </summary>
	/// <param name="method">The method that loads data for all result items (<typeparamref name="ITEM"/>[]).  This method can contain user input query parameters.</param>
	/// <param name="getResult">Reduces the data returned by <paramref name="method"/>.</param>
	public FieldType AddField<ITEM>(StaticMethodEntity method, Func<T, ITEM[], ITEM> getResult)
		where ITEM : notnull
	{
		var arguments = method.Parameters
			.Where(_ => !_.Attributes.GraphQLIgnore
				&& !_.ParameterType.IsAssignableTo<IResolveFieldContext>()
				&& !_.ParameterType.IsAssignableTo<CancellationToken>())
			.Select(_ => new QueryArgument(_.ToGraphQLType())
			{
				Name = _.Attributes.GraphQLName ?? _.Name,
				Description = _.Attributes.GraphQLDescription,
			});
		var fieldType = new FieldType()
		{
			Arguments = new(arguments),
			Name = method.Attributes.GraphQLName ?? method.Name,
			Description = method.Attributes.GraphQLDescription,
			DeprecationReason = method.Attributes.GraphQLDeprecationReason,
			Resolver = new BatchItemFieldResolver<T, ITEM>(method, getResult),
			Type = typeof(OutputGraphType<ITEM>)
		};
		return this.AddField(fieldType);
	}

	/// <summary>
	/// Adds a field that is based on the results of the <paramref name="method"/>,
	/// whose result objects are reduced by <paramref name="getResult"/> based on state data from <see cref="IResolveFieldContext"/>.<br/>
	/// The name of the field is the name or <see cref="GraphQLNameAttribute"/> of the <paramref name="method"/>.
	/// </summary>
	/// <param name="method">The method that loads data for all result items.  This method can contain user input query parameters.</param>
	/// <param name="getResult">Reduces the data returned by <paramref name="method"/>.</param>
	public FieldType AddField<ITEM>(StaticMethodEntity method, Func<T, ITEM[], ITEM[]> getResult)
		where ITEM : notnull
	{
		var arguments = method.Parameters
			.Where(_ => !_.Attributes.GraphQLIgnore
				&& !_.ParameterType.IsAssignableTo<IResolveFieldContext>()
				&& !_.ParameterType.IsAssignableTo<CancellationToken>())
			.Select(_ => new QueryArgument(_.ToGraphQLType())
			{
				Name = _.Attributes.GraphQLName ?? _.Name,
				Description = _.Attributes.GraphQLDescription,
			});
		var fieldType = new FieldType()
		{
			Arguments = new(arguments),
			Name = method.Attributes.GraphQLName ?? method.Name,
			Description = method.Attributes.GraphQLDescription,
			DeprecationReason = method.Attributes.GraphQLDeprecationReason,
			Resolver = new BatchCollectionFieldResolver<T, ITEM>(method, getResult),
			Type = typeof(OutputGraphType<ITEM>)
		};
		return this.AddField(fieldType);
	}
}
