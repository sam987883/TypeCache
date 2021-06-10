// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Language.AST;
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
		public static string? GetGraphDescription(this Member @this)
			=> @this.Attributes.First<GraphDescriptionAttribute>()?.Description;

		public static string? GetGraphDescription(this MethodParameter @this)
			=> @this.Attributes.First<GraphDescriptionAttribute>()?.Description;

		public static string GetGraphName<T>(this EnumOf<T>.Token @this)
			where T : struct, Enum
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name;

		public static string GetGraphName(this Member @this)
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name;

		public static string GetGraphName(this MethodMember @this)
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name.TrimStart("Get")!.TrimEnd("Async")!;

		public static string GetGraphName(this MethodParameter @this)
			=> @this.Attributes.First<GraphNameAttribute>()?.Name ?? @this.Name;

		public static Type GetGraphType(this MethodParameter @this)
			=> GetGraphType(@this.Type, @this.Attributes, true);

		public static Type GetGraphType(this PropertyMember @this, bool isInputType)
			=> GetGraphType(@this.PropertyType, @this.Attributes, isInputType);

		public static Type GetGraphType(this ReturnParameter @this)
			=> GetGraphType(@this.Type, @this.Attributes, false);

		/// <summary>
		/// Gets the query selections for the Connection GraphQL Relay type including nested selections and fragments.
		/// </summary>
		public static ConnectionSelections GetQueryConnectionSelections(this IResolveFieldContext @this)
		{
			var selections = new ConnectionSelections();
			selections.TotalCount = @this.SubFields.ContainsKey("totalCount");

			if (@this.SubFields.TryGetValue("pageInfo", out var pageInfo))
			{
				var fields = pageInfo.SelectionSet.Selections.If<Field>().ToArray();
				selections.HasNextPage = fields.Any(field => field.Name.Is(nameof(PageInfo.HasNextPage)));
				selections.HasPreviousPage = fields.Any(field => field.Name.Is(nameof(PageInfo.HasPreviousPage)));
				selections.StartCursor = fields.Any(field => field.Name.Is(nameof(PageInfo.StartCursor)));
				selections.EndCursor = fields.Any(field => field.Name.Is(nameof(PageInfo.EndCursor)));
			}

			if (@this.SubFields.TryGetValue("edges", out var edges))
			{
				selections.Cursor = edges.SelectionSet.Selections.If<Field>().Any(field => field.Name.Is("cursor"));
				var node = edges.SelectionSet.Selections.First(selection => selection is IHaveName name && name.NameNode.Name.Is("node"));

				if (node is IHaveSelectionSet selectionSet)
					selections.EdgeNodeFields = selectionSet.GetSelections(@this.Document.Fragments, string.Empty).ToArray();
			}

			if (@this.SubFields.TryGetValue("items", out var items) && items.SelectionSet.Selections.Any())
				selections.ItemFields = items.GetSelections(@this.Document.Fragments, string.Empty).ToArray();

			return selections;
		}

		/// <summary>
		/// Gets a list of query selections including nested selections and fragments.
		/// </summary>
		public static IEnumerable<string> GetQuerySelections(this IResolveFieldContext @this)
		{
			foreach (var subField in @this.SubFields)
			{
				if (subField.Value.SelectionSet.Selections.Any())
				{
					foreach (var selection in subField.Value.GetSelections(@this.Document.Fragments, subField.Key))
						yield return selection;
				}
				else
					yield return subField.Key;
			}
		}

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
					Cursor = cursorProperty?.GetValue(item)?.ToString(),
					Node = item
				}).ToList(),
				PageInfo = new PageInfo
				{
					StartCursor = items.Length > 0 ? cursorProperty?.GetValue(items[0])?.ToString() : null,
					EndCursor = items.Length > 0 ? cursorProperty?.GetValue(items[^1])?.ToString() : null,
					HasNextPage = hasNextPage,
					HasPreviousPage = hasPreviousPage
				},
				TotalCount = totalCount
			};
			connection.Items.AddRange(items);
			return connection;
		}

		public static Type ToGraphType(this ScalarType @this)
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
				ScalarType.NonNullableID => typeof(NonNullGraphType<IdGraphType>),
				ScalarType.NonNullableHashID => typeof(NonNullGraphType<GraphHashIdType>),
				ScalarType.NonNullableBoolean => typeof(NonNullGraphType<BooleanGraphType>),
				ScalarType.NonNullableSByte => typeof(NonNullGraphType<SByteGraphType>),
				ScalarType.NonNullableShort => typeof(NonNullGraphType<ShortGraphType>),
				ScalarType.NonNullableInt => typeof(NonNullGraphType<IntGraphType>),
				ScalarType.NonNullableLong => typeof(NonNullGraphType<LongGraphType>),
				ScalarType.NonNullableByte => typeof(NonNullGraphType<ByteGraphType>),
				ScalarType.NonNullableUShort => typeof(NonNullGraphType<UShortGraphType>),
				ScalarType.NonNullableUInt => typeof(NonNullGraphType<UIntGraphType>),
				ScalarType.NonNullableULong => typeof(NonNullGraphType<ULongGraphType>),
				ScalarType.NonNullableFloat => typeof(NonNullGraphType<FloatGraphType>),
				ScalarType.NonNullableDecimal => typeof(NonNullGraphType<DecimalGraphType>),
				ScalarType.NonNullableDate => typeof(NonNullGraphType<DateGraphType>),
				ScalarType.NonNullableDateTime => typeof(NonNullGraphType<DateTimeGraphType>),
				ScalarType.NonNullableDateTimeOffset => typeof(NonNullGraphType<DateTimeOffsetGraphType>),
				ScalarType.NonNullableTimeSpanMilliseconds => typeof(NonNullGraphType<TimeSpanMillisecondsGraphType>),
				ScalarType.NonNullableTimeSpanSeconds => typeof(NonNullGraphType<TimeSpanSecondsGraphType>),
				ScalarType.NonNullableGuid => typeof(NonNullGraphType<GuidGraphType>),
				ScalarType.NonNullableString => typeof(NonNullGraphType<StringGraphType>),
				ScalarType.NonNullableUri => typeof(NonNullGraphType<UriGraphType>),
				_ => typeof(StringGraphType)
			};

		internal static FieldType ToFieldType(this MethodMember @this, IFieldResolver resolver)
			=> new FieldType
			{
				Arguments = @this.Parameters.ToQueryArguments(),
				Name = @this.GetGraphName(),
				Description = @this.GetGraphDescription(),
				DeprecationReason = @this.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = resolver,
				Type = @this.Return.GetGraphType()
			};

		internal static FieldType ToFieldType(this PropertyMember @this, bool isInputType)
			=> new FieldType
			{
				Type = @this.GetGraphType(isInputType),
				Name = @this.GetGraphName(),
				Description = @this.GetGraphDescription(),
				DeprecationReason = @this.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = !isInputType ? new FuncFieldResolver<object>(context => context.Source) : null
			};

		internal static FieldType ToFieldType<T>(this MethodMember @this, SqlApi<T> sqlApi)
			where T : class, new()
		{
			var arguments = new QueryArguments(@this.Parameters
				.If(parameter => parameter!.Attributes.Any<GraphIgnoreAttribute>() && !parameter.Type.Is<IResolveFieldContext>())
				.To(parameter => parameter!.Name.Is("output") || parameter.Name.Is("select")
					? new QueryArgument(new ListGraphType<GraphObjectEnumType<T>>()) { Name = parameter.Name }
					: parameter.ToQueryArgument()));
			var description = @this.GetGraphDescription();

			return new FieldType
			{
				Arguments = @this.Parameters.ToQueryArguments(),
				Name = @this.GetGraphName(),
				Description = description != null ? string.Format(description, sqlApi.TableName) : null,
				DeprecationReason = @this.Attributes.First<ObsoleteAttribute>()?.Message,
				Resolver = new MethodFieldResolver(@this, sqlApi),
				Type = @this.Return.GetGraphType()
			};
		}

		private static Type GetGraphType(TypeMember type, IEnumerable<Attribute> attributes, bool isInputType)
		{
			var graphType = attributes.First<GraphTypeAttribute>()?.GraphType;
			if (graphType is not null)
				return graphType;

			graphType = type.ToGraphType(isInputType);

			if (attributes.Any<NotNullAttribute>() || !type.IsNullable())
				graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

			return graphType;
		}

		private static IEnumerable<string> GetSelections(this IHaveSelectionSet @this, Fragments fragments, string prefix)
		{
			foreach (var selection in @this.SelectionSet.Selections)
			{
				var current = @this switch
				{
					IHaveName name when !prefix.IsBlank() => $"{prefix}.{name.NameNode.Name}",
					IHaveName name => name.NameNode.Name,
					_ => prefix
				};

				if (selection is FragmentSpread fragmentSpread)
				{
					var selectionSet = fragments.FindDefinition(fragmentSpread.Name) as IHaveSelectionSet;
					if (selectionSet is not null)
					{
						foreach (var fragmentSelection in selectionSet.GetSelections(fragments, prefix))
							yield return fragmentSelection;
					}
				}
				else if (selection is IHaveSelectionSet selectionSet && selectionSet.SelectionSet.Selections.Any())
				{
					foreach (var subSelection in selectionSet.GetSelections(fragments, current))
						yield return subSelection;
				}
				else if (selection is Field field)
					yield return current;
			}
		}

		private static Type ToGraphType(this TypeMember @this, bool isInputType)
			=> @this.SystemType switch
			{
				_ when @this.Kind is Kind.Delegate => throw new ArgumentOutOfRangeException($"{nameof(TypeMember)}.{nameof(@this.Kind)}", $"No custom graph type was found that supports: {@this.Kind.Name()}"),
				_ when @this.Kind is Kind.Enum => typeof(GraphEnumType<>).MakeGenericType(@this),
				SystemType.String => typeof(StringGraphType),
				SystemType.Uri => typeof(UriGraphType),
				_ when @this.IsEnumerable => typeof(ListGraphType<>).MakeGenericType(@this.EnclosedTypeHandle!.Value.GetTypeMember().ToGraphType(isInputType)),
				SystemType.Boolean => typeof(BooleanGraphType),
				SystemType.SByte => typeof(SByteGraphType),
				SystemType.Int16 => typeof(ShortGraphType),
				SystemType.Int32 or SystemType.Index => typeof(IntGraphType),
				SystemType.Int64 or SystemType.NInt => typeof(LongGraphType),
				SystemType.Byte => typeof(ByteGraphType),
				SystemType.UInt16 => typeof(UShortGraphType),
				SystemType.UInt32 => typeof(UIntGraphType),
				SystemType.UInt64 or SystemType.NUInt => typeof(ULongGraphType),
				SystemType.Single or SystemType.Double => typeof(FloatGraphType),
				SystemType.Decimal => typeof(DecimalGraphType),
				SystemType.DateTime => typeof(DateTimeGraphType),
				SystemType.DateTimeOffset => typeof(DateTimeOffsetGraphType),
				SystemType.TimeSpan => typeof(TimeSpanSecondsGraphType),
				SystemType.Guid => typeof(GuidGraphType),
				SystemType.Range => typeof(StringGraphType),
				SystemType.Nullable or SystemType.Task or SystemType.ValueTask => @this.EnclosedTypeHandle!.Value.GetTypeMember().ToGraphType(isInputType),
				_ when @this.Kind is Kind.Interface => typeof(GraphInterfaceType<>).MakeGenericType(@this),
				_ when isInputType => typeof(GraphInputType<>).MakeGenericType(@this),
				_ => typeof(GraphObjectType<>).MakeGenericType(@this)
			};

		private static QueryArgument ToQueryArgument(this MethodParameter @this)
			=> new QueryArgument(@this.GetGraphType())
			{
				Name = @this.GetGraphName(),
				Description = @this.GetGraphDescription(),
			};

		private static QueryArguments ToQueryArguments(this IEnumerable<MethodParameter> @this)
			=> new QueryArguments(@this
				.If(parameter => !parameter!.Attributes.Any<GraphIgnoreAttribute>()
					&& !parameter.Type.Handle.Is<IResolveFieldContext>())
				.To(parameter => parameter!.ToQueryArgument()));
	}
}
