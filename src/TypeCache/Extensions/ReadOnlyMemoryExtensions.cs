// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class ReadOnlyMemoryExtensions
	{
		/// <summary>
		/// <see cref="MemoryMarshal"/>.ToEnumerable&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> @this)
			where T : struct
			=> MemoryMarshal.ToEnumerable(@this);

		/// <summary>
		/// <see cref="MemoryMarshal"/>.AsMemory&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Memory<T> ToMemory<T>(this ReadOnlyMemory<T> @this)
			where T : struct
			=> MemoryMarshal.AsMemory<T>(@this);
	}
}
