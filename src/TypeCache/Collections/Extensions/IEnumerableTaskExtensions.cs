// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TypeCache.Collections.Extensions
{
	public static class IEnumerableTaskExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task AllAsync<T>(this IEnumerable<Task> @this)
			=> await Task.WhenAll(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task AllAsync<T>(this Task[] @this)
			=> await Task.WhenAll(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<T[]> AllAsync<T>(this IEnumerable<Task<T>>? @this)
			=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<T[]> AllAsync<T>(this Task<T>[]? @this)
			=> @this.Any() ? await Task.WhenAll(@this) : await Task.FromResult(Array.Empty<T>());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task AnyAsync<T>(this IEnumerable<Task> @this)
			=> await Task.WhenAny(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task AnyAsync<T>(this Task[] @this)
			=> await Task.WhenAny(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<Task<T>> AnyAsync<T>(this IEnumerable<Task<T>> @this)
			=> await Task.WhenAny(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<Task<T>> AnyAsync<T>(this Task<T>[] @this)
			=> await Task.WhenAny(@this);
	}
}
