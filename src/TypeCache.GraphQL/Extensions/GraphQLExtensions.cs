// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using static System.FormattableString;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	public static FieldType[] AddFieldTypes<T>(this ComplexGraphType<T> @this, IEnumerable<FieldType> fieldTypes)
		=> fieldTypes.Select(@this.AddField).ToArray();

	public static void AddOrderBy(this EnumerationGraphType @this, OrderBy orderBy, string? deprecationReason = null)
		=> @this.Add(new(orderBy.Display, orderBy.ToString())
		{
			Description = orderBy.ToString(),
			DeprecationReason = deprecationReason
		});

	/// <summary>
	/// Use this to create a GraphQL Connection object to return in your endpoint to support paging.
	/// </summary>
	/// <typeparam name="T">.</typeparam>
	/// <param name="data">The data<see cref="IEnumerable{T}"/>.</param>
	/// <param name="totalCount">The total record count of the record set being paged.</param>
	/// <param name="offset">The number of records to skip.</param>
	/// <returns>The <see cref="Connection{T}"/>.</returns>
	public static Connection<T> ToConnection<T>(this IEnumerable<T> data, int totalCount, uint offset)
	{
		var items = data.ToArray();
		var start = offset + 1;
		var end = start + items.Length;
		var connection = new Connection<T>
		{
			Edges = items.Select((item, i) => new Edge<T>
			{
				Cursor = (start + i).ToString(),
				Node = item
			}).ToList(),
			PageInfo = new()
			{
				StartCursor = start.ToString(),
				EndCursor = end.ToString(),
				HasNextPage = end < totalCount,
				HasPreviousPage = offset > 0
			},
			TotalCount = totalCount
		};
		connection.Items!.AddRange(items);
		return connection;
	}

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLEnumType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLEnumType(this Type @this, bool isNonNull = false)
		=> isNonNull ? typeof(GraphQLEnumType<>).MakeGenericType(@this) : typeof(GraphQLEnumType<>).MakeGenericType(@this).ToNonNullGraphType();

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLInputType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLInputType(this Type @this, bool isNonNull = false)
		=> isNonNull ? typeof(GraphQLInputType<>).MakeGenericType(@this) : typeof(GraphQLInputType<>).MakeGenericType(@this).ToNonNullGraphType();

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLInterfaceType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLInterfaceType(this Type @this, bool isNonNull = false)
		=> isNonNull ? typeof(GraphQLInterfaceType<>).MakeGenericType(@this) : typeof(GraphQLInterfaceType<>).MakeGenericType(@this).ToNonNullGraphType();

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(GraphQLObjectType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGraphQLObjectType(this Type @this, bool isNonNull = false)
		=> isNonNull ? typeof(GraphQLObjectType<>).MakeGenericType(@this) : typeof(GraphQLObjectType<>).MakeGenericType(@this).ToNonNullGraphType();

	public static Type ToGraphQLType(this ParameterInfo @this)
	{
		var type = @this.GraphQLType() ?? @this.ParameterType!.ToGraphQLType(true, false);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this PropertyInfo @this, bool isInputType)
	{
		var type = @this.GraphQLType() ?? @this.PropertyType.ToGraphQLType(isInputType, false);
		if (!type.Is(typeof(NonNullGraphType<>)) && @this.HasCustomAttribute<NotNullAttribute>())
			type = type.ToNonNullGraphType();

		return type;
	}

	public static Type ToGraphQLType(this Type @this, bool isInputType, bool isNonNull)
	{
		var objectType = @this.GetObjectType();
		(objectType is ObjectType.Delegate).AssertFalse();
		(objectType is ObjectType.Object).AssertFalse();

		var systemType = @this.GetSystemType();
		return @this switch
		{
			{ IsGenericType: true } when systemType is SystemType.Nullable => @this.GenericTypeArguments.First().ToGraphQLType(isInputType, false),
			{ IsGenericType: true } when systemType.IsAny(SystemType.Task, SystemType.ValueTask) => @this.GenericTypeArguments.First().ToGraphQLType(false, isNonNull),
			{ IsEnum: true } => @this.ToGraphQLEnumType(isNonNull),
			_ when SystemGraphTypes.TryGetValue(systemType, out var handle) => isNonNull ? handle.ToType().ToNonNullGraphType() : handle.ToType(),
			{ HasElementType: true } => @this.GetElementType()!.ToGraphQLType(isInputType, true).ToListGraphType(isNonNull),
			{ IsGenericType: true } when objectType is ObjectType.Dictionary => typeof(KeyValuePair<,>).MakeGenericType(@this.GenericTypeArguments).ToGraphQLType(isInputType, true).ToListGraphType(),
			{ IsGenericType: true } => @this.GenericTypeArguments.First().ToGraphQLType(isInputType, true).ToListGraphType(isNonNull),
			{ IsInterface: true } => @this.ToGraphQLInterfaceType(isNonNull),
			_ when isInputType => @this.ToGraphQLInputType(isNonNull),
			_ => @this.ToGraphQLObjectType(isNonNull)
		};
	}

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(ListGraphType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToListGraphType(this Type @this, bool isNonNull = false)
		=> isNonNull ? typeof(ListGraphType<>).MakeGenericType(@this) : typeof(ListGraphType<>).MakeGenericType(@this).ToNonNullGraphType();

	/// <summary>
	/// <c>=&gt; <see langword="typeof"/>(NonNullGraphType&lt;&gt;).MakeGenericType(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToNonNullGraphType(this Type @this)
		=> typeof(NonNullGraphType<>).MakeGenericType(@this);

	internal static FieldType ToFieldType(this MethodInfo @this)
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
			Type = @this.ReturnType.ToGraphQLType(false, true)
		};

	public static FieldType ToFieldType<T>(this PropertyInfo @this)
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
			Resolver = new PropertyFieldResolver<T>(@this)
		};
	}

	public static FieldType ToInputFieldType(this PropertyInfo @this)
		=> new FieldType()
		{
			Type = @this.ToGraphQLType(true),
			Name = @this.GraphQLName(),
			Description = @this.GraphQLDescription(),
			DeprecationReason = @this.GraphQLDeprecationReason()
		};

	private static readonly IReadOnlyDictionary<SystemType, RuntimeTypeHandle> SystemGraphTypes = new Dictionary<SystemType, RuntimeTypeHandle>(26, EnumOf<SystemType>.Comparer)
	{
		{ SystemType.String, typeof(StringGraphType).TypeHandle },
		{ SystemType.Uri, typeof(UriGraphType).TypeHandle },
		{ SystemType.Boolean, typeof(BooleanGraphType).TypeHandle },
		{ SystemType.SByte, typeof(SByteGraphType).TypeHandle },
		{ SystemType.Int16, typeof(ShortGraphType).TypeHandle },
		{ SystemType.Int32, typeof(IntGraphType).TypeHandle },
		{ SystemType.Index, typeof(IntGraphType).TypeHandle },
		{ SystemType.Int64, typeof(LongGraphType).TypeHandle },
		{ SystemType.IntPtr, typeof(LongGraphType).TypeHandle },
		{ SystemType.Int128, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.Byte, typeof(ByteGraphType).TypeHandle },
		{ SystemType.UInt16, typeof(UShortGraphType).TypeHandle },
		{ SystemType.UInt32, typeof(UIntGraphType).TypeHandle },
		{ SystemType.UInt64, typeof(ULongGraphType).TypeHandle },
		{ SystemType.UIntPtr, typeof(ULongGraphType).TypeHandle },
		{ SystemType.UInt128, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.BigInteger, typeof(BigIntGraphType).TypeHandle },
		{ SystemType.Half, typeof(HalfGraphType).TypeHandle },
		{ SystemType.Single, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Double, typeof(FloatGraphType).TypeHandle },
		{ SystemType.Decimal, typeof(DecimalGraphType).TypeHandle },
		{ SystemType.DateOnly, typeof(DateOnlyGraphType).TypeHandle },
		{ SystemType.DateTime, typeof(DateTimeGraphType).TypeHandle },
		{ SystemType.DateTimeOffset, typeof(DateTimeOffsetGraphType).TypeHandle },
		{ SystemType.TimeOnly, typeof(TimeOnlyGraphType).TypeHandle },
		{ SystemType.TimeSpan, typeof(TimeSpanSecondsGraphType).TypeHandle },
		{ SystemType.Guid, typeof(GuidGraphType).TypeHandle },
		{ SystemType.Range, typeof(StringGraphType).TypeHandle }
	}.ToImmutableDictionary();
}
