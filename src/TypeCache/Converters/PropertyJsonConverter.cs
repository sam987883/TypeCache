// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Converters
{
	public class PropertyJsonConverter<T> : JsonConverter<T> where T : class, new()
	{
		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartObject)
			{
				var output = TypeOf<T>.Create();
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString();
					if (reader.Read())
					{
						var property = TypeOf<T>.Properties[name!];
						if (property.SetValue is not null)
							property.SetValue(output, reader.TokenType switch
							{
								JsonTokenType.StartObject => JsonSerializer.Deserialize(ref reader, property.Type, options),
								JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, property.Type, options),
								_ => reader.GetValue()
							});
					}
				}

				return output;
			}

			return null;
		}

		public override void Write(Utf8JsonWriter writer, T? input, JsonSerializerOptions options)
		{
			if (input is not null)
			{
				writer.WriteStartObject();
				TypeOf<T>.Properties.Values.If(property => property!.GetValue is not null).Do(property =>
				{
					writer.WritePropertyName(property!.Name);
					var value = property.GetValue!(input);
					writer.WriteValue(property.Type.SystemType, value, options);
				});
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
