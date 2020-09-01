// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static System.Linq.Expressions.Expression;
using static Sam987883.Common.Extensions.ExpressionExtensions;
using static Sam987883.Common.Extensions.ReflectionExtensions;

namespace Sam987883.Common
{
	public sealed class CustomComparer<T> : IComparer<T>
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
					.Lambda<CompareFunc<T>>(value1, value2).Compile();
			}
			else if (type.Implements(typeof(IComparable<T>)))
				this.Compare = (x, y) => x != null ? ((IComparable<T>)x).CompareTo(y) : (y != null ? ((IComparable<T>)y).CompareTo(x) : 0);
			else if (type.Implements(typeof(IComparable)))
				this.Compare = (x, y) => x != null ? ((IComparable)x).CompareTo(y) : (y != null ? ((IComparable)y).CompareTo(x) : 0);
			else
				throw new NotImplementedException($"'{type.FullName}' does not have a default comparison implementation.");
		}

		public CustomComparer(CompareFunc<T> compare) =>
			this.Compare = compare;

		public CompareFunc<T> Compare { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int IComparer<T>.Compare([AllowNull] T x, [AllowNull] T y) =>
			this.Compare(x, y);
	}
}
