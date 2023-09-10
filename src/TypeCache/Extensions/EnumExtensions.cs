// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static IReadOnlyCollection<Attribute> Attributes<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Attributes : Array<Attribute>.Empty;

	[DebuggerHidden]
	public static bool HasAnyFlag<T>(this T @this, params T[] flags)
		where T : struct, Enum
		=> flags?.Any(flag => @this.HasFlag(flag)) ?? false;

	[DebuggerHidden]
	public static string Hex<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Hex : @this.ToString("X");

	[DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.ContainsKey(@this);

	[DebuggerHidden]
	public static string Name<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Name : @this.ToString();

	[DebuggerHidden]
	public static string Number<T>(this T @this)
		where T : struct, Enum
		=> EnumOf<T>.Tokens.TryGetValue(@this, out var token) ? token.Number : @this.ToString("D");

	/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringComparer ToStringComparer(this StringComparison @this)
		=> StringComparer.FromComparison(@this);

	public static bool IsConcurrent(this CollectionType @this) => @this switch
	{
		CollectionType.ConcurrentBag or CollectionType.ConcurrentDictionary
		or CollectionType.ConcurrentQueue or CollectionType.ConcurrentStack => true,
		_ => false
	};

	public static bool IsConvertibleTo(this ScalarType @this, ScalarType target) => (@this, target) switch
	{
		_ when target == @this => true,
		(ScalarType.String, _) or (_, ScalarType.Boolean or ScalarType.String) => true,
		(ScalarType.Boolean, ScalarType.Char or ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64
			or ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64) => true,
		(ScalarType.Byte, ScalarType.Boolean or ScalarType.Char
			or ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
			or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.Char, ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
			or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.DateOnly, ScalarType.DateTime or ScalarType.DateTimeOffset or Extensions.ScalarType.TimeSpan
			or ScalarType.Int32 or ScalarType.Int64 or ScalarType.UInt32 or ScalarType.UInt64) => true,
		(ScalarType.DateTime, ScalarType.DateOnly or ScalarType.DateTimeOffset or ScalarType.TimeOnly or ScalarType.Int64 or ScalarType.UInt64) => true,
		(ScalarType.DateTimeOffset, ScalarType.DateOnly or ScalarType.DateTime or ScalarType.TimeOnly or ScalarType.Int64 or ScalarType.UInt64) => true,
		(ScalarType.SByte, ScalarType.Char
			or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
			or ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.Int16, ScalarType.Char
			or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
			or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.Int32, ScalarType.DateOnly or ScalarType.Index
			or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
			or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.Int64, ScalarType.DateTime or ScalarType.DateTimeOffset or ScalarType.TimeOnly or ScalarType.TimeSpan
			or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.Int128, ScalarType.UInt128 or ScalarType.BigInteger or ScalarType.Decimal) => true,
		(ScalarType.IntPtr, ScalarType.Int32 or ScalarType.Int64) => true,
		(ScalarType.TimeOnly, ScalarType.TimeSpan or ScalarType.Int64 or ScalarType.UInt64) => true,
		(ScalarType.TimeSpan, ScalarType.TimeOnly or ScalarType.Int64 or ScalarType.UInt64) => true,
		(ScalarType.UInt16, ScalarType.Char
			or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
			or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.UInt32, ScalarType.DateOnly or ScalarType.Index
			or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.UInt64, ScalarType.DateTime or ScalarType.DateTimeOffset or ScalarType.TimeOnly or ScalarType.TimeSpan
			or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger or ScalarType.UInt128 or ScalarType.Double or ScalarType.Decimal) => true,
		(ScalarType.UInt128, ScalarType.Int128 or ScalarType.BigInteger or ScalarType.Decimal) => true,
		(ScalarType.UIntPtr, ScalarType.UInt32 or ScalarType.UInt64) => true,
		_ => false
	};

	public static bool IsDictionary(this CollectionType @this) => @this switch
	{
		CollectionType.Dictionary or CollectionType.ConcurrentDictionary or CollectionType.SortedDictionary
		or CollectionType.ImmutableDictionary or CollectionType.ImmutableSortedDictionary
		or CollectionType.Hashtable or CollectionType.HybridDictionary or CollectionType.OrderedDictionary or CollectionType.ReadOnlyDictionary
		or CollectionType.KeyedCollection or CollectionType.ListDictionary or CollectionType.StringDictionary
		or CollectionType.NameObjectCollection or CollectionType.NameValueCollection or CollectionType.SortedList => true,
		_ => false
	};

	public static bool IsEnumUnderlyingType(this ScalarType @this) => @this switch
	{
		ScalarType.SByte or ScalarType.Byte
		or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64
		or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 => true,
		_ => false
	};

	public static bool IsImmutable(this CollectionType @this) => @this switch
	{
		CollectionType.ImmutableArray
		or CollectionType.ImmutableDictionary or CollectionType.ImmutableSortedDictionary
		or CollectionType.ImmutableSet or CollectionType.ImmutableSortedSet
		or CollectionType.ImmutableList
		or CollectionType.ImmutableQueue
		or CollectionType.ImmutableStack => true,
		_ => false
	};

	public static bool IsPrimitive(this ScalarType @this) => @this switch
	{
		ScalarType.Boolean
		or ScalarType.SByte or ScalarType.Byte
		or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128
		or ScalarType.IntPtr or ScalarType.UIntPtr
		or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
		or ScalarType.Single or ScalarType.Double
		or ScalarType.Char => true,
		_ => false
	};

	public static bool IsQueue(this CollectionType @this) => @this switch
	{
		CollectionType.ConcurrentQueue or CollectionType.ImmutableQueue or CollectionType.PriorityQueue or CollectionType.Queue => true,
		_ => false
	};

	public static bool IsReadOnly(this CollectionType @this) => @this switch
	{
		CollectionType.ReadOnlyCollection or CollectionType.ReadOnlyDictionary or CollectionType.ReadOnlyObservableCollection => true,
		_ => false
	};

	public static bool IsStack(this CollectionType @this) => @this switch
	{
		CollectionType.ConcurrentStack or CollectionType.ImmutableStack or CollectionType.Stack => true,
		_ => false
	};
}
