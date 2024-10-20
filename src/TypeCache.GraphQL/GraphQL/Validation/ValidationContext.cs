﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Validation;

/// <summary>
/// Provides contextual information about the validation of the document.
/// </summary>
public partial class ValidationContext : IProvideUserContext
{
	private List<ValidationError>? _errors;

	internal void Reset()
	{
		_errors = null;
		_fragments.Clear();
		_variables.Clear();
		Operation = null!;
		Schema = null!;
		Document = null!;
		TypeInfo = null!;
		UserContext = null!;
		NonUserContext = null;
		Variables = null!;
		Extensions = null!;
		RequestServices = null!;
		Metrics = null!;
		User = null;
	}

	/// <summary>
	/// Returns the operation requested to be executed.
	/// </summary>
	public GraphQLOperationDefinition Operation { get; set; } = null!;

	/// <inheritdoc cref="IExecutionContext.Schema"/>
	public ISchema Schema { get; set; } = null!;

	/// <inheritdoc cref="IExecutionContext.Document"/>
	public GraphQLDocument Document { get; set; } = null!;

	/// <inheritdoc cref="Validation.TypeInfo"/>
	public TypeInfo TypeInfo { get; set; } = null!;

	/// <inheritdoc/>
	public IDictionary<string, object?> UserContext { get; set; } = null!;

	/// <inheritdoc cref="IExecutionContext.Metrics"/>
	public Metrics Metrics { get; set; } = null!;

	/// <summary>
	/// Dictionary of temporary data used by validation rules.
	/// TODO: think about internal reusable fields in TypeInfo
	/// </summary>
	internal Dictionary<string, object?>? NonUserContext { get; set; }

	/// <summary>
	/// Returns a list of validation errors for this document.
	/// </summary>
	public IEnumerable<ValidationError> Errors => (IEnumerable<ValidationError>?)_errors ?? Array.Empty<ValidationError>();

	/// <summary>
	/// Returns <see langword="true"/> if there are any validation errors for this document.
	/// </summary>
	public bool HasErrors => _errors?.Count > 0;

	/// <inheritdoc cref="ExecutionOptions.Variables"/>
	public IDictionary<string, object> Variables { get; set; } = null!;

	/// <inheritdoc cref="ExecutionOptions.Extensions"/>
	public IDictionary<string, object> Extensions { get; set; } = null!;

	/// <inheritdoc cref="ExecutionOptions.RequestServices"/>
	public IServiceProvider? RequestServices { get; set; }

	/// <inheritdoc cref="ExecutionOptions.User"/>
	public ClaimsPrincipal? User { get; set; }

	/// <summary>
	/// <see cref="System.Threading.CancellationToken">CancellationToken</see> to cancel validation of request;
	/// defaults to <see cref="CancellationToken.None"/>
	/// </summary>
	public CancellationToken CancellationToken { get; set; }

	/// <summary>
	/// Adds a validation error to the list of validation errors.
	/// </summary>
	public void ReportError(ValidationError error)
	{
		if (error is null)
			throw new ArgumentNullException(nameof(error), "Must provide a validation error.");
		(_errors ??= new()).Add(error);
	}

	/// <summary>
	/// Returns a list of fragment spreads within the specified node.
	/// </summary>
	public List<GraphQLFragmentSpread> GetFragmentSpreads(GraphQLSelectionSet node)
	{
		var spreads = new List<GraphQLFragmentSpread>();

		var setsToVisit = new Stack<GraphQLSelectionSet>();
		setsToVisit.Push(node);

		while (setsToVisit.Count > 0)
		{
			var set = setsToVisit.Pop();

			foreach (var selection in set.Selections)
			{
				if (selection is GraphQLFragmentSpread spread)
				{
					spreads.Add(spread);
				}
				else if (selection is IHasSelectionSetNode hasSet && hasSet.SelectionSet is not null)
				{
					setsToVisit.Push(hasSet.SelectionSet);
				}
			}
		}

		return spreads;
	}

