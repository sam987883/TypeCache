// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace sam987883.Extensions
{
	public static class ExpressionExtensions
	{
		public static Expression Add(this Expression @this, Expression expression, bool overflowCheck = false) =>
			overflowCheck ? Expression.AddChecked(@this, expression) : Expression.Add(@this, expression);

		public static Expression AddAssign(this Expression @this, Expression expression, bool overflowCheck = false) =>
			overflowCheck ? Expression.AddAssignChecked(@this, expression) : Expression.AddAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression And(this Expression @this, Expression expression) =>
			Expression.And(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression AndAlso(this Expression @this, Expression expression) =>
			Expression.AndAlso(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression AndAssign(this Expression @this, Expression expression) =>
			Expression.AndAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, int index) =>
			Expression.ArrayAccess(@this, Expression.Constant(index, index.GetType()));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, params int[] indexes) =>
			Expression.ArrayAccess(@this, indexes.To(index => (Expression)Expression.Constant(index, index.GetType())).ToArray());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, long index) =>
			Expression.ArrayAccess(@this, Expression.Constant(index, index.GetType()));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, params long[] indexes) =>
			Expression.ArrayAccess(@this, indexes.To(i => (Expression)Expression.Constant(i, i.GetType())).ToArray());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayIndex(this Expression @this, Expression index) =>
			Expression.ArrayIndex(@this, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayIndex(this Expression @this, IEnumerable<Expression> indexes) =>
			Expression.ArrayIndex(@this, indexes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayIndex(this Expression @this, params Expression[] indexes) =>
			Expression.ArrayIndex(@this, indexes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayLength(this Expression @this) =>
			Expression.ArrayLength(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As<T>(this Expression @this) =>
			Expression.TypeAs(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As(this Expression @this, Type type) =>
			Expression.TypeAs(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Assign(this Expression @this, Expression expression) =>
			Expression.Assign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, MethodInfo methodInfo) =>
			Expression.Call(@this, methodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, MethodInfo methodInfo, Expression argument1) =>
			Expression.Call(@this, methodInfo, argument1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, MethodInfo methodInfo, Expression argument1, Expression argument2) =>
			Expression.Call(@this, methodInfo, argument1, argument2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression> arguments) =>
			Expression.Call(@this, methodInfo, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, MethodInfo methodInfo, params Expression[] arguments) =>
			Expression.Call(@this, methodInfo, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, string method, params Expression[] arguments) =>
			Expression.Call(@this, method, Type.EmptyTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, string method, Type[] genericTypes, params Expression[] arguments) =>
			Expression.Call(@this, method, genericTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Cast<T>(this Expression @this) =>
			@this.Cast(typeof(T));

		public static Expression Cast(this Expression @this, Type type)
		{
			if (type.IsByRef || type.IsPointer)
				type = type.GetElementType();

			return type.IsValueType ? Expression.Unbox(@this, type) : Expression.TypeAs(@this, type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Coalesce(this Expression @this, Expression expression) =>
			Expression.Coalesce(@this, expression);

		public static Expression ConvertTo<T>(this Expression @this, bool overflowCheck = false) =>
			overflowCheck ? Expression.ConvertChecked(@this, typeof(T)) : Expression.Convert(@this, typeof(T));

		public static Expression ConvertTo(this Expression @this, Type type, bool overflowCheck = false) =>
			overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression EqualTo(this Expression @this, Expression expression) =>
			Expression.Equal(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression EqualTo<T>(this Expression @this) =>
			Expression.TypeEqual(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression EqualTo(this Expression @this, Type type) =>
			Expression.TypeEqual(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Decrement(this Expression @this) =>
			Expression.Decrement(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Decrement(this Expression @this, MethodInfo methodInfo) =>
			Expression.Decrement(@this, methodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Divide(this Expression @this, Expression expression) =>
			Expression.Divide(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression DivideAssign(this Expression @this, Expression expression) =>
			Expression.DivideAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Field(this Expression @this, FieldInfo fieldInfo) =>
			Expression.Field(@this, fieldInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression GreaterThan(this Expression @this, Expression expression) =>
			Expression.GreaterThan(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression GreaterThanOrEqual(this Expression @this, Expression expression) =>
			Expression.GreaterThanOrEqual(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression If(this Expression @this, Expression trueResult, Expression falseResult) =>
			Expression.Condition(@this, trueResult, falseResult);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Increment(this Expression @this) =>
			Expression.Increment(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Increment(this Expression @this, MethodInfo methodInfo) =>
			Expression.Increment(@this, methodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Is<T>(this Expression @this) =>
			Expression.TypeIs(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Is(this Expression @this, Type type) =>
			Expression.TypeIs(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters) =>
			Expression.Lambda(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression Lambda(this Expression @this, params ParameterExpression[] parameters) =>
			Expression.Lambda(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters) =>
			Expression.Lambda<T>(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<T> Lambda<T>(this Expression @this, params ParameterExpression[] parameters) =>
			Expression.Lambda<T>(@this, parameters);

		public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters) =>
			Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray()), @this, parameters);

		public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters) =>
			Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray()), @this, parameters);

		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters) =>
			Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[] parameters) =>
			Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

		public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters) =>
			Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

		public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[] parameters) =>
			Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression LessThan(this Expression @this, Expression expression) =>
			Expression.LessThan(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression LessThanOrEqual(this Expression @this, Expression expression) =>
			Expression.LessThanOrEqual(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Member(this Expression @this, string name) =>
			Expression.PropertyOrField(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings) =>
			Expression.MemberInit(@this, bindings);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression MemberInit(this NewExpression @this, params MemberBinding[] bindings) =>
			Expression.MemberInit(@this, bindings);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Modulo(this Expression @this, Expression expression) =>
			Expression.Modulo(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ModuloAssign(this Expression @this, Expression expression) =>
			Expression.ModuloAssign(@this, expression);

		public static Expression Multiply(this Expression @this, Expression expression, bool overflowCheck = false) => overflowCheck
			? Expression.MultiplyChecked(@this, expression)
			: Expression.Multiply(@this, expression);

		public static Expression MultiplyAssign(this Expression @this, Expression expression, bool overflowCheck = false) => overflowCheck
			? Expression.MultiplyAssignChecked(@this, expression)
			: Expression.MultiplyAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression New(this ConstructorInfo @this, params ParameterExpression[] parameters) =>
			Expression.New(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression New(this Type @this) =>
			Expression.New(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter(this ParameterInfo @this) =>
			Expression.Parameter(@this.ParameterType, @this.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter(this string @this, Type type) =>
			Expression.Parameter(type, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter<T>(this string @this) =>
			Expression.Parameter(typeof(T), @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Property(this Expression @this, PropertyInfo propertyInfo) =>
			Expression.Property(@this, propertyInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index) =>
			Expression.Property(@this, propertyInfo, index);

		public static Expression Subtract(this Expression @this, Expression expression, bool overflowCheck = false) => overflowCheck
			? Expression.SubtractChecked(@this, expression)
			: Expression.Subtract(@this, expression);

		public static Expression SubtractAssign(this Expression @this, Expression expression, bool overflowCheck = false) => overflowCheck
			? Expression.SubtractAssignChecked(@this, expression)
			: Expression.SubtractAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Or(this Expression @this, Expression expression) =>
			Expression.Or(@this, expression);

		public static IEnumerable<Expression> ToParameterArray(this ParameterExpression @this, params ParameterInfo[] parameterInfos) =>
			parameterInfos.To(parameterInfo => @this.ArrayAccess(parameterInfo.Position).Cast(parameterInfo.ParameterType));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Unbox<T>(this Expression @this) where T : struct =>
			@this.Unbox(typeof(T));

		public static Expression Unbox(this Expression @this, Type type) =>
			type.IsValueType ? Expression.Unbox(@this, type) : @this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression XOr(this Expression @this, Expression expression) =>
			Expression.ExclusiveOr(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression XOrAssign(this Expression @this, Expression expression) =>
			Expression.ExclusiveOrAssign(@this, expression);
	}
}
