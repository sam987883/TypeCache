﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

public static class ExpressionExtensions
{
	/// <inheritdoc cref="Expression.AndAlso(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.AndAlso(@<paramref name="this"/>, <paramref name="operand"/>);</c>
	/// </remarks>
	/// <remarks><c>a &amp;&amp; b</c></remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression And(this Expression @this, Expression operand)
		=> Expression.AndAlso(@this, operand);

	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="ArrayExpressionBuilder"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ArrayExpressionBuilder Array(this Expression @this)
		=> new ArrayExpressionBuilder(@this);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression Array(this Expression @this, int index)
		=> Expression.ArrayIndex(@this, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.ToArray(index =&gt;
	///		(<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	public static MethodCallExpression Array(this Expression @this, params int[] indexes)
		=> Expression.ArrayIndex(@this, indexes.ToArray(index => (Expression)Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression Array(this Expression @this, long index)
		=> Expression.ArrayIndex(@this, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.ToArray(index =&gt;
	///		(<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	public static MethodCallExpression Array(this Expression @this, params long[] indexes)
		=> Expression.ArrayIndex(@this, indexes.ToArray(index => (Expression)Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression Array(this Expression @this, Expression index)
		=> Expression.ArrayIndex(@this, index);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Array(this Expression @this, IEnumerable<Expression> indexes)
		=> Expression.ArrayIndex(@this, indexes);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Array(this Expression @this, params Expression[] indexes)
		=> Expression.ArrayIndex(@this, indexes);

	/// <inheritdoc cref="Expression.TypeAs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Expression As<T>(this Expression @this)
		where T : class
		=> Expression.TypeAs(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeAs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Expression As(this Expression @this, Type type)
		=> Expression.TypeAs(@this, type);

	/// <remarks>
	/// <code>
	/// <paramref name="operation"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Add"/>		=&gt; <see cref="Expression"/>.AddAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.AddChecked"/>	=&gt; <see cref="Expression"/>.AddAssignChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Divide"/>		=&gt; <see cref="Expression"/>.DivideAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Modulus"/>		=&gt; <see cref="Expression"/>.ModuloAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Multiply"/>		=&gt; <see cref="Expression"/>.MultiplyAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.MultiplyChecked"/>	=&gt; <see cref="Expression"/>.MultiplyAssignChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Power"/>		=&gt; <see cref="Expression"/>.PowerAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Subtract"/>		=&gt; <see cref="Expression"/>.SubtractAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.SubtractChecked"/>	=&gt; <see cref="Expression"/>.SubtractAssignChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.And"/>		=&gt; <see cref="Expression"/>.AndAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Or"/>		=&gt; <see cref="Expression"/>.OrAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.ExclusiveOr"/>	=&gt; <see cref="Expression"/>.ExclusiveOrAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.LeftShift"/>	=&gt; <see cref="Expression"/>.LeftShiftAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.RightShift"/>	=&gt; <see cref="Expression"/>.RightShiftAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/>_					=&gt; <see langword="throw new"/> <see cref="NotSupportedException"/>($"{<see langword="nameof"/>(Assign)}: {<see langword="nameof"/>(<see cref="BinaryOperator"/>)} [{<paramref name="operation"/>:G}] is not supported.")
	/// }<br/>
	/// </code>
	/// </remarks>
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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression Assign(this Expression @this, Expression expression)
		=> Expression.Assign(@this, expression);

	/// <inheritdoc cref="Expression.Block(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Block(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BlockExpression Block(this Expression @this, Expression expression)
		=> Expression.Block(@this, expression);

	/// <inheritdoc cref="Expression.Break(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this)
		=> Expression.Break(@this);

	/// <inheritdoc cref="Expression.Break(LabelTarget, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this, Expression? value)
		=> Expression.Break(@this, value);

	/// <inheritdoc cref="Expression.Break(LabelTarget, Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="value"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this, Expression? value, Type type)
		=> Expression.Break(@this, value, type);

	/// <inheritdoc cref="Expression.Break(LabelTarget, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this, Type type)
		=> Expression.Break(@this, type);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo)
		=> Expression.Call(@this, methodInfo);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression>? arguments)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, params Expression[]? arguments)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, string method, params Expression[]? arguments)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, string method, Type[]? genericTypes, params Expression[]? arguments)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression CallStatic(this MethodInfo @this)
		=> Expression.Call(@this);

	/// <inheritdoc cref="Expression.Call(MethodInfo, IEnumerable{Expression}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression CallStatic(this MethodInfo @this, IEnumerable<Expression> arguments)
		=> Expression.Call(@this, arguments);

	/// <inheritdoc cref="Expression.Call(MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression CallStatic(this MethodInfo @this, params Expression[]? arguments)
		=> Expression.Call(@this, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression CallStatic(this Type @this, string method, params Expression[]? arguments)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MethodCallExpression CallStatic(this Type @this, string method, Type[]? genericTypes, params Expression[]? arguments)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Cast(<see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="overflowCheck"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Expression Cast<T>(this Expression @this, bool overflowCheck = false)
		=> @this.Cast(typeof(T), overflowCheck);

	/// <remarks>
	/// <c>=&gt; (@<paramref name="this"/>.Type.IsValueType, <paramref name="type"/>.IsValueType) <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>(<see langword="false"/>, <see langword="true"/>) =&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <paramref name="type"/>),<br/>
	/// <see langword="    "/>(<see langword="true"/>, <see langword="false"/>) =&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <paramref name="type"/>),<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.Convert(<paramref name="type"/>, <paramref name="overflowCheck"/>)<br/>
	///	<see langword="    "/>_ <see langword="when"/> <paramref name="type"/> == @<paramref name="this"/>.Type =&gt; @<paramref name="this"/>,<br/>
	///	<see langword="    "/>_ <see langword="when"/> <paramref name="type"/>.Implements&lt;<see cref="IConvertible"/>&gt;() =&gt; <see langword="typeof"/>(<see cref="System.Convert"/>).CallStatic(<see langword="nameof"/>(<see cref="System.Convert"/>.ChangeType), <see cref="Type.EmptyTypes"/>, @<paramref name="this"/>, <paramref name="type"/>.Constant&lt;<see cref="Type"/>&gt;()).Cast(<paramref name="type"/>),<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.Cast(<paramref name="type"/>)<br/>
	/// };</c>
	/// </remarks>
	public static Expression Cast(this Expression @this, Type type, bool overflowCheck = false)
		=> (@this.Type.IsValueType, type.IsValueType) switch
		{
			_ when @this.Type == type => @this,
			(false, true) => Expression.Unbox(@this, type),
			(true, true) when type.Implements<IConvertible>() => typeof(Convert).CallStatic(nameof(System.Convert.ChangeType), Type.EmptyTypes, @this.As<object>(), type.Constant<Type>()).Cast(type),
			(true, true) when overflowCheck => Expression.ConvertChecked(@this, type),
			(true, true) => Expression.Convert(@this, type),
			_ => Expression.TypeAs(@this, type),
		};

	/// <inheritdoc cref="Expression.Coalesce(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Coalesce(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression Coalesce(this Expression @this, Expression expression)
		=> Expression.Coalesce(@this, expression);

	/// <inheritdoc cref="Expression.Constant(object?, Type)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> <see langword="is not null"/>
	///	? <see cref="Expression"/>.Constant(@<paramref name="this"/>, @<paramref name="this"/>.GetType())
	///	: <see cref="Expression"/>.Constant(@<paramref name="this"/>);</c>
	/// </remarks>
	public static ConstantExpression Constant<T>(this T? @this)
		=> @this is not null ? Expression.Constant(@this, @this.GetType()) : NullExpression;

	/// <inheritdoc cref="Expression.Continue(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Continue(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Continue(this LabelTarget @this)
		=> Expression.Continue(@this);

	/// <inheritdoc cref="Expression.Continue(LabelTarget, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Continue(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Continue(this LabelTarget @this, Type type)
		=> Expression.Continue(@this, type);

	/// <inheritdoc cref="Expression.Convert(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Convert(<see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="overflowCheck"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Expression Convert<T>(this Expression @this, bool overflowCheck = false)
		=> @this.Convert(typeof(T), overflowCheck);

	/// <inheritdoc cref="Expression.Convert(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <paramref name="overflowCheck"/> ? <see cref="Expression"/>.ConvertChecked(@<paramref name="this"/>, <paramref name="type"/>) : <see cref="Expression"/>.Convert(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	public static UnaryExpression Convert(this Expression @this, Type type, bool overflowCheck = false)
		=> overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type);

	/// <inheritdoc cref="Expression.Default(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Default(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DefaultExpression Default(this Type @this)
		=> Expression.Default(@this);

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="fieldInfo"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Field(this Expression @this, FieldInfo fieldInfo)
		=> Expression.Field(@this, fieldInfo);

	/// <inheritdoc cref="Expression.Field(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Field(this Expression @this, string name)
		=> Expression.Field(@this, name);

	/// <remarks>
	/// <c>=&gt; !@<paramref name="this"/>.IsStatic
	///	? LambdaFactory.Create(<see langword="new"/>[] { @<paramref name="this"/>.DeclaringType! }, parameters =&gt; parameters[0].Field(@<paramref name="this"/>))
	///	: @<paramref name="this"/>.StaticField().Lambda();</c>
	/// </remarks>
	public static LambdaExpression FieldGetter(this FieldInfo @this)
		=> !@this.IsStatic
			? LambdaFactory.Create(new[] { @this.DeclaringType! }, parameters => parameters[0].Field(@this))
			: @this.StaticField().Lambda();

	/// <remarks>
	/// <code>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter&lt;<see cref="object"/>&gt;();<br/>
	/// <see langword="var"/> field = !@<paramref name="this"/>.IsStatic ? instance.Cast(@<paramref name="this"/>.DeclaringType!).Field(@<paramref name="this"/>) : @<paramref name="this"/>.StaticField();<br/>
	/// <see langword="return"/> field.As&lt;<see cref="object"/>&gt;().Lambda&lt;<see cref="GetValue"/>&gt;(instance);
	/// </code>
	/// </remarks>
	public static Expression<GetValue> FieldGetValue(this FieldInfo @this)
	{
		ParameterExpression instance = nameof(instance).Parameter<object>();
		var field = !@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.StaticField();
		return field.As<object>().Lambda<GetValue>(instance);
	}

	/// <remarks>
	/// <code>
	/// @<paramref name="this"/>.IsInitOnly.Assert(<see langword="false"/>);<br/>
	/// @<paramref name="this"/>.IsLiteral.Assert(<see langword="false"/>);<br/>
	/// <br/>
	/// <see langword="return"/> !@<paramref name="this"/>.IsStatic<br/>
	///	<see langword="    "/>? <see cref="LambdaFactory"/>.CreateAction(<see langword="new"/>[] { @<paramref name="this"/>.DeclaringType!, @<paramref name="this"/>.FieldType }, parameters =&gt; parameters[0].Field(@<paramref name="this"/>).Assign(parameters[1]))<br/>
	///	<see langword="    "/>: <see cref="LambdaFactory"/>.CreateAction(<see langword="new"/>[] { @<paramref name="this"/>.FieldType }, parameters =&gt; @<paramref name="this"/>.StaticField().Assign(parameters[0]));<br/>
	/// </code>
	/// </remarks>
	public static LambdaExpression FieldSetter(this FieldInfo @this)
	{
		@this.IsInitOnly.AssertEquals(false);
		@this.IsLiteral.AssertEquals(false);

		return !@this.IsStatic
			? LambdaFactory.CreateAction(new[] { @this.DeclaringType!, @this.FieldType }, parameters => parameters[0].Field(@this).Assign(parameters[1]))
			: LambdaFactory.CreateAction(new[] { @this.FieldType }, parameters => @this.StaticField().Assign(parameters[0]));
	}

	/// <remarks>
	/// <code>
	/// @<paramref name="this"/>.IsInitOnly.Assert(<see langword="false"/>);<br/>
	/// @<paramref name="this"/>.IsLiteral.Assert(<see langword="false"/>);<br/>
	/// <br/>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter&lt;<see cref="object"/>&gt;();<br/>
	/// <see cref="ParameterExpression"/> value = <see langword="nameof"/>(value).Parameter&lt;<see cref="object"/>&gt;();<br/>
	/// <br/>
	/// <see langword="var"/> field = !@<paramref name="this"/>.IsStatic ? instance.Cast(@<paramref name="this"/>.DeclaringType!).Field(@<paramref name="this"/>) : @<paramref name="this"/>.StaticField();<br/>
	/// <see langword="return"/> field.Assign(value.SystemConvert(@<paramref name="this"/>.FieldType)).Lambda&lt;<see cref="SetValue"/>&gt;(instance, value);
	/// </code>
	/// </remarks>
	public static Expression<SetValue> FieldSetValue(this FieldInfo @this)
	{
		@this.IsInitOnly.AssertEquals(false);
		@this.IsLiteral.AssertEquals(false);

		ParameterExpression instance = nameof(instance).Parameter<object>();
		ParameterExpression value = nameof(value).Parameter<object>();

		var field = !@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.StaticField();
		return field.Assign(value.Cast(@this.FieldType)).Lambda<SetValue>(instance, value);
	}

	/// <inheritdoc cref="Expression.Goto(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Goto(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static GotoExpression Goto(this LabelTarget @this)
		=> Expression.Goto(@this);

	/// <inheritdoc cref="Expression.IfThen(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.IfThen(@<paramref name="this"/>, <paramref name="trueResult"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ConditionalExpression If(this Expression @this, Expression trueResult)
		=> Expression.IfThen(@this, trueResult);

	/// <inheritdoc cref="Expression.IfThenElse(Expression, Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.IfThenElse(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ConditionalExpression If(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.IfThenElse(@this, trueResult, falseResult);

	/// <inheritdoc cref="Expression.Condition(Expression, Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Condition(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ConditionalExpression IIf(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.Condition(@this, trueResult, falseResult);

	/// <inheritdoc cref="Expression.Invoke(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static InvocationExpression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
		=> Expression.Invoke(@this, parameters);

	/// <inheritdoc cref="Expression.Invoke(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static InvocationExpression Invoke(this LambdaExpression @this, params Expression[]? parameters)
		=> Expression.Invoke(@this, parameters);

	/// <inheritdoc cref="Expression.TypeIs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static TypeBinaryExpression Is<T>(this Expression @this)
		=> Expression.TypeIs(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeIs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static TypeBinaryExpression Is(this Expression @this, Type type)
		=> Expression.TypeIs(@this, type);

	/// <inheritdoc cref="Expression.ReferenceNotEqual(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ReferenceNotEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression IsNotNull(this Expression @this)
		=> Expression.ReferenceNotEqual(@this, NullExpression);

	/// <inheritdoc cref="Expression.ReferenceEqual(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ReferenceEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression IsNull(this Expression @this)
		=> Expression.ReferenceEqual(@this, NullExpression);

	/// <inheritdoc cref="Expression.Label(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LabelExpression Label(this LabelTarget @this)
		=> Expression.Label(@this);

	/// <inheritdoc cref="Expression.Label(LabelTarget, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="defaultValue"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LabelExpression Label(this LabelTarget @this, Expression? defaultValue)
		=> Expression.Label(@this, defaultValue);

	/// <inheritdoc cref="Expression.Label(string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LabelTarget Label(this string? @this)
		=> Expression.Label(@this);

	/// <inheritdoc cref="Expression.Label(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LabelTarget Label(this Type @this)
		=> Expression.Label(@this);

	/// <inheritdoc cref="Expression.Label(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LabelTarget Label(this Type @this, string? name)
		=> Expression.Label(@this, name);

	/// <inheritdoc cref="Expression.Lambda(Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda{TDelegate}(Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static LambdaExpression Lambda(this Expression @this, params ParameterExpression[]? parameters)
		=> Expression.Lambda(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda{TDelegate}(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Expression<T> Lambda<T>(this Expression @this, params ParameterExpression[]? parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>(<paramref name="parameter1"/>).<see cref="Expression"/>.Lambda(<paramref name="parameter1"/>);</c>
	/// </remarks>
	public static LambdaExpression Lambda(this Func<ParameterExpression, Expression> @this, ParameterExpression parameter1)
		=> @this(parameter1).Lambda(parameter1);

	/// <remarks>
	/// <code>
	/// <see langword="var"/> parameterInfos = @<paramref name="this"/>.GetParameters();<br/>
	/// <see langword="if"/> (parameterInfos.Any())<br/>
	/// <see langword="    "/>parameterInfos.Sort(<see cref="ParameterPositionComparer"/>);<br/>
	/// <br/>
	/// <see langword="var"/> parameters = parameterInfos.To(parameterInfo =&gt; parameterInfo!.Parameter()).ToArray();<br/>
	/// <see langword="return"/> @<paramref name="this"/>.New(parameters).Lambda(parameters);
	/// </code>
	/// </remarks>
	public static LambdaExpression Lambda(this ConstructorInfo @this)
	{
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var parameters = parameterInfos.Map(parameterInfo => parameterInfo!.Parameter()).ToArray();
		return @this.New(parameters).Lambda(parameters);
	}

	/// <remarks>
	/// <code>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter(@<paramref name="this"/>.DeclaringType!);<br/>
	/// <see langword="var"/> parameterInfos = @<paramref name="this"/>.GetParameters();<br/>
	/// <see langword="if"/> (parameterInfos.Any())<br/>
	/// <see langword="    "/>parameterInfos.Sort(<see cref="ParameterPositionComparer"/>);<br/>
	/// <br/>
	/// <see langword="var"/> parameters = parameterInfos.To(parameterInfo =&gt; parameterInfo!.Parameter()).ToArray();<br/>
	/// <see langword="return"/> !@<paramref name="this"/>.IsStatic<br/>
	/// <see langword="    "/>? instance.Call(@<paramref name="this"/>, parameters).Lambda(<see langword="new"/>[] { instance }.And(parameters))<br/>
	/// <see langword="    "/>: @<paramref name="this"/>.CallStatic(parameters).Lambda(parameters);
	/// </code>
	/// </remarks>
	public static LambdaExpression Lambda(this MethodInfo @this)
	{
		ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var parameters = parameterInfos.Map(parameterInfo => parameterInfo!.Parameter()).ToArray();
		return !@this.IsStatic
			? instance.Call(@this, parameters).Lambda(new[] { instance }.Append(parameters))
			: @this.CallStatic(parameters).Lambda(parameters);
	}

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.Map(parameter => parameter.Type).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.ToArray(parameter =&gt; parameter.Type)), @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.ToArray(parameter => parameter.Type)), @this, parameters);

	/// <remarks>
	/// <code>
	/// <see cref="ParameterExpression"/> arguments = <see langword="nameof"/>(arguments).Parameter&lt;<see cref="object"/>[]&gt;();<br/>
	/// <br/>
	/// <see langword="var"/> parameterInfos = @<paramref name="this"/>.GetParameters();<br/>
	/// <see langword="if"/> (parameterInfos.Any())<br/>
	/// <see langword="    "/>parameterInfos.Sort(<see cref="ParameterPositionComparer"/>);<br/>
	/// <br/>
	/// <see langword="var"/> constructorParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.New(constructorParameters).As&lt;<see cref="object"/>&gt;().Lambda&lt;<see cref="CreateType"/>&gt;(arguments);
	/// </code>
	/// </remarks>
	public static Expression<CreateType> LambdaCreateType(this ConstructorInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).Parameter<object[]>();

		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var constructorParameters = parameterInfos.Map(parameterInfo => arguments.Array()[parameterInfo!.Position].Cast(parameterInfo.ParameterType));

		return @this.New(constructorParameters).As<object>().Lambda<CreateType>(arguments);
	}

	/// <remarks>
	/// <code>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter&lt;<see cref="object"/>[]&gt;();<br/>
	/// <see cref="ParameterExpression"/> arguments = <see langword="nameof"/>(arguments).Parameter&lt;<see cref="object"/>[]&gt;();<br/>
	/// <br/>
	/// <see langword="var"/> parameterInfos = @<paramref name="this"/>.GetParameters();<br/>
	/// <see langword="if"/> (parameterInfos.Any())<br/>
	/// <see langword="    "/>parameterInfos.Sort(<see cref="ParameterPositionComparer"/>);<br/>
	/// <br/>
	/// <see langword="var"/> methodParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));<br/>
	/// <br/>
	/// <see langword="var"/> call = !@<paramref name="this"/>.IsStatic<br/>
	/// <see langword="    "/>? instance.Cast(@<paramref name="this"/>.DeclaringType!).Call(@<paramref name="this"/>, methodParameters)<br/>
	/// <see langword="    "/>: @<paramref name="this"/>.CallStatic(methodParameters);<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.ReturnType != <see langword="typeof"/>(<see langword="void"/>)<br/>
	///	<see langword="    "/>? call.As&lt;<see cref="object"/>&gt;().Lambda&lt;<see cref="InvokeType"/>&gt;(instance, arguments)<br/>
	///	<see langword="    "/>: call.Block(<see cref="NullExpression"/>).Lambda&lt;<see cref="InvokeType"/>&gt;(instance, arguments);
	/// </code>
	/// </remarks>
	public static Expression<InvokeType> LambdaInvokeType(this MethodInfo @this)
	{
		ParameterExpression instance = nameof(instance).Parameter<object>();
		ParameterExpression arguments = nameof(arguments).Parameter<object[]>();

		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var methodParameters = parameterInfos.Map(parameterInfo => arguments.Array()[parameterInfo!.Position].Cast(parameterInfo.ParameterType));

		var call = !@this.IsStatic
			? instance.Cast(@this.DeclaringType!).Call(@this, methodParameters)
			: @this.CallStatic(methodParameters);

		return @this.ReturnType != typeof(void)
			? call.As<object>().Lambda<InvokeType>(instance, arguments)
			: call.Block(NullExpression).Lambda<InvokeType>(instance, arguments);
	}

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<see langword="typeof"/>(<typeparamref name="T"/>)).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.Map(parameter => parameter.Type).Append(typeof(T)).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<see langword="typeof"/>(<typeparamref name="T"/>)).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[]? parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.Map(parameter => parameter.Type).Append(typeof(T)).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<paramref name="returnType"/>).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.Map(parameter => parameter.Type).Append(returnType).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<paramref name="returnType"/>).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[]? parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.Map(parameter => parameter.Type).Append(returnType).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.PropertyOrField(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.PropertyOrField(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Member(this Expression @this, string name)
		=> Expression.PropertyOrField(@this, name);

	/// <inheritdoc cref="Expression.MemberInit(NewExpression, IEnumerable{MemberBinding})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberInitExpression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <inheritdoc cref="Expression.MemberInit(NewExpression, MemberBinding[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberInitExpression MemberInit(this NewExpression @this, params MemberBinding[] bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters)
		=> Expression.New(@this, parameters);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static NewExpression New(this ConstructorInfo @this, params Expression[]? parameters)
		=> Expression.New(@this, parameters);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, IEnumerable{MemberInfo})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
		=> Expression.New(@this, parameters, memberInfos);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, MemberInfo[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, params MemberInfo[]? memberInfos)
		=> Expression.New(@this, parameters, memberInfos);

	/// <inheritdoc cref="Expression.New(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static NewExpression New(this Type @this)
		=> Expression.New(@this);

	/// <remarks>
	/// <code>
	/// =&gt; <paramref name="operation"/> <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Add"/>			=&gt; <see cref="Expression"/>.Add(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.AddChecked"/>		=&gt; <see cref="Expression"/>.AddChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Divide"/>			=&gt; <see cref="Expression"/>.Divide(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Modulus"/>			=&gt; <see cref="Expression"/>.Modulus(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Multiply"/>			=&gt; <see cref="Expression"/>.Multiply(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.MultiplyChecked"/>		=&gt; <see cref="Expression"/>.MultiplyChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Power"/>			=&gt; <see cref="Expression"/>.Power(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Subtract"/>			=&gt; <see cref="Expression"/>.Subtract(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.SubtractChecked"/>		=&gt; <see cref="Expression"/>.SubtractChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.And"/>			=&gt; <see cref="Expression"/>.And(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.Or"/>			=&gt; <see cref="Expression"/>.Or(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.ExclusiveOr"/>		=&gt; <see cref="Expression"/>.ExclusiveOr(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.LeftShift"/>		=&gt; <see cref="Expression"/>.LeftShift(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.RightShift"/>		=&gt; <see cref="Expression"/>.RightShift(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.EqualTo"/>			=&gt; <see cref="Expression"/>.EqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.ReferenceEqualTo"/>	=&gt; <see cref="Expression"/>.ReferenceEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.NotEqualTo"/>		=&gt; <see cref="Expression"/>.NotEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.ReferenceNotEqualTo"/>	=&gt; <see cref="Expression"/>.ReferenceNotEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.GreaterThan"/>		=&gt; <see cref="Expression"/>.GreaterThan(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.GreaterThanOrEqualTo"/>	=&gt; <see cref="Expression"/>.GreaterThanOrEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.LessThan"/>			=&gt; <see cref="Expression"/>.LessThan(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/><see cref="BinaryOperator.LessThanOrEqualTo"/>	=&gt; <see cref="Expression"/>.LessThanOrEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///	<see langword="    "/>_						=&gt; <see langword="throw new"/> <see cref="NotSupportedException"/>($"Operation: {<see langword="nameof"/>(<see cref="BinaryOperator"/>)} [{<paramref name="operation"/>:G}] is not supported.")<br/>
	/// };
	/// </code>
	/// </remarks>
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

	/// <remarks>
	/// <code>
	/// =&gt; <paramref name="operation"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/><see cref="UnaryOperator.IsTrue"/>		=&gt; <see cref="Expression.IsTrue(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.IsFalse"/>		=&gt; <see cref="Expression.IsFalse(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.PreIncrement"/>	=&gt; <see cref="Expression.PreIncrementAssign(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.Increment"/>		=&gt; <see cref="Expression.Increment(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.PostIncrement"/>	=&gt; <see cref="Expression.PostIncrementAssign(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.PreDecrement"/>	=&gt; <see cref="Expression.PreDecrementAssign(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.Decrement"/>		=&gt; <see cref="Expression.Decrement(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.PostDecrement"/>	=&gt; <see cref="Expression.PostDecrementAssign(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.Negate"/>		=&gt; <see cref="Expression.Negate(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.NegateChecked"/>	=&gt; <see cref="Expression.NegateChecked(Expression)"/>,<br/>
	/// <see langword="    "/><see cref="UnaryOperator.Complement"/>	=&gt; <see cref="Expression.OnesComplement(Expression)"/>,<br/>
	///	<see langword="    "/>_					=&gt; <see langword="throw new"/> <see cref="NotSupportedException"/>($"Operation: {<see langword="nameof"/>(<see cref="UnaryOperator"/>)} [{<paramref name="operation"/>:G}] is not supported.")<br/>
	/// };
	/// </code>
	/// </remarks>
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
	/// <c>=&gt; <see cref="Expression"/>.OrElse(@<paramref name="this"/>, <paramref name="operand"/>);</c>
	/// </remarks>
	/// <remarks><c>a || b</c></remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BinaryExpression Or(this Expression @this, Expression operand)
		=> Expression.OrElse(@this, operand);

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ParameterExpression Parameter(this ParameterInfo @this)
		=> Expression.Parameter(@this.ParameterType, @this.Name);

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(<see langword="typeof"/>(<typeparamref name="T"/>), @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ParameterExpression Parameter<T>(this string @this)
		=> Expression.Parameter(typeof(T), @this);

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(<paramref name="type"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static ParameterExpression Parameter(this string @this, Type type)
		=> Expression.Parameter(type, @this);

	/// <inheritdoc cref="Expression.Property(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="getMethodInfo"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, MethodInfo getMethodInfo)
		=> Expression.Property(@this, getMethodInfo);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, PropertyInfo propertyInfo)
		=> Expression.Property(@this, propertyInfo);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IndexExpression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index)
		=> Expression.Property(@this, propertyInfo, index);

	/// <inheritdoc cref="Expression.Property(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, string name)
		=> Expression.Property(@this, name);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Property<T>(this Expression @this, string name)
		=> Expression.Property(@this, typeof(T), name);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="type"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, Type type, string name)
		=> Expression.Property(@this, type, name);

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression StaticField(this FieldInfo @this)
		=> Expression.Field(null, @this);

	/// <inheritdoc cref="Expression.Field(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression StaticField(this Type @this, string name)
		=> Expression.Field(null, @this, name);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression StaticProperty(this PropertyInfo @this)
		=> Expression.Property(null, @this);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static IndexExpression StaticProperty(this PropertyInfo @this, ParameterExpression index)
		=> Expression.Property(null, @this, index);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static MemberExpression StaticProperty(this Type @this, string name)
		=> Expression.Property(null, @this, name);

	/// <inheritdoc cref="Expression.TypeEqual(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static TypeBinaryExpression TypeEqual<T>(this Expression @this)
		=> Expression.TypeEqual(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeEqual(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static TypeBinaryExpression TypeEqual(this Expression @this, Type type)
		=> Expression.TypeEqual(@this, type);

	/// <inheritdoc cref="Expression.Unbox(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static UnaryExpression Unbox<T>(this Expression @this)
		where T : struct
		=> Expression.Unbox(@this, typeof(T));

	/// <inheritdoc cref="Expression.Unbox(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static UnaryExpression Unbox(this Expression @this, Type type)
		=> Expression.Unbox(@this, type);

	/// <inheritdoc cref="Expression.Block(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Block(@<paramref name="this"/>, <see cref="Expression.Empty()"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static BlockExpression Void(this MethodCallExpression @this)
		=> Expression.Block(@this, Expression.Empty());
}
