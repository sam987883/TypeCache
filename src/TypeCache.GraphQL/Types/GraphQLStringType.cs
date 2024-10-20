// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLStringType : ScalarGraphType, IStringGraphType
{
	public GraphQLStringType()
	{
		this.Name = nameof(String);
		this.Description = "Any string value.";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value is GraphQLStringValue || value is GraphQLNullValue;

	public override bool CanParseValue(object? value)
		=> value is string || value is null;

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLStringValue graphValue => graphValue.Value.Span.ToString(),
			GraphQLNullValue _ => null,
			_ => ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or string => value,
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> this.ParseValue(value) switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			string text => new GraphQLStringValue(text),
			_ => this.ThrowASTConversionError(value)
		};
}

public sealed class GraphQLStringType<T> : ScalarGraphType, IStringGraphType
	where T : IFormattable, ISpanParsable<T>
{
	public GraphQLStringType()
	{
		this.Name = typeof(T).Name;
		this.Description = Invariant($"{typeof(T).Namespace}.{typeof(T).Name}");
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => true,
			IHasValueNode node => T.TryParse(node.Value.Span, CultureInfo.InvariantCulture, out var _),
			_ => false
		};

	public override bool CanParseValue(object? value)
		=> value switch
		{
			null or T => true,
			string text => T.TryParse(text, CultureInfo.InvariantCulture, out _),
			_ => false
		};

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => null,
			IHasValueNode node when T.TryParse(node.Value.Span, CultureInfo.InvariantCulture, out var parsedValue) => parsedValue,
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or T => value,
			string text when T.TryParse(text, CultureInfo.InvariantCulture, out var parsedValue) => parsedValue,
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> this.ParseValue(value) switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			string x => new GraphQLStringValue(x),
			DateOnly x => new GraphQLStringValue(x.ToISO8601()),
			DateTime x => new GraphQLStringValue(x.ToISO8601()),
			DateTimeOffset x => new GraphQLStringValue(x.ToISO8601()),
			TimeOnly x => new GraphQLStringValue(x.ToISO8601()),
			TimeSpan x => new GraphQLStringValue(x.ToText()),
			Guid x => new GraphQLStringValue(x.ToText()),
			IFormattable formattable => new GraphQLStringValue(formattable.ToString(null, CultureInfo.InvariantCulture)),
			_ => throw new UnreachableException(Invariant($"{nameof(GraphQLStringType)}: [{value}] does not implement {nameof(IFormattable)}."))
		};
}
