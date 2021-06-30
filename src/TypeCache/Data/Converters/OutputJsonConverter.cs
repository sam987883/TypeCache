// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data.Converters
{
	public class OutputJsonConverter : JsonConverter<IDictionary<string, string>>
	{
		public override IDictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var output = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
						output[name] = reader.GetString()!;
				}
			}

			return output;
		}

		public override void Write(Utf8JsonWriter writer, IDictionary<string, string> output, JsonSerializerOptions options)
		{
			if (output.Any())
			{
				writer.WriteStartObject();
				output.Do(_ => writer.WriteString(_.Key, _.Value));
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
