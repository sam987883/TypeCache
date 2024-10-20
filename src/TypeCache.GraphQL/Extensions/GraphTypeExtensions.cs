// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Reflection;
using System.Text;
using GraphQL;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class GraphTypeExtensions
{
	/// <summary>
	/// Determines whether the indicated type implements <see cref="IGraphType"/>.
	/// </summary>
	public static bool IsGraphType(this Type type)
		=> typeof(IGraphType).IsAssignableFrom(type);

	public static string ToString(this IGraphType @this, object? value)
	{
		if (value is null)
			return "null";

		return @this switch
		{
			NonNullGraphType nonNull => nonNull.ResolvedType!.ToString(value),
			ListGraphType list => $"[{string.Join(", ", ((IEnumerable)value).Cast<object>().Select(_ => list.ResolvedType!.ToString(_)))}]",
			IInputObjectGraphType input => input.ToString(value),
			GraphQLEnumType enumeration => (enumeration.ToAST(value) ?? throw new ArgumentOutOfRangeException(nameof(value), $"Unable to convert '{value}' to AST for enumeration type '{enumeration.Name}'.")).Print(),
			ScalarGraphType scalar => (scalar.ToAST(value) ?? throw new ArgumentOutOfRangeException(nameof(value), $"Unable to convert '{value}' to AST for scalar type '{scalar.Name}'.")).Print(),
			_ => throw new NotSupportedException($"Unsupported graph type '{@this.Name}'")
		};
	}

	public static string ToString(this IInputObjectGraphType @this, object value)
	{
		var output = new StringBuilder("{ ");

		var items = new List<string>(@this.Fields.Count);
		if (value is IDictionary<string, object?> dictionary)
		{
			@this.Fields.ForEach(field =>
			{
				var propertyName = field.GetMetadata<string>(InputObjectGraphType.ORIGINAL_EXPRESSION_PROPERTY_NAME) ?? field.Name;
				if (dictionary.TryGetValue(propertyName, out var propertyValue))
					items.Add(Invariant($"{field.Name}: {field.ResolvedType!.ToString(propertyValue)}"));
			});
		}
		else
		{
			@this.Fields.ForEach(field =>
			{
				var propertyName = field.GetMetadata<string>(InputObjectGraphType.ORIGINAL_EXPRESSION_PROPERTY_NAME) ?? field.Name;
				var type = value.GetType();
				var propertyInfo = value.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					?? value.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)!;

				if (propertyInfo is not null)
					items.Add(Invariant($"{field.Name}: {field.ResolvedType!.ToString(propertyInfo.GetValueEx(value))}"));
			});
		}

		return output.AppendJoin(", ", items).Append(" }").ToString();
	}
}
