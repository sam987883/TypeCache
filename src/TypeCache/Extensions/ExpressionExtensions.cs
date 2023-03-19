// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ExpressionExtensions
{
	/// <inheritdoc cref="Expression.AndAlso(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.AndAlso(@<paramref name="this"/>, <paramref name="operand"/>);</c><br/><br/>
	/// <c>a &amp;&amp; b</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression And(this Expression @this, Expression operand)
		=> Expression.AndAlso(@this, operand);

	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="ArrayExpressionBuilder"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ArrayExpressionBuilder Array(this Expression @this)
		=> new ArrayExpressionBuilder(@this);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Array(this Expression @this, int index)
		=> Expression.ArrayIndex(@this, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.Select(index =&gt; (<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	public static MethodCallExpression Array(this Expression @this, params int[] indexes)
		=> Expression.ArrayIndex(@this, indexes.Select(index => (Expression)Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Array(this Expression @this, long index)
		=> Expression.ArrayIndex(@this, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.Select(index =&gt; (<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	public static MethodCallExpression Array(this Expression @this, params long[] indexes)
		=> Expression.ArrayIndex(@this, indexes.Select(index => (Expression)Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Array(this Expression @this, Expression index)
		=> Expression.ArrayIndex(@this, index);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Array(this Expression @this, IEnumerable<Expression> indexes)
		=> Expression.ArrayIndex(@this, indexes);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Array(this Expression @this, params Expression[] indexes)
		=> Expression.ArrayIndex(@this, indexes);

	/// <inheritdoc cref="Expression.TypeAs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression As<T>(this Expression @this)
		where T : class
		=> Expression.TypeAs(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeAs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression As(this Expression @this, Type type)
		=> Expression.TypeAs(@this, type);

	/// <exception cref="NotSupportedException" />
	public static BinaryExpression Assign(this Expression @this, BinaryOperator operation, Expression operand)
		=> operation switch
		{
			BinaryOperator.Add => Expression.AddAssign(@this, operand),
			BinaryOperator.AddChecked => Expression.AddAssignChecked(@this, operand),
			BinaryOperator.Divide => Expression.DivideAssign(@this, operand),
			BinaryOperator.Modulus => Expression.ModuloAssign(@this, operand),
			BinaryOperator.Multiply => Expression.MultiplyAssign(@this, operand),
			BinaryOperator.MultiplyChecked => Expression.MultiplyAssignChecked(@this, operand),
			BinaryOperator.Power => Expression.PowerAssign(@this, operand),
			BinaryOperator.Subtract => Expression.SubtractAssign(@this, operand),
			BinaryOperator.SubtractChecked => Expression.SubtractAssignChecked(@this, operand),
			BinaryOperator.And => Expression.AndAssign(@this, operand),
			BinaryOperator.Or => Expression.OrAssign(@this, operand),
			BinaryOperator.ExclusiveOr => Expression.ExclusiveOrAssign(@this, operand),
			BinaryOperator.LeftShift => Expression.LeftShiftAssign(@this, operand),
			BinaryOperator.RightShift => Expression.RightShiftAssign(@this, operand),
			_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(BinaryOperator)} [{operation:G}] is not supported.")
		};

	/// <inheritdoc cref="Expression.Assign(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Assign(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Assign(this Expression @this, Expression expression)
		=> Expression.Assign(@this, expression);

	/// <inheritdoc cref="Expression.Block(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Block(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BlockExpression Block(this Expression @this, Expression expression)
		=> Expression.Block(@this, expression);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo)
		=> Expression.Call(@this, methodInfo);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression>? arguments)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, params Expression[]? arguments)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, string method, params Expression[]? arguments)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, string method, Type[]? genericTypes, params Expression[]? arguments)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Convert(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Convert(<see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="overflowCheck"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression Cast<T>(this Expression @this, bool overflowCheck = false)
		=> @this.Cast(typeof(T), overflowCheck);

	/// <inheritdoc cref="Expression.Convert(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="overflowCheck"/> ? <see cref="Expression"/>.ConvertChecked(@<paramref name="this"/>, <paramref name="type"/>) : <see cref="Expression"/>.Convert(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	public static UnaryExpression Cast(this Expression @this, Type type, bool overflowCheck = false)
		=> overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type);

	/// <inheritdoc cref="Expression.Coalesce(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Coalesce(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Coalesce(this Expression @this, Expression expression)
		=> Expression.Coalesce(@this, expression);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Convert(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression Convert<T>(this Expression @this)
		=> @this.Convert(typeof(T));

	/// <remarks>
	/// <c>=&gt; <see langword="typeof"/>(<see cref="ValueConverters"/>).ToStaticMethodCallExpression(
	/// <see langword="nameof"/>(<see cref="ValueConverters"/>.ConvertObject),
	/// @<paramref name="this"/>.Cast&lt;<see langword="object"/>&gt;(),
	/// <paramref name="targetType"/>.ToConstantExpression()).Cast(<paramref name="targetType"/>);</c>
	/// </remarks>
	public static Expression Convert(this Expression @this, Type targetType)
		=> typeof(ValueConverters).ToStaticMethodCallExpression(nameof(ValueConverters.ConvertObject), @this.Cast<object>(), targetType.ToConstantExpression()).Cast(targetType);

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="fieldInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Field(this Expression @this, FieldInfo fieldInfo)
		=> Expression.Field(@this, fieldInfo);

	/// <inheritdoc cref="Expression.Field(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Field(this Expression @this, string name)
		=> Expression.Field(@this, name);

	/// <inheritdoc cref="Expression.IfThen(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.IfThen(@<paramref name="this"/>, <paramref name="trueResult"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConditionalExpression If(this Expression @this, Expression trueResult)
		=> Expression.IfThen(@this, trueResult);

	/// <inheritdoc cref="Expression.IfThenElse(Expression, Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.IfThenElse(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConditionalExpression If(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.IfThenElse(@this, trueResult, falseResult);

	/// <inheritdoc cref="Expression.Condition(Expression, Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Condition(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConditionalExpression IIf(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.Condition(@this, trueResult, falseResult);

	/// <inheritdoc cref="Expression.Invoke(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static InvocationExpression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
		=> Expression.Invoke(@this, parameters);

	/// <inheritdoc cref="Expression.Invoke(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static InvocationExpression Invoke(this LambdaExpression @this, params Expression[]? parameters)
		=> Expression.Invoke(@this, parameters);

	/// <inheritdoc cref="Expression.TypeIs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression Is<T>(this Expression @this)
		=> Expression.TypeIs(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeIs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression Is(this Expression @this, Type type)
		=> Expression.TypeIs(@this, type);

	/// <inheritdoc cref="Expression.ReferenceNotEqual(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ReferenceNotEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression IsNotNull(this Expression @this)
		=> Expression.ReferenceNotEqual(@this, Expression.Constant(null));

	/// <inheritdoc cref="Expression.ReferenceEqual(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ReferenceEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression IsNull(this Expression @this)
		=> Expression.ReferenceEqual(@this, Expression.Constant(null));

	/// <inheritdoc cref="Expression.Lambda(Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda{TDelegate}(Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LambdaExpression Lambda(this Expression @this, params ParameterExpression[]? parameters)
		=> Expression.Lambda(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda{TDelegate}(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression<T> Lambda<T>(this Expression @this, params ParameterExpression[]? parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.Select(parameter => parameter.Type).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.Select(parameter => parameter.Type).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.Select(parameter => parameter.Type).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.Select(parameter => parameter.Type).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.Select(parameter => parameter.Type).Append(<see langword="typeof"/>(<typeparamref name="T"/>)).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters?.Select(parameter => parameter.Type).Append(typeof(T)).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.Select(parameter => parameter.Type).Append(<see langword="typeof"/>(<typeparamref name="T"/>)).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[]? parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters?.Select(parameter => parameter.Type).Append(typeof(T)).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.Select(parameter => parameter.Type).Append(<paramref name="returnType"/>).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters?.Select(parameter => parameter.Type).Append(returnType).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.Select(parameter => parameter.Type).Append(<paramref name="returnType"/>).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[]? parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters?.Select(parameter => parameter.Type).Append(returnType).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.PropertyOrField(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.PropertyOrField(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Member(this Expression @this, string name)
		=> Expression.PropertyOrField(@this, name);

	/// <inheritdoc cref="Expression.MemberInit(NewExpression, IEnumerable{MemberBinding})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberInitExpression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <inheritdoc cref="Expression.MemberInit(NewExpression, MemberBinding[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberInitExpression MemberInit(this NewExpression @this, params MemberBinding[] bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <exception cref="NotSupportedException" />
	public static BinaryExpression Operation(this Expression @this, BinaryOperator operation, Expression operand)
		=> operation switch
		{
			BinaryOperator.Add => Expression.Add(@this, operand),
			BinaryOperator.AddChecked => Expression.AddChecked(@this, operand),
			BinaryOperator.Divide => Expression.Divide(@this, operand),
			BinaryOperator.Modulus => Expression.Modulo(@this, operand),
			BinaryOperator.Multiply => Expression.Multiply(@this, operand),
			BinaryOperator.MultiplyChecked => Expression.MultiplyChecked(@this, operand),
			BinaryOperator.Power => Expression.Power(@this, operand),
			BinaryOperator.Subtract => Expression.Subtract(@this, operand),
			BinaryOperator.SubtractChecked => Expression.SubtractChecked(@this, operand),
			BinaryOperator.And => Expression.And(@this, operand),
			BinaryOperator.Or => Expression.Or(@this, operand),
			BinaryOperator.ExclusiveOr => Expression.ExclusiveOr(@this, operand),
			BinaryOperator.LeftShift => Expression.LeftShift(@this, operand),
			BinaryOperator.RightShift => Expression.RightShift(@this, operand),
			BinaryOperator.EqualTo => Expression.Equal(@this, operand),
			BinaryOperator.ReferenceEqualTo => Expression.ReferenceEqual(@this, operand),
			BinaryOperator.NotEqualTo => Expression.NotEqual(@this, operand),
			BinaryOperator.ReferenceNotEqualTo => Expression.ReferenceNotEqual(@this, operand),
			BinaryOperator.GreaterThan => Expression.GreaterThan(@this, operand),
			BinaryOperator.GreaterThanOrEqualTo => Expression.GreaterThanOrEqual(@this, operand),
			BinaryOperator.LessThan => Expression.LessThan(@this, operand),
			BinaryOperator.LessThanOrEqualTo => Expression.LessThanOrEqual(@this, operand),
			_ => throw new NotSupportedException($"{nameof(Operation)}: {nameof(BinaryOperator)} [{operation:G}] is not supported.")
		};

	/// <exception cref="NotSupportedException" />
	public static UnaryExpression Operation(this Expression @this, UnaryOperator operation)
		=> operation switch
		{
			UnaryOperator.IsTrue => Expression.IsTrue(@this),
			UnaryOperator.IsFalse => Expression.IsFalse(@this),
			UnaryOperator.PreIncrement => Expression.PreIncrementAssign(@this),
			UnaryOperator.Increment => Expression.Increment(@this),
			UnaryOperator.PostIncrement => Expression.PostIncrementAssign(@this),
			UnaryOperator.PreDecrement => Expression.PreDecrementAssign(@this),
			UnaryOperator.Decrement => Expression.Decrement(@this),
			UnaryOperator.PostDecrement => Expression.PostDecrementAssign(@this),
			UnaryOperator.Negate => Expression.Negate(@this),
			UnaryOperator.NegateChecked => Expression.NegateChecked(@this),
			UnaryOperator.Complement => Expression.OnesComplement(@this),
			_ => throw new NotSupportedException($"{nameof(Assign)}: {nameof(UnaryOperator)} [{operation:G}] is not supported.")
		};

	/// <inheritdoc cref="Expression.OrElse(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.OrElse(@<paramref name="this"/>, <paramref name="operand"/>);</c><br/><br/>
	/// <c>a || b</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Or(this Expression @this, Expression operand)
		=> Expression.OrElse(@this, operand);

	/// <inheritdoc cref="Expression.Property(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="getMethodInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, MethodInfo getMethodInfo)
		=> Expression.Property(@this, getMethodInfo);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, PropertyInfo propertyInfo)
		=> Expression.Property(@this, propertyInfo);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index)
		=> Expression.Property(@this, propertyInfo, index);

	/// <inheritdoc cref="Expression.Property(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, string name)
		=> Expression.Property(@this, name);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property<T>(this Expression @this, string name)
		=> Expression.Property(@this, typeof(T), name);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="type"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, Type type, string name)
		=> Expression.Property(@this, type, name);

	/// <inheritdoc cref="Expression.TypeEqual(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression TypeEqual<T>(this Expression @this)
		=> Expression.TypeEqual(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeEqual(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression TypeEqual(this Expression @this, Type type)
		=> Expression.TypeEqual(@this, type);

	/// <inheritdoc cref="Expression.Unbox(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UnaryExpression Unbox<T>(this Expression @this)
		where T : struct
		=> Expression.Unbox(@this, typeof(T));

	/// <inheritdoc cref="Expression.Unbox(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UnaryExpression Unbox(this Expression @this, Type type)
		=> Expression.Unbox(@this, type);

	/// <inheritdoc cref="Expression.Block(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Block(@<paramref name="this"/>, <see cref="Expression.Empty()"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BlockExpression Void(this MethodCallExpression @this)
		=> Expression.Block(@this, Expression.Empty());
}
