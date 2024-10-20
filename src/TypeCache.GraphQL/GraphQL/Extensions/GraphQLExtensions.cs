using System.Collections;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using GraphQLParser.AST;
using GraphQLParser.Visitors;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Utilities;

namespace GraphQL;

/// <summary>
/// Provides extension methods for working with graph types.
/// </summary>
public static class GraphQLExtensions
{
	/// <summary>
	/// Determines if this graph type is an introspection type.
	/// </summary>
	internal static bool IsIntrospectionType(this IGraphType graphType)
		=> graphType?.Name?.StartsWithIgnoreCase("__") is true;

	/// <summary>
	/// Indicates if the graph type is a union, interface or object graph type.
	/// </summary>
	public static bool IsCompositeType(this IGraphType graphType)
		=> graphType is IObjectGraphType || graphType is IInterfaceGraphType || graphType is UnionGraphType;

	/// <summary>
	/// Indicates if the graph type is a scalar graph type.
	/// </summary>
	public static bool IsLeafType(this IGraphType graphType)
		=> (graphType.GetNamedGraphType() ?? graphType) switch
		{
			ScalarGraphType => true,
			NonNullGraphType nonNullGraphType => nonNullGraphType.Type?.GetNamedType().IsAssignableTo<ScalarGraphType>() is true,
			ListGraphType listGraphType => listGraphType.Type?.GetNamedType().IsAssignableTo<ScalarGraphType>() is true,
			_ => false
		};

	/// <summary>
	/// Indicates if the type is an input graph type (scalar or input object).<br/><br/>
	/// <c><see href="https://spec.graphql.org/October2021/#sec-Input-and-Output-Types"/></c>
	/// </summary>
	public static bool IsInputType(this Type type)
		=> type switch
		{
			_ when type.Is(typeof(GraphQLClrInputTypeReference<>)) => true,
			_ when type.Is(typeof(GraphQLClrOutputTypeReference<>)) => false,
			_ when type.Is(typeof(NonNullGraphType<>)) => type.GenericTypeArguments[0].IsInputType(),
			_ when type.Is(typeof(ListGraphType<>)) => type.GenericTypeArguments[0].IsInputType(),
			_ when type.IsAssignableTo<ScalarGraphType>() => true,
			_ when type.IsAssignableTo<IInputObjectGraphType>() => true,
			_ => false
		};

	/// <summary>
	/// Indicates if the graph type is an input graph type (scalar or input object).<br/><br/>
	/// <c><see href="https://spec.graphql.org/October2021/#sec-Input-and-Output-Types"/></c>
	/// </summary>
	public static bool IsInputType(this IGraphType graphType)
	{
		var namedGraphType = graphType.GetNamedGraphType();
		var namedType = graphType.GetNamedType()!;

		return namedGraphType is ScalarGraphType
			|| namedGraphType is IInputObjectGraphType
			|| typeof(ScalarGraphType).IsAssignableFrom(namedType)
			|| typeof(IInputObjectGraphType).IsAssignableFrom(namedType);
	}

	/// <summary>
	/// Indicates if the type is an output graph type (scalar, object, interface or union).<br/><br/>
	/// <c><see href="https://spec.graphql.org/October2021/#sec-Input-and-Output-Types"/></c>
	/// </summary>
	public static bool IsOutputType(this Type type)
		=> type switch
		{
			_ when type.Is(typeof(GraphQLClrInputTypeReference<>)) => false,
			_ when type.Is(typeof(GraphQLClrOutputTypeReference<>)) => true,
			_ when type.Is(typeof(NonNullGraphType<>)) => type.GenericTypeArguments[0].IsOutputType(),
			_ when type.Is(typeof(ListGraphType<>)) => type.GenericTypeArguments[0].IsOutputType(),
			_ when type.IsAssignableTo<ScalarGraphType>() => true,
			_ when type.IsAssignableTo<IInputObjectGraphType>() => false,
			_ when type.IsAssignableTo<IObjectGraphType>() => true,
			_ when type.IsAssignableTo<IInterfaceGraphType>() => true,
			_ when type.IsAssignableTo<UnionGraphType>() => true,
			_ => false
		};

