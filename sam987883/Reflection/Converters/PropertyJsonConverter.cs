// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using sam987883.Reflection.Accessors;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace sam987883.Reflection.Converters
{
	public class PropertyJsonConverter<T> : JsonConverter<T?> where T : class
	{
		private readonly IConstructorCache<T> _ConstructorCache;
		private readonly IPropertyAccessorFactory<T> _PropertyAccessorFactory;
		private readonly IPropertyCache<T> _PropertyCache;

		public PropertyJsonConverter(IConstructorCache<T> constructorCache, IPropertyAccessorFactory<T> propertyAccessorFactory, IPropertyCache<T> propertyCache)
		{
			this._ConstructorCache = constructorCache;
			this._PropertyAccessorFactory = propertyAccessorFactory;
			this._PropertyCache = propertyCache;
		}

		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			T? output = null;
			if (reader.TokenType == JsonTokenType.StartObject)
			{
				output = this._ConstructorCache.Create();
				var accessor = this._PropertyAccessorFactory.Create(output);
				while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
				{
					var name = reader.GetString();
					if (reader.Read())
					{
						switch (reader.TokenType)
						{
							case JsonTokenType.StartObject:
								{
									var type = Type.GetTypeFromHandle(this._PropertyCache.Properties[name].TypeHandle);
									accessor.Values[name] = JsonSerializer.Deserialize(ref reader, type, options);
								}
								break;
							case JsonTokenType.StartArray:
								while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
								{
									var type = Type.GetTypeFromHandle(this._PropertyCache.Properties[name].TypeHandle);
									accessor.Values[name] = JsonSerializer.Deserialize(ref reader, type, options);
								}
								break;
							case JsonTokenType.String:
								accessor.Values[name] = reader.GetString();
								break;
							case JsonTokenType.Number:
								accessor.Values[name] = reader.TryGetInt64(out var value) ? value : reader.GetDecimal();
								break;
							case JsonTokenType.True:
								accessor.Values[name] = true;
								break;
							case JsonTokenType.False:
								accessor.Values[name] = false;
								break;
							case JsonTokenType.Null:
								accessor.Values[name] = null;
								break;
						}
					}
				}
			}

			return output;
		}

		public override void Write(Utf8JsonWriter writer, T? instance, JsonSerializerOptions options)
		{
			if (instance != null)
			{
				writer.WriteStartObject();
				var accessor = this._PropertyAccessorFactory.Create(instance);
				this._PropertyCache.Properties.Values.If(_ => _.GetMethod != null).Do(property =>
				{
					writer.WritePropertyName(property.Name);
					var value = accessor[property.Name];
					if (value != null)
					{
						switch (property.Type)
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
