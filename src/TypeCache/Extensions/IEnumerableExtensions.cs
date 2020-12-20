﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Common;

namespace TypeCache.Extensions
{
	public static class IEnumerableExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IEnumerable<T> Empty<T>()
		{
			yield break;
		}

		public static IEnumerable<T?> As<T>(this IEnumerable? @this) where T : class
		{
			if (@this == null)
				yield break;

			var enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
				yield return enumerator.Current as T;
		}

		public static T? Aggregate<T>(this IEnumerable<T>? @this, Func<T, T, T> aggregator)
			where T : struct
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = default(T?);
			@this.Do(item => result = aggregator(result.GetValueOrDefault(), item));
			return result;
		}

		public static async ValueTask<T?> AggregateAsync<T>(this IEnumerable<T>? @this, Func<T, T, Task<T>> aggregator)
			where T : struct
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = default(T?);
			await @this.DoAsync(async item => result = await aggregator(result.GetValueOrDefault(), item));
			return result;
		}

		public static bool All<T>(this IEnumerable<T>? @this, Func<T, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return !@this.If(item => !filter(item)).Any();
		}

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? items)
		{
			var enumerable = @this;
			using var itemEnumerator = items?.GetEnumerator();

		Action:
			switch (enumerable)
			{
				case null:
					break;
				case ImmutableList<T> immutableList when immutableList.Count > 0:
					goto default;
				case IList<T> list when list.Count > 0:
					for (var i = 0; i < list.Count; ++i)
						yield return list[i];
					break;
				case IReadOnlyList<T> list when list.Count > 0:
					for (var i = 0; i < list.Count; ++i)
						yield return list[i];
					break;
				case ImmutableList<T> _:
				case IList<T> _:
				case IReadOnlyList<T> _:
					break;
				default:
					using (var enumerator = enumerable.GetEnumerator())
					{
						while (enumerator.MoveNext())
							yield return enumerator.Current;
					}
					break;
			}

			if (itemEnumerator?.MoveNext() == true)
			{
				enumerable = itemEnumerator.Current;
				goto Action;
			}
		}

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params IEnumerable<T>?[] items)
		{
			var enumerable = @this;
			var index = 0;
			var length = items?.Length ?? 0;

		Action:
			switch (enumerable)
			{
				case null:
					break;
				case ImmutableList<T> _:
					goto default;
				case IList<T> list when list.Count > 0:
					for (var i = 0; i < list.Count; ++i)
						yield return list[i];
					break;
				case IReadOnlyList<T> list when list.Count > 0:
					for (var i = 0; i < list.Count; ++i)
						yield return list[i];
					break;
				case IList<T> _:
				case IReadOnlyList<T> _:
					break;
				default:
					using (var enumerator = enumerable.GetEnumerator())
					{
						while (enumerator.MoveNext())
							yield return enumerator.Current;
					}
					break;
			}

			if (index < length)
			{
				enumerable = items?[index++];
				goto Action;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params T[] items)
			=> @this.And(items as IEnumerable<T>);

		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
			=> @this switch
			{
				null => false,
				ICollection<T> collection => collection.Count > 0,
				IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count > 0,
				ICollection collection => collection.Count > 0,
				_ => @this.GetEnumerator().MoveNext()
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Func<T?, bool> filter)
			=> @this.If(filter).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T, V>([NotNullWhen(true)] this IEnumerable<T>? @this)
			=> @this.If<T, V>().Any();

		public static int Count<T>(this IEnumerable<T>? @this)
		{
			return @this switch
			{
				null => 0,
				ICollection<T> collection => collection.Count,
				IReadOnlyCollection<T> collection => collection.Count,
				_ => count(@this)
			};

			static int count(IEnumerable<T> enumerable)
			{
				using var enumerator = enumerable.GetEnumerator();
				return enumerator.Count();
			}
		}

		public static void Do<T>(this IEnumerable<T>? @this, Action<T> action)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			foreach (var item in @this.ToCustomEnumerable())
				action(item);
		}

		public static void Do<T>(this IEnumerable<T>? @this, Action<T, int> action)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			var i = -1;
			switch (@this)
			{
				case T[] array:
					while (i < array.Length)
						action(array[i], ++i);
					return;
				case ImmutableArray<T> immutableArray:
					while (i < immutableArray.Length)
						action(immutableArray[i], ++i);
					return;
				case List<T> list:
					while (i < list.Count)
						action(list[i], ++i);
					return;
				default:
					foreach (var item in @this.ToCustomEnumerable())
						action(item, ++i);
					return;
			}
		}

		public static void Do<T>(this IEnumerable<T>? @this, Action<T> action, Action between)
		{
			action.AssertNotNull(nameof(action));
			between.AssertNotNull(nameof(between));

			if (!@this.Any())
				return;

			switch (@this)
			{
				case T[] array:
					action(array[0]);
					for (var i = 1; i < array.Length; ++i)
					{
						between();
						action(array[i]);
					}
					return;
				case ImmutableArray<T> array:
					action(array[0]);
					for (var i = 1; i < array.Length; ++i)
					{
						between();
						action(array[i]);
					}
					return;
				default:
					var enumerator = @this.ToCustomEnumerable().GetEnumerator();
					if (enumerator.MoveNext())
					{
						action(enumerator.Current);
						while (enumerator.MoveNext())
						{
							between();
							action(enumerator.Current);
						}
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

			switch (@this)
			{
				case T[] array:
					action(array[0], 0);
					for (var i = 1; i < array.Length; ++i)
					{
						between();
						action(array[i], i);
					}
					return;
				case ImmutableArray<T> array:
					action(array[0], 0);
					for (var i = 1; i < array.Length; ++i)
					{
						between();
						action(array[i], i);
					}
					return;
				default:
					var enumerator = @this.ToCustomEnumerable().GetEnumerator();
					if (enumerator.MoveNext())
					{
						action(enumerator.Current, 0);
						var i = 0;
						while (enumerator.MoveNext())
						{
							between();
							action(enumerator.Current, ++i);
						}
					}
					return;
			}
		}

		public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, Task> action)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			await Task.WhenAll(@this.To(item => action(item)));
		}

		public static async ValueTask DoAsync<T>(this IEnumerable<T>? @this, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
		{
			action.AssertNotNull(nameof(action));

			if (!@this.Any())
				return;

			await Task.WhenAll(@this.To(item => action(item, cancellationToken)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? First<T>(this IEnumerable<T>? @this) where T : class
			=> @this?.ToCustomEnumerable().First();

		public static T? First<T>(this IEnumerable<T>? @this, Func<T?, bool> filter) where T : class
        {
			filter.AssertNotNull(nameof(filter));

			return @this?.If(filter).First();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? First<T, V>(this IEnumerable<T>? @this) where T : class
			=> @this.If<T, V>().ToCustomEnumerable().First();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? FirstValue<T>(this IEnumerable<T>? @this) where T : struct
			=> @this?.ToCustomEnumerable().First();

		public static T? FirstValue<T>(this IEnumerable<T>? @this, Func<T, bool> filter) where T : struct
		{
			filter.AssertNotNull(nameof(filter));

			return @this.If(filter).FirstValue();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Gather<T>(this IEnumerable<IEnumerable<T>?>? @this)
			=> Empty<T>().And(@this);

		public static T? Get<T>(this IEnumerable<T>? @this, Index index) where T : class
		{
			if (!@this.Any() || index.Value < 0)
				return null;

			if (index.IsFromEnd)
				index = @this switch
				{
					IReadOnlyCollection<T> collection when collection.Count > 0 => index.Normalize(collection.Count),
					ICollection<T> collection when collection.Count > 0 => index.Normalize(collection.Count),
					ICollection collection when collection.Count > 0 => index.Normalize(collection.Count),
					_ => index.Normalize(@this.Count()),
				};

			return @this switch
			{
				T[] array => index.Value < array.Length ? array[index.Value] : null,
				ImmutableArray<T> immutableArray => index.Value < immutableArray.Length ? immutableArray[index.Value] : null,
				_ => @this.ToCustomEnumerable().GetItem(index)
			};
		}

		public static T? GetValue<T>(this IEnumerable<T>? @this, Index index) where T : struct
		{
			if (!@this.Any() || index.Value < 0)
				return default;

			if (index.IsFromEnd)
				index = @this switch
				{
					IReadOnlyCollection<T> collection when collection.Count > 0 => index.Normalize(collection.Count),
					ICollection<T> collection when collection.Count > 0 => index.Normalize(collection.Count),
					ICollection collection when collection.Count > 0 => index.Normalize(collection.Count),
					_ => index.Normalize(@this.Count()),
				};

			return @this switch
			{
				T[] array => index.Value < array.Length ? array[index.Value] : default,
				ImmutableArray<T> immutableArray => index.Value < immutableArray.Length ? immutableArray[index.Value] : default,
				_ => @this.ToCustomEnumerable().GetItem(index)
			};
		}

		public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Range range)
		{
			if (!@this.Any())
				yield break;

			if (range.Start.IsFromEnd || range.End.IsFromEnd)
				range = @this switch
				{
					ICollection<T> collection when collection.Count > 0 => range.Normalize(collection.Count),
					IReadOnlyCollection<T> collection when collection.Count > 0 => range.Normalize(collection.Count),
					ICollection collection when collection.Count > 0 => range.Normalize(collection.Count),
					_ => range.Normalize(@this.Count()),
				};

			if (range.Start.Value < 0 || range.End.Value < 0)
				range = new Range(range.Start.Value >= 0 ? range.Start : Index.Start, range.End.Value >= 0 ? range.End : Index.Start);

			if (@this is T[] array)
            {
				if (!range.IsReverse())
					for (var i = range.Start.Value; i < range.End.Value; ++i)
						yield return array[i];
				else
					for (var i = range.End.Value - 1; i >= range.Start.Value; --i)
						yield return array[i];
			}
			else if (@this is ImmutableArray<T> immutableArray)
            {
				if (!range.IsReverse())
					for (var i = range.Start.Value; i < range.End.Value; ++i)
						yield return immutableArray[i];
				else
					for (var i = range.End.Value - 1; i >= range.Start.Value; --i)
						yield return immutableArray[i];
			}
			else if (!range.IsReverse())
			{
				var count = range.Start.Value - range.End.Value;
				var items = new Stack<T>(count);

				var enumerator = @this.ToCustomEnumerable().GetEnumerator();
				if (range.End.Value > 0)
				{
					var i = range.End.Value;
					while (i > 0 && enumerator.MoveNext())
						--i;

					if (i == 0)
						yield return enumerator.Current;
					else
						yield break;
				}

				while (enumerator.MoveNext() && --count >= 0)
					items.Push(enumerator.Current);

				while (items.TryPop(out var item))
					yield return item;
			}
			else
			{
				var enumerator = @this.ToCustomEnumerable().GetEnumerator();
				if (range.Start.Value > 0)
				{
					var i = range.End.Value;
					while (i > 0 && enumerator.MoveNext())
						--i;

					if (i == 0)
						yield return enumerator.Current;
					else
						yield break;
				}

				var count = range.End.Value - range.Start.Value;
				while (enumerator.MoveNext() && --count >= 0)
					yield return enumerator.Current;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Func<T, IEnumerable<T>?> map)
			=> @this.And(@this.To<T, IEnumerable<T>?>(map));

		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
		{
			if (!@this.Any() || index.Value < 0)
				return default;

			if (index.IsFromEnd)
				index = @this switch
				{
					IReadOnlyCollection<T> collection when collection.Count > 0 => index.Normalize(collection.Count),
					ICollection<T> collection when collection.Count > 0 => index.Normalize(collection.Count),
					ICollection collection when collection.Count > 0 => index.Normalize(collection.Count),
					_ => index.Normalize(@this.Count()),
				};

			return @this switch
			{
				T[] array => index.Value < array.Length,
				ImmutableArray<T> immutableArray => index.Value < immutableArray.Length,
				_ => hasItem(@this, index),
			};

			static bool hasItem(IEnumerable<T> enumerable, Index index)
			{
				var enumerator = enumerable.ToCustomEnumerable().GetEnumerator();
				var i = index.Value;
				while (i > 0 && enumerator.MoveNext())
					--i;
				return i == 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value)
			=> @this.ToIndex(value).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
			=> @this.ToIndex(value, comparer).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values)
			=> values.All(value => @this.Has(value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T> comparer)
			=> values.All(value => @this.Has(value, comparer));

		public static IEnumerable<T?> If<T>(this IEnumerable<T?>? @this, Func<T?, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				if (filter(item))
					yield return item;
		}

		public static IEnumerable<R> If<T, R>(this IEnumerable<T?>? @this)
		{
			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				if (item is R value)
					yield return value;
		}

		public static async IAsyncEnumerable<T?> IfAsync<T>(this IEnumerable<T?>? @this, Func<T?, Task<bool>> filter, [EnumeratorCancellation] CancellationToken _ = default)
		{
			filter.AssertNotNull(nameof(filter));

			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				if (await filter(item))
					yield return item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
			=> (IEnumerable<T>)@this.If(_ => _ != null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
			=> @this.ToHashSet().SetEquals(items ?? Empty<T>());

		public static bool Is<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.ToHashSet(comparer).SetEquals(items ?? Empty<T>());
		}

		public static string Join<T>(this IEnumerable<T>? @this, char delimeter) => @this switch
		{
			null => string.Empty,
			T[] array => string.Join(delimeter, array),
			_ => string.Join(delimeter, @this)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, char delimeter, Func<T, string> map)
			=> @this != null ? string.Join(delimeter, @this.To(map)) : string.Empty;

		public static string Join<T>(this IEnumerable<T>? @this, string delimeter) => @this switch
		{
			null => string.Empty,
			T[] array => string.Join(delimeter, array),
			_ => string.Join(delimeter, @this)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, string delimeter, Func<T, string> map)
			=> @this != null ? string.Join(delimeter, @this.To(map)) : string.Empty;

		public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items != null)
				hashSet.IntersectWith(items);
			return hashSet;
		}

		public static HashSet<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items != null)
				hashSet.IntersectWith(items);
			return hashSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Maximum<T>(this IEnumerable<T>? @this) where T : struct, IComparable<T>
			=> @this.Aggregate((x, y) => x.MoreThan(y) ? x : y);

		public static T? Maximum<T>(this IEnumerable<T>? @this, IComparer<T> comparer) where T : struct
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.Aggregate(comparer.Maximum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Minimum<T>(this IEnumerable<T>? @this) where T : struct, IComparable<T>
			=> @this.Aggregate((x, y) => x.LessThan(y) ? x : y);

		public static T? Minimum<T>(this IEnumerable<T>? @this, IComparer<T> comparer) where T : struct
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.Aggregate(comparer.Minimum);
		}

		public static HashSet<T> Neither<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items != null)
				hashSet.SymmetricExceptWith(items);
			return hashSet;
		}

		public static HashSet<T> Neither<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items != null)
				hashSet.SymmetricExceptWith(items);
			return hashSet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Sort<T>(this IEnumerable<T>? @this) where T : IComparable<T>
			=> @this.Sort(Comparer<T>.Create((x, y) => x.CompareTo(y)));

		public static IEnumerable<T> Sort<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
		{
			var items = @this switch
			{
				null => new T[0],
				T[] array => array,
				List<T> list => list.ToArray(),
				HashSet<T> hashSet => hashSet.ToArray(),
				_ => @this.ToList().ToArray()
			};
			items.Sort(comparer);
			return items;
		}

		public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, V> map)
		{
			map.AssertNotNull(nameof(map));

			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				yield return map(item);
		}

		public static async IAsyncEnumerable<V?> ToAsync<T, V>(this IEnumerable<T?>? @this, Func<T?, Task<V>> map, [EnumeratorCancellation] CancellationToken _ = default)
		{
			map.AssertNotNull(nameof(map));

			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				yield return await map(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map)
			=> Empty<V>().And(@this.To<T, IEnumerable<V>>(map));

		public static T[] ToArray<T>(this IEnumerable<T>? @this)
		{
			if (!@this.Any())
				return Array.Empty<T>();

			var array = new T[@this.Count()];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		public static T[] ToArrayOf<T>(this IEnumerable<T>? @this, int length)
		{
			if (!@this.Any())
				return Array.Empty<T>();

			var array = new T[length];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToCsv<T>(this IEnumerable<T>? @this, Func<T, string> map)
			=> @this.To(map).ToCsv();

		public static string ToCsv<T>(this IEnumerable<T>? @this)
			=> @this.Any() ? string.Join(',', @this.To(value => value switch
			{
				bool _ => value.ToString(),
				byte _ => value.ToString(),
				sbyte _ => value.ToString(),
				short _ => value.ToString(),
				ushort _ => value.ToString(),
				int _ => value.ToString(),
				uint _ => value.ToString(),
				long _ => value.ToString(),
				ulong _ => value.ToString(),
				decimal _ => value.ToString(),
				double _ => value.ToString(),
				float _ => value.ToString(),
				char text => text.Equals(',') ? "\",\"" : text.Equals('\"') ? "\"\"\"\"" : text.ToString(),
				DateTime dateTime => dateTime.ToString("o"),
				DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("o"),
				TimeSpan time => time.ToString("c"),
				Guid guid => guid.ToString("D"),
				Enum token => token.Number(),
				string text => text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\""),
				null => string.Empty,
				_ => $"\"{value.ToString().Replace("\"", "\"\"")}\""
			})) : string.Empty;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static CustomEnumerable<T> ToCustomEnumerable<T>(this IEnumerable<T> @this)
			=> new CustomEnumerable<T>(@this);

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory) where K : notnull
		{
			valueFactory.AssertNotNull(nameof(valueFactory));

			var dictionary = new Dictionary<K, V>(@this.Count());
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K>? @this, Func<K, V> valueFactory, IEqualityComparer<K> comparer) where K : notnull
		{
			valueFactory.AssertNotNull(nameof(valueFactory));
			comparer.AssertNotNull(nameof(comparer));

			var dictionary = new Dictionary<K, V>(@this.Count(), comparer);
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory) where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			valueFactory.AssertNotNull(nameof(valueFactory));

			var dictionary = new Dictionary<K, V>(@this.Count());
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K> comparer) where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			valueFactory.AssertNotNull(nameof(valueFactory));
			comparer.AssertNotNull(nameof(comparer));

			var dictionary = new Dictionary<K, V>(@this.Count(), comparer);
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this)
			=> @this != null ? new HashSet<T>(@this) : new HashSet<T>(0);

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this != null ? new HashSet<T>(@this, comparer) : new HashSet<T>(comparer);
		}

		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T>? @this)
			=> @this is T[] array ? Unsafe.As<T[], ImmutableArray<T>>(ref array) : @this.ToImmutable(@this.Count());

		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T>? @this, int count)
		{
			if (@this == null)
				return ImmutableArray<T>.Empty;

			var arrayBuilder = ImmutableArray.CreateBuilder<T>(count);
			arrayBuilder.AddRange(@this);
			return arrayBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this)
		{
			if (@this == null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>();
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K> keyComparer)
		{
			keyComparer.AssertNotNull(nameof(keyComparer));

			if (@this == null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>(keyComparer);
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
		{
			keyComparer.AssertNotNull(nameof(keyComparer));
			valueComparer.AssertNotNull(nameof(valueComparer));

			if (@this == null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder(keyComparer, valueComparer);
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, Func<T, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			if (!@this.Any())
				yield break;

			var index = 0;
			foreach (var item in @this.ToCustomEnumerable())
			{
				if (filter(item))
					yield return index;
				++index;
			}
		}

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value) => value switch
		{
			IEquatable<T> equatable => @this.ToIndex(equatable.Equals),
			null => @this.ToIndex(item => item == null),
			_ => @this.ToIndex(item => object.Equals(item, value))
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
			=> @this.ToIndex(item => comparer.Equals(item, value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> ToList<T>(this IEnumerable<T>? @this)
			=> @this != null ? new List<T>(@this) : new List<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this)
			=> @this != null ? new Queue<T>(@this) : new Queue<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IEnumerable<T>? @this)
			=> @this.ToSpan();

		public static Span<T> ToSpan<T>(this IEnumerable<T>? @this) => @this switch
		{
			null => Span<T>.Empty,
			T[] array => array.AsSpan(),
			_ => @this.ToList().ToArray().AsSpan(),
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Stack<T> ToStack<T>(this IEnumerable<T>? @this)
			=> @this != null ? new Stack<T>(@this) : new Stack<T>(0);

		public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items != null)
				hashSet.UnionWith(items);
			return hashSet;
		}

		public static HashSet<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items != null)
				hashSet.UnionWith(items);
			return hashSet;
		}

		public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			if (items != null)
				hashSet.ExceptWith(items);
			return hashSet;
		}

		public static HashSet<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			if (items != null)
				hashSet.ExceptWith(items);
			return hashSet;
		}
	}
}