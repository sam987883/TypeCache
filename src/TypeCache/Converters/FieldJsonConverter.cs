// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Converters
{
	public class FieldJsonConverter<T> : JsonConverter<T?>
		where T : class, new()
	{
		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartObject)
			{
				var output = TypeOf<T>.Create();
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString()!;
					if (reader.Read())
					{
						var field = TypeOf<T>.Fields[name];
						var value = reader.TokenType switch
						{
							JsonTokenType.StartObject => JsonSerializer.Deserialize(ref reader, field.Type.Handle.ToType(), options),
							JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, field.Type.Handle.ToType(), options),
							JsonTokenType.String => reader.GetString(),
							JsonTokenType.Number => reader.TryGetInt64(out var number) ? number : reader.GetDecimal(),
							JsonTokenType.True => true,
							JsonTokenType.False => false,
							_ => null
						};
						if (field.SetValue != null)
							field.SetValue(output, value);
					}
				}

				return output;
			}

			return null;
		}

		public override void Write(Utf8JsonWriter writer, T? input, JsonSerializerOptions options)
		{
			if (input != null)
			{
				writer.WriteStartObject();
				TypeOf<T>.Fields.Values.If(field => field!.Getter != null).Do(field =>
				{
					writer.WritePropertyName(field!.Name);
					var value = field.GetValue!(input);
					if (value != null)
					{
						switch (field.Type.SystemType)
						{
							case SystemType.DBNull:
								writer.WriteNullValue();
								break;
							case SystemType.Boolean:
								writer.WriteBooleanValue((bool)value);
								break;
							case SystemType.SByte:
								writer.WriteNumberValue((sbyte)value);
								break;
							case SystemType.Byte:
								writer.WriteNumberValue((byte)value);
								break;
							case SystemType.Int16:
								writer.WriteNumberValue((short)value);
								break;
							case SystemType.UInt16:
								writer.WriteNumberValue((ushort)value);
								break;
							case SystemType.Int32:
								writer.WriteNumberValue((int)value);
								break;
							case SystemType.UInt32:
								writer.WriteNumberValue((uint)value);
								break;
							case SystemType.Int64:
								writer.WriteNumberValue((long)value);
								break;
							case SystemType.UInt64:
								writer.WriteNumberValue((ulong)value);
								break;
							case SystemType.Single:
								writer.WriteNumberValue((float)value);
								break;
							case SystemType.Double:
								writer.WriteNumberValue((double)value);
								break;
							case SystemType.Decimal:
								writer.WriteNumberValue((decimal)value);
								break;
							case SystemType.DateTime:
								writer.WriteStringValue((DateTime)value);
								break;
							case SystemType.DateTimeOffset:
								writer.WriteStringValue((DateTimeOffset)value);
								break;
							case SystemType.TimeSpan:
								writer.WriteStringValue(((TimeSpan)value).ToString("c"));
								break;
							case SystemType.Guid:
								writer.WriteStringValue((Guid)value);
								break;
							case SystemType.Char:
							case SystemType.Index:
							case SystemType.Range:
							case SystemType.Uri:
								writer.WriteStringValue(value.ToString());
								break;
							case SystemType.String:
								writer.WriteStringValue((string)value);
								break;
							default:
								JsonSerializer.Serialize(writer, value, options);
								break;
						}
					}
					else
						writer.WriteNullValue();
				});
				writer.WriteEndObject();
			}
			else
				writer.WriteNullValue();
		}
	}
}
