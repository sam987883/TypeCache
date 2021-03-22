// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class ObjectJsonConverter : JsonConverter<object?>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> reader.GetValue();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
			=> JsonSerializer.Serialize(writer, value, null);
	}
}
