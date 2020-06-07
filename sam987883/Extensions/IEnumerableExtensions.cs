// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using sam987883.Collections;
using sam987883.Common;

namespace sam987883.Extensions
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T?> As<T>(this IEnumerable @this) where T : class
		{
			if (@this == null)
				yield break;

			var enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
				yield return enumerator.Current as T;
		}

		public static (T Value, bool Exists) Aggregate<T>(this IEnumerable<T>? @this, Func<T, T, T> aggregator)
		{
			var result = default((T Value, bool));
			@this.Do(item => result = (aggregator(result.Value, item), true));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool All<T>(this IEnumerable<T> @this, Func<T, bool> filter) =>
			!@this.If(item => !filter(item)).Any();

		public static IEnumerable<T> And<T>(this IEnumerable<T> @this, IEnumerable<IEnumerable<T>> items)
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

		public static IEnumerable<T> And<T>(this IEnumerable<T> @this, params IEnumerable<T>[] items)
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
				enumerable = items[index++];
				goto Action;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> And<T>(this IEnumerable<T> @this, params T[] items) =>
			@this.And(items as IEnumerable<T>);

		public static bool Any<T>(this IEnumerable<T>? @this)
		{
			return @this switch
			{
				null => false,
				ICollection<T> collection => collection.Any(),
				IReadOnlyCollection<T> collection => collection.Any(),
				_ => any(@this)
			};

			static bool any(IEnumerable<T> enumerable)
			{
				using var enumerator = enumerable.GetEnumerator();
				return enumerator.MoveNext();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>(this IEnumerable<T>? @this, Func<T, bool> filter) =>
			@this.If(filter).Any();

		public static IReadOnlyList<T>? AsReadOnlyList<T>(this IEnumerable<T>? @this) => @this switch
		{
			IList<T> list => new ReadOnlyList<T>(list),
			IReadOnlyList<T> list => list,
			_ => null
		};

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

		public static void Do<T>(this IEnumerable<T>? @this, Action<T> action, Action? between = null)
		{
			switch (@this)
			{
				case null:
					return;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> readOnlyList:
					readOnlyList.Do(action, between);
					return;
				case IList<T> list:
					list.Do(action, between);
					return;
				default:
					using (var enumerator = @this.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							action(enumerator.Current);
							if (between != null)
							{
								while (enumerator.MoveNext())
								{
									between();
									action(enumerator.Current);
								}
							}
							else
							{
								while (enumerator.MoveNext())
									action(enumerator.Current);
							}
						}
					}
					return;
			}
		}

		public static void Do<T>(this IEnumerable<T>? @this, Action<T, int> action, Action? between = null)
		{
			switch (@this)
			{
				case null:
					return;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> readOnlyList:
					readOnlyList.Do(action, between);
					return;
				case IList<T> list:
					list.Do(action, between);
					return;
				default:
					using (var enumerator = @this.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							var i = 0;
							action(enumerator.Current, i);
							if (between != null)
							{
								while (enumerator.MoveNext())
								{
									between();
									action(enumerator.Current, ++i);
								}
							}
							else
							{
								while (enumerator.MoveNext())
									action(enumerator.Current, ++i);
							}
						}
					}
					return;
			}
		}

		public static (T Value, bool Exists) First<T>(this IEnumerable<T>? @this)
		{
			return @this switch
			{
				null => (default, false),
				IReadOnlyList<T> list when list.Count > 0 => (list[0], true),
				IList<T> list when list.Count > 0 => (list[0], true),
				IReadOnlyList<T> _ => (default, false),
				IList<T> _ => (default, false),
				_ => first(@this)
			};

			static (T, bool) first(IEnumerable<T> enumerable)
			{
				using var enumerator = enumerable.GetEnumerator();
				var exists = enumerator.MoveNext();
				var item = exists ? enumerator.Current : default;
				return (item, exists);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) First<T>(this IEnumerable<T>? @this, Func<T, bool> filter) =>
			@this.If(filter).First();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Gather<T>(this IEnumerable<IEnumerable<T>> @this) =>
			Factory.Empty<T>().And(@this);

		public static T[] Get<T>(this IEnumerable<T>? @this, Range range) => @this switch
		{
			null => new T[0],
			T[] array => array.Get(range).ToArray(),
			IReadOnlyCollection<T> readOnlyCollection => @this.ToArray(readOnlyCollection.Count).Get(range).ToArray(),
			ICollection<T> collection => @this.ToArray(collection.Count).Get(range).ToArray(),
			_ => @this.ToArray().Get(range).ToArray(),
		};

		public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Func<T, IEnumerable<T>> getItems) =>
			@this.And(@this.To<T, IEnumerable<T>>(getItems));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T>? comparer = null) =>
			@this.ToIndex(value, comparer).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this IEnumerable<T> @this, IEnumerable<T> values, IEqualityComparer<T>? comparer = null) =>
			values.All(value => @this.Has(value, comparer));

		public static IEnumerable<T> If<T>(this IEnumerable<T>? @this, Func<T, bool> filter)
		{
			switch (@this)
			{
				case null:
					yield break;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> list:
					for (var i = 0; i < list.Count; ++i)
					{
						var item = list[i];
						if (filter(item))
							yield return item;
					}
					yield break;
				case IList<T> list:
					for (var i = 0; i < list.Count; ++i)
					{
						var item = list[i];
						if (filter(item))
							yield return item;
					}
					yield break;
				default:
					using (var enumerator = @this.GetEnumerator())
					{
						var (value, exists) = enumerator.MoveUntil(filter);
						while (exists)
						{
							yield return value;
							(value, exists) = enumerator.MoveUntil(filter);
						}
					}
					yield break;
			}
		}

		public static IEnumerable<R> If<T, R>(this IEnumerable<T>? @this)
		{
			switch (@this)
			{
				case null:
					yield break;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> list:
					for (var i = 0; i < list.Count; ++i)
					{
						if (list[i] is R item)
							yield return item;
					}
					yield break;
				case IList<T> list:
					for (var i = 0; i < list.Count; ++i)
					{
						if (list[i] is R item)
							yield return item;
					}
					yield break;
				default:
					using (var enumerator = @this.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current is R item)
								yield return item;
						}
					}
					yield break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this IEnumerable<T>? @this, IEnumerable<T>? items) =>
			@this.ToHashSet().SetEquals(items ?? Factory.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer) =>
			@this.ToHashSet(comparer).SetEquals(items ?? Factory.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T> @this, char delimeter) =>
			string.Join(delimeter, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T> @this, char delimeter, Func<T, string> map) =>
			string.Join(delimeter, @this.To(map));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T> @this, string delimeter) =>
			string.Join(delimeter, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T> @this, string delimeter, Func<T, string> map) =>
			string.Join(delimeter, @this.To(map));

		public static IEnumerable<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = @this.ToHashSet();
			return items.If(item => !hashSet.Add(item));
		}

		public static IEnumerable<T> Match<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = @this.ToHashSet(comparer);
			return items.If(item => !hashSet.Add(item));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Maximum<T>(this IEnumerable<T> @this) where T : IComparable<T> =>
			@this.Aggregate((x, y) => x.CompareTo(y) > 0 ? x : y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Maximum<T>(this IEnumerable<T> @this, IComparer<T> comparer) =>
			@this.Aggregate(comparer.Maximum);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Minimum<T>(this IEnumerable<T> @this) where T : IComparable<T> =>
			@this.Aggregate((x, y) => x.CompareTo(y) < 0 ? x : y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Minimum<T>(this IEnumerable<T> @this, IComparer<T> comparer) =>
			@this.Aggregate(comparer.Minimum);

		public static IEnumerable<T> Neither<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer = null) =>
			@this.Without(items).Union(items.Without(@this));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T>? Sort<T>(this IEnumerable<T> @this) where T : IComparable<T> =>
			@this.Sort(Comparer<T>.Create((x, y) => x.CompareTo(y)));

		public static IEnumerable<T>? Sort<T>(this IEnumerable<T>? @this, IComparer<T> comparer)
		{
			var items = @this switch
			{
				null => null,
				T[] array => array,
				_ => @this.ToArray()
			};
			items?.Sort(comparer);
			return items;
		}

		public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, V> map)
		{
			switch (@this)
			{
				case null:
					yield break;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> list:
					for (var i = 0; i < list.Count; ++i)
						yield return map(list[i]);
					yield break;
				case IList<T> list:
					for (var i = 0; i < list.Count; ++i)
						yield return map(list[i]);
					yield break;
				default:
					using (var enumerator = @this.GetEnumerator())
					{
						while (enumerator.MoveNext())
							yield return map(enumerator.Current);
					}
					yield break;
			}
		}

		public static IEnumerable<V> To<T, V>(this IEnumerable<T>? @this, Func<T, IEnumerable<V>> map) =>
			map != null ? Factory.Empty<V>().And(@this.To<T, IEnumerable<V>>(map)) : Factory.Empty<V>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ToArray<T>(this IEnumerable<T>? @this) =>
			@this.ToArray(@this.Count());

		public static T[] ToArray<T>(this IEnumerable<T>? @this, int length)
		{
			var array = new T[length];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToCsv<T>(this IEnumerable<T>? @this) =>
			@this != null ? string.Join(", ", @this) : string.Empty;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToCsv<T>(this IEnumerable<T>? @this, Func<T, string> map) =>
			@this != null ? string.Join(", ", @this.To(map)) : string.Empty;

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K> @this, Func<K, V> valueFactory) where K : notnull
		{
			var dictionary = new Dictionary<K, V>();
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K> @this, Func<K, V> valueFactory, IEqualityComparer<K> comparer) where K : notnull
		{
			var dictionary = new Dictionary<K, V>(comparer);
			@this?.Do(key => dictionary.Add(key, valueFactory(key)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T> @this, Func<T, K> keyFactory, Func<T, V> valueFactory) where K : notnull
		{
			var dictionary = new Dictionary<K, V>();
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T> @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K> comparer) where K : notnull
		{
			var dictionary = new Dictionary<K, V>(comparer);
			@this?.Do(value => dictionary.Add(keyFactory(value), valueFactory(value)));
			return dictionary;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this) =>
			@this != null ? new HashSet<T>(@this) : new HashSet<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T> comparer) =>
			@this != null ? new HashSet<T>(@this, comparer) : new HashSet<T>(comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T> @this) =>
			@this.ToImmutable(@this.Count());

		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T> @this, int count)
		{
			var arrayBuilder = ImmutableArray.CreateBuilder<T>(count);
			arrayBuilder.AddRange(@this);
			return arrayBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>> @this)
		{
			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>();
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>> @this, IEqualityComparer<K> keyComparer)
		{
			var dictionaryBuilder = ImmutableDictionary.CreateBuilder<K, V>(keyComparer);
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IImmutableDictionary<K, V> ToImmutable<K, V>(this IEnumerable<KeyValuePair<K, V>> @this, IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer)
		{
			var dictionaryBuilder = ImmutableDictionary.CreateBuilder(keyComparer, valueComparer);
			dictionaryBuilder.AddRange(@this);
			return dictionaryBuilder.ToImmutable();
		}

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, Func<T, bool> filter)
		{
			switch (@this)
			{
				case null:
					yield break;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> list:
					for (var i = 0; i < list.Count; ++i)
					{
						if (filter(list[i]))
							yield return i;
					}
					yield break;
				case IList<T> list:
					for (var i = 0; i < list.Count; ++i)
					{
						if (filter(list[i]))
							yield return i;
					}
					yield break;
				default:
					using (var enumerator = @this.GetEnumerator())
					{
						var index = -1;
						while (enumerator.MoveUntil(filter).Exists)
							yield return ++index;
					}
					yield break;
			}
		}

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value) =>
			@this.ToIndex(item => object.Equals(item, value));

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer) =>
			@this.ToIndex(item => comparer.Equals(item, value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> ToList<T>(this IEnumerable<T>? @this) =>
			@this != null ? new List<T>(@this) : new List<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this) =>
			@this != null ? new Queue<T>(@this) : new Queue<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyDictionary<K, V> ToReadOnlyDictionary<K, V>(this IEnumerable<K> @this, Func<K, V> valueFactory, IEqualityComparer<K>? comparer = null) where K : notnull =>
			new ReadOnlyDictionary<K, V>(@this.ToDictionary(valueFactory, comparer));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyDictionary<K, V> ToReadOnlyDictionary<T, K, V>(this IEnumerable<T> @this, Func<T, K> keyFactory, Func<T, V> valueFactory, IEqualityComparer<K>? comparer = null) where K : notnull =>
			new ReadOnlyDictionary<K, V>(@this.ToDictionary(keyFactory, valueFactory, comparer));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> @this) =>
			new ReadOnlyList<T>(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IEnumerable<T>? @this) =>
			@this.ToSpan();

		public static Span<T> ToSpan<T>(this IEnumerable<T>? @this) => @this switch
		{
			null => Span<T>.Empty,
			T[] array => array.AsSpan(),
			_ => @this.ToArray().AsSpan(),
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Stack<T> ToStack<T>(this IEnumerable<T>? @this) =>
			@this != null ? new Stack<T>(@this) : new Stack<T>(0);

		public static IEnumerable<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = items.ToHashSet();
			return @this.If(hashSet.Add).And(items.If(hashSet.Add));
		}

		public static IEnumerable<T> Union<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = items.ToHashSet(comparer);
			return @this.If(hashSet.Add).And(items.If(hashSet.Add));
		}

		public static IEnumerable<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items)
		{
			var hashSet = items.ToHashSet();
			return @this.If(hashSet.Add);
		}

		public static IEnumerable<T> Without<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer)
		{
			var hashSet = items.ToHashSet(comparer);
			return @this.If(hashSet.Add);
		}
	}
}