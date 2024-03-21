// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using GraphQLParser.AST;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLStringType : ScalarGraphType
{
	public GraphQLStringType()
	{
		this.Name = "System_String";
		this.Description = "System.String";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => true,
			IHasValueNode node => true,
			_ => false
		};

	public override bool CanParseValue(object? value)
		=> true;

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => null,
			IHasValueNode node => new string(node.Value.Span),
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or string => value,
			_ => value.ToString()
		};

	public override GraphQLValue ToAST(object? value)
		=> value?.ToString() is not null ? new GraphQLStringValue(value.ToString()!) : new GraphQLNullValue();
}
