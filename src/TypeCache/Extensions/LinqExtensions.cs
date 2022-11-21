// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class LinqExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().Any();</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Any<T>(this IEnumerable @this)
		=> @this.OfType<T>().Any();

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Contains(<paramref name="value"/>, <see cref="EnumOf{T}.Comparer"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool ContainsEnum<T>(this IEnumerable<T> @this, T value)
		where T : struct, Enum
		=> @this.Contains(value, EnumOf<T>.Comparer);

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    "/><paramref name="first"/> = enumerator.Next();<br/>
	/// <see langword="    "/><paramref name="rest"/> = enumerator.Rest();<br/>
	/// }
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out IEnumerable<T> rest)
	{
		using var enumerator = @this.GetEnumerator();
		first = enumerator.Next();
		rest = enumerator.Rest();
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    "/><paramref name="first"/> = enumerator.Next();<br/>
	/// <see langword="    "/><paramref name="second"/> = enumerator.Next();<br/>
	/// <see langword="    "/><paramref name="rest"/> = enumerator.Rest();<br/>
	/// }
	/// </code>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
	{
		using var enumerator = @this.GetEnumerator();
		first = enumerator.Next();
		second = enumerator.Next();
		rest = enumerator.Rest();
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    "/><paramref name="first"/> = enumerator.Next();<br/>
	/// <see langword="    "/><paramref name="second"/> = enumerator.Next();<br/>
	/// <see langword="    "/><paramref name="third"/> = enumerator.Next();<br/>
	/// <see langword="    "/><paramref name="rest"/> = enumerator.Rest();<br/>
	/// }
	/// </code>
	/// </summary>
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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? First<T>(this IEnumerable @this)
		=> @this.OfType<T>().First();

	/// <inheritdoc cref="System.Linq.Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().FirstOrDefault();</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? FirstOrDefault<T>(this IEnumerable @this)
		=> @this.OfType<T>().FirstOrDefault();

	/// <inheritdoc cref="System.Linq.Enumerable.Single{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().Single();</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? Single<T>(this IEnumerable @this)
		=> @this.OfType<T>().Single();

	/// <inheritdoc cref="System.Linq.Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.OfType&lt;<typeparamref name="T"/>&gt;().SingleOrDefault();</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? SingleOrDefault<T>(this IEnumerable @this)
		=> @this.OfType<T>().SingleOrDefault();

	/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey}?)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Dictionary{TKey, TValue}"/>(@<paramref name="this"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this, IEqualityComparer<TKey>? comparer = null)
		where TKey : notnull
		=> new Dictionary<TKey, TValue>(@this, comparer);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> ReadOnlyDictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> @this)
		where K : notnull
		=> new ReadOnlyDictionary<K, V>(@this);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Select(_ =&gt; _.AsTask());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<Task> ToTasks(this IEnumerable<ValueTask> @this)
		=> @this.Select(_ => _.AsTask());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Select(_ =&gt; _.AsTask());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<Task<T>> ToTasks<T>(this IEnumerable<ValueTask<T>> @this)
		=> @this.Select(_ => _.AsTask());

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    var"/> success = enumerator.MoveNext();<br/>
	/// <see langword="    "/>value = success ? enumerator.Current : <see langword="default"/>;<br/>
	/// <see langword="    return"/> success;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		var success = enumerator.MoveNext();
		value = success ? enumerator.Current : default;
		return success;
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> success = <see langword="false"/>;<br/>
	/// <see langword="    var"/> filter = <see langword="new"/> Func&lt;<typeparamref name="T"/>, <see cref="bool"/>&gt;(value =&gt; success = <paramref name="predicate"/>(value));<br/>
	/// <see langword="    "/>value = @<paramref name="this"/>.FirstOrDefault(filter);<br/>
	/// <see langword="    return"/> success;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, Func<T, bool> predicate, [NotNullWhen(true)] out T? value)
	{
		var success = false;
		var filter = new Func<T, bool>(value => success = predicate(value));
		value = @this.FirstOrDefault(filter);
		return success;
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    "/>value = enumerator.Next();<br/>
	/// <see langword="    return"/> enumerator.MoveNext();<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		value = enumerator.Next();
		return enumerator.MoveNext();
	}
}
