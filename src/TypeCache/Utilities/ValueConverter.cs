// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Utilities;

public static class ValueConverter
{
	private const string TRUE_CHARS = "1XxYyTt";

	public static Expression CreateConversionExpression(this Expression @this, Type targetType)
	{
		@this.ThrowIfNull();
		targetType.ThrowIfNull();

		var isSourceNullable = @this.Type.IsNullable();
		var value = isSourceNullable ? @this.Property(nameof(Nullable<int>.Value)) : @this;
		var sourceScalarType = @this.Type.GetScalarType();
		var targetScalarType = targetType.GetScalarType();

		var expression = (sourceScalarType, targetScalarType) switch
		{
			(ScalarType.Boolean, ScalarType.Char) => LambdaFactory.CreateFunc<bool, char>(_ => _ ? '1' : '0'),
			(ScalarType.Boolean, ScalarType.SByte) => LambdaFactory.CreateFunc<bool, sbyte>(_ => _ ? (sbyte)1 : (sbyte)0),
			(ScalarType.Boolean, ScalarType.Int16) => LambdaFactory.CreateFunc<bool, short>(_ => _ ? (short)1 : (short)0),
			(ScalarType.Boolean, ScalarType.Int32) => LambdaFactory.CreateFunc<bool, int>(_ => _ ? 1 : 0),
			(ScalarType.Boolean, ScalarType.Int64) => LambdaFactory.CreateFunc<bool, long>(_ => _ ? 1L : 0L),
			(ScalarType.Boolean, ScalarType.Int128) => LambdaFactory.CreateFunc<bool, Int128>(_ => _ ? Int128.One : Int128.Zero),
			(ScalarType.Boolean, ScalarType.BigInteger) => LambdaFactory.CreateFunc<bool, BigInteger>(_ => _ ? BigInteger.One : BigInteger.Zero),
			(ScalarType.Boolean, ScalarType.Byte) => LambdaFactory.CreateFunc<bool, byte>(_ => _ ? (byte)1 : (byte)0),
			(ScalarType.Boolean, ScalarType.UInt16) => LambdaFactory.CreateFunc<bool, ushort>(_ => _ ? (ushort)1 : (ushort)0),
			(ScalarType.Boolean, ScalarType.UInt32) => LambdaFactory.CreateFunc<bool, uint>(_ => _ ? 1U : 0U),
			(ScalarType.Boolean, ScalarType.UInt64) => LambdaFactory.CreateFunc<bool, ulong>(_ => _ ? 1UL : 0UL),
			(ScalarType.Boolean, ScalarType.UInt128) => LambdaFactory.CreateFunc<bool, UInt128>(_ => _ ? UInt128.One : UInt128.Zero),

			(ScalarType.Char, ScalarType.Boolean) => LambdaFactory.CreateFunc<char, bool>(_ => TRUE_CHARS.Contains(_)),
			(ScalarType.SByte, ScalarType.Boolean) => LambdaFactory.CreateFunc<sbyte, bool>(_ => !_.IsZero()),
			(ScalarType.Int16, ScalarType.Boolean) => LambdaFactory.CreateFunc<short, bool>(_ => !_.IsZero()),
			(ScalarType.Int32, ScalarType.Boolean) => LambdaFactory.CreateFunc<int, bool>(_ => !_.IsZero()),
			(ScalarType.Int64, ScalarType.Boolean) => LambdaFactory.CreateFunc<long, bool>(_ => !_.IsZero()),
			(ScalarType.Int128, ScalarType.Boolean) => LambdaFactory.CreateFunc<Int128, bool>(_ => !_.IsZero()),
			(ScalarType.BigInteger, ScalarType.Boolean) => LambdaFactory.CreateFunc<BigInteger, bool>(_ => !_.IsZero()),
			(ScalarType.Byte, ScalarType.Boolean) => LambdaFactory.CreateFunc<byte, bool>(_ => !_.IsZero()),
			(ScalarType.UInt16, ScalarType.Boolean) => LambdaFactory.CreateFunc<ushort, bool>(_ => !_.IsZero()),
			(ScalarType.UInt32, ScalarType.Boolean) => LambdaFactory.CreateFunc<uint, bool>(_ => !_.IsZero()),
			(ScalarType.UInt64, ScalarType.Boolean) => LambdaFactory.CreateFunc<ulong, bool>(_ => !_.IsZero()),
			(ScalarType.UInt128, ScalarType.Boolean) => LambdaFactory.CreateFunc<UInt128, bool>(_ => !_.IsZero()),
			(ScalarType.IntPtr, ScalarType.Boolean) => LambdaFactory.CreateFunc<IntPtr, bool>(_ => !_.IsZero()),
			(ScalarType.UIntPtr, ScalarType.Boolean) => LambdaFactory.CreateFunc<UIntPtr, bool>(_ => !_.IsZero()),
			(ScalarType.Half, ScalarType.Boolean) => LambdaFactory.CreateFunc<Half, bool>(_ => !_.IsZero()),
			(ScalarType.Single, ScalarType.Boolean) => LambdaFactory.CreateFunc<float, bool>(_ => !_.IsZero()),
			(ScalarType.Double, ScalarType.Boolean) => LambdaFactory.CreateFunc<double, bool>(_ => !_.IsZero()),
			(ScalarType.Decimal, ScalarType.Boolean) => LambdaFactory.CreateFunc<decimal, bool>(_ => !_.IsZero()),
			(ScalarType.DateOnly, ScalarType.Boolean) => LambdaFactory.CreateFunc<DateOnly, bool>(_ => _ != DateOnly.MinValue),
			(ScalarType.DateTime, ScalarType.Boolean) => LambdaFactory.CreateFunc<DateTime, bool>(_ => _ != DateTime.MinValue),
			(ScalarType.DateTimeOffset, ScalarType.Boolean) => LambdaFactory.CreateFunc<DateTimeOffset, bool>(_ => _ != DateTimeOffset.MinValue),
			(ScalarType.TimeOnly, ScalarType.Boolean) => LambdaFactory.CreateFunc<TimeOnly, bool>(_ => _ != TimeOnly.MinValue),
			(ScalarType.TimeSpan, ScalarType.Boolean) => LambdaFactory.CreateFunc<TimeSpan, bool>(_ => _ != TimeSpan.Zero),

			(ScalarType.Char, ScalarType.String) => LambdaFactory.CreateFunc<char, string>(_ => _.ToString()),
			(ScalarType.SByte, ScalarType.String) => LambdaFactory.CreateFunc<sbyte, string>(_ => _.ToString()),
			(ScalarType.Int16, ScalarType.String) => LambdaFactory.CreateFunc<short, string>(_ => _.ToString()),
			(ScalarType.Int32, ScalarType.String) => LambdaFactory.CreateFunc<int, string>(_ => _.ToString()),
			(ScalarType.Int64, ScalarType.String) => LambdaFactory.CreateFunc<long, string>(_ => _.ToString()),
			(ScalarType.Int128, ScalarType.String) => LambdaFactory.CreateFunc<Int128, string>(_ => _.ToString()),
			(ScalarType.BigInteger, ScalarType.String) => LambdaFactory.CreateFunc<BigInteger, string>(_ => _.ToString()),
			(ScalarType.Byte, ScalarType.String) => LambdaFactory.CreateFunc<sbyte, string>(_ => _.ToString()),
			(ScalarType.UInt16, ScalarType.String) => LambdaFactory.CreateFunc<short, string>(_ => _.ToString()),
			(ScalarType.UInt32, ScalarType.String) => LambdaFactory.CreateFunc<int, string>(_ => _.ToString()),
			(ScalarType.UInt64, ScalarType.String) => LambdaFactory.CreateFunc<long, string>(_ => _.ToString()),
			(ScalarType.UInt128, ScalarType.String) => LambdaFactory.CreateFunc<Int128, string>(_ => _.ToString()),
			(ScalarType.IntPtr, ScalarType.String) => LambdaFactory.CreateFunc<IntPtr, string>(_ => _.ToString()),
			(ScalarType.UIntPtr, ScalarType.String) => LambdaFactory.CreateFunc<UIntPtr, string>(_ => _.ToString()),
			(ScalarType.Half, ScalarType.String) => LambdaFactory.CreateFunc<Half, string>(_ => _.ToString()),
			(ScalarType.Single, ScalarType.String) => LambdaFactory.CreateFunc<float, string>(_ => _.ToString()),
			(ScalarType.Double, ScalarType.String) => LambdaFactory.CreateFunc<double, string>(_ => _.ToString()),
			(ScalarType.Decimal, ScalarType.String) => LambdaFactory.CreateFunc<decimal, string>(_ => _.ToString()),
			(ScalarType.DateOnly, ScalarType.String) => LambdaFactory.CreateFunc<DateOnly, string>(_ => _.ToISO8601(null)),
			(ScalarType.DateTime, ScalarType.String) => LambdaFactory.CreateFunc<DateTime, string>(_ => _.ToISO8601(null)),
			(ScalarType.DateTimeOffset, ScalarType.String) => LambdaFactory.CreateFunc<DateTimeOffset, string>(_ => _.ToISO8601(null)),
			(ScalarType.Enum, ScalarType.String) => LambdaFactory.CreateFunc<Enum, string>(_ => _.Name()),
			(ScalarType.TimeOnly, ScalarType.String) => LambdaFactory.CreateFunc<TimeOnly, string>(_ => _.ToISO8601(null)),
			(ScalarType.TimeSpan, ScalarType.String) => LambdaFactory.CreateFunc<TimeSpan, string>(_ => _.ToText(null)),
			(ScalarType.Guid, ScalarType.String) => LambdaFactory.CreateFunc<Guid, string>(_ => _.ToString("D")),
			(ScalarType.Index, ScalarType.String) => LambdaFactory.CreateFunc<Index, string>(_ => _.Value.ToString()),
			(ScalarType.Uri, ScalarType.String) => LambdaFactory.CreateFunc<Uri, string>(_ => _.ToString()),

			(ScalarType.Int32, ScalarType.Index) => LambdaFactory.CreateFunc<int, Index>(_ => new Index(_, false)),
			(ScalarType.Int32, ScalarType.DateOnly) => LambdaFactory.CreateFunc<int, DateOnly>(_ => DateOnly.FromDayNumber(_)),

			(ScalarType.UInt32, ScalarType.Index) => LambdaFactory.CreateFunc<uint, Index>(_ => new Index((int)_, false)),
			(ScalarType.UInt32, ScalarType.DateOnly) => LambdaFactory.CreateFunc<uint, DateOnly>(_ => DateOnly.FromDayNumber((int)_)),

			(ScalarType.Int64, ScalarType.DateOnly) => LambdaFactory.CreateFunc<long, DateOnly>(_ => DateOnly.FromDayNumber((int)_)),
			(ScalarType.Int64, ScalarType.DateTime) => LambdaFactory.CreateFunc<long, DateTime>(_ => new DateTime(_)),

			(ScalarType.UInt64, ScalarType.DateOnly) => LambdaFactory.CreateFunc<ulong, DateOnly>(_ => DateOnly.FromDayNumber((int)_)),
			(ScalarType.UInt64, ScalarType.DateTime) => LambdaFactory.CreateFunc<ulong, DateTime>(_ => new DateTime((long)_)),

			(ScalarType.DateOnly, ScalarType.DateTime) => LambdaFactory.CreateFunc<DateOnly, DateTime>(_ => _.ToDateTime(TimeOnly.MinValue)),
			(ScalarType.DateOnly, ScalarType.DateTimeOffset) => LambdaFactory.CreateFunc<DateOnly, DateTimeOffset>(_ => _.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset()),
			(ScalarType.DateOnly, ScalarType.TimeSpan) => LambdaFactory.CreateFunc<DateOnly, TimeSpan>(_ => TimeSpan.FromDays(_.DayNumber)),
			(ScalarType.DateOnly, ScalarType.Int32) => LambdaFactory.CreateFunc<DateOnly, int>(_ => _.DayNumber),
			(ScalarType.DateOnly, ScalarType.UInt32) => LambdaFactory.CreateFunc<DateOnly, uint>(_ => (uint)_.DayNumber),
			(ScalarType.DateOnly, ScalarType.Int64) => LambdaFactory.CreateFunc<DateOnly, long>(_ => (long)_.DayNumber),
			(ScalarType.DateOnly, ScalarType.UInt64) => LambdaFactory.CreateFunc<DateOnly, ulong>(_ => (ulong)_.DayNumber),

			(ScalarType.DateTime, ScalarType.DateOnly) => LambdaFactory.CreateFunc<DateTime, DateOnly>(_ => _.ToDateOnly()),
			(ScalarType.DateTime, ScalarType.DateTimeOffset) => LambdaFactory.CreateFunc<DateTime, DateTimeOffset>(_ => _.ToDateTimeOffset()),
			(ScalarType.DateTime, ScalarType.TimeOnly) => LambdaFactory.CreateFunc<DateTime, TimeOnly>(_ => _.ToTimeOnly()),
			(ScalarType.DateTime, ScalarType.Int64) => LambdaFactory.CreateFunc<DateTime, long>(_ => _.Ticks),
			(ScalarType.DateTime, ScalarType.UInt64) => LambdaFactory.CreateFunc<DateTime, ulong>(_ => (ulong)_.Ticks),

			(ScalarType.DateTimeOffset, ScalarType.DateOnly) => LambdaFactory.CreateFunc<DateTimeOffset, DateOnly>(_ => _.ToDateOnly()),
			(ScalarType.DateTimeOffset, ScalarType.DateTime) => LambdaFactory.CreateFunc<DateTimeOffset, DateTime>(_ => _.LocalDateTime),
			(ScalarType.DateTimeOffset, ScalarType.TimeOnly) => LambdaFactory.CreateFunc<DateTimeOffset, TimeOnly>(_ => _.ToTimeOnly()),
			(ScalarType.DateTimeOffset, ScalarType.Int64) => LambdaFactory.CreateFunc<DateTimeOffset, long>(_ => _.Ticks),
			(ScalarType.DateTimeOffset, ScalarType.UInt64) => LambdaFactory.CreateFunc<DateTimeOffset, ulong>(_ => (ulong)_.Ticks),

			(ScalarType.TimeOnly, ScalarType.TimeSpan) => LambdaFactory.CreateFunc<TimeOnly, TimeSpan>(_ => _.ToTimeSpan()),
			(ScalarType.TimeOnly, ScalarType.Int64) => LambdaFactory.CreateFunc<TimeOnly, long>(_ => _.Ticks),
			(ScalarType.TimeOnly, ScalarType.UInt64) => LambdaFactory.CreateFunc<TimeOnly, ulong>(_ => (ulong)_.Ticks),

			(ScalarType.TimeSpan, ScalarType.TimeOnly) => LambdaFactory.CreateFunc<TimeSpan, TimeOnly>(_ => TimeOnly.FromTimeSpan(_)),
			(ScalarType.TimeSpan, ScalarType.Int64) => LambdaFactory.CreateFunc<TimeSpan, long>(_ => _.Ticks),
			(ScalarType.TimeSpan, ScalarType.UInt64) => LambdaFactory.CreateFunc<TimeSpan, ulong>(_ => (ulong)_.Ticks),

			(ScalarType.String, ScalarType.Char) => LambdaFactory.CreateFunc<string, char>(_ => Convert.ToChar(_)),
			(ScalarType.String, ScalarType.Enum) => LambdaFactory.CreateEnumParseFunc(targetType),
			(ScalarType.String, ScalarType.Guid) => LambdaFactory.CreateFunc<string, Guid>(_ => Guid.Parse(_)),
			(ScalarType.String, ScalarType.Index) => LambdaFactory.CreateFunc<string, Index>(_ => Index.FromStart(int.Parse(_))),
			(ScalarType.String, ScalarType.Uri) => LambdaFactory.CreateFunc<string, Uri>(_ => new Uri(_)),
			(ScalarType.String, ScalarType.Boolean) => LambdaFactory.CreateFunc<string, bool>(_ => bool.Parse(_)),
			(ScalarType.String, ScalarType.SByte) => LambdaFactory.CreateFunc<string, sbyte>(_ => sbyte.Parse(_)),
			(ScalarType.String, ScalarType.Int16) => LambdaFactory.CreateFunc<string, short>(_ => short.Parse(_)),
			(ScalarType.String, ScalarType.Int32) => LambdaFactory.CreateFunc<string, int>(_ => int.Parse(_)),
			(ScalarType.String, ScalarType.Int64) => LambdaFactory.CreateFunc<string, long>(_ => long.Parse(_)),
			(ScalarType.String, ScalarType.Int128) => LambdaFactory.CreateFunc<string, Int128>(_ => Int128.Parse(_)),
			(ScalarType.String, ScalarType.BigInteger) => LambdaFactory.CreateFunc<string, BigInteger>(_ => BigInteger.Parse(_)),
			(ScalarType.String, ScalarType.Byte) => LambdaFactory.CreateFunc<string, byte>(_ => byte.Parse(_)),
			(ScalarType.String, ScalarType.UInt16) => LambdaFactory.CreateFunc<string, ushort>(_ => ushort.Parse(_)),
			(ScalarType.String, ScalarType.UInt32) => LambdaFactory.CreateFunc<string, uint>(_ => uint.Parse(_)),
			(ScalarType.String, ScalarType.UInt64) => LambdaFactory.CreateFunc<string, ulong>(_ => ulong.Parse(_)),
			(ScalarType.String, ScalarType.UInt128) => LambdaFactory.CreateFunc<string, UInt128>(_ => UInt128.Parse(_)),
			(ScalarType.String, ScalarType.IntPtr) => LambdaFactory.CreateFunc<string, nint>(_ => nint.Parse(_)),
			(ScalarType.String, ScalarType.UIntPtr) => LambdaFactory.CreateFunc<string, nuint>(_ => nuint.Parse(_)),
			(ScalarType.String, ScalarType.Half) => LambdaFactory.CreateFunc<string, Half>(_ => Half.Parse(_)),
			(ScalarType.String, ScalarType.Single) => LambdaFactory.CreateFunc<string, float>(_ => float.Parse(_)),
			(ScalarType.String, ScalarType.Double) => LambdaFactory.CreateFunc<string, double>(_ => double.Parse(_)),
			(ScalarType.String, ScalarType.Decimal) => LambdaFactory.CreateFunc<string, decimal>(_ => decimal.Parse(_)),
			(ScalarType.String, ScalarType.DateOnly) => LambdaFactory.CreateFunc<string, DateOnly>(_ => DateOnly.Parse(_)),
			(ScalarType.String, ScalarType.DateTime) => LambdaFactory.CreateFunc<string, DateTime>(_ => DateTime.Parse(_)),
			(ScalarType.String, ScalarType.DateTimeOffset) => LambdaFactory.CreateFunc<string, DateTimeOffset>(_ => DateTimeOffset.Parse(_)),
			(ScalarType.String, ScalarType.TimeOnly) => LambdaFactory.CreateFunc<string, TimeOnly>(_ => TimeOnly.Parse(_)),
			(ScalarType.String, ScalarType.TimeSpan) => LambdaFactory.CreateFunc<string, TimeSpan>(_ => TimeSpan.Parse(_)),

			_ => null
		};

		if (expression is null)
			return value.Cast(targetType);

		if (isSourceNullable)
			return expression.IsNotNull().IIf(expression, @this);

		return expression;
	}

