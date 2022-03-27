// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections.Extensions;

public static class EnumerableExtensions
{
	/// <summary>
	/// <code>
	/// <paramref name="aggregator"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> result = <paramref name="initialValue"/>;<br/>
	/// @<paramref name="this"/>.Do(item =&gt; result = <paramref name="aggregator"/>(result, item));<br/>
	/// <see langword="return"/> result;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T Aggregate<T>(this IEnumerable<T>? @this, T initialValue, Func<T, T, T> aggregator)
	{
		aggregator.AssertNotNull();

		var result = initialValue;
		@this.Do(item => result = aggregator(result, item));
		return result;
	}

	/// <summary>
	/// <code>
	/// <paramref name="aggregator"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> result = <paramref name="initialValue"/>;<br/>
	/// <see langword="await"/> @<paramref name="this"/>.DoAsync(<see langword="async"/> item =&gt; result = <see langword="await"/> <paramref name="aggregator"/>(result, item));<br/>
	/// <see langword="return"/> result;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask<T> AggregateAsync<T>(this IEnumerable<T>? @this, T initialValue, Func<T, T, ValueTask<T>> aggregator)
	{
		aggregator.AssertNotNull();

		var result = initialValue;
		await @this.DoAsync(async item => result = await aggregator(result, item));
		return result;
	}

