// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;

namespace TypeCache.Converters
{
	public class ValueJsonConverter : JsonConverter<object?>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> reader.GetValue();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
			=> JsonSerializer.Serialize(writer, value, null);

		private static IDictionary<string, object> GetObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			var dictionary = new Dictionary<string, object>(Default.STRING_COMPARISON.ToStringComparer());
			while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				var name = reader.GetString();
				if (reader.Read())
				{
					var value = JsonSerializer.Deserialize<object>(ref reader, options);
					dictionary.Add(name!, value!);
				}
			}
			return dictionary;
		}

		private static object[] GetArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			var list = new List<object>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				var value = JsonSerializer.Deserialize<object>(ref reader, options);
				list.Add(value!);
			}
			return list.ToArray();
		}
	}
}
