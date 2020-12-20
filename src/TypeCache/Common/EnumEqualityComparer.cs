// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Common
{
	public sealed class EnumEqualityComparer<T> : IEnumEqualityComparer<T>
		where T : struct, Enum
	{
		private readonly Func<T, T, bool> _Equals;

		private readonly Func<T, int> _GetHashCode;

		public EnumEqualityComparer()
		{
			var underlyingType = typeof(T).GetEnumUnderlyingType();

			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			this._Equals = value1.ConvertTo(underlyingType).EqualTo(value2.ConvertTo(underlyingType)).Lambda<Func<T, T, bool>>(value1, value2).Compile();

			ParameterExpression value = nameof(value).Parameter<T>();
			this._GetHashCode = value.ConvertTo(underlyingType).Call(nameof(object.GetHashCode)).Lambda<Func<T, int>>(value).Compile();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals([AllowNull] T x, [AllowNull] T y)
			=> this._Equals(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode([DisallowNull] T value)
			=> this._GetHashCode(value);
	}
}
