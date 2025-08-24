// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Mapping;
using TypeCache.Reflection;
using static System.Globalization.CultureInfo;

namespace TypeCache.Utilities;

public static class ValueConverter
{
	private const string TRUE_CHARS = "1XxYyTt";

	private static T FromBoolean<T>(bool value)
		where T : INumberBase<T>
		=> value ? T.One : T.Zero;

	public static Expression CreateConversionExpression(this Expression @this, Type targetType)
	{
		@this.ThrowIfNull();
		targetType.ThrowIfNull();

		ParameterExpression value = nameof(value).ToParameterExpression(@this.Type);
		var sourceScalarType = @this.Type.ScalarType();
		var targetScalarType = targetType.ScalarType();
		var expression = (sourceScalarType, targetScalarType) switch
		{
			(ScalarType.Boolean, ScalarType.Char) => LambdaFactory.CreateFunc<bool, char>(_ => _ ? '1' : '0'),
			(ScalarType.Boolean, ScalarType.String) => LambdaFactory.CreateFunc<bool, string>(_ => _ ? bool.TrueString : bool.FalseString),
			(ScalarType.Boolean, _) when targetType.Implements(typeof(INumberBase<>)) =>
				typeof(ValueConverter).GetMethod(nameof(FromBoolean))!
					.MakeGenericMethod(targetType)
					.ToExpression(null, [value])
					.LambdaFunc(targetType, [value]),

			(ScalarType.Char, ScalarType.Boolean) => LambdaFactory.CreateFunc<char, bool>(_ => TRUE_CHARS.ContainsIgnoreCase(_)),
			(_, ScalarType.Boolean) when @this.Type.Implements(typeof(INumberBase<>)) =>
				typeof(NumericExtensions).GetMethod(nameof(NumericExtensions.IsZero))!
					.MakeGenericMethod(@this.Type)
					.ToExpression(null, [value])
					.LambdaFunc(targetType, [value]),
			(ScalarType.DateOnly, ScalarType.Boolean) => LambdaFactory.CreateFunc<DateOnly, bool>(_ => _ != DateOnly.MinValue),
			(ScalarType.DateTime, ScalarType.Boolean) => LambdaFactory.CreateFunc<DateTime, bool>(_ => _ != DateTime.MinValue),
			(ScalarType.DateTimeOffset, ScalarType.Boolean) => LambdaFactory.CreateFunc<DateTimeOffset, bool>(_ => _ != DateTimeOffset.MinValue),
			(ScalarType.TimeOnly, ScalarType.Boolean) => LambdaFactory.CreateFunc<TimeOnly, bool>(_ => _ != TimeOnly.MinValue),
			(ScalarType.TimeSpan, ScalarType.Boolean) => LambdaFactory.CreateFunc<TimeSpan, bool>(_ => _ != TimeSpan.Zero),

			(ScalarType.DateOnly, ScalarType.String) => LambdaFactory.CreateFunc<DateOnly, string>(_ => _.ToISO8601(null)),
			(ScalarType.DateTime, ScalarType.String) => LambdaFactory.CreateFunc<DateTime, string>(_ => _.ToISO8601(null)),
			(ScalarType.DateTimeOffset, ScalarType.String) => LambdaFactory.CreateFunc<DateTimeOffset, string>(_ => _.ToISO8601(null)),
			(ScalarType.Enum, ScalarType.String) => LambdaFactory.CreateFunc<Enum, string>(_ => _.Name()),
			(ScalarType.TimeOnly, ScalarType.String) => LambdaFactory.CreateFunc<TimeOnly, string>(_ => _.ToISO8601(null)),
			(ScalarType.TimeSpan, ScalarType.String) => LambdaFactory.CreateFunc<TimeSpan, string>(_ => _.ToText(null)),
			(ScalarType.Guid, ScalarType.String) => LambdaFactory.CreateFunc<Guid, string>(_ => _.ToString("D")),
			(_, ScalarType.String) =>
				@this.Type.GetMethod(nameof(object.ToString))!
					.ToExpression(value, [])
					.LambdaFunc(targetType, [value]),

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
			(ScalarType.String, ScalarType.Index) => LambdaFactory.CreateFunc<string, Index>(_ => Index.FromStart(int.Parse(_, InvariantCulture))),
			(ScalarType.String, ScalarType.Uri) => LambdaFactory.CreateFunc<string, Uri>(_ => new Uri(_, _.StartsWith('/') ? UriKind.Relative : UriKind.Absolute)),
			(ScalarType.String, ScalarType.Boolean) => LambdaFactory.CreateFunc<string, bool>(_ => bool.Parse(_)),
			(ScalarType.String, _) when targetType.Implements(typeof(IParsable<>)) =>
				targetType.GetMethod(nameof(IParsable<int>.Parse), [targetType, typeof(IFormatProvider)])!
					.ToExpression(null, [value, Expression.Constant(InvariantCulture, typeof(IFormatProvider))])
					.LambdaFunc(targetType, [value]),

			_ => null
		};

		const string HasValue = nameof(HasValue);
		const string Value = nameof(Value);

		if (expression is not null)
		{
			if (@this.Type.Is(typeof(Nullable<>)))
			{
				ParameterExpression source = nameof(source).ToParameterExpression(@this.Type);
				return @this.Property(HasValue).IIf(expression.Invoke([source.Property(Value)]).Lambda([source]), @this);
			}

			if (@this.Type.IsNullable())
				return @this.IsNotNull().IIf(expression, @this);

			return expression;
		}

		if (@this.Type.Is(typeof(Nullable<>)))
			return @this.Property(HasValue).IIf(@this.Property(Value).Cast(targetType), @this);

		if (@this.Type.IsNullable())
			return @this.IsNotNull().IIf(@this.Cast(targetType), @this);

		return @this.Cast(targetType);
	}

