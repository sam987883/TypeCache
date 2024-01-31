// Copyright (c) 2021 Samuel Abraham

using System;
using System.Globalization;
using System.Numerics;
using GraphQL.Types;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.FormattableString;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLScalarType<T> : ScalarGraphType
	where T : ISpanParsable<T>
{
	public GraphQLScalarType()
	{
		this.Name = Invariant($"{typeof(T).Namespace}_{typeof(T).Name}");
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
			IHasValueNode node => T.Parse(node.Value.Span, CultureInfo.InvariantCulture),
			_ => this.ThrowLiteralConversionError(value)
		};

	public override object? ParseValue(object? value)
		=> value switch
		{
			null or T => value,
			string text => T.Parse(text, CultureInfo.InvariantCulture),
			_ => this.ThrowValueConversionError(value)
		};

	public override GraphQLValue ToAST(object? value)
		=> this.ParseValue(value) switch
		{
			null => Singleton<GraphQLNullValue>.Instance,
			true => Singleton<GraphQLTrueBooleanValue>.Instance,
			false => Singleton<GraphQLFalseBooleanValue>.Instance,
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
			Half x => new GraphQLFloatValue((float)x),
			float x => new GraphQLFloatValue(x),
			double x => new GraphQLFloatValue(x),
			decimal x => new GraphQLFloatValue(x),
			DateOnly x => new GraphQLStringValue(x.ToISO8601()),
			DateTime x => new GraphQLStringValue(x.ToISO8601()),
			DateTimeOffset x => new GraphQLStringValue(x.ToISO8601()),
			TimeOnly x => new GraphQLStringValue(x.ToISO8601()),
			TimeSpan x => new GraphQLStringValue(x.ToText()),
			char x => new GraphQLStringValue(x.ToString()),
			Guid x => new GraphQLStringValue(x.ToText()),
			string x => new GraphQLStringValue(x),
			_ => this.ThrowASTConversionError(value)
		};
}