	public static bool? ConvertToBoolean(object? value)
		=> value switch
		{
			bool x => x,
			(sbyte)0 or (short)0 or 0 or 0L or (byte)0 or (ushort)0 or 0U or 0UL or 0F or 0D or 0M => false,
			sbyte or short or int or long or byte or ushort or uint or ulong or float or double or decimal => true,
			char x when TRUE_CHARS.Contains(x) => true,
			string text when text.IsNotBlank() => text.Parse<bool>(),
			null or string => null,
			Int128 x => !x.IsZero(),
			BigInteger x => !x.IsZero(),
			UInt128 x => !x.IsZero(),
			IntPtr x => !x.IsZero(),
			UIntPtr x => !x.IsZero(),
			Half x => !x.IsZero(),
			DateOnly x => x != DateOnly.MinValue,
			DateTime x => x != DateTime.MinValue,
			DateTimeOffset x => x != DateTimeOffset.MinValue,
			TimeOnly x => x != TimeOnly.MinValue,
			TimeSpan x => x != TimeSpan.MinValue,
			Guid x => x != Guid.Empty,
			_ => (bool)value
		};

	public static DateOnly? ConvertToDateOnly(object? value)
		=> value switch
		{
			DateOnly x => x,
			string text when text.IsNotBlank() => DateOnly.Parse(text),
			null or string => null,
			int x => DateOnly.FromDayNumber(x),
			uint x => DateOnly.FromDayNumber(checked((int)x)),
			DateTime x => x.ToDateOnly(),
			DateTimeOffset x => x.ToDateOnly(),
			_ => (DateOnly)value
		};

