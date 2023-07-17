// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using TypeCache.Collections;
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
	/// <c>=&gt; @<paramref name="this"/> <see langword="as"/> <typeparamref name="T"/>[] ?? @<paramref name="this"/>?.ToArray() ?? <see cref="Array{T}.Empty"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T[] AsArray<T>(this IEnumerable<T>? @this)
		=> @this as T[] ?? @this?.ToArray() ?? Array<T>.Empty;

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="as"/> <see cref="HashSet{T}"/> ?? @<paramref name="this"/>?.ToHashSet() ?? <see langword="new"/> <see cref="HashSet{T}"/>(0);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static HashSet<T> AsHashSet<T>(this IEnumerable<T>? @this)
		=> @this as HashSet<T> ?? @this?.ToHashSet() ?? new HashSet<T>(0);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="as"/> <see cref="List{T}"/> ?? @<paramref name="this"/>?.ToList() ?? <see langword="new"/> <see cref="List{T}"/>(0);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static List<T> AsList<T>(this IEnumerable<T>? @this)
		=> @this as List<T> ?? @this?.ToList() ?? new List<T>(0);

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

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action action)
		where K : notnull
	{
		@this.AssertNotNull();

		var success = @this.ContainsKey(key);
		if (success)
			action();

		return success;
	}

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action<V> action)
		where K : notnull
	{
		@this.AssertNotNull();

		var success = @this.TryGetValue(key, out var value);
		if (success)
			action(value!);

		return success;
	}

	/// <exception cref="ArgumentNullException"/>
	public static bool If<K, V>(this IDictionary<K, V> @this, K key, Action<K, V> action)
		where K : notnull
	{
		@this.AssertNotNull();

		var success = @this.TryGetValue(key, out var value);
		if (success)
			action(key, value!);

		return success;
	}

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

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="condition"/> ? @<paramref name="this"/>.Where(<paramref name="predicate"/>) : <paramref name="this"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> @this, bool condition, Func<T, bool> predicate)
		=> condition ? @this.Where(predicate) : @this;

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="condition"/> ? @<paramref name="this"/>.Where(<paramref name="predicate"/>) : <paramref name="this"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> @this, bool condition, Func<T, int, bool> predicate)
		=> condition ? @this.Where(predicate) : @this;

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Where((_ =&gt; _ <see langword="is not null"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> @this)
		=> @this.Where(_ => _ is not null)!;
}
