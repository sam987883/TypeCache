// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.SQL;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLExtensions
{
	public static IEnumerable<object?> GetArguments<TSource>(this IResolveFieldContext @this, MethodMember method, object? overrideValue = null)
	{
		foreach (var parameter in method.Parameters)
		{
			if (parameter.GraphIgnore())
				continue;

			var name = parameter.GraphName();
			if (parameter.Type.Is<IResolveFieldContext>())
				yield return @this;
			else if (parameter.Type.Is<TSource>() && !typeof(TSource).Is<object>())
				yield return @this.Source;
			else if (overrideValue is not null && parameter.Type.Is(overrideValue.GetType()))
				yield return overrideValue;
			else if (@this.HasArgument(name))
			{
				var argument = @this.GetArgument<object>(name);
				if (argument is IDictionary<string, object?> dictionary && !parameter.Type.Is<IDictionary<string, object?>>())
					yield return dictionary.MapModel(parameter.Type);
				else
					yield return argument;
			}
		}
	}

	/// <summary>
	/// Gets the input arguments and fields that have had a value set.
	/// </summary>
	public static IEnumerable<string> GetMutationInputs(this IResolveFieldContext @this)
	{
		foreach (var pair in @this.Arguments!)
		{
			if (pair.Value.Value is IDictionary<string, object> dictionary)
			{
				foreach (var key in GetKeys(pair.Key, dictionary))
					yield return key;
			}
			else
				yield return pair.Key;
		}
	}

	/// <summary>
	/// Gets the input fields that have had a value set for a particular input object.
	/// </summary>
	public static IEnumerable<string> GetMutationInputs(this IResolveFieldContext @this, string path)
	{
		var inputs = path.Split('.', StringSplitOptions.RemoveEmptyEntries).ToQueue();
		if (inputs.TryDequeue(out var input) && @this.Arguments!.TryGetValue(input, out var value))
		{
			var dictionary = value.Value as IDictionary<string, object>;
			if (inputs.Any())
			{
				if (dictionary is not null)
				{
					foreach (var key in GetKeys(inputs, dictionary))
						yield return key;
				}
			}
			else if (dictionary is not null)
			{
				foreach (var key in dictionary.Keys)
					yield return key;
			}
			else
				yield return input;
		}
	}

	/// <summary>
	/// Gets the query selections for the Connection GraphQL Relay type including nested selections and fragments.
	/// </summary>
	public static ConnectionSelections GetQueryConnectionSelections(this IResolveFieldContext @this)
	{
		var selections = new ConnectionSelections();
		selections.TotalCount = @this.SubFields!.ContainsKey("totalCount");

		if (@this.SubFields.TryGetValue("pageInfo", out var pageInfo))
		{
			var fields = pageInfo.SelectionSet!.Selections.If<Field>().ToArray();
			selections.HasNextPage = fields.Any(field => field.Name.Is(nameof(PageInfo.HasNextPage)));
			selections.HasPreviousPage = fields.Any(field => field.Name.Is(nameof(PageInfo.HasPreviousPage)));
			selections.StartCursor = fields.Any(field => field.Name.Is(nameof(PageInfo.StartCursor)));
			selections.EndCursor = fields.Any(field => field.Name.Is(nameof(PageInfo.EndCursor)));
		}

		if (@this.SubFields.TryGetValue("edges", out var edges))
		{
			selections.Cursor = edges.SelectionSet!.Selections.If<Field>().Any(field => field.Name.Is("cursor"));
			var node = edges.SelectionSet.Selections.First(selection => selection is IHaveName name && name.NameNode.Name.Is("node"));

			if (node is IHaveSelectionSet selectionSet)
				selections.EdgeNodeFields = selectionSet.GetSelections(@this.Document.Fragments, string.Empty).ToArray();
		}

		if (@this.SubFields.TryGetValue("items", out var items) && items.SelectionSet!.Selections.Any())
			selections.ItemFields = items.GetSelections(@this.Document.Fragments, string.Empty).ToArray();

		return selections;
	}

	/// <summary>
	/// Gets a list of query selections including nested selections and fragments.
	/// </summary>
	public static IEnumerable<string> GetQuerySelections(this IResolveFieldContext @this)
	{
		foreach (var subField in @this.SubFields!)
		{
			if (subField.Value.SelectionSet!.Selections.Any())
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

	internal static EventStreamFieldType ToEventStreamFieldType(this MethodMember @this, object? handler)
		=> new()
		{
			Arguments = @this.Parameters.ToQueryArguments(),
			Name = @this.GraphName(),
			Description = @this.GraphDescription(),
			DeprecationReason = @this.ObsoleteMessage(),
			Resolver = new FuncFieldResolver<object?>(context => @this.Invoke(handler, context.GetArguments<object>(@this).ToArray())),
			Subscriber = (IEventStreamResolver)typeof(GraphQLExtensions).GetTypeMember().InvokeGenericMethod(nameof(CreateEventStreamResolver), new[] { (Type)@this.Return.Type! }, @this, handler)!,
			Type = @this.Return.GraphType()
		};

	internal static FieldType ToFieldType(this MethodMember @this, object? handler)
		=> new()
		{
			Arguments = @this.Parameters.ToQueryArguments(),
			Name = @this.GraphName(),
			Description = @this.GraphDescription(),
			DeprecationReason = @this.ObsoleteMessage(),
			Resolver = new FuncFieldResolver<object?>(context => @this.Invoke(handler, context.GetArguments<object>(@this).ToArray())),
			Type = @this.Return.GraphType()
		};

	internal static FieldType ToFieldType<T>(this MethodMember @this, string table, SqlApi<T> sqlApi)
		where T : class, new()
		=> new()
		{
			Arguments = @this.Parameters.ToQueryArguments(),
			Name = string.Format(@this.GraphName()!, table),
			Description = string.Format(@this.GraphDescription()!, table),
			DeprecationReason = @this.ObsoleteMessage(),
			Resolver = new FuncFieldResolver<object?>(context => @this.Invoke(sqlApi, context.GetArguments<object>(@this).ToArray())),
			Type = @this.Return.GraphType()
		};

	internal static FieldType ToFieldType(this PropertyMember @this, bool isInputType)
		=> new()
		{
			Type = @this.GraphType(isInputType),
			Name = @this.GraphName(),
			Description = @this.GraphDescription(),
			DeprecationReason = @this.ObsoleteMessage(),
			Resolver = !isInputType ? new FuncFieldResolver<object>(context => context.Source) : null
		};

	private static IEventStreamResolver CreateEventStreamResolver<T>(MethodMember method, object? handler)
		=> new EventStreamResolver<T>(context => (IObservable<T>)method.Invoke(handler, context.GetArguments<object>(method).ToArray())!);

	private static IEnumerable<string> GetKeys(Queue<string> inputs, IDictionary<string, object> dictionary)
	{
		if (inputs.TryDequeue(out var input) && dictionary.TryGetValue(input, out var value))
		{
			var subDictionary = value as IDictionary<string, object>;
			if (inputs.Any())
			{
				if (subDictionary is not null)
				{
					foreach (var key in GetKeys(inputs, subDictionary))
						yield return key;
				}
			}
			else if (subDictionary is not null)
			{
				foreach (var key in subDictionary.Keys)
					yield return key;
			}
			else
				yield return input;
		}
	}

	private static IEnumerable<string> GetKeys(string path, IDictionary<string, object> dictionary)
	{
		foreach (var pair in dictionary)
		{
			var current = $"{path}.{pair.Key}";
			if (pair.Value is IDictionary<string, object> subDictionary)
			{
				foreach (var key in GetKeys(current, subDictionary))
					yield return key;
			}
			else
				yield return current;
		}
	}

	private static IEnumerable<string> GetSelections(this IHaveSelectionSet @this, Fragments fragments, string prefix)
	{
		foreach (var selection in @this.SelectionSet!.Selections)
		{
			var current = @this switch
			{
				IHaveName name when prefix.IsNotBlank() => $"{prefix}.{name.NameNode.Name}",
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
			else if (selection is IHaveSelectionSet selectionSet && selectionSet.SelectionSet!.Selections.Any())
			{
				foreach (var subSelection in selectionSet.GetSelections(fragments, current))
					yield return subSelection;
			}
			else if (selection is Field field)
				yield return current;
		}
	}

	private static object MapModel(this IDictionary<string, object?> @this, TypeMember modelType)
	{
		var model = modelType.Create();

		var properties = modelType.Properties.Values.ToDictionary(property => property.GraphName(), property => property, StringComparison.Ordinal);
		foreach (var match in (@this, properties).Match(StringComparison.Ordinal))
		{
			var value = match.Value.Item1;
			if (value is IDictionary<string, object?> argument && !match.Value.Item2.PropertyType.Implements<IDictionary<string, object?>>())
				value = argument.MapModel(match.Value.Item2.PropertyType);
			match.Value.Item2.SetValue(model, value);
		}

		return model;
	}

	private static QueryArguments ToQueryArguments(this IEnumerable<MethodParameter> @this)
		=> new QueryArguments(@this
			.If(parameter => !parameter.GraphIgnore())
			.Map(parameter => new QueryArgument(parameter.GraphType())
			{
				Name = parameter.GraphName(),
				Description = parameter.GraphDescription(),
			}));
}
