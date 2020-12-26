// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeCache.Common;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Converters
{
	public class PropertyJsonConverter<T> : JsonConverter<T> where T : class, new()
	{
		private readonly ITypeCache _TypeCache;

		public PropertyJsonConverter(ITypeCache typeCache)
		{
			this._TypeCache = typeCache;
		}

		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> this.ReadObject(ref reader, typeToConvert, options) as T;

		private object[] ReadArray(ref Utf8JsonReader reader, Type elementType, JsonSerializerOptions options)
		{
			var values = new List<object?>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				values.Add(JsonSerializer.Deserialize(ref reader, elementType, options));
			}
			return values.ToArray();
		}

		private object? ReadObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var propertyCache = this._TypeCache.GetPropertyCache(typeToConvert);
			var instance = this._TypeCache.Create(typeToConvert);
			while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
			{
				var name = reader.GetString();
				if (reader.Read())
				{
					var property = propertyCache.Properties[name];
					var type = property.TypeHandle.ToType();
					var value = JsonSerializer.Deserialize(ref reader, type, options);
					property[instance] = value;
				}
			}
			return instance;
		}

		private object? ReadValue(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> reader.TokenType switch
			{
				JsonTokenType.StartObject => ReadObject(ref reader, typeToConvert, options),
				JsonTokenType.StartArray => this.ReadArray(ref reader, typeToConvert.GetElementType(), options),
				JsonTokenType.String => reader.GetString(),
				JsonTokenType.Number => reader.TryGetInt64(out var number) ? number : reader.GetDecimal(),
				JsonTokenType.True => true,
				JsonTokenType.False => false,
				JsonTokenType.Null => null,
				_ => null,
			};

		public override void Write(Utf8JsonWriter writer, T? input, JsonSerializerOptions options)
		{
			if (input != null)
			{
				writer.WriteStartObject();
				var propertyCache = this._TypeCache.GetPropertyCache<T>();
				var accessor = this._TypeCache.CreatePropertyAccessor(input);
				propertyCache.Properties.Values.If(_ => _.GetMethod != null).Do(property =>
				{
					writer.WritePropertyName(property.Name);
					var value = propertyCache.Properties[property.Name][input];
					if (value != null)
					{
						switch (property.NativeType)
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
