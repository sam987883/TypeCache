﻿// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Utilities;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static Attribute[] Attributes<T>(this T @this)
		where T : struct, Enum
		=> Enum<T>.IsValid(@this)
			? typeof(T).GetField(@this.Name(), Public | Static)!.GetCustomAttributes(false).Cast<Attribute>().ToArray()
			: Array<Attribute>.Empty;

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
	/// <c>=&gt; <see cref="Enum"/>.IsDefined(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsDefined<T>(this T @this)
		where T : struct, Enum
		=> Enum.IsDefined(@this);

	/// <remarks>
	/// <c>=&gt; <see cref="Enum"/>&lt;<typeparamref name="T"/>&gt;.IsValid(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool IsValid<T>(this T @this)
		where T : struct, Enum
		=> Enum<T>.IsValid(@this);

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

	public static bool IsConvertibleTo(this ScalarType @this, ScalarType target)
		=> (@this, target) switch
		{
			_ when target == @this => true,
			(ScalarType.String, _) or (_, ScalarType.Boolean or ScalarType.String) => true,
			(ScalarType.Boolean, ScalarType.Char or ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128
				or ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128) => true,
			(ScalarType.Byte, ScalarType.Boolean or ScalarType.Char
				or ScalarType.SByte or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
				or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
				or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.Char, ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
				or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
				or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.DateOnly, ScalarType.DateTime or ScalarType.DateTimeOffset or ScalarType.TimeSpan
				or ScalarType.Int32 or ScalarType.Int64 or ScalarType.UInt32 or ScalarType.UInt64) => true,
			(ScalarType.DateTime, ScalarType.DateOnly or ScalarType.DateTimeOffset or ScalarType.TimeOnly or ScalarType.Int64 or ScalarType.UInt64) => true,
			(ScalarType.DateTimeOffset, ScalarType.DateOnly or ScalarType.DateTime or ScalarType.TimeOnly or ScalarType.Int64 or ScalarType.UInt64) => true,
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
			(ScalarType.SByte, ScalarType.Char
				or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger
				or ScalarType.Byte or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
				or ScalarType.Half or ScalarType.Single or ScalarType.Double or ScalarType.Decimal) => true,
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
				or ScalarType.Int64 or ScalarType.Int128 or ScalarType.BigInteger or ScalarType.UInt128
				or ScalarType.Double or ScalarType.Decimal) => true,
			(ScalarType.UInt128, ScalarType.Int128 or ScalarType.BigInteger or ScalarType.Decimal) => true,
			(ScalarType.UIntPtr, ScalarType.UInt32 or ScalarType.UInt64) => true,
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
			ScalarType.SByte or ScalarType.Byte
			or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64
			or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 => true,
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
	/// Returns true for the current .Net primitives.<br/>
	/// In addition, returns true for the following types:
	/// <list type="bullet">
	/// <item><c><see cref="ScalarType.Decimal"/></c></item>
	/// <item><c><see cref="ScalarType.Int128"/></c></item>
	/// <item><c><see cref="ScalarType.UInt128"/></c></item>
	/// </list>
	/// </summary>
	public static bool IsPrimitive(this ScalarType @this)
		=> @this switch
		{
			ScalarType.Boolean
			or ScalarType.SByte or ScalarType.Byte
			or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64 or ScalarType.Int128
			or ScalarType.IntPtr or ScalarType.UIntPtr
			or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64 or ScalarType.UInt128
			or ScalarType.Single or ScalarType.Double or ScalarType.Decimal
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
			CollectionType.ReadOnlyCollection or CollectionType.ReadOnlyDictionary or CollectionType.ReadOnlyObservableCollection => true,
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
