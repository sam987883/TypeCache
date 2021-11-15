// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Converters
{
	public class ObjectJsonConverter : JsonConverter<object?>
	{
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> reader.GetValue();

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
			=> JsonSerializer.Serialize(writer, value, options);
	}
}
