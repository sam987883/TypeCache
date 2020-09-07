// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T?> As<T>(this IEnumerable? @this) where T : class
		{
			if (@this == null)
				yield break;

			var enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
				yield return enumerator.Current as T;
		}

		public static (T Value, bool Exists) Aggregate<T>(this IEnumerable<T>? @this, Func<T, T, T> aggregator)
		{
			aggregator.AssertNotNull(nameof(aggregator));

			var result = default((T Value, bool));
			@this.Do(item => result = (aggregator(result.Value, item), true));
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
		public static IEnumerable<T> And<T>(this IEnumerable<T>? @this, params T[] items) =>
			@this.And(items as IEnumerable<T>);

		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this)
		{
			return @this switch
			{
				null => false,
				ICollection<T> collection => collection.Count > 0,
				IReadOnlyCollection<T> collection => collection.Count > 0,
				_ => any(@this)
			};

			static bool any(IEnumerable<T> enumerable)
			{
				using var enumerator = enumerable.GetEnumerator();
				return enumerator.MoveNext();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Func<T, bool> filter) =>
			@this.If(filter).Any();

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
			action.AssertNotNull(nameof(action));

			switch (@this)
			{
				case null:
					return;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> readOnlyList:
					if (readOnlyList.Count > 0)
					{
						action(readOnlyList[0]);
						if (between != null)
						{
							for (var i = 1; i < readOnlyList.Count; ++i)
							{
								between();
								action(readOnlyList[i]);
							}
						}
						else
						{
							for (var i = 1; i < readOnlyList.Count; ++i)
								action(readOnlyList[i]);
						}
					}
					return;
				case IList<T> list:
					if (list.Count > 0)
					{
						action(list[0]);
						if (between != null)
						{
							for (var i = 1; i < list.Count; ++i)
							{
								between();
								action(list[i]);
							}
						}
						else
						{
							for (var i = 1; i < list.Count; ++i)
								action(list[i]);
						}
					}
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
			action.AssertNotNull(nameof(action));

			switch (@this)
			{
				case null:
					return;
				case ImmutableList<T> _:
					goto default;
				case IReadOnlyList<T> readOnlyList:
					if (readOnlyList.Count > 0)
					{
						action(readOnlyList[0], 0);
						if (between != null)
						{
							for (var i = 1; i < readOnlyList.Count; ++i)
							{
								between();
								action(readOnlyList[i], i);
							}
						}
						else
						{
							for (var i = 1; i < readOnlyList.Count; ++i)
								action(readOnlyList[i], i);
						}
					}
					return;
				case IList<T> list:
					if (list.Count > 0)
					{
						action(list[0], 0);
						if (between != null)
						{
							for (var i = 1; i < list.Count; ++i)
							{
								between();
								action(list[i], i);
							}
						}
						else
						{
							for (var i = 1; i < list.Count; ++i)
								action(list[i], i);
						}
					}
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
		public static IEnumerable<T> Gather<T>(this IEnumerable<IEnumerable<T>?>? @this) =>
			Enumerable.Empty<T>().And(@this);

		public static T[] Get<T>(this IEnumerable<T>? @this, Range range) => @this switch
		{
			null => new T[0],
			T[] array => array.Get(range).ToArrayOf(array.Length),
			IReadOnlyCollection<T> readOnlyCollection => @this.Get(range).ToArrayOf(readOnlyCollection.Count),
			ICollection<T> collection => @this.Get(range).ToArrayOf(collection.Count),
			_ => @this.Get(range).ToList().ToArray(),
		};

		public static IEnumerable<T> Get<T>(this IEnumerable<T>? @this, Func<T, IEnumerable<T>?> getItems) =>
			@this.And(@this.To<T, IEnumerable<T>?>(getItems));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value) =>
			@this.ToIndex(value).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer) =>
			@this.ToIndex(value, comparer).Any();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values) =>
			values.All(value => @this.Has(value));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>([NotNullWhen(true)] this IEnumerable<T>? @this, IEnumerable<T>? values, IEqualityComparer<T> comparer) =>
			values.All(value => @this.Has(value, comparer));

		public static IEnumerable<T> If<T>([NotNullWhen(true)] this IEnumerable<T>? @this, Func<T, bool> filter)
		{
			filter.AssertNotNull(nameof(filter));

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
		public static IEnumerable<string> IfNotBlank(this IEnumerable<string?>? @this) =>
			@this.If(_ => !_.IsBlank());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this) where T : class =>
			@this.If(_ => _ != null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> IfNotNull<T>(this IEnumerable<T?>? @this) where T : struct =>
			@this.If(_ => _.HasValue).To(_ => _.Value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this IEnumerable<T>? @this, IEnumerable<T>? items) =>
			@this.ToHashSet().SetEquals(items ?? Enumerable.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<T>(this IEnumerable<T>? @this, IEnumerable<T>? items, IEqualityComparer<T> comparer) =>
			@this.ToHashSet(comparer).SetEquals(items ?? Enumerable.Empty<T>());

		public static string Join<T>(this IEnumerable<T>? @this, char delimeter) => @this switch
		{
			null => string.Empty,
			T[] array => string.Join(delimeter, array),
			_ => string.Join(delimeter, @this)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, char delimeter, Func<T, string> map) =>
			@this != null ? string.Join(delimeter, @this.To(map)) : string.Empty;

		public static string Join<T>(this IEnumerable<T>? @this, string delimeter) => @this switch
		{
			null => string.Empty,
			T[] array => string.Join(delimeter, array),
			_ => string.Join(delimeter, @this)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T>? @this, string delimeter, Func<T, string> map) =>
			@this != null ? string.Join(delimeter, @this.To(map)) : string.Empty;

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
		public static (T Value, bool Exists) Maximum<T>(this IEnumerable<T>? @this) where T : IComparable<T> =>
			@this.Aggregate((x, y) => x.CompareTo(y) > 0 ? x : y);

		public static (T Value, bool Exists) Maximum<T>(this IEnumerable<T>? @this, IComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.Aggregate(comparer.Maximum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (T Value, bool Exists) Minimum<T>(this IEnumerable<T>? @this) where T : IComparable<T> =>
			@this.Aggregate((x, y) => x.CompareTo(y) < 0 ? x : y);

		public static (T Value, bool Exists) Minimum<T>(this IEnumerable<T>? @this, IComparer<T> comparer)
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
		public static IEnumerable<T> Sort<T>(this IEnumerable<T>? @this) where T : IComparable<T> =>
			@this.Sort(Comparer<T>.Create((x, y) => x.CompareTo(y)));

		public static IEnumerable<T> Sort<T>(this IEnumerable<T>? @this, IComparer<T>? comparer = null)
		{
			var items = @this switch
			{
				null => new T[0],
				T[] array => array,
				_ => @this.ToList().ToArray()
			};
			items.Sort(comparer);
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
			map != null ? Enumerable.Empty<V>().And(@this.To<T, IEnumerable<V>>(map)) : Enumerable.Empty<V>();

		public static T[] ToArray<T>(this ICollection<T>? @this)
		{
			var array = new T[@this?.Count ?? 0];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		public static T[] ToArrayOf<T>(this IEnumerable<T>? @this, int length)
		{
			var array = new T[length];
			@this.Do((item, index) => array[index] = item);
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToCsv<T>(this IEnumerable<T>? @this, Func<T, string> map) =>
			@this.To(map).ToCsv();

		public static string ToCsv(this IEnumerable<string>? @this) =>
			@this.Any() ? string.Join(',', @this.To(text => text.Contains(',') ? $"\"{text.Replace("\"", "\"\"")}\"" : text.Replace("\"", "\"\""))) : string.Empty;

		public static string ToCsv<T>(this IEnumerable<T>? @this) =>
			@this.Any() ? string.Join(',', @this.To(value => value switch
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
				_ => $"\"{value.ToString().Replace("\"", "\"\"")}\"" ?? string.Empty
			})) : string.Empty;

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
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this) =>
			@this != null ? new HashSet<T>(@this) : new HashSet<T>(0);

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? @this, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this != null ? new HashSet<T>(@this, comparer) : new HashSet<T>(comparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<T> ToImmutable<T>(this IEnumerable<T>? @this) =>
			@this.ToImmutable(@this.Count());

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

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value) => value switch
		{
			IEquatable<T> equatable => @this.ToIndex(equatable.Equals),
			null => @this.ToIndex(item => item == null),
			_ => @this.ToIndex(item => object.Equals(item, value))
		};

		public static IEnumerable<int> ToIndex<T>(this IEnumerable<T>? @this, T value, IEqualityComparer<T> comparer)
		{
			comparer.AssertNotNull(nameof(comparer));

			return @this.ToIndex(item => comparer.Equals(item, value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> ToList<T>(this IEnumerable<T>? @this) =>
			@this != null ? new List<T>(@this) : new List<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Queue<T> ToQueue<T>(this IEnumerable<T>? @this) =>
			@this != null ? new Queue<T>(@this) : new Queue<T>(0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IEnumerable<T>? @this) =>
			@this.ToSpan();

		public static Span<T> ToSpan<T>(this IEnumerable<T>? @this) => @this switch
		{
			null => Span<T>.Empty,
			T[] array => array.AsSpan(),
			_ => @this.ToList().ToArray().AsSpan(),
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Stack<T> ToStack<T>(this IEnumerable<T>? @this) =>
			@this != null ? new Stack<T>(@this) : new Stack<T>(0);

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