	/// <summary>
	/// Indicates if the graph type is an output graph type (scalar, object, interface or union).<br/><br/>
	/// <c><see href="https://spec.graphql.org/October2021/#sec-Input-and-Output-Types"/></c>
	/// </summary>
	public static bool IsOutputType(this IGraphType graphType)
	{
		var namedGraphType = graphType.GetNamedGraphType();
		var namedType = graphType.GetNamedType()!;

		return namedGraphType is ScalarGraphType
			|| namedGraphType is IObjectGraphType
			|| namedGraphType is IInterfaceGraphType
			|| namedGraphType is UnionGraphType
			|| typeof(ScalarGraphType).IsAssignableFrom(namedType)
			|| typeof(IObjectGraphType).IsAssignableFrom(namedType)
			|| typeof(IInterfaceGraphType).IsAssignableFrom(namedType)
			|| typeof(UnionGraphType).IsAssignableFrom(namedType);
	}

	/// <summary>
	/// Indicates if the graph type is an input object graph type.
	/// </summary>
	public static bool IsInputObjectType(this IGraphType graphType)
	{
		var namedGraphType = graphType.GetNamedGraphType();

		return graphType switch
		{
			null => false,
			IInputObjectGraphType => true,
			_ when namedGraphType is not null => namedGraphType is IInputObjectGraphType,
			NonNullGraphType nonNullGraphType => nonNullGraphType.Type?.GetNamedType().IsAssignableTo<IInputObjectGraphType>() is true,
			ListGraphType listGraphType => listGraphType.Type?.GetNamedType().IsAssignableTo<IInputObjectGraphType>() is true,
			_ => false
		};
	}

	internal static bool IsGraphQLTypeReference(this IGraphType? graphType)
		=> graphType?.GetNamedGraphType() is GraphQLTypeReference;

	/// <summary>
	/// Unwraps any list/non-null graph type wrappers from a graph type and returns the base graph type.
	/// </summary>
	public static IGraphType? GetNamedGraphType(this IGraphType graphType)
		=> graphType switch
		{
			NonNullGraphType nonNull => nonNull.ResolvedType?.GetNamedGraphType(),
			ListGraphType list => list.ResolvedType?.GetNamedGraphType(),
			_ => graphType
		};

	/// <summary>
	/// Unwraps any list/non-null graph type wrappers from a graph type and returns the base type.
	/// </summary>
	public static Type? GetNamedType(this IGraphType graphType)
		=> graphType switch
		{
			NonNullGraphType nonNull => nonNull.Type?.GetNamedType(),
			ListGraphType list => list.Type?.GetNamedType(),
			_ => null
		};

	/// <summary>
	/// Unwraps any list/non-null graph type wrappers from a graph type and returns the base type.
	/// </summary>
	public static Type GetNamedType(this Type type)
		=> type switch
		{
			{ IsGenericType: false } => type,
			_ when type.Is(typeof(NonNullGraphType<>)) || type.Is(typeof(ListGraphType<>)) => type.GenericTypeArguments[0].GetNamedType(),
			_ => type
		};

	/// <summary>
	/// An Interface defines a list of fields; Object types that implement that interface are guaranteed to implement those fields.
	/// Whenever the type system claims it will return an interface, it will return a valid implementing type.
	/// </summary>
	/// <param name="interfaceGraphType">The interface graph type.</param>
	/// <param name="type">The object graph type to verify it against.</param>
	/// <param name="throwError">Set to <see langword="true"/> to generate an error if the type does not match the interface.</param>
	public static bool IsValidInterfaceFor(this IInterfaceGraphType interfaceGraphType, IObjectGraphType type, bool throwError = true)
	{
		foreach (var field in interfaceGraphType.Fields)
		{
			var found = type[field.Name];
			if (found is null)
				return throwError
					? throw new ArgumentException($"Type {type.GetType().GetTypeName()} with name '{type.Name}' does not implement interface {interfaceGraphType.GetType().GetTypeName()} with name '{interfaceGraphType.Name}'. It has no field '{field.Name}'.")
					: false;

			if (found.ResolvedType is not null && found.ResolvedType is not GraphQLTypeReference
				&& field.ResolvedType is not null && field.ResolvedType is not GraphQLTypeReference
				&& !IsSubtypeOf(found.ResolvedType, field.ResolvedType))
			{
				return throwError
					? throw new ArgumentException($"Type {type.GetType().GetTypeName()} with name '{type.Name}' does not implement interface {interfaceGraphType.GetType().GetTypeName()} with name '{interfaceGraphType.Name}'. Field '{field.Name}' must be of type '{field.ResolvedType}' or covariant from it, but in fact it is of type '{found.ResolvedType}'.")
					: false;
			}
		}

		return true;
	}

