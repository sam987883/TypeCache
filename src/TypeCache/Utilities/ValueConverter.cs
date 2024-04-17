// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.Utilities;

public static class ValueConverter
{
	private const string TRUE_CHARS = "1XxYyTt";

	[MethodImpl(AggressiveInlining)]
	private static bool IsNotZero<T>(in T value)
		where T : INumberBase<T>
		=> value != T.Zero;

	public static Expression CreateConversionExpression(this Expression @this, Type targetType)
	{
		@this.AssertNotNull();
		targetType.AssertNotNull();

		var isSourceNullable = @this.Type.IsNullable();
		var value = isSourceNullable ? @this.Property(nameof(Nullable<int>.Value)) : @this;
		var sourceScalarType = @this.Type.GetScalarType();
		var targetScalarType = targetType.GetScalarType();

		var expression = (sourceScalarType, targetScalarType) switch
		{
			(ScalarType.Boolean, ScalarType.Char) => LambdaFactory.CreateFunc((bool _) => _ ? '1' : '0'),
			(ScalarType.Boolean, ScalarType.SByte) => LambdaFactory.CreateFunc((bool _) => _ ? (sbyte)1 : (sbyte)0),
			(ScalarType.Boolean, ScalarType.Int16) => LambdaFactory.CreateFunc((bool _) => _ ? (short)1 : (short)0),
			(ScalarType.Boolean, ScalarType.Int32) => LambdaFactory.CreateFunc((bool _) => _ ? 1 : 0),
			(ScalarType.Boolean, ScalarType.Int64) => LambdaFactory.CreateFunc((bool _) => _ ? 1L : 0L),
			(ScalarType.Boolean, ScalarType.Int128) => LambdaFactory.CreateFunc((bool _) => _ ? Int128.One : Int128.Zero),
			(ScalarType.Boolean, ScalarType.BigInteger) => LambdaFactory.CreateFunc((bool _) => _ ? BigInteger.One : BigInteger.Zero),
			(ScalarType.Boolean, ScalarType.Byte) => LambdaFactory.CreateFunc((bool _) => _ ? (byte)1 : (byte)0),
			(ScalarType.Boolean, ScalarType.UInt16) => LambdaFactory.CreateFunc((bool _) => _ ? (ushort)1 : (ushort)0),
			(ScalarType.Boolean, ScalarType.UInt32) => LambdaFactory.CreateFunc((bool _) => _ ? 1U : 0U),
			(ScalarType.Boolean, ScalarType.UInt64) => LambdaFactory.CreateFunc((bool _) => _ ? 1UL : 0UL),
			(ScalarType.Boolean, ScalarType.UInt128) => LambdaFactory.CreateFunc((bool _) => _ ? UInt128.One : UInt128.Zero),

			(ScalarType.Char, ScalarType.Boolean) => LambdaFactory.CreateFunc((char _) => TRUE_CHARS.Contains(_)),
			(ScalarType.SByte, ScalarType.Boolean) => LambdaFactory.CreateFunc((sbyte _) => IsNotZero<sbyte>(_)),
			(ScalarType.Int16, ScalarType.Boolean) => LambdaFactory.CreateFunc((short _) => _ != 0),
			(ScalarType.Int32, ScalarType.Boolean) => LambdaFactory.CreateFunc((int _) => _ != 0),
			(ScalarType.Int64, ScalarType.Boolean) => LambdaFactory.CreateFunc((long _) => _ != 0L),
			(ScalarType.Int128, ScalarType.Boolean) => LambdaFactory.CreateFunc((Int128 _) => _ != Int128.Zero),
			(ScalarType.BigInteger, ScalarType.Boolean) => LambdaFactory.CreateFunc((BigInteger _) => _ != BigInteger.Zero),
			(ScalarType.Byte, ScalarType.Boolean) => LambdaFactory.CreateFunc((byte _) => _ != 0),
			(ScalarType.UInt16, ScalarType.Boolean) => LambdaFactory.CreateFunc((ushort _) => _ != 0),
			(ScalarType.UInt32, ScalarType.Boolean) => LambdaFactory.CreateFunc((uint _) => _ != 0U),
			(ScalarType.UInt64, ScalarType.Boolean) => LambdaFactory.CreateFunc((ulong _) => _ != 0UL),
			(ScalarType.UInt128, ScalarType.Boolean) => LambdaFactory.CreateFunc((UInt128 _) => _ != UInt128.Zero),
			(ScalarType.IntPtr, ScalarType.Boolean) => LambdaFactory.CreateFunc((IntPtr _) => _ != nint.Zero),
			(ScalarType.UIntPtr, ScalarType.Boolean) => LambdaFactory.CreateFunc((UIntPtr _) => _ != nuint.Zero),
			(ScalarType.Half, ScalarType.Boolean) => LambdaFactory.CreateFunc((Half _) => _ != Half.Zero),
			(ScalarType.Single, ScalarType.Boolean) => LambdaFactory.CreateFunc((float _) => _ != 0F),
			(ScalarType.Double, ScalarType.Boolean) => LambdaFactory.CreateFunc((double _) => _ != 0D),
			(ScalarType.Decimal, ScalarType.Boolean) => LambdaFactory.CreateFunc((decimal _) => _ != 0M),
			(ScalarType.DateOnly, ScalarType.Boolean) => LambdaFactory.CreateFunc((DateOnly _) => _ != DateOnly.MinValue),
			(ScalarType.DateTime, ScalarType.Boolean) => LambdaFactory.CreateFunc((DateTime _) => _ != DateTime.MinValue),
			(ScalarType.DateTimeOffset, ScalarType.Boolean) => LambdaFactory.CreateFunc((DateTimeOffset _) => _ != DateTimeOffset.MinValue),
			(ScalarType.TimeOnly, ScalarType.Boolean) => LambdaFactory.CreateFunc((TimeOnly _) => _ != TimeOnly.MinValue),
			(ScalarType.TimeSpan, ScalarType.Boolean) => LambdaFactory.CreateFunc((TimeSpan _) => _ != TimeSpan.Zero),

			(ScalarType.Char, ScalarType.String) => LambdaFactory.CreateFunc((char _) => _.ToString()),
			(ScalarType.SByte, ScalarType.String) => LambdaFactory.CreateFunc((sbyte _) => _.ToString()),
			(ScalarType.Int16, ScalarType.String) => LambdaFactory.CreateFunc((short _) => _.ToString()),
			(ScalarType.Int32, ScalarType.String) => LambdaFactory.CreateFunc((int _) => _.ToString()),
			(ScalarType.Int64, ScalarType.String) => LambdaFactory.CreateFunc((long _) => _.ToString()),
			(ScalarType.Int128, ScalarType.String) => LambdaFactory.CreateFunc((Int128 _) => _.ToString()),
			(ScalarType.BigInteger, ScalarType.String) => LambdaFactory.CreateFunc((BigInteger _) => _.ToString()),
			(ScalarType.Byte, ScalarType.String) => LambdaFactory.CreateFunc((sbyte _) => _.ToString()),
			(ScalarType.UInt16, ScalarType.String) => LambdaFactory.CreateFunc((short _) => _.ToString()),
			(ScalarType.UInt32, ScalarType.String) => LambdaFactory.CreateFunc((int _) => _.ToString()),
			(ScalarType.UInt64, ScalarType.String) => LambdaFactory.CreateFunc((long _) => _.ToString()),
			(ScalarType.UInt128, ScalarType.String) => LambdaFactory.CreateFunc((Int128 _) => _.ToString()),
			(ScalarType.IntPtr, ScalarType.String) => LambdaFactory.CreateFunc((IntPtr _) => _.ToString()),
			(ScalarType.UIntPtr, ScalarType.String) => LambdaFactory.CreateFunc((UIntPtr _) => _.ToString()),
			(ScalarType.Half, ScalarType.String) => LambdaFactory.CreateFunc((Half _) => _.ToString()),
			(ScalarType.Single, ScalarType.String) => LambdaFactory.CreateFunc((float _) => _.ToString()),
			(ScalarType.Double, ScalarType.String) => LambdaFactory.CreateFunc((double _) => _.ToString()),
			(ScalarType.Decimal, ScalarType.String) => LambdaFactory.CreateFunc((decimal _) => _.ToString()),
			(ScalarType.DateOnly, ScalarType.String) => LambdaFactory.CreateFunc((DateOnly _) => _.ToISO8601(null)),
			(ScalarType.DateTime, ScalarType.String) => LambdaFactory.CreateFunc((DateTime _) => _.ToISO8601(null)),
			(ScalarType.DateTimeOffset, ScalarType.String) => LambdaFactory.CreateFunc((DateTimeOffset _) => _.ToISO8601(null)),
			(ScalarType.Enum, ScalarType.String) => LambdaFactory.CreateFunc((Enum _) => _.Name()),
			(ScalarType.TimeOnly, ScalarType.String) => LambdaFactory.CreateFunc((TimeOnly _) => _.ToISO8601(null)),
			(ScalarType.TimeSpan, ScalarType.String) => LambdaFactory.CreateFunc((TimeSpan _) => _.ToText(null)),
			(ScalarType.Guid, ScalarType.String) => LambdaFactory.CreateFunc((Guid _) => _.ToString("D")),
			(ScalarType.Index, ScalarType.String) => LambdaFactory.CreateFunc((Index _) => _.Value.ToString()),
			(ScalarType.Uri, ScalarType.String) => LambdaFactory.CreateFunc((Uri _) => _.ToString()),

			(ScalarType.Int32, ScalarType.Index) => LambdaFactory.CreateFunc((int _) => new Index(_, false)),
			(ScalarType.UInt32, ScalarType.Index) => LambdaFactory.CreateFunc((uint _) => new Index((int)_, false)),

			(ScalarType.Int32, ScalarType.DateOnly) => LambdaFactory.CreateFunc((int _) => DateOnly.FromDayNumber(_)),
			(ScalarType.UInt32, ScalarType.DateOnly) => LambdaFactory.CreateFunc((uint _) => DateOnly.FromDayNumber((int)_)),
			(ScalarType.Int64, ScalarType.DateOnly) => LambdaFactory.CreateFunc((long _) => DateOnly.FromDayNumber((int)_)),
			(ScalarType.UInt64, ScalarType.DateOnly) => LambdaFactory.CreateFunc((ulong _) => DateOnly.FromDayNumber((int)_)),

			(ScalarType.Int64, ScalarType.DateTime) => LambdaFactory.CreateFunc((long _) => new DateTime(_)),
			(ScalarType.UInt64, ScalarType.DateTime) => LambdaFactory.CreateFunc((ulong _) => new DateTime((long)_)),

			(ScalarType.DateOnly, ScalarType.DateTime) => LambdaFactory.CreateFunc((DateOnly _) => _.ToDateTime(TimeOnly.MinValue)),
			(ScalarType.DateOnly, ScalarType.DateTimeOffset) => LambdaFactory.CreateFunc((DateOnly _) => _.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset()),
			(ScalarType.DateOnly, ScalarType.TimeSpan) => LambdaFactory.CreateFunc((DateOnly _) => TimeSpan.FromDays(_.DayNumber)),
			(ScalarType.DateOnly, ScalarType.Int32) => LambdaFactory.CreateFunc((DateOnly _) => _.DayNumber),
			(ScalarType.DateOnly, ScalarType.UInt32) => LambdaFactory.CreateFunc((DateOnly _) => (uint)_.DayNumber),
			(ScalarType.DateOnly, ScalarType.Int64) => LambdaFactory.CreateFunc((DateOnly _) => (long)_.DayNumber),
			(ScalarType.DateOnly, ScalarType.UInt64) => LambdaFactory.CreateFunc((DateOnly _) => (ulong)_.DayNumber),

			(ScalarType.DateTime, ScalarType.DateOnly) => LambdaFactory.CreateFunc((DateTime _) => _.ToDateOnly()),
			(ScalarType.DateTime, ScalarType.DateTimeOffset) => LambdaFactory.CreateFunc((DateTime _) => _.ToDateTimeOffset()),
			(ScalarType.DateTime, ScalarType.TimeOnly) => LambdaFactory.CreateFunc((DateTime _) => _.ToTimeOnly()),
			(ScalarType.DateTime, ScalarType.Int64) => LambdaFactory.CreateFunc((DateTime _) => _.Ticks),
			(ScalarType.DateTime, ScalarType.UInt64) => LambdaFactory.CreateFunc((DateTime _) => (ulong)_.Ticks),

			(ScalarType.DateTimeOffset, ScalarType.DateOnly) => LambdaFactory.CreateFunc((DateTimeOffset _) => _.ToDateOnly()),
			(ScalarType.DateTimeOffset, ScalarType.DateTime) => LambdaFactory.CreateFunc((DateTimeOffset _) => _.LocalDateTime),
			(ScalarType.DateTimeOffset, ScalarType.TimeOnly) => LambdaFactory.CreateFunc((DateTimeOffset _) => _.ToTimeOnly()),
			(ScalarType.DateTimeOffset, ScalarType.Int64) => LambdaFactory.CreateFunc((DateTimeOffset _) => _.Ticks),
			(ScalarType.DateTimeOffset, ScalarType.UInt64) => LambdaFactory.CreateFunc((DateTimeOffset _) => (ulong)_.Ticks),

			(ScalarType.TimeOnly, ScalarType.TimeSpan) => LambdaFactory.CreateFunc((TimeOnly _) => _.ToTimeSpan()),
			(ScalarType.TimeOnly, ScalarType.Int64) => LambdaFactory.CreateFunc((TimeOnly _) => _.Ticks),
			(ScalarType.TimeOnly, ScalarType.UInt64) => LambdaFactory.CreateFunc((TimeOnly _) => (ulong)_.Ticks),

			(ScalarType.TimeSpan, ScalarType.TimeOnly) => LambdaFactory.CreateFunc((TimeSpan _) => TimeOnly.FromTimeSpan(_)),
			(ScalarType.TimeSpan, ScalarType.Int64) => LambdaFactory.CreateFunc((TimeSpan _) => _.Ticks),
			(ScalarType.TimeSpan, ScalarType.UInt64) => LambdaFactory.CreateFunc((TimeSpan _) => (ulong)_.Ticks),

			(ScalarType.String, ScalarType.Char) => LambdaFactory.CreateFunc((string _) => Convert.ToChar(_)),
			(ScalarType.String, ScalarType.Enum) => LambdaFactory.CreateFunc((string _) => Enum.Parse(targetType, _, true)),
			(ScalarType.String, ScalarType.Guid) => LambdaFactory.CreateFunc((string _) => Guid.Parse(_)),
			(ScalarType.String, ScalarType.Index) => LambdaFactory.CreateFunc((string _) => Index.FromStart(int.Parse(_))),
			(ScalarType.String, ScalarType.Uri) => LambdaFactory.CreateFunc((string _) => new Uri(_)),
			(ScalarType.String, ScalarType.Boolean) => LambdaFactory.CreateFunc((string _) => bool.Parse(_)),
			(ScalarType.String, ScalarType.SByte) => LambdaFactory.CreateFunc((string _) => sbyte.Parse(_)),
			(ScalarType.String, ScalarType.Int16) => LambdaFactory.CreateFunc((string _) => short.Parse(_)),
			(ScalarType.String, ScalarType.Int32) => LambdaFactory.CreateFunc((string _) => int.Parse(_)),
			(ScalarType.String, ScalarType.Int64) => LambdaFactory.CreateFunc((string _) => long.Parse(_)),
			(ScalarType.String, ScalarType.Int128) => LambdaFactory.CreateFunc((string _) => Int128.Parse(_)),
			(ScalarType.String, ScalarType.BigInteger) => LambdaFactory.CreateFunc((string _) => BigInteger.Parse(_)),
			(ScalarType.String, ScalarType.Byte) => LambdaFactory.CreateFunc((string _) => byte.Parse(_)),
			(ScalarType.String, ScalarType.UInt16) => LambdaFactory.CreateFunc((string _) => ushort.Parse(_)),
			(ScalarType.String, ScalarType.UInt32) => LambdaFactory.CreateFunc((string _) => uint.Parse(_)),
			(ScalarType.String, ScalarType.UInt64) => LambdaFactory.CreateFunc((string _) => ulong.Parse(_)),
			(ScalarType.String, ScalarType.UInt128) => LambdaFactory.CreateFunc((string _) => UInt128.Parse(_)),
			(ScalarType.String, ScalarType.IntPtr) => LambdaFactory.CreateFunc((string _) => nint.Parse(_)),
			(ScalarType.String, ScalarType.UIntPtr) => LambdaFactory.CreateFunc((string _) => nuint.Parse(_)),
			(ScalarType.String, ScalarType.Half) => LambdaFactory.CreateFunc((string _) => Half.Parse(_)),
			(ScalarType.String, ScalarType.Single) => LambdaFactory.CreateFunc((string _) => float.Parse(_)),
			(ScalarType.String, ScalarType.Double) => LambdaFactory.CreateFunc((string _) => double.Parse(_)),
			(ScalarType.String, ScalarType.Decimal) => LambdaFactory.CreateFunc((string _) => decimal.Parse(_)),
			(ScalarType.String, ScalarType.DateOnly) => LambdaFactory.CreateFunc((string _) => DateOnly.Parse(_)),
			(ScalarType.String, ScalarType.DateTime) => LambdaFactory.CreateFunc((string _) => DateTime.Parse(_)),
			(ScalarType.String, ScalarType.DateTimeOffset) => LambdaFactory.CreateFunc((string _) => DateTimeOffset.Parse(_)),
			(ScalarType.String, ScalarType.TimeOnly) => LambdaFactory.CreateFunc((string _) => TimeOnly.Parse(_)),
			(ScalarType.String, ScalarType.TimeSpan) => LambdaFactory.CreateFunc((string _) => TimeSpan.Parse(_)),

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
			Int128 x => x != Int128.Zero,
			BigInteger x => x != BigInteger.Zero,
			UInt128 x => x != UInt128.Zero,
			IntPtr x => x != IntPtr.Zero,
			UIntPtr x => x != UIntPtr.Zero,
			Half x => x != Half.Zero,
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
			T value => value,
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
