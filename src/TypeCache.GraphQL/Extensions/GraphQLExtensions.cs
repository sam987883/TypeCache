// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	public static FieldType AddField<T>(this IComplexGraphType @this, string name, IFieldResolver resolver)
		=> @this.AddField(new()
		{
			Name = name,
			Type = typeof(T).ToGraphQLType(false),
			Resolver = resolver
		});

	public static FieldType AddField(this IComplexGraphType @this, string name, IGraphType resolvedType, IFieldResolver resolver)
		=> @this.AddField(new()
		{
			Name = name,
			ResolvedType = resolvedType,
			Resolver = resolver
		});

	public static FieldType AddField(this IComplexGraphType @this, MethodInfo methodInfo, IFieldResolver resolver)
		=> @this.AddField(new()
		{
			Arguments = new QueryArguments(methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.Is<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.ToGraphQLType())
				{
					Name = parameterInfo.GraphQLName(),
					Description = parameterInfo.GraphQLDescription(),
				})),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			Resolver = resolver,
			Type = methodInfo.ReturnType.ToGraphQLType(false).ToNonNullGraphType()
		});

	public static FieldType AddField(this IComplexGraphType @this, MethodInfo methodInfo, ISourceStreamResolver resolver)
		=> @this.AddField(new()
		{
			Arguments = new QueryArguments(methodInfo.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.Is<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.ToGraphQLType())
				{
					Name = parameterInfo.GraphQLName(),
					Description = parameterInfo.GraphQLDescription(),
				})),
			Name = methodInfo.GraphQLName(),
			Description = methodInfo.GraphQLDescription(),
			DeprecationReason = methodInfo.GraphQLDeprecationReason(),
			StreamResolver = resolver,
			Type = methodInfo.ReturnType.ToGraphQLType(false).ToNonNullGraphType()
		});

	public static FieldType AddField(this IComplexGraphType @this, PropertyInfo propertyInfo, IFieldResolver resolver)
	{
		var type = propertyInfo.ToGraphQLType(false);
		var arguments = new QueryArguments();

		if (type.IsAssignableTo<ScalarGraphType>() && !type.Implements(typeof(NonNullGraphType<>)))
			arguments.Add("null", type, description: "Return this if the value is null.");

		if (propertyInfo.PropertyType.IsAssignableTo<IFormattable>())
			arguments.Add<string>("format", nullable: true, description: "Use .NET format specifiers to format the data.");

		if (type.Is<GraphQLScalarType<DateTime>>() || type.Is<NonNullGraphType<GraphQLScalarType<DateTime>>>())
			arguments.Add<string>("timeZone", nullable: true, description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, [..., ...] | [UTC, ...])"));
		else if (type.Is<GraphQLScalarType<DateTimeOffset>>() || type.Is<NonNullGraphType<GraphQLScalarType<DateTimeOffset>>>())
			arguments.Add<string>("timeZone", nullable: true, description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, ...)"));
		else if (type.Is<GraphQLScalarType<string>>() || type.Is<NonNullGraphType<GraphQLScalarType<string>>>())
		{
			arguments.Add<StringCase?>("case", description: "value.ToLower(), value.ToLowerInvariant(), value.ToUpper(), value.ToUpperInvariant()");
			arguments.Add<int?>("length", description: "value.Left(length)");
			arguments.Add<string>("match", nullable: true, description: "value.ToRegex(RegexOptions.Compiled | RegexOptions.Singleline).Match(match).");
			arguments.Add<string>("trim", nullable: true, description: "value.Trim()");
			arguments.Add<string>("trimEnd", nullable: true, description: "value.TrimEnd()");
			arguments.Add<string>("trimStart", nullable: true, description: "value.TrimStart()");
		}

		return @this.AddField(new()
		{
			Arguments = arguments,
			Type = type,
			Name = propertyInfo.GraphQLName(),
			Description = propertyInfo.GraphQLDescription(),
			DeprecationReason = propertyInfo.GraphQLDeprecationReason(),
			Resolver = resolver
		});
	}

	public static void AddOrderBy(this EnumerationGraphType @this, string column, string? deprecationReason = null)
	{
		var asc = Sort.Ascending.ToSQL();
		var desc = Sort.Descending.ToSQL();
		@this.Add(Invariant($"{column}_{asc}"), Invariant($"{column} {asc}"), Invariant($"{column} {asc}"), deprecationReason);
		@this.Add(Invariant($"{column}_{desc}"), Invariant($"{column} {desc}"), Invariant($"{column} {desc}"), deprecationReason);
	}

	/// <summary>
	/// Use this to create a GraphQL Connection object to return in your endpoint to support paging.
	/// </summary>
	/// <typeparam name="T">.</typeparam>
	/// <param name="data">The data<see cref="IEnumerable{T}"/>.</param>
	/// <param name="offset">The number of records to skip.</param>
	/// <param name="totalCount">The total record count of the record set being paged.</param>
	/// <returns>The <see cref="Connection{T}"/>.</returns>
	public static Connection<T> ToConnection<T>(this IEnumerable<T> data, uint offset, int totalCount)
		where T : notnull
	{
		var items = data.AsArray();
		return new(offset, items)
		{
			PageInfo = new(offset, offset + (uint)items.Length, totalCount),
			TotalCount = totalCount
		};
	}

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLEnumType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLEnumType(this Type @this)
		=> typeof(GraphQLEnumType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLInputType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLInputType(this Type @this)
		=> typeof(GraphQLInputType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLInterfaceType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLInterfaceType(this Type @this)
		=> typeof(GraphQLInterfaceType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLObjectType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLObjectType(this Type @this)
		=> typeof(GraphQLObjectType<>).MakeGenericType(@this);

	public static Type ToGraphQLType(this ParameterInfo @this)
	{
		var type = @this.GraphQLType() ?? @this.ParameterType!.ToGraphQLType(true);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this PropertyInfo @this, bool isInputType)
	{
		var type = @this.GraphQLType() ?? @this.PropertyType.ToGraphQLType(isInputType);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this Type @this, bool isInputType)
	{
		if (@this.Is(typeof(Nullable<>)))
			return @this.GenericTypeArguments[0].ToGraphQLType(isInputType);

		if (@this.IsEnum)
			return @this.ToGraphQLEnumType();

		var objectType = @this.GetObjectType();
		(objectType is ObjectType.Delegate).ThrowIfTrue();
		(objectType is ObjectType.Object).ThrowIfTrue();

		var scalarGraphType = @this.GetScalarType().ToGraphType();
		if (scalarGraphType is not null)
			return scalarGraphType;

		var collectionType = @this.GetCollectionType();
		if (collectionType.IsDictionary())
			return typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).ToGraphQLType(isInputType).ToNonNullGraphType().ToListGraphType();

		if (objectType is ObjectType.Task || objectType is ObjectType.ValueTask)
			return @this.IsGenericType
				? @this.GenericTypeArguments.First()!.ToGraphQLType(false)
				: throw new ArgumentOutOfRangeException(nameof(@this), Invariant($"{nameof(Task)} and {nameof(ValueTask)} are not allowed as GraphQL types."));

		if (@this.HasElementType)
		{
			var elementType = @this.GetElementType()!.ToGraphQLType(isInputType);
			if (elementType.IsValueType && !elementType.Is(typeof(Nullable<>)))
				elementType = elementType.ToNonNullGraphType();

			return elementType.ToListGraphType();
		}

		if (@this.Is(typeof(IEnumerable<>)) || @this.Implements(typeof(IEnumerable<>)))
			return @this.GenericTypeArguments.First()!.ToGraphQLType(isInputType).ToListGraphType();

		if (@this.IsInterface)
			return @this.ToGraphQLInterfaceType();

		return isInputType ? @this.ToGraphQLInputType() : @this.ToGraphQLObjectType();
	}

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(ListGraphType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToListGraphType(this Type @this)
		=> typeof(ListGraphType<>).MakeGenericType(@this);

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(NonNullGraphType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToNonNullGraphType(this Type @this)
		=> typeof(NonNullGraphType<>).MakeGenericType(@this);
}
