// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static sam987883.Extensions.ExpressionExtensions;
using static sam987883.Extensions.ReflectionExtensions;

namespace sam987883.Common
{
	internal sealed class CustomEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> _Equals;

		private readonly Func<T, int> _GetHashCode;

		public CustomEqualityComparer(Func<T, T, bool> equals) : this(equals, value => value?.GetHashCode() ?? 0)
		{
		}

		public CustomEqualityComparer()
		{
			var type = typeof(T);
			if (type.IsEnum)
			{
				var underlyingType = type.GetEnumUnderlyingType();

				ParameterExpression value1 = nameof(value1).Parameter<T>();
				ParameterExpression value2 = nameof(value2).Parameter<T>();
				this._Equals = value1.ConvertTo(underlyingType).EqualTo(value2.ConvertTo(underlyingType)).Lambda<Func<T, T, bool>>(value1, value2).Compile();

				ParameterExpression value = nameof(value).Parameter<T>();
				this._GetHashCode = value.ConvertTo(underlyingType).Call(nameof(object.GetHashCode)).Lambda<Func<T, int>>(value).Compile();
			}
			else if (type.Implements(typeof(IEquatable<T>)))
			{
				this._Equals = (x, y) =>
				{
					if (x != null)
						return ((IEquatable<T>)x).Equals(y);
					if (y != null)
						return ((IEquatable<T>)y).Equals(x);
					return true;
				};
				this._GetHashCode = value => value?.GetHashCode() ?? 0;
			}
			else
			{
				this._Equals = (x, y) => (x == null && y == null) || (x != null ? x.Equals(y) : y.Equals(x));
				this._GetHashCode = value => value?.GetHashCode() ?? 0;
			}
		}

		public CustomEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
		{
			this._Equals = equals;
			this._GetHashCode = getHashCode;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals([AllowNull] T x, [AllowNull] T y) =>
			this._Equals(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode([DisallowNull] T value) =>
			this._GetHashCode(value);
	}
}