	/// <summary>
	/// Adds a key-value metadata pair to the specified provider.
	/// </summary>
	/// <typeparam name="TMetadataProvider">The type of metadata provider. Generics are used here to let compiler infer the returning type to allow methods chaining.</typeparam>
	/// <param name="provider">Metadata provider which must implement <see cref="IProvideMetadata"/> interface.</param>
	/// <param name="key">String key.</param>
	/// <param name="value">Arbitrary value.</param>
	/// <returns>The reference to the specified <paramref name="provider"/>.</returns>
	public static TMetadataProvider WithMetadata<TMetadataProvider>(this TMetadataProvider provider, string key, object? value)
		where TMetadataProvider : IProvideMetadata
	{
		provider.Metadata[key] = value;
		return provider;
	}

	/// <summary>
	/// Provided a type and a super type, return <see langword="true"/> if the first type is either
	/// equal or a subset of the second super type (covariant).
	/// When <paramref name="allowScalarsForLists"/> is <see langword="true"/>, it will allow
	/// scalar types to match list types, as long as the inner types match, pursuant to the
	/// GraphQL June 2018 Specification, section 3.11 "List: Input Coercion".
	/// </summary>
	// TODO: roll this into IsSubtypeOf and remove the allowScalarsForLists parameter
	public static bool IsSubtypeOf(this IGraphType maybeSubType, IGraphType superType)
	{
		//maybeSubType = variableType
		//superType = locationType

		// >> - Return {true} if {variableType} and {locationType} are identical, otherwise {false}.
		if (maybeSubType.Equals(superType))
			return true;

		// >> If {locationType} is a non-null type
		// If superType is non-null, maybeSubType must also be nullable.
		if (superType is NonNullGraphType sup1)
		{
			// >> - If {variableType} is NOT a non-null type, return {false}.
			if (maybeSubType is not NonNullGraphType sub)
				return false;

			// >> - Let {nullableLocationType} be the unwrapped nullable type of {locationType}.
			// >> - Let {nullableVariableType} be the unwrapped nullable type of {variableType}.
			// >> - Return {AreTypesCompatible(nullableVariableType, nullableLocationType)}.
			return IsSubtypeOf(sub.ResolvedType!, sup1.ResolvedType!);
		}
		// >> - Otherwise, if {variableType} is a non-null type:
		else if (maybeSubType is NonNullGraphType sub)
		{
			// >> - Let {nullableVariableType} be the nullable type of {variableType}.
			// >> - Return {AreTypesCompatible(nullableVariableType, locationType)}.
			return IsSubtypeOf(sub.ResolvedType!, superType);
		}

		// If superType type is a list, maybeSubType type must also be a list, unless allowScalarsForLists is true.
		// >> Otherwise, if {locationType} is a list type:
		if (superType is ListGraphType sup)
		{
			// >> Let {itemLocationType} be the unwrapped item type of {locationType}.
			//var itemLocationType = sup.ResolvedType!;

			// >> If {variableType} is NOT a list type:
			if (maybeSubType is not ListGraphType sub)
			{
				// >> - If {itemLocationType} is a non-null type:
				if (sup.ResolvedType is NonNullGraphType sup2)
				{
					// >>   - Let {nullableItemLocationType} be the unwrapped nullable type of {itemLocationType}.
					// >>   - Return {AreTypesCompatible(variableType, nullableItemLocationType)}.
					return IsSubtypeOf(maybeSubType, sup2);
				}
				else
				{
					// >> - Otherwise, return {AreTypesCompatible(variableType, itemLocationType)}.
					return IsSubtypeOf(maybeSubType, sup.ResolvedType!);
				}
			}
			else
			{
				// >> - Let {itemVariableType} be the unwrapped item type of {variableType}.
				// >> - Return {AreTypesCompatible(itemVariableType, itemLocationType)}.
				return IsSubtypeOf(sub.ResolvedType!, sup.ResolvedType!);
			}
		}
		// >> - Otherwise, if {variableType} is a list type, return {false}.
		else if (maybeSubType is ListGraphType)
		{
			// If superType is not a list, maybeSubType must also be not a list.
			return false;
		}

		// >> - Return {true} if {variableType} and {locationType} are identical, otherwise {false}.
		// If superType type is an abstract type, maybeSubType type may be a currently
		// possible object type.
		if (superType is IAbstractGraphType type && maybeSubType is IObjectGraphType)
		{
			return type.IsPossibleType(maybeSubType);
		}

		return false;
	}

