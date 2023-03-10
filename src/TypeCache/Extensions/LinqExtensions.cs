﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class LinqExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().Any();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Any<T>(this IEnumerable @this)
		=> @this.OfType<T>().Any();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="EnumOf{T}.Comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsEnum<T>(this IEnumerable<T> @this, T value)
		where T : struct, Enum
		=> @this.Contains(value, EnumOf<T>.Comparer);

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out IEnumerable<T> rest)
	{
		using var enumerator = @this.GetEnumerator();
		first = enumerator.Next();
		rest = enumerator.Rest();
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
	{
		using var enumerator = @this.GetEnumerator();
		first = enumerator.Next();
		second = enumerator.Next();
		rest = enumerator.Rest();
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
	{
		using var enumerator = @this.GetEnumerator();
		first = enumerator.Next();
		second = enumerator.Next();
		third = enumerator.Next();
		rest = enumerator.Rest();
	}

	/// <inheritdoc cref="System.Linq.Enumerable.First{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().First();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? First<T>(this IEnumerable @this)
		=> @this.OfType<T>().First();

	/// <inheritdoc cref="System.Linq.Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().FirstOrDefault();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? FirstOrDefault<T>(this IEnumerable @this)
		=> @this.OfType<T>().FirstOrDefault();

	/// <inheritdoc cref="System.Linq.Enumerable.Single{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().Single();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? Single<T>(this IEnumerable @this)
		=> @this.OfType<T>().Single();

	/// <inheritdoc cref="System.Linq.Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().SingleOrDefault();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? SingleOrDefault<T>(this IEnumerable @this)
		=> @this.OfType<T>().SingleOrDefault();

	/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey}?)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Dictionary{TKey, TValue}"/>(@<paramref name="this"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this, IEqualityComparer<TKey>? comparer = null)
		where TKey : notnull
		=> new Dictionary<TKey, TValue>(@this, comparer);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
		where K : notnull
		=> new ReadOnlyDictionary<K, V>(@this);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Select(_ =&gt; _.AsTask());</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<Task> ToTasks(this IEnumerable<ValueTask> @this)
		=> @this.Select(_ => _.AsTask());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Select(_ =&gt; _.AsTask());</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<Task<T>> ToTasks<T>(this IEnumerable<ValueTask<T>> @this)
		=> @this.Select(_ => _.AsTask());

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		var success = enumerator.MoveNext();
		value = success ? enumerator.Current : default;
		return success;
	}

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, Func<T, bool> predicate, [NotNullWhen(true)] out T? value)
	{
		var success = false;
		var filter = new Func<T, bool>(value => success = predicate(value));
		value = @this.FirstOrDefault(filter);
		return success;
	}

	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		value = enumerator.Next();
		return enumerator.MoveNext();
	}
}
