// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLUriType : ScalarGraphType
{
	public GraphQLUriType()
	{
		this.Name = nameof(Uri);
		this.Description = Invariant($"GraphQL Type for: `{this.Name}`.");
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => true,
			GraphQLStringValue graphQLValue => Uri.TryCreate(graphQLValue.Value.Span.ToString(), graphQLValue.Value.Span[0] is '/' ? UriKind.Relative : UriKind.Absolute, out _),
			_ => false
		};

	public override bool CanParseValue(object? value)
		=> value switch
		{
			null or Uri => true,
			string text => Uri.TryCreate(text, text[0] is '/' ? UriKind.Relative : UriKind.Absolute, out _),
			_ => false
		};

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => null,
			GraphQLStringValue graphQLValue => new Uri(graphQLValue.Value.Span.ToString(), graphQLValue.Value.Span[0] is '/' ? UriKind.Relative : UriKind.Absolute),
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or Uri => value,
			string text => new Uri(text, text[0] is '/' ? UriKind.Relative : UriKind.Absolute),
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> this.ParseValue(value) switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			char c => new GraphQLStringValue(c.ToString()),
			string text => new GraphQLStringValue(text),
			Uri uri => new GraphQLStringValue(uri.ToString()),
			_ => this.ThrowASTConversionError(value)
		};
}
