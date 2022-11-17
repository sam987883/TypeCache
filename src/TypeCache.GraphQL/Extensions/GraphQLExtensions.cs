// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SqlApi;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	public static void AddOrderBy(this EnumerationGraphType @this, OrderBy orderBy, string? deprecationReason = null)
		=> @this.Add(new(orderBy.Display, orderBy.ToString())
		{
			Description = orderBy.ToString(),
			DeprecationReason = deprecationReason
		});

	/// <summary>
	/// Use this to create a Graph QL Connection object to return in your endpoint to support paging.
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
			Edges = items.Map((item, i) => new Edge<T>
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

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Type ToGraphQLInputType(this Type @this)
		=> typeof(GraphQLInputType<>).MakeGenericType(@this);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Type ToGraphQLObjectType(this Type @this)
		=> typeof(GraphQLObjectType<>).MakeGenericType(@this);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Type ToListGraphType(this Type @this)
		=> typeof(ListGraphType<>).MakeGenericType(@this);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Type ToNonNullGraphType(this Type @this)
		=> typeof(NonNullGraphType<>).MakeGenericType(@this);

	public static FieldType ToFieldType(this MethodMember @this, IFieldResolver resolver)
	{
		var fieldType = @this.ToFieldType();
		fieldType.Resolver = resolver;
		return fieldType;
	}

	public static FieldType ToFieldType(this MethodMember @this, ISourceStreamResolver resolver)
	{
		var fieldType = @this.ToFieldType();
		fieldType.StreamResolver = resolver;
		return fieldType;
	}

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static FieldType ToFieldType(this MethodMember @this, object? controller)
	{
		var fieldType = @this.ToFieldType();
		if (@this.Return.Type.Is(typeof(IObservable<>)) || @this.Return.Type.Implements(typeof(IObservable<>)))
			fieldType.StreamResolver = new MethodSourceStreamResolver(@this, controller);
		else
			fieldType.Resolver = new MethodFieldResolver(@this, controller);
		return fieldType;
	}

	internal static FieldType ToFieldType(this MethodMember @this)
		=> new()
		{
			Arguments = @this.Parameters.ToQueryArguments(),
			Name = @this.GraphQLName(),
			Description = @this.GraphQLDescription(),
			DeprecationReason = @this.GraphQLDeprecationReason(),
			Type = @this.Return.GraphQLType()
		};

	public static FieldType ToFieldType<T>(this PropertyMember @this)
	{
		var type = @this.GraphQLType(false);
		var arguments = new QueryArguments();

		if (type.Implements<ScalarGraphType>() && !type.Implements(typeof(NonNullGraphType<>)))
		{
			arguments.Add(new QueryArgument(type)
			{
				Name = "null",
				Description = "Return this value instead of null."
			});
		}

		if (@this.PropertyType.Implements<IFormattable>())
		{
			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "format",
				Description = "Use .NET format specifiers to format the data."
			});
		}

		if (type.Is<DateTimeGraphType>() || type.Is<NonNullGraphType<DateTimeGraphType>>())
		{
			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "timeZone",
				Description = "Converts the DateTime value to the specified time zone which must be supported by `System.TimeZoneInfo`.  Use a comma to separate `from,to` time zones otherwise UTC will be assumed for the from value."
			});
		}

		if (type.Is<DateTimeOffsetGraphType>() || type.Is<NonNullGraphType<DateTimeOffsetGraphType>>())
		{
			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "timeZone",
				Description = "Converts the DateTimeOffset value to the specified time zone which must be supported by `System.TimeZoneInfo`."
			});
		}
		else if (type.Is<StringGraphType>()
			|| type.Is<NonNullGraphType<StringGraphType>>())
		{
			arguments.Add(new QueryArgument<GraphQLEnumType<StringCase>>()
			{
				Name = "case",
				Description = "Convert string value to upper or lower case."
			});

			arguments.Add(new QueryArgument<IntGraphType>()
			{
				Name = "length",
				Description = "Exclude the rest of the string value if it exceeds this length."
			});

			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "match",
				Description = "Returns the matching result based on the specified regular expression pattern, null if no match."
			});

			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "trim",
				Description = "Use .NET string.Trim to trim the string value of specified chars, or whitespaces if empty."
			});

			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "trimEnd",
				Description = "Use .NET string.TrimEnd to trim the string value of specified chars, or whitespaces if empty."
			});

			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "trimStart",
				Description = "Use .NET string.TrimStart to trim the string value of specified chars, or whitespaces if empty."
			});
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

	public static IEnumerable<OrderBy> ToOrderBy(this string[] @this)
	{
		var ascending = @this.Map(column => new OrderBy(column, Sort.Ascending));
		var descending = @this.Map(column => new OrderBy(column, Sort.Descending));
		var tokens = ascending.Union(descending).ToArray();
		tokens.Sort(new CustomComparer<OrderBy>((orderBy1, orderBy2) => StringComparer.Ordinal.Compare(orderBy1.Display, orderBy2.Display)));
		return tokens;
	}

	private static QueryArguments ToQueryArguments(this IEnumerable<MethodParameter> @this)
		=> new QueryArguments(@this
			.If(parameter => !parameter.GraphQLIgnore() && !parameter.Type.TypeHandle.Is<IResolveFieldContext>())
			.Map(parameter => new QueryArgument(parameter.GraphQLType())
			{
				Name = parameter.GraphQLName(),
				Description = parameter.GraphQLDescription(),
			}));
}
