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

namespace TypeCache.Collections.Extensions
{
	public static class IEnumerableExtensions
	{
		/// <exception cref="ArgumentNullException"/>
		public static T Aggregate<T>(this IEnumerable<T>? @this, T initialValue, Func<T, T, T> aggregator)
			where T : unmanaged
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = initialValue;
			@this.Do(item => result = aggregator(result, item));
			return result;
		}

		/// <exception cref="ArgumentNullException"/>
		public static async ValueTask<T> AggregateAsync<T>(this IEnumerable<T>? @this, T initialValue, Func<T, T, ValueTask<T>> aggregator)
			where T : unmanaged
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = initialValue;
			await @this.DoAsync(async item => result = await aggregator(result, item));
			return result;
		}

		/// <summary>
		/// <c>!@<paramref name="this"/>.If(item =&gt; !<paramref name="filter"/>(item)).Any()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public static bool All<T>(this IEnumerable<T>? @this, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return !@this.If(item => !filter(item)).Any();
		}

		/// <summary>
		/// <c><see cref="Task.WhenAll{TResult}(IEnumerable{Task{TResult}})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<T[]> AllAsync<T>(this IEnumerable<Task<T>>? @this)
			=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array<T>.Empty);

		/// <summary>
		/// <c><see cref="Task.WhenAll(IEnumerable{Task})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask AllAsync<T>(this IEnumerable<Task> @this)
			=> await Task.WhenAll(@this);

		/// <summary>
		/// <c>@<paramref name="this"/>.And(<paramref name="sets"/>.Gather())</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? sets)
			=> @this.And(sets.Gather());

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<T?>? items)
		{
			if (@this is not null)
			{
				foreach (var item in @this)
					yield return item;
			}

			if (items is not null)
			{
				foreach (var item in items)
					if (item is not null)
						yield return item;
			}
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.And(<paramref name="items"/> as <see cref="IEnumerable{T}"/>)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params T?[]? items)
			=> @this.And(items as IEnumerable<T>);

		/// <summary>
		/// <c>@<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().Any()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>([NotNullWhen(true)] this IEnumerable? @this)
			=> @this.If<T>().Any();

		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
			=> @this switch
			{
				null => false,
				T[] array => array.Length > 0,
				ICollection<T> collection => collection.Count > 0,
				IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count > 0,
				ICollection collection => collection.Count > 0,
				_ => @this.GetEnumerator().MoveNext()
			};

		/// <summary>
		/// <c>@<paramref name="this"/>.If(<paramref name="filter"/>).Any()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Predicate<T> filter)
			=> @this.If(filter).Any();

		/// <summary>
		/// <c><see cref="Task.WhenAny{TResult}(IEnumerable{Task{TResult}})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<Task<T>> AnyAsync<T>(this IEnumerable<Task<T>> @this)
			=> await Task.WhenAny(@this);

		/// <summary>
		/// <c><see cref="Task.WhenAny(IEnumerable{Task})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask AnyAsync<T>(this IEnumerable<Task> @this)
			=> await Task.WhenAny(@this);

		public static IEnumerable<T?> As<T>(this IEnumerable? @this)
			=> @this switch
			{
				null => Enumerable<T>.Empty,
				IEnumerable<T> enumerable => enumerable,
				_ => Enumerable<T>.As(@this)
			};

		public static int Count<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => 0,
				T[] array => array.Length,
				ICollection<T> collection => collection.Count,
				IReadOnlyCollection<T> collection => collection.Count,
				ICollection collection => collection.Count,
				_ => @this.GetEnumerator().Count()
			};

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
		/// <c>@<paramref name="this"/>.To(item => <paramref name="action"/>(item, <paramref name="cancellationToken"/>)).AllAsync&lt;<typeparamref name="T"/>&gt;()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			await @this.To(item => action(item, cancellationToken)).AllAsync<T>();
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.To(<paramref name="action"/>).AllAsync&lt;<typeparamref name="T"/>&gt;()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, Task> action)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			await @this.To(action).AllAsync<T>();
		}

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
		/// <c>@<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().First()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? First<T>(this IEnumerable? @this)
			where T : class
			=> @this.If<T>().First();

		/// <summary>
		/// <c>@<paramref name="this"/>?.GetEnumerator().Next()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? First<T>(this IEnumerable<T>? @this)
			where T : class
			=> @this.Get(0);

		/// <summary>
		/// <c>@<paramref name="this"/>.If(<paramref name="filter"/>).First()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? First<T>(this IEnumerable<T>? @this, Predicate<T> filter)
			where T : class
			=> @this.If(filter).Get(0);

		/// <summary>
		/// <c>@<paramref name="this"/>.If&lt;<typeparamref name="T"/>&gt;().FirstValue()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? FirstValue<T>(this IEnumerable? @this)
			where T : struct
			=> @this.If<T>().FirstValue();

		/// <summary>
		/// <c>@<paramref name="this"/>?.GetEnumerator().NextValue()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? FirstValue<T>(this IEnumerable<T>? @this)
			where T : struct
			=> @this.GetValue(0);

		/// <summary>
		/// <c>@<paramref name="this"/>.If(<paramref name="filter"/>).FirstValue()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? FirstValue<T>(this IEnumerable<T>? @this, Predicate<T> filter)
			where T : struct
			=> @this.If(filter).GetValue(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		public static T? Get<T>(this IEnumerable<T>? @this, Index index)
			where T : class
			=> @this switch
			{
				null => null,
				T[] array when array.Has(index) => array[index],
				IList<T> list when list.Has(index) => list[index],
				IReadOnlyList<T> list when list.Has(index) => list[index],
				T[] or IList<T> or IReadOnlyList<T> _ => null,
				_ => @this.GetEnumerator().Get(index.IsFromEnd ? index.GetOffset(@this.Count()) : index.Value)
			};

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

		public static T? GetValue<T>(this IEnumerable<T>? @this, Index index)
			where T : struct
			=> @this switch
			{
				null => null,
				T[] array when array.Has(index) => array[index],
				IList<T> list when list.Has(index) => list[index],
				IReadOnlyList<T> list when list.Has(index) => list[index],
				T[] or IList<T> or IReadOnlyList<T> _ => null,
				_ => @this.GetEnumerator().GetValue(index.IsFromEnd ? index.GetOffset(@this.Count()) : index.Value)
			};

		/// <exception cref="ArgumentNullException"/>
		public static IDictionary<K, IEnumerable<V>> Group<K, V>(this IEnumerable<V>? @this, Func<V, K> keyFactory, IEqualityComparer<K>? comparer = null)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));

			(K Key, V Value)[] items = @this.To(item => (keyFactory(item), item)).ToArray();
			var keys = items.To(_ => _.Key).ToHashSet(comparer);
			return keys.ToDictionary(key => items.If(_ => keys.Comparer.Equals(_.Key, key)).To(_ => _.Value));
		}

		/// <summary>
		/// <c><paramref name="values"/>.All(value => @<paramref name="this"/>.Has(value, <paramref name="comparer"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T>? comparer = null)
			=> values.All(value => @this.Has(value, comparer));

		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
			=> @this switch
			{
				null => false,
				T[] array => index.Value < array.Length,
				IImmutableList<T> list => index.Value < list.Count,
				ICollection<T> collection => index.Value < collection.Count,
				IReadOnlyCollection<T> collection => index.Value < collection.Count,
				ICollection collection => index.Value < collection.Count,
				_ => @this.GetEnumerator().Skip(index.Value + 1),
			};

		/// <summary>
		/// <c>@<paramref name="this"/>.ToIndex(value, <paramref name="comparer"/>).Any()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value, IEqualityComparer<T>? comparer = null)
			=> @this.ToIndex(value, comparer).Any();

		public static IEnumerable<T> If<T>(this IEnumerable? @this)
			=> @this switch
			{
				null => Enumerable<T>.Empty,
				IEnumerable<T> items => items,
				_ => Enumerable<T>.If(@this)
			};

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<T> If<T>(this IEnumerable<T>? @this, Predicate<T> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return @this switch
			{
				null => Enumerable<T>.Empty,
				T[] array => array.If(filter),
				ImmutableArray<T> immutableArray => immutableArray.If(filter),
				List<T> list => list.If(filter),
				_ => Enumerable<T>.If(@this, filter)
			};
		}

		/// <exception cref="ArgumentNullException"/>
		public static async IAsyncEnumerable<T> IfAsync<T>(this IEnumerable<T>? @this, PredicateAsync<T> filter, [EnumeratorCancellation] CancellationToken _ = default)
		{
			filter.AssertNotNull(nameof(filter));

			if (@this is null)
				yield break;

			foreach (var item in @this)
				if (await filter(item))
					yield return item;
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.If(_ =&gt; _ is not null)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
			where T : class
			=> @this.If(_ => _ is not null)!;

		/// <summary>
		/// <c>@<paramref name="this"/>.If(_ =&gt; _.HasValue).To(_ => _.Value)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
			where T : struct
			=> @this.If(_ => _.HasValue).To(_ => _!.Value);

		public static bool IsSequence<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		{
			if (@this is null && items is null)
				return true;

			if (@this is null || items is null)
				return false;

			comparer ??= EqualityComparer<T>.Default;

			using var enumerator = @this.GetEnumerator();
			using var itemEnumerator = items.GetEnumerator();

			while (enumerator.MoveNext() && itemEnumerator.MoveNext())
			{
				if (!comparer.Equals(enumerator.Current, itemEnumerator.Current))
					return false;
			}

			return !enumerator.MoveNext() && !itemEnumerator.MoveNext();
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.ToHashSet(<paramref name="comparer"/>).SetEquals(<paramref name="items"/> ?? <see cref="Enumerable{T}.Empty"/>)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSet<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
			=> @this.ToHashSet(comparer).SetEquals(items ?? Enumerable<T>.Empty);

		/// <summary>
		/// <c>@<paramref name="this"/> is not null ? <see cref="string"/>.Join(<paramref name="delimeter"/>, @<paramref name="this"/>) : <see cref="string.Empty"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, char delimeter)
			=> @this is not null ? string.Join(delimeter, @this) : string.Empty;

		/// <summary>
		/// <c>@<paramref name="this"/> is not null ? <see cref="string"/>.Join(<paramref name="delimeter"/>)) : <see cref="string.Empty"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, string delimeter)
			=> @this is not null ? string.Join(delimeter, @this) : string.Empty;

		/// <summary>
		/// <c>@<paramref name="this"/>?.GetEnumerator().Next()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Last<T>(this IEnumerable<T>? @this)
			where T : class
			=> @this.Get(^0);

		/// <summary>
		/// <c>@<paramref name="this"/>.If(<paramref name="filter"/>).First()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Last<T>(this IEnumerable<T>? @this, Predicate<T> filter)
			where T : class
			=> @this.If(filter).Get(^0);

		/// <summary>
		/// <c>@<paramref name="this"/>?.GetEnumerator().NextValue()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? LastValue<T>(this IEnumerable<T>? @this)
			where T : struct
			=> @this.GetValue(^0);

		/// <summary>
		/// <c>@<paramref name="this"/>.If(<paramref name="filter"/>).FirstValue()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? LastValue<T>(this IEnumerable<T>? @this, Predicate<T> filter)
			where T : struct
			=> @this.If(filter).GetValue(^0);

		/// <summary>
		/// <c><see cref="HashSet{T}.IntersectWith(IEnumerable{T})"/></c>
		/// </summary>
		public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		{
			var hashSet = @this.ToHashSet(comparer);
			hashSet.IntersectWith(items ?? Enumerable<T>.Empty);
			return hashSet;
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.Aggregate(default, <paramref name="comparer"/>.Maximum)</c>
		/// </summary>
		public static T Maximum<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
			where T : unmanaged
		{
			comparer ??= Comparer<T>.Default;

			var initial = @this.FirstValue();
			return initial.HasValue ? @this.Aggregate(initial.Value, comparer.Maximum) : default;
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.Aggregate(default, <paramref name="comparer"/>.Minimum)</c>
		/// </summary>
		public static T Minimum<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
			where T : unmanaged
		{
			comparer ??= Comparer<T>.Default;

			var initial = @this.FirstValue();
			return initial.HasValue ? @this.Aggregate(initial.Value, comparer.Minimum) : default;
		}

		/// <summary>
		/// <c><see cref="HashSet{T}.SymmetricExceptWith(IEnumerable{T})"/></c>
		/// </summary>
		public static HashSet<T> NotMatch<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		{
			var hashSet = @this.ToHashSet(comparer);
			hashSet.SymmetricExceptWith(items ?? Enumerable<T>.Empty);
			return hashSet;
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.Get(new Range(count, ^1))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Skip<T>(this IEnumerable<T> @this, int count)
			=> @this.Get(new Range(count, ^0));

		/// <summary>
		/// <c>@<paramref name="this"/>.Sort(<paramref name="comparer"/> ?? <see cref="Comparer{T}.Default"/>)</c>
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
		/// <c>@<paramref name="this"/>.Get(new Range(0, <paramref name="count"/>))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Take<T>(this IEnumerable<T> @this, int count)
			=> @this.Get(new Range(0, count));

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

		public static T[] ToArray<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => Array<T>.Empty,
				T[] array => array.AsSpan().ToArray(),
				ImmutableArray<T> array => array.AsSpan().ToArray(),
				List<T> list => list.ToArray(),
				Stack<T> stack => stack.ToArray(),
				Queue<T> queue => queue.ToArray(),
				_ => Enumerable<T>.ToArray(@this)
			};

		public static async IAsyncEnumerable<T> ToAsync<T>(this IEnumerable<T>? @this, [EnumeratorCancellation] CancellationToken token = default)
		{
			if (@this is not null)
				foreach (var item in @this)
					yield return item;

			await Task.CompletedTask;
		}

		/// <exception cref="ArgumentNullException"/>
		public static async IAsyncEnumerable<V> ToAsync<T, V>(this IEnumerable<T>? @this, Func<T?, Task<V>> map, [EnumeratorCancellation] CancellationToken token = default)
		{
			map.AssertNotNull(nameof(map));

			if (@this is null)
				yield break;

			foreach (var item in @this)
				yield return await map(item);
		}

		public static string ToCSV<T>(this IEnumerable<T>? @this)
			=> @this.Any() ? string.Join(',', @this.To(value => value switch
			{
				null => string.Empty,
				bool or sbyte or short or int or nint or long or byte or ushort or uint or nuint or ulong or float or double or Half or decimal _ => value.ToString(),
				',' => "\",\"",
				'\"' => "\"\"\"\"",
				char character => character.ToString(),
				DateTime dateTime => dateTime.ToString("o"),
				DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("o"),
				TimeSpan time => time.ToString("c"),
				Guid guid => guid.ToString("D"),
				Enum token => token.ToString("D"),
				string text when text.Contains(',') => $"\"{text.Replace("\"", "\"\"")}\"",
				string text => text.Replace("\"", "\"\""),
				_ => $"\"{value.ToString()?.Replace("\"", "\"\"")}\""
			})) : string.Empty;

		/// <summary>
		/// <c>@<paramref name="this"/>.To(<paramref name="map"/>).ToCsv()</c>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public static string ToCSV<T>(this IEnumerable<T>? @this, Func<T, string> map)
		{
			map.AssertNotNull(nameof(map));

			return @this.To(map).ToCSV();
		}

		/// <exception cref="ArgumentNullException"/>
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory, IEqualityComparer<K>? comparer = null)
			where K : notnull
		{
			valueFactory.AssertNotNull(nameof(valueFactory));

			var dictionary = comparer is not null ? new Dictionary<K, V>(@this.Count(), comparer) : new Dictionary<K, V>(@this.Count());
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K>? comparer = null)
			where K : notnull
			=> @this switch
			{
				null when comparer is not null => new Dictionary<K, V>(comparer),
				null => new Dictionary<K, V>(),
				_ when comparer is not null => new Dictionary<K, V>(@this, comparer),
				_ => new Dictionary<K, V>(@this)
			};

		/// <exception cref="ArgumentNullException"/>
		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K>? comparer = null)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			valueFactory.AssertNotNull(nameof(valueFactory));

			var dictionary = comparer is not null ? new Dictionary<K, V>(@this.Count(), comparer) : new Dictionary<K, V>(@this.Count());
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<Tuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
			where K : notnull
			=> @this.To(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<ValueTuple<K, V>>? @this, IEqualityComparer<K>? comparer = null)
			where K : notnull
			=> @this.To(tuple => KeyValuePair.Create(tuple.Item1, tuple.Item2)).ToDictionary(comparer);

		public static int ToHashCode<T>(this IEnumerable<T>? @this)
		{
			var hashCode = new HashCode();
			@this?.Do(hashCode.Add);
			return hashCode.ToHashCode();
		}

		/// <summary>
		/// <c>@<paramref name="this"/>.Any() ? new <see cref="HashSet{T}"/>(@<paramref name="this"/>, <paramref name="comparer"/>) : new <see cref="HashSet{T}"/>(0, <paramref name="comparer"/>)</c>
		/// </summary>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T>? comparer = null)
			=> @this.Any() ? new HashSet<T>(@this, comparer) : new HashSet<T>(0, comparer);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="ImmutableQueue.CreateRange{T}(IEnumerable{T})"/></item>
		/// <item><see cref="ImmutableQueue.Create{T}(T[])"/></item>
		/// </list>
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
		/// <list type="table">
		/// <item><see cref="ImmutableStack.CreateRange{T}(IEnumerable{T})"/></item>
		/// <item><see cref="ImmutableStack.Create{T}(T[])"/></item>
		/// <item><see cref="ImmutableStack{T}.Empty"/></item>
		/// </list>
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

		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, Predicate<T> filter)
			=> @this switch
			{
				null => Enumerable<int>.Empty,
				T[] array => array.ToIndex(filter),
				ImmutableArray<T> immutableArray => immutableArray.ToIndex(filter),
				List<T> list => list.ToIndex(filter),
				_ => Enumerable<T>.ToIndex(@this, filter)
			};

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value, IEqualityComparer<T>? comparer = null)
			=> value switch
			{
				_ when !@this.Any() => Enumerable<int>.Empty,
				_ when comparer is not null => @this.ToIndex(item => comparer.Equals(item, value)),
				IEquatable<T> equatable => @this.ToIndex(equatable.Equals),
				_ => @this.ToIndex(item => Equals(item, value))
			};

		/// <summary>
		/// <c>@<paramref name="this"/> is not null ? new <see cref="List{T}"/>(@<paramref name="this"/>) : new <see cref="List{T}"/>(0)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> ToList<T>(this IEnumerable<T>? @this)
			=> @this is not null ? new List<T>(@this) : new List<T>(0);

		/// <summary>
		/// <c>@<paramref name="this"/>.To(<paramref name="map"/>).Gather()</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<V> ToMany<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map)
			=> @this.To(map).Gather();

		/// <summary>
		/// <c>@<paramref name="this"/> is not null ? new <see cref="Queue{T}"/>(@<paramref name="this"/>) : new <see cref="Queue{T}"/>(0)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this)
			=> @this.Any() ? new Queue<T>(@this) : new Queue<T>(0);

		public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => ReadOnlySpan<T>.Empty,
				ImmutableArray<T> immutableArray => immutableArray.AsSpan(),
				T[] array => array.AsSpan(),
				List<T> list => list.ToArray().AsSpan(),
				_ => @this.ToArray().AsSpan(),
			};

		public static Span<T> ToSpan<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => Span<T>.Empty,
				T[] array => array.AsSpan(),
				List<T> list => list.ToArray().AsSpan(),
				_ => @this.ToArray().AsSpan(),
			};

		/// <summary>
		/// <c>@<paramref name="this"/> is not null ? new <see cref="Stack{T}"/>(@<paramref name="this"/>) : new <see cref="Stack{T}"/>(0)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Stack<T> ToStack<T>(this IEnumerable<T>? @this)
			=> @this is not null ? new Stack<T>(@this) : new Stack<T>(0);

		/// <summary>
		/// <c><see cref="HashSet{T}.UnionWith(IEnumerable{T})"/></c>
		/// </summary>
		public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		{
			var hashSet = @this.ToHashSet(comparer);
			hashSet.UnionWith(items ?? Enumerable<T>.Empty);
			return hashSet;
		}

		/// <summary>
		/// <c><see cref="HashSet{T}.ExceptWith(IEnumerable{T})"/></c>
		/// </summary>
		public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null)
		{
			var hashSet = @this.ToHashSet(comparer);
			hashSet.ExceptWith(items ?? Enumerable<T>.Empty);
			return hashSet;
		}
	}
}
