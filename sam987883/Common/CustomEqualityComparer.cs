// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static sam987883.Extensions.ExpressionExtensions;
using static sam987883.Extensions.ReflectionExtensions;

namespace sam987883.Common
{
	internal sealed class CustomEqualityComparer<T> : IEqualityComparer<T>
	{
		public CustomEqualityComparer()
		{
			var type = typeof(T);
			if (type.IsEnum)
			{
				var underlyingType = type.GetEnumUnderlyingType();

				ParameterExpression value1 = nameof(value1).Parameter<T>();
				ParameterExpression value2 = nameof(value2).Parameter<T>();
				this.Equals = value1.ConvertTo(underlyingType).EqualTo(value2.ConvertTo(underlyingType)).Lambda<Func<T, T, bool>>(value1, value2).Compile();

				ParameterExpression value = nameof(value).Parameter<T>();
				this.GetHashCode = value.ConvertTo(underlyingType).Call(nameof(object.GetHashCode)).Lambda<Func<T, int>>(value).Compile();
			}
			else if (type.Implements(typeof(IEquatable<T>)))
			{
				this.Equals = (x, y) => (x == null && y == null) || (x != null ? ((IEquatable<T>)x).Equals(y) : ((IEquatable<T>)y).Equals(x));
				this.GetHashCode = value => value?.GetHashCode() ?? 0;
			}
			else
			{
				this.Equals = (x, y) => (x == null && y == null) || (x != null ? x.Equals(y) : y.Equals(x));
				this.GetHashCode = value => value?.GetHashCode() ?? 0;
			}
		}

		public CustomEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
		{
			this.Equals = equals;
			this.GetHashCode = getHashCode;
		}

		public CustomEqualityComparer(Func<T, T, bool> equals) : this(equals, value => value?.GetHashCode() ?? 0)
		{
		}

		public new Func<T, T, bool> Equals { get; } 

		public new Func<T, int> GetHashCode { get; }

		bool IEqualityComparer<T>.Equals(T x, T y) =>
			this.Equals(x, y);

		int IEqualityComparer<T>.GetHashCode(T value) =>
			this.GetHashCode(value);
	}
}
