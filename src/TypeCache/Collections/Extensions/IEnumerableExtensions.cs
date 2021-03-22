﻿// Copyright (c) 2021 Samuel Abraham

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
		public static IEnumerable<T> As<T>(this IEnumerable? @this)
			where T : notnull
		{
			int count;
			switch (@this)
			{
				case null:
					break;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
						yield return array[i];
					break;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
						yield return immutableArray[i];
					break;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
						yield return list[i];
					break;
				default:
					foreach (var item in @this)
						if (item is T value)
							yield return value;
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>([NotNullWhen(true)] this IEnumerable? @this)
			=> @this.If<T>().Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? First<T>(this IEnumerable? @this)
			where T : class
			=> @this.If<T>().First();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? FirstValue<T>(this IEnumerable? @this)
			where T : struct
			=> @this.If<T>().FirstValue();

		public static IEnumerable<T> If<T>(this IEnumerable? @this)
		{
			return @this switch
			{
				null => CustomEnumerable<T>.Empty,
				IEnumerable<T> items => items,
				_ => getItems(@this)
			};

			static IEnumerable<T> getItems(IEnumerable enumerable)
			{
				foreach (var item in enumerable)
					if (item is T value)
						yield return value;
			}
		}

		public static T Aggregate<T>(this IEnumerable<T>? @this, T initialValue, Func<T, T, T> aggregator)
			where T : unmanaged
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = initialValue;
			@this.Do(item => result = aggregator(result, item));
			return result;
		}

		public static async ValueTask<T> AggregateAsync<T>(this IEnumerable<T>? @this, T initialValue, Func<T, T, Task<T>> aggregator)
			where T : unmanaged
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = initialValue;
			await @this.DoAsync(async item => result = await aggregator(result, item));
			return result;
		}

		public static bool All<T>(this IEnumerable<T>? @this, Func<T?, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return !@this.If(item => !filter(item)).Any();
		}

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? sets)
		{
			if (@this is not null)
				foreach (var item in @this)
					yield return item;

			if (sets is not null)
				foreach (var item in sets.Gather())
					yield return item;
		}

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params IEnumerable<T>?[] sets)
		{
			if (@this is not null)
				foreach (var item in @this)
					yield return item;

			if (sets is not null)
				foreach (var item in sets.Gather())
					yield return item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params T[] items)
			=> @this.And(items as IEnumerable<T>);

		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
			=> @this switch
			{
				null => false,
				T[] array => array.Length > 0,
				IImmutableList<T> list => list.Count > 0,
				ICollection<T> collection => collection.Count > 0,
				IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count > 0,
				ICollection collection => collection.Count > 0,
				_ => @this.GetEnumerator().MoveNext()
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Func<T?, bool> filter)
			=> @this.If(filter).Any();

		public static int Count<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => 0,
				T[] array => array.Length,
				IImmutableList<T> list => list.Count,
				ICollection<T> collection => collection.Count,
				IReadOnlyCollection<T> collection => collection.Count,
				ICollection collection => collection.Count,
				_ => @this.GetEnumerator().Count()
			};

		public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out IEnumerable<T> rest)
		{
			switch (@this)
			{
				case null:
					first = default;
					rest = CustomEnumerable<T>.Empty;
					return;
				case T[] array:
					first = array[0];
					rest = array.Get(new Range(1, ^1));
					return;
				case ImmutableArray<T> immutableArray:
					first = immutableArray[0];
					rest = immutableArray.Get(new Range(2, ^1));
					return;
				case List<T> list:
					first = list[0];
					rest = list.Get(new Range(2, ^1));
					return;
				default:
					var enumerator = @this.GetEnumerator();
					first = enumerator.MoveNext() ? enumerator.Current : default;
					rest = enumerator.Rest();
					return;
			}
		}

		public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out IEnumerable<T> rest)
		{
			switch (@this)
			{
				case null:
					first = default;
					second = default;
					rest = CustomEnumerable<T>.Empty;
					return;
				case T[] array:
					first = array[0];
					second = array[1];
					rest = array.Get(new Range(2, ^1));
					return;
				case ImmutableArray<T> immutableArray:
					first = immutableArray[0];
					second = immutableArray[1];
					rest = immutableArray.Get(new Range(2, ^1));
					return;
				case List<T> list:
					first = list[0];
					second = list[1];
					rest = list.Get(new Range(2, ^1));
					return;
				default:
					var enumerator = @this.GetEnumerator();
					first = enumerator.MoveNext() ? enumerator.Current : default;
					second = enumerator.MoveNext() ? enumerator.Current : default;
					rest = enumerator.Rest();
					return;
			}
		}

		public static void Deconstruct<T>(this IEnumerable<T> @this, out T? first, out T? second, out T? third, out IEnumerable<T> rest)
		{
			switch (@this)
			{
				case null:
					first = default;
					second = default;
					third = default;
					rest = CustomEnumerable<T>.Empty;
					return;
				case T[] array:
					first = array[0];
					second = array[1];
					third = array[2];
					rest = array.Get(new Range(3, ^1));
					return;
				case ImmutableArray<T> immutableArray:
					first = immutableArray[0];
					second = immutableArray[1];
					third = immutableArray[2];
					rest = immutableArray.Get(new Range(3, ^1));
					return;
				case List<T> list:
					first = list[0];
					second = list[1];
					third = list[2];
					rest = list.Get(new Range(3, ^1));
					return;
				default:
					var enumerator = @this.GetEnumerator();
					first = enumerator.MoveNext() ? enumerator.Current : default;
					second = enumerator.MoveNext() ? enumerator.Current : default;
					third = enumerator.MoveNext() ? enumerator.Current : default;
					rest = enumerator.Rest();
					return;
			}
		}

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
					action.AssertNotNull(nameof(action));

					foreach (var item in @this)
						action(item);
					return;
			}
		}

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
					action.AssertNotNull(nameof(action));

					var i = -1;
					foreach (var item in @this)
						action(item, ++i);
					return;
			}
		}

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
					action.AssertNotNull(nameof(action));
					between.AssertNotNull(nameof(between));

					var first = true;
					foreach (var item in @this)
					{
						if (first)
							first = false;
						else
							between();
						action(item);
					}
					return;
			}
		}

		public static void Do<T>(this IEnumerable<T>? @this, Action<T, int> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			if (!@this.Any())
				return;

			var i = 0;
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
					foreach (var item in @this)
					{
						if (i > 0)
							between();
						action(item, ++i);
					}
					return;
			}
		}

		public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, Task> action)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			await @this.To(item => action(item)).AllAsync<T>();
		}

		public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			await @this.To(item => action(item, cancellationToken)).AllAsync<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? First<T>(this IEnumerable<T?>? @this)
			where T : class
			=> @this?.GetEnumerator().Next();

		public static T? First<T>(this IEnumerable<T?>? @this, Func<T?, bool> filter)
			where T : class
		{
			filter.AssertNotNull(nameof(filter));

			return @this.If(filter).First();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? FirstValue<T>(this IEnumerable<T>? @this)
			where T : struct
			=> @this?.GetEnumerator().NextValue();

		public static T? FirstValue<T>(this IEnumerable<T>? @this, Func<T, bool> filter)
			where T : struct
		{
			filter.AssertNotNull(nameof(filter));

			return @this.If(filter).FirstValue();
		}

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
		{
			if (!@this.Any() || index.Value < 0)
				return null;

			if (index.IsFromEnd)
				index = index.Normalize(@this.Count());

			return @this switch
			{
				T[] array when index.Value < array.Length => array[index.Value],
				IList<T> list when index.Value < list.Count => list[index.Value],
				IReadOnlyList<T> list when index.Value < list.Count => list[index.Value],
				T[] or IList<T> or IReadOnlyList<T> _ => null,
				_ => @this.GetEnumerator().Get(index)
			};
		}

		public static T? GetValue<T>(this IEnumerable<T>? @this, Index index)
			where T : struct
		{
			if (!@this.Any() || index.Value < 0)
				return null;

			if (index.IsFromEnd)
				index = index.Normalize(@this.Count());

			return @this switch
			{
				T[] array when index.Value < array.Length => array[index.Value],
				IList<T> list when index.Value < list.Count => list[index.Value],
				IReadOnlyList<T> list when index.Value < list.Count => list[index.Value],
				T[] or IList<T> or IReadOnlyList<T> _ => null,
				_ => @this.GetEnumerator().GetValue(index)
			};
		}

		public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Range range)
		{
			if (!@this.Any())
				yield break;

			if (range.Start.IsFromEnd || range.End.IsFromEnd)
				range = range.Normalize(@this.Count());

			if (range.Start.Value < 0 || range.End.Value < 0)
				range = new Range(range.Start.Value >= 0 ? range.Start : Index.Start, range.End.Value >= 0 ? range.End : Index.Start);

			if (@this is T[] array)
			{
				foreach (var i in range.Values())
					yield return array[i];
			}
			else if (@this is ImmutableArray<T> immutableArray)
			{
				foreach (var i in range.Values())
					yield return immutableArray[i];
			}
			else if (@this is List<T> list)
			{
				foreach (var i in range.Values())
					yield return list[i];
			}
			else if (!range.IsReverse())
			{
				using var enumerator = @this.GetEnumerator();
				if (range.Start.Value == 0 || enumerator.Skip(range.Start.Value))
				{
					var count = range.End.Value - range.Start.Value + 1;
					while (enumerator.MoveNext() && count > 0)
					{
						yield return enumerator.Current;
						--count;
					}
				}
			}
			else
			{
				using var enumerator = @this.GetEnumerator();
				if (range.End.Value == 0 || enumerator.Skip(range.End.Value))
				{
					var count = range.Start.Value - range.End.Value + 1;
					var items = new Stack<T>(count);

					while (enumerator.MoveNext() && count > 0)
					{
						items.Push(enumerator.Current);
						--count;
					}

					while (items.TryPop(out var item))
						yield return item;
				}
			}
		}

		public static IDictionary<K, IEnumerable<V>> Group<K, V>(this IEnumerable<V>? @this, Func<V, K> keyFactory, IEqualityComparer<K> comparer)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			comparer.AssertNotNull(nameof(comparer));

			(K Key, V Value)[] items = @this.To(item => (keyFactory(item), item)).ToArray(@this.Count());
			return items
				.To(pair => pair.Key)
				.ToHashSet(comparer)
				.ToDictionary(key => items.If(pair => comparer.Equals(pair.Key, key)).To(pair => pair.Value));
		}

		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
		{
			if (!@this.Any() || index.Value < 0)
				return false;

			if (index.IsFromEnd)
				index = index.Normalize(@this.Count());

			return @this switch
			{
				T[] array => index.Value < array.Length,
				IImmutableList<T> list => index.Value < list.Count,
				ICollection<T> collection => index.Value < collection.Count,
				IReadOnlyCollection<T> collection => index.Value < collection.Count,
				ICollection collection => index.Value < collection.Count,
				_ => @this.GetEnumerator().Skip(index.Value),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value)
			=> @this.ToIndex(value).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
			=> @this.ToIndex(value, comparer).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values)
			where T : struct
			=> values.All(@this.Has);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T> comparer)
			=> values.All(value => @this.Has(value, comparer!));

		public static IEnumerable<T?> If<T>(this IEnumerable<T?>? @this, Func<T?, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			int count;
			switch (@this)
			{
				case null:
					yield break;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
					{
						var item = array[i];
						if (filter(item))
							yield return item;
					}
					yield break;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
					{
						var item = immutableArray[i];
						if (filter(item))
							yield return item;
					}
					yield break;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
					{
						var item = list[i];
						if (filter(item))
							yield return item;
					}
					yield break;
				default:
					foreach (var item in @this)
						if (filter(item))
							yield return item;
					yield break;
			}
		}

		public static async IAsyncEnumerable<T?> IfAsync<T>(this IEnumerable<T?>? @this, Func<T?, Task<bool>> filter, [EnumeratorCancellation] CancellationToken _ = default)
		{
			filter.AssertNotNull(nameof(filter));

			int count;
			switch (@this)
			{
				case null:
					yield break;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
					{
						var item = array[i];
						if (await filter(item))
							yield return item;
					}
					yield break;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
					{
						var item = immutableArray[i];
						if (await filter(item))
							yield return item;
					}
					yield break;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
					{
						var item = list[i];
						if (await filter(item))
							yield return item;
					}
					yield break;
				default:
					foreach (var item in @this)
						if (await filter(item))
							yield return item;
					yield break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
			where T : class
			=> @this.If(_ => _ is not null)!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
			where T : struct
			=> @this.If(_ => _.HasValue).To(_ => _!.Value);

		public static bool IsSequence<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			if (@this is null && items is null)
				return true;
			else if (@this is null || items is null)
				return false;

			using var enumerator = @this.GetEnumerator();
			using var itemsEnumerator = items.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(itemsEnumerator.MoveNext()
					&& (enumerator.Current is null && itemsEnumerator.Current is null)
					&& enumerator.Current?.Equals(itemsEnumerator.Current) is true))
					return false;
			}

			return !itemsEnumerator.MoveNext();
		}

		public static bool IsSequence<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			if (@this is null && items is null)
				return true;
			else if (@this is null || items is null)
				return false;

			using var enumerator = @this.GetEnumerator();
			using var itemsEnumerator = items.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(itemsEnumerator.MoveNext() && comparer.Equals(enumerator.Current, itemsEnumerator.Current)))
					return false;
			}

			return !itemsEnumerator.MoveNext();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSet<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
			=> @this.ToHashSet().SetEquals(items ?? CustomEnumerable<T>.Empty);

		public static bool IsSet<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.ToHashSet(comparer).SetEquals(items ?? CustomEnumerable<T>.Empty);
		}

		public static string Join<T>(this IEnumerable<T>? @this, char delimeter)
			=> @this switch
			{
				null => string.Empty,
				T[] array => string.Join(delimeter, array),
				_ => string.Join(delimeter, @this)
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, char delimeter, Func<T, string> map)
			=> @this is not null ? string.Join(delimeter, @this.To(map)) : string.Empty;

		public static string Join<T>(this IEnumerable<T>? @this, string delimeter)
			=> @this switch
			{
				null => string.Empty,
				T[] array => string.Join(delimeter, array),
				_ => string.Join(delimeter, @this)
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, string delimeter, Func<T, string> map)
			=> @this is not null ? string.Join(delimeter, @this.To(map)) : string.Empty;

		public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items is not null)
				hashSet.IntersectWith(items);
			return hashSet;
		}

		public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items is not null)
				hashSet.IntersectWith(items);
			return hashSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Maximum<T>(this IEnumerable<T>? @this)
			where T : unmanaged, IComparable<T>
			=> @this.Aggregate(default(T), (x, y) => x.MoreThan(y) ? x : y);

		public static T Maximum<T>(this IEnumerable<T>? @this, IComparer<T> comparer)
			where T : unmanaged
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.Aggregate(default(T), comparer.Maximum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Minimum<T>(this IEnumerable<T>? @this)
			where T : unmanaged, IComparable<T>
			=> @this.Aggregate(default(T), (x, y) => x.LessThan(y) ? x : y);

		public static T Minimum<T>(this IEnumerable<T>? @this, IComparer<T> comparer)
			where T : unmanaged
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.Aggregate(default(T), comparer.Minimum);
		}

		public static HashSet<T> Neither<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items is not null)
				hashSet.SymmetricExceptWith(items);
			return hashSet;
		}

		public static HashSet<T> Neither<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items is not null)
				hashSet.SymmetricExceptWith(items);
			return hashSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Skip<T>(this IEnumerable<T> @this, int count)
			=> @this.Get(new Range(count, ^1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] Sort<T>(this IEnumerable<T>? @this)
			where T : IComparable<T>
			=> @this.Sort(Comparer<T>.Create((x, y) => x.CompareTo(y)));

		public static T[] Sort<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
		{
			var items = @this switch
			{
				null => Array.Empty<T>(),
				T[] array => array,
				_ => @this.ToArray()
			};
			items.Sort(comparer);
			return items;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Take<T>(this IEnumerable<T> @this, int count)
			=> @this.Get(new Range(0, count));

		public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, V> map)
		{
			map.AssertNotNull(nameof(map));

			int count;
			switch (@this)
			{
				case null:
					yield break;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
						yield return map(array[i]);
					yield break;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
						yield return map(immutableArray[i]);
					yield break;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
						yield return map(list[i]);
					yield break;
				default:
					foreach (var item in @this)
						yield return map(item);
					yield break;
			}
		}

		public static T[] ToArray<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				ImmutableArray<T> array => array.AsSpan().ToArray(),
				List<T> list => list.ToArray(),
				Stack<T> stack => stack.ToArray(),
				Queue<T> queue => queue.ToArray(),
				_ => @this.ToArray(@this.Count())
			};

		public static T[] ToArray<T>(this IEnumerable<T>? @this, int length)
		{
			var array = new T[length];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		public static async IAsyncEnumerable<V?> ToAsync<T, V>(this IEnumerable<T?>? @this, Func<T?, Task<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
		{
			map.AssertNotNull(nameof(map));

			int count;
			switch (@this)
			{
				case null:
					yield break;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
						yield return await map(array[i]);
					yield break;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
						yield return await map(immutableArray[i]);
					yield break;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
						yield return await map(list[i]);
					yield break;
				default:
					foreach (var item in @this)
						yield return await map(item);
					yield break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToCsv<T>(this IEnumerable<T>? @this, Func<T, string> map)
			=> @this.To(map).ToCsv();

		public static string ToCsv<T>(this IEnumerable<T>? @this)
			=> @this.Any() ? string.Join(',', @this.To(value => value switch
			{
				bool or sbyte or short or int or nint or long or byte or ushort or uint or nuint or ulong or float or double or Half or decimal _ => value.ToString(),
				char text => text.Equals(',') ? "\",\"" : text.Equals('\"') ? "\"\"\"\"" : text.ToString(),
				DateTime dateTime => dateTime.ToString("o"),
				DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("o"),
				TimeSpan time => time.ToString("c"),
				Guid guid => guid.ToString("D"),
				Enum token => token.Number(),
				string text => text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\""),
				null => string.Empty,
				_ => $"\"{value.ToString()?.Replace("\"", "\"\"")}\""
			})) : string.Empty;

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory)
			where K : notnull
		{
			valueFactory.AssertNotNull(nameof(valueFactory));

			var dictionary = new Dictionary<K, V>(@this.Count());
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory, IEqualityComparer<K> comparer)
			where K : notnull
		{
			valueFactory.AssertNotNull(nameof(valueFactory));
			comparer.AssertNotNull(nameof(comparer));

			var dictionary = new Dictionary<K, V>(@this.Count(), comparer);
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			valueFactory.AssertNotNull(nameof(valueFactory));

			var dictionary = new Dictionary<K, V>(@this.Count());
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K> comparer)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			valueFactory.AssertNotNull(nameof(valueFactory));
			comparer.AssertNotNull(nameof(comparer));

			var dictionary = new Dictionary<K, V>(@this.Count(), comparer);
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		public static int ToHashCode<T>(this IEnumerable<T>? @this)
		{
			var hashCode = new HashCode();
			@this.Do(hashCode.Add);
			return hashCode.ToHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this)
			=> @this is not null ? new HashSet<T>(@this) : new HashSet<T>(0);

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this is not null ? new HashSet<T>(@this, comparer) : new HashSet<T>(comparer);
		}

		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T>? @this)
			where T : notnull
			=> @this is T[] array ? Unsafe.As<T[], ImmutableArray<T>>(ref array) : @this.ToImmutable(@this.Count());

		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T>? @this, int count)
			where T : notnull
		{
			if (@this is null)
				return ImmutableArray<T>.Empty;

			var arrayBuilder = ImmutableArray.CreateBuilder<T>(count);
			arrayBuilder.AddRange(@this);
			return arrayBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this)
			where K : notnull
		{
			if (@this is null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>();
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K> keyComparer)
			where K : notnull
		{
			keyComparer.AssertNotNull(nameof(keyComparer));

			if (@this is null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>(keyComparer);
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
			where K : notnull
		{
			keyComparer.AssertNotNull(nameof(keyComparer));
			valueComparer.AssertNotNull(nameof(valueComparer));

			if (@this is null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder(keyComparer, valueComparer);
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, Func<T, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			int count;
			switch (@this)
			{
				case null:
					yield break;
				case T[] array:
					count = array.Length;
					for (var i = 0; i < count; ++i)
						if (filter(array[i]))
							yield return i;
					yield break;
				case ImmutableArray<T> immutableArray:
					count = immutableArray.Length;
					for (var i = 0; i < count; ++i)
						if (filter(immutableArray[i]))
							yield return i;
					yield break;
				case List<T> list:
					count = list.Count;
					for (var i = 0; i < count; ++i)
						if (filter(list[i]))
							yield return i;
					yield break;
				default:
					var index = 0;
					foreach (var item in @this)
					{
						if (filter(item))
							yield return index;
						++index;
					}
					yield break;
			}
		}

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value)
			=> value switch
			{
				IEquatable<T> equatable => @this.ToIndex(equatable.Equals),
				_ => @this.ToIndex(item => Equals(item, value))
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
			=> @this.ToIndex(item => comparer.Equals(item, value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> ToList<T>(this IEnumerable<T>? @this)
			=> @this is not null ? new List<T>(@this) : new List<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<V> ToMany<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map)
			=> @this.To(map).Gather();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this)
			=> @this is not null ? new Queue<T>(@this) : new Queue<T>(0);

		public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => Span<T>.Empty,
				ImmutableArray<T> array => array.AsSpan(),
				T[] array => array.AsSpan(),
				_ => @this.ToArray().AsSpan(),
			};

		public static Span<T> ToSpan<T>(this IEnumerable<T>? @this)
			=> @this switch
			{
				null => Span<T>.Empty,
				T[] array => array.AsSpan(),
				_ => @this.ToArray().AsSpan(),
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Stack<T> ToStack<T>(this IEnumerable<T>? @this)
			=> @this is not null ? new Stack<T>(@this) : new Stack<T>(0);

		public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items is not null)
				hashSet.UnionWith(items);
			return hashSet;
		}

		public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items is not null)
				hashSet.UnionWith(items);
			return hashSet;
		}

		public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items is not null)
				hashSet.ExceptWith(items);
			return hashSet;
		}

		public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items is not null)
				hashSet.ExceptWith(items);
			return hashSet;
		}
	}
}
