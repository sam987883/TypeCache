// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLBooleanType : ScalarGraphType
{
	public GraphQLBooleanType()
	{
		this.Name = "System_Boolean";
		this.Description = "System.Boolean";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => true,
			IHasValueNode node => true,
			_ => false
		};

	public override bool CanParseValue(object? value)
		=> value switch
		{
			null => true,
			bool => true,
			_ => false
		};

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => null,
			IHasValueNode node => bool.Parse(node.Value.Span),
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or bool => value,
			string text when text.EqualsIgnoreCase(bool.TrueString) => true,
			string text when text.EqualsIgnoreCase(bool.FalseString) => false,
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> value switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			true => Singleton<GraphQLTrueBooleanValue>.Instance,
			false => Singleton<GraphQLFalseBooleanValue>.Instance,
			string text when text.EqualsIgnoreCase(bool.TrueString) => Singleton<GraphQLTrueBooleanValue>.Instance,
			string text when text.EqualsIgnoreCase(bool.FalseString) => Singleton<GraphQLFalseBooleanValue>.Instance,
			_ => this.ThrowASTConversionError(value)
		};
}
