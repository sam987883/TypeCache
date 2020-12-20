﻿// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace TypeCache.Converters
{
	public class ObjectJsonConverter : JsonConverter<object?>
	{
		public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType switch
		{
			JsonTokenType.StartObject => JsonSerializer.Deserialize(ref reader, typeToConvert, options),
			JsonTokenType.StartArray => JsonSerializer.Deserialize<object[]>(ref reader, options),
			JsonTokenType.String => reader.GetString(),
			JsonTokenType.Number => reader.TryGetInt64(out var value) ? value : reader.GetDecimal(),
			JsonTokenType.True => true,
			JsonTokenType.False => false,
			_ => null,
		};

		public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
			=> JsonSerializer.Serialize(writer, value, null);
	}
}