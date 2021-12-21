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
		where T : unmanaged
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
		where T : unmanaged
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
	public static bool All<T>(this IEnumerable<T>? @this, Predicate<T> filter)
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
	/// <c>=&gt; @<paramref name="this"/>.And(<paramref name="sets"/>.Gather());</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? sets)
		=> @this.And(sets.Gather());

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is not null"/>)<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> @<paramref name="this"/>)<br/>
	/// <see langword="        yield return"/> item;<br/>
	/// <br/>
	/// <see langword="if"/> (<paramref name="items"/> <see langword="is not null"/>)<br/>
	/// <see langword="    foreach"/> (<see langword="var"/> item <see langword="in"/> items)<br/>
	/// <see langword="        if"/> (item <see langword="is not null"/>)<br/>
	/// <see langword="            yield return"/> item;
	/// </code>
	/// </summary>
	public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<T?>? items)
	{
		if (@this is not null)
			foreach (var item in @this)
				yield return item;

		if (items is not null)
			foreach (var item in items)
				if (item is not null)
					yield return item;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.And(<paramref name="items"/> <see langword="as"/> <see cref="IEnumerable{T}"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params T?[]? items)
		=> @this.And(items as IEnumerable<T>);

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
	///	<see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; collection.Count &gt; 0,<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; collection.Count &gt; 0,<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Any(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
		=> @this switch
		{
			null => false,
			ICollection<T> collection => collection.Count > 0,
			IReadOnlyCollection<T> collection => collection.Count > 0,
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
	/// <c>=&gt; @<paramref name="this"/>.Any(<paramref name="item"/>.Equals);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool AnyOf<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T item)
		where T : IEquatable<T>
		=> @this.Any(item.Equals);

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
	///	<see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; <paramref name="count"/> &gt;= collection.Count,<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; <paramref name="count"/> &gt;= collection.Count,<br/>
	///	<see langword="    "/>_ =&gt; @<paramref name="this"/>.GetEnumerator().Skip(<paramref name="count"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool AtLeast<T>(this IEnumerable<T>? @this, int count)
		=> @this switch
		{
			null => false,
			ICollection<T> collection => count >= collection.Count,
			IReadOnlyCollection<T> collection => count >= collection.Count,
			_ => @this.GetEnumerator().Move(count)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see langword="false"/>,<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; <paramref name="count"/> &lt;= collection.Count,<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; <paramref name="count"/> &lt;= collection.Count,<br/>
	///	<see langword="    "/>_ =&gt; !@<paramref name="this"/>.GetEnumerator().Skip(<paramref name="count"/> + 1)<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool AtMost<T>(this IEnumerable<T>? @this, int count)
		=> @this switch
		{
			null => false,
			ICollection<T> collection => count <= collection.Count,
			IReadOnlyCollection<T> collection => count <= collection.Count,
			_ => !@this.GetEnumerator().Move(count + 1)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; 0,<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; collection.Count,<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; collection.Count,<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Count(@<paramref name="this"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	public static int Count<T>(this IEnumerable<T>? @this)
		=> @this switch
		{
			null => 0,
			ICollection<T> collection => collection.Count,
			IReadOnlyCollection<T> collection => collection.Count,
			_ => Enumerable<T>.Count(@this)
		};

	/// <summary>
	/// <c>=&gt; <paramref name="item"/> <see langword="is not null"/>
	/// ? @<paramref name="this"/>.If(<paramref name="item"/>.Equals).Count()
	/// : @<paramref name="this"/>.If(_ =&gt; _ <see langword="is null"/>).Count();</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static int CountOf<T>(this IEnumerable<T>? @this, T item)
		where T : IEquatable<T>
		=> item is not null ? @this.If(item.Equals).Count() : @this.If(_ => _ is null).Count();

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
			Task.WaitAll(@this.To(action).ToArray(), cancellationToken);
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
			Task.WaitAll(@this.To(item => action(item, cancellationToken)).ToArray(), cancellationToken);
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
			await @this.To(item => action(item, cancellationToken)).AllAsync<T>();
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
			await @this.To(action).AllAsync<T>();
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="    return"/>;<br/>
	///	<br/>
	/// <see langword="if"/> (<paramref name="options"/> <see langword="is not null"/>)<br/>
	///	<see langword="    "/><see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>);<br/>
	/// <see langword="else"/><br/>
	///	<see langword="    "/><see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void DoInParallel<T>(this IEnumerable<T>? @this, Action<T, ParallelLoopState, long> action, ParallelOptions? options = null)
	{
		if (@this is null)
			return;

		if (options is not null)
			Parallel.ForEach(@this, options, action);
		else
			Parallel.ForEach(@this, action);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="    return"/>;<br/>
	///	<br/>
	/// <see langword="if"/> (<paramref name="options"/> <see langword="is not null"/>)<br/>
	///	<see langword="    "/><see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>);<br/>
	/// <see langword="else"/><br/>
	///	<see langword="    "/><see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void DoInParallel<T>(this IEnumerable<T>? @this, Action<T, ParallelLoopState> action, ParallelOptions? options = null)
	{
		if (@this is null)
			return;

		if (options is not null)
			Parallel.ForEach(@this, options, action);
		else
			Parallel.ForEach(@this, action);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="    return"/>;<br/>
	///	<br/>
	/// <see langword="if"/> (<paramref name="options"/> <see langword="is not null"/>)<br/>
	///	<see langword="    "/><see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="options"/>, <paramref name="action"/>);<br/>
	/// <see langword="else"/><br/>
	///	<see langword="    "/><see cref="Parallel"/>.ForEach(@<paramref name="this"/>, <paramref name="action"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void DoInParallel<T>(this IEnumerable<T>? @this, Action<T> action, ParallelOptions? options = null)
	{
		if (@this is null)
			return;

		if (options is not null)
			Parallel.ForEach(@this, options, action);
		else
			Parallel.ForEach(@this, action);
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
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().First();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? First<T>(this IEnumerable? @this)
		=> @this.If<T>().First();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? First<T>(this IEnumerable<T>? @this)
		=> @this.Get(0);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).First();</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? First<T>(this IEnumerable<T>? @this, Predicate<T> filter)
		=> @this.If(filter).Get(0);

	/// <summary>
	/// <code>
	/// <see langword="if"/> (!@<paramref name="this"/>.Any())<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="foreach"/> (<see langword="var"/> items <see langword="in"/> @<paramref name="this"/>)<br/>
	/// {<br/>
	/// <see langword="    "/><see cref="int"/> count;<br/>
	/// <see langword="    switch"/> (items)<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        case null"/>:<br/>
	/// <see langword="            continue"/>;<br/>
	/// <see langword="        case "/><typeparamref name="T"/>[]:<br/>
	/// <see langword="            "/>count = array.Length;<br/>
	/// <see langword="            for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="                yield return"/> array[i];<br/>
	/// <see langword="            continue"/>;<br/>
	/// <see langword="        case "/><see cref="ImmutableArray{T}"/> immutableArray:<br/>
	/// <see langword="            "/>count = immutableArray.Length;<br/>
	/// <see langword="            for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="                yield return"/> immutableArray[i];<br/>
	/// <see langword="            continue"/>;<br/>
	/// <see langword="        case "/><see cref="List{T}"/> list:<br/>
	/// <see langword="            "/>count = list.Count;<br/>
	/// <see langword="            for"/> (<see langword="var"/> i = 0; i &lt; count; ++i)<br/>
	/// <see langword="                yield return"/> list[i];<br/>
	/// <see langword="            continue"/>;<br/>
	/// <see langword="        default"/>:<br/>
	/// <see langword="            foreach"/> (<see langword="var"/> item <see langword="in"/> items)<br/>
	/// <see langword="                yield return"/> item;<br/>
	/// <see langword="            continue"/>;<br/>
	/// <see langword="    "/>}<br/>
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
			int count;
			switch (items)
			{
				case null:
					continue;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
						yield return array[i];
					continue;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
						yield return immutableArray[i];
					continue;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
						yield return list[i];
					continue;
				default:
					foreach (var item in items)
						yield return item;
					continue;
			}
		}
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list <see langword="when"/> list.Has(<paramref name="index"/>) =&gt; list[<paramref name="index"/>],<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> list <see langword="when"/> list.Has(<paramref name="index"/>) =&gt; list[<paramref name="index"/>],<br/>
	///	<see langword="    null or"/> <see cref="IList{T}"/> <see langword="or"/> <see cref="IReadOnlyList{T}"/> =&gt; <see langword="null"/>,<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; <see cref="Enumerable{T}"/>.Get(@<paramref name="this"/>, <paramref name="index"/>.FromStart(collection.Count).Value)<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; <see cref="Enumerable{T}"/>.Get(@<paramref name="this"/>, <paramref name="index"/>.FromStart(collection.Count).Value)<br/>
	///	<see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.Get(@<paramref name="this"/>, <paramref name="index"/>.IsFromEnd ? <paramref name="index"/>.FromStart(@<paramref name="this"/>.Count()).Value : <paramref name="index"/>.Value)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="IndexOutOfRangeException" />
	public static T? Get<T>(this IEnumerable<T>? @this, Index index)
		=> @this switch
		{
			IList<T> list when list.Has(index) => list[index],
			IReadOnlyList<T> list when list.Has(index) => list[index],
			null or IList<T> or IReadOnlyList<T> => default,
			ICollection<T> collection => Enumerable<T>.Get(@this, index.FromStart(collection.Count).Value),
			IReadOnlyCollection<T> collection => Enumerable<T>.Get(@this, index.FromStart(collection.Count).Value),
			_ => Enumerable<T>.Get(@this, index.IsFromEnd ? index.FromStart(@this.Count()).Value : index.Value)
		};

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

		(K Key, V Value)[] items = @this.To(item => (keyFactory(item), item)).ToArray();
		var keys = items.To(_ => _.Key).ToHashSet(comparer);
		return keys.ToDictionary(key => items.If(_ => keys.Comparer.Equals(_.Key, key)).To(_ => _.Value));
	}

	/// <summary>
	/// <c>=&gt; <paramref name="values"/>.All(@<paramref name="this"/>.Has);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values)
		where T : IEquatable<T>
		=> values.All(@this.Has);

	/// <summary>
	/// <c>=&gt; <paramref name="values"/>.All(value =&gt; @<paramref name="this"/>.Has(value, <paramref name="comparer"/>));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T> comparer)
		=> values.All(value => @this.Has(value, comparer));

	/// <summary>
	/// <code>
	/// =&gt; <paramref name="index"/>.Value &lt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; 0,<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; collection.Count,<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; collection.Count,<br/>
	///	<see langword="    "/>_ =&gt; @<paramref name="this"/>.Count()<br/>
	/// };
	/// </code>
	/// </summary>
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
		=> index.Value < @this switch
		{
			null => 0,
			ICollection<T> collection => collection.Count,
			IReadOnlyCollection<T> collection => collection.Count,
			_ => @this.Count()
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToIndex(<paramref name="value"/>).Any();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value)
		where T : IEquatable<T>
		=> @this.ToIndex(value).Any();

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.ToIndex(value, <paramref name="comparer"/>).Any();</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
		=> @this.ToIndex(value, comparer).Any();

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
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().Get(^0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Last<T>(this IEnumerable? @this)
		=> @this.If<T>().Get(^0);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Get(^0);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Last<T>(this IEnumerable<T>? @this)
		=> @this.Get(^0);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).Get(^0);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Last<T>(this IEnumerable<T>? @this, Predicate<T> filter)
		=> @this.If(filter).Get(^0);

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
	/// <paramref name="comparer"/> ??= <see cref="Comparer{T}.Default"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.TryFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, <paramref name="comparer"/>.Maximum) : <see langword="default"/>;
	/// </code>
	/// </summary>
	public static T Maximum<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
		where T : unmanaged
	{
		comparer ??= Comparer<T>.Default;
		return @this.TryFirst(out var initial) ? @this.Aggregate(initial, comparer.Maximum) : default;
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/> ??= <see cref="Comparer{T}.Default"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.TryFirst(<see langword="out var"/> initial) ? @<paramref name="this"/>.Aggregate(initial, <paramref name="comparer"/>.Minimum) : <see langword="default"/>;
	/// </code>
	/// </summary>
	public static T Minimum<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
		where T : unmanaged
	{
		comparer ??= Comparer<T>.Default;
		return @this.TryFirst(out var initial) ? @this.Aggregate(initial, comparer.Minimum) : default;
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
	/// <c>=&gt; @<paramref name="this"/>.Get(<paramref name="count"/>..^1));</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IEnumerable<T> Skip<T>(this IEnumerable<T> @this, int count)
		=> @this.Get(count..^0);

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
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see cref="Enumerable"/>&lt;V&gt;.Empty,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.To(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.To(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.To(<paramref name="map"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.To(@<paramref name="this"/>, <paramref name="map"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, int, V> map)
		=> @this switch
		{
			null => Enumerable<V>.Empty,
			T[] array => array.To(map),
			ImmutableArray<T> immutableArray => immutableArray.To(map),
			List<T> list => list.To(map),
			_ => Enumerable<T>.To(@this, map)
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see cref="Enumerable"/>&lt;V&gt;.Empty,<br/>
	/// <see langword="    "/><typeparamref name="T"/>[] array =&gt; array.To(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.To(<paramref name="map"/>),<br/>
	/// <see langword="    "/><see cref="List{T}"/> list =&gt; list.To(<paramref name="map"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Enumerable{T}"/>.To(@<paramref name="this"/>, <paramref name="map"/>)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, V> map)
		=> @this switch
		{
			null => Enumerable<V>.Empty,
			T[] array => array.To(map),
			ImmutableArray<T> immutableArray => immutableArray.To(map),
			List<T> list => list.To(map),
			_ => Enumerable<T>.To(@this, map)
		};

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
			T[] array => array.AsSpan().ToArray(),
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
	public static async IAsyncEnumerable<V> ToAsync<T, V>(this IEnumerable<T>? @this, Func<T?, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
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
	public static async IAsyncEnumerable<V> ToAsync<T, V>(this IEnumerable<T>? @this, Func<T?, CancellationToken, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
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
	/// <paramref name="map"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.To(<paramref name="map"/>).ToCsv();
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static string ToCSV<T>(this IEnumerable<T>? @this, Func<T, string> map)
	{
		map.AssertNotNull();

		return @this.To(map).ToCSV();
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
		=> @this.To(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.To(tuple =&gt; <see cref="KeyValuePair"/>.Create(tuple.Item1, tuple.Item2)).ToDictionary(<paramref name="comparer"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<ValueTuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
		where K : notnull
		=> @this.To(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

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
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///		_ <see langword="when"/> !@<paramref name="this"/>.Any() =&gt; <see cref="ImmutableQueue{T}.Empty"/>,<br/>
	///		T[] array =&gt; <see cref="ImmutableQueue"/>.Create(array),<br/>
	///		<see cref="List{T}"/> list =&gt; <see cref="ImmutableQueue"/>.Create(list.ToArray()),<br/>
	///		_ =&gt; <see cref="ImmutableQueue.CreateRange{T}"/>(@<paramref name="this"/>)<br/>
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
	///		_ <see langword="when"/> !@<paramref name="this"/>.Any() =&gt; <see cref="ImmutableStack{T}.Empty"/>,<br/>
	///		T[] array =&gt; <see cref="ImmutableStack"/>.Create(array),<br/>
	///		<see cref="List{T}"/> list =&gt; <see cref="ImmutableStack"/>.Create(list.ToArray()),<br/>
	///		_ =&gt; <see cref="ImmutableStack.CreateRange{T}"/>(@<paramref name="this"/>)<br/>
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
	///		<see langword="null"/> =&gt; Enumerable&lt;<see cref="int"/>&gt;.Empty,<br/>
	///		<see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.ToIndex(<paramref name="filter"/>),<br/>
	///		T[] array =&gt; array.ToIndex(<paramref name="filter"/>),<br/>
	///		<see cref="List{T}"/> list =&gt; list.ToIndex(<paramref name="filter"/>),<br/>
	///		_ =&gt; <see cref="Enumerable{T}"/>.ToIndex(@<paramref name="this"/>, <paramref name="filter"/>)<br/>
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
	/// <c>=&gt; <paramref name="item"/> <see langword="is not null"/> ? @<paramref name="this"/>.ToIndex(item.Equals) : @<paramref name="this"/>.ToIndex(value =&gt; value <see langword="is null"/>);</c>
	/// </summary>
	public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T item)
		where T : IEquatable<T>
		=> item is not null ? @this.ToIndex(item.Equals) : @this.ToIndex(value => value is null);

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/>.AssertNotNull();<br/>
	/// <br/>
	/// @<paramref name="this"/>.ToIndex(value =&gt; <paramref name="comparer"/>.Equals(value, <paramref name="item"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T item, IEqualityComparer<T> comparer)
	{
		comparer.AssertNotNull();

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
		=> @this.To(map).Gather();

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
	/// <see langword="null"/> =&gt; <see cref="ReadOnlySpan{T}"/>.Empty,<br/>
	/// <see cref="ImmutableArray{T}"/> immutableArray =&gt; immutableArray.AsSpan(),<br/>
	/// T[] array =&gt; array.AsSpan(),<br/>
	/// <see cref="List{T}"/> list =&gt; list.ToArray().AsSpan(),<br/>
	/// _ =&gt; @<paramref name="this"/>.ToArray().AsSpan(),<br/>
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
			_ => @this.ToArray().AsSpan(),
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///		<see langword="null"/> =&gt; <see cref="Span{T}"/>.Empty,<br/>
	///		<typeparamref name="T"/>[] array =&gt; array.AsSpan(),<br/>
	///		<see cref="List{T}"/> list =&gt; list.ToArray().AsSpan(),<br/>
	///		_ =&gt; @<paramref name="this"/>.ToArray().AsSpan()<br/>
	/// };
	/// </code>
	/// </summary>
	public static Span<T> ToSpan<T>(this IEnumerable<T>? @this)
		=> @this switch
		{
			null => Span<T>.Empty,
			T[] array => array.AsSpan(),
			List<T> list => list.ToArray().AsSpan(),
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
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().TryGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryFirst<T>(this IEnumerable? @this, out T? item)
		=> @this.If<T>().TryGet(0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryFirst<T>(this IEnumerable<T>? @this, out T? item)
		=> @this.TryGet(0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).TryGet(0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryFirst<T>(this IEnumerable<T>? @this, Predicate<T> filter, out T? item)
		=> @this.If(filter).TryGet(0, out item);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    null"/> =&gt; <see cref="Enumerable{T}"/>.NoGet(out <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IList{T}"/> list =&gt; list.TryGet(<paramref name="index"/>, out <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="IReadOnlyList{T}"/> list =&gt; list.TryGet(<paramref name="index"/>, out <paramref name="item"/>),<br/>
	///	<see langword="    "/><see cref="ICollection{T}"/> collection =&gt; @<paramref name="this"/>.GetEnumerator().Get(<paramref name="index"/>.FromStart(collection.Count).Value),<br/>
	///	<see langword="    "/><see cref="IReadOnlyCollection{T}"/> collection =&gt; @<paramref name="this"/>.GetEnumerator().Get(<paramref name="index"/>.FromStart(collection.Count).Value),<br/>
	///	<see langword="    "/>_ =&gt; @<paramref name="this"/>.GetEnumerator().Get(<paramref name="index"/>.IsFromEnd ? <paramref name="index"/>.FromStart(@<paramref name="this"/>.Count()) : <paramref name="index"/>.Value)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="IndexOutOfRangeException" />
	public static bool TryGet<T>(this IEnumerable<T>? @this, Index index, out T? item)
		=> @this switch
		{
			null => Enumerable<T>.NoGet(out item),
			IList<T> list => list.TryGet(index, out item),
			IReadOnlyList<T> list => list.TryGet(index, out item),
			ICollection<T> collection => Enumerable<T>.TryGet(@this, index.FromStart(collection.Count).Value, out item),
			IReadOnlyCollection<T> collection => Enumerable<T>.TryGet(@this, index.FromStart(collection.Count).Value, out item),
			_ => Enumerable<T>.TryGet(@this, index.IsFromEnd ? index.FromStart(@this.Count()).Value : index.Value, out item)
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().TryGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryLast<T>(this IEnumerable? @this, out T? item)
		=> @this.If<T>().TryGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.TryGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryLast<T>(this IEnumerable<T>? @this, out T? item)
		where T : class
		=> @this.TryGet(^0, out item);

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.If(<paramref name="filter"/>).TryGet(^0, <see langword="out"/> <paramref name="item"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryLast<T>(this IEnumerable<T>? @this, Predicate<T> filter, out T? item)
		where T : class
		=> @this.If(filter).TryGet(^0, out item);

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
}
