// Copyright (c) 2020 Samuel Abraham

using System;
using System.Text.Json;

namespace sam987883.Reflection.Extensions
{
	public static class TypeExtensions
	{
		public static NativeType ToNativeType(this Type @this)
		{
			if (@this.IsEnum)
				return NativeType.Enum;
			else if (@this == typeof(bool))
				return NativeType.Boolean;
			else if (@this == typeof(sbyte))
				return NativeType.SByte;
			else if (@this == typeof(byte))
				return NativeType.Byte;
			else if (@this == typeof(short))
				return NativeType.Int16;
			else if (@this == typeof(ushort))
				return NativeType.UInt16;
			else if (@this == typeof(int))
				return NativeType.Int32;
			else if (@this == typeof(uint))
				return NativeType.UInt32;
			else if (@this == typeof(long))
				return NativeType.Int64;
			else if (@this == typeof(ulong))
				return NativeType.UInt64;
			else if (@this == typeof(float))
				return NativeType.Single;
			else if (@this == typeof(double))
				return NativeType.Double;
			else if (@this == typeof(decimal))
				return NativeType.Decimal;
			else if (@this == typeof(char))
				return NativeType.Char;
			else if (@this == typeof(DateTime))
				return NativeType.DateTime;
			else if (@this == typeof(DateTimeOffset))
				return NativeType.DateTimeOffset;
			else if (@this == typeof(TimeSpan))
				return NativeType.TimeSpan;
			else if (@this == typeof(Guid))
				return NativeType.Guid;
			else if (@this == typeof(Index))
				return NativeType.Index;
			else if (@this == typeof(Range))
				return NativeType.Range;
			else if (@this == typeof(JsonElement))
				return NativeType.JsonElement;
			else if (@this.IsValueType)
				return NativeType.ValueType;
			else if (@this == typeof(string))
				return NativeType.String;
			else if (@this == typeof(DBNull))
				return NativeType.DBNull;
			else if (@this == typeof(Uri))
				return NativeType.Uri;
			else
				return NativeType.Object;
		}
	}
}
