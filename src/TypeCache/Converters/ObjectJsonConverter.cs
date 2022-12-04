// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters;

public sealed class ObjectJsonConverter : JsonConverter<object?>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetValue();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
		=> JsonSerializer.Serialize(writer, value, options);
}
