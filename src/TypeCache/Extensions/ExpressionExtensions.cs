// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Common;

namespace TypeCache.Extensions
{
	public static class ExpressionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Add(this Expression @this, Expression expression, bool overflowCheck = false)
			=> overflowCheck ? Expression.AddChecked(@this, expression) : Expression.Add(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression AddAssign(this Expression @this, Expression expression, bool overflowCheck = false)
			=> overflowCheck ? Expression.AddAssignChecked(@this, expression) : Expression.AddAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression And(this Expression @this, Expression expression)
			=> Expression.And(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression AndAlso(this Expression @this, Expression expression)
			=> Expression.AndAlso(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression AndAssign(this Expression @this, Expression expression)
			=> Expression.AndAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, int index)
			=> Expression.ArrayAccess(@this, Expression.Constant(index, index.GetType()));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, params int[] indexes)
			=> Expression.ArrayAccess(@this, indexes.To(index => (Expression)Expression.Constant(index, index.GetType())).ToArrayOf(indexes.Length));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, long index)
			=> Expression.ArrayAccess(@this, Expression.Constant(index, index.GetType()));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayAccess(this Expression @this, params long[] indexes)
			=> Expression.ArrayAccess(@this, indexes.To(i => (Expression)Expression.Constant(i, i.GetType())).ToArrayOf(indexes.Length));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayIndex(this Expression @this, Expression index)
			=> Expression.ArrayIndex(@this, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayIndex(this Expression @this, IEnumerable<Expression> indexes)
			=> Expression.ArrayIndex(@this, indexes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayIndex(this Expression @this, params Expression[] indexes)
			=> Expression.ArrayIndex(@this, indexes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ArrayLength(this Expression @this)
			=> Expression.ArrayLength(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As<T>(this Expression @this)
			=> Expression.TypeAs(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As(this Expression @this, Type type)
			=> Expression.TypeAs(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Assign(this Expression @this, Expression expression)
			=> Expression.Assign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, string method, params Expression[] arguments)
			=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this Expression @this, string method, Type[] genericTypes, params Expression[] arguments)
			=> Expression.Call(@this, method, genericTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this MethodInfo @this, Expression instance)
			=> Expression.Call(instance, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this MethodInfo @this, Expression instance, IEnumerable<Expression> arguments)
			=> Expression.Call(instance, @this, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Call(this MethodInfo @this, Expression instance, params Expression[] arguments)
			=> Expression.Call(instance, @this, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression CallStatic(this MethodInfo @this)
			=> Expression.Call(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression CallStatic(this MethodInfo @this, IEnumerable<Expression> arguments)
			=> Expression.Call(@this, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression CallStatic(this MethodInfo @this, params Expression[] arguments)
			=> Expression.Call(@this, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression CallStatic(this Type @this, string method, params Expression[] arguments)
			=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression CallStatic(this Type @this, string method, Type[] genericTypes, params Expression[] arguments)
			=> Expression.Call(@this, method, genericTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Cast<T>(this Expression @this)
			=> @this.Cast(typeof(T));

		public static Expression Cast(this Expression @this, Type type)
		{
			if (type.IsByRef || type.IsPointer)
				type = type.GetElementType() ?? type;

			return type.IsValueType ? Expression.Unbox(@this, type) : Expression.TypeAs(@this, type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Coalesce(this Expression @this, Expression expression)
			=> Expression.Coalesce(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Constant<T>([NotNull] this T @this)
			=> Expression.Constant(@this, @this.GetType());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ConvertTo<T>(this Expression @this, bool overflowCheck = false)
			=> overflowCheck ? Expression.ConvertChecked(@this, typeof(T)) : Expression.Convert(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ConvertTo(this Expression @this, Type type, bool overflowCheck = false)
			=> overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Decrement(this Expression @this)
			=> Expression.Decrement(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Decrement(this Expression @this, MethodInfo methodInfo)
			=> Expression.Decrement(@this, methodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Divide(this Expression @this, Expression expression)
			=> Expression.Divide(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression DivideAssign(this Expression @this, Expression expression)
			=> Expression.DivideAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression EqualTo(this Expression @this, Expression expression)
			=> Expression.Equal(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression EqualTo<T>(this Expression @this)
			=> Expression.TypeEqual(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression EqualTo(this Expression @this, Type type)
			=> Expression.TypeEqual(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Field(this Expression @this, string name)
			=> Expression.Field(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Field(this Expression @this, FieldInfo fieldInfo)
			=> Expression.Field(@this, fieldInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression GreaterThan(this Expression @this, Expression expression)
			=> Expression.GreaterThan(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression GreaterThanOrEqual(this Expression @this, Expression expression)
			=> Expression.GreaterThanOrEqual(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression If(this Expression @this, Expression trueResult, Expression falseResult)
			=> Expression.Condition(@this, trueResult, falseResult);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Increment(this Expression @this)
			=> Expression.Increment(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Increment(this Expression @this, MethodInfo methodInfo)
			=> Expression.Increment(@this, methodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
			=> Expression.Invoke(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Invoke(this LambdaExpression @this, params Expression[] parameters)
			=> Expression.Invoke(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Is<T>(this Expression @this)
			=> Expression.TypeIs(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Is(this Expression @this, Type type)
			=> Expression.TypeIs(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression Lambda(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda<T>(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<T> Lambda<T>(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda<T>(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray()), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArrayOf(parameters.Length)), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArrayOf(parameters.Length)), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArrayOf(parameters.Length)), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression LessThan(this Expression @this, Expression expression)
			=> Expression.LessThan(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression LessThanOrEqual(this Expression @this, Expression expression)
			=> Expression.LessThanOrEqual(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Member(this Expression @this, string name)
			=> Expression.PropertyOrField(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
			=> Expression.MemberInit(@this, bindings);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression MemberInit(this NewExpression @this, params MemberBinding[] bindings)
			=> Expression.MemberInit(@this, bindings);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Modulo(this Expression @this, Expression expression)
			=> Expression.Modulo(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression ModuloAssign(this Expression @this, Expression expression)
			=> Expression.ModuloAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Multiply(this Expression @this, Expression expression, bool overflowCheck = false)
			=> overflowCheck ? Expression.MultiplyChecked(@this, expression) : Expression.Multiply(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression MultiplyAssign(this Expression @this, Expression expression, bool overflowCheck = false)
			=> overflowCheck ? Expression.MultiplyAssignChecked(@this, expression) : Expression.MultiplyAssign(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression New(this ConstructorInfo @this, params Expression[]? parameters)
			=> Expression.New(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, params MemberInfo[]? memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression New(this Type @this)
			=> Expression.New(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Or(this Expression @this, Expression expression)
			=> Expression.Or(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter(this ParameterInfo @this)
			=> Expression.Parameter(@this.ParameterType, @this.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter(this string @this, Type type)
			=> Expression.Parameter(type, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter<T>(this string @this)
			=> Expression.Parameter(typeof(T), @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Property(this Expression @this, PropertyInfo propertyInfo)
			=> Expression.Property(@this, propertyInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index)
			=> Expression.Property(@this, propertyInfo, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression StaticField(this FieldInfo @this)
			=> Expression.Field(null, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression StaticProperty(this PropertyInfo @this)
			=> Expression.Property(null, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression StaticProperty(this PropertyInfo @this, ParameterExpression index)
			=> Expression.Property(null, @this, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Subtract(this Expression @this, Expression expression, bool overflowCheck = false)
			=> overflowCheck ? Expression.SubtractChecked(@this, expression) : Expression.Subtract(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression SubtractAssign(this Expression @this, Expression expression, bool overflowCheck = false)
			=> overflowCheck ? Expression.SubtractAssignChecked(@this, expression) : Expression.SubtractAssign(@this, expression);

		public static Expression SystemConvert(this Expression @this, Type type)
		{
			if (@this.Type == type || (@this.Type.IsGenericType && @this.Type == typeof(Nullable<>).MakeGenericType(type)))
				return @this;

			var name = type.ToNativeType() switch
			{
				NativeType.Boolean => nameof(Convert.ToBoolean),
				NativeType.Char => nameof(Convert.ToChar),
				NativeType.SByte => nameof(Convert.ToSByte),
				NativeType.Byte => nameof(Convert.ToByte),
				NativeType.Int16 => nameof(Convert.ToInt16),
				NativeType.UInt16 => nameof(Convert.ToUInt16),
				NativeType.Int32 => nameof(Convert.ToInt32),
				NativeType.UInt32 => nameof(Convert.ToUInt32),
				NativeType.Int64 => nameof(Convert.ToInt64),
				NativeType.UInt64 => nameof(Convert.ToUInt64),
				NativeType.Single => nameof(Convert.ToSingle),
				NativeType.Double => nameof(Convert.ToDouble),
				NativeType.Decimal => nameof(Convert.ToDecimal),
				NativeType.DateTime => nameof(Convert.ToDateTime),
				NativeType.String => nameof(Convert.ToString),
				_ => null
			};

			return name != null
				? typeof(Convert).CallStatic(name, Type.EmptyTypes, @this)
				: @this.Cast(type);
		}

		public static IEnumerable<Expression> ToParameterArray(this ParameterExpression @this, params ParameterInfo[] parameterInfos)
			=> parameterInfos.To(parameterInfo => @this.ArrayAccess(parameterInfo.Position).SystemConvert(parameterInfo.ParameterType));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Unbox<T>(this Expression @this) where T : struct
			=> @this.Unbox(typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Unbox(this Expression @this, Type type)
			=> type.IsValueType ? Expression.Unbox(@this, type) : @this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression XOr(this Expression @this, Expression expression)
			=> Expression.ExclusiveOr(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression XOrAssign(this Expression @this, Expression expression)
			=> Expression.ExclusiveOrAssign(@this, expression);
	}
}
