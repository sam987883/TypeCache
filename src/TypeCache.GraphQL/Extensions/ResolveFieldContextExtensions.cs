// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Extensions;

public static class ResolveFieldContextExtensions
{
	public static IEnumerable<object?> GetArguments<TSource>(this IResolveFieldContext @this, MethodMember method, object? overrideValue = null)
	{
		foreach (var parameter in method.Parameters)
		{
			var name = parameter.GraphQLName();
			var argument = @this.HasArgument(name) ? @this.GetArgument(parameter.Type, name) : null;
			yield return argument switch
			{
				_ when parameter.GraphQLIgnore() => null,
				_ when parameter.Type.Is<IResolveFieldContext>() => @this,
				_ when parameter.Type.Is<TSource>() && !parameter.Type.Is<object>() => @this.Source,
				_ when overrideValue is not null && parameter.Type.Is(overrideValue.GetType()) => overrideValue,
				IDictionary<string, object?> dictionary when !parameter.Type.Is<IDictionary<string, object?>>() => dictionary.MapModel(parameter.Type),
				_ => argument
			};
		}
	}

	/// <summary>
	/// Gets the input fields that have had a value set for a particular input object.
	/// </summary>
	public static IDictionary<string, object?> GetInputs(this IResolveFieldContext @this)
	{
		var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
		if (!@this.Operation.SelectionSet.Selections.TryFirst<Field>(out var root) || !root.Arguments.Any())
			return dictionary;

		foreach (var argument in root.Arguments)
		{
			dictionary.AddInputs(argument.Name, (IValue)argument.Children.First()!);
		}

		return dictionary;
	}

	/// <summary>
	/// Gets the query selections for the Connection GraphQL Relay type including nested selections and fragments.
	/// </summary>
	public static ConnectionSelections GetConnectionSelections(this IResolveFieldContext @this)
	{
		var selections = new ConnectionSelections
		{
			TotalCount = @this.SubFields!.ContainsKey("totalCount")
		};

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
	public static IEnumerable<string> GetSelections(this IResolveFieldContext @this)
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

	private static void AddInputs(this IDictionary<string, object?> @this, string path, IValue value)
	{
		if (value is ListValue listValue)
		{
			if (listValue.Children.Any<ObjectValue>())
			{
				var rows = new List<object[]>(listValue.Children.Count());
				var columns = Array<string>.Empty;
				foreach (var node in listValue.Children)
				{
					var dictionary = (IDictionary<string, object?>)((IValue)node).Value!;
					if (!columns.Any())
						columns = dictionary.Keys.ToArray();
					rows.Add(dictionary.Values.ToArray()!);
				}
				@this[path] = new RowSet
				{
					Columns = columns,
					Count = rows.Count,
					Rows = rows.ToArray()
				};
			}
			else
				foreach (var node in listValue.Children)
					@this.AddInputs(path, (IValue)node);
		}
		else if (value is ObjectField objectField)
			@this.AddInputs(Invariant($"{path}.{objectField.Name}"), (IValue)objectField.Value);
		else if (value is ObjectValue objectValue)
			((IDictionary<string, object?>)objectValue.Value).Do(pair => @this.Add(pair));
		else
			@this[path] = value.Value;
	}

	private static IEnumerable<string> GetSelections(this IHaveSelectionSet @this, Fragments fragments, string prefix)
	{
		foreach (var selection in @this.SelectionSet!.Selections)
		{
			var current = selection switch
			{
				IHaveName name when prefix.IsNotBlank() => Invariant($"{prefix}.{name.NameNode.Name}"),
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

		var properties = modelType.Properties.Values.ToDictionary(property => property.GraphQLName(), property => property, StringComparison.Ordinal);
		foreach (var match in (@this, properties).Match(StringComparison.Ordinal))
		{
			var value = match.Value.Item1;
			if (value is IDictionary<string, object?> argument && !match.Value.Item2.PropertyType.Implements<IDictionary<string, object?>>())
				value = argument.MapModel(match.Value.Item2.PropertyType);
			match.Value.Item2.SetValue(model, value);
		}

		return model;
	}
}
