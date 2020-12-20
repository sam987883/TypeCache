// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Common;

namespace TypeCache.Extensions
{
	public static class EnumExtensions
	{
		private static IDictionary<RuntimeTypeHandle, CollectionType> _GenericCollectionTypeMap =
			new Dictionary<RuntimeTypeHandle, CollectionType>
			{
				{ typeof(IDictionary<,>).TypeHandle, CollectionType.Dictionary},
				{ typeof(IReadOnlyDictionary<,>).TypeHandle, CollectionType.Dictionary},
				{ typeof(Dictionary<,>).TypeHandle, CollectionType.Dictionary},
				{ typeof(HashSet<>).TypeHandle, CollectionType.HashSet},
				{ typeof(ImmutableArray<>).TypeHandle, CollectionType.ImmutableArray},
				{ typeof(ImmutableDictionary<,>).TypeHandle, CollectionType.ImmutableDictionary},
				{ typeof(ImmutableHashSet<>).TypeHandle, CollectionType.ImmutableHashSet},
				{ typeof(ImmutableList<>).TypeHandle, CollectionType.ImmutableList},
				{ typeof(ImmutableQueue<>).TypeHandle, CollectionType.ImmutableQueue},
				{ typeof(ImmutableSortedDictionary<,>).TypeHandle, CollectionType.ImmutableSortedDictionary},
				{ typeof(ImmutableSortedSet<>).TypeHandle, CollectionType.ImmutableSortedSet},
				{ typeof(ImmutableStack<>).TypeHandle, CollectionType.ImmutableStack},
				{ typeof(LinkedList<>).TypeHandle, CollectionType.LinkedList},
				{ typeof(IList<>).TypeHandle, CollectionType.List},
				{ typeof(IReadOnlyList<>).TypeHandle, CollectionType.List},
				{ typeof(List<>).TypeHandle, CollectionType.List},
				{ typeof(Queue<>).TypeHandle, CollectionType.Queue},
				{ typeof(SortedDictionary<,>).TypeHandle, CollectionType.SortedDictionary},
				{ typeof(SortedList<,>).TypeHandle, CollectionType.SortedList},
				{ typeof(SortedSet<>).TypeHandle, CollectionType.SortedSet},
				{ typeof(Stack<>).TypeHandle, CollectionType.Stack}
			};

		public static CollectionType ToCollectionType(this Type @this)
			=> @this.IsArray ? CollectionType.Array : _GenericCollectionTypeMap.GetValue(@this.TypeHandle)
				?? (@this.Implements(typeof(IEnumerable)) ? CollectionType.Enumerable : CollectionType.None);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Hex<T>(this T @this) where T : Enum
			=> @this.ToString("X");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Name<T>(this T @this) where T : Enum
			=> @this.ToString("G");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Number<T>(this T @this) where T : Enum
			=> @this.ToString("D");
	}
}
