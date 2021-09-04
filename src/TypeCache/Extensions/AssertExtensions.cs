// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class AssertExtensions
	{
		/// <exception cref="ArgumentOutOfRangeException"/>
		public static void Assert<T>(this T @this, string name, T value, [CallerMemberName] string? caller = null)
			where T : struct
		{
			name.AssertNotBlank(nameof(name));

			if ((@this is IEquatable<T> equatable && !equatable.Equals(value)) || !object.Equals(@this, value))
				throw new ArgumentOutOfRangeException($"{caller} -> {nameof(Assert)}: [{@this}] <> [{value.ToString()}].", name);
		}

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public static void Assert<T>(this T? @this, string name, T? value, IEqualityComparer<T> comparer, [CallerMemberName] string? caller = null)
		{
			name.AssertNotBlank(nameof(name));
			comparer.AssertNotNull(nameof(comparer));

			if (!comparer.Equals(@this, value))
				throw new ArgumentOutOfRangeException($"{caller} -> {nameof(Assert)}: {(@this is not null ? $"[{@this}]" : "null")} <> {(value is not null ? $"[{value}]" : "null")}.", name);
		}

		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public static void Assert(this string? @this, string name, string? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase, [CallerMemberName] string? caller = null)
		{
			name.AssertNotNull(nameof(name), caller);

			if (!comparison.ToStringComparer().Equals(@this, value))
				throw new ArgumentOutOfRangeException($"{nameof(Assert)}: [{(@this is not null ? $"\"{@this}\"" : "null")}] <> {(value is not null ? $"\"{value}\"" : "null")}.", name);
		}

		/// <exception cref="ArgumentOutOfRangeException"/>
		public static void AssertNotBlank(this string? @this, string name, [CallerMemberName] string? caller = null)
		{
			if (@this.IsBlank())
				throw new ArgumentOutOfRangeException($"{caller} -> {nameof(AssertNotBlank)}: [{name}] is blank.");
		}

		/// <exception cref="ArgumentNullException"/>
		public static void AssertNotNull<T>(this T? @this, string name, [CallerMemberName] string? caller = null)
		{
			if (@this is null)
				throw new ArgumentNullException($"{caller} -> {nameof(AssertNotNull)}: [{name}] is null.");
		}

		/// <exception cref="ArgumentException"/>
		public static void AssertNotSame<T>(this (T?, T?) @this, (string, string) name, [CallerMemberName] string? caller = null)
		{
			if (object.ReferenceEquals(@this.Item1, @this.Item2))
				throw new ArgumentException($"{caller} -> {nameof(AssertNotSame)}: [{name.Item1}] cannot be the same as [{name.Item2}].");
		}
	}
}
