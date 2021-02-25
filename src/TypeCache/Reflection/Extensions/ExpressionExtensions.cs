// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Expressions;

namespace TypeCache.Reflection.Extensions
{
	public static class ExpressionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression And(this Expression @this, Expression operand)
			=> Expression.AndAlso(@this, operand);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArrayExpressionBuilder Array(this Expression @this)
			=> new ArrayExpressionBuilder { Expression = @this };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As<T>(this Expression @this)
			=> Expression.TypeAs(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As(this Expression @this, Type type)
			=> Expression.TypeAs(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression Assign(this Expression @this, Expression expression)
			=> Expression.Assign(@this, expression);

		public static UnaryExpression Assign(this Expression @this, Unary operation)
			=> operation switch
			{
				Unary.PreIncrement => Expression.PreIncrementAssign(@this),
				Unary.PostIncrement => Expression.PostIncrementAssign(@this),
				Unary.PreDecrement => Expression.PreDecrementAssign(@this),
				Unary.PostDecrement => Expression.PostDecrementAssign(@this),
				_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(Unary)} [{operation}] is not supported.")
			};

		public static BinaryExpression Assign(this Expression @this, Expression operand, Arithmetic operation)
			=> operation switch
			{
				Arithmetic.Add => Expression.AddAssign(@this, operand),
				Arithmetic.AddChecked => Expression.AddAssignChecked(@this, operand),
				Arithmetic.Divide => Expression.DivideAssign(@this, operand),
				Arithmetic.Modulus => Expression.ModuloAssign(@this, operand),
				Arithmetic.Multiply => Expression.MultiplyAssign(@this, operand),
				Arithmetic.MultiplyChecked => Expression.MultiplyAssignChecked(@this, operand),
				Arithmetic.Power => Expression.PowerAssign(@this, operand),
				Arithmetic.Subtract => Expression.SubtractAssign(@this, operand),
				Arithmetic.SubtractChecked => Expression.SubtractAssignChecked(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(Arithmetic)} [{operation}] is not supported.")
			};

		public static BinaryExpression Assign(this Expression @this, Expression operand, Bitwise operation)
			=> operation switch
			{
				Bitwise.And => Expression.AndAssign(@this, operand),
				Bitwise.Or => Expression.OrAssign(@this, operand),
				Bitwise.ExclusiveOr => Expression.ExclusiveOrAssign(@this, operand),
				Bitwise.LeftShift => Expression.LeftShiftAssign(@this, operand),
				Bitwise.RightShift => Expression.RightShiftAssign(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(Bitwise)} [{operation}] is not supported.")
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, string method, params Expression[] arguments)
			=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, string method, Type[] genericTypes, params Expression[] arguments)
			=> Expression.Call(@this, method, genericTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo)
			=> Expression.Call(@this, methodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression> arguments)
			=> Expression.Call(@this, methodInfo, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, params Expression[] arguments)
			=> Expression.Call(@this, methodInfo, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this MethodInfo @this)
			=> Expression.Call(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this MethodInfo @this, IEnumerable<Expression> arguments)
			=> Expression.Call(@this, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this MethodInfo @this, params Expression[] arguments)
			=> Expression.Call(@this, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this Type @this, string method, params Expression[] arguments)
			=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this Type @this, string method, Type[] genericTypes, params Expression[] arguments)
			=> Expression.Call(@this, method, genericTypes, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Cast<T>(this Expression @this)
			=> @this.Cast(typeof(T));

		public static UnaryExpression Cast(this Expression @this, Type type)
		{
			if (type.IsByRef || type.IsPointer)
				type = type.GetElementType() ?? type;

			return type.IsValueType ? Expression.Unbox(@this, type) : Expression.TypeAs(@this, type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression Coalesce(this Expression @this, Expression expression)
			=> Expression.Coalesce(@this, expression);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnaryExpression Complement(this Expression @this)
			=> Expression.OnesComplement(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConstantExpression Constant<T>([NotNull] this T @this)
			=> Expression.Constant(@this, @this!.GetType());

		public static UnaryExpression ConvertTo<T>(this Expression @this, bool overflowCheck = false)
			=> overflowCheck ? Expression.ConvertChecked(@this, typeof(T)) : Expression.Convert(@this, typeof(T));

		public static UnaryExpression ConvertTo(this Expression @this, Type type, bool overflowCheck = false)
			=> overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnaryExpression Decrement(this Expression @this)
			=> Expression.Decrement(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DefaultExpression Default(this Type @this)
			=> Expression.Default(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Field(this Expression @this, string name)
			=> Expression.Field(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Field(this Expression @this, FieldInfo fieldInfo)
			=> Expression.Field(@this, fieldInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConditionalExpression If(this Expression @this, Expression trueResult)
			=> Expression.IfThen(@this, trueResult);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConditionalExpression If(this Expression @this, Expression trueResult, Expression falseResult)
			=> Expression.IfThenElse(@this, trueResult, falseResult);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConditionalExpression IIf(this Expression @this, Expression trueResult, Expression falseResult)
			=> Expression.Condition(@this, trueResult, falseResult);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnaryExpression Increment(this Expression @this)
			=> Expression.Increment(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InvocationExpression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
			=> Expression.Invoke(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InvocationExpression Invoke(this LambdaExpression @this, params Expression[] parameters)
			=> Expression.Invoke(@this, parameters);

		public static UnaryExpression Is(this Expression @this, bool value)
			=> value ? Expression.IsTrue(@this) : Expression.IsFalse(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression Is<T>(this Expression @this)
			=> Expression.TypeIs(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression Is(this Expression @this, Type type)
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

		public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray()), @this, parameters);

		public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray(parameters.Length)), @this, parameters);

		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray(parameters.Length + 1)), @this, parameters);

		public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

		public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray(parameters.Length + 1)), @this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Member(this Expression @this, string name)
			=> Expression.PropertyOrField(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInitExpression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
			=> Expression.MemberInit(@this, bindings);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInitExpression MemberInit(this NewExpression @this, params MemberBinding[] bindings)
			=> Expression.MemberInit(@this, bindings);

		public static UnaryExpression Negate(this Expression @this, bool overflowCheck = false)
			=> overflowCheck ? Expression.NegateChecked(@this) : Expression.Negate(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, params Expression[]? parameters)
			=> Expression.New(@this, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, params MemberInfo[]? memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this Type @this)
			=> Expression.New(@this);

		public static BinaryExpression Operation(this Expression @this, Arithmetic operation, Expression operand)
			=> operation switch
			{
				Arithmetic.Add => Expression.Add(@this, operand),
				Arithmetic.AddChecked => Expression.AddChecked(@this, operand),
				Arithmetic.Divide => Expression.Divide(@this, operand),
				Arithmetic.Modulus => Expression.Modulo(@this, operand),
				Arithmetic.Multiply => Expression.Multiply(@this, operand),
				Arithmetic.MultiplyChecked => Expression.MultiplyChecked(@this, operand),
				Arithmetic.Power => Expression.Power(@this, operand),
				Arithmetic.Subtract => Expression.Subtract(@this, operand),
				Arithmetic.SubtractChecked => Expression.SubtractChecked(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(Arithmetic)} [{operation}] is not supported.")
			};

		public static BinaryExpression Operation(this Expression @this, Bitwise operation, Expression operand)
			=> operation switch
			{
				Bitwise.And => Expression.And(@this, operand),
				Bitwise.Or => Expression.Or(@this, operand),
				Bitwise.ExclusiveOr => Expression.ExclusiveOr(@this, operand),
				Bitwise.LeftShift => Expression.LeftShift(@this, operand),
				Bitwise.RightShift => Expression.RightShift(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(Bitwise)} [{operation}] is not supported.")
			};

		public static BinaryExpression Operation(this Expression @this, Equality operation, Expression operand)
			=> operation switch
			{
				Equality.EqualTo => Expression.Equal(@this, operand),
				Equality.ReferenceEqualTo => Expression.ReferenceEqual(@this, operand),
				Equality.NotEqualTo => Expression.NotEqual(@this, operand),
				Equality.ReferenceNotEqualTo => Expression.ReferenceNotEqual(@this, operand),
				Equality.MoreThan => Expression.GreaterThan(@this, operand),
				Equality.MoreThanOrEqualTo => Expression.GreaterThanOrEqual(@this, operand),
				Equality.LessThan => Expression.LessThan(@this, operand),
				Equality.LessThanOrEqualTo => Expression.LessThanOrEqual(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(Equality)} [{operation}] is not supported.")
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression Or(this Expression @this, Expression operand)
			=> Expression.OrElse(@this, operand);

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
		public static MemberExpression Property(this Expression @this, string name)
			=> Expression.Property(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, Type type, string name)
			=> Expression.Property(@this, type, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property<T>(this Expression @this, string name)
			=> Expression.Property(@this, typeof(T), name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, PropertyInfo propertyInfo)
			=> Expression.Property(@this, propertyInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexExpression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index)
			=> Expression.Property(@this, propertyInfo, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, MethodInfo getMethodInfo)
			=> Expression.Property(@this, getMethodInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticField(this FieldInfo @this)
			=> Expression.Field(null, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticField(this Type @this, string name)
			=> Expression.Field(null, @this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticProperty(this PropertyInfo @this)
			=> Expression.Property(null, @this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexExpression StaticProperty(this PropertyInfo @this, ParameterExpression index)
			=> Expression.Property(null, @this, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticProperty(this Type @this, string name)
			=> Expression.Property(null, @this, name);

		public static Expression SystemConvert(this Expression @this, Type type)
			=> type.GetSystemType() switch
			{
				_ when @this.Type == type => @this,
				SystemType.Boolean => typeof(Convert).CallStatic(nameof(Convert.ToBoolean), Type.EmptyTypes, @this),
				SystemType.Char => typeof(Convert).CallStatic(nameof(Convert.ToChar), Type.EmptyTypes, @this),
				SystemType.SByte => typeof(Convert).CallStatic(nameof(Convert.ToSByte), Type.EmptyTypes, @this),
				SystemType.Byte => typeof(Convert).CallStatic(nameof(Convert.ToByte), Type.EmptyTypes, @this),
				SystemType.Int16 => typeof(Convert).CallStatic(nameof(Convert.ToInt16), Type.EmptyTypes, @this),
				SystemType.UInt16 => typeof(Convert).CallStatic(nameof(Convert.ToUInt16), Type.EmptyTypes, @this),
				SystemType.Int32 => typeof(Convert).CallStatic(nameof(Convert.ToInt32), Type.EmptyTypes, @this),
				SystemType.UInt32 => typeof(Convert).CallStatic(nameof(Convert.ToUInt32), Type.EmptyTypes, @this),
				SystemType.Int64 => typeof(Convert).CallStatic(nameof(Convert.ToInt64), Type.EmptyTypes, @this),
				SystemType.UInt64 => typeof(Convert).CallStatic(nameof(Convert.ToUInt64), Type.EmptyTypes, @this),
				SystemType.Single => typeof(Convert).CallStatic(nameof(Convert.ToSingle), Type.EmptyTypes, @this),
				SystemType.Double => typeof(Convert).CallStatic(nameof(Convert.ToDouble), Type.EmptyTypes, @this),
				SystemType.Decimal => typeof(Convert).CallStatic(nameof(Convert.ToDecimal), Type.EmptyTypes, @this),
				SystemType.DateTime => typeof(Convert).CallStatic(nameof(Convert.ToDateTime), Type.EmptyTypes, @this),
				SystemType.String => typeof(Convert).CallStatic(nameof(Convert.ToString), Type.EmptyTypes, @this),
				_ => @this.Cast(type)
			};

		public static IEnumerable<Expression> ToParameterArray(this ParameterExpression @this, params ParameterInfo[] parameterInfos)
			=> parameterInfos.To(parameterInfo => @this.Array()[parameterInfo.Position].SystemConvert(parameterInfo.ParameterType));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression TypeEqual<T>(this Expression @this)
			=> Expression.TypeEqual(@this, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression TypeEqual(this Expression @this, Type type)
			=> Expression.TypeEqual(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Unbox<T>(this Expression @this) where T : struct
			=> @this.Unbox(typeof(T));

		public static Expression Unbox(this Expression @this, Type type)
			=> type.IsValueType ? Expression.Unbox(@this, type) : @this;

		#region LabelTarget
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this)
			=> Expression.Break(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this, Expression? value)
			=> Expression.Break(@this, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this, Type type)
			=> Expression.Break(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this, Expression? value, Type type)
			=> Expression.Break(@this, value, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Continue(this LabelTarget @this)
			=> Expression.Continue(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Continue(this LabelTarget @this, Type type)
			=> Expression.Continue(@this, type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Goto(this LabelTarget @this)
			=> Expression.Goto(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelTarget Label(this Type @this)
			=> Expression.Label(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelTarget Label(this Type @this, string? name)
			=> Expression.Label(@this, name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelExpression Label(this LabelTarget @this)
			=> Expression.Label(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelExpression Label(this LabelTarget @this, Expression? defaultValue)
			=> Expression.Label(@this, defaultValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelTarget Label(this string? @this)
			=> Expression.Label(@this);
		#endregion
	}
}
