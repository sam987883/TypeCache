// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.SQL;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Extensions
{
	public static class GraphExtensions
	{
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
			var cursorProperty = TypeOf<T>.Properties.FirstValue(property => property.Value.Attributes.GraphCursor())?.Value;
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

		internal static FieldType ToFieldType(this MethodMember @this, IFieldResolver resolver)
			=> new FieldType
			{
				Arguments = @this.Parameters.ToQueryArguments(),
				Name = @this.Attributes.GraphName() ?? @this.Name.TrimStart("Get")!.TrimEnd("Async")!,
				Description = @this.Attributes.GraphDescription(),
				DeprecationReason = @this.Attributes.ObsoleteMessage(),
				Resolver = resolver,
				Type = @this.Return.GetGraphType()
			};

		internal static FieldType ToFieldType(this PropertyMember @this, bool isInputType)
			=> new FieldType
			{
				Type = @this.GetGraphType(isInputType),
				Name = @this.Attributes.GraphName() ?? @this.Name,
				Description = @this.Attributes.GraphDescription(),
				DeprecationReason = @this.Attributes.ObsoleteMessage(),
				Resolver = !isInputType ? new FuncFieldResolver<object>(context => context.Source) : null
			};

		internal static FieldType ToFieldType<T>(this MethodMember @this, SqlApi<T> sqlApi)
			where T : class, new()
			=> new FieldType
			{
				Arguments = @this.Parameters.ToQueryArguments(),
				Name = string.Format(@this.Attributes.GraphName()!, sqlApi.Table),
				Description = string.Format(@this.Attributes.GraphDescription()!, sqlApi.Table),
				DeprecationReason = @this.Attributes.ObsoleteMessage(),
				Resolver = new MethodFieldResolver(@this, sqlApi),
				Type = @this.Return.GetGraphType()
			};

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

		private static QueryArgument ToQueryArgument(this MethodParameter @this)
			=> new QueryArgument(@this.GetGraphType())
			{
				Name = @this.Attributes.GraphName() ?? @this.Name,
				Description = @this.Attributes.GraphDescription(),
			};

		private static QueryArguments ToQueryArguments(this IEnumerable<MethodParameter> @this)
			=> new QueryArguments(@this
				.If(parameter => !parameter.Attributes.GraphIgnore() && !parameter.Type.Handle.Is<IResolveFieldContext>())
				.To(parameter => parameter.ToQueryArgument()));
	}
}
