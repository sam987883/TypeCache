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
using GraphQLParser.AST;
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

	public static void AddOrderBy(this EnumerationGraphType @this, OrderBy orderBy, string? deprecationReason = null)
		=> @this.Add(orderBy.Display, orderBy.ToString(), orderBy.ToString(), deprecationReason);

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
		var objectType = @this.GetObjectType();
		(objectType is ObjectType.Delegate).AssertFalse();
		(objectType is ObjectType.Object).AssertFalse();

		if (objectType is ObjectType.Dictionary || objectType is ObjectType.ReadOnlyDictionary)
			return typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).ToGraphQLType(isInputType).ToNonNullGraphType().ToListGraphType();

		if (@this.IsEnum)
			return @this.ToGraphQLEnumType();

		var systemType = @this.GetSystemType();
		if (systemType is SystemType.Task || systemType is SystemType.ValueTask)
			return @this.IsGenericType
				? @this.GenericTypeArguments.First()!.ToGraphQLType(false)
				: throw new ArgumentOutOfRangeException(nameof(@this), Invariant($"{nameof(Task)} and {nameof(ValueTask)} are not allowed as GraphQL types."));

		if (systemType is SystemType.Nullable)
			return @this.GenericTypeArguments.First()!.ToGraphQLType(isInputType);

		var systemGraphType = systemType.ToGraphType();
		if (systemGraphType is not null)
			return systemGraphType;

		if (@this.HasElementType)
		{
			var elementType = @this.GetElementType()!.ToGraphQLType(isInputType);
			if (elementType.IsValueType && elementType.GetSystemType() is not SystemType.Nullable)
				elementType = elementType.ToNonNullGraphType();

			return elementType.ToListGraphType();
		}

		if (@this.IsGenericType && @this.IsOrImplements(typeof(IEnumerable<>)))
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
			arguments.Add<StringGraphType>("format", description: "Use .NET format specifiers to format the data.");

		if (type.Is<DateTimeGraphType>() || type.Is<NonNullGraphType<DateTimeGraphType>>())
			arguments.Add<StringGraphType>("timeZone", description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, [..., ...] | [UTC, ...])"));
		else if (type.Is<DateTimeOffsetGraphType>() || type.Is<NonNullGraphType<DateTimeOffsetGraphType>>())
			arguments.Add<StringGraphType>("timeZone", description: Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, ...)"));
		else if (type.Is<StringGraphType>() || type.Is<NonNullGraphType<StringGraphType>>())
		{
			arguments.Add<GraphQLEnumType<StringCase>>("case", description: "Convert string value to upper or lower case.");
			arguments.Add<IntGraphType>("length", description: "Exclude the rest of the string value if it exceeds this length.");
			arguments.Add<StringGraphType>("match", description: "Returns the matching result based on the specified regular expression pattern, null if no match.");
			arguments.Add<StringGraphType>("trim", description: Invariant($"{typeof(string).Namespace}.{nameof(String)}.{nameof(string.Trim)}(value)"));
			arguments.Add<StringGraphType>("trimEnd", description: Invariant($"{typeof(string).Namespace}.{nameof(String)}.{nameof(string.TrimEnd)}(value)"));
			arguments.Add<StringGraphType>("trimStart", description: Invariant($"{typeof(string).Namespace}.{nameof(String)}.{nameof(string.TrimStart)}(value)"));
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
