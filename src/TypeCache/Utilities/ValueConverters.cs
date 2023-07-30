﻿// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Utilities;

internal static class ValueConverters
{
	public static object? ConvertObject(object value, Type targetType)
		=> value switch
		{
			null or DBNull when targetType == typeof(DBNull) => DBNull.Value,
			null or DBNull => null,
			_ when value.GetType().IsAssignableTo(targetType) => value,
			DateOnly dateOnly => ConvertDateOnly(dateOnly, targetType),
			DateTime dateTime => ConvertDateTime(dateTime, targetType),
			DateTimeOffset dateTimeOffset => ConvertDateTimeOffset(dateTimeOffset, targetType),
			Enum token => ConvertEnum(token, targetType),
			Int128 number => ConvertInt128(number, targetType),
			nint pointer => ConvertIntPtr(pointer, targetType),
			TimeOnly timeOnly => ConvertTimeOnly(timeOnly, targetType),
			TimeSpan timeSpan => ConvertTimeSpan(timeSpan, targetType),
			UInt128 number => ConvertUInt128(number, targetType),
			nuint pointer => ConvertUIntPtr(pointer, targetType),
			char character => ConvertChar(character, targetType),
			string text => ConvertString(text, targetType),
			_ when targetType == typeof(string) => value.ToString(),
			IConvertible convertible when targetType.IsAssignableTo<IConvertible>() => Convert.ChangeType(value, targetType, InvariantCulture),
			_ => value,
		};

	public static object? ConvertChar(char value, Type targetType)
		=> value switch
		{
			'0' or 'o' or 'O' or 'n' or 'N' or 'f' or 'F' when targetType == typeof(bool) => false,
			'1' or 'x' or 'X' or 'y' or 'Y' or 't' or 'T' when targetType == typeof(bool) => true,
			_ when targetType == typeof(string) => value.ToString(),
			_ when targetType == typeof(Int128) => (Int128)value,
			_ when targetType == typeof(UInt128) => (UInt128)value,
			_ when targetType.IsAssignableTo<IConvertible>() => Convert.ChangeType(value, targetType, InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertDateOnly(DateOnly value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(DateTime) => value.ToDateTime(TimeOnly.MinValue),
			_ when targetType == typeof(DateTimeOffset) => value.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset(),
			_ when targetType == typeof(TimeSpan) => TimeSpan.FromDays(value.DayNumber),
			_ when targetType == typeof(string) => value.ToISO8601(),
			_ when targetType == typeof(int) => value.DayNumber,
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertDateTime(DateTime value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(DateOnly) => value.ToDateOnly(),
			_ when targetType == typeof(DateTimeOffset) => value.ToDateTimeOffset(),
			_ when targetType == typeof(TimeOnly) => value.ToTimeOnly(),
			_ when targetType == typeof(string) => value.ToISO8601(),
			_ when targetType == typeof(long) => value.Ticks,
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertDateTimeOffset(DateTimeOffset value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(DateOnly) => value.ToDateOnly(),
			_ when targetType == typeof(DateTime) => value.DateTime,
			_ when targetType == typeof(TimeOnly) => value.ToTimeOnly(),
			_ when targetType == typeof(string) => value.ToISO8601(),
			_ when targetType == typeof(long) => value.Ticks,
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertEnum(Enum value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(string) => value.ToString(),
			_ when targetType.IsEnumUnderlyingType() => Convert.ChangeType(value, targetType, InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertInt128(Int128 value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(int) => (int)value,
			_ when targetType == typeof(uint) => (uint)value,
			_ when targetType == typeof(long) => (long)value,
			_ when targetType == typeof(ulong) => (ulong)value,
			_ when targetType == typeof(UInt128) => (UInt128)value,
			_ when targetType == typeof(string) => value.ToString(InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertIntPtr(IntPtr value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(nuint) => (nuint)value,
			_ when targetType == typeof(int) => value.ToInt32(),
			_ when targetType == typeof(long) => value.ToInt64(),
			_ when targetType == typeof(string) => value.ToString(InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertString(string value, Type targetType)
		=> value switch
		{
			"" => null,
			_ when targetType == typeof(char) => value[0],
			_ when targetType.IsEnum => Enum.Parse(targetType, value, true),
			_ when targetType == typeof(Guid) => Guid.Parse(value),
			_ when targetType == typeof(Uri) => new Uri(value),
			_ when targetType == typeof(DateOnly) => DateOnly.Parse(value, InvariantCulture),
			_ when targetType == typeof(DateTime) => DateTime.Parse(value, InvariantCulture),
			_ when targetType == typeof(DateTimeOffset) => DateTimeOffset.Parse(value, InvariantCulture),
			_ when targetType == typeof(Int128) => Int128.Parse(value, InvariantCulture),
			_ when targetType == typeof(UInt128) => UInt128.Parse(value, InvariantCulture),
			_ when targetType == typeof(nint) => nint.Parse(value, InvariantCulture),
			_ when targetType == typeof(nuint) => nint.Parse(value, InvariantCulture),
			_ when targetType == typeof(TimeOnly) => TimeOnly.Parse(value, InvariantCulture),
			_ when targetType == typeof(TimeSpan) => TimeSpan.Parse(value, InvariantCulture),
			_ when targetType.IsAssignableTo<IConvertible>() => Convert.ChangeType(value, targetType, InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertTimeOnly(TimeOnly value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(TimeSpan) => value.ToTimeSpan(),
			_ when targetType == typeof(string) => value.ToISO8601(),
			_ when targetType == typeof(long) => value.Ticks,
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertTimeSpan(TimeSpan value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(TimeOnly) => TimeOnly.FromTimeSpan(value),
			_ when targetType == typeof(string) => value.ToText(InvariantCulture),
			_ when targetType == typeof(long) => value.Ticks,
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertUInt128(UInt128 value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(int) => (int)value,
			_ when targetType == typeof(uint) => (uint)value,
			_ when targetType == typeof(long) => (long)value,
			_ when targetType == typeof(ulong) => (ulong)value,
			_ when targetType == typeof(Int128) => (Int128)value,
			_ when targetType == typeof(string) => value.ToString(InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	public static object? ConvertUIntPtr(UIntPtr value, Type targetType)
		=> targetType switch
		{
			_ when targetType == typeof(nint) => (nint)value,
			_ when targetType == typeof(uint) => value.ToUInt32(),
			_ when targetType == typeof(ulong) => value.ToUInt64(),
			_ when targetType == typeof(string) => value.ToString(InvariantCulture),
			_ => throw new InvalidCastException(GetInvalidCastMessage(value.GetType(), targetType))
		};

	private static string GetInvalidCastMessage(Type sourceType, Type targetType)
		=> Invariant($"Type [{sourceType.Name}] cannot be converted to type [{targetType.Name}].");
}