﻿// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sam987883.Common
{
	public sealed class CustomComparer<T> : IComparer<T>
	{
		private readonly Comparison<T> _Compare;

		public CustomComparer(Comparison<T> compare)
			=> this._Compare = compare;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int IComparer<T>.Compare([AllowNull] T x, [AllowNull] T y)
			=> this._Compare(x, y);
	}
}
