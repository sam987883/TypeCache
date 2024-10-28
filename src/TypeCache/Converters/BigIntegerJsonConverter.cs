// Copyright (c) 2021 Samuel Abraham

using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class BigIntegerJsonConverter : JsonConverter<BigInteger>
{
	public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		reader.TokenType.ThrowIfNotEqual(JsonTokenType.Number);

		using var doc = JsonDocument.ParseValue(ref reader);
		return BigInteger.Parse(doc.RootElement.GetRawText(), NumberFormatInfo.InvariantInfo);
	}

	public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
	{
		var text = value.ToString(NumberFormatInfo.InvariantInfo);
		using var doc = JsonDocument.Parse(text);
		doc.WriteTo(writer);
	}
}
