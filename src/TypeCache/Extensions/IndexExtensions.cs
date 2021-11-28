// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class IndexExtensions
	{
		/// <summary>
		/// <c><see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value + <paramref name="increment"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Index Next(this Index @this, int increment = 1)
			=> new Index(@this.Value + increment);

		/// <summary>
		/// <c>@<paramref name="this"/>.IsFromEnd ? <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.GetOffset(<paramref name="count"/>)) : @<paramref name="this"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Index Normalize(this Index @this, int count)
			=> @this.IsFromEnd ? new Index(@this.GetOffset(count)) : @this;

		/// <summary>
		/// <c><see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value - <paramref name="increment"/>)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Index Previous(this Index @this, int increment = 1)
			=> new Index(@this.Value - increment);
	}
}
