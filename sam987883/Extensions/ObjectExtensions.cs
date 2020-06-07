// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;

namespace sam987883.Extensions
{
	public static class ObjectExtensions
	{
		public static void Assert<T>(this T @this, string name, T value)
		{
			@this.AssertNotNull(name);
			if (!Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: {@this} <> {(value != null ? value.ToString() : "null")}.", name);
		}

		public static void Assert<T>(this T @this, string name, T value, IEqualityComparer<T> comparer)
		{
			@this.AssertNotNull(name);
			if (!comparer.Equals(@this, value))
				throw new ArgumentException($"{nameof(Assert)}: {@this} <> {(value != null ? value.ToString() : "null")}.", name);
		}

		public static void AssertNotNull<T>(this T @this, string name)
		{
			if (@this == null)
				throw new NullReferenceException($"{nameof(Assert)}: [{name}] is null.");
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
