// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection.Extensions;

public static class ExpressionExtensions
{
	/// <summary>
	/// <c><see cref="Expression"/>.AndAlso(@<paramref name="this"/>, <paramref name="operand"/>)</c>
	/// </summary>
	/// <remarks><c>a &amp;&amp; b</c></remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression And(this Expression @this, Expression operand)
		=> Expression.AndAlso(@this, operand);

	/// <summary>
	/// <c><see langword="new"/> <see cref="ArrayExpressionBuilder"/>(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ArrayExpressionBuilder Array(this Expression @this)
		=> new ArrayExpressionBuilder(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression Array(this Expression @this, int index)
		=> Expression.ArrayIndex(@this, Expression.Constant(index));

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.ToArray(index =&gt;
	///		(<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)))</c>
	/// </summary>
	public static MethodCallExpression Array(this Expression @this, params int[] indexes)
		=> Expression.ArrayIndex(@this, indexes.ToArray(index => (Expression)Expression.Constant(index)));

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression Array(this Expression @this, long index)
		=> Expression.ArrayIndex(@this, Expression.Constant(index));

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.ToArray(index =&gt;
	///		(<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)))</c>
	/// </summary>
	public static MethodCallExpression Array(this Expression @this, params long[] indexes)
		=> Expression.ArrayIndex(@this, indexes.ToArray(index => (Expression)Expression.Constant(index)));

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="index"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression Array(this Expression @this, Expression index)
		=> Expression.ArrayIndex(@this, index);

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Array(this Expression @this, IEnumerable<Expression> indexes)
		=> Expression.ArrayIndex(@this, indexes);

	/// <summary>
	/// <c><see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Array(this Expression @this, params Expression[] indexes)
		=> Expression.ArrayIndex(@this, indexes);

	/// <summary>
	/// <c><see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Expression As<T>(this Expression @this)
		where T : class
		=> Expression.TypeAs(@this, typeof(T));

	/// <summary>
	/// <c><see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Expression As(this Expression @this, Type type)
		=> Expression.TypeAs(@this, type);

	/// <summary>
	/// <code>
	/// <paramref name="operation"/> <see langword="switch"/><br/>
	/// {<br/>
	///		<see cref="BinaryOperator.Add"/>			=&gt; <see cref="Expression"/>.AddAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.AddChecked"/>		=&gt; <see cref="Expression"/>.AddAssignChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Divide"/>		=&gt; <see cref="Expression"/>.DivideAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Modulus"/>		=&gt; <see cref="Expression"/>.ModuloAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Multiply"/>		=&gt; <see cref="Expression"/>.MultiplyAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.MultiplyChecked"/>	=&gt; <see cref="Expression"/>.MultiplyAssignChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Power"/>			=&gt; <see cref="Expression"/>.PowerAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Subtract"/>		=&gt; <see cref="Expression"/>.SubtractAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.SubtractChecked"/>	=&gt; <see cref="Expression"/>.SubtractAssignChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.And"/>			=&gt; <see cref="Expression"/>.AndAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Or"/>			=&gt; <see cref="Expression"/>.OrAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.ExclusiveOr"/>		=&gt; <see cref="Expression"/>.ExclusiveOrAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.LeftShift"/>		=&gt; <see cref="Expression"/>.LeftShiftAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.RightShift"/>		=&gt; <see cref="Expression"/>.RightShiftAssign(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		_					=&gt; <see langword="throw new"/> <see cref="NotSupportedException"/>($"{<see langword="nameof"/>(Assign)}: {<see langword="nameof"/>(<see cref="BinaryOperator"/>)} [{<paramref name="operation"/>:G}] is not supported.")
	/// }<br/>
	/// </code>
	/// </summary>
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

	/// <summary>
	/// <c><see cref="Expression"/>.Assign(@<paramref name="this"/>, <paramref name="expression"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression Assign(this Expression @this, Expression expression)
		=> Expression.Assign(@this, expression);

	/// <summary>
	/// <c><see cref="Expression"/>.Block(@<paramref name="this"/>, <paramref name="expression"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BlockExpression Block(this Expression @this, Expression expression)
		=> Expression.Block(@this, expression);

	/// <summary>
	/// <c><see cref="Expression"/>.Break(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Break(this LabelTarget @this)
		=> Expression.Break(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="value"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Break(this LabelTarget @this, Expression? value)
		=> Expression.Break(@this, value);

	/// <summary>
	/// <c><see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="value"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Break(this LabelTarget @this, Expression? value, Type type)
		=> Expression.Break(@this, value, type);

	/// <summary>
	/// <c><see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Break(this LabelTarget @this, Type type)
		=> Expression.Break(@this, type);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo)
		=> Expression.Call(@this, methodInfo);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression> arguments)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, params Expression[] arguments)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Call(this Expression @this, string method, params Expression[] arguments)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression Call(this Expression @this, string method, Type[] genericTypes, params Expression[] arguments)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression CallStatic(this MethodInfo @this)
		=> Expression.Call(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression CallStatic(this MethodInfo @this, IEnumerable<Expression> arguments)
		=> Expression.Call(@this, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression CallStatic(this MethodInfo @this, params Expression[] arguments)
		=> Expression.Call(@this, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression CallStatic(this Type @this, string method, params Expression[] arguments)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <summary>
	/// <c><see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MethodCallExpression CallStatic(this Type @this, string method, Type[] genericTypes, params Expression[] arguments)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <summary>
	/// <code>
	/// @<paramref name="this"/>.Cast(<see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="overflowCheck"/>)
	/// </code>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Expression Cast<T>(this Expression @this, bool overflowCheck = false)
		=> @this.Cast(typeof(T), overflowCheck);

	/// <summary>
	/// <code>
	/// (@<paramref name="this"/>.Type.IsValueType, <paramref name="type"/>.IsValueType) <see langword="switch"/><br/>
	/// {<br/>
	/// (<see langword="false"/>, <see langword="true"/>) =&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <paramref name="type"/>),<br/>
	/// (<see langword="true"/>, <see langword="false"/>) =&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <paramref name="type"/>),<br/>
	/// _ =&gt; @<paramref name="this"/>.Convert(<paramref name="type"/>, <paramref name="overflowCheck"/>)<br/>
	/// }<br/>
	/// </code>
	/// </summary>
	public static UnaryExpression Cast(this Expression @this, Type type, bool overflowCheck = false)
		=> (@this.Type.IsValueType, type.IsValueType) switch
		{
			(false, true) => Expression.Unbox(@this, type),
			(true, false) => Expression.TypeAs(@this, type),
			_ => @this.Convert(type, overflowCheck)
		};

	/// <summary>
	/// <c><see cref="Expression"/>.Coalesce(@<paramref name="this"/>, <paramref name="expression"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression Coalesce(this Expression @this, Expression expression)
		=> Expression.Coalesce(@this, expression);

	/// <summary>
	/// <c>@<paramref name="this"/> <see langword="is not null"/>
	/// ? <see cref="Expression"/>.Constant(@<paramref name="this"/>, @<paramref name="this"/>.GetType())
	/// : <see cref="Expression"/>.Constant(@<paramref name="this"/>)</c>
	/// </summary>
	public static ConstantExpression Constant<T>(this T? @this)
		=> @this is not null ? Expression.Constant(@this, @this.GetType()) : NullExpression;

	/// <summary>
	/// <c><see cref="Expression"/>.Continue(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Continue(this LabelTarget @this)
		=> Expression.Continue(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Continue(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Continue(this LabelTarget @this, Type type)
		=> Expression.Continue(@this, type);

	/// <summary>
	/// <c><paramref name="overflowCheck"/>
	/// ? <see cref="Expression"/>.ConvertChecked(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>))
	/// : <see cref="Expression"/>.Convert(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Expression Convert<T>(this Expression @this, bool overflowCheck = false)
		=> overflowCheck ? Expression.ConvertChecked(@this, typeof(T)) : Expression.Convert(@this, typeof(T));

	/// <summary>
	/// <c><paramref name="overflowCheck"/> ? <see cref="Expression"/>.ConvertChecked(@<paramref name="this"/>, <paramref name="type"/>) : <see cref="Expression"/>.Convert(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	public static UnaryExpression Convert(this Expression @this, Type type, bool overflowCheck = false)
		=> overflowCheck ? Expression.ConvertChecked(@this, type) : Expression.Convert(@this, type);

	/// <summary>
	/// <c><see cref="Expression"/>.Default(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DefaultExpression Default(this Type @this)
		=> Expression.Default(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="fieldInfo"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Field(this Expression @this, FieldInfo fieldInfo)
		=> Expression.Field(@this, fieldInfo);

	/// <summary>
	/// <c><see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Field(this Expression @this, string name)
		=> Expression.Field(@this, name);

	/// <summary>
	/// <c>!@<paramref name="this"/>.IsStatic
	/// ? LambdaFactory.Create(<see langword="new"/>[] { @<paramref name="this"/>.DeclaringType! }, parameters =&gt; parameters[0].Field(@<paramref name="this"/>))
	/// : @<paramref name="this"/>.StaticField().Lambda()</c>
	/// </summary>
	public static LambdaExpression FieldGetter(this FieldInfo @this)
		=> !@this.IsStatic
			? LambdaFactory.Create(new[] { @this.DeclaringType! }, parameters => parameters[0].Field(@this))
			: @this.StaticField().Lambda();

	/// <summary>
	/// <code>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter&lt;<see cref="object"/>&gt;();<br/>
	/// <br/>
	/// <see langword="return"/> (!@<paramref name="this"/>.IsStatic ? instance.Cast(@<paramref name="this"/>.DeclaringType!).Field(@<paramref name="this"/>) : @<paramref name="this"/>.StaticField())
	/// .As&lt;<see cref="object"/>&gt;().Lambda&lt;<see cref="GetValue"/>&gt;(instance);
	/// </code>
	/// </summary>
	public static Expression<GetValue> FieldGetValue(this FieldInfo @this)
	{
		ParameterExpression instance = nameof(instance).Parameter<object>();
		return (!@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.StaticField()).As<object>().Lambda<GetValue>(instance);
	}

	/// <summary>
	/// <code>
	/// @<paramref name="this"/>.IsInitOnly.Assert(<see langword="false"/>);<br/>
	/// @<paramref name="this"/>.IsLiteral.Assert(<see langword="false"/>);<br/>
	/// <br/>
	/// <see langword="return"/> !@<paramref name="this"/>.IsStatic<br/>
	///		? <see cref="LambdaFactory"/>.CreateAction(<see langword="new"/>[] { @<paramref name="this"/>.DeclaringType!, @<paramref name="this"/>.FieldType }, parameters =&gt; parameters[0].Field(@<paramref name="this"/>).Assign(parameters[1]))<br/>
	///		: <see cref="LambdaFactory"/>.CreateAction(<see langword="new"/>[] { @<paramref name="this"/>.FieldType }, parameters =&gt; @<paramref name="this"/>.StaticField().Assign(parameters[0]));<br/>
	/// </code>
	/// </summary>
	public static LambdaExpression FieldSetter(this FieldInfo @this)
	{
		@this.IsInitOnly.Assert(false);
		@this.IsLiteral.Assert(false);

		return !@this.IsStatic
			? LambdaFactory.CreateAction(new[] { @this.DeclaringType!, @this.FieldType }, parameters => parameters[0].Field(@this).Assign(parameters[1]))
			: LambdaFactory.CreateAction(new[] { @this.FieldType }, parameters => @this.StaticField().Assign(parameters[0]));
	}

	/// <summary>
	/// <code>
	/// @<paramref name="this"/>.IsInitOnly.Assert(<see langword="false"/>);<br/>
	/// @<paramref name="this"/>.IsLiteral.Assert(<see langword="false"/>);<br/>
	/// <br/>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter&lt;<see cref="object"/>&gt;();<br/>
	/// <see cref="ParameterExpression"/> value = <see langword="nameof"/>(value).Parameter&lt;<see cref="object"/>&gt;();<br/>
	/// <br/>
	/// <see langword="return"/> (!@<paramref name="this"/>.IsStatic ? instance.Cast(@<paramref name="this"/>.DeclaringType!).Field(@<paramref name="this"/>) : @<paramref name="this"/>.StaticField())<br/>
	///		.Assign(value.SystemConvert(@<paramref name="this"/>.FieldType)).Lambda&lt;<see cref="SetValue"/>&gt;(instance, value);
	/// </code>
	/// </summary>
	public static Expression<SetValue> FieldSetValue(this FieldInfo @this)
	{
		@this.IsInitOnly.Assert(false);
		@this.IsLiteral.Assert(false);

		ParameterExpression instance = nameof(instance).Parameter<object>();
		ParameterExpression value = nameof(value).Parameter<object>();

		return (!@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.StaticField())
			.Assign(value.SystemConvert(@this.FieldType)).Lambda<SetValue>(instance, value);
	}

	/// <summary>
	/// <c><see cref="Expression"/>.Goto(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static GotoExpression Goto(this LabelTarget @this)
		=> Expression.Goto(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.IfThen(@<paramref name="this"/>, <paramref name="trueResult"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ConditionalExpression If(this Expression @this, Expression trueResult)
		=> Expression.IfThen(@this, trueResult);

	/// <summary>
	/// <c><see cref="Expression"/>.IfThenElse(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ConditionalExpression If(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.IfThenElse(@this, trueResult, falseResult);

	/// <summary>
	/// <c><see cref="Expression"/>.Condition(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ConditionalExpression IIf(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.Condition(@this, trueResult, falseResult);

	/// <summary>
	/// <c><see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static InvocationExpression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
		=> Expression.Invoke(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static InvocationExpression Invoke(this LambdaExpression @this, params Expression[] parameters)
		=> Expression.Invoke(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static TypeBinaryExpression Is<T>(this Expression @this)
		=> Expression.TypeIs(@this, typeof(T));

	/// <summary>
	/// <c><see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static TypeBinaryExpression Is(this Expression @this, Type type)
		=> Expression.TypeIs(@this, type);

	/// <summary>
	/// <c><see cref="Expression"/>.ReferenceNotEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression IsNotNull(this Expression @this)
		=> Expression.ReferenceNotEqual(@this, NullExpression);

	/// <summary>
	/// <c><see cref="Expression"/>.ReferenceEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression IsNull(this Expression @this)
		=> Expression.ReferenceEqual(@this, NullExpression);

	/// <summary>
	/// <c><see cref="Expression"/>.Label(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LabelExpression Label(this LabelTarget @this)
		=> Expression.Label(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="defaultValue"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LabelExpression Label(this LabelTarget @this, Expression? defaultValue)
		=> Expression.Label(@this, defaultValue);

	/// <summary>
	/// <c><see cref="Expression"/>.Label(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LabelTarget Label(this string? @this)
		=> Expression.Label(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Label(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LabelTarget Label(this Type @this)
		=> Expression.Label(@this);

	/// <summary>
	/// <c><see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LabelTarget Label(this Type @this, string? name)
		=> Expression.Label(@this, name);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static LambdaExpression Lambda(this Expression @this, params ParameterExpression[] parameters)
		=> Expression.Lambda(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Expression<T> Lambda<T>(this Expression @this, params ParameterExpression[] parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <summary>
	/// <c>@<paramref name="this"/>(<paramref name="parameter1"/>).<see cref="Expression"/>.Lambda(<paramref name="parameter1"/>)</c>
	/// </summary>
	public static LambdaExpression Lambda(this Func<ParameterExpression, Expression> @this, ParameterExpression parameter1)
		=> @this(parameter1).Lambda(parameter1);

	public static LambdaExpression Lambda(this ConstructorInfo @this)
	{
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var parameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray();

		return @this.New(parameters).Lambda(parameters);
	}

	public static LambdaExpression Lambda(this MethodInfo @this)
	{
		ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var parameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray();

		return !@this.IsStatic
			? instance.Call(@this, parameters).Lambda(new[] { instance }.And(parameters))
			: @this.CallStatic(parameters).Lambda(parameters);
	}

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.To(parameter => parameter.Type).ToArray()), @this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.ToArray(parameter =&gt; parameter.Type)), @<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	public static LambdaExpression LambdaAction(this Expression @this, params ParameterExpression[] parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.ToArray(parameter => parameter.Type)), @this, parameters);

	/// <summary>
	/// <code>
	/// <see cref="ParameterExpression"/> arguments = <see langword="nameof"/>(arguments).Parameter&lt;<see cref="object"/>[]&gt;();<br/>
	/// <br/>
	/// <see langword="var"/> parameterInfos = @<paramref name="this"/>.GetParameters();<br/>
	/// <see langword="if"/> (parameterInfos.Any()) parameterInfos.Sort(<see cref="ParameterPositionComparer"/>);<br/>
	/// <br/>
	/// <see langword="var"/> constructorParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.New(constructorParameters).As&lt;<see cref="object"/>&gt;().Lambda&lt;<see cref="CreateType"/>&gt;(arguments);
	/// </code>
	/// </summary>
	public static Expression<CreateType> LambdaCreateType(this ConstructorInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).Parameter<object[]>();

		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var constructorParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));

		return @this.New(constructorParameters).As<object>().Lambda<CreateType>(arguments);
	}

	/// <summary>
	/// <code>
	/// <see cref="ParameterExpression"/> instance = <see langword="nameof"/>(instance).Parameter&lt;<see cref="object"/>[]&gt;();<br/>
	/// <see cref="ParameterExpression"/> arguments = <see langword="nameof"/>(arguments).Parameter&lt;<see cref="object"/>[]&gt;();<br/>
	/// <br/>
	/// <see langword="var"/> parameterInfos = @<paramref name="this"/>.GetParameters();<br/>
	/// <see langword="if"/> (parameterInfos.Any()) parameterInfos.Sort(<see cref="ParameterPositionComparer"/>);<br/>
	/// <br/>
	/// <see langword="var"/> methodParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));<br/>
	/// <br/>
	/// <see langword="var"/> call = !@<paramref name="this"/>.IsStatic ? instance.Cast(@<paramref name="this"/>.DeclaringType!).Call(@<paramref name="this"/>, methodParameters) : @<paramref name="this"/>.CallStatic(methodParameters);<br/>
	/// <br/>
	/// <see langword="return"/> @<paramref name="this"/>.ReturnType != <see langword="typeof"/>(<see langword="void"/>)<br/>
	///		? call.As&lt;<see cref="object"/>&gt;().Lambda&lt;<see cref="InvokeType"/>&gt;(instance, arguments)<br/>
	///		: call.Block(<see cref="NullExpression"/>).Lambda&lt;<see cref="InvokeType"/>&gt;(instance, arguments);
	/// </code>
	/// </summary>
	public static Expression<InvokeType> LambdaInvokeType(this MethodInfo @this)
	{
		ParameterExpression instance = nameof(instance).Parameter<object>();
		ParameterExpression arguments = nameof(arguments).Parameter<object[]>();

		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any())
			parameterInfos.Sort(ParameterPositionComparer);

		var methodParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));

		var call = !@this.IsStatic
			? instance.Cast(@this.DeclaringType!).Call(@this, methodParameters)
			: @this.CallStatic(methodParameters);

		return @this.ReturnType != typeof(void)
			? call.As<object>().Lambda<InvokeType>(instance, arguments)
			: call.Block(NullExpression).Lambda<InvokeType>(instance, arguments);
	}

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<see langword="typeof"/>(<typeparamref name="T"/>)).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<see langword="typeof"/>(<typeparamref name="T"/>)).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, params ParameterExpression[] parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(typeof(T)).ToArray()), @this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<paramref name="returnType"/>).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType(<paramref name="parameters"/>.To(parameter =&gt; parameter.Type).And(<paramref name="returnType"/>).ToArray()), @<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, params ParameterExpression[] parameters)
		=> Expression.Lambda(Expression.GetFuncType(parameters.To(parameter => parameter.Type).And(returnType).ToArray()), @this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.PropertyOrField(@<paramref name="this"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Member(this Expression @this, string name)
		=> Expression.PropertyOrField(@this, name);

	/// <summary>
	/// <c><see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberInitExpression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <summary>
	/// <c><see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberInitExpression MemberInit(this NewExpression @this, params MemberBinding[] bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <summary>
	/// <c><see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters)
		=> Expression.New(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
		=> Expression.New(@this, parameters, memberInfos);

	/// <summary>
	/// <c><see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static NewExpression New(this ConstructorInfo @this, IEnumerable<Expression> parameters, MemberInfo[] memberInfos)
		=> Expression.New(@this, parameters, memberInfos);

	/// <summary>
	/// <c><see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static NewExpression New(this ConstructorInfo @this, params Expression[]? parameters)
		=> Expression.New(@this, parameters);

	/// <summary>
	/// <c><see cref="Expression"/>.New(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static NewExpression New(this Type @this)
		=> Expression.New(@this);

	/// <summary>
	/// <code>
	/// <paramref name="operation"/> <see langword="switch"/><br/>
	/// {<br/>
	///		<see cref="BinaryOperator.Add"/> =&gt; <see cref="Expression"/>.Add(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.AddChecked"/> =&gt; <see cref="Expression"/>.AddChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Divide"/> =&gt; <see cref="Expression"/>.Divide(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Modulus"/> =&gt; <see cref="Expression"/>.Modulus(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Multiply"/> =&gt; <see cref="Expression"/>.Multiply(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.MultiplyChecked"/> =&gt; <see cref="Expression"/>.MultiplyChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Power"/> =&gt; <see cref="Expression"/>.Power(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Subtract"/> =&gt; <see cref="Expression"/>.Subtract(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.SubtractChecked"/> =&gt; <see cref="Expression"/>.SubtractChecked(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.And"/> =&gt; <see cref="Expression"/>.And(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.Or"/> =&gt; <see cref="Expression"/>.Or(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.ExclusiveOr"/> =&gt; <see cref="Expression"/>.ExclusiveOr(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.LeftShift"/> =&gt; <see cref="Expression"/>.LeftShift(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.RightShift"/> =&gt; <see cref="Expression"/>.RightShift(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.EqualTo"/> =&gt; <see cref="Expression"/>.EqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.ReferenceEqualTo"/> =&gt; <see cref="Expression"/>.ReferenceEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.NotEqualTo"/> =&gt; <see cref="Expression"/>.NotEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.ReferenceNotEqualTo"/> =&gt; <see cref="Expression"/>.ReferenceNotEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.GreaterThan"/> =&gt; <see cref="Expression"/>.GreaterThan(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.GreaterThanOrEqualTo"/> =&gt; <see cref="Expression"/>.GreaterThanOrEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.LessThan"/> =&gt; <see cref="Expression"/>.LessThan(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		<see cref="BinaryOperator.LessThanOrEqualTo"/> =&gt; <see cref="Expression"/>.LessThanOrEqualTo(@<paramref name="this"/>, <paramref name="operand"/>),<br/>
	///		_ =&gt; <see langword="throw new"/> <see cref="NotSupportedException"/>($"Operation: {<see langword="nameof"/>(<see cref="BinaryOperator"/>)} [{<paramref name="operation"/>:G}] is not supported.")<br/>
	/// }<br/>
	/// </code>
	/// </summary>
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

	/// <summary>
	/// <list type="table">
	/// <listheader><c><see cref="UnaryOperator"/></c> to <c><see cref="Expression"/></c> Mapping:</listheader>
	/// <item><c><term><see cref="UnaryOperator.IsTrue"/></term> <see cref="Expression.IsTrue(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.IsFalse"/></term> <see cref="Expression.IsFalse(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.PreIncrement"/></term> <see cref="Expression.PreIncrementAssign(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.Increment"/></term> <see cref="Expression.Increment(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.PostIncrement"/></term> <see cref="Expression.PostIncrementAssign(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.PreDecrement"/></term> <see cref="Expression.PreDecrementAssign(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.Decrement"/></term> <see cref="Expression.Decrement(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.PostDecrement"/></term> <see cref="Expression.PostDecrementAssign(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.Negate"/></term> <see cref="Expression.Negate(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.NegateChecked"/></term> <see cref="Expression.NegateChecked(Expression)"/></c></item>
	/// <item><c><term><see cref="UnaryOperator.Complement"/></term> <see cref="Expression.OnesComplement(Expression)"/></c></item>
	/// </list>
	/// </summary>
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

	/// <summary>
	/// <c><see cref="Expression"/>.OrElse(@<paramref name="this"/>, <paramref name="operand"/>)</c>
	/// </summary>
	/// <remarks><c>a || b</c></remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BinaryExpression Or(this Expression @this, Expression operand)
		=> Expression.OrElse(@this, operand);

	/// <summary>
	/// <c><see cref="Expression"/>.Parameter(@<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ParameterExpression Parameter(this ParameterInfo @this)
		=> Expression.Parameter(@this.ParameterType, @this.Name);

	/// <summary>
	/// <c><see cref="Expression"/>.Parameter(<see langword="typeof"/>(<typeparamref name="T"/>), @<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ParameterExpression Parameter<T>(this string @this)
		=> Expression.Parameter(typeof(T), @this);

	/// <summary>
	/// <c><see cref="Expression"/>.Parameter(<paramref name="type"/>, @<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static ParameterExpression Parameter(this string @this, Type type)
		=> Expression.Parameter(type, @this);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="getMethodInfo"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Property(this Expression @this, MethodInfo getMethodInfo)
		=> Expression.Property(@this, getMethodInfo);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Property(this Expression @this, PropertyInfo propertyInfo)
		=> Expression.Property(@this, propertyInfo);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>, <paramref name="index"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IndexExpression Property(this Expression @this, PropertyInfo propertyInfo, ParameterExpression index)
		=> Expression.Property(@this, propertyInfo, index);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Property(this Expression @this, string name)
		=> Expression.Property(@this, name);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Property<T>(this Expression @this, string name)
		=> Expression.Property(@this, typeof(T), name);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="type"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression Property(this Expression @this, Type type, string name)
		=> Expression.Property(@this, type, name);

	/// <summary>
	/// <c><see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression StaticField(this FieldInfo @this)
		=> Expression.Field(null, @this);

	/// <summary>
	/// <c><see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression StaticField(this Type @this, string name)
		=> Expression.Field(null, @this, name);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression StaticProperty(this PropertyInfo @this)
		=> Expression.Property(null, @this);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="index"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static IndexExpression StaticProperty(this PropertyInfo @this, ParameterExpression index)
		=> Expression.Property(null, @this, index);

	/// <summary>
	/// <c><see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static MemberExpression StaticProperty(this Type @this, string name)
		=> Expression.Property(null, @this, name);

	/// <summary>
	/// <code>
	/// <paramref name="type"/> <see langword="switch"/><br/>
	/// {<br/>
	///		_ <see langword="when"/> <paramref name="type"/> == @<paramref name="this"/>.Type =&gt; @<paramref name="this"/>,<br/>
	///		_ <see langword="when"/> <paramref name="type"/>.Implements&lt;<see cref="IConvertible"/>&gt;() =&gt; <see langword="typeof"/>(<see cref="System.Convert"/>).CallStatic(<see langword="nameof"/>(<see cref="System.Convert"/>.ChangeType), <see cref="Type.EmptyTypes"/>, @<paramref name="this"/>, <paramref name="type"/>.Constant&lt;<see cref="Type"/>&gt;()).Cast(<paramref name="type"/>),<br/>
	///		_ =&gt; @<paramref name="this"/>.Cast(<paramref name="type"/>)<br/>
	/// }<br/>
	/// </code>
	/// </summary>
	public static Expression SystemConvert(this Expression @this, Type type)
		=> type switch
		{
			_ when type == @this.Type => @this,
			_ when type.Implements<IConvertible>() => typeof(Convert).CallStatic(nameof(System.Convert.ChangeType), Type.EmptyTypes, @this, type.Constant<Type>()).Cast(type),
			_ => @this.Cast(type)
		};

	/// <summary>
	/// <c><see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static TypeBinaryExpression TypeEqual<T>(this Expression @this)
		=> Expression.TypeEqual(@this, typeof(T));

	/// <summary>
	/// <c><see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static TypeBinaryExpression TypeEqual(this Expression @this, Type type)
		=> Expression.TypeEqual(@this, type);

	/// <summary>
	/// <c><see cref="Expression"/>.Unbox(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>))</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static UnaryExpression Unbox<T>(this Expression @this)
		where T : struct
		=> Expression.Unbox(@this, typeof(T));

	/// <summary>
	/// <c><see cref="Expression"/>.Unbox(@<paramref name="this"/>, <paramref name="type"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static UnaryExpression Unbox(this Expression @this, Type type)
		=> Expression.Unbox(@this, type);

	/// <summary>
	/// <c><see cref="Expression"/>.Block(@<paramref name="this"/>, <see cref="Expression.Empty()"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static BlockExpression Void(this MethodCallExpression @this)
		=> Expression.Block(@this, Expression.Empty());
}