	public static DateTime? ConvertToDateTime(object? value)
		=> value switch
		{
			DateTime x => x,
			string text when text.IsNotBlank() => DateTime.Parse(text),
			null or string => null,
			long x => new(x),
			ulong x => new(checked((long)x)),
			DateOnly x => x.ToDateTime(TimeOnly.MinValue),
			DateTimeOffset x => x.DateTime,
			_ => (DateTime)value
		};

	public static DateTimeOffset? ConvertToDateTimeOffset(object? value)
		=> value switch
		{
			DateTimeOffset x => x,
			string text when text.IsNotBlank() => DateTimeOffset.Parse(text),
			null or string => null,
			long x => new DateTime(x).ToDateTimeOffset(),
			ulong x => new DateTime(checked((long)x)).ToDateTimeOffset(),
			DateOnly x => x.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset(),
			DateTime x => x.ToDateTimeOffset(),
			_ => (DateTimeOffset)value
		};

	public static T? ConvertToEnum<T>(object? value)
		where T : struct, Enum
		=> value switch
		{
			T x => x,
			string text when text.IsNotBlank() => text.ToEnum<T>(),
			null or string => null,
			sbyte or short or int or long or byte or ushort or uint or ulong => (T)Enum.ToObject(typeof(T), value),
			_ => (T)value
		};