	/// <summary>
	/// Returns all of the variable values defined for the operation from the attached <see cref="Variables"/> object.
	/// Only correctly validated variables are returned. If the variable is specified incorrectly, then an instance of
	/// <see cref="ValidationError"/> is added to the <see cref="Errors"/>.
	/// </summary>
	[Obsolete("This method can repeatedly add the same error into ValidationContext.Errors collection when called multiple times. Use GetVariablesValuesAsync method instead.")]
	public async ValueTask<Variables> GetVariableValuesAsync(IVariableVisitor? visitor = null)
	{
		(var variables, var errors) = await GetVariablesValuesAsync(visitor);

		if (errors is not null)
		{
			foreach (var error in errors)
				ReportError(error);
		}

		return variables;
	}

	/// <summary>
	/// Returns all of the variable values defined for the operation from the attached <see cref="Variables"/> object.
	/// Only correctly validated variables are returned. If the variable is specified incorrectly, then an instance of
	/// <see cref="ValidationError"/> is returned within the list of errors.
	/// </summary>
	public async ValueTask<(Variables Variables, List<ValidationError>? Errors)> GetVariablesValuesAsync(IVariableVisitor? visitor = null)
	{
		var variableDefinitions = Operation?.Variables;

		if ((variableDefinitions?.Count ?? 0) == 0)
		{
			return (Validation.Variables.None, null);
		}

		var usages = GetRecursiveVariables(Operation!);

		List<ValidationError>? errors = null;
		var variablesObj = new Variables(variableDefinitions!.Count);

		if (variableDefinitions is not null)
		{
			foreach (var variableDef in variableDefinitions.Items)
			{
				string variableDefName = variableDef.Variable.Name.StringValue; //ISSUE:allocation
																				// find the IGraphType instance for the variable type
				var graphType = variableDef.Type.GraphTypeFromType(Schema);

				if (graphType is null)
				{
					(errors ??= new()).Add(new InvalidVariableError(this, variableDef, variableDefName, $"Variable has unknown type '{variableDef.Type.Name()}'"));
					continue;
				}

				// create a new variable object
				var variable = new Variable(variableDefName, variableDef);

				// attempt to retrieve the variable value from the inputs
				if (Variables.TryGetValue(variableDefName, out object? variableValue))
				{
					// for nullable variable types that are supplied (not default value),
					if (graphType is not NonNullGraphType)
					{
						// determine if a non-null value is required
						//   (it may have passed validation for a nullable type if the variable definition,
						//    argument definition, or input object field definition includes a default value)
						bool requiresNonNull = false;
						if (usages is not null)
						{
							foreach (var usage in usages)
							{
								if (usage.Node.Name == variableDef.Variable.Name && usage.Type is NonNullGraphType)
								{
									requiresNonNull = true;
									break;
								}
							}
						}
						if (requiresNonNull)
							graphType = new NonNullGraphType(graphType);
					}

					// parse the variable via ParseValue (for scalars) and ParseDictionary (for objects) as applicable
					try
					{
						variable.Value = await GetVariableValueAsync(graphType, variableDef, variableValue, visitor);
					}
					catch (ValidationError error)
					{
						(errors ??= new()).Add(error);
						continue;
					}
				}
				else if (variableDef.DefaultValue is not null)
				{
					// if the variable was not specified in the inputs, and a default literal value was specified, use the specified default variable value

					// parse the variable literal via ParseLiteral (for scalars) and ParseDictionary (for objects) as applicable
					try
					{
						variable.Value = ExecutionHelper.CoerceValue(graphType, variableDef.DefaultValue, variablesObj, null).Value;
					}
					catch (Exception ex)
					{
						(errors ??= new()).Add(new InvalidVariableError(this, variableDef, variableDefName, "Error coercing default value.", ex));
						continue;
					}
					variable.IsDefault = true;
				}
				else if (graphType is NonNullGraphType)
				{
					(errors ??= new()).Add(new InvalidVariableError(this, variableDef, variable.Name, "No value provided for a non-null variable."));
					continue;
				}

				// if the variable was not specified and no default was specified, do not set variable.Value

				// add the variable to the list of parsed variables defined for the operation
				variablesObj.Add(variable);
			}
		}

		// return the list of parsed variables defined for the operation
		return (variablesObj, errors);
	}