	/// <summary>
	/// Provided two composite types, determine if they "overlap". Two composite
	/// types overlap when the Sets of possible concrete types for each intersect.
	///
	/// This is often used to determine if a fragment of a given type could possibly
	/// be visited in a context of another type.
	///
	/// This function is commutative.
	/// </summary>
	public static bool DoTypesOverlap(IGraphType typeA, IGraphType typeB)
	{
		if (typeA.Equals(typeB))
			return true;

		var b = typeB as IAbstractGraphType;

		if (typeA is IAbstractGraphType a)
		{
			if (b is not null)
			{
				// DO NOT USE LINQ ON HOT PATH
				foreach (var type in a.PossibleTypes)
				{
					if (b.IsPossibleType(type))
						return true;
				}

				return false;
			}

			return a.IsPossibleType(typeB);
		}

		return b?.IsPossibleType(typeA) is true;
	}

	/// <summary>
	/// Returns a value indicating whether the provided value is a valid default value
	/// for the specified input graph type.
	/// </summary>
	public static bool IsValidDefault(this IGraphType type, object? value)
	{
		type.ThrowIfNull();

		if (type is NonNullGraphType nonNullGraphType)
			return value is not null && nonNullGraphType.ResolvedType!.IsValidDefault(value);

		if (value is null)
			return true;

		// Convert IEnumerable to GraphQL list. If the GraphQLType is a list, but
		// the value is not an IEnumerable, convert the value using the list's item type.
		if (type is ListGraphType listType)
		{
			var itemType = listType.ResolvedType!;

			if (value is not string && value is IEnumerable list)
			{
				foreach (var item in list)
				{
					if (!IsValidDefault(itemType, item))
						return false;
				}

				return true;
			}
			else
				return IsValidDefault(itemType, value);
		}

		if (type is IInputObjectGraphType inputObjectGraphType)
			return inputObjectGraphType.IsValidDefault(value);

		if (type is ScalarGraphType scalar)
			return scalar.IsValidDefault(value);

		throw new ArgumentOutOfRangeException(nameof(type), $"Must provide Input Type, cannot use {type.GetType().Name} '{type}'");
	}

	/// <summary>
	/// Attempts to serialize a value into an AST representation for a specified graph type.
	/// May throw exceptions during the serialization process.
	/// </summary>
	public static GraphQLValue ToAST(this IGraphType type, object? value)
	{
		type.ThrowIfNull();

		if (type is NonNullGraphType nonnull)
		{
			var astValue = ToAST(nonnull.ResolvedType!, value);
			if (astValue is GraphQLNullValue)
				throw new InvalidOperationException($"Unable to get an AST representation of {(value is null ? "null" : $"'{value}'")} value for type '{nonnull}'.");

			return astValue;
		}

		if (type is ScalarGraphType scalar)
			return scalar.ToAST(value) ?? scalar.ThrowASTConversionError(value);

		if (value is null)
			return Singleton<GraphQLNullValue>.Instance;

		// Convert IEnumerable to GraphQL list. If the GraphQLType is a list, but
		// the value is not an IEnumerable, convert the value using the list's item type.
		if (type is ListGraphType listType)
		{
			var itemType = listType.ResolvedType!;

			if (value is not string && value is IEnumerable list)
			{
				var values = list
					.Cast<object>()
					.Select(_ => ToAST(itemType, _))
					.ToList();

				return new GraphQLListValue { Values = values };
			}

			return ToAST(itemType, value);
		}

		// Populate the fields of the input object by creating ASTs from each value
		// in the dictionary according to the fields in the input type.
		if (type is IInputObjectGraphType input)
			return input.ToAST(value) ?? throw new InvalidOperationException($"Unable to get an AST representation of the input object type '{input.Name}' for '{value}'.");

		throw new ArgumentOutOfRangeException(nameof(type), $"Must provide Input Type, cannot use {type.GetType().Name} '{type}'");
	}

