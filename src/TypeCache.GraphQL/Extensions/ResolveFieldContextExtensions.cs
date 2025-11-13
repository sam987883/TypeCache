// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using GraphQLParser.AST;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static class ResolveFieldContextExtensions
{
	extension(IResolveFieldContext @this)
	{
		public DataTable GetArgumentAsDataTable(string name, ObjectSchema objectSchema)
		{
			name.ThrowIfBlank();

			var table = objectSchema.CreateDataTable();
			var arguments = @this.GetArgument<IEnumerable<IDictionary<string, object>>>(name);
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

		public IEnumerable<object?> GetArguments(IReadOnlyList<ParameterEntity> parameters)
		{
			var sourceType = @this.Source?.GetType();
			if (sourceType == typeof(object))
				sourceType = null;

			foreach (var parameter in parameters)
			{
				var name = parameter.Attributes.GraphQLName ?? parameter.Name;
				var argument = @this.HasArgument(name) ? @this.GetArgument(parameter.ParameterType, name) : null;
				yield return argument switch
				{
					_ when parameter.Attributes.GraphQLIgnore => null,
					_ when parameter.ParameterType.Is<IResolveFieldContext>() => @this,
					_ when sourceType is not null && parameter.ParameterType.Is(sourceType) => @this.Source,
					IDictionary<string, object?> dictionary when !parameter.ParameterType.Is<IDictionary<string, object?>>() =>
						map(dictionary, parameter.ParameterType.Create()),
					_ => argument
				};
			}

			static object? map(IDictionary<string, object?> source, object? target)
			{
				if (target is not null)
					source.MapProperties(target);

				return target;
			}
		}

		public IEnumerable<object?> GetArguments<TSource, MATCH>(IReadOnlyList<ParameterEntity> parameters, IEnumerable<MATCH> keys)
		{
			foreach (var parameter in parameters)
			{
				var name = parameter.Attributes.GraphQLName ?? parameter.Name;
				if (!@this.HasArgument(name))
					continue;

				var argument = @this.GetArgument(parameter.ParameterType, name);
				yield return argument switch
				{
					_ when parameter.Attributes.GraphQLIgnore => null,
					_ when parameter.ParameterType.Is<IResolveFieldContext>() => @this,
					_ when parameter.ParameterType.Is<TSource>() => @this.Source,
					_ when parameter.ParameterType.Is<IEnumerable<MATCH>>() => keys,
					_ when parameter.ParameterType.Is<MATCH[]>() => keys.ToArray(),
					IDictionary<string, object?> dictionary when !parameter.ParameterType.Is<IDictionary<string, object?>>() =>
						map(dictionary, parameter.ParameterType.Create()),
					_ => argument
				};
			}

			static object? map(IDictionary<string, object?> source, object? target)
			{
				if (target is not null)
					source.MapProperties(target);

				return target;
			}
		}

		/// <summary>
		/// Gets the input fields that have had a value set for a particular input object.
		/// </summary>
		public IEnumerable<string> GetInputs()
		{
			if (!@this.Operation.SelectionSet.Selections.TryFirst<GraphQLField>(out var root) || root.Arguments?.Any() is not true)
				yield break;

			foreach (var argument in root.Arguments)
				yield return argument.Name.StringValue;
		}

		/// <summary>
		/// Gets a list of query selections including nested selections and fragments.
		/// </summary>
		public IEnumerable<string> GetSelections()
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
	}

	extension(GraphQLSelectionSet @this)
	{
		private IEnumerable<string> GetSelections(IEnumerable<GraphQLFragmentDefinition> fragments, string prefix)
		{
			foreach (var selection in @this.Selections)
			{
				var current = selection switch
				{
					INamedNode name when prefix.IsNotBlank => Invariant($"{prefix}.{name.Name.StringValue}"),
					INamedNode name => name.Name.StringValue,
					_ => prefix
				};

				if (selection is GraphQLField)
					yield return current;

				var selectionSet = selection switch
				{
					IHasSelectionSetNode selectionSetNode => selectionSetNode.SelectionSet,
					GraphQLFragmentSpread fragmentSpread => fragments.First(fragment => fragment.FragmentName == fragmentSpread.FragmentName)?.SelectionSet,
					_ => null
				};

				if (selectionSet?.Selections.Any() is true)
					foreach (var subSelection in selectionSet.GetSelections(fragments, current))
						yield return subSelection;
			}
		}
	}
}
