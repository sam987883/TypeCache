// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Reflection;
using GraphQL;
using GraphQLParser.AST;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Extensions;

public static class ResolveFieldContextExtensions
{
	public static DataTable GetArgumentAsDataTable(this IResolveFieldContext @this, string name, ObjectSchema objectSchema)
	{
		name.ThrowIfBlank();

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

	public static IEnumerable<object?> GetArguments(this IResolveFieldContext @this, MethodInfo methodInfo)
	{
		var sourceType = @this.Source?.GetType();
		if (sourceType == typeof(object))
			sourceType = null;

		var parameterInfos = methodInfo.GetParameters()
			.Where(parameterInfo => !parameterInfo.IsOut && !parameterInfo.IsRetval)
			.OrderBy(parameterInfo => parameterInfo.Position);
		foreach (var parameterInfo in parameterInfos)
		{
			var name = parameterInfo.GraphQLName();
			var argument = @this.HasArgument(name) ? @this.GetArgument(parameterInfo.ParameterType, name) : null;
			yield return argument switch
			{
				_ when parameterInfo.GraphQLIgnore() => null,
				_ when parameterInfo.ParameterType.Is<IResolveFieldContext>() => @this,
				_ when sourceType is not null && parameterInfo.ParameterType.Is(sourceType) => @this.Source,
				IDictionary<string, object?> dictionary when !parameterInfo.ParameterType.Is<IDictionary<string, object?>>() =>
					dictionary.MapTo(parameterInfo.ParameterType.Create()!),
				_ => argument
			};
		}
	}

	public static IEnumerable<object?> GetArguments<TSource, MATCH>(this IResolveFieldContext @this, MethodInfo methodInfo, IEnumerable<MATCH> keys)
	{
		var parameterInfos = methodInfo.GetParameters()
			.Where(parameterInfo => !parameterInfo.IsOut && !parameterInfo.IsRetval)
			.OrderBy(parameterInfo => parameterInfo.Position);
		foreach (var parameterInfo in parameterInfos)
		{
			var name = parameterInfo.GraphQLName();
			var argument = @this.HasArgument(name) ? @this.GetArgument(parameterInfo.ParameterType, name) : null;
			yield return argument switch
			{
				_ when parameterInfo.GraphQLIgnore() => null,
				_ when parameterInfo.ParameterType.Is<IResolveFieldContext>() => @this,
				_ when parameterInfo.ParameterType.Is<TSource>() => @this.Source,
				_ when parameterInfo.ParameterType.Is<IEnumerable<MATCH>>() => keys,
				_ when parameterInfo.ParameterType.Is<MATCH[]>() => keys.ToArray(),
				IDictionary<string, object?> dictionary when !parameterInfo.ParameterType.Is<IDictionary<string, object?>>() =>
					dictionary.MapTo(parameterInfo.ParameterType.Create()!),
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
		if (!@this.Operation.SelectionSet.Selections.TryFirst<GraphQLField>(out var root) || root.Arguments?.Any() is not true)
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
			GraphQLBooleanValue value => value.BoolValue,
			GraphQLEnumValue value => value.Name,
			GraphQLFloatValue value => decimal.Parse(value.Value.Span),
			GraphQLIntValue value => long.Parse(value.Value.Span),
			GraphQLStringValue value => new string(value.Value.Span),
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
}
