// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static partial class EnumExtensions
{
	public static bool IsConvertibleTo(this ScalarType @this, ScalarType target)
		=> (@this, target) switch
		{
			_ when target == @this => true,
			(ScalarType.String, _) => true,
			(_, ScalarType.Boolean or ScalarType.String) => true,
			(ScalarType.Boolean, ScalarType.Char) => true,
			(ScalarType.Boolean, ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.Boolean, ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Byte, ScalarType.Char) => true,
			(ScalarType.Byte, ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.Byte, ScalarType.BigInteger) => true,
			(ScalarType.Byte, ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Byte, ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.Char, ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.Char, ScalarType.BigInteger) => true,
			(ScalarType.Char, ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Char, ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.DateOnly, ScalarType.DateTime or ScalarType.DateTimeOffset) => true,
			(ScalarType.DateOnly, ScalarType.TimeSpan) => true,
			(ScalarType.DateOnly, ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.DateOnly, ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.DateTime, ScalarType.DateOnly or ScalarType.DateTimeOffset) => true,
			(ScalarType.DateTime, ScalarType.TimeOnly or ScalarType.TimeSpan) => true,
			(ScalarType.DateTime, ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.DateTime, ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.DateTimeOffset, ScalarType.DateOnly or ScalarType.DateTime) => true,
			(ScalarType.DateTimeOffset, ScalarType.TimeOnly or ScalarType.TimeSpan) => true,
			(ScalarType.DateTimeOffset, ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.DateTimeOffset, ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Int16, ScalarType.Char) => true,
			(ScalarType.Int16, ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.Int16, ScalarType.BigInteger) => true,
			(ScalarType.Int16, ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Int16, ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.Int32, ScalarType.DateOnly) => true,
			(ScalarType.Int32, ScalarType.Index) => true,
			(ScalarType.Int32, ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.Int32, ScalarType.BigInteger) => true,
			(ScalarType.Int32, ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Int32, ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.Int64, ScalarType.DateTime or ScalarType.DateTimeOffset) => true,
			(ScalarType.Int64, ScalarType.TimeOnly or ScalarType.TimeSpan) => true,
			(ScalarType.Int64, ScalarType.Int128) => true,
			(ScalarType.Int64, ScalarType.BigInteger) => true,
			(ScalarType.Int64, ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Int64, ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.Int128, ScalarType.BigInteger) => true,
			(ScalarType.Int128, ScalarType.UInt128) => true,
			(ScalarType.Int128, ScalarType.Decimal) => true,
			(ScalarType.IntPtr, ScalarType.Int32 or ScalarType.Int64) => true,
			(ScalarType.SByte, ScalarType.Char) => true,
			(ScalarType.SByte, ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.SByte, ScalarType.BigInteger) => true,
			(ScalarType.SByte, ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.SByte, ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.TimeOnly, ScalarType.TimeSpan or ScalarType.Int64 or ScalarType.UInt64) => true,
			(ScalarType.TimeSpan, ScalarType.TimeOnly or ScalarType.Int64 or ScalarType.UInt64) => true,
			(ScalarType.UInt16, ScalarType.Char) => true,
			(ScalarType.UInt16, ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.UInt16, ScalarType.BigInteger) => true,
			(ScalarType.UInt16, ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.UInt16, ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.UInt32, ScalarType.DateOnly) => true,
			(ScalarType.UInt32, ScalarType.Index) => true,
			(ScalarType.UInt32, ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.UInt32, ScalarType.BigInteger) => true,
			(ScalarType.UInt32, ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.UInt32, ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.UInt64, ScalarType.DateTime or ScalarType.DateTimeOffset) => true,
			(ScalarType.UInt64, ScalarType.TimeOnly or ScalarType.TimeSpan) => true,
			(ScalarType.UInt64, ScalarType.Int64 or ScalarType.Int128) => true,
			(ScalarType.UInt64, ScalarType.BigInteger) => true,
			(ScalarType.UInt64, ScalarType.UInt128) => true,
			(ScalarType.UInt64, ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.UInt128, ScalarType.Int128 or ScalarType.BigInteger or ScalarType.Decimal) => true,
			(ScalarType.UIntPtr, ScalarType.UInt32 or ScalarType.UInt64) => true,
			_ => false
		};

	public static bool IsEnumUnderlyingType(this ScalarType @this)
		=> @this switch
		{
			ScalarType.SByte
			or ScalarType.Int16
			or ScalarType.Int32
			or ScalarType.Int64
			or ScalarType.Byte
			or ScalarType.UInt16
			or ScalarType.UInt32
			or ScalarType.UInt64 => true,
			_ => false
		};

	public static bool IsPointer(this ScalarType @this)
		=> @this switch
		{
			ScalarType.IntPtr
			or ScalarType.UIntPtr => true,
			_ => false
		};

	/// <summary>
	/// Returns true for the current .Net primitives.
	/// </summary>
	public static bool IsPrimitive(this ScalarType @this)
		=> @this switch
		{
			ScalarType.Boolean
			or ScalarType.SByte
			or ScalarType.Int16
			or ScalarType.Int32
			or ScalarType.Int64
			or ScalarType.Byte
			or ScalarType.UInt16
			or ScalarType.UInt32
			or ScalarType.UInt64
			or ScalarType.IntPtr
			or ScalarType.UIntPtr
			or ScalarType.Single
			or ScalarType.Double
			or ScalarType.Char => true,
			_ => false
		};

	public static Type GetScalarType(this ScalarType @this)
		=> @this switch
		{
			ScalarType.None => typeof(void),
			ScalarType.Boolean => typeof(bool),
			ScalarType.Char => typeof(char),
			ScalarType.String => typeof(string),
			ScalarType.Guid => typeof(Guid),
			ScalarType.Uri => typeof(Uri),
			ScalarType.Enum => typeof(Enum),
			ScalarType.Index => typeof(Index),
			ScalarType.SByte => typeof(sbyte),
			ScalarType.Int16 => typeof(short),
			ScalarType.Int32 => typeof(int),
			ScalarType.Int64 => typeof(long),
			ScalarType.Int128 => typeof(Int128),
			ScalarType.BigInteger => typeof(BigInteger),
			ScalarType.Byte => typeof(byte),
			ScalarType.UInt16 => typeof(ushort),
			ScalarType.UInt32 => typeof(uint),
			ScalarType.UInt64 => typeof(ulong),
			ScalarType.UInt128 => typeof(UInt128),
			ScalarType.IntPtr => typeof(nint),
			ScalarType.UIntPtr => typeof(nuint),
			ScalarType.Half => typeof(Half),
			ScalarType.Single => typeof(float),
			ScalarType.Double => typeof(double),
			ScalarType.Decimal => typeof(decimal),
			ScalarType.DateOnly => typeof(DateOnly),
			ScalarType.DateTime => typeof(DateTime),
			ScalarType.DateTimeOffset => typeof(DateTimeOffset),
			ScalarType.TimeOnly => typeof(TimeOnly),
			ScalarType.TimeSpan => typeof(TimeSpan),
			_ => throw new UnreachableException()
		};
}
