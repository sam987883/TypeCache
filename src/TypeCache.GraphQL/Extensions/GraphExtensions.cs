﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Extensions
{
	public static class GraphExtensions
	{
		private static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> _scalarGraphTypes = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>()
		{
			{ typeof(bool).TypeHandle, typeof(BooleanGraphType).TypeHandle },
			{ typeof(sbyte).TypeHandle, typeof(SByteGraphType).TypeHandle },
			{ typeof(byte).TypeHandle, typeof(ByteGraphType).TypeHandle },
			{ typeof(short).TypeHandle, typeof(ShortGraphType).TypeHandle },
			{ typeof(ushort).TypeHandle, typeof(UShortGraphType).TypeHandle },
			{ typeof(int).TypeHandle, typeof(IntGraphType).TypeHandle },
			{ typeof(Index).TypeHandle, typeof(IntGraphType).TypeHandle },
			{ typeof(uint).TypeHandle, typeof(UIntGraphType).TypeHandle },
			{ typeof(long).TypeHandle, typeof(LongGraphType).TypeHandle },
			{ typeof(ulong).TypeHandle, typeof(ULongGraphType).TypeHandle },
			{ typeof(float).TypeHandle, typeof(FloatGraphType).TypeHandle },
			{ typeof(double).TypeHandle, typeof(FloatGraphType).TypeHandle },
			{ typeof(decimal).TypeHandle, typeof(DecimalGraphType).TypeHandle },
			{ typeof(DateTime).TypeHandle, typeof(DateTimeGraphType).TypeHandle },
			{ typeof(DateTimeOffset).TypeHandle, typeof(DateTimeOffsetGraphType).TypeHandle },
			{ typeof(TimeSpan).TypeHandle, typeof(TimeSpanSecondsGraphType).TypeHandle },
			{ typeof(Guid).TypeHandle, typeof(GuidGraphType).TypeHandle },
			{ typeof(Uri).TypeHandle, typeof(UriGraphType).TypeHandle },
			{ typeof(Range).TypeHandle, typeof(StringGraphType).TypeHandle },
			{ typeof(string).TypeHandle, typeof(StringGraphType).TypeHandle }
		};

		internal static void AddSqlApiFieldType<T>(this ObjectGraphType @this, MethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var graphName = TypeOf<T>.Attributes.First<GraphAttribute>()?.Name ?? TypeOf<T>.Name;
			graphName = string.Format(graphAttribute!.Name!, graphName);
			var hasNotNull = method.Return.Attributes.Any<NotNullAttribute>();
			var graphType = method.Return.Attributes.First<GraphAttribute>()?.Type ?? method.Return.Type.GetGraphType(hasNotNull, false);
			var description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null;
			var deprecationReason = method.Attributes.First<ObsoleteAttribute>()?.Message;
			var resolver = new MethodFieldResolver<T[]>(method, handler);
			var arguments = new QueryArguments(method.Parameters
				.If(parameter => parameter!.Attributes.First<GraphAttribute>()?.Ignore is not true && !parameter.Type.Is<IResolveFieldContext>())
				.To(parameter =>
				{
					if (parameter!.Name.Is("output") || parameter.Name.Is("select"))
						return new QueryArgument(typeof(ListGraphType<GraphObjectEnumType<T>>))
						{
							Name = parameter.Name
						};
					return parameter.ToQueryArgument();
				}));

			var field = @this.FieldAsync(graphType, graphName, description, arguments, resolver.ResolveAsync, deprecationReason);
		}

		internal static void AddSqlFieldType<T>(this ObjectGraphType @this, MethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var graphName = TypeOf<T>.Attributes.First<GraphAttribute>()?.Name ?? TypeOf<T>.Name;
			graphName = string.Format(graphAttribute!.Name!, graphName);
			var hasNotNull = method.Return.Attributes.Any<NotNullAttribute>();
			var graphType = method.Return.Attributes.First<GraphAttribute>()?.Type ?? method.Return.Type.GetGraphType(hasNotNull, false);
			var description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null;
			var deprecationReason = method.Attributes.First<ObsoleteAttribute>()?.Message;
			var resolver = new MethodFieldResolver(method, handler);
			var arguments = method.ToQueryArguments();

			var field = @this.Field(graphType, graphName, description, arguments, resolver.Resolve, deprecationReason);
		}

		/// <summary>
		/// Use this to create a GraphQL Connection object to return in your endpoint to support paging.
		/// </summary>
		/// <typeparam name="T">.</typeparam>
		/// <param name="data">The data<see cref="IEnumerable{T}"/>.</param>
		/// <param name="totalCount">The total record count of the record set being paged.</param>
		/// <param name="hasNextPage">Whether the paged set of records are not at the end of the complete record set.</param>
		/// <param name="hasPreviousPage">Whether the paged set of records are not at the start of the complete record set.</param>
		/// <returns>The <see cref="Connection{T}"/>.</returns>
		public static Connection<T> CreateConnection<T>(this IEnumerable<T> data, int totalCount, bool hasNextPage, bool hasPreviousPage)
			where T : class
		{
			var cursorProperty = TypeOf<T>.Properties.FirstValue(property => property.Value.Attributes.Any<GraphCursorAttribute>())?.Value;
			var items = data.ToArray();
			var connection = new Connection<T>
			{
				Edges = items.To(item => new Edge<T>
				{
					Cursor = cursorProperty?.GetValue!(item)?.ToString(),
					Node = item
				}).ToList(),
				PageInfo = new PageInfo
				{
					StartCursor = items.Length > 0 ? cursorProperty?.GetValue!(items[0])?.ToString() : null,
					EndCursor = items.Length > 0 ? cursorProperty?.GetValue!(items[^1])?.ToString() : null,
					HasNextPage = hasNextPage,
					HasPreviousPage = hasPreviousPage
				},
				TotalCount = totalCount
			};
			connection.Items.AddRange(items);
			return connection;
		}

		public static FieldType CreateHandlerFieldType(this MethodMember method, object handler)
		{
			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var graphType = method.Return.Attributes.First<GraphAttribute>()?.Type;
			var hasNotNull = method.Return.Attributes.Any<NotNullAttribute>();
			return new FieldType
			{
				Type = graphType ?? method.Return.Type.GetGraphType(hasNotNull, false),
				Name = graphAttribute?.Name ?? method.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = method.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(method, handler),
				Arguments = method.ToQueryArguments()
			};
		}

		internal static FieldType CreateSqlApiFieldType<T>(this MethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var graphName = typeof(T).GetCustomAttribute<GraphAttribute>()?.Name ?? typeof(T).Name;
			var hasNotNull = method.Return.Attributes.Any<NotNullAttribute>();
			var graphType = method.Return.Attributes.First<GraphAttribute>()?.Type ?? method.Return.Type.GetGraphType(hasNotNull, false);
			return new FieldType
			{
				Type = typeof(ListGraphType<GraphObjectType<T>>),
				Name = string.Format(graphAttribute!.Name!, graphName),
				Description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null,
				DeprecationReason = method.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(method, handler),
				Arguments = new QueryArguments(
					new QueryArgument(typeof(StringGraphType)) { Name = "where" }
				, new QueryArgument(typeof(ListGraphType<GraphInputType<TypeCache.Reflection.Parameter>>)) { Name = "parameters" }
				)
			};
		}

		public static FieldType CreateFieldType(this PropertyMember property, bool isInputType)
		{
			var graphAttribute = property.Attributes.First<GraphAttribute>();
			var hasNotNull = property.Attributes.Any<NotNullAttribute>();
			return new FieldType
			{
				Type = graphAttribute?.Type ?? property.Type.GetGraphType(hasNotNull, isInputType),
				Name = graphAttribute?.Name ?? property.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = property.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = !isInputType ? new FuncFieldResolver<object>(context => context.Source) : null
			};
		}

		public static FieldType CreateFieldType(this ColumnSchema column)
		{
			return new FieldType
			{
				Type = column.Type.ToType(),
				Name = column.Name,
				Resolver = new FuncFieldResolver<object>(context => context.Source)
			};
		}

		public static Type GetGraphType(this TypeMember @this, bool hasNotNull, bool isInputType)
		{
			var graphType = @this.Handle.ToType();

			if (graphType.IsArray && @this.SystemType != SystemType.String)
				graphType = graphType.GetElementType();
			else if (graphType.IsGenericType)
				graphType = graphType.GenericTypeArguments.First();

			graphType = @this.Kind switch
			{
				Kind.Enum => typeof(GraphEnumType<>).MakeGenericType(graphType!),
				Kind.Class or Kind.Interface or Kind.Struct when isInputType => typeof(GraphInputType<>).MakeGenericType(graphType!),
				Kind.Class or Kind.Interface or Kind.Struct => typeof(GraphObjectType<>).MakeGenericType(graphType!),
				_ => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}")
			};

			if (@this.IsEnumerable())
				graphType = typeof(ListGraphType<>).MakeGenericType(graphType);

			if (hasNotNull || !@this.IsNullable)
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		public static QueryArguments ToQueryArguments(this MethodMember @this)
			=> new QueryArguments(@this.Parameters
				.If(parameter =>
				{
					var graphAttribute = parameter!.Attributes.First<GraphAttribute>();
					return graphAttribute?.Ignore is not true && !parameter.Type.Handle.Is<IResolveFieldContext>();
				})
				.To(parameter => parameter!.ToQueryArgument()));

		private static QueryArgument ToQueryArgument(this TypeCache.Reflection.Parameter @this)
		{
			var graphAttribute = @this.Attributes.First<GraphAttribute>();
			var hasNotNull = @this.Attributes.Any<NotNullAttribute>();
			var graphType = graphAttribute?.Type ?? @this.Type.GetGraphType(hasNotNull, true);

			return new QueryArgument(graphType)
			{
				Name = graphAttribute?.Name ?? @this.Name,
				Description = graphAttribute?.Description
			};
		}
	}
}
