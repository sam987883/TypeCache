// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using System.Numerics;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLNumberType : ScalarGraphType, INumberGraphType
{
	public GraphQLNumberType()
	{
		this.Name = "Number";
		this.Description = "Any decimal or integer number.";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => true,
			IHasValueNode node => decimal.TryParse(node.Value.Span, CultureInfo.InvariantCulture, out var _),
			_ => false
		};

	public override bool CanParseValue(object? value)
		=> value switch
		{
			null
			or sbyte or short or char or int or long or Int128 or BigInteger
			or byte or ushort or uint or ulong or UInt128
			or nint or nuint
			or Half or float or double or decimal => true,
			string text => decimal.TryParse(text, CultureInfo.InvariantCulture, out _),
			_ => false
		};

	public override object? ParseLiteral(GraphQLValue value)
		=> value switch
		{
			GraphQLNullValue => null,
			IHasValueNode node when decimal.TryParse(node.Value.Span, CultureInfo.InvariantCulture, out var parsedValue) => parsedValue,
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null
			or sbyte or short or char or int or long or Int128 or BigInteger
			or byte or ushort or uint or ulong or UInt128
			or nint or nuint
			or Half or float or double or decimal => value,
			string text when decimal.TryParse(text, CultureInfo.InvariantCulture, out var parsedValue) => parsedValue,
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> this.ParseValue(value) switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			sbyte x => new GraphQLIntValue(x),
			short x => new GraphQLIntValue(x),
			char x => new GraphQLIntValue(x),
			int x => new GraphQLIntValue(x),
			long x => new GraphQLIntValue(x),
			Int128 x => new GraphQLIntValue(x),
			BigInteger x => new GraphQLIntValue(x),
			byte x => new GraphQLIntValue(x),
			ushort x => new GraphQLIntValue(x),
			uint x => new GraphQLIntValue(x),
			ulong x => new GraphQLIntValue(x),
			UInt128 x => new GraphQLIntValue(x),
			nint x => new GraphQLIntValue(x),
			nuint x => new GraphQLIntValue(x),
			Half x => new GraphQLFloatValue((float)x),
			float x => new GraphQLFloatValue(x),
			double x => new GraphQLFloatValue(x),
			decimal x => new GraphQLFloatValue(x),
			_ => this.ThrowASTConversionError(value)
		};
}

public sealed class GraphQLNumberType<T> : ScalarGraphType, INumberGraphType
	where T : IBinaryInteger<T>, ISpanParsable<T>
{
	public GraphQLNumberType()
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
			sbyte x => new GraphQLIntValue(x),
			short x => new GraphQLIntValue(x),
			char x => new GraphQLIntValue(x),
			int x => new GraphQLIntValue(x),
			long x => new GraphQLIntValue(x),
			Int128 x => new GraphQLIntValue(x),
			BigInteger x => new GraphQLIntValue(x),
			byte x => new GraphQLIntValue(x),
			ushort x => new GraphQLIntValue(x),
			uint x => new GraphQLIntValue(x),
			ulong x => new GraphQLIntValue(x),
			UInt128 x => new GraphQLIntValue(x),
			nint x => new GraphQLIntValue(x),
			nuint x => new GraphQLIntValue(x),
			_ => this.ThrowASTConversionError(value)
		};
}
