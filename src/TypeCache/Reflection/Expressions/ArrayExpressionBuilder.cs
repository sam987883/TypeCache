// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Linq.Expressions;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Expressions
{
	public record ArrayExpressionBuilder(Expression Expression)
	{
		/// <summary>
		/// <c><see cref="Expression.ArrayIndex(Expression, Expression)"/></c>
		/// </summary>
		public Expression this[int index] => Expression.ArrayAccess(this.Expression, index.Constant());

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public Expression this[params int[] indexes] => Expression.ArrayAccess(this.Expression, indexes.ToArray(index => (Expression)index.Constant()));

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public Expression this[long index] => Expression.ArrayAccess(this.Expression, index.Constant());

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public Expression this[params long[] indexes] => Expression.ArrayAccess(this.Expression, indexes.ToArray(index => (Expression)index.Constant()));

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public Expression this[Expression index] => Expression.ArrayAccess(this.Expression, index);

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, IEnumerable{Expression})"/></c>
		/// </summary>
		public Expression this[IEnumerable<Expression> indexes] =>  Expression.ArrayAccess(this.Expression, indexes);

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public Expression this[params Expression[] indexes] => Expression.ArrayAccess(this.Expression, indexes);

		/// <summary>
		/// <c><see cref="Expression.ArrayLength(Expression)"/></c>
		/// </summary>
		public Expression Length => Expression.ArrayLength(this.Expression);

		/// <summary>
		/// <c><see cref="Expression.ArrayIndex(Expression, Expression)"/></c>
		/// </summary>
		public Expression Get(int index)
			=> Expression.ArrayIndex(this.Expression, index.Constant());

		/// <summary>
		/// <c><see cref="Expression.ArrayIndex(Expression, Expression[])"/></c>
		/// </summary>
		public Expression Get(params int[] indexes)
			=> Expression.ArrayIndex(this.Expression, indexes.ToArray(index => (Expression)index.Constant()));
	}
}