	/// <summary>
	/// <code>
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> !@<paramref name="this"/>.If(item =&gt; !<paramref name="filter"/>(item)).Any();
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static bool All<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		return !@this.If(item => !filter(item)).Any();
	}

	/// <summary>
	/// <c>=&gt; <see langword="await"/> <see cref="Task"/>.WhenAll(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask AllAsync<T>(this IEnumerable<Task> @this)
		=> await Task.WhenAll(@this);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any()
	/// ? <see langword="await"/> <see cref="Task"/>.WhenAll(@<paramref name="this"/>)
	/// : <see langword="await"/> <see cref="Task"/>.FromResult(<see cref="Array{T}.Empty"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask<T[]> AllAsync<T>(this IEnumerable<Task<T>>? @this)
		=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array<T>.Empty);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().Any()</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable? @this)
		=> @this.If<T>().Any();

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.TryCount(<see langword="out var"/> count) =&gt; count &gt; 0,<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Any(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
		=> @this switch
		{
			null => false,
			_ when @this.TryCount(out var count) => count > 0,
			_ => Enumerable<T>.Any(@this)
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).Any();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter)
		=> @this.If(filter).Any();

	/// <summary>
	/// <c>=&gt; <see langword="await"/> <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask AnyAsync<T>(this IEnumerable<Task> @this)
		=> await Task.WhenAny(@this);

	/// <summary>
	/// <c>=&gt; <see langword="await await"/> <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static async ValueTask<T> AnyAsync<T>(this IEnumerable<Task<T>> @this)
		=> await await Task.WhenAny(@this);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; !value.HasValue);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool AnyNull<T>([NotNullWhen(true)] this IEnumerable<T?>? @this)
		where T : struct
		=> @this.Any(value => !value.HasValue);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value <see langword="is null"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool AnyNull<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
		where T : class
		=> @this.Any(value => value is null);

	/// <summary>
	/// <c>=&gt; <see langword="new"/>[] { @<paramref name="this"/>, <paramref name="items"/> }.Gather();</c>
	/// </summary>
	public static IEnumerable<T> Append<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		=> new[] { @this, items }.Gather();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.And(<paramref name="sets"/>.Gather());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> Append<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? sets)
		=> @this.Append(sets.Gather());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.And(<paramref name="items"/> <see langword="as"/> <see cref="IEnumerable{T}"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> Append<T>(this IEnumerable<T>? @this, params T[]? items)
		=> @this.Append(items as IEnumerable<T>);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}.Empty"/>,<br/>
	///	<see langword="    "/><see cref="IEnumerable{T}"/> enumerable =&gt; enumerable,<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.As(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static IEnumerable<T?> As<T>(this IEnumerable? @this)
		=> @this switch
		{
			null => Enumerable<T>.Empty,
			IEnumerable<T> enumerable => enumerable,
			_ => Enumerable<T>.As(@this)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.TryCount(<see langword="out var"/> collectionCount) =&gt; <paramref name="count"/> &gt;= collectionCount,<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.GetEnumerator().Move(<paramref name="count"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool AtLeast<T>([NotNullWhen(true)] this IEnumerable<T>? @this, int count)
		=> @this switch
		{
			null => false,
			_ when @this.TryCount(out var collectionCount) => count >= collectionCount,
			_ => @this.GetEnumerator().Move(count)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.TryCount(<see langword="out var"/> collectionCount) =&gt; <paramref name="count"/> &lt;= collectionCount,<br/>
	/// <see langword="    "/>_ =&gt; !@<paramref name="this"/>.GetEnumerator().Move(<paramref name="count"/> + 1)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool AtMost<T>([NotNullWhen(true)] this IEnumerable<T>? @this, int count)
		=> @this switch
		{
			null => false,
			_ when @this.TryCount(out var collectionCount) => count <= collectionCount,
			_ => !@this.GetEnumerator().Move(count + 1)
		};

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<paramref name="tuple"/>.Item1 <see langword="is null"/> || tuple.Item2 <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="using var"/> enumerator1 = <paramref name="tuple"/>.Item1.GetEnumerator();<br/>
	/// <see langword="using var"/> enumerator2 = <paramref name="tuple"/>.Item2.GetEnumerator();<br/>
	/// <see langword="while"/> (enumerator1.TryNext(<see langword="out var"/> item1) &amp;&amp; enumerator2.TryNext(<see langword="out var"/> item2))<br/>
	/// <see langword="    yield return"/> (item1, item2);<br/>
	/// </code>
	/// </summary>
	public static IEnumerable<(A, B)> Combine<A, B>((IEnumerable<A>, IEnumerable<B>) tuple)
	{
		if (tuple.Item1 is null || tuple.Item2 is null)
			yield break;

		using var enumerator1 = tuple.Item1.GetEnumerator();
		using var enumerator2 = tuple.Item2.GetEnumerator();
		while (enumerator1.TryNext(out var item1) && enumerator2.TryNext(out var item2))
			yield return (item1, item2);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<paramref name="tuple"/>.Item1 <see langword="is null"/> || tuple.Item2 <see langword="is null"/> || tuple.Item3 <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="using var"/> enumerator1 = <paramref name="tuple"/>.Item1.GetEnumerator();<br/>
	/// <see langword="using var"/> enumerator2 = <paramref name="tuple"/>.Item2.GetEnumerator();<br/>
	/// <see langword="using var"/> enumerator3 = <paramref name="tuple"/>.Item3.GetEnumerator();<br/>
	/// <see langword="while"/> (enumerator1.TryNext(<see langword="out var"/> item1) &amp;&amp; enumerator2.TryNext(<see langword="out var"/> item2) &amp;&amp; enumerator3.TryNext(<see langword="out var"/> item3))<br/>
	/// <see langword="    yield return"/> (item1, item2, item3);<br/>
	/// </code>
	/// </summary>
	public static IEnumerable<(A, B, C)> Combine<A, B, C>((IEnumerable<A>, IEnumerable<B>, IEnumerable<C>) tuple)
	{
		if (tuple.Item1 is null || tuple.Item2 is null || tuple.Item3 is null)
			yield break;

		using var enumerator1 = tuple.Item1.GetEnumerator();
		using var enumerator2 = tuple.Item2.GetEnumerator();
		using var enumerator3 = tuple.Item3.GetEnumerator();
		while (enumerator1.TryNext(out var item1) && enumerator2.TryNext(out var item2) && enumerator3.TryNext(out var item3))
			yield return (item1, item2, item3);
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; 0,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.TryCount(<see langword="out var"/> count) =&gt; count,<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Count(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static int Count<T>(this IEnumerable<T>? @this)
		=> @this switch
		{
			null => 0,
			_ when @this.TryCount(out var count) => count,
			_ => Enumerable<T>.Count(@this)
		};

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.If(_ =&gt; <paramref name="comparer"/>.Equals(_, item)).Count();
	/// </code>
	/// </summary>
	public static int CountOf<T>(this IEnumerable<T>? @this, T item, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return @this.If(_ => comparer.Equals(_, item)).Count();
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out IEnumerable<T> rest)
		where T : struct
	{
		switch (@this)
		{
			case null:
				(first, rest) = (null, Enumerable<T>.Empty);
				return;
			case T[] array:
				(first, rest) = array;
				return;
			case ImmutableArray<T> immutableArray:
				(first, rest) = immutableArray;
				return;
			case List<T> list:
				(first, rest) = list;
				return;
			default:
				(first, rest) = @this.GetEnumerator();
				return;
		}
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out IEnumerable<T> rest)
		where T : class
	{
		switch (@this)
		{
			case null:
				(first, rest) = (null, Enumerable<T>.Empty);
				return;
			case T[] array:
				(first, rest) = array;
				return;
			case ImmutableArray<T> immutableArray:
				(first, rest) = immutableArray;
				return;
			case List<T> list:
				(first, rest) = list;
				return;
			default:
				(first, rest) = @this.GetEnumerator();
				return;
		}
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
		where T : struct
	{
		switch (@this)
		{
			case null:
				(first, second, rest) = (null, null, Enumerable<T>.Empty);
				return;
			case T[] array:
				(first, second, rest) = array;
				return;
			case ImmutableArray<T> immutableArray:
				(first, second, rest) = immutableArray;
				return;
			case List<T> list:
				(first, second, rest) = list;
				return;
			default:
				(first, second, rest) = @this.GetEnumerator();
				return;
		}
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
		where T : class
	{
		switch (@this)
		{
			case null:
				(first, second, rest) = (null, null, Enumerable<T>.Empty);
				return;
			case T[] array:
				(first, second, rest) = array;
				return;
			case ImmutableArray<T> immutableArray:
				(first, second, rest) = immutableArray;
				return;
			case List<T> list:
				(first, second, rest) = list;
				return;
			default:
				(first, second, rest) = @this.GetEnumerator();
				return;
		}
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		where T : struct
	{
		switch (@this)
		{
			case null:
				(first, second, third, rest) = (null, null, null, Enumerable<T>.Empty);
				return;
			case T[] array:
				(first, second, third, rest) = array;
				return;
			case ImmutableArray<T> immutableArray:
				(first, second, third, rest) = immutableArray;
				return;
			case List<T> list:
				(first, second, third, rest) = list;
				return;
			default:
				(first, second, third, rest) = @this.GetEnumerator();
				return;
		}
	}

	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		where T : class
	{
		switch (@this)
		{
			case null:
				(first, second, third, rest) = (null, null, null, Enumerable<T>.Empty);
				return;
			case T[] array:
				(first, second, third, rest) = array;
				return;
			case ImmutableArray<T> immutableArray:
				(first, second, third, rest) = immutableArray;
				return;
			case List<T> list:
				(first, second, third, rest) = list;
				return;
			default:
				(first, second, third, rest) = @this.GetEnumerator();
				return;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> T[] array:<br/>
	/// <see langword="        "/>array.Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="List{T}"/> list:<br/>
	/// <see langword="        "/>list.Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    default"/>:<br/>
	/// <see langword="        "/><see cref="Enumerable{T}"/>.Do(@<paramref name="this"/>, <paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Action<T> action)
	{
		switch (@this)
		{
			case null:
				return;
			case T[] array:
				array.Do(action);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.Do(action);
				return;
			case List<T> list:
				list.Do(action);
				return;
			default:
				Enumerable<T>.Do(@this, action);
				return;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> T[] array:<br/>
	/// <see langword="        "/>array.Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="List{T}"/> list:<br/>
	/// <see langword="        "/>list.Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    default"/>:<br/>
	/// <see langword="        "/><see cref="Enumerable{T}"/>.Do(@<paramref name="this"/>, <paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Action<T> action, Action between)
	{
		switch (@this)
		{
			case null:
				return;
			case T[] array:
				array.Do(action, between);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.Do(action, between);
				return;
			case List<T> list:
				list.Do(action, between);
				return;
			default:
				Enumerable<T>.Do(@this, action, between);
				return;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> T[] array:<br/>
	/// <see langword="        "/>array.Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="List{T}"/> list:<br/>
	/// <see langword="        "/>list.Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    default"/>:<br/>
	/// <see langword="        "/><see cref="Enumerable{T}"/>.Do(@<paramref name="this"/>, <paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Action<T, int> action)
	{
		switch (@this)
		{
			case null:
				return;
			case T[] array:
				array.Do(action);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.Do(action);
				return;
			case List<T> list:
				list.Do(action);
				return;
			default:
				Enumerable<T>.Do(@this, action);
				return;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> T[] array:<br/>
	/// <see langword="        "/>array.Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="List{T}"/> list:<br/>
	/// <see langword="        "/>list.Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    default"/>:<br/>
	/// <see langword="        "/><see cref="Enumerable{T}"/>.Do(@<paramref name="this"/>, <paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Action<T, int> action, Action between)
	{
		switch (@this)
		{
			case null:
				return;
			case T[] array:
				array.Do(action, between);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.Do(action, between);
				return;
			case List<T> list:
				list.Do(action, between);
				return;
			default:
				Enumerable<T>.Do(@this, action, between);
				return;
		}
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/><see cref="Task"/>.WaitAll(@<paramref name="this"/>.To(<paramref name="action"/>).ToArray(), <paramref name="cancellationToken"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Func<T, Task> action, CancellationToken cancellationToken = default)
	{
		action.AssertNotNull();

		if (@this.Any())
			Task.WaitAll(@this.Map(action).ToArray(), cancellationToken);
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/><see cref="Task"/>.WaitAll(@<paramref name="this"/>.To(item =&gt; <paramref name="action"/>>(item, <paramref name="cancellationToken"/>)).ToArray(), <paramref name="cancellationToken"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
	{
		action.AssertNotNull();

		if (@this.Any())
			Task.WaitAll(@this.Map(item => action(item, cancellationToken)).ToArray(), cancellationToken);
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/>@<paramref name="this"/>.To(item =&gt; <paramref name="action"/>(item, <paramref name="cancellationToken"/>)).AllAsync&gt;<typeparamref name="T"/>&lt;();
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
	{
		action.AssertNotNull();

		if (@this.Any())
			await @this.Map(item => action(item, cancellationToken)).AllAsync<T>();
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/>@<paramref name="this"/>.To(<paramref name="action"/>).AllAsync&gt;<typeparamref name="T"/>&lt;();
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, Task> action)
	{
		action.AssertNotNull();

		if (@this.Any())
			await @this.Map(action).AllAsync<T>();
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="    return default"/>;<br/>
	///	<br/>
	/// <see langword="return"/> <paramref name="options"/> <see langword="is not null"/><br/>
	///	<see langword="    "/>? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>)<br/>
	///	<see langword="    "/>: <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static ParallelLoopResult DoInParallel<T>(this IEnumerable<T>? @this, Action<T> action, ParallelOptions? options = null)
	{
		if (@this is null)
			return default;

		return options is not null
			? Parallel.ForEach(@this, options, action)
			: Parallel.ForEach(@this, action);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="    return default"/>;<br/>
	///	<br/>
	/// <see langword="return"/> <paramref name="options"/> <see langword="is not null"/><br/>
	///	<see langword="    "/>? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>)<br/>
	///	<see langword="    "/>: <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static ParallelLoopResult DoInParallel<T>(this IEnumerable<T>? @this, Action<T, ParallelLoopState> action, ParallelOptions? options = null)
	{
		if (@this is null)
			return default;

		return options is not null
			? Parallel.ForEach(@this, options, action)
			: Parallel.ForEach(@this, action);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="    return default"/>;<br/>
	///	<br/>
	/// <see langword="return"/> <paramref name="options"/> <see langword="is not null"/><br/>
	///	<see langword="    "/>? <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>)<br/>
	///	<see langword="    "/>: <see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static ParallelLoopResult DoInParallel<T>(this IEnumerable<T>? @this, Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
	{
		if (@this is null)
			return default;

		return options is not null
			? Parallel.ForEach(@this, options, action)
			: Parallel.ForEach(@this, action);
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; list.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/>_ =&gt; list.Each(@<paramref name="this"/>, <paramref name="edit"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this IEnumerable<T>? @this, Func<T, T> edit)
		=> @this switch
		{
			null => Enumerable<T>.Empty,
			T[] array => array.Each(edit),
			ImmutableArray<T> immutableArray => immutableArray.Each(edit),
			List<T> list => list.Each(edit),
			_ => Enumerable<T>.Each(@this, edit)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; list.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/>_ =&gt; list.Each(@<paramref name="this"/>, <paramref name="edit"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this IEnumerable<T>? @this, Func<T, int, T> edit)
		=> @this switch
		{
			null => Enumerable<T>.Empty,
			T[] array => array.Each(edit),
			ImmutableArray<T> immutableArray => immutableArray.Each(edit),
			List<T> list => list.Each(edit),
			_ => Enumerable<T>.Each(@this, edit)
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().First(<paramref name="defaultValue"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? First<T>(this IEnumerable? @this, T? defaultValue = default)
		=> @this.If<T>().First(defaultValue);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0, <paramref name="defaultValue"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? First<T>(this IEnumerable<T>? @this, T? defaultValue = default)
		=> @this.Get(0, defaultValue);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).First(<paramref name="defaultValue"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? First<T>(this IEnumerable<T>? @this, Predicate<T> filter, T? defaultValue = default)
		=> @this.If(filter).First(defaultValue);

	/// <summary>
	/// <code>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="foreach"/> (<see langword="var"/> items <see langword="in"/> @<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="        continue"/>;<br/>
	/// <br/>
	/// <see langword="    if"/> (items.TryAsSpan(<see langword="out var"/> memory))<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> count = memory.Length;<br/>
	/// <see langword="        for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="            yield return"/> memory.Span[i]:<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    else"/><br/>
	/// <see langword="        foreach"/> (<see langword="var"/> item <see langword="in"/> items)<br/>
	/// <see langword="            yield return"/> item:<br/>
	/// }
	/// </code>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> Gather<T>(this IEnumerable<IEnumerable<T>?>? @this)
	{
		if (!@this.Any())
			yield break;

		foreach (var items in @this)
		{
			if (!items.Any())
				continue;

			if (items.TryAsMemory(out var memory))
			{
				var count = memory.Length;
				for (var i = 0; i < count; ++i)
				{
					var item = memory.Span[i];
					yield return item;
				}
			}
			else
				foreach (var item in items)
					yield return item;
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryGet(<paramref name="index"/>, <see langword="out var"/> value) ? value : <paramref name="defaultValue"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Get<T>(this IEnumerable<T>? @this, Index index, T? defaultValue = default)
		=> @this.TryGet(index, out var value) ? value : defaultValue;

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Get(<paramref name="range"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Get(<paramref name="range"/>),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; list.Get(<paramref name="range"/>),<br/>
	///	<see langword="    "/>_ =&gt; @<paramref name="this"/>.ToArray().Get(<paramref name="range"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="IndexOutOfRangeException" />
	public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Range range)
		=> @this switch
		{
			T[] array => array.Get(range),
			ImmutableArray<T> immutableArray => immutableArray.Get(range),
			List<T> list => list.Get(range),
			_ => @this.ToArray().Get(range)
		};

	/// <summary>
	/// <code>
	/// <paramref name="keyFactory"/>.AssertNotNull();<br/>
	/// <br/>
	/// (<typeparamref name="K"/> Key, <typeparamref name="V"/> Value)[] items = @<paramref name="this"/>.To(item =&gt; (<paramref name="keyFactory"/>(item), item)).ToArray();<br/>
	/// <see langword="var"/> keys = items.To(_ =&gt; _.Key).ToHashSet(<paramref name="comparer"/>);<br/>
	/// <see langword="return"/> keys.ToDictionary(key => items.If(_ =&gt; keys.Comparer.Equals(_.Key, key)).To(_ =&gt; _.Value));
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IDictionary<K, IEnumerable<V>> Group<K, V>(this IEnumerable<V>? @this, Func<V, K> keyFactory, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		keyFactory.AssertNotNull();

		(K Key, V Value)[] items = @this.Map(item => (keyFactory(item), item)).ToArray();
		var keys = items.Map(_ => _.Key).ToHashSet(comparer);
		return keys.ToDictionary(key => items.If(_ => keys.Comparer.Equals(_.Key, key)).Map(_ => _.Value));
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.Any(value =&gt; <paramref name="comparer"/>.Equals(item, value));
	/// </code>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T item, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return @this.Any(value => comparer.Equals(item, value));
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// <see langword="return"/> <paramref name="values"/>.All(value =&gt; @<paramref name="this"/>.Has(value, <paramref name="comparer"/>));
	/// </code>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return values.All(value => @this.Has(value, comparer));
	}

	/// <summary>
	/// <c>=&gt; <paramref name="index"/>.Value &lt; @<paramref name="this"/>.Count();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
		=> index.Value < @this.Count();

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// <see langword="return"/> <paramref name="values"/>.Any(value =&gt; @<paramref name="this"/>.Has(value, <paramref name="comparer"/>));
	/// </code>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool HasAny<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return values.Any(value => @this.Has(value, comparer));
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}.Empty"/>,<br/>
	///	<see langword="    "/><see cref="IEnumerable{T}"/> items =&gt; items,<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.If(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static IEnumerable<T> If<T>(this IEnumerable? @this)
		=> @this switch
		{
			null => Enumerable<T>.Empty,
			IEnumerable<T> items => items,
			_ => Enumerable<T>.If(@this)
		};

	/// <summary>
	/// <code>
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.If(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.If(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; list.If(<paramref name="filter"/>),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.If(@<paramref name="this"/>, <paramref name="filter"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> If<T>(this IEnumerable<T>? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		return @this switch
		{
			null => Enumerable<T>.Empty,
			T[] array => array.If(filter),
			ImmutableArray<T> immutableArray => immutableArray.If(filter),
			List<T> list => list.If(filter),
			_ => Enumerable<T>.If(@this, filter)
		};
	}

	/// <summary>
	/// <code>
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="foreach"/> (<see langword="var"/> item <see langword="in"/> @<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    if"/> (<see langword="await"/> <paramref name="filter"/>(item))<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// <br/>
	/// <see langword="    if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	/// <see langword="        yield break"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this IEnumerable<T>? @this, Func<T, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		if (@this is null)
			yield break;

		foreach (var item in @this)
		{
			if (await filter(item))
				yield return item;

			if (token.IsCancellationRequested)
				yield break;
		}
	}

	/// <summary>
	/// <code>
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="foreach"/> (<see langword="var"/> item <see langword="in"/> @<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    if"/> (<see langword="await"/> <paramref name="filter"/>(item, <paramref name="token"/>))<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// <br/>
	/// <see langword="    if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	/// <see langword="        yield break"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<T> IfAsync<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task<bool>> filter, [EnumeratorCancellation] CancellationToken token = default)
	{
		filter.AssertNotNull();

		if (@this is null)
			yield break;

		foreach (var item in @this)
		{
			if (await filter(item, token))
				yield return item;

			if (token.IsCancellationRequested)
				yield break;
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(_ =&gt; _ <see langword="is not null"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
		=> @this.If(_ => _ is not null)!;

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/> &amp;&amp; <paramref name="items"/> <see langword="is null"/>)<br/>
	/// <see langword="    return true"/>;<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/> || <paramref name="items"/> <see langword="is null"/>)<br/>
	/// <see langword="    return false"/>;<br/>
	/// <br/>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// <see langword="using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="using var"/> itemEnumerator = <paramref name="items"/>.GetEnumerator();<br/>
	/// <br/>
	/// <see langword="while"/> (enumerator.TryNext(<see langword="out var"/> item1) &amp;&amp; itemEnumerator.TryNext(<see langword="out var"/> item2))<br/>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="comparer"/>.Equals(item1, item2))<br/>
	/// <see langword="        return false"/>;<br/>
	/// }<br/>
	/// <br/>
	/// <see langword="return"/> !enumerator.MoveNext() &amp;&amp; !itemEnumerator.MoveNext();
	/// </code>
	/// </summary>
	public static bool IsSequence<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		if (@this is null && items is null)
			return true;

		if (@this is null || items is null)
			return false;

		comparer ??= EqualityComparer<T>.Default;

		using var enumerator = @this.GetEnumerator();
		using var itemEnumerator = items.GetEnumerator();

		while (enumerator.TryNext(out var item1) && itemEnumerator.TryNext(out var item2))
		{
			if (!comparer.Equals(item1, item2))
				return false;
		}

		return !enumerator.MoveNext() && !itemEnumerator.MoveNext();
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>).SetEquals(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsSet<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		=> @this.ToHashSet(comparer).SetEquals(items ?? Enumerable<T>.Empty);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>).SetEquals(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsSetBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
		=> @this.ToHashSetBy(getKey, keyComparer).SetEquals(items ?? Enumerable<T>.Empty);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see cref="string"/>.Join(<paramref name="delimeter"/>, @<paramref name="this"/>) : <see cref="string.Empty"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Join<T>(this IEnumerable<T>? @this, char delimeter)
		=> @this is not null ? string.Join(delimeter, @this) : string.Empty;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see cref="string"/>.Join(<paramref name="delimeter"/>)) : <see cref="string.Empty"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Join<T>(this IEnumerable<T>? @this, string delimeter)
		=> @this is not null ? string.Join(delimeter, @this) : string.Empty;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().Last(<paramref name="defaultValue"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Last<T>(this IEnumerable? @this, T? defaultValue = default)
		=> @this.If<T>().Last(defaultValue);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(^1, <paramref name="defaultValue"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Last<T>(this IEnumerable<T>? @this, T? defaultValue = default)
		=> @this.Get(^1, defaultValue);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).Last(<paramref name="defaultValue"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Last<T>(this IEnumerable<T>? @this, Predicate<T> filter, T? defaultValue = default)
		=> @this.If(filter).Last(defaultValue);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; Enumerable&lt;V&gt;.Empty,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Map(@<paramref name="this"/>, <paramref name="map"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this IEnumerable<T>? @this, Func<T, V> map)
		=> @this switch
		{
			null => Enumerable<V>.Empty,
			T[] array => array.Map(map),
			ImmutableArray<T> immutableArray => immutableArray.Map(map),
			List<T> list => list.Map(map),
			_ => Enumerable<T>.Map(@this, map)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; Enumerable&lt;V&gt;.Empty,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Map(@<paramref name="this"/>, <paramref name="map"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this IEnumerable<T>? @this, Func<T, int, V> map)
		=> @this switch
		{
			null => Enumerable<V>.Empty,
			T[] array => array.Map(map),
			ImmutableArray<T> immutableArray => immutableArray.Map(map),
			List<T> list => list.Map(map),
			_ => Enumerable<T>.Map(@this, map)
		};

	/// <summary>
	/// <code>
	/// <paramref name="map"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="     yield break"/>;<br/>
	/// <br/>
	/// <see cref="int"/> count;<br/>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case"/> <typeparamref name="T"/>[] array:<br/>
	/// <see langword="        for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="        "/>{<br/>
	///	<see langword="            yield return await"/> <paramref name="map"/>(item[i]);<br/>
	///	<see langword="            if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	///	<see langword="                yield break"/>;<br/>
	///	<see langword="        "/>}<br/>
	///	<see langword="        yield break"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> array:<br/>
	/// <see langword="        for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="        "/>{<br/>
	///	<see langword="            yield return await"/> <paramref name="map"/>(item[i]);<br/>
	///	<see langword="            if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	///	<see langword="                yield break"/>;<br/>
	///	<see langword="        "/>}<br/>
	///	<see langword="        yield break"/>;<br/>
	/// <see langword="    default"/>:<br/>
	/// <see langword="        foreach"/> (<see langword="var"/> item <see langword="in"/> @<paramref name="this"/>)<br/>
	/// <see langword="        "/>{<br/>
	///	<see langword="            yield return await"/> <paramref name="map"/>(item);<br/>
	///	<see langword="            if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	///	<see langword="                yield break"/>;<br/>
	///	<see langword="        "/>}<br/>
	///	<see langword="        yield break"/>;<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<V> MapAsync<T, V>(this IEnumerable<T>? @this, Func<T?, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
	{
		map.AssertNotNull();

		if (@this is null)
			yield break;

		int count;
		switch (@this)
		{
			case T[] array:
				count = array.Length;
				for (var i = 0; i < count; ++i)
				{
					yield return await map(array[i]);
					if (token.IsCancellationRequested)
						yield break;
				}
				yield break;
			case ImmutableArray<T> array:
				count = array.Length;
				for (var i = 0; i < count; ++i)
				{
					yield return await map(array[i]);
					if (token.IsCancellationRequested)
						yield break;
				}
				yield break;
			default:
				foreach (var item in @this)
				{
					yield return await map(item);
					if (token.IsCancellationRequested)
						yield break;
				}
				yield break;
		}
	}

	/// <summary>
	/// <code>
	/// <paramref name="map"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="     yield break"/>;<br/>
	/// <br/>
	/// <see cref="int"/> count;<br/>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case"/> <typeparamref name="T"/>[] array:<br/>
	/// <see langword="        for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="        "/>{<br/>
	///	<see langword="            yield return await"/> <paramref name="map"/>(item[i], <paramref name="token"/>);<br/>
	///	<see langword="            if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	///	<see langword="                yield break"/>;<br/>
	///	<see langword="        "/>}<br/>
	///	<see langword="        yield break"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> array:<br/>
	/// <see langword="        for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="        "/>{<br/>
	///	<see langword="            yield return await"/> <paramref name="map"/>(item[i], <paramref name="token"/>);<br/>
	///	<see langword="            if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	///	<see langword="                yield break"/>;<br/>
	///	<see langword="        "/>}<br/>
	///	<see langword="        yield break"/>;<br/>
	/// <see langword="    default"/>:<br/>
	/// <see langword="        foreach"/> (<see langword="var"/> item <see langword="in"/> @<paramref name="this"/>)<br/>
	/// <see langword="        "/>{<br/>
	///	<see langword="            yield return await"/> <paramref name="map"/>(item, <paramref name="token"/>);<br/>
	///	<see langword="            if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	///	<see langword="                yield break"/>;<br/>
	///	<see langword="        "/>}<br/>
	///	<see langword="        yield break"/>;<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async IAsyncEnumerable<V> MapAsync<T, V>(this IEnumerable<T>? @this, Func<T?, CancellationToken, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
	{
		map.AssertNotNull();

		if (@this is null)
			yield break;

		int count;
		switch (@this)
		{
			case T[] array:
				count = array.Length;
				for (var i = 0; i < count; ++i)
				{
					yield return await map(array[i], token);
					if (token.IsCancellationRequested)
						yield break;
				}
				yield break;
			case ImmutableArray<T> array:
				count = array.Length;
				for (var i = 0; i < count; ++i)
				{
					yield return await map(array[i], token);
					if (token.IsCancellationRequested)
						yield break;
				}
				yield break;
			default:
				foreach (var item in @this)
				{
					yield return await map(item, token);
					if (token.IsCancellationRequested)
						yield break;
				}
				yield break;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.IntersectWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.IntersectWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.IntersectWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> MatchBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.IntersectWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="Comparer{T}.Default"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.TryFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, <paramref name="comparer"/>.Maximum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	public static T? Maximum<T>(this IEnumerable<T>? @this, T? defaultValue = default, IComparer<T>? comparer = null)
	{
		comparer ??= Comparer<T>.Default;
		return @this.TryFirst(out var initial) ? @this.Aggregate(initial, comparer.Maximum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <paramref name="getValue"/>.AssertNotNull();<br/>
	/// <br/>
	/// <paramref name="valueComparer"/> ??= Comparer&lt;<typeparamref name="V"/>&gt;.Default;<br/>
	/// <see langword="var"/> comparer = <see langword="new"/> <see cref="CustomComparer{T}"/>((item1, item2) =&gt; <paramref name="valueComparer"/>.Compare(<paramref name="getValue"/>(item1!), <paramref name="getValue"/>(item2!)));<br/>
	/// <see langword="return"/> @<paramref name="this"/>.TryFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, comparer.Maximum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T? MaximumBy<T, V>(this IEnumerable<T>? @this, Func<T, V> getValue, T? defaultValue = default, IComparer<V>? valueComparer = null)
	{
		getValue.AssertNotNull();

		valueComparer ??= Comparer<V>.Default;
		var comparer = new CustomComparer<T>((item1, item2) => valueComparer.Compare(getValue(item1!), getValue(item2!)));
		return @this.TryFirst(out var initial) ? @this.Aggregate(initial, comparer.Maximum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="Comparer{T}.Default"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.TryFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, <paramref name="comparer"/>.Minimum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	public static T? Minimum<T>(this IEnumerable<T>? @this, T? defaultValue = default, IComparer<T>? comparer = null)
	{
		comparer ??= Comparer<T>.Default;
		return @this.TryFirst(out var initial) ? @this.Aggregate(initial, comparer.Minimum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <paramref name="getValue"/>.AssertNotNull();<br/>
	/// <br/>
	/// <paramref name="valueComparer"/> ??= Comparer&lt;<typeparamref name="V"/>&gt;.Default;<br/>
	/// <see langword="var"/> comparer = <see langword="new"/> <see cref="CustomComparer{T}"/>((item1, item2) =&gt; <paramref name="valueComparer"/>.Compare(<paramref name="getValue"/>(item1!), <paramref name="getValue"/>(item2!)));<br/>
	/// <see langword="return"/> @<paramref name="this"/>.TryFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, comparer.Minimum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T? MinimumBy<T, V>(this IEnumerable<T>? @this, Func<T, V> getValue, T? defaultValue = default, IComparer<V>? valueComparer = null)
	{
		getValue.AssertNotNull();

		valueComparer ??= Comparer<V>.Default;
		var comparer = new CustomComparer<T>((item1, item2) => valueComparer.Compare(getValue(item1!), getValue(item2!)));
		return @this.TryFirst(out var initial) ? @this.Aggregate(initial, comparer.Minimum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.SymmetricExceptWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> NotMatch<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.SymmetricExceptWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.SymmetricExceptWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> NotMatchBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.SymmetricExceptWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(<paramref name="count"/>..^0));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> Skip<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(count..^0);

	/// <summary>
	/// <code>
	/// <paramref name="skip"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.TryAsMemory(<see langword="out var"/> memory))<br/>
	/// {<br/>
	/// <see langword="    var"/> count = memory.Length;<br/>
	/// <see langword="    var"/> i = 0;<br/>
	/// <see langword="    while"/> (i &lt; count &amp;&amp; <paramref name="skip"/>(memory.Span[i]))<br/>
	/// <see langword="        "/>++i;<br/>
	/// <see langword="    while"/> (i &lt; count)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>++i;<br/>
	/// <see langword="        yield return"/> memory.Span[i];<br/>
	/// <see langword="    "/>}<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    while"/> (enumerator.TryNext(<see langword="out var"/> item))<br/>
	/// <see langword="        if"/> (!<paramref name="skip"/>(item))<br/>
	/// <see langword="            break"/>;<br/>
	/// <see langword="    while"/> (enumerator.TryNext(<see langword="out var"/> item))<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> @this, Predicate<T> skip)
	{
		skip.AssertNotNull();

		if (!@this.Any())
			yield break;

		if (@this.TryAsMemory(out var memory))
		{
			var count = memory.Length;
			var i = 0;
			while (i < count && skip(memory.Span[i]))
				++i;
			while (i < count)
			{
				++i;
				yield return memory.Span[i];
			}
		}
		else
		{
			using var enumerator = @this.GetEnumerator();
			while (enumerator.TryNext(out var item))
				if (!skip(item))
					break;
			while (enumerator.TryNext(out var item))
				yield return item;
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> items = @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/> _ <see langword="when"/> !@<paramref name="this"/>.Any() =&gt; <see cref="Array{T}.Empty"/>,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array,<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.ToArray()<br/>
	/// }<br/>
	/// items.Sort(<paramref name="comparer"/> ?? <see cref="Comparer{T}.Default"/>);<br/>
	/// <see langword="return"/> items;
	/// </code>
	/// </summary>
	public static T[] Sort<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
	{
		var items = @this switch
		{
			_ when !@this.Any() => Array<T>.Empty,
			T[] array => array,
			_ => @this.ToArray()
		};
		items.Sort(comparer ?? Comparer<T>.Default);
		return items;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0..<paramref name="count"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> Take<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(0..count);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0..<paramref name="count"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(Index.FromEnd(count)..^0);

	/// <summary>
	/// <code>
	/// <paramref name="take"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.TryAsMemory(<see langword="out var"/> memory))<br/>
	/// {<br/>
	/// <see langword="    var"/> count = memory.Length;<br/>
	/// <see langword="    var"/> i = -1;<br/>
	/// <see langword="    while"/> (++i &lt; count &amp;&amp; <paramref name="take"/>(memory.Span[i]))<br/>
	/// <see langword="        yield return"/> memory.Span[i];<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    while"/> (enumerator.TryNext(<see langword="out var"/> item) &amp;&amp; <paramref name="take"/>(item))<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> @this, Predicate<T> take)
	{
		take.AssertNotNull();

		if (!@this.Any())
			yield break;

		if (@this.TryAsMemory(out var memory))
		{
			var count = memory.Length;
			var i = -1;
			while (++i < count && take(memory.Span[i]))
				yield return memory.Span[i];
		}
		else
		{
			using var enumerator = @this.GetEnumerator();
			while (enumerator.TryNext(out var item) && take(item))
				yield return item;
		}
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see cref="Array{T}.Empty"/>,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.AsSpan().ToArray(),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.AsSpan().ToArray(),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.ToArray(),<br/>
	/// <see langword="    "/><see cref="Stack{T}"/> stack =&gt; stack.ToArray(),<br/>
	/// <see langword="    "/><see cref="Queue{T}"/> queue =&gt; queue.ToArray(),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.ToArray(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T[] ToArray<T>(this IEnumerable<T>? @this)
		=> @this switch
		{
			null => Array<T>.Empty,
			T[] array => array.GetCopy(),
			ImmutableArray<T> immutableArray => immutableArray.AsSpan().ToArray(),
			List<T> list => list.ToArray(),
			Stack<T> stack => stack.ToArray(),
			Queue<T> queue => queue.ToArray(),
			_ => Enumerable<T>.ToArray(@this)
		};

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is not null"/>)<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> @<paramref name="this"/>)<br/>
	///	<see langword="        yield return"/> item;<br/>
	///	<br/>
	///	<see langword="await"/> <see cref="Task.CompletedTask"/>;
	/// </code>
	/// </summary>
	public static async IAsyncEnumerable<T> ToAsync<T>(this IEnumerable<T>? @this)
	{
		if (@this is not null)
			foreach (var item in @this)
				yield return item;

		await Task.CompletedTask;
	}

	/// <summary>
	/// <code>
	/// <paramref name="map"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.To(<paramref name="map"/>).ToCsv();
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static string ToCSV<T>(this IEnumerable<T>? @this, Func<T, string> map)
	{
		map.AssertNotNull();

		return @this.Map(map).ToCSV();
	}

	/// <summary>
	/// <code>
	/// <paramref name="valueFactory"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> dictionary = <see langword="new"/> Dictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>.Count(), <paramref name="comparer"/>);<br/>
	/// @<paramref name="this"/>?.Do(key =&gt; dictionary.Add(key, <paramref name="valueFactory"/>(key)));<br/>
	/// <see langword="return"/> dictionary;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		valueFactory.AssertNotNull();

		var dictionary = new Dictionary<K, V>(@this.Count(), comparer);
		@this?.Do(key => dictionary.Add(key, valueFactory(key)));
		return dictionary;
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="new"/> Dictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(0, <paramref name="comparer"/>),<br/>
	/// <see langword="    "/> _ =&gt; <see langword="new"/> Dictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>, <paramref name="comparer"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K>? comparer = null)
		where K : notnull
		=> @this switch
		{
			null => new Dictionary<K, V>(0, comparer),
			_ => new Dictionary<K, V>(@this, comparer),
		};

	/// <summary>
	/// <code>
	/// <paramref name="keyFactory"/>.AssertNotNull();<br/>
	/// <paramref name="valueFactory"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> dictionary = <see langword="new"/> Dictionary&lt;<typeparamref name="K"/>, <typeparamref name="V"/>&gt;(@<paramref name="this"/>.Count(), <paramref name="comparer"/>);<br/>
	/// @<paramref name="this"/>?.Do(value =&gt; dictionary.Add(<paramref name="keyFactory"/>(value), <paramref name="valueFactory"/>(value)));<br/>
	/// <see langword="return"/> dictionary;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K>? comparer = null)
		where K : notnull
	{
		keyFactory.AssertNotNull();
		valueFactory.AssertNotNull();

		var dictionary = new Dictionary<K, V>(@this.Count(), comparer);
		@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
		return dictionary;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(tuple =&gt; <see cref="KeyValuePair"/>.Create(tuple.Item1, tuple.Item2)).ToDictionary(<paramref name="comparer"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<Tuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
		where K : notnull
		=> @this.Map(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(tuple =&gt; <see cref="KeyValuePair"/>.Create(tuple.Item1, tuple.Item2)).ToDictionary(<paramref name="comparer"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<ValueTuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
		where K : notnull
		=> @this.Map(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashCode = <see langword="new"/> <see cref="HashCode"/>();<br/>
	/// @<paramref name="this"/>?.Do(hashCode.Add);<br/>
	/// <see langword="return"/> hashCode.ToHashCode();
	/// </code>
	/// </summary>
	public static int ToHashCode<T>(this IEnumerable<T>? @this)
	{
		var hashCode = new HashCode();
		@this?.Do(hashCode.Add);
		return hashCode.ToHashCode();
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any()
	/// ? <see langword="new"/> <see cref="HashSet{T}"/>(@<paramref name="this"/>, <paramref name="comparer"/>)
	/// : <see langword="new"/> <see cref="HashSet{T}"/>(0, <paramref name="comparer"/>);</c>
	/// </summary>
	public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T>? comparer = null)
		=> @this.Any() ? new HashSet<T>(@this, comparer) : new HashSet<T>(0, comparer);

	/// <summary>
	/// <code>
	/// <paramref name="getKey"/>.AssertNotNull();<br/>
	/// <br/>
	/// <paramref name="keyComparer"/> ??= EqualityComparer&lt;<typeparamref name="K"/>&gt;.Default;<br/>
	/// <see langword="var"/> comparer = <see langword="new"/> <see cref="CustomEqualityComparer{T}"/>((item1, item2) =&gt; <paramref name="keyComparer"/>.Equals(getKey(item1!), getKey(item2!)));<br/>
	/// <see langword="reyurn"/> @this.Any() ? <see langword="new"/> <see cref="HashSet{T}"/>(@<paramref name="this"/>, comparer) : <see langword="new"/> <see cref="HashSet{T}"/>(0, comparer);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> ToHashSetBy<T, K>(this IEnumerable<T>? @this, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		getKey.AssertNotNull();

		keyComparer ??= EqualityComparer<K>.Default;
		var comparer = new CustomEqualityComparer<T>((item1, item2) => keyComparer.Equals(getKey(item1!), getKey(item2!)));
		return @this.Any() ? new HashSet<T>(@this, comparer) : new HashSet<T>(0, comparer);
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>_ <see langword="when"/> !@<paramref name="this"/>.Any() =&gt; <see cref="ImmutableQueue{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; <see cref="ImmutableQueue"/>.Create(array),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; <see cref="ImmutableQueue"/>.Create(list.ToArray()),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="ImmutableQueue.CreateRange{T}"/>(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static ImmutableQueue<T> ToImmutableQueue<T>(this IEnumerable<T>? @this)
		where T : notnull
		=> @this switch
		{
			_ when !@this.Any() => ImmutableQueue<T>.Empty,
			T[] array => ImmutableQueue.Create(array),
			List<T> list => ImmutableQueue.Create(list.ToArray()),
			_ => ImmutableQueue.CreateRange<T>(@this)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>_ <see langword="when"/> !@<paramref name="this"/>.Any() =&gt; <see cref="ImmutableStack{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; <see cref="ImmutableStack"/>.Create(array),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; <see cref="ImmutableStack"/>.Create(list.ToArray()),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="ImmutableStack.CreateRange{T}"/>(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static ImmutableStack<T> ToImmutableStack<T>(this IEnumerable<T>? @this)
		where T : notnull
		=> @this switch
		{
			_ when !@this.Any() => ImmutableStack<T>.Empty,
			T[] array => ImmutableStack.Create(array),
			List<T> list => ImmutableStack.Create(list.ToArray()),
			_ => ImmutableStack.CreateRange<T>(@this)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; Enumerable&lt;<see cref="int"/>&gt;.Empty,<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; list.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.ToIndex(@<paramref name="this"/>, <paramref name="filter"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, Predicate<T> filter)
		=> @this switch
		{
			null => Enumerable<int>.Empty,
			ImmutableArray<T> immutableArray => immutableArray.ToIndex(filter),
			T[] array => array.ToIndex(filter),
			List<T> list => list.ToIndex(filter),
			_ => Enumerable<T>.ToIndex(@this, filter)
		};

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// @<paramref name="this"/>.ToIndex(value =&gt; <paramref name="comparer"/>.Equals(value, <paramref name="item"/>);
	/// </code>
	/// </summary>
	public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T item, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return @this.ToIndex(value => comparer.Equals(value, item));
	}

	/// <summary>
	/// <c>
	/// =&gt; @<paramref name="this"/> <see langword="is not null"/>
	/// ? <see langword="new"/> <see cref="List{T}"/>(@<paramref name="this"/>)
	/// : <see langword="new"/> <see cref="List{T}"/>(0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static List<T> ToList<T>(this IEnumerable<T>? @this)
		=> @this is not null ? new List<T>(@this) : new List<T>(0);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(<paramref name="map"/>).Gather();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<V> ToMany<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map)
		=> @this.Map(map).Gather();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/>
	/// ? <see langword="new"/> <see cref="Queue{T}"/>(@<paramref name="this"/>)
	/// : <see langword="new"/> <see cref="Queue{T}"/>(0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this)
		=> @this.Any() ? new Queue<T>(@this) : new Queue<T>(0);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see cref="ReadOnlySpan{T}"/>.Empty,<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.AsSpan(),<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.AsSpan(),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.ToArray().AsSpan(),<br/>
	/// <see langword="    "/><see cref="Queue{T}"/> queue =&gt; queue.ToArray().AsSpan(),<br/>
	/// <see langword="    "/><see cref="Stack{T}"/> stack =&gt; stack.ToArray().AsSpan(),<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.ToArray().AsSpan(),<br/>
	/// };
	/// </code>
	/// </summary>
	public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IEnumerable<T>? @this)
		=> @this switch
		{
			null => ReadOnlySpan<T>.Empty,
			ImmutableArray<T> immutableArray => immutableArray.AsSpan(),
			T[] array => array.AsSpan(),
			List<T> list => list.ToArray().AsSpan(),
			Queue<T> queue => queue.ToArray().AsSpan(),
			Stack<T> stack => stack.ToArray().AsSpan(),
			_ => @this.ToArray().AsSpan(),
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Span{T}"/>.Empty,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.AsSpan(),<br/>
	///	<see langword="    "/><see cref="List{T}"/> list =&gt; list.ToArray().AsSpan(),<br/>
	///	<see langword="    "/><see cref="Queue{T}"/> queue =&gt; queue.ToArray().AsSpan(),<br/>
	///	<see langword="    "/><see cref="Stack{T}"/> stack =&gt; stack.ToArray().AsSpan(),<br/>
	///	<see langword="    "/>_ =&gt; @<paramref name="this"/>.ToArray().AsSpan()<br/>
	/// };
	/// </code>
	/// </summary>
	public static Span<T> ToSpan<T>(this IEnumerable<T>? @this)
		=> @this switch
		{
			null => Span<T>.Empty,
			T[] array => array.AsSpan(),
			List<T> list => list.ToArray().AsSpan(),
			Queue<T> queue => queue.ToArray().AsSpan(),
			Stack<T> stack => stack.ToArray().AsSpan(),
			_ => @this.ToArray().AsSpan(),
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/>
	/// ? <see langword="new"/> <see cref="Stack{T}"/>(@<paramref name="this"/>)
	/// : <see langword="new"/> <see cref="Stack{T}"/>(0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Stack<T> ToStack<T>(this IEnumerable<T>? @this)
		=> @this is not null ? new Stack<T>(@this) : new Stack<T>(0);

	/// <summary>
	/// <code>
	/// <paramref name="memory"/> = @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.AsMemory(),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> array =&gt; array.AsMemory(),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.ToArray().AsMemory(),<br/>
	/// <see langword="    "/><see cref="Queue{T}"/> queue =&gt; queue.ToArray().AsMemory(),<br/>
	/// <see langword="    "/><see cref="Stack{T}"/> stack =&gt; stack.ToArray().AsMemory(),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="ReadOnlyMemory{T}.Empty"/><br/>
	/// }<br/>
	/// <see langword="return"/> !<paramref name="memory"/>.IsEmpty;
	/// </code>
	/// </summary>
	public static bool TryAsMemory<T>([NotNullWhen(true)] this IEnumerable<T>? @this, out ReadOnlyMemory<T> memory)
	{
		memory = @this switch
		{
			T[] array => array.AsMemory(),
			ImmutableArray<T> array => array.AsMemory(),
			List<T> list => list.ToArray().AsMemory(),
			Queue<T> queue => queue.ToArray().AsMemory(),
			Stack<T> stack => stack.ToArray().AsMemory(),
			_ => ReadOnlyMemory<T>.Empty
		};
		return !memory.IsEmpty;
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is"/> <see cref="ICollection{T}"/> collection)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="collectionCount"/> = collection.Count;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <see langword="else if"/> (@<paramref name="this"/> <see langword="is"/> <see cref="IReadOnlyCollection{T}"/> readOnlyCollection)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="collectionCount"/> = readOnlyCollection.Count;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <see langword="else if"/> (@<paramref name="this"/> <see langword="is"/> <see cref="ICollection"/> collectionOld)<br/>
	/// {<br/>
	/// <see langword="    "/><paramref name="collectionCount"/> = collectionOld.Count;<br/>
	/// <see langword="    return true"/>;<br/>
	/// }<br/>
	/// <paramref name="collectionCount"/> = 0;<br/>
	/// <see langword="return false"/>;
	/// </code>
	/// </summary>
	/// <param name="collectionCount">Sets the count if the underlying enumerable implements <see cref="ICollection{T}"/>, <see cref="IReadOnlyCollection{T}"/>, or <see cref="ICollection"/>.</param>
	/// <returns>Returns <see langword="true"/> if the underlying enumerable implements <see cref="ICollection{T}"/>, <see cref="IReadOnlyCollection{T}"/>, or <see cref="ICollection"/>.</returns>
	public static bool TryCount<T>([NotNullWhen(true)] this IEnumerable<T>? @this, out int collectionCount)
	{
		if (@this is ICollection<T> collection)
		{
			collectionCount = collection.Count;
			return true;
		}
		else if (@this is IReadOnlyCollection<T> readOnlyCollection)
		{
			collectionCount = readOnlyCollection.Count;
			return true;
		}
		else if (@this is ICollection collectionOld)
		{
			collectionCount = collectionOld.Count;
			return true;
		}
		collectionCount = 0;
		return false;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().TryGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable? @this, [NotNullWhen(true)] out T? item)
		=> @this.If<T>().TryGet(0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T>? @this, [NotNullWhen(true)] out T? item)
		=> @this.TryGet(0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).TryGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryFirst<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter, [NotNullWhen(true)] out T? item)
		=> @this.If(filter).TryGet(0, out item);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}"/>.NoGet(<see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; list.TryGet(<paramref name="index"/>, <see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> list =&gt; list.TryGet(<paramref name="index"/>, <see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/>_ <see langword="when"/> <paramref name="index"/>.IsFromEnd =&gt; <see cref="Enumerable{T}"/>.TryGet(@<paramref name="this"/>, <paramref name="index"/>.FromStart(@<paramref name="this"/>.Count()).Value, <see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.TryGet(@<paramref name="this"/>, <paramref name="index"/>.Value, <see langword="out"/> <paramref name="item"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="IndexOutOfRangeException"/>
	public static bool TryGet<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index, [NotNullWhen(true)] out T? item)
		=> @this switch
		{
			null => Enumerable<T>.NoGet(out item),
			IList<T> list => list.TryGet(index, out item),
			IReadOnlyList<T> list => list.TryGet(index, out item),
			_ when index.IsFromEnd => Enumerable<T>.TryGet(@this, index.FromStart(@this.Count()).Value, out item),
			_ => Enumerable<T>.TryGet(@this, index.Value, out item)
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().TryGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryLast<T>([NotNullWhen(true)] this IEnumerable? @this, [NotNullWhen(true)] out T? item)
		=> @this.If<T>().TryGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryLast<T>([NotNullWhen(true)] this IEnumerable<T>? @this, [NotNullWhen(true)] out T? item)
		where T : class
		=> @this.TryGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).TryGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryLast<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter, [NotNullWhen(true)] out T? item)
		where T : class
		=> @this.If(filter).TryGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().TrySingle(<see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable? @this, [NotNullWhen(true)] out T? item)
		=> @this.If<T>().TrySingle(out item);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}"/>.NoGet(<see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; list.TryGet(0, <see langword="out"/> <paramref name="item"/>) &amp;&amp; list.Count == 1,<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> list =&gt; list.TryGet(0, <see langword="out"/> <paramref name="item"/>) &amp;&amp; list.Count == 1,<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.TrySingle(@<paramref name="this"/>, <see langword="out"/> <paramref name="item"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T>? @this, [NotNullWhen(true)] out T? item)
		=> @this switch
		{
			null => Enumerable<T>.NoGet(out item),
			IList<T> list => list.TryGet(0, out item) && list.Count == 1,
			IReadOnlyList<T> list => list.TryGet(0, out item) && list.Count == 1,
			_ => Enumerable<T>.TrySingle(@this, out item)
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).TrySingle(<see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TrySingle<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter, [NotNullWhen(true)] out T? item)
		=> @this.If(filter).TrySingle(out item);

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.UnionWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.UnionWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.UnionWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> UnionBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.UnionWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.ExceptWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.ExceptWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.ExceptWith(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> WithoutBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.ExceptWith(items ?? Enumerable<T>.Empty);
		return hashSet;
	}
}
