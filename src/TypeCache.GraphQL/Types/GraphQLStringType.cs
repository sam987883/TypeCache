// Copyright (c) 2021 Samuel Abraham

using System;
using System.Globalization;
using GraphQLParser.AST;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

public sealed class GraphQLStringType : GraphQLScalarType<GraphQLStringValue>
{
	public GraphQLStringType() : base(
		nameof(String),
		value => true,
		value => value.Value.Span.ToString(),
		value => value?.ToString())
	{
	}
}

public sealed class GraphQLStringType<T> : GraphQLScalarType<GraphQLStringValue>
	where T : ISpanParsable<T>
{
	public GraphQLStringType() : base(
		typeof(T).Name,
		value => T.TryParse(value.Value.Span, CultureInfo.InvariantCulture, out _),
		value => T.Parse(value.Value.Span, CultureInfo.InvariantCulture),
		value => typeof(T).GetScalarType() switch
		{
			ScalarType.Char => ValueConverter.ConvertToChar(value),
			ScalarType.DateOnly => ValueConverter.ConvertToDateOnly(value),
			ScalarType.DateTime => ValueConverter.ConvertToDateTime(value),
			ScalarType.DateTimeOffset => ValueConverter.ConvertToDateTimeOffset(value),
			ScalarType.Guid => ValueConverter.ConvertToGuid(value),
			ScalarType.TimeOnly => ValueConverter.ConvertToTimeOnly(value),
			ScalarType.TimeSpan => ValueConverter.ConvertToTimeSpan(value),
			ScalarType.Uri => ValueConverter.ConvertToUri(value),
			_ => null
		})
	{
	}
}
