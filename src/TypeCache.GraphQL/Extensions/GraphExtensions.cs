// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection;

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

		internal static void AddSqlApiFieldType<T>(this ObjectGraphType @this, IMethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<Attribute, GraphAttribute>();
			var graphName = Class<T>.Attributes.First<Attribute, GraphAttribute>()?.Name ?? Class<T>.Name;
			graphName = string.Format(graphAttribute.Name, graphName);
			var graphType = method.ReturnAttributes.First<Attribute, GraphAttribute>()?.Type ?? method.GetGraphType(false);
			var description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null;
			var deprecationReason = method.Attributes.First<Attribute, ObsoleteAttribute>()?.Message;
			var resolver = new MethodFieldResolver<T[]>(method, handler);
			var arguments = new QueryArguments(method.Parameters
				.If(parameter =>
				{
					var graphAttribute = parameter.Attributes.First<Attribute, GraphAttribute>();
					return graphAttribute?.Ignore != true && !parameter.TypeHandle.Is<IResolveFieldContext>();
				})
				.To(parameter =>
				{
					if (parameter.Name.Is("output") || parameter.Name.Is("select"))
						return new QueryArgument(typeof(ListGraphType<GraphObjectEnumType<T>>))
						{
							Name = parameter.Name
						};
					return parameter.ToQueryArgument();
				}));

			var field = @this.FieldAsync(graphType, graphName, description, arguments, resolver.ResolveAsync, deprecationReason);
		}

		internal static void AddSqlFieldType<T>(this ObjectGraphType @this, IMethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<Attribute, GraphAttribute>();
			var graphName = Class<T>.Attributes.First<Attribute, GraphAttribute>()?.Name ?? Class<T>.Name;
			graphName = string.Format(graphAttribute.Name, graphName);
			var graphType = method.ReturnAttributes.First<Attribute, GraphAttribute>()?.Type ?? method.GetGraphType(false);
			var description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null;
			var deprecationReason = method.Attributes.First<Attribute, ObsoleteAttribute>()?.Message;
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
			var cursorProperty = Class<T>.Properties.FirstValue(property => property.Value.Attributes.Any<Attribute, GraphCursorAttribute>())?.Value;
			var items = data.ToArray();
			var connection = new Connection<T>
			{
				Edges = items.To(item => new Edge<T>
				{
					Cursor = cursorProperty != null ? cursorProperty[item]?.ToString() : null,
					Node = item
				}).ToList(),
				PageInfo = new PageInfo
				{
					StartCursor = items.Length > 0 ? cursorProperty[items[0]]?.ToString() : null,
					EndCursor = items.Length > 0 ? cursorProperty[items[^1]]?.ToString() : null,
					HasNextPage = hasNextPage,
					HasPreviousPage = hasPreviousPage
				},
				TotalCount = totalCount
			};
			connection.Items.AddRange(items);
			return connection;
		}

		public static FieldType CreateHandlerFieldType(this IMethodMember method, object handler)
		{
			var graphAttribute = method.Attributes.First<Attribute, GraphAttribute>();
			var graphType = method.ReturnAttributes.First<Attribute, GraphAttribute>()?.Type;
			return new FieldType
			{
				Type = graphType ?? method.GetGraphType(false),
				Name = graphAttribute?.Name ?? method.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = method.Attributes.First<Attribute, ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(method, handler),
				Arguments = method.ToQueryArguments()
			};
		}

		internal static FieldType CreateSqlApiFieldType<T>(this IMethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<Attribute, GraphAttribute>();
			var graphName = typeof(T).GetCustomAttribute<GraphAttribute>()?.Name ?? typeof(T).Name;
			var graphType = method.ReturnAttributes.First<Attribute, GraphAttribute>()?.Type ?? method.GetGraphType(false);
			return new FieldType
			{
				Type = typeof(ListGraphType<GraphObjectType<T>>),
				Name = string.Format(graphAttribute.Name, graphName),
				Description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null,
				DeprecationReason = method.Attributes.First<Attribute, ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(method, handler),
				Arguments = new QueryArguments(
					new QueryArgument(typeof(StringGraphType)) { Name = "where" }
				, new QueryArgument(typeof(ListGraphType<GraphInputType<Parameter>>)) { Name = "parameters" }
				)
			};
		}

		public static FieldType CreateFieldType(this IPropertyMember property, bool isInputType)
		{
			var graphAttribute = property.Attributes.First<Attribute, GraphAttribute>();
			return new FieldType
			{
				Type = graphAttribute?.Type ?? property.GetGraphType(isInputType),
				Name = graphAttribute?.Name ?? property.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = property.Attributes.First<Attribute, ObsoleteAttribute>()?.Message,
				Resolver = !isInputType ? new FuncFieldResolver<object>(context => context.Source) : null
			};
		}

		public static FieldType CreateFieldType(this ColumnSchema column)
		{
			return new FieldType
			{
				Type = column.TypeHandle.ToType(),
				Name = column.Name,
				Resolver = new FuncFieldResolver<object>(context => context.Source)
			};
		}

		public static Type GetGraphType(this IMember @this, bool isInputType)
		{
			var graphType = @this.TypeHandle.ToType();
			if (graphType.IsGenericType && (@this.IsNullable || @this.IsTask || @this.IsValueTask))
				graphType = graphType.GenericTypeArguments.First();

			if (graphType.IsArray && graphType != typeof(string))
				graphType = graphType.GetElementType();

			graphType = @this.NativeType.GetGraphType(graphType, isInputType);

			if (@this.CollectionType != CollectionType.None)
				graphType = typeof(ListGraphType<>).MakeGenericType(graphType);

			if ((!@this.IsNullable && @this.NativeType != NativeType.Object)
				|| @this.Attributes.Any<Attribute, NotNullAttribute>())
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		private static Type GetGraphType(this NativeType @this, Type type, bool isInputType)
			=> @this switch
			{
				NativeType.Object => (isInputType ? typeof(GraphInputType<>) : typeof(GraphObjectType<>)).MakeGenericType(type),
				NativeType.Enum => typeof(GraphEnumType<>).MakeGenericType(type),
				_ => _scalarGraphTypes[type.TypeHandle].ToType()
			};

		public static QueryArguments ToQueryArguments(this IMethodMember @this)
			=> new QueryArguments(@this.Parameters
				.If(parameter =>
				{
					var graphAttribute = parameter.Attributes.First<Attribute, GraphAttribute>();
					return graphAttribute?.Ignore != true && !parameter.TypeHandle.Is<IResolveFieldContext>();
				})
				.To(parameter => parameter.ToQueryArgument()));

		private static QueryArgument ToQueryArgument(this IParameter @this)
		{
			var graphAttribute = @this.Attributes.First<Attribute, GraphAttribute>();
			var graphType = graphAttribute?.Type;
			if (graphType == null)
			{
				graphType = @this.TypeHandle.ToType();
				if (graphType.IsGenericType)
				{
					var genericType = graphType.GetGenericTypeDefinition();
					var typeArgument = graphType.GenericTypeArguments.First();
					if (@this.IsNullable || @this.IsTask || @this.IsValueTask)
						graphType = typeArgument;

					if (genericType == typeof(Connection<>))
						graphType = typeof(GraphConnectionType<>).MakeGenericType(typeArgument);
				}

				if (graphType.IsArray && graphType != typeof(string))
					graphType = graphType.GetElementType();

				graphType = @this.NativeType.GetGraphType(graphType, true);

				if (@this.CollectionType != CollectionType.None)
					graphType = typeof(ListGraphType<>).MakeGenericType(graphType);

				if ((!@this.IsNullable && @this.NativeType != NativeType.Object)
					|| @this.Attributes.Any<Attribute, NotNullAttribute>())
					graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);
			}

			return new QueryArgument(graphType)
			{
				Name = graphAttribute?.Name ?? @this.Name,
				Description = graphAttribute?.Description
			};
		}
	}
}
