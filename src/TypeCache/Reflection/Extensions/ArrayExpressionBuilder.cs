// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Linq.Expressions;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public record ArrayExpressionBuilder(Expression Expression)
	{
		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public IndexExpression this[int index]
			=> Expression.ArrayAccess(this.Expression, Expression.Constant(index));

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public IndexExpression this[params int[] indexes]
			=> Expression.ArrayAccess(this.Expression, indexes.ToArray(index => (Expression)Expression.Constant(index)));

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public IndexExpression this[long index]
			=> Expression.ArrayAccess(this.Expression, Expression.Constant(index));

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public IndexExpression this[params long[] indexes]
			=> Expression.ArrayAccess(this.Expression, indexes.ToArray(index => (Expression)Expression.Constant(index)));

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public IndexExpression this[Expression index]
			=> Expression.ArrayAccess(this.Expression, index);

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, IEnumerable{Expression})"/></c>
		/// </summary>
		public IndexExpression this[IEnumerable<Expression> indexes]
			=> Expression.ArrayAccess(this.Expression, indexes);

		/// <summary>
		/// <c><see cref="Expression.ArrayAccess(Expression, Expression[])"/></c>
		/// </summary>
		public IndexExpression this[params Expression[] indexes]
			=> Expression.ArrayAccess(this.Expression, indexes);

		/// <summary>
		/// <c><see cref="Expression.ArrayLength(Expression)"/></c>
		/// </summary>
		public UnaryExpression Length => Expression.ArrayLength(this.Expression);
	}
}
