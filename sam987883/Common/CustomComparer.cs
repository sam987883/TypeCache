// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static sam987883.Extensions.ExpressionExtensions;
using static sam987883.Extensions.ReflectionExtensions;

namespace sam987883.Common
{
	internal sealed class CustomComparer<T> : IComparer<T>
	{
		public CustomComparer()
		{
			var type = typeof(T);
			if (type.IsEnum)
			{
				var underlyingType = type.GetEnumUnderlyingType();

				ParameterExpression value1 = nameof(value1).Parameter<T>();
				ParameterExpression value2 = nameof(value2).Parameter<T>();
				this.Compare = value1.ConvertTo(underlyingType).GreaterThan(value2.ConvertTo(underlyingType))
					.If(Constant(1), value1.ConvertTo(underlyingType).LessThan(value2.ConvertTo(underlyingType))
					.If(Constant(-1), Constant(0)))
					.Lambda<Func<T, T, int>>(value1, value2).Compile();
			}
			else if (type.Implements(typeof(IComparable<T>)))
				this.Compare = (x, y) =>
				{
					if (x != null)
						return ((IComparable<T>)x).CompareTo(y);
					if (y != null)
						return ((IComparable<T>)y).CompareTo(x);
					return 0;
				};
			else if (type.Implements(typeof(IComparable)))
				this.Compare = (x, y) =>
				{
					if (x != null)
						return ((IComparable)x).CompareTo(y);
					if (y != null)
						return ((IComparable)y).CompareTo(x);
					return 0;
				};
			else
				throw new NotImplementedException($"'{type.FullName}' does not have a default comparison implementation.");
		}

		public CustomComparer(Func<T, T, int> compare) =>
			this.Compare = compare;

		public Func<T, T, int> Compare { get; } 

		int IComparer<T>.Compare([AllowNull] T x, [AllowNull] T y) =>
			this.Compare(x, y);
	}
}