	/// <summary>
	/// Return the specified variable's value for the document from the attached <see cref="Variables"/> object.
	/// <br/><br/>
	/// Validates and parses the supplied input object according to the variable's type, and converts the object
	/// with <see cref="ScalarGraphType.ParseValue(object)"/> and
	/// <see cref="IInputObjectGraphType.ParseDictionary(IDictionary{string, object})"/> as applicable.
	/// <br/><br/>
	/// Since v3.3, returns null for variables set to null rather than the variable's default value.
	/// </summary>
	private ValueTask<object?> GetVariableValueAsync(IGraphType graphType, GraphQLVariableDefinition variableDef, object? input, IVariableVisitor? visitor)
	{
		return ParseValueAsync(graphType, variableDef, variableDef.Variable.Name.StringValue, input, visitor); //ISSUE:allocation

		// Coerces a value depending on the graph type.
		async ValueTask<object?> ParseValueAsync(IGraphType type, GraphQLVariableDefinition variableDef, VariableName variableName, object? value, IVariableVisitor? visitor)
		{
			if (type is IInputObjectGraphType inputObjectGraphType)
			{
				object? parsedValue = await ParseValueObjectAsync(inputObjectGraphType, variableDef, variableName, value, visitor);
				if (visitor is not null)
					await visitor.VisitObjectAsync(this, variableDef, variableName, inputObjectGraphType, value, parsedValue);
				return parsedValue;
			}
			else if (type is NonNullGraphType nonNullGraphType)
			{
				return value is null
					? throw new InvalidVariableError(this, variableDef, variableName, "Received a null input for a non-null variable.")
					: await ParseValueAsync(nonNullGraphType.ResolvedType!, variableDef, variableName, value, visitor);
			}
			else if (type is ListGraphType listGraphType)
			{
				var parsedValue = await ParseValueListAsync(listGraphType, variableDef, variableName, value, visitor);
				if (visitor is not null)
					await visitor.VisitListAsync(this, variableDef, variableName, listGraphType, value, parsedValue);
				return parsedValue;
			}
			else if (type is ScalarGraphType scalarGraphType)
			{
				object? parsedValue = ParseValueScalar(scalarGraphType, variableDef, variableName, value);
				if (visitor is not null)
					await visitor.VisitScalarAsync(this, variableDef, variableName, scalarGraphType, value, parsedValue);
				return parsedValue;
			}
			else
			{
				throw new InvalidVariableError(this, variableDef, variableName, $"The graph type '{type.Name}' is not an input graph type.");
			}
		}

		// Coerces a scalar value
		object? ParseValueScalar(ScalarGraphType scalarGraphType, GraphQLVariableDefinition variableDef, VariableName variableName, object? value)
		{
			try
			{
				return scalarGraphType.ParseValue(value);
			}
			catch (Exception ex)
			{
				throw new InvalidVariableError(this, variableDef, variableName, $"Unable to convert '{value?.ToString() ?? "null"}' to '{scalarGraphType.Name}'", ex);
			}
		}

		async ValueTask<IList<object?>?> ParseValueListAsync(ListGraphType listGraphType, GraphQLVariableDefinition variableDef, VariableName variableName, object? value, IVariableVisitor? visitor)
		{
			if (value is null)
				return null;

			// note: a list can have a single child element which will automatically be interpreted as a list of 1 elements. (see below rule)
			// so, to prevent a string as being interpreted as a list of chars (which get converted to strings), we ignore considering a string as an IEnumerable
			if (value is IEnumerable values && value is not string)
			{
				// create a list containing the parsed elements in the input list
				if (values is IList list)
				{
					var valueOutputs = new object?[list.Count];
					for (int index = 0; index < list.Count; index++)
					{
						// parse/validate values as required by graph type
						valueOutputs[index] = await ParseValueAsync(listGraphType.ResolvedType!, variableDef, new VariableName(variableName, index), list[index], visitor);
					}
					return valueOutputs;
				}
				else
				{
					var valueOutputs = new List<object?>(values is ICollection collection ? collection.Count : 0);
					int index = 0;
					foreach (object val in values)
					{
						// parse/validate values as required by graph type
						valueOutputs.Add(await ParseValueAsync(listGraphType.ResolvedType!, variableDef, new VariableName(variableName, index++), val, visitor));
					}
					return valueOutputs;
				}
			}
			else
			{
				// RULE: If the value passed as an input to a list type is not a list and not the null value,
				// then the result of input coercion is a list of size one, where the single item value is the
				// result of input coercion for the list’s item type on the provided value (note this may apply
				// recursively for nested lists).
				object? result = await ParseValueAsync(listGraphType.ResolvedType!, variableDef, variableName, value, visitor);
				return new object?[] { result };
			}
		}

		async ValueTask<object?> ParseValueObjectAsync(IInputObjectGraphType graphType, GraphQLVariableDefinition variableDef, VariableName variableName, object? value, IVariableVisitor? visitor)
		{
			if (value is null)
				return null;

			if (value is not IDictionary<string, object?> dic)
			{
				// RULE: The value for an input object should be an input object literal
				// or an unordered map supplied by a variable, otherwise a query error
				// must be thrown.

				throw new InvalidVariableError(this, variableDef, variableName, $"Unable to parse input as a '{graphType.Name}' type. Did you provide a List or Scalar value accidentally?");
			}

			var newDictionary = new Dictionary<string, object?>(dic.Count);
			foreach (var field in graphType.Fields)
			{
				var childFieldVariableName = new VariableName(variableName, field.Name);

				if (dic.TryGetValue(field.Name, out object? fieldValue))
				{
					// RULE: If the value null was provided for an input object field, and
					// the field’s type is not a non‐null type, an entry in the coerced
					// unordered map is given the value null. In other words, there is a
					// semantic difference between the explicitly provided value null
					// versus having not provided a value.

					// Note: we always call ParseValue even for null values, and if it
					// is a non-null graph type, the NonNullGraphType.ParseValue method will throw an error
					object? parsedFieldValue = await ParseValueAsync(field.ResolvedType!, variableDef, childFieldVariableName, fieldValue, visitor);
					if (visitor is not null)
						await visitor.VisitFieldAsync(this, variableDef, childFieldVariableName, graphType, field, fieldValue, parsedFieldValue);
					newDictionary[field.Name] = parsedFieldValue;
				}
				else if (field.DefaultValue is not null)
				{
					// RULE: If no value is provided for a defined input object field and that
					// field definition provides a default value, the default value should be used.
					newDictionary[field.Name] = field.DefaultValue;
				}
				else if (field.ResolvedType is NonNullGraphType)
				{
					// RULE: If no default value is provided and the input object field’s type is non‐null,
					// an error should be thrown.
					throw new InvalidVariableError(this, variableDef, childFieldVariableName, "No value provided for a non-null variable.");
				}

				// RULE: Otherwise, if the field is not required, then no
				// entry is added to the coerced unordered map.

				// so do not do this:    else { newDictionary[field.Name] = null; }
			}

			// RULE: The value for an input object should be an input object literal
			// or an unordered map supplied by a variable, otherwise a query error
			// must be thrown. In either case, the input object literal or unordered
			// map must not contain any entries with names not defined by a field
			// of this input object type, ***otherwise an error must be thrown.***
			List<string>? unknownFields = null;
			foreach (string key in dic.Keys)
			{
				var match = false;
				foreach (var field in graphType.Fields)
				{
					if (key == field.Name)
					{
						match = true;
						break;
					}
				}

				if (!match)
					(unknownFields ??= new(1)).Add(key);
			}

			if (unknownFields?.Count > 0)
				throw new InvalidVariableError(this, variableDef, variableName, $"Unrecognized input fields {string.Join(", ", unknownFields.Select(k => $"'{k}'"))} for type '{graphType.Name}'.");

			return graphType.ParseDictionary(newDictionary);
		}
	}

