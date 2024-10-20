using System.Collections.ObjectModel;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;
using TypeCache.Extensions;

namespace GraphQL.Execution;

/// <summary>
/// Provides helper methods for document execution.
/// </summary>
public static class ExecutionHelper
{
	private static readonly IDictionary<string, ArgumentValue> _emptyDirectiveArguments = new ReadOnlyDictionary<string, ArgumentValue>(new Dictionary<string, ArgumentValue>());

	/// <summary>
	/// Returns a dictionary of directives with their arguments values for a field.
	/// Values will be retrieved from literals or variables as specified by the document.
	/// </summary>
	public static IDictionary<string, DirectiveInfo>? GetDirectives(GraphQLField field, Variables? variables, ISchema schema)
	{
		if (field.Directives is null || field.Directives.Count == 0)
			return null;

		Dictionary<string, DirectiveInfo>? directives = null;

		foreach (var directive in field.Directives.Items)
		{
			var dirDefinition = schema.Directives[directive.Name.StringValue];

			// KnownDirectivesInAllowedLocations validation rule should handle unknown directives, so
			// if someone purposely removed the validation rule, it would ignore unknown directives
			// while executing the request
			if (dirDefinition is null)
				continue;

			directives ??= new();
			directives[dirDefinition.Name] = new DirectiveInfo(dirDefinition, GetArguments(dirDefinition.Arguments!.ToArray(), directive.Arguments!.ToArray(), variables) ?? _emptyDirectiveArguments);
		}

		return directives;
	}

	/// <summary>
	/// Returns a dictionary of arguments and their values for a field or directive.
	/// Values will be retrieved from literals or variables as specified by the document.
	/// </summary>
	public static Dictionary<string, ArgumentValue>? GetArguments(QueryArgument[] definitionArguments, GraphQLArgument[] astArguments, Variables? variables)
	{
		if (definitionArguments.Length == 0)
			return null;

		var values = new Dictionary<string, ArgumentValue>(definitionArguments.Length);

		foreach (var arg in definitionArguments)
		{
			var value = astArguments.First(_ => _.Name == arg.Name).Value;
			values[arg.Name] = CoerceValue(arg.ResolvedType!, value, variables, arg.DefaultValue);
		}

		return values;
	}

	/// <summary>
	/// Coerces a literal value to a compatible .NET type for the variable's graph type.
	/// Typically this is a value for a field argument or default value for a variable.
	/// </summary>
	public static ArgumentValue CoerceValue(IGraphType type, GraphQLValue? input, Variables? variables = null, object? fieldDefault = null)
	{
		type.ThrowIfNull();

		if (type is NonNullGraphType nonNull)
		{
			// validation rules have verified that this is not null; if the validation rule was not executed, it
			// is assumed that the caller does not wish this check to be executed
			return CoerceValue(nonNull.ResolvedType!, input, variables, fieldDefault);
		}

		if (input is null)
			return new(fieldDefault, ArgumentSource.FieldDefault);

		if (input is GraphQLVariable variable)
		{
			var v = variables?.Find(variable.Name);
			if (v is not null && (v.IsDefault || v.ValueSpecified))
			{
				// get the variable value
				var value = v.Value;

				// wrap list if necessary
				// todo: v.Definition is not null for backwards compatibility for 7.x; remove in 8.x
				if (value is not null && v.Definition is not null && !IsASTListType(v.Definition.Type))
				{
					//---THE FOLLOWING CODE CRASHES THE .NET 7.0.304 COMPILER
					//
					//while (type is ListGraphType listType2)
					//{
					//    value = new object?[] { value };
					//    type = listType2.ResolvedType!;
					//}
					//
					//---SO INSTEAD WE HAVE:
					while (WrapType(ref type, ref value))
					{
					}

					static bool WrapType(ref IGraphType type, ref object? value)
					{
						if (type is ListGraphType listType)
						{
							value = new object?[] { value };
							type = listType.ResolvedType!;
							return true;
						}

						return false;
					}
					//-----
				}

				// return the variable
				return new ArgumentValue(value, v.IsDefault ? ArgumentSource.VariableDefault : ArgumentSource.Variable);

				static bool IsASTListType(GraphQLType type)
					=> type is GraphQLListType || (type is GraphQLNonNullType nonNullType && nonNullType.Type is GraphQLListType);
			}
			else
				return new(fieldDefault, ArgumentSource.FieldDefault);
		}

		if (type is ScalarGraphType scalarType)
			return new(scalarType.ParseLiteral(input), ArgumentSource.Literal);

		if (input is GraphQLNullValue)
			return ArgumentValue.NullLiteral;

		if (type is ListGraphType listType)
		{
			var listItemType = listType.ResolvedType!;

			if (input is GraphQLListValue list)
			{
				var count = list.Values?.Count ?? 0;
				if (count == 0)
					return new ArgumentValue(Array.Empty<object>(), ArgumentSource.Literal);

				var values = new object?[count];
				for (int i = 0; i < count; ++i)
					values[i] = CoerceValue(listItemType, list.Values![i], variables).Value;

				return new(values, ArgumentSource.Literal);
			}
			else
				return new(new[] { CoerceValue(listItemType, input, variables).Value }, ArgumentSource.Literal);
		}

		if (type is IInputObjectGraphType inputObjectGraphType)
		{
			if (input is not GraphQLObjectValue objectValue)
				throw new ArgumentOutOfRangeException(nameof(input), $"Expected object value for '{inputObjectGraphType.Name}', found not an object '{input.Print()}'.");

			var obj = new Dictionary<string, object?>();

			foreach (var field in inputObjectGraphType.Fields)
			{
				// https://spec.graphql.org/October2021/#sec-Input-Objects
				var objectField = objectValue.Field(field.Name);
				if (objectField is not null)
				{
					// Rules covered:

					// If a literal value is provided for an input object field, an entry in the coerced unordered map is
					// given the result of coercing that value according to the input coercion rules for the type of that field.

					// If a variable is provided for an input object field, the runtime value of that variable must be used.
					// If the runtime value is null and the field type is non‐null, a field error must be thrown.
					// If no runtime value is provided, the variable definition’s default value should be used.
					// If the variable definition does not provide a default value, the input object field definition’s
					// default value should be used.

					var value = CoerceValue(field.ResolvedType!, objectField.Value, variables, field.DefaultValue);
					// when a optional variable is specified for the input field, and the variable is not defined, and
					//   when there is no default value specified for the input field, then do not add the entry to the
					//   unordered map.
					if (value.Source is not ArgumentSource.FieldDefault || value.Value is not null)
						obj[field.Name] = value.Value;
				}
				else if (field.DefaultValue is not null)
				{
					// If no value is provided for a defined input object field and that field definition provides a default value,
					// the default value should be used.
					obj[field.Name] = field.DefaultValue;
				}
				// Otherwise, if the field is not required, then no entry is added to the coerced unordered map.

				// Covered by validation rules:
				// If no default value is provided and the input object field’s type is non‐null, an error should be
				// thrown.
			}

			return new ArgumentValue(inputObjectGraphType.ParseDictionary(obj), ArgumentSource.Literal);
		}

		throw new ArgumentOutOfRangeException(nameof(input), $"Unknown type of input object '{type.GetType()}'");
	}
}
