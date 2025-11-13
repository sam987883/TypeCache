// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Collections;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class EnumerableExtensions
{
	extension<T>([NotNullWhen(true)] IEnumerable<T> @this)
	{
		/// <remarks>
		/// <c>=&gt; @this <see langword="as"/> <typeparamref name="T"/>[] ?? @this?.ToArray() ?? <see cref="Array{T}.Empty"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T[] AsArray()
			=> @this as T[] ?? @this?.ToArray() ?? Array<T>.Empty;

		/// <remarks>
		/// <c>=&gt; @this <see langword="as"/> <see cref="ISet{T}"/> ?? @this?.ToHashSet(<paramref name="comparer"/>) ?? <see langword="new"/> <see cref="HashSet{T}"/>(0, <paramref name="comparer"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ISet<T> AsHashSet(IEqualityComparer<T>? comparer = null)
			=> @this as ISet<T> ?? @this?.ToHashSet(comparer) ?? new HashSet<T>(0, comparer);

		/// <remarks>
		/// <c>=&gt; @this <see langword="as"/> <see cref="IList{T}"/> ?? @this?.ToList() ?? <see langword="new"/> <see cref="List{T}"/>(0);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IList<T> AsList()
			=> @this as IList<T> ?? @this?.ToList() ?? new List<T>(0);

		public void Deconstruct(out T? first, out IEnumerable<T> rest)
		{
			using var enumerator = @this.GetEnumerator();
			first = enumerator.Next();
			rest = enumerator.Rest();
		}

		public void Deconstruct(out T? first, out T? second, out IEnumerable<T> rest)
		{
			using var enumerator = @this.GetEnumerator();
			first = enumerator.Next();
			second = enumerator.Next();
			rest = enumerator.Rest();
		}

		public void Deconstruct(out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		{
			using var enumerator = @this.GetEnumerator();
			first = enumerator.Next();
			second = enumerator.Next();
			third = enumerator.Next();
			rest = enumerator.Rest();
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T> action)
		{
			@this.ThrowIfNull();
			action.ThrowIfNull();

			foreach (var item in @this)
				action(item);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T, int> action)
		{
			@this.ThrowIfNull();
			action.ThrowIfNull();

			var i = -1;
			foreach (var item in @this)
				action(item, ++i);
		}

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<T> action, Action between)
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
		public void ForEach(Action<T, int> action, Action between)
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

		/// <inheritdoc cref="Enumerable.ToDictionary{TKey, TValue}(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; @this.ToDictionary(<paramref name="keySelector"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<string, T> ToDictionaryIgnoreCase(Func<T, string> keySelector)
			=> @this.ToDictionary(keySelector, StringComparer.OrdinalIgnoreCase);

		/// <inheritdoc cref="Enumerable.ToDictionary{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; @this.ToDictionary(<paramref name="keySelector"/>, <paramref name="valueSelector"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<string, V> ToDictionaryIgnoreCase<V>(Func<T, string> keySelector, Func<T, V> valueSelector)
			=> @this.ToDictionary(keySelector, valueSelector, StringComparer.OrdinalIgnoreCase);

		/// <inheritdoc cref="Enumerable.ToDictionary{TKey, TValue}(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; @this.ToDictionary(<paramref name="keySelector"/>, <see cref="StringComparer.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<string, T> ToDictionaryOrdinal(Func<T, string> keySelector)
			=> @this.ToDictionary(keySelector, StringComparer.Ordinal);

		/// <inheritdoc cref="Enumerable.ToDictionary{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; @this.ToDictionary(<paramref name="keySelector"/>, <paramref name="valueSelector"/>, <see cref="StringComparer.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<string, V> ToDictionaryOrdinal<V>(Func<T, string> keySelector, Func<T, V> valueSelector)
			=> @this.ToDictionary(keySelector, valueSelector, StringComparer.Ordinal);

		public bool TryFirst([NotNullWhen(true)] out T? value)
		{
			using var enumerator = @this.GetEnumerator();
			return enumerator.IfNext(out value);
		}

		public bool TrySingle([NotNullWhen(true)] out T? value)
		{
			using var enumerator = @this.GetEnumerator();
			return enumerator.IfNext(out value) && !enumerator.MoveNext();
		}

		/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		/// <remarks>
		/// <c>=&gt; <paramref name="condition"/> ? @this.Where(<paramref name="predicate"/>) : this;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IEnumerable<T> WhereIf(bool condition, Func<T, bool> predicate)
			=> condition ? @this.Where(predicate) : @this;

		/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
		/// <remarks>
		/// <c>=&gt; <paramref name="condition"/> ? @this.Where(<paramref name="predicate"/>) : this;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IEnumerable<T> WhereIf(bool condition, Func<T, int, bool> predicate)
			=> condition ? @this.Where(predicate) : @this;
	}

	extension<T>(IEnumerable<T?> @this) where T : class
	{
		/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		/// <remarks>
		/// <c>=&gt; @this.Where(_ =&gt; _ <see langword="is not null"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IEnumerable<T> WhereNotNull()
			=> @this.Where(_ => _ is not null)!;
	}

	extension<T>(IEnumerable<T?> @this) where T : struct
	{
		/// <remarks>
		/// <c>=&gt; @this.Where(_ =&gt; _.HasValue).Select(_ =&gt; _.Value);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IEnumerable<T> WhereHasValue()
			=> @this.Where(_ => _.HasValue).Select(_ => _!.Value);
	}

	extension<T>(IEnumerable<T> @this) where T : struct, Enum
	{
		/// <remarks>
		/// <c>=&gt; @this.Contains(<paramref name="value"/>, <see langword="new"/> <see cref="EnumComparer{T}"/>());</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsEnum(T value)
			=> @this.Contains(value, new EnumComparer<T>());
	}

	extension(IEnumerable<string> @this)
	{
		/// <inheritdoc cref="string.Concat(IEnumerable{string})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Concat(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Concat()
			=> string.Concat(@this);

		/// <remarks>
		/// <c>=&gt; @this.Contains(<paramref name="value"/>, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool ContainsIgnoreCase(string value)
			=> @this.Contains(value, StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Determines if collection contains multiple strings with the same value but different case.<br/>
		/// For example: <c>FirstName</c> and <c>firstName</c>
		/// </summary>
		/// <remarks>
		/// <c>=&gt; @this.ToHashSet(<see cref="StringComparer.Ordinal"/>).Count != @this.ToHashSet(<see cref="StringComparer.OrdinalIgnoreCase"/>).Count;</c>
		/// </remarks>
		public bool IsCaseSensitive()
			=> @this.ToHashSet(StringComparer.Ordinal).Count != @this.ToHashSet(StringComparer.OrdinalIgnoreCase).Count;
	}

	extension(IEnumerable<char> @this)
	{
		/// <inheritdoc cref="string.Concat{T}(IEnumerable{T})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="string"/>.Concat(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string Concat()
			=> string.Concat(@this);
	}

	extension<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> @this) where TKey : notnull
	{
		/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this, <paramref name="comparer"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<TKey, TValue> ToDictionary(IEqualityComparer<TKey>? comparer = null)
			=> new(@this, comparer);
	}

	extension<T>(IEnumerable<KeyValuePair<string, T>> @this)
	{
		/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this, <see cref="StringComparer.OrdinalIgnoreCase"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<string, T> ToDictionaryIgnoreCase()
			=> new(@this, StringComparer.OrdinalIgnoreCase);

		/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey})"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this, <see cref="StringComparer.Ordinal"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Dictionary<string, T> ToDictionaryOrdinal()
			=> new(@this, StringComparer.Ordinal);
	}

	extension([NotNullWhen(true)] IEnumerable @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.OfType&lt;<typeparamref name="T"/>&gt;().Any();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Any<T>()
			=> @this.OfType<T>().Any();

		/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource})"/>
		/// <remarks>
		/// <c>=&gt; @this.OfType&lt;<typeparamref name="T"/>&gt;().First();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T? First<T>()
			=> @this.OfType<T>().First();

		/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
		/// <remarks>
		/// <c>=&gt; @this.OfType&lt;<typeparamref name="T"/>&gt;().FirstOrDefault();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T? FirstOrDefault<T>()
			=> @this.OfType<T>().FirstOrDefault();

		/// <inheritdoc cref="Enumerable.Single{TSource}(IEnumerable{TSource})"/>
		/// <remarks>
		/// <c>=&gt; @this.OfType&lt;<typeparamref name="T"/>&gt;().Single();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T? Single<T>()
			=> @this.OfType<T>().Single();

		/// <inheritdoc cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>
		/// <remarks>
		/// <c>=&gt; @this.OfType&lt;<typeparamref name="T"/>&gt;().SingleOrDefault();</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T? SingleOrDefault<T>()
			=> @this.OfType<T>().SingleOrDefault();

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TryFirst<T>([NotNullWhen(true)] out T? value)
			=> @this.OfType<T>().TryFirst(out value);

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool TrySingle<T>([NotNullWhen(true)] out T? value)
			=> @this.OfType<T>().TrySingle(out value);
	}
}
