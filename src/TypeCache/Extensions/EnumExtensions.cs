// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static Attribute[] Attributes<T>(this T @this)
		where T : struct, Enum
		=> Enum<T>.IsDefined(@this)
			? typeof(T).GetField(@this.Name(), Public | Static)!.GetCustomAttributes(false).Cast<Attribute>().ToArray()
			: Array<Attribute>.Empty;

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringComparer Comparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);

	[DebuggerHidden]
	public static bool HasAnyFlag<T>(this T @this, T[] flags)
		where T : struct, Enum
		=> flags?.Any(flag => @this.HasFlag(flag)) ?? false;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("X");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Hex(this Enum @this)
		=> @this.ToString("X");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("X");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> @this.ToString("X");

	/// <remarks>
	/// <c>=&gt; <see cref="Enum{T}"/>.IsDefined(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> Enum<T>.IsDefined(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("F");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Name(this Enum @this)
		=> @this.ToString("F");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("F");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> @this.ToString("F");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Number(this Enum @this)
		=> @this.ToString("D");

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D");</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> @this.ToString("D");

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this T @this, T value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
		where T : struct, Enum
	{
		if (Enum<T>.Equals(@this, value))
			Throw(caller!, (argument1!, argument2!), (@this, value), message, logger);
	}

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument1">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument2">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual<T>(this T @this, T value, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument1 = null,
		[CallerArgumentExpression("value")] string? argument2 = null)
		where T : struct, Enum
	{
		if (!Enum<T>.Equals(@this, value))
			Throw(caller!, (argument1!, argument2!), (@this, value), message, logger);
	}

	private static void Throw(string method, (string, string) arguments, (object?, object?) items, string? message, ILogger? logger,
		[CallerMemberName] string? caller = null)
	{
		var exception = new ArgumentOutOfRangeException(
			paramName: arguments.ToString(),
			actualValue: items,
			message: message ?? Invariant($"{method}: {caller}"));

		logger?.LogError(exception, exception.Message);

		throw exception;
	}

	public static bool IsConcurrent(this CollectionType @this) => @this switch
	{
		CollectionType.ConcurrentBag or CollectionType.ConcurrentDictionary
		or CollectionType.ConcurrentQueue or CollectionType.ConcurrentStack => true,
		_ => false
	};

	public static bool IsConvertibleTo(this ScalarType @this, ScalarType target)
		=> (@this, target) switch
		{
			_ when target == @this => true,
			(ScalarType.String, _) => true,
			(_, ScalarType.Boolean or ScalarType.String) => true,
			(ScalarType.Boolean, ScalarType.Char) => true,
			(ScalarType.Boolean, ScalarType.SByte) => true,
			(ScalarType.Boolean, ScalarType.Int16) => true,
			(ScalarType.Boolean, ScalarType.Int32) => true,
			(ScalarType.Boolean, ScalarType.Int64) => true,
			(ScalarType.Boolean, ScalarType.Int128) => true,
			(ScalarType.Boolean, ScalarType.Byte) => true,
			(ScalarType.Boolean, ScalarType.UInt16) => true,
			(ScalarType.Boolean, ScalarType.UInt32) => true,
			(ScalarType.Boolean, ScalarType.UInt64) => true,
			(ScalarType.Boolean, ScalarType.UInt128) => true,
			(ScalarType.Byte, ScalarType.Char) => true,
			(ScalarType.Byte, ScalarType.SByte) => true,
			(ScalarType.Byte, ScalarType.Int16) => true,
			(ScalarType.Byte, ScalarType.Int32) => true,
			(ScalarType.Byte, ScalarType.Int64) => true,
			(ScalarType.Byte, ScalarType.Int128) => true,
			(ScalarType.Byte, ScalarType.BigInteger) => true,
			(ScalarType.Byte, ScalarType.UInt16) => true,
			(ScalarType.Byte, ScalarType.UInt32) => true,
			(ScalarType.Byte, ScalarType.UInt64) => true,
			(ScalarType.Byte, ScalarType.UInt128) => true,
			(ScalarType.Byte, ScalarType.Half) => true,
			(ScalarType.Byte, ScalarType.Single) => true,
			(ScalarType.Byte, ScalarType.Double) => true,
			(ScalarType.Byte, ScalarType.Decimal) => true,
			(ScalarType.Char, ScalarType.SByte) => true,
			(ScalarType.Char, ScalarType.Int16) => true,
			(ScalarType.Char, ScalarType.Int32) => true,
			(ScalarType.Char, ScalarType.Int64) => true,
			(ScalarType.Char, ScalarType.Int128) => true,
			(ScalarType.Char, ScalarType.BigInteger) => true,
			(ScalarType.Char, ScalarType.UInt16) => true,
			(ScalarType.Char, ScalarType.UInt32) => true,
			(ScalarType.Char, ScalarType.UInt64) => true,
			(ScalarType.Char, ScalarType.UInt128) => true,
			(ScalarType.Char, ScalarType.Half) => true,
			(ScalarType.Char, ScalarType.Single) => true,
			(ScalarType.Char, ScalarType.Double) => true,
			(ScalarType.Char, ScalarType.Decimal) => true,
			(ScalarType.DateOnly, ScalarType.DateTime) => true,
			(ScalarType.DateOnly, ScalarType.DateTimeOffset) => true,
			(ScalarType.DateOnly, ScalarType.TimeSpan) => true,
			(ScalarType.DateOnly, ScalarType.Int32) => true,
			(ScalarType.DateOnly, ScalarType.Int64) => true,
			(ScalarType.DateOnly, ScalarType.UInt32) => true,
			(ScalarType.DateOnly, ScalarType.UInt64) => true,
			(ScalarType.DateTime, ScalarType.DateOnly) => true,
			(ScalarType.DateTime, ScalarType.DateTimeOffset) => true,
			(ScalarType.DateTime, ScalarType.TimeOnly) => true,
			(ScalarType.DateTime, ScalarType.TimeSpan) => true,
			(ScalarType.DateTime, ScalarType.Int64) => true,
			(ScalarType.DateTime, ScalarType.UInt64) => true,
			(ScalarType.DateTimeOffset, ScalarType.DateOnly) => true,
			(ScalarType.DateTimeOffset, ScalarType.DateTime) => true,
			(ScalarType.DateTimeOffset, ScalarType.TimeOnly) => true,
			(ScalarType.DateTimeOffset, ScalarType.TimeSpan) => true,
			(ScalarType.DateTimeOffset, ScalarType.Int64) => true,
			(ScalarType.DateTimeOffset, ScalarType.UInt64) => true,
			(ScalarType.Int16, ScalarType.Char) => true,
			(ScalarType.Int16, ScalarType.Int32) => true,
			(ScalarType.Int16, ScalarType.Int64) => true,
			(ScalarType.Int16, ScalarType.Int128) => true,
			(ScalarType.Int16, ScalarType.BigInteger) => true,
			(ScalarType.Int16, ScalarType.UInt16) => true,
			(ScalarType.Int16, ScalarType.UInt32) => true,
			(ScalarType.Int16, ScalarType.UInt64) => true,
			(ScalarType.Int16, ScalarType.UInt128) => true,
			(ScalarType.Int16, ScalarType.Half) => true,
			(ScalarType.Int16, ScalarType.Single) => true,
			(ScalarType.Int16, ScalarType.Double) => true,
			(ScalarType.Int16, ScalarType.Decimal) => true,
			(ScalarType.Int32, ScalarType.DateOnly) => true,
			(ScalarType.Int32, ScalarType.Index) => true,
			(ScalarType.Int32, ScalarType.Int64) => true,
			(ScalarType.Int32, ScalarType.Int128) => true,
			(ScalarType.Int32, ScalarType.BigInteger) => true,
			(ScalarType.Int32, ScalarType.UInt32) => true,
			(ScalarType.Int32, ScalarType.UInt64) => true,
			(ScalarType.Int32, ScalarType.UInt128) => true,
			(ScalarType.Int32, ScalarType.Single) => true,
			(ScalarType.Int32, ScalarType.Double) => true,
			(ScalarType.Int32, ScalarType.Decimal) => true,
			(ScalarType.Int64, ScalarType.DateTime) => true,
			(ScalarType.Int64, ScalarType.DateTimeOffset) => true,
			(ScalarType.Int64, ScalarType.TimeOnly) => true,
			(ScalarType.Int64, ScalarType.TimeSpan) => true,
			(ScalarType.Int64, ScalarType.Int128) => true,
			(ScalarType.Int64, ScalarType.BigInteger) => true,
			(ScalarType.Int64, ScalarType.UInt64) => true,
			(ScalarType.Int64, ScalarType.UInt128) => true,
			(ScalarType.Int64, ScalarType.Double) => true,
			(ScalarType.Int64, ScalarType.Decimal) => true,
			(ScalarType.Int128, ScalarType.UInt128) => true,
			(ScalarType.Int128, ScalarType.BigInteger) => true,
			(ScalarType.Int128, ScalarType.Decimal) => true,
			(ScalarType.IntPtr, ScalarType.Int32) => true,
			(ScalarType.IntPtr, ScalarType.Int64) => true,
			(ScalarType.SByte, ScalarType.Char) => true,
			(ScalarType.SByte, ScalarType.Int16) => true,
			(ScalarType.SByte, ScalarType.Int32) => true,
			(ScalarType.SByte, ScalarType.Int64) => true,
			(ScalarType.SByte, ScalarType.Int128) => true,
			(ScalarType.SByte, ScalarType.BigInteger) => true,
			(ScalarType.SByte, ScalarType.Byte) => true,
			(ScalarType.SByte, ScalarType.UInt16) => true,
			(ScalarType.SByte, ScalarType.UInt32) => true,
			(ScalarType.SByte, ScalarType.UInt64) => true,
			(ScalarType.SByte, ScalarType.UInt128) => true,
			(ScalarType.SByte, ScalarType.Half) => true,
			(ScalarType.SByte, ScalarType.Single) => true,
			(ScalarType.SByte, ScalarType.Double) => true,
			(ScalarType.SByte, ScalarType.Decimal) => true,
			(ScalarType.TimeOnly, ScalarType.TimeSpan) => true,
			(ScalarType.TimeOnly, ScalarType.Int64) => true,
			(ScalarType.TimeOnly, ScalarType.UInt64) => true,
			(ScalarType.TimeSpan, ScalarType.TimeOnly) => true,
			(ScalarType.TimeSpan, ScalarType.Int64) => true,
			(ScalarType.TimeSpan, ScalarType.UInt64) => true,
			(ScalarType.UInt16, ScalarType.Char) => true,
			(ScalarType.UInt16, ScalarType.Int16) => true,
			(ScalarType.UInt16, ScalarType.Int32) => true,
			(ScalarType.UInt16, ScalarType.Int64) => true,
			(ScalarType.UInt16, ScalarType.Int128) => true,
			(ScalarType.UInt16, ScalarType.BigInteger) => true,
			(ScalarType.UInt16, ScalarType.UInt32) => true,
			(ScalarType.UInt16, ScalarType.UInt64) => true,
			(ScalarType.UInt16, ScalarType.UInt128) => true,
			(ScalarType.UInt16, ScalarType.Half) => true,
			(ScalarType.UInt16, ScalarType.Single) => true,
			(ScalarType.UInt16, ScalarType.Double) => true,
			(ScalarType.UInt16, ScalarType.Decimal) => true,
			(ScalarType.UInt32, ScalarType.DateOnly) => true,
			(ScalarType.UInt32, ScalarType.Index) => true,
			(ScalarType.UInt32, ScalarType.Int32) => true,
			(ScalarType.UInt32, ScalarType.Int64) => true,
			(ScalarType.UInt32, ScalarType.Int128) => true,
			(ScalarType.UInt32, ScalarType.BigInteger) => true,
			(ScalarType.UInt32, ScalarType.UInt32) => true,
			(ScalarType.UInt32, ScalarType.UInt64) => true,
			(ScalarType.UInt32, ScalarType.UInt128) => true,
			(ScalarType.UInt32, ScalarType.Single) => true,
			(ScalarType.UInt32, ScalarType.Double) => true,
			(ScalarType.UInt32, ScalarType.Decimal) => true,
			(ScalarType.UInt64, ScalarType.DateTime) => true,
			(ScalarType.UInt64, ScalarType.DateTimeOffset) => true,
			(ScalarType.UInt64, ScalarType.TimeOnly) => true,
			(ScalarType.UInt64, ScalarType.TimeSpan) => true,
			(ScalarType.UInt64, ScalarType.Int64) => true,
			(ScalarType.UInt64, ScalarType.Int128) => true,
			(ScalarType.UInt64, ScalarType.BigInteger) => true,
			(ScalarType.UInt64, ScalarType.UInt128) => true,
			(ScalarType.UInt64, ScalarType.Double) => true,
			(ScalarType.UInt64, ScalarType.Decimal) => true,
			(ScalarType.UInt128, ScalarType.Int128) => true,
			(ScalarType.UInt128, ScalarType.BigInteger) => true,
			(ScalarType.UInt128, ScalarType.Decimal) => true,
			(ScalarType.UIntPtr, ScalarType.UInt32) => true,
			(ScalarType.UIntPtr, ScalarType.UInt64) => true,
			_ => false
		};

	public static bool IsDictionary(this CollectionType @this)
		=> @this switch
		{
			CollectionType.Dictionary or CollectionType.ConcurrentDictionary or CollectionType.FrozenDictionary or CollectionType.SortedDictionary
			or CollectionType.ImmutableDictionary or CollectionType.ImmutableSortedDictionary
			or CollectionType.Hashtable or CollectionType.HybridDictionary or CollectionType.OrderedDictionary or CollectionType.ReadOnlyDictionary
			or CollectionType.KeyedCollection or CollectionType.ListDictionary or CollectionType.StringDictionary
			or CollectionType.NameObjectCollection or CollectionType.NameValueCollection or CollectionType.SortedList => true,
			_ => false
		};

	public static bool IsEnumUnderlyingType(this ScalarType @this)
		=> @this switch
		{
			ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64
			or ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 => true,
			_ => false
		};

	public static bool IsFrozen(this CollectionType @this)
		=> @this switch
		{
			CollectionType.FrozenDictionary or CollectionType.FrozenSet => true,
			_ => false
		};

	public static bool IsImmutable(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ImmutableArray
			or CollectionType.ImmutableDictionary or CollectionType.ImmutableSortedDictionary
			or CollectionType.ImmutableSet or CollectionType.ImmutableSortedSet
			or CollectionType.ImmutableList
			or CollectionType.ImmutableQueue
			or CollectionType.ImmutableStack => true,
			_ => false
		};

	/// <summary>
	/// Returns true for the current .Net primitives.
	/// </summary>
	public static bool IsPrimitive(this ScalarType @this)
		=> @this switch
		{
			ScalarType.Boolean
			or ScalarType.SByte or ScalarType.Byte
			or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64
			or ScalarType.IntPtr or ScalarType.UIntPtr
			or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64
			or ScalarType.Half or ScalarType.Single or ScalarType.Double
			or ScalarType.Char => true,
			_ => false
		};

	public static bool IsQueue(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ConcurrentQueue or CollectionType.ImmutableQueue or CollectionType.PriorityQueue or CollectionType.Queue => true,
			_ => false
		};

	public static bool IsReadOnly(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ReadOnlyCollection or CollectionType.ReadOnlyDictionary or CollectionType.ReadOnlyObservableCollection
			or CollectionType.ReadOnlyList or CollectionType.ReadOnlyObservableCollection or CollectionType.ReadOnlySet => true,
			_ => false
		};

	public static bool IsSet(this CollectionType @this)
		=> @this switch
		{
			CollectionType.Set or CollectionType.ReadOnlySet or CollectionType.SortedSet
			or CollectionType.FrozenSet or CollectionType.ImmutableSet or CollectionType.ImmutableSortedSet => true,
			_ => false
		};

	public static bool IsStack(this CollectionType @this)
		=> @this switch
		{
			CollectionType.ConcurrentStack or CollectionType.ImmutableStack or CollectionType.Stack => true,
			_ => false
		};
}
