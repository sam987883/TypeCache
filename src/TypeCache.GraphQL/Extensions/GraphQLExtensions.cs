// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Data;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.GraphQL.Types;
using static System.FormattableString;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	public static FieldType AddField(this IComplexGraphType @this, MethodInfo methodInfo, IFieldResolver resolver)
	{
		var fieldType = methodInfo.ToFieldType();
		fieldType.Resolver = resolver;
		return @this.AddField(fieldType);
	}

	public static FieldType AddField(this IComplexGraphType @this, MethodInfo methodInfo, ISourceStreamResolver resolver)
	{
		var fieldType = methodInfo.ToFieldType();
		fieldType.StreamResolver = resolver;
		return @this.AddField(fieldType);
	}

	public static FieldType AddField(this IComplexGraphType @this, PropertyInfo propertyInfo, IFieldResolver resolver)
	{
		var fieldType = propertyInfo.ToFieldType();
		fieldType.Resolver = resolver;
		return @this.AddField(fieldType);
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
		(objectType is ObjectType.Delegate).AssertFalse();
		(objectType is ObjectType.Object).AssertFalse();

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

	public static FieldType ToFieldType(this MethodInfo @this)
		=> new()
		{
			Arguments = new QueryArguments(@this.GetParameters()
				.Where(parameterInfo => !parameterInfo.GraphQLIgnore() && !parameterInfo.ParameterType.Is<IResolveFieldContext>())
				.Select(parameterInfo => new QueryArgument(parameterInfo.ToGraphQLType())
				{
					Name = parameterInfo.GraphQLName(),
					Description = parameterInfo.GraphQLDescription(),
				})),
			Name = @this.GraphQLName(),
			Description = @this.GraphQLDescription(),
			DeprecationReason = @this.GraphQLDeprecationReason(),
			Type = @this.ReturnType.ToGraphQLType(false).ToNonNullGraphType()
		};

	public static FieldType ToFieldType(this PropertyInfo @this)
	{
		var type = @this.ToGraphQLType(false);
		var arguments = new QueryArguments();

		if (type.IsAssignableTo<ScalarGraphType>() && !type.Implements(typeof(NonNullGraphType<>)))
			arguments.Add("null", type, description: "Return this value instead of null.");

		if (@this.PropertyType.IsAssignableTo<IFormattable>())
			arguments.Add<string>("format", nullable: true, description: "Use .NET format specifiers to format the data.");

		if (type.Is<GraphQLScalarType<DateTime>>() || type.Is<NonNullGraphType<GraphQLScalarType<DateTime>>>())
			arguments.Add<string>("timeZone", nullable: true, description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, [..., ...] | [UTC, ...])"));
		else if (type.Is<GraphQLScalarType<DateTimeOffset>>() || type.Is<NonNullGraphType<GraphQLScalarType<DateTimeOffset>>>())
			arguments.Add<string>("timeZone", nullable: true, description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, ...)"));
		else if (type.Is<GraphQLScalarType<string>>() || type.Is<NonNullGraphType<GraphQLScalarType<string>>>())
		{
			arguments.Add<StringCase>("case", nullable: true, description: "Convert string value to upper or lower case.");
			arguments.Add<int>("length", nullable: true, description: "Exclude the rest of the string value if it exceeds this length.");
			arguments.Add<string>("match", nullable: true, description: "Returns the matching result based on the specified regular expression pattern, null if no match.");
			arguments.Add<string>("trim", nullable: true, description: Invariant($"{typeof(string).Namespace}.{nameof(String)}.{nameof(string.Trim)}(value)"));
			arguments.Add<string>("trimEnd", nullable: true, description: Invariant($"{typeof(string).Namespace}.{nameof(String)}.{nameof(string.TrimEnd)}(value)"));
			arguments.Add<string>("trimStart", nullable: true, description: Invariant($"{typeof(string).Namespace}.{nameof(String)}.{nameof(string.TrimStart)}(value)"));
		}

		return new()
		{
			Arguments = arguments,
			Type = type,
			Name = @this.GraphQLName(),
			Description = @this.GraphQLDescription(),
			DeprecationReason = @this.GraphQLDeprecationReason(),
		};
	}

	public static FieldType ToInputFieldType(this PropertyInfo @this)
		=> new()
		{
			Type = @this.ToGraphQLType(true),
			Name = @this.GraphQLName(),
			Description = @this.GraphQLDescription(),
			DeprecationReason = @this.GraphQLDeprecationReason()
		};
}
