﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GraphQL;
using GraphQLParser.AST;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.GraphQL.Extensions;

public static class ResolveFieldContextExtensions
{
	public static DataTable GetArgumentAsDataTable(this IResolveFieldContext @this, string name, ObjectSchema objectSchema)
	{
		name.AssertNotBlank();

		var table = objectSchema.CreateDataTable();
		var arguments = @this.GetArgument<IEnumerable<IDictionary<string, object>>>(name).ToArray();
		if (arguments is null)
			return table;

		table.BeginLoadData();
		arguments.ForEach(argument =>
		{
			var row = table.NewRow();
			foreach (var pair in argument)
				row[pair.Key] = pair.Value ?? DBNull.Value;

			table.Rows.Add(row);
		});
		table.EndLoadData();

		return table;
	}

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
				IDictionary<string, object?> dictionary when !parameter.Type.Is<IDictionary<string, object?>>() => parameter.Type.MapModel(dictionary),
				_ => argument
			};
		}
	}

	public static IEnumerable<object?> GetArguments(this IResolveFieldContext @this, Type? sourceType, MethodMember method, object? overrideValue = null)
	{
		foreach (var parameter in method.Parameters)
		{
			var name = parameter.GraphQLName();
			var argument = @this.HasArgument(name) ? @this.GetArgument(parameter.Type, name) : null;
			yield return argument switch
			{
				_ when parameter.GraphQLIgnore() => null,
				_ when parameter.Type.Is<IResolveFieldContext>() => @this,
				_ when sourceType is not null && parameter.Type.Is(sourceType) && !parameter.Type.Is<object>() => @this.Source,
				_ when overrideValue is not null && parameter.Type.Is(overrideValue.GetType()) => overrideValue,
				IDictionary<string, object?> dictionary when !parameter.Type.Is<IDictionary<string, object?>>() => parameter.Type.MapModel(dictionary),
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
		if (!@this.Operation.SelectionSet.Selections.OfType<GraphQLField>().TryFirst(out var root) || root.Arguments?.Any() is not true)
			return dictionary;

		foreach (var argument in root.Arguments)
			dictionary.AddInputs(argument.Name.StringValue, argument.Value);

		return dictionary;
	}

	/// <summary>
	/// Gets a list of query selections including nested selections and fragments.
	/// </summary>
	public static IEnumerable<string> GetSelections(this IResolveFieldContext @this)
	{
		var fragments = @this.Document.Definitions.OfType<GraphQLFragmentDefinition>();
		foreach (var pair in @this.SubFields!)
		{
			var selectionSet = pair.Value.Field.SelectionSet;
			if (selectionSet?.Selections.Any() is true)
			{
				foreach (var selection in selectionSet.GetSelections(fragments, pair.Key))
					yield return selection;
			}
			else
				yield return pair.Key;
		}
	}

	private static void AddInputs(this IDictionary<string, object?> @this, string path, GraphQLValue value)
	{
		if (value is GraphQLListValue listValue)
		{
			if (listValue.Values?.Any<GraphQLObjectValue>() is true)
				listValue.Values.OfType<GraphQLObjectValue>().ToArray().ForEach((objectValue, i) =>
					objectValue.Fields?.ForEach(field => @this[Invariant($"{path}.{i}.{field.Name}")] = field.Value.GetScalarValue()));
			else if (listValue.Values?.Any() is true)
				@this[path] = listValue.Values.Select(_ => _.GetScalarValue()).ToArray();
		}
		else if (value is GraphQLObjectValue objectValue)
			objectValue.Fields?.ForEach(field => @this.AddInputs(Invariant($"{path}.{field.Name}"), field.Value));
		else
			@this[path] = value.GetScalarValue();
	}

	private static object? GetScalarValue(this GraphQLValue @this)
		=> @this switch
		{
			GraphQLBooleanValue booleanValue => booleanValue.BoolValue,
			GraphQLEnumValue enumValue => enumValue.Name,
			GraphQLFloatValue floatValue => double.Parse(floatValue.Value.Span),
			GraphQLIntValue intValue => int.Parse(intValue.Value.Span),
			GraphQLStringValue stringValue => new string(stringValue.Value.Span),
			_ => null
		};

	private static IEnumerable<string> GetSelections(this GraphQLSelectionSet @this, IEnumerable<GraphQLFragmentDefinition> fragments, string prefix)
	{
		foreach (var selection in @this.Selections)
		{
			var current = selection switch
			{
				INamedNode name when prefix.IsNotBlank() => Invariant($"{prefix}.{name.Name.StringValue}"),
				INamedNode name => name.Name.StringValue,
				_ => prefix
			};

			if (selection is GraphQLFragmentSpread fragmentSpread)
			{
				var selectionSet = fragments.First(fragment => fragment.FragmentName == fragmentSpread.FragmentName)?.SelectionSet;
				if (selectionSet is not null)
				{
					foreach (var fragmentSelection in selectionSet.GetSelections(fragments, prefix))
						yield return fragmentSelection;
				}
			}
			else if (selection is IHasSelectionSetNode selectionSet && selectionSet.SelectionSet?.Selections.Any() is true)
			{
				foreach (var subSelection in selectionSet.SelectionSet.GetSelections(fragments, current))
					yield return subSelection;
			}
			else if (selection is GraphQLField)
				yield return current;
		}
	}

	private static object MapModel(this TypeMember @this, IDictionary<string, object?> source)
	{
		var model = @this.Create()!;
		model.MapProperties(source, StringComparison.OrdinalIgnoreCase);
		return model;
	}
}