	// optimized version of Print(this ASTNode node) for primitive values
	internal static string Print(this IGraphType type, object value)
		=> (type, value) switch
		{
			(GraphQLStringType, string text) when text.IndexOfAny(['\b', '\f', '\n', '\r', '\t', '\\', '"']) == -1 => $"\"{text}\"",
			(GraphQLBooleanType, false) => "false",
			(GraphQLBooleanType, true) => "true",
			_ => type.ToAST(value).Print(),
		};

	/// <summary>
	/// Returns a string representation of the specified node.
	/// </summary>
	internal static string Print(this ASTNode node)
	{
		using var writer = new StringWriter();
		_sdlPrinter.PrintAsync(node, writer).GetAwaiter().GetResult(); // actually is sync
		return writer.ToString()!;
	}

	private static readonly SDLPrinter _sdlPrinter = new();

	internal static object? ParseAnyLiteral(this GraphQLValue value)
		=> value switch
		{
			GraphQLObjectValue v => ParseObject(v),
			GraphQLListValue v => ParseList(v),
			GraphQLIntValue v => ParseInt(v),
			GraphQLFloatValue v => ParseFloat(v),
			GraphQLStringValue v => v.Value.Length == 0 ? string.Empty : (string)v.Value,
			GraphQLBooleanValue v => (v.Value == "true").Box(),
			GraphQLEnumValue e => e.Name.StringValue, // TODO: think about it later if/when refactoring federation
			GraphQLNullValue _ => null,
			IHasValueNode node => throw new InvalidOperationException($"Unable to convert '{node.Value}' literal from AST representation to any possible type"),
			_ => throw new InvalidOperationException($"Unable to convert '{value}' literal from AST representation to any possible type")
		};

	private static IDictionary<string, object?> ParseObject(GraphQLObjectValue v)
	{
		if (v.Fields is null || v.Fields.Count == 0)
			return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(0));

		var @object = new Dictionary<string, object?>(v.Fields.Count);
		foreach (var field in v.Fields)
			@object.Add(field.Name.StringValue, ParseAnyLiteral(field.Value));

		return @object;
	}

	private static IList<object?> ParseList(GraphQLListValue v)
	{
		if (v.Values is null || v.Values.Count == 0)
			return Array<object?>.Empty;

		var array = new object?[v.Values.Count];
		for (int i = 0; i < v.Values.Count; ++i)
			array[i] = ParseAnyLiteral(v.Values[i]);

		return array;
	}

	private static object ParseInt(GraphQLIntValue graphQLIntValue)
		=> graphQLIntValue switch
		{
			_ when graphQLIntValue.Value.Length == 0 => throw new InvalidOperationException("Invalid number (empty string)"),
			_ when int.TryParse(graphQLIntValue.Value, out var value) => value,
			_ when long.TryParse(graphQLIntValue.Value, out var value) => value,
			_ when decimal.TryParse(graphQLIntValue.Value, out var value) => value,
			_ when BigInteger.TryParse(graphQLIntValue.Value, out var value) => value,
			_ => throw new InvalidOperationException($"Invalid number {graphQLIntValue.Value}")
		};

	private static object ParseFloat(GraphQLFloatValue v)
	{
		if (v.Value.Length == 0)
			throw new InvalidOperationException("Invalid number (empty string)");

		// the idea is to see if there is a loss of accuracy of value
		// for example, 12.1 or 12.11 is double but 12.10 is decimal
		if (!double.TryParse(v.Value, out double dbl))
			throw new InvalidOperationException($"Unable to convert '{(string)v.Value}' literal from AST representation to any possible type");

		// it is possible for a FloatValue to overflow a decimal; however, with a double, it just returns Infinity or -Infinity
		if (decimal.TryParse(v.Value, out decimal dec))
		{
			// Cast the decimal to our struct to avoid the decimal.GetBits allocations.
			var decBits = Unsafe.As<decimal, DecimalData>(ref dec);
			decimal temp = new(dbl);
			var dblAsDecBits = Unsafe.As<decimal, DecimalData>(ref temp);
			if (!decBits.Equals(dblAsDecBits))
				return dec;
		}

		return dbl;
	}
}