	public static object? ConvertTo(this object? @this, Type targetType)
	{
		if (@this is null)
			return null;

		var sourceType = @this.GetType();
		if (sourceType == targetType || sourceType.IsAssignableTo(targetType))
			return @this;

		var targetScalarType = targetType.ScalarType();
		var value = targetScalarType switch
		{
			ScalarType.Boolean => ConvertToBoolean(@this),
			ScalarType.DateOnly => ConvertToDateOnly(@this),
			ScalarType.DateTime => ConvertToDateTime(@this),
			ScalarType.DateTimeOffset => ConvertToDateTimeOffset(@this),
			ScalarType.Enum => @this switch
			{
				string text when text.IsNotBlank() => Enum.Parse(targetType, text, true),
				_ when @this.GetType() == targetType => @this,
				_ => Enum.ToObject(targetType, @this)
			},
			ScalarType.Guid => ConvertToGuid(@this),
			ScalarType.Index => ConvertToIndex(@this),
			ScalarType.String => ConvertToString(@this),
			ScalarType.TimeOnly => ConvertToTimeOnly(@this),
			ScalarType.TimeSpan => ConvertToTimeSpan(@this),
			ScalarType.Uri => ConvertToUri(@this),
			ScalarType.Char => ConvertToNumber<char>(@this),
			ScalarType.SByte => ConvertToNumber<sbyte>(@this),
			ScalarType.Int16 => ConvertToNumber<short>(@this),
			ScalarType.Int32 => ConvertToNumber<int>(@this),
			ScalarType.Int64 => ConvertToNumber<long>(@this),
			ScalarType.Int128 => ConvertToNumber<Int128>(@this),
			ScalarType.BigInteger => ConvertToNumber<BigInteger>(@this),
			ScalarType.IntPtr => ConvertToNumber<nint>(@this),
			ScalarType.Byte => ConvertToNumber<byte>(@this),
			ScalarType.UInt16 => ConvertToNumber<ushort>(@this),
			ScalarType.UInt32 => ConvertToNumber<uint>(@this),
			ScalarType.UInt64 => ConvertToNumber<ulong>(@this),
			ScalarType.UInt128 => ConvertToNumber<UInt128>(@this),
			ScalarType.UIntPtr => ConvertToNumber<nuint>(@this),
			ScalarType.Half => ConvertToNumber<Half>(@this),
			ScalarType.Single => ConvertToNumber<float>(@this),
			ScalarType.Double => ConvertToNumber<double>(@this),
			ScalarType.Decimal => ConvertToNumber<decimal>(@this),
			_ => null
		};

