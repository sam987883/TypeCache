// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLBooleanType : ScalarGraphType
{
	public GraphQLBooleanType()
	{
		this.Name = nameof(Boolean);
		this.Description = "True/False.";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => true,
			IHasValueNode node => bool.TryParse(node.Value.Span, out var _),
			_ => false
		};

	public override bool CanParseValue(object? value)
		=> value switch
		{
			null or bool => true,
			string text => bool.TryParse(text, out _),
			_ => false
		};

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => null,
			IHasValueNode node when bool.TryParse(node.Value.Span, out var parsedValue) => parsedValue,
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or bool => value,
			string text when bool.TryParse(text, out var parsedValue) => parsedValue,
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> this.ParseValue(value) switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			true => Singleton<GraphQLTrueBooleanValue>.Instance,
			false => Singleton<GraphQLFalseBooleanValue>.Instance,
			_ => this.ThrowASTConversionError(value)
		};
}
