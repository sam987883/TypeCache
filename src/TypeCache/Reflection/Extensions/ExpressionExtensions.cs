// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
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
		/// <summary>
		/// <c>new <see cref="ArrayExpressionBuilder"/>(@<paramref name="this"/>)</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArrayExpressionBuilder Array(this Expression @this)
			=> new ArrayExpressionBuilder(@this);

		/// <summary>
		/// <c><see cref="Expression.TypeAs(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As<T>(this Expression @this)
			where T : class
			=> Expression.TypeAs(@this, typeof(T));

		/// <summary>
		/// <c><see cref="Expression.TypeAs(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression As(this Expression @this, Type type)
			=> Expression.TypeAs(@this, type);

		/// <summary>
		/// <c><see cref="Expression.Assign(Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression Assign(this Expression @this, Expression expression)
			=> Expression.Assign(@this, expression);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.AddAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.AddAssignChecked(Expression, Expression)"/></item>
		/// <item><see cref="Expression.DivideAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.ModuloAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.MultiplyAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.MultiplyAssignChecked(Expression, Expression)"/></item>
		/// <item><see cref="Expression.PowerAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.SubtractAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.SubtractAssignChecked(Expression, Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static BinaryExpression Assign(this Expression @this, ArithmeticOp operation, Expression operand)
			=> operation switch
			{
				ArithmeticOp.Add => Expression.AddAssign(@this, operand),
				ArithmeticOp.AddChecked => Expression.AddAssignChecked(@this, operand),
				ArithmeticOp.Divide => Expression.DivideAssign(@this, operand),
				ArithmeticOp.Modulus => Expression.ModuloAssign(@this, operand),
				ArithmeticOp.Multiply => Expression.MultiplyAssign(@this, operand),
				ArithmeticOp.MultiplyChecked => Expression.MultiplyAssignChecked(@this, operand),
				ArithmeticOp.Power => Expression.PowerAssign(@this, operand),
				ArithmeticOp.Subtract => Expression.SubtractAssign(@this, operand),
				ArithmeticOp.SubtractChecked => Expression.SubtractAssignChecked(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(ArithmeticOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.AndAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.OrAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.ExclusiveOrAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.LeftShiftAssign(Expression, Expression)"/></item>
		/// <item><see cref="Expression.RightShiftAssign(Expression, Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static BinaryExpression Assign(this Expression @this, BitwiseOp operation, Expression operand)
			=> operation switch
			{
				BitwiseOp.And => Expression.AndAssign(@this, operand),
				BitwiseOp.Or => Expression.OrAssign(@this, operand),
				BitwiseOp.ExclusiveOr => Expression.ExclusiveOrAssign(@this, operand),
				BitwiseOp.LeftShift => Expression.LeftShiftAssign(@this, operand),
				BitwiseOp.RightShift => Expression.RightShiftAssign(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(BitwiseOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <c><see cref="Expression.Block(Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BlockExpression Block(this Expression @this, Expression expression)
			=> Expression.Block(@this, expression);

		/// <summary>
		/// <c><see cref="Expression.Call(Expression, string, Type[], Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, string method, params Expression[] arguments)
			=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(Expression, string, Type[], Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, string method, Type[] genericTypes, params Expression[] arguments)
			=> Expression.Call(@this, method, genericTypes, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(Expression?, MethodInfo)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo)
			=> Expression.Call(@this, methodInfo);

		/// <summary>
		/// <c><see cref="Expression.Call(Expression?, MethodInfo, IEnumerable{Expression}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression> arguments)
			=> Expression.Call(@this, methodInfo, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(Expression?, MethodInfo, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, params Expression[] arguments)
			=> Expression.Call(@this, methodInfo, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(MethodInfo, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this MethodInfo @this)
			=> Expression.Call(@this);

		/// <summary>
		/// <c><see cref="Expression.Call(MethodInfo, IEnumerable{Expression}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this MethodInfo @this, IEnumerable<Expression> arguments)
			=> Expression.Call(@this, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(MethodInfo, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this MethodInfo @this, params Expression[] arguments)
			=> Expression.Call(@this, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(Type, string, Type[], Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this Type @this, string method, params Expression[] arguments)
			=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

		/// <summary>
		/// <c><see cref="Expression.Call(Type, string, Type[], Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodCallExpression CallStatic(this Type @this, string method, Type[] genericTypes, params Expression[] arguments)
			=> Expression.Call(@this, method, genericTypes, arguments);

		/// <summary>
		/// <code>
		/// !@<paramref name="this"/>.Type.IsValueType &amp;&amp; typeof(<typeparamref name="T"/>).IsValueType<br/>
		/// ? <see cref="Expression.Unbox(Expression, Type)"/><br/>
		/// : (<paramref name="overflowCheck"/> ? <see cref="Expression.ConvertChecked(Expression, Type)"/> : <see cref="Expression.Convert(Expression, Type)"/>)
		/// </code>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression Cast<T>(this Expression @this, bool overflowCheck = false)
			=> @this.Cast(typeof(T), overflowCheck);

		/// <summary>
		/// <code>
		/// !@<paramref name="this"/>.Type.IsValueType &amp;&amp; <paramref name="type"/>.IsValueType<br/>
		/// ? <see cref="Expression.Unbox(Expression, Type)"/><br/>
		/// : (<paramref name="overflowCheck"/> ? <see cref="Expression.ConvertChecked(Expression, Type)"/> : <see cref="Expression.Convert(Expression, Type)"/>)
		/// </code>
		/// </summary>
		public static UnaryExpression Cast(this Expression @this, Type type, bool overflowCheck = false)
			=> !@this.Type.IsValueType && type.IsValueType
				? Expression.Unbox(@this, type)
				: (overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type));

		/// <summary>
		/// <c><see cref="Expression.Coalesce(Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression Coalesce(this Expression @this, Expression expression)
			=> Expression.Coalesce(@this, expression);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><term>null</term> <description><see cref="Expression.Constant(object?)"/></description></item>
		/// <item><term>not null</term> <description><see cref="Expression.Constant(object?, Type)"/></description></item>
		/// </list>
		/// </code>
		/// </summary>
		public static ConstantExpression Constant<T>(this T? @this)
			=> @this is not null ? Expression.Constant(@this, @this.GetType()) : Expression.Constant(@this);

		/// <summary>
		/// <c><see cref="Expression.Default(Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DefaultExpression Default(this Type @this)
			=> Expression.Default(@this);

		/// <summary>
		/// <c><see cref="Expression.Field(Expression, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Field(this Expression @this, string name)
			=> Expression.Field(@this, name);

		/// <summary>
		/// <c><see cref="Expression.Field(Expression?, FieldInfo)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Field(this Expression @this, FieldInfo fieldInfo)
			=> Expression.Field(@this, fieldInfo);

		/// <summary>
		/// <c><see cref="Expression.IfThen(Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConditionalExpression If(this Expression @this, Expression trueResult)
			=> Expression.IfThen(@this, trueResult);

		/// <summary>
		/// <c><see cref="Expression.IfThenElse(Expression, Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConditionalExpression If(this Expression @this, Expression trueResult, Expression falseResult)
			=> Expression.IfThenElse(@this, trueResult, falseResult);

		/// <summary>
		/// <c><see cref="Expression.Condition(Expression, Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ConditionalExpression IIf(this Expression @this, Expression trueResult, Expression falseResult)
			=> Expression.Condition(@this, trueResult, falseResult);

		/// <summary>
		/// <c><see cref="Expression.Invoke(Expression, IEnumerable{Expression}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InvocationExpression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
			=> Expression.Invoke(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Invoke(Expression, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InvocationExpression Invoke(this LambdaExpression @this, params Expression[] parameters)
			=> Expression.Invoke(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.TypeIs(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression Is<T>(this Expression @this)
			=> Expression.TypeIs(@this, typeof(T));

		/// <summary>
		/// <c><see cref="Expression.TypeIs(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression Is(this Expression @this, Type type)
			=> Expression.TypeIs(@this, type);

		/// <summary>
		/// <c><see cref="Expression"/>.ReferenceNotEqual(<see cref="Expression"/>, <see cref="Expression"/>.Constant(null))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression IsNotNull(this Expression @this)
			=> Expression.ReferenceNotEqual(@this, Expression.Constant(null));

		/// <summary>
		/// <c><see cref="Expression"/>.ReferenceEqual(<see cref="Expression"/>, <see cref="Expression"/>.Constant(null))</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BinaryExpression IsNull(this Expression @this)
			=> Expression.ReferenceEqual(@this, Expression.Constant(null));

		/// <summary>
		/// <c><see cref="Expression.Lambda(Expression, IEnumerable{ParameterExpression}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Expression, ParameterExpression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LambdaExpression Lambda(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda{TDelegate}(Expression, IEnumerable{ParameterExpression}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda<T>(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda{TDelegate}(Expression, ParameterExpression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<T> Lambda<T>(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda<T>(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression}?)"/></c>
		/// </summary>
		public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray()), @this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/></c>
		/// </summary>
		public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetActionType(parameters.ToArray(parameter => parameter.Type)), @this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression}?)"/></c>
		/// </summary>
		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/></c>
		/// </summary>
		public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression}?)"/></c>
		/// </summary>
		public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

		/// <summary>
		/// <c><see cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/></c>
		/// </summary>
		public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[] parameters)
			=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

		/// <summary>
		/// <c><see cref="Expression.PropertyOrField(Expression, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Member(this Expression @this, string name)
			=> Expression.PropertyOrField(@this, name);

		/// <summary>
		/// <c><see cref="Expression.MemberInit(NewExpression, IEnumerable{MemberBinding})"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInitExpression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
			=> Expression.MemberInit(@this, bindings);

		/// <summary>
		/// <c><see cref="Expression.MemberInit(NewExpression, MemberBinding[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInitExpression MemberInit(this NewExpression @this, params MemberBinding[] bindings)
			=> Expression.MemberInit(@this, bindings);

		/// <summary>
		/// <c><see cref="Expression.New(ConstructorInfo, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, params Expression[]? parameters)
			=> Expression.New(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.New(ConstructorInfo, IEnumerable{Expression}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters)
			=> Expression.New(@this, parameters);

		/// <summary>
		/// <c><see cref="Expression.New(ConstructorInfo, IEnumerable{Expression}?, MemberInfo[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, MemberInfo[] memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		/// <summary>
		/// <c><see cref="Expression.New(ConstructorInfo, IEnumerable{Expression}?, IEnumerable{MemberInfo}?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		/// <summary>
		/// <c><see cref="Expression.New(Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NewExpression New(this Type @this)
			=> Expression.New(@this);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.IsTrue(Expression)"/></item>
		/// <item><see cref="Expression.IsFalse(Expression)"/></item>
		/// <item><see cref="Expression.PreIncrementAssign(Expression)"/></item>
		/// <item><see cref="Expression.Increment(Expression)"/></item>
		/// <item><see cref="Expression.PostIncrementAssign(Expression)"/></item>
		/// <item><see cref="Expression.PreDecrementAssign(Expression)"/></item>
		/// <item><see cref="Expression.Decrement(Expression)"/></item>
		/// <item><see cref="Expression.PostDecrementAssign(Expression)"/></item>
		/// <item><see cref="Expression.Negate(Expression)"/></item>
		/// <item><see cref="Expression.NegateChecked(Expression)"/></item>
		/// <item><see cref="Expression.OnesComplement(Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static UnaryExpression Operation(this Expression @this, UnaryOp operation)
			=> operation switch
			{
				UnaryOp.IsTrue => Expression.IsTrue(@this),
				UnaryOp.IsFalse => Expression.IsFalse(@this),
				UnaryOp.PreIncrement => Expression.PreIncrementAssign(@this),
				UnaryOp.Increment => Expression.Increment(@this),
				UnaryOp.PostIncrement => Expression.PostIncrementAssign(@this),
				UnaryOp.PreDecrement => Expression.PreDecrementAssign(@this),
				UnaryOp.Decrement => Expression.Decrement(@this),
				UnaryOp.PostDecrement => Expression.PostDecrementAssign(@this),
				UnaryOp.Negate => Expression.Negate(@this),
				UnaryOp.NegateChecked => Expression.NegateChecked(@this),
				UnaryOp.Complement => Expression.OnesComplement(@this),
				_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(UnaryOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.Add(Expression, Expression)"/></item>
		/// <item><see cref="Expression.AddChecked(Expression, Expression)"/></item>
		/// <item><see cref="Expression.Divide(Expression, Expression)"/></item>
		/// <item><see cref="Expression.Modulo(Expression, Expression)"/></item>
		/// <item><see cref="Expression.Multiply(Expression, Expression)"/></item>
		/// <item><see cref="Expression.MultiplyChecked(Expression, Expression)"/></item>
		/// <item><see cref="Expression.Power(Expression, Expression)"/></item>
		/// <item><see cref="Expression.Subtract(Expression, Expression)"/></item>
		/// <item><see cref="Expression.SubtractChecked(Expression, Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static BinaryExpression Operation(this Expression @this, ArithmeticOp operation, Expression operand)
			=> operation switch
			{
				ArithmeticOp.Add => Expression.Add(@this, operand),
				ArithmeticOp.AddChecked => Expression.AddChecked(@this, operand),
				ArithmeticOp.Divide => Expression.Divide(@this, operand),
				ArithmeticOp.Modulus => Expression.Modulo(@this, operand),
				ArithmeticOp.Multiply => Expression.Multiply(@this, operand),
				ArithmeticOp.MultiplyChecked => Expression.MultiplyChecked(@this, operand),
				ArithmeticOp.Power => Expression.Power(@this, operand),
				ArithmeticOp.Subtract => Expression.Subtract(@this, operand),
				ArithmeticOp.SubtractChecked => Expression.SubtractChecked(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(ArithmeticOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.And(Expression, Expression)"/></item>
		/// <item><see cref="Expression.Or(Expression, Expression)"/></item>
		/// <item><see cref="Expression.ExclusiveOr(Expression, Expression)"/></item>
		/// <item><see cref="Expression.LeftShift(Expression, Expression)"/></item>
		/// <item><see cref="Expression.RightShift(Expression, Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static BinaryExpression Operation(this Expression @this, BitwiseOp operation, Expression operand)
			=> operation switch
			{
				BitwiseOp.And => Expression.And(@this, operand),
				BitwiseOp.Or => Expression.Or(@this, operand),
				BitwiseOp.ExclusiveOr => Expression.ExclusiveOr(@this, operand),
				BitwiseOp.LeftShift => Expression.LeftShift(@this, operand),
				BitwiseOp.RightShift => Expression.RightShift(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(BitwiseOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.Equal(Expression, Expression)"/></item>
		/// <item><see cref="Expression.ReferenceEqual(Expression, Expression)"/></item>
		/// <item><see cref="Expression.NotEqual(Expression, Expression)"/></item>
		/// <item><see cref="Expression.ReferenceNotEqual(Expression, Expression)"/></item>
		/// <item><see cref="Expression.GreaterThan(Expression, Expression)"/></item>
		/// <item><see cref="Expression.GreaterThanOrEqual(Expression, Expression)"/></item>
		/// <item><see cref="Expression.LessThan(Expression, Expression)"/></item>
		/// <item><see cref="Expression.LessThanOrEqual(Expression, Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static BinaryExpression Operation(this Expression @this, EqualityOp operation, Expression operand)
			=> operation switch
			{
				EqualityOp.EqualTo => Expression.Equal(@this, operand),
				EqualityOp.ReferenceEqualTo => Expression.ReferenceEqual(@this, operand),
				EqualityOp.NotEqualTo => Expression.NotEqual(@this, operand),
				EqualityOp.ReferenceNotEqualTo => Expression.ReferenceNotEqual(@this, operand),
				EqualityOp.MoreThan => Expression.GreaterThan(@this, operand),
				EqualityOp.MoreThanOrEqualTo => Expression.GreaterThanOrEqual(@this, operand),
				EqualityOp.LessThan => Expression.LessThan(@this, operand),
				EqualityOp.LessThanOrEqualTo => Expression.LessThanOrEqual(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(EqualityOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.AndAlso(Expression, Expression)"/></item>
		/// <item><see cref="Expression.OrElse(Expression, Expression)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		/// <remarks>Throws <see cref="NotSupportedException"/>.</remarks>
		public static BinaryExpression Operation(this Expression @this, LogicalOp operation, Expression operand)
			=> operation switch
			{
				LogicalOp.And => Expression.AndAlso(@this, operand),
				LogicalOp.Or => Expression.OrElse(@this, operand),
				_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(EqualityOp)} [{operation}] is not supported.")
			};

		/// <summary>
		/// <c><see cref="Expression.Parameter(Type, string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter(this ParameterInfo @this)
			=> Expression.Parameter(@this.ParameterType, @this.Name);

		/// <summary>
		/// <c><see cref="Expression.Parameter(Type, string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter(this string @this, Type type)
			=> Expression.Parameter(type, @this);

		/// <summary>
		/// <c><see cref="Expression.Parameter(Type, string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParameterExpression Parameter<T>(this string @this)
			=> Expression.Parameter(typeof(T), @this);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, string name)
			=> Expression.Property(@this, name);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, Type, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, Type type, string name)
			=> Expression.Property(@this, type, name);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, Type, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property<T>(this Expression @this, string name)
			=> Expression.Property(@this, typeof(T), name);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, PropertyInfo)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, PropertyInfo propertyInfo)
			=> Expression.Property(@this, propertyInfo);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, PropertyInfo, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexExpression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index)
			=> Expression.Property(@this, propertyInfo, index);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, MethodInfo)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression Property(this Expression @this, MethodInfo getMethodInfo)
			=> Expression.Property(@this, getMethodInfo);

		/// <summary>
		/// <c><see cref="Expression.Field(Expression?, FieldInfo)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticField(this FieldInfo @this)
			=> Expression.Field(null, @this);

		/// <summary>
		/// <c><see cref="Expression.Field(Expression?, Type, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticField(this Type @this, string name)
			=> Expression.Field(null, @this, name);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, PropertyInfo)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticProperty(this PropertyInfo @this)
			=> Expression.Property(null, @this);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, PropertyInfo, Expression[])"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexExpression StaticProperty(this PropertyInfo @this, ParameterExpression index)
			=> Expression.Property(null, @this, index);

		/// <summary>
		/// <c><see cref="Expression.Property(Expression?, Type, string)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberExpression StaticProperty(this Type @this, string name)
			=> Expression.Property(null, @this, name);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><see cref="Expression.Unbox(Expression, Type)"/>, <see cref="Expression.Convert(Expression, Type)"/></item>
		/// <item><see cref="Convert.ToBoolean(object?)"/></item>
		/// <item><see cref="Convert.ToSByte(object?)"/>, <see cref="Convert.ToByte(object?)"/></item>
		/// <item><see cref="Convert.ToInt16(object?)"/>, <see cref="Convert.ToUInt16(object?)"/></item>
		/// <item><see cref="Convert.ToInt32(object?)"/>, <see cref="Convert.ToUInt32(object?)"/></item>
		/// <item><see cref="Convert.ToInt64(object?)"/>, <see cref="Convert.ToUInt64(object?)"/></item>
		/// <item><see cref="Convert.ToSingle(object?)"/>, <see cref="Convert.ToDouble(object?)"/>, <see cref="Convert.ToDecimal(object?)"/></item>
		/// <item><see cref="Convert.ToDateTime(object?)"/></item>
		/// <item><see cref="Convert.ToChar(object?)"/></item>
		/// <item><see cref="Convert.ToString(object?)"/></item>
		/// </list>
		/// </code>
		/// </summary>
		public static Expression SystemConvert(this Expression @this, Type type)
			=> type.GetSystemType() switch
			{
				_ when type == @this.Type => @this,
				_ when type.IsEnum => @this.Cast(type),
				SystemType.Boolean => typeof(Convert).CallStatic(nameof(Convert.ToBoolean), Type.EmptyTypes, @this),
				SystemType.Char => typeof(Convert).CallStatic(nameof(Convert.ToChar), Type.EmptyTypes, @this),
				SystemType.SByte => typeof(Convert).CallStatic(nameof(Convert.ToSByte), Type.EmptyTypes, @this),
				SystemType.Byte => typeof(Convert).CallStatic(nameof(Convert.ToByte), Type.EmptyTypes, @this),
				SystemType.Int16 => typeof(Convert).CallStatic(nameof(Convert.ToInt16), Type.EmptyTypes, @this),
				SystemType.UInt16 => typeof(Convert).CallStatic(nameof(Convert.ToUInt16), Type.EmptyTypes, @this),
				SystemType.Int32 => typeof(Convert).CallStatic(nameof(Convert.ToInt32), Type.EmptyTypes, @this),
				SystemType.UInt32 => typeof(Convert).CallStatic(nameof(Convert.ToUInt32), Type.EmptyTypes, @this),
				SystemType.Int64 or SystemType.NInt => typeof(Convert).CallStatic(nameof(Convert.ToInt64), Type.EmptyTypes, @this),
				SystemType.UInt64 or SystemType.NUInt => typeof(Convert).CallStatic(nameof(Convert.ToUInt64), Type.EmptyTypes, @this),
				SystemType.Single => typeof(Convert).CallStatic(nameof(Convert.ToSingle), Type.EmptyTypes, @this),
				SystemType.Double => typeof(Convert).CallStatic(nameof(Convert.ToDouble), Type.EmptyTypes, @this),
				SystemType.Decimal => typeof(Convert).CallStatic(nameof(Convert.ToDecimal), Type.EmptyTypes, @this),
				SystemType.DateTime => typeof(Convert).CallStatic(nameof(Convert.ToDateTime), Type.EmptyTypes, @this),
				SystemType.String => typeof(Convert).CallStatic(nameof(Convert.ToString), Type.EmptyTypes, @this),
				_ => @this.Cast(type)
			};

		/// <summary>
		/// <c><see cref="Expression.TypeEqual(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression TypeEqual<T>(this Expression @this)
			=> Expression.TypeEqual(@this, typeof(T));

		/// <summary>
		/// <c><see cref="Expression.TypeEqual(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeBinaryExpression TypeEqual(this Expression @this, Type type)
			=> Expression.TypeEqual(@this, type);

		/// <summary>
		/// <c><see cref="Expression.Unbox(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnaryExpression Unbox<T>(this Expression @this)
			where T : struct
			=> Expression.Unbox(@this, typeof(T));

		/// <summary>
		/// <c><see cref="Expression.Unbox(Expression, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnaryExpression Unbox(this Expression @this, Type type)
			=> Expression.Unbox(@this, type);

		/// <summary>
		/// <c><see cref="Expression.Block(Expression, Expression)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BlockExpression Void(this MethodCallExpression @this)
			=> Expression.Block(@this, Expression.Empty());

		#region LabelTarget

		/// <summary>
		/// <c><see cref="Expression.Break(LabelTarget)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this)
			=> Expression.Break(@this);

		/// <summary>
		/// <c><see cref="Expression.Break(LabelTarget, Expression?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this, Expression? value)
			=> Expression.Break(@this, value);

		/// <summary>
		/// <c><see cref="Expression.Break(LabelTarget, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this, Type type)
			=> Expression.Break(@this, type);

		/// <summary>
		/// <c><see cref="Expression.Break(LabelTarget, Expression?, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Break(this LabelTarget @this, Expression? value, Type type)
			=> Expression.Break(@this, value, type);

		/// <summary>
		/// <c><see cref="Expression.Continue(LabelTarget)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Continue(this LabelTarget @this)
			=> Expression.Continue(@this);

		/// <summary>
		/// <c><see cref="Expression.Continue(LabelTarget, Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Continue(this LabelTarget @this, Type type)
			=> Expression.Continue(@this, type);

		/// <summary>
		/// <c><see cref="Expression.Goto(LabelTarget)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GotoExpression Goto(this LabelTarget @this)
			=> Expression.Goto(@this);

		/// <summary>
		/// <c><see cref="Expression.Label(Type)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelTarget Label(this Type @this)
			=> Expression.Label(@this);

		/// <summary>
		/// <c><see cref="Expression.Label(Type, string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelTarget Label(this Type @this, string? name)
			=> Expression.Label(@this, name);

		/// <summary>
		/// <c><see cref="Expression.Label(LabelTarget)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelExpression Label(this LabelTarget @this)
			=> Expression.Label(@this);

		/// <summary>
		/// <c><see cref="Expression.Label(LabelTarget, Expression?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelExpression Label(this LabelTarget @this, Expression? defaultValue)
			=> Expression.Label(@this, defaultValue);

		/// <summary>
		/// <c><see cref="Expression.Label(string?)"/></c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LabelTarget Label(this string? @this)
			=> Expression.Label(@this);

		#endregion
	}
}