	public static Guid? ConvertToGuid(object? value)
		=> value switch
		{
			Guid guid => guid,
			string text when text.IsNotBlank() => Guid.Parse(text),
			null or string => null,
			_ => (Guid)value
		};

	public static Index? ConvertToIndex(object? value)
		=> value switch
		{
			Index x => x,
			string text when text.IsNotBlank() => new Index(text.Parse<int>()),
			null or string => null,
			int number => new Index(number),
			sbyte number => new Index(int.CreateChecked(number)),
			short number => new Index(int.CreateChecked(number)),
			long number => new Index(int.CreateChecked(number)),
			Int128 number => new Index(int.CreateChecked(number)),
			BigInteger number => new Index(int.CreateChecked(number)),
			nint number => new Index(int.CreateChecked(number)),
			ushort number => new Index(int.CreateChecked(number)),
			uint number => new Index(int.CreateChecked(number)),
			ulong number => new Index(int.CreateChecked(number)),
			UInt128 number => new Index(int.CreateChecked(number)),
			nuint number => new Index(int.CreateChecked(number)),
			_ => (Index)value
		};

	public static nint? ConvertToIntPtr(object? value)
		=> value switch
		{
			nint x => x,
			string text when text.IsNotBlank() => IntPtr.Parse(text),
			null or string => null,
			_ => (nint)value
		};

