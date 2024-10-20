// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// Also called Enums, enumeration types are a special kind of scalar that is restricted to a
/// particular set of allowed values. This allows you to:
/// 1. Validate that any arguments of this type are one of the allowed values.
/// 2. Communicate through the type system that a field will always be one of a finite set of values.
/// </summary>
public class GraphQLEnumType : ScalarGraphType
{
	public sealed class EnumValue(string name, string? description, string? deprecationReason, Enum? value) : MetadataProvider, IProvideDescription, IProvideDeprecationReason
	{
		public string Name { get; } = name;

		public string? Description { get; set; } = description;

		public string? DeprecationReason { get; set; } = deprecationReason;

		public Enum? Value { get; } = value;
	}

	public IReadOnlyDictionary<string, EnumValue> Values { get; set; }

	/// <inheritdoc/>
	public override bool CanParseLiteral(GraphQLValue graphQLValue)
		=> graphQLValue switch
		{
			GraphQLEnumValue graphQLEnumValue => this.Values.ContainsKey(graphQLEnumValue.Name.StringValue),
			GraphQLNullValue _ => true,
			_ => false
		};

	/// <inheritdoc/>
	public override bool CanParseValue(object? value)
		=> value switch
		{
			string name => this.Values.ContainsKey(name),
			Enum token => this.Values.ContainsKey(token.Name()),
			null => true,
			_ => false
		};

	/// <inheritdoc/>
	public override object? ParseLiteral(GraphQLValue graphQLValue)
		=> graphQLValue switch
		{
			GraphQLEnumValue graphQLEnumValue when this.Values.TryGetValue(graphQLEnumValue.Name.StringValue, out var enumValue) => enumValue.Name,
			GraphQLNullValue _ => null,
			_ => this.ThrowLiteralConversionError(graphQLValue)
		};

	/// <inheritdoc/>
	public override object? ParseValue(object? value)
		=> value switch
		{
			string name when this.Values.TryGetValue(name, out var enumValue) => enumValue.Name,
			null => null,
			_ => ThrowValueConversionError(value)
		};

	/// <inheritdoc/>
	public override object? Serialize(object? value)
	=> value switch
	{
			Enum token when this.Values.TryGetValue(token.Name(), out var enumValue) => enumValue.Name,
			string name when this.Values.TryGetValue(name, out var enumValue) => enumValue.Name,
			null => null,
			_ => throw new InvalidOperationException($"Unable to serialize '{value}' value of type '{value.GetType().FullName}' to the enumeration type '{this.Name}'. Enumeration does not contain such value. Available values: {string.Join(", ", this.Values.Select(_ => $"'{_.Value.Name}'."))}."),
			//_ => ThrowValueConversionError(value)
		};

	/// <inheritdoc/>
	public override GraphQLValue ToAST(object? value)
		=> value switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			Enum token when this.Values.TryGetValue(token.Name(), out var enumValue) => new GraphQLEnumValue(new GraphQLName(enumValue.Name)),
			_ => this.ThrowASTConversionError(value)
		};
}

/// <inheritdoc/>
public sealed class GraphQLEnumType<T> : GraphQLEnumType
	where T : struct, Enum
{
	public GraphQLEnumType()
		: base()
	{
		this.Name = typeof(T).GraphQLName();
		this.Description = typeof(T).GraphQLDescription();
		this.DeprecationReason = typeof(T).GraphQLDeprecationReason();

		Func<string, string>? changeEnumCase = Enum<T>.Attributes switch
		{
			_ when Enum<T>.Attributes.Any<ConstantCaseAttribute>() => value => value.ToConstantCase(),
			_ when Enum<T>.Attributes.Any<CamelCaseAttribute>() => value => value.ToCamelCase(),
			_ when Enum<T>.Attributes.Any<PascalCaseAttribute>() => value => value.ToPascalCase(),
			_ => null
		};

		this.Values = Enum<T>.Values
			.Where(_ => !_.Attributes().Any<GraphQLIgnoreAttribute>())
			.Select(_ => new EnumValue(
				_.Attributes().FirstOrDefault<GraphQLNameAttribute>()?.Name ?? changeEnumCase?.Invoke(_.Name()) ?? _.Name(),
				_.Attributes().FirstOrDefault<GraphQLDescriptionAttribute>()?.Description,
				_.Attributes().FirstOrDefault<GraphQLDeprecationReasonAttribute>()?.DeprecationReason,
				_))
			.ToDictionary(_ => _.Name)
			.ToFrozenDictionary();
	}
}
