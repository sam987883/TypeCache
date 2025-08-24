// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TypeCache.Collections;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class EnumerableExtensions
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
	/// <c>=&gt; @<paramref name="this"/> <see langword="as"/> <see cref="ISet{T}"/> ?? @<paramref name="this"/>?.ToHashSet(<paramref name="comparer"/>) ?? <see langword="new"/> <see cref="HashSet{T}"/>(0, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ISet<T> AsHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T>? comparer = null)
		=> @this as ISet<T> ?? @this?.ToHashSet(comparer) ?? new HashSet<T>(0, comparer);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="as"/> <see cref="IList{T}"/> ?? @<paramref name="this"/>?.ToList() ?? <see langword="new"/> <see cref="List{T}"/>(0);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IList<T> AsList<T>(this IEnumerable<T>? @this)
		=> @this as IList<T> ?? @this?.ToList() ?? new List<T>(0);

	/// <inheritdoc cref="string.Concat(IEnumerable{string})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Concat(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Concat(this IEnumerable<string> @this)
		=> string.Concat(@this);

	/// <inheritdoc cref="string.Concat{T}(IEnumerable{T})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="string"/>.Concat(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string Concat(this IEnumerable<char> @this)
		=> string.Concat(@this);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsIgnoreCase(this IEnumerable<string> @this, string value)
		=> @this.Contains(value, StringComparer.OrdinalIgnoreCase);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see langword="new"/> <see cref="EnumComparer{T}"/>());</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool ContainsEnum<T>(this IEnumerable<T> @this, T value)
		where T : struct, Enum
		=> @this.Contains(value, new EnumComparer<T>());

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

	/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().First();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? First<T>(this IEnumerable @this)
		=> @this.OfType<T>().First();

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().FirstOrDefault();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? FirstOrDefault<T>(this IEnumerable @this)
		=> @this.OfType<T>().FirstOrDefault();

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
	{
		@this.ThrowIfNull();
		action.ThrowIfNull();

		foreach (var item in @this)
			action(item);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T, int> action)
	{
		@this.ThrowIfNull();
		action.ThrowIfNull();

		var i = -1;
		foreach (var item in @this)
			action(item, ++i);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action, Action between)
	{
		@this.ThrowIfNull();
		between.ThrowIfNull();

		using var enumerator = @this.GetEnumerator();

		if (!enumerator.MoveNext())
			return;

		action(enumerator.Current);

		while (enumerator.MoveNext())
		{
			between();
			action(enumerator.Current);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T, int> action, Action between)
	{
		@this.ThrowIfNull();
		between.ThrowIfNull();

		using var enumerator = @this.GetEnumerator();

		if (!enumerator.MoveNext())
			return;

		action(enumerator.Current, 0);

		var i = 0;
		while (enumerator.MoveNext())
		{
			between();
			action(enumerator.Current, ++i);
		}
	}

	/// <summary>
	/// Determines if collection contains multiple strings with the same value but different case.<br/>
	/// For example: <c>FirstName</c> and <c>firstName</c>
	/// </summary>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSet(<see cref="StringComparer.Ordinal"/>).Count != @<paramref name="this"/>.ToHashSet(<see cref="StringComparer.OrdinalIgnoreCase"/>).Count;</c>
	/// </remarks>
	public static bool IsCaseSensitive(this IEnumerable<string> @this)
		=> @this.ToHashSet(StringComparer.Ordinal).Count != @this.ToHashSet(StringComparer.OrdinalIgnoreCase).Count;

	/// <inheritdoc cref="Enumerable.Single{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().Single();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? Single<T>(this IEnumerable @this)
		=> @this.OfType<T>().Single();

	/// <inheritdoc cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().SingleOrDefault();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T? SingleOrDefault<T>(this IEnumerable @this)
		=> @this.OfType<T>().SingleOrDefault();

	/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Dictionary{TKey, TValue}"/>(@<paramref name="this"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this, IEqualityComparer<TKey>? comparer = null)
		where TKey : notnull
		=> new(@this, comparer);

	/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<string, T> ToDictionaryIgnoreCase<T>(this IEnumerable<KeyValuePair<string, T>> @this)
		=> new(@this, StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc cref="Enumerable.ToDictionary{TKey, TValue}(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionary(<paramref name="keySelector"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<string, T> ToDictionaryIgnoreCase<T>(this IEnumerable<T> @this, Func<T, string> keySelector)
		=> @this.ToDictionary(keySelector, StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc cref="Enumerable.ToDictionary{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionary(<paramref name="keySelector"/>, <paramref name="valueSelector"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<string, V> ToDictionaryIgnoreCase<T, V>(this IEnumerable<T> @this, Func<T, string> keySelector, Func<T, V> valueSelector)
		=> @this.ToDictionary(keySelector, valueSelector, StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>, <see cref="StringComparer.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<string, T> ToDictionaryOrdinal<T>(this IEnumerable<KeyValuePair<string, T>> @this)
		=> new(@this, StringComparer.Ordinal);

	/// <inheritdoc cref="Enumerable.ToDictionary{TKey, TValue}(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionary(<paramref name="keySelector"/>, <see cref="StringComparer.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<string, T> ToDictionaryOrdinal<T>(this IEnumerable<T> @this, Func<T, string> keySelector)
		=> @this.ToDictionary(keySelector, StringComparer.Ordinal);

	/// <inheritdoc cref="Enumerable.ToDictionary{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToDictionary(<paramref name="keySelector"/>, <paramref name="valueSelector"/>, <see cref="StringComparer.Ordinal"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<string, V> ToDictionaryOrdinal<T, V>(this IEnumerable<T> @this, Func<T, string> keySelector, Func<T, V> valueSelector)
		=> @this.ToDictionary(keySelector, valueSelector, StringComparer.Ordinal);

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

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable @this, [NotNullWhen(true)] out T? value)
		=> @this.OfType<T>().TryFirst(out value);

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		return enumerator.IfNext(out value);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, Func<T, bool> predicate, [NotNullWhen(true)] out T? value)
		=> @this.Where(predicate).TryFirst(out value);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable @this, [NotNullWhen(true)] out T? value)
		=> @this.OfType<T>().TrySingle(out value);

	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		return enumerator.IfNext(out value) && !enumerator.MoveNext();
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, Func<T, bool> predicate, [NotNullWhen(true)] out T? value)
		=> @this.Where(predicate).TrySingle(out value);

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

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Where(_ =&gt; _.HasValue).Select(_ =&gt; _.Value);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> WhereHasValue<T>(this IEnumerable<T?> @this)
		where T : struct
		=> @this.Where(_ => _.HasValue).Select(_ => _!.Value);

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Where(_ =&gt; _ <see langword="is not null"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> @this)
		where T : class
		=> @this.Where(_ => _ is not null)!;
}