	public static T? ConvertToNumber<T>(object? @this)
		where T : struct, INumberBase<T>
		=> @this switch
		{
			T value => (T?)value,
			sbyte number => T.CreateChecked(number),
			short number => T.CreateChecked(number),
			int number => T.CreateChecked(number),
			long number => T.CreateChecked(number),
			Int128 number => T.CreateChecked(number),
			BigInteger number => T.CreateChecked(number),
			nint number => T.CreateChecked(number),
			ushort number => T.CreateChecked(number),
			uint number => T.CreateChecked(number),
			ulong number => T.CreateChecked(number),
			UInt128 number => T.CreateChecked(number),
			nuint number => T.CreateChecked(number),
			string text when text.IsNotBlank() => T.Parse(text, InvariantCulture),
			null or string => null,
			true => T.One,
			false => T.Zero,
			Index i => T.CreateChecked(i.Value),
			_ => (T)@this
		};

	public static string? ConvertToString(object? value)
		=> value switch
		{
			string x => x,
			null => null,
			DateOnly x => x.ToISO8601(),
			DateTime x => x.ToISO8601(),
			DateTimeOffset x => x.ToISO8601(),
			Enum x => x.Name(),
			TimeOnly x => x.ToISO8601(),
			TimeSpan x => x.ToText(),
			_ => value.ToString(),
		};

