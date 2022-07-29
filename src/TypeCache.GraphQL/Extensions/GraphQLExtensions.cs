// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SQL;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	/// <summary>
	/// Use this to create a Graph QL Connection object to return in your endpoint to support paging.
	/// </summary>
	/// <typeparam name="T">.</typeparam>
	/// <param name="data">The data<see cref="IEnumerable{T}"/>.</param>
	/// <param name="totalCount">The total record count of the record set being paged.</param>
	/// <param name="pager">The Pager used to retrieve the record set.</param>
	/// <returns>The <see cref="Connection{T}"/>.</returns>
	public static Connection<T> ToConnection<T>(this IEnumerable<T> data, int totalCount, Pager pager)
		where T : class
	{
		var items = data.ToArray();
		var start = pager.After + 1;
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
				HasPreviousPage = pager.After > 0
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

	internal static FieldType ToFieldType<T>(this MethodMember @this, SqlApiController<T> controller, string table)
		where T : class, new()
		=> new()
		{
			Arguments = @this.Parameters.ToQueryArguments(),
			Name = string.Format(@this.GraphQLName(), table),
			Description = string.Format(@this.GraphQLDescription() ?? string.Empty, table),
			DeprecationReason = @this.GraphQLDeprecationReason(),
			Resolver = new MethodFieldResolver(@this, controller),
			Type = @this.Return.GraphQLType()
		};

	public static FieldType ToFieldType<T>(this PropertyMember @this)
	{
		var type = @this.GraphQLType(false);
		var arguments = new QueryArguments();

		if (type.Implements<ScalarGraphType>())
			arguments.Add(new QueryArgument(type)
			{
				Name = "null",
				Description = "Return this value instead of null."
			});

		if (type.Is<DateTimeGraphType>() || type.Is<NonNullGraphType<DateTimeGraphType>>())
		{
			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "timeZone",
				Description = "Converts the DateTime value to the specified time zone which must be supported by .Net's TimeZoneInfo class.  Use a comma to separate `from,to` time zones otherwise UTC will be assumed for the from value."
			});
		}
		if (type.Is<DateTimeOffsetGraphType>() || type.Is<NonNullGraphType<DateTimeOffsetGraphType>>())
		{
			arguments.Add(new QueryArgument<StringGraphType>()
			{
				Name = "timeZone",
				Description = "Converts the DateTimeOffset value to the specified time zone which must be supported by .Net's TimeZoneInfo class."
			});
		}
		else if (type.Is<StringGraphType>()
			|| type.Is<NonNullGraphType<StringGraphType>>())
		{
			if (@this.PropertyType.SystemType is SystemType.Boolean
				|| @this.PropertyType.Is<Nullable<bool>>())
			{
				arguments.Add(new QueryArgument<StringGraphType>()
				{
					Name = "true",
					Description = "Return this value when property value is true."
				});
				arguments.Add(new QueryArgument<StringGraphType>()
				{
					Name = "false",
					Description = "Return this value when property value is false."
				});
			}

			arguments.Add(new QueryArgument<GraphQLEnumType<StringCase>>()
			{
				Name = "case",
				Description = "Convert string value to upper or lower case."
			});

			if (@this.PropertyType.Implements<IFormattable>())
				arguments.Add(new QueryArgument<NonNullGraphType<StringGraphType>>()
				{
					Name = "format",
					Description = "Use .NET format specifiers to format the data."
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

	private static QueryArguments ToQueryArguments(this IEnumerable<MethodParameter> @this)
		=> new QueryArguments(@this
			.If(parameter => !parameter.GraphQLIgnore() && !parameter.Type.Handle.Is<IResolveFieldContext>())
			.Map(parameter => new QueryArgument(parameter.GraphQLType())
			{
				Name = parameter.GraphQLName(),
				Description = parameter.GraphQLDescription(),
			}));
}
