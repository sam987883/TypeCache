// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class ValueConverter
{

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	public static Expression CreateConversionExpression(this Expression @this, Type targetType)
	{
		@this.AssertNotNull();
		targetType.AssertNotNull();

		var isSourceNullable = @this.Type.IsNullable();
		var value = isSourceNullable ? @this.Property(nameof(Nullable<int>.Value)) : @this;
		var expression = (@this.Type.GetDataType(), targetType.GetDataType()) switch
		{
			(ScalarType.DBNull, _) when targetType.IsNullable() => Expression.Constant(null),
			(_, ScalarType.DBNull) when isSourceNullable => DBNull.Value.ToConstantExpression(),

			(ScalarType.Boolean, ScalarType.Char) => value.IIf('1'.ToConstantExpression(), '0'.ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.SByte) => value.IIf(((sbyte)1).ToConstantExpression(), ((sbyte)0).ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.Int16) => value.IIf(((short)1).ToConstantExpression(), ((short)0).ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.Int32) => value.IIf(1.ToConstantExpression(), 0.ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.Int64) => value.IIf(1L.ToConstantExpression(), 0L.ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.Byte) => value.IIf(((byte)1).ToConstantExpression(), ((byte)0).ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.UInt16) => value.IIf(((ushort)1).ToConstantExpression(), ((ushort)0).ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.UInt32) => value.IIf(1U.ToConstantExpression(), 0U.ToConstantExpression()),
			(ScalarType.Boolean, ScalarType.UInt64) => value.IIf(1UL.ToConstantExpression(), 0UL.ToConstantExpression()),

			(ScalarType.Char, ScalarType.Boolean) => ((LambdaExpression)((char value) =>
				value == '1' || value == 'x' || value == 'X' || value == 'y' || value == 'Y' || value == 't' || value == 'T')).Invoke(value),
			(ScalarType.SByte, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, ((sbyte)0).ToConstantExpression()),
			(ScalarType.Int16, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, ((short)0).ToConstantExpression()),
			(ScalarType.Int32, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, 0.ToConstantExpression()),
			(ScalarType.Int64, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, 0L.ToConstantExpression()),
			(ScalarType.Int128, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, Int128.Zero.ToConstantExpression()),
			(ScalarType.BigInteger, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, BigInteger.Zero.ToConstantExpression()),
			(ScalarType.Byte, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, ((byte)0).ToConstantExpression()),
			(ScalarType.UInt16, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, ((ushort)0).ToConstantExpression()),
			(ScalarType.UInt32, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, 0U.ToConstantExpression()),
			(ScalarType.UInt64, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, 0UL.ToConstantExpression()),
			(ScalarType.UInt128, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, UInt128.Zero.ToConstantExpression()),
			(ScalarType.IntPtr, ScalarType.Boolean) => value.Operation(BinaryOperator.NotEqualTo, nint.Zero.ToConstantExpression()),
			(ScalarType.UIntPtr, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, nuint.Zero.ToConstantExpression()),
			(ScalarType.Half, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, ((Half)0).ToConstantExpression()),
			(ScalarType.Single, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, 0F.ToConstantExpression()),
			(ScalarType.Double, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, 0D.ToConstantExpression()),
			(ScalarType.Decimal, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, 0M.ToConstantExpression()),
			(ScalarType.DateOnly, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, DateOnly.MinValue.ToConstantExpression()),
			(ScalarType.DateTime, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, DateTime.MinValue.ToConstantExpression()),
			(ScalarType.DateTimeOffset, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, DateTimeOffset.MinValue.ToConstantExpression()),
			(ScalarType.TimeOnly, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, TimeOnly.MinValue.ToConstantExpression()),
			(ScalarType.TimeSpan, ScalarType.Boolean) => value.Operation(BinaryOperator.GreaterThan, TimeSpan.Zero.ToConstantExpression()),

			(ScalarType.DateOnly, ScalarType.String) => ((LambdaExpression)((DateOnly value) => value.ToISO8601(null))).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTime, ScalarType.String) => ((LambdaExpression)((DateTime value) => value.ToISO8601(null))).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTimeOffset, ScalarType.String) => ((LambdaExpression)((DateTimeOffset value) => value.ToISO8601(null))).Invoke(value).ReduceExtensions(),
			(ScalarType.Enum, ScalarType.String) => value.Call(nameof(Enum.ToString), "F".ToConstantExpression()),
			(ScalarType.TimeOnly, ScalarType.String) => ((LambdaExpression)((TimeOnly value) => value.ToISO8601(null))).Invoke(value).ReduceExtensions(),
			(ScalarType.TimeSpan, ScalarType.String) => ((LambdaExpression)((TimeSpan value) => value.ToText(null))).Invoke(value).ReduceExtensions(),
			(_, ScalarType.String) => value.Call(nameof(object.ToString)),

			(ScalarType.Int32, ScalarType.Index) => typeof(Index).ToNewExpression(value),
			(ScalarType.UInt32, ScalarType.Index) => typeof(Index).ToNewExpression(value.Cast<int>(true)),

			(ScalarType.Int32, ScalarType.DateOnly) => typeof(DateOnly).ToStaticMethodCallExpression(nameof(DateOnly.FromDayNumber), value),
			(ScalarType.UInt32, ScalarType.DateOnly) => typeof(DateOnly).ToStaticMethodCallExpression(nameof(DateOnly.FromDayNumber), value.Cast<int>(true)),

			(ScalarType.Int64, ScalarType.DateTime) => typeof(DateTime).ToNewExpression(value),
			(ScalarType.UInt64, ScalarType.DateTime) => typeof(DateTime).ToNewExpression(value.Cast<long>(true)),

			(ScalarType.DateOnly, ScalarType.DateTime) => ((LambdaExpression)((DateOnly value) => value.ToDateTime(TimeOnly.MinValue))).Invoke(value),
			(ScalarType.DateOnly, ScalarType.DateTimeOffset) => ((LambdaExpression)((DateOnly value) => value.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset())).Invoke(value),
			(ScalarType.DateOnly, ScalarType.TimeSpan) => ((LambdaExpression)((DateOnly value) => TimeSpan.FromDays(value.DayNumber))).Invoke(value),
			(ScalarType.DateOnly, ScalarType.Int32) => value.Property(nameof(DateOnly.DayNumber)),
			(ScalarType.DateOnly, ScalarType.UInt32) => value.Property(nameof(DateOnly.DayNumber)).Cast<uint>(),
			(ScalarType.DateOnly, ScalarType.Int64) => value.Property(nameof(DateOnly.DayNumber)).Cast<long>(),
			(ScalarType.DateOnly, ScalarType.UInt64) => value.Property(nameof(DateOnly.DayNumber)).Cast<ulong>(),

			(ScalarType.DateTime, ScalarType.DateOnly) => ((LambdaExpression)((DateTime value) => value.ToDateOnly())).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTime, ScalarType.DateTimeOffset) => ((LambdaExpression)((DateTime value) => value.ToDateTimeOffset())).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTime, ScalarType.TimeOnly) => ((LambdaExpression)((DateTime value) => value.ToTimeOnly())).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTime, ScalarType.Int64) => value.Property(nameof(DateTime.Ticks)),
			(ScalarType.DateTime, ScalarType.UInt64) => value.Property(nameof(DateTime.Ticks)).Cast<ulong>(),

			(ScalarType.DateTimeOffset, ScalarType.DateOnly) => ((LambdaExpression)((DateTimeOffset value) => value.ToDateOnly())).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTimeOffset, ScalarType.DateTime) => value.Property(nameof(DateTimeOffset.LocalDateTime)),
			(ScalarType.DateTimeOffset, ScalarType.TimeOnly) => ((LambdaExpression)((DateTimeOffset value) => value.ToTimeOnly())).Invoke(value).ReduceExtensions(),
			(ScalarType.DateTimeOffset, ScalarType.Int64) => value.Property(nameof(DateTimeOffset.Ticks)),
			(ScalarType.DateTimeOffset, ScalarType.UInt64) => value.Property(nameof(DateTimeOffset.Ticks)).Cast<ulong>(),

			(ScalarType.TimeOnly, ScalarType.TimeSpan) => ((LambdaExpression)((TimeOnly value) => value.ToTimeSpan())).Invoke(value),
			(ScalarType.TimeOnly, ScalarType.Int64) => value.Property(nameof(TimeOnly.Ticks)),
			(ScalarType.TimeOnly, ScalarType.UInt64) => value.Property(nameof(TimeOnly.Ticks)).Cast<ulong>(),

			(ScalarType.TimeSpan, ScalarType.TimeOnly) => typeof(TimeOnly).ToStaticMethodCallExpression(nameof(TimeOnly.FromTimeSpan), value),
			(ScalarType.TimeSpan, ScalarType.Int64) => value.Property(nameof(TimeSpan.Ticks)),
			(ScalarType.TimeSpan, ScalarType.UInt64) => value.Property(nameof(TimeSpan.Ticks)).Cast<ulong>(),

			(ScalarType.String, ScalarType.Char) => typeof(Convert).ToStaticMethodCallExpression(nameof(System.Convert.ToChar), value),
			(ScalarType.String, ScalarType.Enum) => typeof(Enum).ToStaticMethodCallExpression(nameof(Enum.Parse), new[] { targetType }, value, true.ToConstantExpression()),
			(ScalarType.String, ScalarType.Uri) => typeof(Uri).ToNewExpression(value),
			(ScalarType.String, ScalarType.Boolean
				or ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
				or ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
				or ScalarType.IntPtr or ScalarType.UIntPtr
				or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal
				or ScalarType.DateOnly or ScalarType.DateTime or ScalarType.DateTimeOffset or ScalarType.TimeOnly or ScalarType.TimeSpan
				or ScalarType.Guid) => targetType.ToStaticMethodCallExpression(nameof(int.Parse), value),

			_ => value.Cast(targetType)
		};

		if (isSourceNullable)
			expression = expression.IsNotNull().IIf(expression, @this);

		return expression;
	}

	public static bool? ConvertToBoolean(object? value) => value switch
	{
		string text when text.IsNotBlank() => bool.Parse(text),
		null or string => null,
		'1' or 'x' or 'X' or 'y' or 'Y' or 't' or 'T' => true,
		(sbyte)0 or (short)0 or 0 or 0L or (byte)0 or (ushort)0 or 0U or 0UL or 0F or 0D or 0M => false,
		sbyte or short or int or long or byte or ushort or uint or ulong or float or double or decimal => true,
		Int128 x => x != Int128.Zero,
		BigInteger x => x != BigInteger.Zero,
		UInt128 x => x != UInt128.Zero,
		IntPtr x => x != IntPtr.Zero,
		UIntPtr x => x != UIntPtr.Zero,
		Half x => x != (Half)0,
		DateOnly x => x != DateOnly.MinValue,
		DateTime x => x != DateTime.MinValue,
		DateTimeOffset x => x != DateTimeOffset.MinValue,
		TimeOnly x => x != TimeOnly.MinValue,
		TimeSpan x => x != TimeSpan.MinValue,
		Guid x => x != Guid.Empty,
		_ => (bool)value
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BigInteger? ConvertToBigInteger(object? value) => value.ConvertToPrimitive(BigInteger.Zero, BigInteger.One, BigInteger.Parse, x => (BigInteger)x);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte? ConvertToByte(object? value) => value.ConvertToPrimitive((byte)0, (byte)1, byte.Parse, Convert.ToByte);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static char? ConvertToChar(object? value) => value.ConvertToPrimitive('0', '1', Convert.ToChar, Convert.ToChar);

	public static DateOnly? ConvertToDateOnly(object? value) => value switch
	{
		string text when text.IsNotBlank() => DateOnly.Parse(text),
		null or string => null,
		int x => DateOnly.FromDayNumber(x),
		uint x => DateOnly.FromDayNumber(checked((int)x)),
		DateTime x => x.ToDateOnly(),
		DateTimeOffset x => x.ToDateOnly(),
		_ => (DateOnly)value
	};

	public static DateTime? ConvertToDateTime(object? value) => value switch
	{
		string text when text.IsNotBlank() => DateTime.Parse(text),
		null or string => null,
		long x => new(x),
		ulong x => new(checked((long)x)),
		DateOnly x => x.ToDateTime(TimeOnly.MinValue),
		DateTimeOffset x => x.DateTime,
		_ => (DateTime)value
	};

	public static DateTimeOffset? ConvertToDateTimeOffset(object? value) => value switch
	{
		string text when text.IsNotBlank() => DateTimeOffset.Parse(text),
		null or string => null,
		long x => new DateTime(x).ToDateTimeOffset(),
		ulong x => new DateTime(checked((long)x)).ToDateTimeOffset(),
		DateOnly x => x.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset(),
		DateTime x => x.ToDateTimeOffset(),
		_ => (DateTimeOffset)value
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static decimal? ConvertToDecimal(object? value) => value.ConvertToPrimitive(0M, 1M, decimal.Parse, Convert.ToDecimal);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static double? ConvertToDouble(object? value) => value.ConvertToPrimitive(0D, 1D, double.Parse, Convert.ToDouble);

	public static T? ConvertToEnum<T>(object? value) where T : struct, Enum => value switch
	{
		string text when text.IsNotBlank() => Enum.Parse<T>(text, true),
		null or string => null,
		sbyte or short or int or long or byte or ushort or uint or ulong => (T)Enum.ToObject(typeof(T), value),
		_ => (T)value
	};

	public static Guid? ConvertToGuid(object? value) => value switch
	{
		string text when text.IsNotBlank() => Guid.Parse(text),
		null or string => null,
		_ => (Guid)value
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Half? ConvertToHalf(object? value) => value.ConvertToPrimitive((Half)0, (Half)1, Half.Parse, x => (Half)x);

	public static Index? ConvertToIndex(object? value) => value switch
	{
		string text when text.IsNotBlank() => new Index(int.Parse(text)),
		null or string => null,
		sbyte x => new Index(x),
		short x => new Index(x),
		int x => new Index(x),
		long x => new Index(checked((int)x)),
		byte x => new Index(x),
		ushort x => new Index(x),
		uint x => new Index(checked((int)x)),
		ulong x => new Index(checked((int)x)),
		_ => (Index)value
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static short? ConvertToInt16(object? value) => value.ConvertToPrimitive((short)0, (short)1, short.Parse, Convert.ToInt16);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int? ConvertToInt32(object? value) => value.ConvertToPrimitive(0, 1, int.Parse, x => x);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static long? ConvertToInt64(object? value) => value.ConvertToPrimitive((long)0, (long)1, long.Parse, Convert.ToInt64);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Int128? ConvertToInt128(object? value) => value.ConvertToPrimitive(Int128.Zero, Int128.One, Int128.Parse, x => (Int128)x);

	public static nint? ConvertToIntPtr(object? value) => value switch
	{
		string text when text.IsNotBlank() => IntPtr.Parse(text),
		null or string => null,
		_ => (nint)value
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static sbyte? ConvertToSByte(object? value) => value.ConvertToPrimitive((sbyte)0, (sbyte)1, sbyte.Parse, Convert.ToSByte);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float? ConvertToSingle(object? value) => value.ConvertToPrimitive(0F, 1F, float.Parse, Convert.ToSingle);

	public static string? ConvertToString(object? value) => value switch
	{
		null => null,
		DateOnly x => x.ToISO8601(),
		DateTime x => x.ToISO8601(),
		DateTimeOffset x => x.ToISO8601(),
		Enum x => x.ToString("F"),
		TimeOnly x => x.ToISO8601(),
		TimeSpan x => x.ToText(),
		_ => value.ToString(),
	};

	public static TimeOnly? ConvertToTimeOnly(object? value) => value switch
	{
		string text when text.IsNotBlank() => TimeOnly.Parse(text),
		null or string => null,
		long x => new(x),
		ulong x => new(checked((long)x)),
		DateTime x => TimeOnly.FromDateTime(x),
		DateTimeOffset x => TimeOnly.FromDateTime(x.DateTime),
		TimeSpan x => TimeOnly.FromTimeSpan(x),
		_ => (TimeOnly)value
	};

	public static TimeSpan? ConvertToTimeSpan(object? value) => value switch
	{
		string text when text.IsNotBlank() => TimeSpan.Parse(text),
		null or string => null,
		long x => new(x),
		ulong x => new(checked((long)x)),
		_ => (TimeSpan)value
	};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ushort? ConvertToUInt16(object? value) => value.ConvertToPrimitive((ushort)0, (ushort)1, ushort.Parse, Convert.ToUInt16);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static uint? ConvertToUInt32(object? value) => value.ConvertToPrimitive(0U, 1U, uint.Parse, Convert.ToUInt32);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ulong? ConvertToUInt64(object? value) => value.ConvertToPrimitive((ulong)0, (ulong)1, ulong.Parse, Convert.ToUInt64);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UInt128? ConvertToUInt128(object? value) => value.ConvertToPrimitive(UInt128.Zero, UInt128.One, UInt128.Parse, x => (UInt128)x);

	public static nuint? ConvertToUIntPtr(object? value) => value switch
	{
		string text when text.IsNotBlank() => UIntPtr.Parse(text),
		null or string => null,
		_ => (nuint)value
	};

	public static Uri? ConvertToUri(object? value) => value switch
	{
		string text when text.IsNotBlank() => new Uri(text, text[0] is '/' ? UriKind.Relative : UriKind.Absolute),
		null or string => null,
		_ => (Uri)value
	};

	private static T? ConvertToPrimitive<T>(this object? @this, T trueValue, T falseValue, Func<string, T> parse, Func<int, T> fromInt) where T : struct => @this switch
	{
		string text when text.IsNotBlank() => parse(text),
		null or string => null,
		true => trueValue,
		false => falseValue,
		Index x => fromInt(x.Value),
		_ => (T)@this
	};
}
