// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Extensions;
using TypeCache.Reflection;

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
						field[output] = reader.TokenType switch
						{
							JsonTokenType.StartObject => JsonSerializer.Deserialize(ref reader, field.TypeHandle.ToType(), options),
							JsonTokenType.StartArray => JsonSerializer.Deserialize(ref reader, field.TypeHandle.ToType(), options),
							JsonTokenType.String => reader.GetString(),
							JsonTokenType.Number => reader.TryGetInt64(out var value) ? value : reader.GetDecimal(),
							JsonTokenType.True => true,
							JsonTokenType.False => false,
							_ => null
						};
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
					var value = field[input];
					if (value != null)
					{
						switch (field.NativeType)
						{
							case NativeType.DBNull:
								writer.WriteNullValue();
								break;
							case NativeType.Boolean:
								writer.WriteBooleanValue((bool)value);
								break;
							case NativeType.SByte:
								writer.WriteNumberValue((sbyte)value);
								break;
							case NativeType.Byte:
								writer.WriteNumberValue((byte)value);
								break;
							case NativeType.Int16:
								writer.WriteNumberValue((short)value);
								break;
							case NativeType.UInt16:
								writer.WriteNumberValue((ushort)value);
								break;
							case NativeType.Int32:
								writer.WriteNumberValue((int)value);
								break;
							case NativeType.UInt32:
								writer.WriteNumberValue((uint)value);
								break;
							case NativeType.Int64:
								writer.WriteNumberValue((long)value);
								break;
							case NativeType.UInt64:
								writer.WriteNumberValue((ulong)value);
								break;
							case NativeType.Single:
								writer.WriteNumberValue((float)value);
								break;
							case NativeType.Double:
								writer.WriteNumberValue((double)value);
								break;
							case NativeType.Decimal:
								writer.WriteNumberValue((decimal)value);
								break;
							case NativeType.DateTime:
								writer.WriteStringValue((DateTime)value);
								break;
							case NativeType.DateTimeOffset:
								writer.WriteStringValue((DateTimeOffset)value);
								break;
							case NativeType.TimeSpan:
								writer.WriteStringValue(((TimeSpan)value).ToString("c"));
								break;
							case NativeType.Guid:
								writer.WriteStringValue((Guid)value);
								break;
							case NativeType.Char:
							case NativeType.Index:
							case NativeType.Range:
							case NativeType.Uri:
								writer.WriteStringValue(value.ToString());
								break;
							case NativeType.String:
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
