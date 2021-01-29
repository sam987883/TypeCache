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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IEnumerable<T> Empty<T>()
		{
			yield break;
		}

		public static IEnumerable<T> As<T>(this IEnumerable? @this)
			where T : notnull
		{
			if (@this == null)
				yield break;

			var enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
				if (enumerator.Current is T value)
					yield return value;
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

		public static bool All<T>(this IEnumerable<T>? @this, Func<T?, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			return !@this.If(item => !filter(item)).Any();
		}

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, IEnumerable<IEnumerable<T>?>? sets)
		{
			if (@this.Any())
			{
				foreach (var item in @this.ToCustomEnumerable())
					yield return item;
			}

			if (sets.Any())
			{
				foreach (var items in sets.IfNotNull().ToCustomEnumerable())
					foreach (var item in items.ToCustomEnumerable())
						yield return item;
			}
		}

		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params IEnumerable<T>?[] sets)
		{
			if (@this.Any())
			{
				foreach (var item in @this.ToCustomEnumerable())
					yield return item;
			}

			var length = sets?.Length ?? 0;
			for (var i = 0; i < length; ++i)
			{
				var enumerable = sets![i]?.ToCustomEnumerable();
				if (enumerable != null)
					foreach (var item in enumerable)
						yield return item;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params T[] items)
			=> @this.And(items as IEnumerable<T>);

		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
			=> @this switch
			{
				null => false,
				T[] array => array.Length > 0,
				ImmutableArray<T> array => array.Length > 0,
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
			=> @this switch
			{
				null => 0,
				T[] array => array.Length,
				ImmutableArray<T> array => array.Length,
				ICollection<T> collection => collection.Count,
				IReadOnlyCollection<T> collection => collection.Count,
				ICollection collection => collection.Count,
				_ => @this.ToCustomEnumerable().Count()
			};

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
					while (++i < array.Length)
						action(array[i], i);
					return;
				case ImmutableArray<T> immutableArray:
					while (++i < immutableArray.Length)
						action(immutableArray[i], i);
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
					var first = true;
					foreach (var item in @this.ToCustomEnumerable())
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
				case T[] array:
					action(array[0], i);
					for (i = 1; i < array.Length; ++i)
					{
						between();
						action(array[i], i);
					}
					return;
				case ImmutableArray<T> array:
					action(array[0], i);
					for (i = 1; i < array.Length; ++i)
					{
						between();
						action(array[i], i);
					}
					return;
				default:
					foreach (var item in @this.ToCustomEnumerable())
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
		public static T? First<T>(this IEnumerable<T?>? @this) where T : class
			=> @this?.ToCustomEnumerable().First();

		public static T? First<T>(this IEnumerable<T?>? @this, Func<T?, bool> filter) where T : class
		{
			filter.AssertNotNull(nameof(filter));

			return @this.If(filter).First();
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
				index = index.Normalize(@this.Count());

			return @this switch
			{
				ImmutableList<T> _ => @this.ToCustomEnumerable().Get(index),
				T[] array when index.Value < array.Length => array[index.Value],
				T[] _ => null,
				IList<T> list when index.Value < list.Count => list[index.Value],
				IList<T> _ => null,
				IReadOnlyList<T> list when index.Value < list.Count => list[index.Value],
				IReadOnlyList<T> _ => null,
				_ => @this.ToCustomEnumerable().Get(index)
			};
		}

		public static T? GetValue<T>(this IEnumerable<T>? @this, Index index) where T : struct
		{
			if (!@this.Any() || index.Value < 0)
				return default;

			if (index.IsFromEnd)
				index = index.Normalize(@this.Count());

			return @this switch
			{
				ImmutableList<T> _ => @this.ToCustomEnumerable().Get(index),
				T[] array when index.Value < array.Length => array[index.Value],
				T[] _ => null,
				IList<T> list when index.Value < list.Count => list[index.Value],
				IList<T> _ => null,
				IReadOnlyList<T> list when index.Value < list.Count => list[index.Value],
				IReadOnlyList<T> _ => null,
				_ => @this.ToCustomEnumerable().Get(index)
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
				if (!range.IsReverse())
					for (var i = range.Start.Value; i < range.End.Value; ++i)
						yield return array[i];
				else
					for (var i = range.End.Value - 1; i >= range.Start.Value; --i)
						yield return array[i];
			}
			else if (@this is IList<T> list)
			{
				if (!range.IsReverse())
					for (var i = range.Start.Value; i < range.End.Value; ++i)
						yield return list[i];
				else
					for (var i = range.End.Value - 1; i >= range.Start.Value; --i)
						yield return list[i];
			}
			else if (@this is IReadOnlyList<T> readOnlyList)
			{
				if (!range.IsReverse())
					for (var i = range.Start.Value; i < range.End.Value; ++i)
						yield return readOnlyList[i];
				else
					for (var i = range.End.Value - 1; i >= range.Start.Value; --i)
						yield return readOnlyList[i];
			}
			else if (range.IsReverse())
			{
				var count = range.Start.Value - range.End.Value;
				var items = new Stack<T>(count);

				using var enumerator = @this.ToCustomEnumerable().GetEnumerator();
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
				using var enumerator = @this.ToCustomEnumerable().GetEnumerator();
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
			=> @this.And(@this.To(map));

		public static IDictionary<K, IEnumerable<V>> Group<K, V>(this IEnumerable<V>? @this, Func<V, K> keyFactory, IEqualityComparer<K> comparer)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			comparer.AssertNotNull(nameof(comparer));

			(K Key, V Value)[] items = @this.To(item => (keyFactory(item), item)).ToArray(@this.Count());
			return items
				.To(pair => pair.Key)
				.ToHashSet()
				.ToDictionary(key => items.If(pair => comparer.Equals(pair.Key, key)).To(pair => pair.Value));
		}

		public static IDictionary<K, IEnumerable<V>> Group<T, K, V>(this IEnumerable<T>? @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K> comparer)
			where K : notnull
		{
			keyFactory.AssertNotNull(nameof(keyFactory));
			valueFactory.AssertNotNull(nameof(valueFactory));
			comparer.AssertNotNull(nameof(comparer));

			(K Key, V Value)[] items = @this.To(item => (keyFactory(item), valueFactory(item))).ToArray(@this.Count());
			return items
				.To(pair => pair.Key)
				.ToHashSet()
				.ToDictionary(key => items.If(pair => comparer.Equals(pair.Key, key)).To(pair => pair.Value));
		}

		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Index index)
		{
			if (!@this.Any() || index.Value < 0)
				return default;

			if (index.IsFromEnd)
				index = index.Normalize(@this.Count());

			return @this switch
			{
				T[] array => index.Value < array.Length,
				IList<T> list => index.Value < list.Count,
				IReadOnlyList<T> list => index.Value < list.Count,
				_ => @this.ToCustomEnumerable().Has(index),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value)
			where T : struct
			=> @this.ToIndex(value).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEquatable<T> value)
			=> @this.ToIndex(value).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
			=> @this.ToIndex(value, comparer).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values)
			where T : struct
			=> values.All(@this.Has);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<IEquatable<T>>? values)
			=> values.All(@this.Has!);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T> comparer)
			=> values.All(value => @this.Has(value, comparer!));

		public static IEnumerable<T?> If<T>(this IEnumerable<T?>? @this, Func<T?, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				if (filter(item))
					yield return item;
		}

		public static IEnumerable<V> If<T, V>(this IEnumerable<T?>? @this)
		{
			if (!@this.Any())
				yield break;

			foreach (var item in @this.ToCustomEnumerable())
				if (item is V value)
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
			where T : class
			=> @this.If(_ => _ != null)!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this)
			where T : struct
			=> @this.If(_ => _.HasValue).To(_ => _!.Value);

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
				null => Array.Empty<T>(),
				T[] array => array,
				List<T> list => list.ToArray(),
				HashSet<T> hashSet => hashSet.ToArray(),
				_ => @this.ToArray()
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
		public static IEnumerable<V> ToMany<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map)
			=> Empty<V>().And(@this.To(map));

		public static T[] ToArray<T>(this IEnumerable<T>? @this)
		{
			if (!@this.Any())
				return Array.Empty<T>();

			var array = new T[@this.Count()];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		public static T[] ToArray<T>(this IEnumerable<T>? @this, int length)
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
				Enum token => token.ToString(),
				string text => text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\""),
				null => string.Empty,
				_ => $"\"{value.ToString()?.Replace("\"", "\"\"")}\""
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
			where T : notnull
			=> @this is T[] array ? Unsafe.As<T[], ImmutableArray<T>>(ref array) : @this.ToImmutable(@this.Count());

		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T>? @this, int count)
			where T : notnull
		{
			if (@this == null)
				return ImmutableArray<T>.Empty;

			var arrayBuilder = ImmutableArray.CreateBuilder<T>(count);
			arrayBuilder.AddRange(@this);
			return arrayBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this)
			where K : notnull
		{
			if (@this == null)
				return ImmutableDictionary<K, V>.Empty;

			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>();
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>>? @this, IEqualityComparer<K> keyComparer)
			where K : notnull
		{
			keyComparer.AssertNotNull(nameof(keyComparer));

			if (@this == null)
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

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value)
			where T : struct
			=> value switch
			{
				IEquatable<T> equatable => @this.ToIndex(equatable.Equals),
				_ => @this.ToIndex(item => Equals(item, value))
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, IEquatable<T> value)
			=> @this.ToIndex(value.Equals);

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
			_ => @this.ToArray().AsSpan(),
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