	/// <summary>
	/// Validates that the specified AST value is valid for the specified scalar or input graph type.
	/// Graph types that are lists or non-null types are handled appropriately by this method.
	/// Returns a string representing the errors encountered while validating the value.
	/// </summary>
	public string? IsValidLiteralValue(IGraphType type, GraphQLValue valueAst)
	{
		if (type is NonNullGraphType nonNull)
		{
			var ofType = nonNull.ResolvedType!;

			if (valueAst is null || valueAst is GraphQLNullValue)
			{
				return ofType is null
					? "Expected non-null value, found null"
					: $"Expected '{ofType.Name}!', found null.";
			}

			return IsValidLiteralValue(ofType, valueAst);
		}
		else if (valueAst is GraphQLNullValue)
			return null;

		if (valueAst is null)
			return null;

		// This function only tests literals, and assumes variables will provide
		// values of the correct type.
		if (valueAst is GraphQLVariable)
		{
			return null;
		}

		if (type is ListGraphType list)
		{
			var ofType = list.ResolvedType!;

			if (valueAst is GraphQLListValue listValue)
			{
				List<string>? errors = null;

				if (listValue.Values is not null)
				{
					for (int index = 0; index < listValue.Values.Count; ++index)
					{
						string? error = IsValidLiteralValue(ofType, listValue.Values[index]);
						if (error is not null)
							(errors ??= new()).Add($"In element #{index + 1}: [{error}]");
					}
				}

				return errors is null ? null : string.Join(" ", errors);
			}

			return IsValidLiteralValue(ofType, valueAst);
		}

		if (type is IInputObjectGraphType inputType)
		{
			if (valueAst is not GraphQLObjectValue objValue)
				return $"Expected '{inputType.Name}', found not an object.";

			var errors = new List<string>();

			// ensure every provided field is defined
			var fieldAsts = objValue.Fields;
			if (fieldAsts is not null)
			{
				foreach (var providedFieldAst in fieldAsts)
				{
					var found = inputType[providedFieldAst.Name];
					if (found is null)
						errors.Add($"In field '{providedFieldAst.Name}': Unknown field.");
				}
			}

			// ensure every defined field is valid
			foreach (var field in inputType.Fields)
			{
				var fieldAst = fieldAsts?.Find(_ => _.Name == field.Name);
				if (fieldAst is not null)
				{
					var error = IsValidLiteralValue(field.ResolvedType!, fieldAst.Value);
					if (error is not null)
						errors.Add($"In field '{field.Name}': [{error}]");
				}
				else if (field.ResolvedType is NonNullGraphType nonNull2 && field.DefaultValue is null)
					errors.Add($"Missing required field '{field.Name}' of type '{nonNull2.ResolvedType}'.");
			}

			return errors.Count > 0 ? null : string.Join(" ", errors);
		}

		if (type is ScalarGraphType scalar)
			return scalar.CanParseLiteral(valueAst) ? null : $"Expected type '{type.Name}', found {valueAst.Print()}.";

		throw new ArgumentOutOfRangeException(nameof(type), $"Type {type?.Name ?? "<NULL>"} is not a valid input graph type.");
	}
}

internal static class ValidationContextExtensions
{
	public static void AddListItem<T>(this ValidationContext context, string listKey, T item)
	{
		List<T> items;
		if (context.NonUserContext?.TryGetValue(listKey, out object? obj) is true)
		{
			items = (List<T>)obj!;
			items.Add(item);
		}
		else
		{
			items = new() { item };
			(context.NonUserContext ??= new()).Add(listKey, items);
		}
	}

	public static List<T>? GetList<T>(this ValidationContext context, string listKey, bool reset)
	{
		if (context.NonUserContext?.TryGetValue(listKey, out object? items) is true)
		{
			if (reset)
				context.NonUserContext.Remove(listKey);

			return (List<T>?)items;
		}
		else
			return null;
	}
}
