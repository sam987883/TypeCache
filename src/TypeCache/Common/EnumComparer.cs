// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Common
{
	internal sealed class EnumComparer<T> : IEnumComparer<T> where T : struct, Enum
	{
		private readonly Comparison<T> _Compare;

		public EnumComparer()
		{
			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			var underlyingType = typeof(T).GetEnumUnderlyingType();
			this._Compare = value1.ConvertTo(underlyingType)
				.Call(nameof(IComparable<T>.CompareTo), value2.ConvertTo(underlyingType))
				.Lambda<Comparison<T>>(value1, value2).Compile();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int IComparer<T>.Compare([AllowNull] T x, [AllowNull] T y)
			=> this._Compare(x, y);
	}
}
