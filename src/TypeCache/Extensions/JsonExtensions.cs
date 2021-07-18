// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Extensions
{
	public static class JsonExtensions
	{
		private static IEnumerable<JsonElement> EnumerateArrayValues(this JsonElement @this)
		{
			using var enumerator = @this.EnumerateArray();
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}

		public static JsonElement[] GetArrayElements(this JsonElement @this)
		{
			@this.ValueKind.Assert(nameof(GetArrayValues), JsonValueKind.Array);

			return @this.EnumerateArrayValues().ToArray();
		}

		public static object?[]? GetArrayValues(this JsonElement @this)
		{
			@this.ValueKind.Assert(nameof(GetArrayValues), JsonValueKind.Array);

			return @this.EnumerateArrayValues().To(jsonElement => jsonElement.GetValue()).ToArray();
		}

		public static IDictionary<string, JsonElement> GetObjectElements(this JsonElement @this)
		{
			@this.ValueKind.Assert(nameof(GetObjectValues), JsonValueKind.Object);

			var properties = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
			using var enumerator = @this.EnumerateObject();
			while (enumerator.MoveNext())
			{
				properties.Add(enumerator.Current.Name, enumerator.Current.Value);
			}
			return properties;
		}

		public static IDictionary<string, object?> GetObjectValues(this JsonElement @this)
		{
			@this.ValueKind.Assert(nameof(GetObjectValues), JsonValueKind.Object);

			var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
			using var enumerator = @this.EnumerateObject();
			while (enumerator.MoveNext())
			{
				properties.Add(enumerator.Current.Name, enumerator.Current.Value.GetValue());
			}
			return properties;
		}

		public static object? GetValue(this JsonElement @this)
			=> @this.ValueKind switch
			{
				JsonValueKind.Undefined => throw new NotImplementedException(),
				JsonValueKind.True => true,
				JsonValueKind.False => false,
				JsonValueKind.Number when @this.TryGetInt32(out var value) => value,
				JsonValueKind.Number when @this.TryGetInt64(out var value) => value,
				JsonValueKind.Number => @this.GetDecimal(),
				JsonValueKind.String when @this.TryGetDateTime(out var value) => value,
				JsonValueKind.String when @this.TryGetGuid(out var value) => value,
				JsonValueKind.String => @this.GetString()!,
				JsonValueKind.Array => @this.GetArrayValues(),
				JsonValueKind.Object => @this.GetObjectValues(),
				_ => null
			};

		public static object? GetValue(this ref Utf8JsonReader @this)
			=> @this.TokenType switch
			{
				JsonTokenType.True => true,
				JsonTokenType.False => false,
				JsonTokenType.Number when @this.TryGetInt32(out var value) => value,
				JsonTokenType.Number when @this.TryGetInt64(out var value) => value,
				JsonTokenType.Number => @this.GetDecimal(),
				JsonTokenType.String when @this.TryGetDateTime(out var value) => value,
				JsonTokenType.String when @this.TryGetGuid(out var value) => value,
				JsonTokenType.String => @this.GetString(),
				_ => null
			};

		public static void ReadToEnd(this ref Utf8JsonReader @this)
		{
			while (@this.Read()) { }
		}

		public static bool ReadUntil(this ref Utf8JsonReader @this, JsonTokenType tokenType)
		{
			while (@this.Read())
				if (@this.TokenType == tokenType)
					return true;
			return false;
		}

		public static void WriteValue(this Utf8JsonWriter @this, object? value, JsonSerializerOptions options)
		{
			if (value is not null)
			{
				switch (value.GetTypeMember().SystemType)
				{
					case SystemType.DBNull:
						@this.WriteNullValue();
						break;
					case SystemType.Boolean:
						@this.WriteBooleanValue((bool)value);
						break;
					case SystemType.SByte:
						@this.WriteNumberValue((sbyte)value);
						break;
					case SystemType.Byte:
						@this.WriteNumberValue((byte)value);
						break;
					case SystemType.Int16:
						@this.WriteNumberValue((short)value);
						break;
					case SystemType.UInt16:
						@this.WriteNumberValue((ushort)value);
						break;
					case SystemType.Int32:
						@this.WriteNumberValue((int)value);
						break;
					case SystemType.UInt32:
						@this.WriteNumberValue((uint)value);
						break;
					case SystemType.NInt:
					case SystemType.Int64:
						@this.WriteNumberValue((long)value);
						break;
					case SystemType.NUInt:
					case SystemType.UInt64:
						@this.WriteNumberValue((ulong)value);
						break;
					case SystemType.Single:
						@this.WriteNumberValue((float)value);
						break;
					case SystemType.Double:
						@this.WriteNumberValue((double)value);
						break;
					case SystemType.Decimal:
						@this.WriteNumberValue((decimal)value);
						break;
					case SystemType.DateTime:
						@this.WriteStringValue((DateTime)value);
						break;
					case SystemType.DateTimeOffset:
						@this.WriteStringValue((DateTimeOffset)value);
						break;
					case SystemType.TimeSpan:
						@this.WriteStringValue(((TimeSpan)value).ToString("c"));
						break;
					case SystemType.Guid:
						@this.WriteStringValue((Guid)value);
						break;
					case SystemType.Char:
					case SystemType.Index:
					case SystemType.Range:
					case SystemType.Uri:
						@this.WriteStringValue(value.ToString());
						break;
					case SystemType.String:
						@this.WriteStringValue((string)value);
						break;
					default:
						JsonSerializer.Serialize(@this, value, options);
						break;
				}
			}
			else
				@this.WriteNullValue();
		}
	}
}
