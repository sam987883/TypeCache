// Copyright (c) 2021 Samuel Abraham

using System;
using System.Numerics;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public abstract class GraphQLScalarType<T> : ScalarGraphType
	where T : GraphQLValue
{
	private static readonly GraphQLFalseBooleanValue _GraphQLFalseBooleanValue = new GraphQLFalseBooleanValue();
	private static readonly GraphQLNullValue _GraphQLNullValue = new GraphQLNullValue();
	private static readonly GraphQLTrueBooleanValue _GraphQLTrueBooleanValue = new GraphQLTrueBooleanValue();

	private readonly Func<GraphQLValue, bool> _CanParseLiteral;
	private readonly Func<GraphQLValue, object?> _ParseLiteral;
	private readonly Func<object?, object?> _ParseValue;

	public GraphQLScalarType(string typeName, Func<T, bool> canParseLiteral, Func<T, object?> parseLiteral, Func<object?, object?> parseValue)
	{
		canParseLiteral.AssertNotNull();
		parseLiteral.AssertNotNull();
		parseValue.AssertNotNull();

		this.Name = Invariant($"GraphQL{typeName}Type");
		this._CanParseLiteral = value => value switch
		{
			GraphQLNullValue => true,
			T graphQLValue => canParseLiteral(graphQLValue),
			_ => false
		};
		this._ParseLiteral = value => value switch
		{
			GraphQLNullValue => null,
			T graphQLValue => parseLiteral(graphQLValue),
			_ => this.ThrowLiteralConversionError(value)
		};
		this._ParseValue = parseValue;
	}

	public override bool CanParseLiteral(GraphQLValue value) => this._CanParseLiteral(value);

	public override object? ParseLiteral(GraphQLValue value) => this._ParseLiteral(value);

	public override object? ParseValue(object? value) => this._ParseValue(value);

	public override GraphQLValue ToAST(object? value) => this._ParseValue(value) switch
	{
		null => _GraphQLNullValue,
		true => _GraphQLTrueBooleanValue,
		false => _GraphQLFalseBooleanValue,
		sbyte x => new GraphQLIntValue(x),
		short x => new GraphQLIntValue(x),
		int x => new GraphQLIntValue(x),
		long x => new GraphQLIntValue(x),
		Int128 x => new GraphQLIntValue(x),
		BigInteger x => new GraphQLIntValue(x),
		byte x => new GraphQLIntValue(x),
		ushort x => new GraphQLIntValue(x),
		uint x => new GraphQLIntValue(x),
		ulong x => new GraphQLIntValue(x),
		UInt128 x => new GraphQLIntValue(x),
		IntPtr x => new GraphQLIntValue(x),
		UIntPtr x => new GraphQLIntValue(x),
		Half x => new GraphQLFloatValue((decimal)x),
		float x => new GraphQLFloatValue(x),
		double x => new GraphQLFloatValue(x),
		decimal x => new GraphQLFloatValue(x),
		DateOnly x => new GraphQLStringValue(x.ToISO8601()),
		DateTime x => new GraphQLStringValue(x.ToISO8601()),
		DateTimeOffset x => new GraphQLStringValue(x.ToISO8601()),
		TimeOnly x => new GraphQLStringValue(x.ToISO8601()),
		TimeSpan x => new GraphQLStringValue(x.ToText()),
		Enum x => new GraphQLEnumValue(new GraphQLName(x.ToString("F"))),
		char x => new GraphQLStringValue(x.ToString()),
		string x => new GraphQLStringValue(x),
		_ => ThrowASTConversionError(value)
	};
}
