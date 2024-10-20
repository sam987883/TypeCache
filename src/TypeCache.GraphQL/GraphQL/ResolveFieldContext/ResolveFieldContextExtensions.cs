using System.Xml.Linq;
using GraphQL.Execution;
using GraphQL.Types;
using TypeCache.Extensions;

namespace GraphQL;

/// <summary>
/// Provides extension methods for <see cref="IResolveFieldContext"/> instances.
/// </summary>
public static class ResolveFieldContextExtensions
{
	/// <summary>
	/// Returns the value of the specified field argument, or <paramref name="defaultValue"/> when unspecified or when specified as <see langword="null"/>.
	/// Field and variable default values take precedence over the <paramref name="defaultValue"/> parameter.
	/// </summary>
	public static TType GetArgument<TType>(this IResolveFieldContext context, string name, TType defaultValue = default!)
		=> context.TryGetArgument(typeof(TType), name, out object? result)
			? (result is not null ? (TType)result : defaultValue)
			: defaultValue;

	/// <inheritdoc cref="GetArgument{TType}(IResolveFieldContext, string, TType)"/>
	public static object? GetArgument(this IResolveFieldContext context, Type argumentType, string name, object? defaultValue = null)
		=> context.TryGetArgument(argumentType, name, out object? result)
			? result ?? defaultValue
			: defaultValue;

	internal static bool TryGetArgument(this IResolveFieldContext context, Type argumentType, string name, out object? result)
	{
		var isIntrospection = context.ParentType is null ? (context.FieldDefinition.Name?.StartsWith("__") is true) : context.ParentType.IsIntrospectionType();
		var argumentName = isIntrospection ? name : (context.Schema?.NameConverter.NameForArgument(name, context.ParentType!, context.FieldDefinition) ?? name);

		if (context.Arguments?.TryGetValue(argumentName, out var argument) is not true)
		{
			result = null;
			return false;
		}

		if (argument.Value is IDictionary<string, object?> inputObject)
		{
			if (argumentType == typeof(object))
			{
				result = argument.Value;
				return true;
			}

			result = argumentType.Create();
			if (result is null)
				return false;

			inputObject.MapTo(result);
			return true;
		}

		result = argument.Value.GetPropertyValue(argumentType);
		return true;
	}

	/// <summary>Determines if the specified field argument has been provided in the GraphQL query request.</summary>
	public static bool HasArgument(this IResolveFieldContext context, string name)
	{
		var isIntrospection = context.ParentType is null ? (context.FieldDefinition.Name?.StartsWith("__") ?? false) : context.ParentType.IsIntrospectionType();
		var argumentName = isIntrospection ? name : (context.Schema?.NameConverter.NameForArgument(name, context.ParentType!, context.FieldDefinition) ?? name);
		return context.Arguments is not null && context.Arguments.TryGetValue(argumentName, out var value) && value.Source is not ArgumentSource.FieldDefault;
	}
}