	public static TimeOnly? ConvertToTimeOnly(object? value)
		=> value switch
		{
			TimeOnly x => x,
			string text when text.IsNotBlank() => TimeOnly.Parse(text),
			null or string => null,
			long x => new(x),
			ulong x => new(checked((long)x)),
			DateTime x => TimeOnly.FromDateTime(x),
			DateTimeOffset x => TimeOnly.FromDateTime(x.DateTime),
			TimeSpan x => TimeOnly.FromTimeSpan(x),
			_ => (TimeOnly)value
		};

	public static TimeSpan? ConvertToTimeSpan(object? value)
		=> value switch
		{
			TimeSpan x => x,
			string text when text.IsNotBlank() => TimeSpan.Parse(text),
			null or string => null,
			long x => new(x),
			ulong x => new(checked((long)x)),
			_ => (TimeSpan)value
		};

	public static nuint? ConvertToUIntPtr(object? value)
		=> value switch
		{
			nuint x => x,
			string text when text.IsNotBlank() => UIntPtr.Parse(text),
			null or string => null,
			_ => (nuint)value
		};

	public static Uri? ConvertToUri(object? value)
		=> value switch
		{
			Uri x => x,
			string text when text.IsNotBlank() => new Uri(text, text[0] is '/' ? UriKind.Relative : UriKind.Absolute),
			null or string => null,
			_ => (Uri)value
		};
}
