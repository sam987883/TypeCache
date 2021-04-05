// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
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
		/// <summary>
		/// Use this to create a Graph QL Connection object to return in your endpoint to support paging.
		/// </summary>
		/// <typeparam name="T">.</typeparam>
		/// <param name="data">The data<see cref="IEnumerable{T}"/>.</param>
		/// <param name="totalCount">The total record count of the record set being paged.</param>
		/// <param name="hasNextPage">Whether the paged set of records are not at the end of the complete record set.</param>
		/// <param name="hasPreviousPage">Whether the paged set of records are not at the start of the complete record set.</param>
		/// <returns>The <see cref="Connection{T}"/>.</returns>
		public static Connection<T> ToConnection<T>(this IEnumerable<T> data, int totalCount, bool hasNextPage, bool hasPreviousPage)
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

		internal static void AddField(this IObjectGraphType @this, MethodMember method, object handler)
		{
			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var graphTypeAttribute = method.Return.Attributes.First<GraphTypeAttribute>();
			var name = graphAttribute?.Name ?? (method.Name.EndsWith("Async", StringComparison.OrdinalIgnoreCase)
				? method.Name.Left(method.Name.LastIndexOf("Async", StringComparison.OrdinalIgnoreCase))
				: method.Name);

			@this.AddField(new FieldType
			{
				Arguments = method.ToQueryArguments(),
				Name = name,
				Description = graphAttribute?.Description,
				DeprecationReason = method.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(method, handler),
				Type = graphTypeAttribute?.GraphType ?? method.Return.GetGraphType()
			});
		}

		internal static void AddField<T>(this IObjectGraphType @this, MethodMember method, SqlApi<T> handler)
			where T : class, new()
		{
			var graphAttribute = method.Attributes.First<GraphAttribute>();
			var graphTypeAttribute = method.Return.Attributes.First<GraphTypeAttribute>();
			var graphName = TypeOf<T>.Attributes.First<GraphAttribute>()?.Name ?? TypeOf<T>.Name;
			var hasNotNull = method.Return.Attributes.Any<NotNullAttribute>();
			var arguments = new QueryArguments(method.Parameters
				.If(parameter => parameter!.Attributes.Any<GraphIgnoreAttribute>() && !parameter.Type.Is<IResolveFieldContext>())
				.To(parameter => parameter!.Name.Is("output") || parameter.Name.Is("select")
					? new QueryArgument(new ListGraphType<GraphObjectEnumType<T>>()) { Name = parameter.Name }
					: parameter.ToQueryArgument()));

			@this.AddField(new FieldType
			{
				Arguments = arguments,
				Name = string.Format(graphAttribute!.Name!, graphName),
				Description = !graphAttribute.Description.IsBlank() ? string.Format(graphAttribute.Description, handler.TableName) : null,
				DeprecationReason = method.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(method, handler),
				Type = graphTypeAttribute?.GraphType ?? method.Return.GetGraphType()
			});
		}

		internal static void AddField(this IInterfaceGraphType @this, PropertyMember property)
			=> @this.AddField(property.CreateFieldType(false));

		internal static void AddField(this IInputObjectGraphType @this, PropertyMember property)
			=> @this.AddField(property.CreateFieldType(true));

		internal static void AddField(this IObjectGraphType @this, PropertyMember property)
			=> @this.AddField(property.CreateFieldType(false));

		private static FieldType CreateFieldType(this PropertyMember @this, bool isInputType)
		{
			var graphAttribute = @this.Attributes.First<GraphAttribute>();
			return new FieldType
			{
				Type = @this.Attributes.First<GraphTypeAttribute>()?.GraphType ?? @this.GetGraphType(isInputType),
				Name = graphAttribute?.Name ?? @this.Name,
				Description = graphAttribute?.Description,
				DeprecationReason = @this.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = !isInputType ? new FuncFieldResolver<object>(context => context.Source) : null
			};
		}

		private static Type? GetGraphType(this ScalarType @this)
			=> @this switch
			{
				ScalarType.ID => typeof(IdGraphType),
				ScalarType.HashID => typeof(GraphHashIdType),
				ScalarType.Boolean => typeof(BooleanGraphType),
				ScalarType.SByte => typeof(SByteGraphType),
				ScalarType.Short => typeof(ShortGraphType),
				ScalarType.Int => typeof(IntGraphType),
				ScalarType.Long => typeof(LongGraphType),
				ScalarType.Byte => typeof(ByteGraphType),
				ScalarType.UShort => typeof(UShortGraphType),
				ScalarType.UInt => typeof(UIntGraphType),
				ScalarType.ULong => typeof(ULongGraphType),
				ScalarType.Float => typeof(FloatGraphType),
				ScalarType.Decimal => typeof(DecimalGraphType),
				ScalarType.Date => typeof(DateGraphType),
				ScalarType.DateTime => typeof(DateTimeGraphType),
				ScalarType.DateTimeOffset => typeof(DateTimeOffsetGraphType),
				ScalarType.TimeSpanMilliseconds => typeof(TimeSpanMillisecondsGraphType),
				ScalarType.TimeSpanSeconds => typeof(TimeSpanSecondsGraphType),
				ScalarType.Guid => typeof(GuidGraphType),
				ScalarType.String => typeof(StringGraphType),
				ScalarType.Uri => typeof(UriGraphType),
				_ => null
			};

		private static Type GetGraphType(this TypeMember @this, bool isInputType, ScalarType? scalarType)
			=> @this.SystemType switch
			{
				_ when @this.Kind == Kind.Delegate => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
				_ when @this.Kind == Kind.Enum => typeof(GraphEnumType<>).MakeGenericType(@this.Handle.ToType()),
				SystemType.String => scalarType?.GetGraphType() ?? typeof(StringGraphType),
				SystemType.Uri => scalarType?.GetGraphType() ?? typeof(UriGraphType),
				_ when @this.IsEnumerable => typeof(ListGraphType<>).MakeGenericType(@this.EnclosedTypeHandle!.Value.ToType().GetTypeMember().GetGraphType(isInputType, scalarType)),
				SystemType.Boolean => scalarType?.GetGraphType() ?? typeof(BooleanGraphType),
				SystemType.SByte => scalarType?.GetGraphType() ?? typeof(SByteGraphType),
				SystemType.Int16 => scalarType?.GetGraphType() ?? typeof(ShortGraphType),
				SystemType.Int32 or SystemType.Index => scalarType?.GetGraphType() ?? typeof(IntGraphType),
				SystemType.Int64 or SystemType.NInt => scalarType?.GetGraphType() ?? typeof(LongGraphType),
				SystemType.Byte => scalarType?.GetGraphType() ?? typeof(ByteGraphType),
				SystemType.UInt16 => scalarType?.GetGraphType() ?? typeof(UShortGraphType),
				SystemType.UInt32 => scalarType?.GetGraphType() ?? typeof(UIntGraphType),
				SystemType.UInt64 or SystemType.NUInt => scalarType?.GetGraphType() ?? typeof(ULongGraphType),
				SystemType.Single or SystemType.Double => scalarType?.GetGraphType() ?? typeof(FloatGraphType),
				SystemType.Decimal => scalarType?.GetGraphType() ?? typeof(DecimalGraphType),
				SystemType.DateTime => scalarType?.GetGraphType() ?? typeof(DateTimeGraphType),
				SystemType.DateTimeOffset => scalarType?.GetGraphType() ?? typeof(DateTimeOffsetGraphType),
				SystemType.TimeSpan => scalarType?.GetGraphType() ?? typeof(TimeSpanSecondsGraphType),
				SystemType.Guid => scalarType?.GetGraphType() ?? typeof(GuidGraphType),
				SystemType.Range => scalarType?.GetGraphType() ?? typeof(StringGraphType),
				SystemType.Nullable or SystemType.Task or SystemType.ValueTask => @this.EnclosedTypeHandle!.Value.ToType().GetTypeMember().GetGraphType(isInputType, scalarType),
				_ when @this.Kind == Kind.Interface => typeof(GraphInterfaceType<>).MakeGenericType(@this.Handle.ToType()),
				_ when isInputType => typeof(GraphInputType<>).MakeGenericType(@this.Handle.ToType()),
				_ => typeof(GraphObjectType<>).MakeGenericType(@this.Handle.ToType())
			};

		private static Type GetGraphType(this MethodParameter @this)
		{
			var graphTypeAttribute = @this.Attributes.First<GraphTypeAttribute>();
			var graphType = graphTypeAttribute?.GraphType;
			if (graphType != null)
				return graphType;

			graphType = @this.Type.GetGraphType(true, graphTypeAttribute?.ScalarType);

			if (@this.Attributes.Any<NotNullAttribute>() || !@this.Type.IsNullable)
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		private static Type GetGraphType(this PropertyMember @this, bool isInputType)
		{
			var graphTypeAttribute = @this.Attributes.First<GraphTypeAttribute>();
			var graphType = graphTypeAttribute?.GraphType;
			if (graphType != null)
				return graphType;

			graphType = @this.Type.GetGraphType(isInputType, graphTypeAttribute?.ScalarType);

			if (@this.Attributes.Any<NotNullAttribute>() || !@this.Type.IsNullable)
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		private static Type GetGraphType(this ReturnParameter @this)
		{
			var graphTypeAttribute = @this.Attributes.First<GraphTypeAttribute>();
			var graphType = graphTypeAttribute?.GraphType;
			if (graphType != null)
				return graphType;

			return @this.Type.GetGraphType(false, graphTypeAttribute?.ScalarType);
		}

		private static QueryArgument ToQueryArgument(this MethodParameter @this)
		{
			var graphAttribute = @this.Attributes.First<GraphAttribute>();
			return new QueryArgument(@this.GetGraphType())
			{
				Name = graphAttribute?.Name ?? @this.Name,
				Description = graphAttribute?.Description
			};
		}

		private static QueryArguments ToQueryArguments(this MethodMember @this)
			=> new QueryArguments(@this.Parameters
				.If(parameter => !parameter!.Attributes.Any<GraphIgnoreAttribute>() && !parameter.Type.Handle.Is<IResolveFieldContext>())
				.To(parameter => parameter!.ToQueryArgument()));
	}
}