		return value;
	}

	public static bool? ConvertToBoolean(object? value)
		=> value switch
		{
			bool x => x,
			DateOnly x => x != DateOnly.MinValue,
			DateTime x => x != DateTime.MinValue,
			DateTimeOffset x => x != DateTimeOffset.MinValue,
			TimeOnly x => x != TimeOnly.MinValue,
			TimeSpan x => x != TimeSpan.MinValue,
			Guid x => x != Guid.Empty,
			char x when TRUE_CHARS.ContainsIgnoreCase(x) => true,
			sbyte x => !x.IsZero(),
			short x => !x.IsZero(),
			int x => !x.IsZero(),
			long x => !x.IsZero(),
			Int128 x => !x.IsZero(),
			BigInteger x => !x.IsZero(),
			IntPtr x => !x.IsZero(),
			byte x => !x.IsZero(),
			ushort x => !x.IsZero(),
			uint x => !x.IsZero(),
			ulong x => !x.IsZero(),
			UInt128 x => !x.IsZero(),
			UIntPtr x => !x.IsZero(),
			Half x => !x.IsZero(),
			float x => !x.IsZero(),
			double x => !x.IsZero(),
			decimal x => !x.IsZero(),
			string text when text.IsNotBlank() => text.Parse<bool>(),
			null or string => null,
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(Boolean)}]."))
		};

	public static DateOnly? ConvertToDateOnly(object? value)
		=> value switch
		{
			DateOnly x => x,
			string text when text.IsNotBlank() => DateOnly.Parse(text, InvariantCulture),
			null or string => null,
			int x => DateOnly.FromDayNumber(x),
			uint x => DateOnly.FromDayNumber(checked((int)x)),
			DateTime x => x.ToDateOnly(),
			DateTimeOffset x => x.ToDateOnly(),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(DateOnly)}]."))
		};

	public static DateTime? ConvertToDateTime(object? value)
		=> value switch
		{
			DateTime x => x,
			string text when text.IsNotBlank() => DateTime.Parse(text, InvariantCulture),
			null or string => null,
			long x => new(x),
			ulong x => new(checked((long)x)),
			DateOnly x => x.ToDateTime(TimeOnly.MinValue),
			DateTimeOffset x => x.DateTime,
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(DateTime)}]."))
		};

	public static DateTimeOffset? ConvertToDateTimeOffset(object? value)
		=> value switch
		{
			DateTimeOffset x => x,
			string text when text.IsNotBlank() => DateTimeOffset.Parse(text, InvariantCulture),
			null or string => null,
			long x => new DateTime(x).ToDateTimeOffset(),
			ulong x => new DateTime(checked((long)x)).ToDateTimeOffset(),
			DateOnly x => x.ToDateTime(TimeOnly.MinValue).ToDateTimeOffset(),
			DateTime x => x.ToDateTimeOffset(),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(DateTimeOffset)}]."))
		};

	public static T? ConvertToEnum<T>(object? value)
		where T : struct, Enum
		=> value switch
		{
			T x => x,
			Enum x => throw new NotSupportedException(Invariant($"Cannot convert value [{x.Name()}] of type [{value.GetType().Name}] to [{typeof(T).Name}].")),
			string text when text.IsNotBlank() => text.ToEnum<T>(),
			null or string => null,
			_ when value.GetType().ScalarType().IsEnumUnderlyingType() => (T)Enum.ToObject(typeof(T), value),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{typeof(T).Name}]."))
		};

	public static Guid? ConvertToGuid(object? value)
		=> value switch
		{
			Guid guid => guid,
			string text when text.IsNotBlank() => Guid.Parse(text, InvariantCulture),
			null or string => null,
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(Guid)}]."))
		};

	public static Index? ConvertToIndex(object? value)
		=> value switch
		{
			Index x => x,
			string text when text.IsNotBlank() => new Index(text.Parse<int>(InvariantCulture)),
			null or string => null,
			int x => new Index(x),
			sbyte x => new Index(int.CreateChecked(x)),
			short x => new Index(int.CreateChecked(x)),
			long x => new Index(int.CreateChecked(x)),
			Int128 x => new Index(int.CreateChecked(x)),
			BigInteger x => new Index(int.CreateChecked(x)),
			nint x => new Index(int.CreateChecked(x)),
			ushort x => new Index(int.CreateChecked(x)),
			uint x => new Index(int.CreateChecked(x)),
			ulong x => new Index(int.CreateChecked(x)),
			UInt128 x => new Index(int.CreateChecked(x)),
			nuint x => new Index(int.CreateChecked(x)),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(Index)}]."))
		};

	public static T? ConvertToNumber<T>(object? value)
		where T : struct, INumberBase<T>
		=> value switch
		{
			T x => (T?)x,
			sbyte x => T.CreateChecked(x),
			short x => T.CreateChecked(x),
			int x => T.CreateChecked(x),
			long x => T.CreateChecked(x),
			Int128 x => T.CreateChecked(x),
			BigInteger x => T.CreateChecked(x),
			nint x => T.CreateChecked(x),
			byte x => T.CreateChecked(x),
			ushort x => T.CreateChecked(x),
			uint x => T.CreateChecked(x),
			ulong x => T.CreateChecked(x),
			UInt128 x => T.CreateChecked(x),
			nuint x => T.CreateChecked(x),
			Half x => T.CreateChecked(x),
			float x => T.CreateChecked(x),
			double x => T.CreateChecked(x),
			decimal x => T.CreateChecked(x),
			string text when text.IsNotBlank() => T.Parse(text, InvariantCulture),
			null or string => null,
			true => T.One,
			false => T.Zero,
			Index index => T.CreateChecked(index.Value),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to a number."))
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
			Guid x => x.ToText(),
			TimeOnly x => x.ToISO8601(),
			TimeSpan x => x.ToText(),
			IFormattable x => x.ToString(null, InvariantCulture),
			_ => value.ToString(),
		};

	public static TimeOnly? ConvertToTimeOnly(object? value)
		=> value switch
		{
			TimeOnly x => x,
			string text when text.IsNotBlank() => TimeOnly.Parse(text, InvariantCulture),
			null or string => null,
			long x => new(x),
			ulong x => new(checked((long)x)),
			DateTime x => TimeOnly.FromDateTime(x),
			DateTimeOffset x => TimeOnly.FromDateTime(x.DateTime),
			TimeSpan x => TimeOnly.FromTimeSpan(x),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(TimeOnly)}]."))
		};

	public static TimeSpan? ConvertToTimeSpan(object? value)
		=> value switch
		{
			TimeSpan x => x,
			string text when text.IsNotBlank() => TimeSpan.Parse(text, InvariantCulture),
			null or string => null,
			TimeOnly x => TimeSpan.FromTicks(x.Ticks),
			long x => TimeSpan.FromTicks(x),
			ulong x => TimeSpan.FromTicks(checked((long)x)),
			_ => throw new NotSupportedException(Invariant($"Cannot convert value of type [{value.GetType().Name}] to [{nameof(TimeSpan)}]."))
		};

	public static Uri? ConvertToUri(object? value)
		=> value switch
		{
			Uri x => x,
			string text when text.IsNotBlank() => text.ToUri(),
			null or string => null,
			_ => value.ToString()?.ToUri()
		};
}
