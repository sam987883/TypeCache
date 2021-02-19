// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Expressions
{
	public readonly struct ArrayExpressionBuilder
	{
		public Expression Expression { get; init; }

		public Expression this[int index] => Expression.ArrayAccess(this.Expression, index.Constant());

		public Expression this[params int[] indexes] => Expression.ArrayAccess(this.Expression, indexes.To(index => (Expression)index.Constant()).ToArray(indexes.Length));

		public Expression this[long index] => Expression.ArrayAccess(this.Expression, index.Constant());

		public Expression this[params long[] indexes] => Expression.ArrayAccess(this.Expression, indexes.To(index => (Expression)index.Constant()).ToArray(indexes.Length));

		public Expression this[Expression index] => Expression.ArrayAccess(this.Expression, index);

		public Expression this[IEnumerable<Expression> indexes] =>  Expression.ArrayAccess(this.Expression, indexes);

		public Expression this[params Expression[] indexes] => Expression.ArrayAccess(this.Expression, indexes);

		public Expression Length => Expression.ArrayLength(this.Expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Expression Get(int index)
			=> Expression.ArrayIndex(this.Expression, index.Constant());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Expression Get(params int[] indexes)
			=> Expression.ArrayIndex(this.Expression, indexes.To(index => (Expression)index.Constant()).ToArray(indexes.Length));
	}
}
