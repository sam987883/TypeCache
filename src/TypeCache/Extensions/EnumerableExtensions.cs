// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
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
		action.ThrowIfNull();

		foreach (var item in @this)
			action(item);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action, Action between)
	{
		between.ThrowIfNull();

		var first = true;
		foreach (var item in @this)
		{
			if (first)
				first = false;
			else
				between();

			action(item);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	public static async Task ForEachAsync<T>(this IEnumerable<T> @this, Action<T> action, CancellationToken token = default)
	{
		@this.ThrowIfNull();
		action.ThrowIfNull();

		foreach (var item in @this)
		{
			if (token.IsCancellationRequested)
			{
				await Task.FromCanceled(token);
				return;
			}

			action(item);
		}

		await Task.CompletedTask;
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ForEachAsync(<see langword="async"/> item =&gt; <see langword="await"/> <paramref name="action"/>(item), <paramref name="token"/>);</c>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Task ForEachAsync<T>(this IEnumerable<T> @this, Func<T, Task> action, CancellationToken token = default)
		=> @this.ForEachAsync(async item => await action(item), token);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ForEachAsync(<see langword="async"/> item =&gt; <see langword="await"/> <paramref name="action"/>(item, <paramref name="token"/>), <paramref name="token"/>);</c>
	/// </remarks>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Task ForEachAsync<T>(this IEnumerable<T> @this, Func<T, CancellationToken, Task> action, CancellationToken token = default)
		=> @this.ForEachAsync(async item => await action(item, token), token);

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

	/// <param name="message">Pass in a custom error message or omit to use a default message.</param>
	/// <param name="logger">Pass a logger to log exception if thrown.</param>
	/// <param name="caller">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <param name="argument">Do not pass any value to this parameter as it will be injected automatically</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEmpty<T>([NotNull] this IEnumerable<T> @this, string? message = null, ILogger? logger = null,
		[CallerMemberName] string? caller = null,
		[CallerArgumentExpression("this")] string? argument = null)
		where T : notnull
	{
		if (!@this.Any())
		{
			var exception = new ArgumentOutOfRangeException(
				paramName: argument,
				actualValue: @this,
				message: message ?? Invariant($"{caller}: {nameof(ThrowIfEmpty)}"));

			logger?.LogError(exception, exception.Message);

			throw exception;
		}
	}

	/// <inheritdoc cref="Dictionary{TKey, TValue}.Dictionary(IEnumerable{KeyValuePair{TKey, TValue}}, IEqualityComparer{TKey}?)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Dictionary{TKey, TValue}"/>(@<paramref name="this"/>, <paramref name="comparer"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this, IEqualityComparer<TKey>? comparer = null)
		where TKey : notnull
		=> new(@this, comparer);

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

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable @this, [NotNullWhen(true)] out T? value)
		where T : class
	{
		value = @this.OfType<T>().FirstOrDefault();
		return value is not null;
	}

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable @this, T defaultValue, [NotNullWhen(true)] out T value)
		where T : struct
	{
		value = @this.OfType<T>().FirstOrDefault(defaultValue);
		return !value.Equals(defaultValue);
	}

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
	{
		using var enumerator = @this.GetEnumerator();
		var success = enumerator.MoveNext();
		value = success ? enumerator.Current : default;
		return success;
	}

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, Func<T, bool> predicate, [NotNullWhen(true)] out T? value)
		where T : class
	{
		value = @this.FirstOrDefault(predicate);
		return value is not null;
	}

	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T> @this, T defaultValue, Func<T, bool> predicate, [NotNullWhen(true)] out T value)
		where T : struct
	{
		value = @this.FirstOrDefault(predicate, defaultValue);
		return !value.Equals(defaultValue);
	}

	/// <exception cref="InvalidOperationException"/>
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, [NotNullWhen(true)] out T? value)
		where T : class
	{
		value = @this.SingleOrDefault();
		return value is not null;
	}

	/// <exception cref="InvalidOperationException"/>
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, T defaultValue, [NotNullWhen(true)] out T? value)
		where T : struct
	{
		value = @this.SingleOrDefault(defaultValue);
		return !value.Equals(defaultValue);
	}

	/// <exception cref="InvalidOperationException"/>
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, Func<T, bool> predicate, [NotNullWhen(true)] out T? value)
		where T : class
	{
		value = @this.SingleOrDefault(predicate);
		return value is not null;
	}

	/// <exception cref="InvalidOperationException"/>
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T> @this, T defaultValue, Func<T, bool> predicate, [NotNullWhen(true)] out T value)
		where T : struct
	{
		value = @this.SingleOrDefault(predicate, defaultValue);
		return !value.Equals(defaultValue);
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
