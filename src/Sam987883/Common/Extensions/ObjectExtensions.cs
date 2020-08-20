// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sam987883.Common.Extensions
{
	public static class ObjectExtensions
	{
		public static void Assert<T>([AllowNull] this T @this, string name, [AllowNull] T value)
		{
			name.AssertNotNull(nameof(name));

			switch (@this)
			{
				case IEquatable<T> equatable when !equatable.Equals(value):
					throw new ArgumentException($"{nameof(Assert)}: [{@this}] <> [{(value != null ? value.ToString() : "null")}].", name);
				case null when value != null:
					throw new ArgumentException($"{nameof(Assert)}: null <> [{value}].", name);
				case IEquatable<T> _:
				case null:
					return;
				default:
					if (!object.Equals(@this, value))
						throw new ArgumentException($"{nameof(Assert)}: [{@this}] <> [{(value != null ? value.ToString() : "null")}].", name);
					return;
			}
		}

		public static void Assert<T>([AllowNull] this T @this, string name, [AllowNull] T value, IEqualityComparer<T> comparer)
		{
			name.AssertNotNull(nameof(name));
			comparer.AssertNotNull(nameof(comparer));

			if (!comparer.Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: {(@this != null ? $"[{@this}]" : "null")} <> {(value != null ? $"[{value}]" : "null")}.", name);
		}

		public static void AssertNotNull<T>([AllowNull] this T @this, string name)
		{
			if (@this == null)
				throw new NullReferenceException($"{nameof(AssertNotNull)}: [{name}] is null.");
		}

		public static Type? GetTypeOf(this object @this, Type type)
		{
			if (@this == null || type == null)
				return null;

			var thisType = @this.GetType();
			if (thisType == type)
				return type;

			if (type.IsGenericType)
			{
				type = type.GetGenericTypeDefinition();
				var (matchingType, exists) = thisType.GetInterfaces().First(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type);
				if (exists)
					return matchingType;
			}

			return null;
		}

		public static IEnumerable<T> Repeat<T>(this T @this, int count)
		{
			while (count > 0)
			{
				yield return @this;
				--count;
			}
		}
	}
}
