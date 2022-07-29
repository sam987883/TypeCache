// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task AllAsync<T>(this IEnumerable<Task> @this)
		=> await Task.WhenAll(@this);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any() ? <see langword="await"/> <see cref="Task"/>.WhenAll(@<paramref name="this"/>) : <see cref="Array{T}.Empty"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async ValueTask<T[]> AllAsync<T>(this IEnumerable<Task<T>>? @this)
		=> @this.Any() ? await Task.WhenAll(@this) : Array<T>.Empty;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().Any()</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable? @this)
		=> @this.If<T>().Any();

	/// <summary>
	/// <code>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IfCount(<see langword="out var"/> count) =&gt; count &gt; 0,<br/>
	/// <see langword="    "/>_ =&gt; any(@<paramref name="this"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static bool"/> any(<see cref="IEnumerable{T}"/> enumerable)<br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = enumerable.GetEnumerator();<br/>
	/// <see langword="    return"/> enumerator.MoveNext();<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
	{
		return @this switch
		{
			null => false,
			_ when @this.IfCount(out var count) => count > 0,
			_ => any(@this)
		};

		static bool any(IEnumerable<T> enumerable)
		{
			using var enumerator = enumerable.GetEnumerator();
			return enumerator.MoveNext();
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).Any();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter)
		=> @this.If(filter).Any();

	/// <summary>
	/// <c>=&gt; <see langword="await"/> <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task AnyAsync<T>(this IEnumerable<Task> @this)
		=> await Task.WhenAny(@this);

	/// <summary>
	/// <c>=&gt; <see langword="await await"/> <see cref="Task"/>.WhenAny(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static async Task<T> AnyAsync<T>(this IEnumerable<Task<T>> @this)
		=> await await Task.WhenAny(@this);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; !value.HasValue);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyNull<T>([NotNullWhen(true)] this IEnumerable<T?>? @this)
		where T : struct
		=> @this.Any(value => !value.HasValue);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Any(value =&gt; value <see langword="is null"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool AnyNull<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
		where T : class
		=> @this.Any(value => value is null);

	/// <summary>
	/// <c>=&gt; <see langword="new"/>[] { @<paramref name="this"/>, <paramref name="items"/> }.Gather();</c>
	/// </summary>
	public static IEnumerable<T> Append<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		=> new[] { @this, items }.Gather();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Append(<paramref name="sets"/>.Gather());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> Append<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? sets)
		=> @this.Append(sets.Gather());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Append(<paramref name="items"/> <see langword="as"/> <see cref="IEnumerable{T}"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> Append<T>(this IEnumerable<T>? @this, params T[]? items)
		=> @this.Append(items as IEnumerable<T>);

	/// <summary>
	/// <code>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Array{T}.Empty"/>,<br/>
	///	<see langword="    "/><see cref="IEnumerable{T}"/> enumerable =&gt; enumerable,<br/>
	///	<see langword="    "/>_ =&gt; map(@<paramref name="this"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> <see cref="IEnumerable{T}"/> map(<see cref="IEnumerable"/> enumerable)<br/>
	/// {<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="        yield return"/> item <see langword="is"/> <typeparamref name="T"/> value ? value : <see langword="default"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	public static IEnumerable<T?> As<T>(this IEnumerable? @this)
	{
		return @this switch
		{
			null => Array<T>.Empty,
			IEnumerable<T> enumerable => enumerable,
			_ => map(@this)
		};

		static IEnumerable<T?> map(IEnumerable enumerable)
		{
			foreach (var item in enumerable)
				yield return item is T value ? value : default;
		}
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IfCount(<see langword="out var"/> collectionCount) =&gt; <paramref name="count"/> &gt;= collectionCount,<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.GetEnumerator().Move(<paramref name="count"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool AtLeast<T>([NotNullWhen(true)] this IEnumerable<T>? @this, int count)
		=> @this switch
		{
			null => false,
			_ when @this.IfCount(out var collectionCount) => count >= collectionCount,
			_ => @this.GetEnumerator().Move(count)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IfCount(<see langword="out var"/> collectionCount) =&gt; <paramref name="count"/> &lt;= collectionCount,<br/>
	/// <see langword="    "/>_ =&gt; !@<paramref name="this"/>.GetEnumerator().Move(<paramref name="count"/> + 1)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool AtMost<T>([NotNullWhen(true)] this IEnumerable<T>? @this, int count)
		=> @this switch
		{
			null => false,
			_ when @this.IfCount(out var collectionCount) => count <= collectionCount,
			_ => !@this.GetEnumerator().Move(count + 1)
		};

	/// <summary>
	/// <code>
	/// <see langword="if"/> (<paramref name="tuple"/>.Item1 <see langword="is null"/> || tuple.Item2 <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="using var"/> enumerator1 = <paramref name="tuple"/>.Item1.GetEnumerator();<br/>
	/// <see langword="using var"/> enumerator2 = <paramref name="tuple"/>.Item2.GetEnumerator();<br/>
	/// <see langword="while"/> (enumerator1.IfNext(<see langword="out var"/> item1) &amp;&amp; enumerator2.IfNext(<see langword="out var"/> item2))<br/>
	/// <see langword="    yield return"/> (item1, item2);<br/>
	/// </code>
	/// </summary>
	public static IEnumerable<(A, B)> Combine<A, B>((IEnumerable<A>, IEnumerable<B>) tuple)
	{
		if (tuple.Item1 is null || tuple.Item2 is null)
			yield break;

		using var enumerator1 = tuple.Item1.GetEnumerator();
		using var enumerator2 = tuple.Item2.GetEnumerator();
		while (enumerator1.IfNext(out var item1) && enumerator2.IfNext(out var item2))
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
	/// <see langword="while"/> (enumerator1.IfNext(<see langword="out var"/> item1) &amp;&amp; enumerator2.IfNext(<see langword="out var"/> item2) &amp;&amp; enumerator3.IfNext(<see langword="out var"/> item3))<br/>
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
		while (enumerator1.IfNext(out var item1) && enumerator2.IfNext(out var item2) && enumerator3.IfNext(out var item3))
			yield return (item1, item2, item3);
	}

	/// <summary>
	/// <code>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; 0,<br/>
	/// <see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.IfCount(<see langword="out var"/> value) =&gt; value,<br/>
	/// <see langword="    "/>_ =&gt; count(@<paramref name="this"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> <see cref="int"/> count(<see cref="IEnumerable{T}"/> enumerable)<br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = enumerable.GetEnumerator();<br/>
	/// <see langword="    return"/> enumerator.Count();<br/>
	/// }
	/// </code>
	/// </summary>
	public static int Count<T>(this IEnumerable<T>? @this)
	{
		return @this switch
		{
			null => 0,
			_ when @this.IfCount(out var value) => value,
			_ => count(@this)
		};

		static int count(IEnumerable<T> enumerable)
		{
			using var enumerator = enumerable.GetEnumerator();
			return enumerator.Count();
		}
	}

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

	/// <summary>
	/// <c>=&gt; (<paramref name="first"/>, <paramref name="rest"/>) = @<paramref name="this"/>.GetEnumerator();</c>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out IEnumerable<T> rest)
		=> (first, rest) = @this.GetEnumerator();

	/// <summary>
	/// <c>=&gt; (<paramref name="first"/>, <paramref name="second"/>, <paramref name="rest"/>) = @<paramref name="this"/>.GetEnumerator();</c>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
		=> (first, second, rest) = @this.GetEnumerator();

	/// <summary>
	/// <c>=&gt; (<paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>, <paramref name="rest"/>) = @<paramref name="this"/>.GetEnumerator();</c>
	/// </summary>
	public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		=> (first, second, third, rest) = @this.GetEnumerator();

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <typeparamref name="T"/>[] array:<br/>
	/// <see langword="        "/>array.AsSpan().AsReadOnly().Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.AsSpan().Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }<br/>
	/// <br/>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="using"/> <see langword="var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="while"/> (enumerator.IfNext(<see langword="out var"/> item))<br/>
	/// <see langword="    "/>action(item);<br/>
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
				array.AsSpan().AsReadOnly().Do(action);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.AsSpan().Do(action);
				return;
		}

		action.AssertNotNull();

		using var enumerator = @this.GetEnumerator();
		while (enumerator.IfNext(out var item))
			action(item);
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <typeparamref name="T"/>[] array:<br/>
	/// <see langword="        "/>array.AsSpan().AsReadOnly().Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.AsSpan().Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }<br/>
	/// <br/>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <paramref name="between"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="using"/> <see langword="var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="if"/> (enumerator.IfNext(<see langword="out var"/> item))<br/>
	/// <see langword="    "/>action(item);<br/>
	/// <br/>
	/// <see langword="while"/> (enumerator.IfNext(<see langword="out"/> item))<br/>
	/// {<br/>
	/// <see langword="    "/>between();<br/>
	/// <see langword="    "/>action(item);<br/>
	/// }<br/>
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
				array.AsSpan().AsReadOnly().Do(action, between);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.AsSpan().Do(action, between);
				return;
		}

		action.AssertNotNull();
		between.AssertNotNull();

		using var enumerator = @this.GetEnumerator();
		if (enumerator.IfNext(out var item))
			action(item);

		while (enumerator.IfNext(out item))
		{
			between();
			action(item);
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <typeparamref name="T"/>[] array:<br/>
	/// <see langword="        "/>array.AsSpan().AsReadOnly().Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.AsSpan().Do(<paramref name="action"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }<br/>
	/// <br/>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> i = -1;<br/>
	/// <see langword="using"/> <see langword="var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="while"/> (enumerator.IfNext(<see langword="out var"/> item))<br/>
	/// <see langword="    "/>action(item, ++i);<br/>
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
				array.AsSpan().AsReadOnly().Do(action);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.AsSpan().Do(action);
				return;
		}

		action.AssertNotNull();

		var i = -1;
		using var enumerator = @this.GetEnumerator();
		while (enumerator.IfNext(out var item))
			action(item, ++i);
	}

	/// <summary>
	/// <code>
	/// <see langword="switch"/> (@<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    case null"/>:<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <typeparamref name="T"/>[] array:<br/>
	/// <see langword="        "/>array.AsSpan().AsReadOnly().Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// <see langword="    case"/> <see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="        "/>immutableArray.AsSpan().Do(<paramref name="action"/>, <paramref name="between"/>);<br/>
	/// <see langword="        return"/>;<br/>
	/// }<br/>
	/// <br/>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <paramref name="between"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> i = 0;<br/>
	/// <see langword="using"/> <see langword="var"/> enumerator = enumerable.GetEnumerator();<br/>
	/// <see langword="if"/> (enumerator.IfNext(<see langword="out var"/> item))<br/>
	/// <see langword="    "/>action(item, i);<br/>
	/// <br/>
	/// <see langword="while"/> (enumerator.IfNext(<see langword="out"/> item))<br/>
	/// {<br/>
	/// <see langword="    "/>between();<br/>
	/// <see langword="    "/>action(item, ++i);<br/>
	/// }<br/>
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
				array.AsSpan().AsReadOnly().Do(action, between);
				return;
			case ImmutableArray<T> immutableArray:
				immutableArray.AsSpan().Do(action, between);
				return;
		}

		action.AssertNotNull();
		between.AssertNotNull();

		var i = 0;
		using var enumerator = @this.GetEnumerator();
		if (enumerator.IfNext(out var item))
			action(item, i);

		while (enumerator.IfNext(out item))
		{
			between();
			action(item, ++i);
		}
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/><see cref="Task"/>.WaitAll(@<paramref name="this"/>.To(<paramref name="action"/>).ToArray(), <paramref name="token"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Func<T, Task> action, CancellationToken token = default)
	{
		action.AssertNotNull();

		if (@this.Any())
			Task.WaitAll(@this.Map(action).ToArray(), token);
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/><see cref="Task"/>.WaitAll(@<paramref name="this"/>.To(item =&gt; <paramref name="action"/>>(item, <paramref name="token"/>)).ToArray(), <paramref name="token"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken token = default)
	{
		action.AssertNotNull();

		if (@this.Any())
			Task.WaitAll(@this.Map(item => action(item, token)).ToArray(), token);
	}

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.Any())<br/>
	///	<see langword="    "/>@<paramref name="this"/>.To(item =&gt; <paramref name="action"/>(item, <paramref name="token"/>)).AllAsync&gt;<typeparamref name="T"/>&lt;();
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async Task DoAsync<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken token = default)
	{
		action.AssertNotNull();

		if (@this.Any())
			await @this.Map(item => action(item, token)).AllAsync<T>();
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
	public static async Task DoAsync<T>(this IEnumerable<T>? @this, Func<T, Task> action)
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
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Array{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/>_ =&gt; each(@<paramref name="this"/>, <paramref name="edit"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> <see cref="IEnumerable{T}"/> each(<see cref="IEnumerable{T}"/> enumerable, Func&lt;<typeparamref name="T"/>, <typeparamref name="T"/>&gt; edit)<br/>
	/// {<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="        yield return"/> edit(item);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this IEnumerable<T>? @this, Func<T, T> edit)
	{
		edit.AssertNotNull();

		return @this switch
		{
			null => Array<T>.Empty,
			T[] array => array.Each(edit),
			ImmutableArray<T> immutableArray => immutableArray.Each(edit),
			_ => each(@this, edit)
		};

		static IEnumerable<T> each(IEnumerable<T> enumerable, Func<T, T> edit)
		{
			foreach (var item in enumerable)
				yield return edit(item);
		}
	}

	/// <summary>
	/// <code>
	/// <paramref name="edit"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Array{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Each(<paramref name="edit"/>),<br/>
	///	<see langword="    "/>_ =&gt; each(@<paramref name="this"/>, <paramref name="edit"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> <see cref="IEnumerable{T}"/> each(<see cref="IEnumerable{T}"/> enumerable, Func&lt;<typeparamref name="T"/>, <see cref="int"/>, <typeparamref name="T"/>&gt; edit)<br/>
	/// {<br/>
	/// <see langword="    var"/> i = -1;<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="        yield return"/> edit(item, ++i);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Each<T>(this IEnumerable<T>? @this, Func<T, int, T> edit)
	{
		edit.AssertNotNull();

		return @this switch
		{
			null => Array<T>.Empty,
			T[] array => array.Each(edit),
			ImmutableArray<T> immutableArray => immutableArray.Each(edit),
			_ => each(@this, edit)
		};

		static IEnumerable<T> each(IEnumerable<T> enumerable, Func<T, int, T> edit)
		{
			var i = -1;
			foreach (var item in enumerable)
				yield return edit(item, ++i);
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().First();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? First<T>(this IEnumerable? @this)
		=> @this.If<T>().First();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? First<T>(this IEnumerable<T>? @this)
		=> @this.Get(0);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).First();</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? First<T>(this IEnumerable<T>? @this, Predicate<T> filter, T? defaultValue = default)
		=> @this.If(filter).First();

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
	/// <see langword="    if"/> (items.IfMemory(<see langword="out var"/> memory))<br/>
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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> Gather<T>(this IEnumerable<IEnumerable<T>?>? @this)
	{
		if (!@this.Any())
			yield break;

		foreach (var items in @this)
		{
			if (!items.Any())
				continue;

			if (items.IfMemory(out var memory))
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
	/// <c>=&gt; @<paramref name="this"/>.IfGet(<paramref name="index"/>, <see langword="out var"/> value) ? value : <see langword="default"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? Get<T>(this IEnumerable<T>? @this, Index index)
		=> @this.IfGet(index, out var value) ? value : default;

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/>.IfCount(<see langword="out var"/> count))<br/>
	/// <see langword="    "/>range = range.Normalize(count);<br/>
	/// <br/>
	/// <see langword="var"/> reverse = range.IsReverse();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Array{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; range.Map(i =&gt; array[i]),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; range.Map(i =&gt; immutableArray[i]),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; range.Map(i =&gt; list[i]),<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> readOnlyList =&gt; range.Map(i =&gt; readOnlyList[i]),<br/>
	///	<see langword="    "/>_ <see langword="when"/> reverse <see langword="is false"/> =&gt; getRange(@<paramref name="this"/>, range.Start.Value, range.Length()),<br/>
	///	<see langword="    "/>_ <see langword="when"/> reverse <see langword="is true"/> =&gt; new <see cref="Stack{T}"/>(getRange(@<paramref name="this"/>, range.End.Value, range.Length())),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Array{T}.Empty"/><br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> <see cref="IEnumerable{T}"/> getRange(<see cref="IEnumerable{T}"/> enumerable, <see cref="int"/> skip, <see cref="int"/> count)<br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = enumerable.GetEnumerator();<br/>
	/// <see langword="    if"/> (!enumerator.Move(skip))<br/>
	/// <see langword="        yield break"/>;<br/>
	/// <br/>
	/// <see langword="    while"/> (count &gt; 0 &amp;&amp; enumerator.MoveNext())<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        yield return"/> enumerator.Current;<br/>
	/// <see langword="        "/>--count;<br/>
	/// <see langword="    "/>}<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="IndexOutOfRangeException" />
	public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Range range)
	{
		if (@this.IfCount(out var count))
			range = range.Normalize(count);

		var reverse = range.IsReverse();

		return @this switch
		{
			null => Array<T>.Empty,
			T[] array => range.Map(i => array[i]),
			ImmutableArray<T> immutableArray => range.Map(i => immutableArray[i]),
			IList<T> list => range.Map(i => list[i]),
			IReadOnlyList<T> readOnlyList => range.Map(i => readOnlyList[i]),
			_ when reverse is false => getRange(@this, range.Start.Value, range.Length()),
			_ when reverse is true => new Stack<T>(getRange(@this, range.End.Value, range.Length())),
			_ => Array<T>.Empty,
		};

		static IEnumerable<T> getRange(IEnumerable<T> enumerable, int skip, int count)
		{
			using var enumerator = enumerable.GetEnumerator();
			if (!enumerator.Move(skip))
				yield break;

			while (count > 0 && enumerator.MoveNext())
			{
				yield return enumerator.Current;
				--count;
			}
		}
	}

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
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return values.All(value => @this.Has(value, comparer));
	}

	/// <summary>
	/// <c>=&gt; <paramref name="index"/>.Value &lt; @<paramref name="this"/>.Count();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
		=> index.Value < @this.Count();

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="EqualityComparer{T}.Default"/>;<br/>
	/// <br/>
	/// <see langword="return"/> <paramref name="values"/>.Any(value =&gt; @<paramref name="this"/>.Has(value, <paramref name="comparer"/>));
	/// </code>
	/// </summary>
	public static bool HasAny<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T>? comparer = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		return values.Any(value => @this.Has(value, comparer));
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is"/> <see cref="IEnumerable{T}"/> items)<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> items)<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// <br/>
	/// <see langword="foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="    if"/> (item <see langword="is"/> <typeparamref name="T"/> value)<br/>
	/// <see langword="        yield return"/> value;
	/// </code>
	/// </summary>
	public static IEnumerable<T> If<T>(this IEnumerable? @this)
	{
		if (@this is null)
			yield break;

		if (@this is IEnumerable<T> items)
			foreach (var item in items)
				yield return item;

		foreach (var item in @this)
			if (item is T value)
				yield return value;
	}

	/// <summary>
	/// <code>
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Array{T}.Empty"/>,<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.If(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.If(<paramref name="filter"/>),<br/>
	///	<see langword="    "/>_ =&gt; each(@<paramref name="this"/>, <paramref name="filter"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> <see cref="IEnumerable{T}"/> each(<see cref="IEnumerable{T}"/> enumerable, <see cref="Predicate{T}"/> filter)<br/>
	/// {<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="        if"/> (filter(item))<br/>
	/// <see langword="            yield return"/> item;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> If<T>(this IEnumerable<T>? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		return @this switch
		{
			null => Array<T>.Empty,
			T[] array => array.If(filter),
			ImmutableArray<T> immutableArray => immutableArray.If(filter),
			_ => each(@this, filter)
		};

		static IEnumerable<T> each(IEnumerable<T> enumerable, Predicate<T> filter)
		{
			foreach (var item in enumerable)
				if (filter(item))
					yield return item;
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
	/// <see langword="    if"/> (<paramref name="token"/>.IsCancellationRequested)<br/>
	/// <see langword="        yield break"/>;<br/>
	/// <br/>
	/// <see langword="    if"/> (<see langword="await"/> <paramref name="filter"/>(item))<br/>
	/// <see langword="        yield return"/> item;<br/>
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
			if (token.IsCancellationRequested)
				yield break;

			if (await filter(item))
				yield return item;
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
	/// <code>
	/// <paramref name="count"/> = <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/><see cref="ICollection{T}"/> collection =&gt; collection.Count,<br/>
	/// <see langword="    "/><see cref="ICollection"/> collection =&gt; collection.Count,<br/>
	/// <see langword="    "/><see cref="IReadOnlyCollection{T}"/> readOnlyCollection =&gt; readOnlyCollection.Count,<br/>
	/// <see langword="    "/>_ =&gt; -1<br/>
	/// };<br/>
	/// <see langword="return"/> <paramref name="count"/> &gt; -1;
	/// </code>
	/// </summary>
	/// <param name="count">
	/// Sets the <paramref name="count"/> if the underlying enumerable implements <see cref="ICollection{T}"/>, <see cref="IReadOnlyCollection{T}"/>, or <see cref="ICollection"/>.<br/>
	/// Otherwise sets the <paramref name="count"/> to -1.
	/// </param>
	/// <returns>Returns <see langword="true"/> if the underlying enumerable implements <see cref="ICollection{T}"/>, <see cref="IReadOnlyCollection{T}"/>, or <see cref="ICollection"/>.</returns>
	public static bool IfCount<T>([NotNullWhen(true)] this IEnumerable<T>? @this, out int count)
	{
		count = @this switch
		{
			ICollection<T> collection => collection.Count,
			ICollection collection => collection.Count,
			IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count,
			_ => -1
		};
		return count > -1;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().IfGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfFirst<T>([NotNullWhen(true)] this IEnumerable? @this, [NotNullWhen(true)] out T? item)
		=> @this.If<T>().IfGet(0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IfGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfFirst<T>([NotNullWhen(true)] this IEnumerable<T>? @this, [NotNullWhen(true)] out T? item)
		=> @this.IfGet(0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).IfGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfFirst<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter, [NotNullWhen(true)] out T? item)
		=> @this.If(filter).IfGet(0, out item);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; noGet(<see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; list.IfGet(<paramref name="index"/>, <see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> readOnlyList =&gt; readOnlyList.IfGet(<paramref name="index"/>, <see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/>_ <see langword="when"/> <paramref name="index"/>.IsFromEnd =&gt; <see cref="Enumerable{T}"/>.IfGet(@<paramref name="this"/>, <paramref name="index"/>.FromStart(@<paramref name="this"/>.Count()).Value, <see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/>_ =&gt; ifGet(@<paramref name="this"/>, <paramref name="index"/>.Value, <see langword="out"/> <paramref name="item"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static bool"/> ifGet(<see cref="IEnumerable{T}"/> enumerable, <see cref="Index"/> index, <see langword="out"/> <typeparamref name="T"/>? item)<br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = enumerable.GetEnumerator();<br/>
	/// <see langword="    return"/> enumerator.IfGet(index, <see langword="out"/> item);<br/>
	/// }<br/>
	/// <br/>
	/// <see langword="static bool"/> noGet(<see langword="out"/> <typeparamref name="T"/>? item)<br/>
	/// {<br/>
	/// <see langword="    "/>item = <see langword="default"/>;<br/>
	/// <see langword="    return false"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="IndexOutOfRangeException"/>
	public static bool IfGet<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index, [NotNullWhen(true)] out T? item)
	{
		return @this switch
		{
			null => noGet(out item),
			IList<T> list => list.IfGet(index, out item),
			IReadOnlyList<T> readOnlyList => readOnlyList.IfGet(index, out item),
			_ when index.IsFromEnd => ifGet(@this, index.FromStart(@this.Count()).Value, out item),
			_ => ifGet(@this, index.Value, out item)
		};

		static bool ifGet(IEnumerable<T> enumerable, Index index, out T? item)
		{
			using var enumerator = enumerable.GetEnumerator();
			return enumerator.IfGet(index.Value, out item);
		}

		static bool noGet(out T? item)
		{
			item = default;
			return false;
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().IfGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfLast<T>([NotNullWhen(true)] this IEnumerable? @this, [NotNullWhen(true)] out T? item)
		=> @this.If<T>().IfGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.IfGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfLast<T>([NotNullWhen(true)] this IEnumerable<T>? @this, [NotNullWhen(true)] out T? item)
		where T : class
		=> @this.IfGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).IfGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfLast<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter, [NotNullWhen(true)] out T? item)
		where T : class
		=> @this.If(filter).IfGet(^0, out item);

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
	public static bool IfMemory<T>([NotNullWhen(true)] this IEnumerable<T>? @this, out ReadOnlyMemory<T> memory)
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
	/// <c>=&gt; @<paramref name="this"/>.If(_ =&gt; _ <see langword="is not null"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
		=> @this.If(_ => _ is not null)!;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().IfSingle(<see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfSingle<T>([NotNullWhen(true)] this IEnumerable? @this, [NotNullWhen(true)] out T? item)
		=> @this.If<T>().IfSingle(out item);

	/// <summary>
	/// <code>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}"/>.NoGet(<see langword="out"/> <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; list.IfGet(0, <see langword="out"/> <paramref name="item"/>) &amp;&amp; list.Count == 1,<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> readOnlyList =&gt; readOnlyList.IfGet(0, <see langword="out"/> <paramref name="item"/>) &amp;&amp; readOnlyList.Count == 1,<br/>
	///	<see langword="    "/>_ =&gt; ifSingle(@<paramref name="this"/>, <see langword="out"/> <paramref name="item"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static bool"/> ifSingle(<see cref="IEnumerable{T}"/> enumerable, <see langword="out"/> <typeparamref name="T"/>? item)<br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = enumerable.GetEnumerator();<br/>
	/// <see langword="    return"/> enumerator.IfNext(<see langword="out"/> item);<br/>
	/// }<br/>
	/// <br/>
	/// <see langword="static bool"/> noGet(<see langword="out"/> <typeparamref name="T"/>? item)<br/>
	/// {<br/>
	/// <see langword="    "/>item = <see langword="default"/>;<br/>
	/// <see langword="    return false"/>;<br/>
	/// }
	/// </code>
	/// </summary>
	public static bool IfSingle<T>([NotNullWhen(true)] this IEnumerable<T>? @this, [NotNullWhen(true)] out T? item)
	{
		return @this switch
		{
			null => noGet(out item),
			IList<T> list => list.IfGet(0, out item) && list.Count == 1,
			IReadOnlyList<T> readOnlyList => readOnlyList.IfGet(0, out item) && readOnlyList.Count == 1,
			_ => ifSingle(@this, out item)
		};

		static bool ifSingle(IEnumerable<T> enumerable, out T? item)
		{
			using var enumerator = enumerable.GetEnumerator();
			return enumerator.IfNext(out item) && !enumerator.MoveNext();
		}

		static bool noGet(out T? item)
		{
			item = default;
			return false;
		}
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).IfSingle(<see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IfSingle<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter, [NotNullWhen(true)] out T? item)
		=> @this.If(filter).IfSingle(out item);

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
	/// <see langword="while"/> (enumerator.IfNext(<see langword="out var"/> item1) &amp;&amp; itemEnumerator.IfNext(<see langword="out var"/> item2))<br/>
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

		while (enumerator.IfNext(out var item1) && itemEnumerator.IfNext(out var item2))
		{
			if (!comparer.Equals(item1, item2))
				return false;
		}

		return !enumerator.MoveNext() && !itemEnumerator.MoveNext();
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>).SetEquals(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsSet<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		=> @this.ToHashSet(comparer).SetEquals(items ?? Array<T>.Empty);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>).SetEquals(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsSetBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
		=> @this.ToHashSetBy(getKey, keyComparer).SetEquals(items ?? Array<T>.Empty);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see cref="string"/>.Join(<paramref name="delimeter"/>, @<paramref name="this"/>) : <see cref="string.Empty"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Join<T>(this IEnumerable<T>? @this, char delimeter)
		=> @this is not null ? string.Join(delimeter, @this) : string.Empty;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/> ? <see cref="string"/>.Join(<paramref name="delimeter"/>)) : <see cref="string.Empty"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Join<T>(this IEnumerable<T>? @this, string delimeter)
		=> @this is not null ? string.Join(delimeter, @this) : string.Empty;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().Last();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? Last<T>(this IEnumerable? @this)
		=> @this.If<T>().Last();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(^1, <paramref name="defaultValue"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? Last<T>(this IEnumerable<T>? @this)
		=> @this.Get(^1);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).Last();</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static T? Last<T>(this IEnumerable<T>? @this, Predicate<T> filter)
		=> @this.If(filter).Last();

	/// <summary>
	/// <code>
	/// <see langword="return"/> @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; Array&lt;<typeparamref name="V"/>&gt;.Empty,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="IList{T}"/> list =&gt; list.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="IReadOnlyList{T}"/> readOnlyList =&gt; readOnlyList.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/>_ =&gt; each(@<paramref name="this"/>, <paramref name="map"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> IEnumerable&lt;<typeparamref name="V"/>&gt; each(IEnumerable&lt;<typeparamref name="T"/>&gt; enumerable, Func&lt;<typeparamref name="T"/>, <typeparamref name="V"/>&gt; map)<br/>
	/// {<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="        yield return"/> map(item);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this IEnumerable<T>? @this, Func<T, V> map)
	{
		return @this switch
		{
			null => Array<V>.Empty,
			T[] array => array.Map(map),
			ImmutableArray<T> immutableArray => immutableArray.Map(map),
			IList<T> list => list.Map(map),
			IReadOnlyList<T> readOnlyList => readOnlyList.Map(map),
			_ => each(@this, map)
		};

		static IEnumerable<V> each(IEnumerable<T> enumerable, Func<T, V> map)
		{
			foreach (var item in enumerable)
				yield return map(item);
		}
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; Array&lt;<typeparamref name="V"/>&gt;.Empty,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="IList{T}"/> list =&gt; list.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="IReadOnlyList{T}"/> readOnlyList =&gt; readOnlyList.Map(<paramref name="map"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Map(@<paramref name="this"/>, <paramref name="map"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> IEnumerable&lt;<typeparamref name="V"/>&gt; each(IEnumerable&lt;<typeparamref name="T"/>&gt; enumerable, Func&lt;<typeparamref name="T"/>, <see cref="int"/>, <typeparamref name="V"/>&gt; map)<br/>
	/// {<br/>
	/// <see langword="    var"/> i = -1;<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="        yield return"/> map(item, ++i);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> Map<T, V>(this IEnumerable<T>? @this, Func<T, int, V> map)
	{
		return @this switch
		{
			null => Array<V>.Empty,
			T[] array => array.Map(map),
			ImmutableArray<T> immutableArray => immutableArray.Map(map),
			IList<T> list => list.Map(map),
			IReadOnlyList<T> readOnlyList => readOnlyList.Map(map),
			_ => each(@this, map)
		};

		static IEnumerable<V> each(IEnumerable<T> enumerable, Func<T, int, V> map)
		{
			var i = -1;
			foreach (var item in enumerable)
				yield return map(item, ++i);
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
	/// hashSet.IntersectWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.IntersectWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.IntersectWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> MatchBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.IntersectWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="Comparer{T}.Default"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.IfFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, <paramref name="comparer"/>.Maximum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	public static T? Maximum<T>(this IEnumerable<T>? @this, T? defaultValue = default, IComparer<T>? comparer = null)
	{
		comparer ??= Comparer<T>.Default;
		return @this.IfFirst(out var initial) ? @this.Aggregate(initial, comparer.Maximum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <paramref name="getValue"/>.AssertNotNull();<br/>
	/// <br/>
	/// <paramref name="valueComparer"/> ??= Comparer&lt;<typeparamref name="V"/>&gt;.Default;<br/>
	/// <see langword="var"/> comparer = <see langword="new"/> <see cref="CustomComparer{T}"/>((item1, item2) =&gt; <paramref name="valueComparer"/>.Compare(<paramref name="getValue"/>(item1!), <paramref name="getValue"/>(item2!)));<br/>
	/// <see langword="return"/> @<paramref name="this"/>.IfFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, comparer.Maximum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T? MaximumBy<T, V>(this IEnumerable<T>? @this, Func<T, V> getValue, T? defaultValue = default, IComparer<V>? valueComparer = null)
	{
		getValue.AssertNotNull();

		valueComparer ??= Comparer<V>.Default;
		var comparer = new CustomComparer<T>((item1, item2) => valueComparer.Compare(getValue(item1!), getValue(item2!)));
		return @this.IfFirst(out var initial) ? @this.Aggregate(initial, comparer.Maximum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="Comparer{T}.Default"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.IfFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, <paramref name="comparer"/>.Minimum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	public static T? Minimum<T>(this IEnumerable<T>? @this, T? defaultValue = default, IComparer<T>? comparer = null)
	{
		comparer ??= Comparer<T>.Default;
		return @this.IfFirst(out var initial) ? @this.Aggregate(initial, comparer.Minimum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <paramref name="getValue"/>.AssertNotNull();<br/>
	/// <br/>
	/// <paramref name="valueComparer"/> ??= Comparer&lt;<typeparamref name="V"/>&gt;.Default;<br/>
	/// <see langword="var"/> comparer = <see langword="new"/> <see cref="CustomComparer{T}"/>((item1, item2) =&gt; <paramref name="valueComparer"/>.Compare(<paramref name="getValue"/>(item1!), <paramref name="getValue"/>(item2!)));<br/>
	/// <see langword="return"/> @<paramref name="this"/>.IfFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, comparer.Minimum) : <paramref name="defaultValue"/>;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static T? MinimumBy<T, V>(this IEnumerable<T>? @this, Func<T, V> getValue, T? defaultValue = default, IComparer<V>? valueComparer = null)
	{
		getValue.AssertNotNull();

		valueComparer ??= Comparer<V>.Default;
		var comparer = new CustomComparer<T>((item1, item2) => valueComparer.Compare(getValue(item1!), getValue(item2!)));
		return @this.IfFirst(out var initial) ? @this.Aggregate(initial, comparer.Minimum) : defaultValue;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.SymmetricExceptWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> NotMatch<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.SymmetricExceptWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.SymmetricExceptWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> NotMatchBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.SymmetricExceptWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(<paramref name="count"/>..^0));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> Skip<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(count..^0);

	/// <summary>
	/// <code>
	/// <paramref name="skip"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IfMemory(<see langword="out var"/> memory))<br/>
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
	/// <see langword="    while"/> (enumerator.IfNext(<see langword="out var"/> item))<br/>
	/// <see langword="        if"/> (!<paramref name="skip"/>(item))<br/>
	/// <see langword="            break"/>;<br/>
	/// <see langword="    while"/> (enumerator.IfNext(<see langword="out var"/> item))<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> @this, Predicate<T> skip)
	{
		skip.AssertNotNull();

		if (!@this.Any())
			yield break;

		if (@this.IfMemory(out var memory))
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
			while (enumerator.IfNext(out var item))
				if (!skip(item))
					break;
			while (enumerator.IfNext(out var item))
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

	public static long Sum(this IEnumerable<int>? @this)
	{
		var result = 0L;
		@this.Do(value => result += value);
		return result;
	}

	public static long Sum(this IEnumerable<long>? @this)
	{
		var result = 0L;
		@this.Do(value => result += value);
		return result;
	}

	public static decimal Sum(this IEnumerable<decimal>? @this)
	{
		var result = 0M;
		@this.Do(value => result += value);
		return result;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0..<paramref name="count"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> Take<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(0..count);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0..<paramref name="count"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(Index.FromEnd(count)..^0);

	/// <summary>
	/// <code>
	/// <paramref name="take"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/>.IfMemory(<see langword="out var"/> memory))<br/>
	/// {<br/>
	/// <see langword="    var"/> count = memory.Length;<br/>
	/// <see langword="    var"/> i = -1;<br/>
	/// <see langword="    while"/> (++i &lt; count &amp;&amp; <paramref name="take"/>(memory.Span[i]))<br/>
	/// <see langword="        yield return"/> memory.Span[i];<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    using var"/> enumerator = @<paramref name="this"/>.GetEnumerator();<br/>
	/// <see langword="    while"/> (enumerator.IfNext(<see langword="out var"/> item) &amp;&amp; <paramref name="take"/>(item))<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> @this, Predicate<T> take)
	{
		take.AssertNotNull();

		if (!@this.Any())
			yield break;

		if (@this.IfMemory(out var memory))
		{
			var count = memory.Length;
			var i = -1;
			while (++i < count && take(memory.Span[i]))
				yield return memory.Span[i];
		}
		else
		{
			using var enumerator = @this.GetEnumerator();
			while (enumerator.IfNext(out var item) && take(item))
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
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.ToQueue().ToArray()<br/>
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
			_ => @this.ToQueue().ToArray()
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
	public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory, IEqualityComparer<K>? comparer = null)
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
	public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K>? comparer = null)
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
	public static IDictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K>? comparer = null)
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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<Tuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
		where K : notnull
		=> @this.Map(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(tuple =&gt; <see cref="KeyValuePair"/>.Create(tuple.Item1, tuple.Item2)).ToDictionary(<paramref name="comparer"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<ValueTuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
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
		=> new HashSet<T>(@this ?? Array<T>.Empty, comparer);

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
		return new HashSet<T>(@this ?? Array<T>.Empty, comparer);
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
	/// <paramref name="filter"/>.AssertNotNull();<br/>
	/// <br/>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; Array&lt;<see cref="int"/>&gt;.Empty,<br/>
	///	<see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><typeparamref name="T"/>[] array =&gt; array.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; list.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> readOnlyList =&gt; readOnlyList.ToIndex(<paramref name="filter"/>),<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.ToIndex(@<paramref name="this"/>, <paramref name="filter"/>)<br/>
	/// };<br/>
	/// <br/>
	/// <see langword="static"/> IEnumerable&lt;int&gt; toIndex(<see cref="IEnumerable{T}"/> enumerable, <see cref="Predicate{T}"/> filter)<br/>
	/// {<br/>
	/// <see langword="    var"/> i = 0;<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> enumerable)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        if"/> (filter(item))<br/>
	/// <see langword="            yield return"/> i;<br/>
	/// <see langword="        ++i;"/><br/>
	/// <see langword="    "/>}<br/>
	/// }
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, Predicate<T> filter)
	{
		filter.AssertNotNull();

		return @this switch
		{
			null => Array<int>.Empty,
			ImmutableArray<T> immutableArray => (0..immutableArray.Length).If(i => filter(immutableArray[i])),
			T[] array => (0..array.Length).If(i => filter(array[i])),
			IList<T> list => (0..list.Count).If(i => filter(list[i])),
			IReadOnlyList<T> readOnlyList => (0..readOnlyList.Count).If(i => filter(readOnlyList[i])),
			_ => toIndex(@this, filter)
		};

		static IEnumerable<int> toIndex(IEnumerable<T> enumerable, Predicate<T> filter)
		{
			var i = 0;
			foreach (var item in enumerable)
			{
				if (filter(item))
					yield return i;
				++i;
			}
		}
	}

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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static List<T> ToList<T>(this IEnumerable<T>? @this)
		=> new List<T>(@this ?? Array<T>.Empty);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(<paramref name="map"/>).Gather();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IEnumerable<V> ToMany<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map)
		=> @this.Map(map).Gather();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/>
	/// ? <see langword="new"/> <see cref="Queue{T}"/>(@<paramref name="this"/>)
	/// : <see langword="new"/> <see cref="Queue{T}"/>(0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this)
		=> new Queue<T>(@this ?? Array<T>.Empty);

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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Stack<T> ToStack<T>(this IEnumerable<T>? @this)
		=> new Stack<T>(@this ?? Array<T>.Empty);

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.UnionWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.UnionWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.UnionWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> UnionBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.UnionWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>);<br/>
	/// hashSet.ExceptWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
	{
		var hashSet = @this.ToHashSet(comparer);
		hashSet.ExceptWith(items ?? Array<T>.Empty);
		return hashSet;
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> hashSet = @<paramref name="this"/>.ToHashSetBy(<paramref name="getKey"/>, <paramref name="keyComparer"/>);<br/>
	/// hashSet.ExceptWith(<paramref name="items"/> ?? <see cref="Array{T}.Empty"/>);<br/>
	/// <see langword="return"/> hashSet;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static HashSet<T> WithoutBy<T, K>(this IEnumerable<T>? @this, IEnumerable<T>? items, Func<T, K> getKey, IEqualityComparer<K>? keyComparer = null)
	{
		var hashSet = @this.ToHashSetBy(getKey, keyComparer);
		hashSet.ExceptWith(items ?? Array<T>.Empty);
		return hashSet;
	}
}
