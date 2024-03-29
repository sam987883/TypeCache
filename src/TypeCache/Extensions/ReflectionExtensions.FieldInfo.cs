﻿// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <inheritdoc cref="FieldInfo.GetValue(object?)"/>
	/// <remarks>
	/// <b>The code to get the field value is built once and used subsequently.<br/>
	/// This is much faster than late binding.<br/>
	/// In the case of a constant, <c><see cref="FieldInfo.GetRawConstantValue"/></c> is used instead.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static object? GetFieldValue(this FieldInfo @this, object? instance)
		=> @this.IsLiteral
			? @this.GetRawConstantValue()
			: TypeStore.FieldGetFuncs.GetOrAdd(@this.FieldHandle, handle => @this.GetFieldValueFuncExpression().Compile())(instance);

	public static Expression<Func<object?, object?>> GetFieldValueFuncExpression(this FieldInfo @this)
	{
		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		var field = !@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.ToExpression(null);
		return field.As<object>().Lambda<Func<object?, object?>>([instance]);
	}

	public static LambdaExpression GetFieldValueLambdaExpression(this FieldInfo @this)
		=> !@this.IsStatic
			? LambdaFactory.Create([@this.DeclaringType!], parameters => parameters[0].Field(@this))
			: @this.ToExpression(null).Lambda();

	/// <inheritdoc cref="FieldInfo.SetValue(object, object)"/>
	/// <remarks>
	/// <b>The code to set the field is built once and used subsequently.<br/>
	/// This is much faster than late binding.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static void SetFieldValue(this FieldInfo @this, object? instance, object? value)
	{
		if (!@this.IsInitOnly && !@this.IsLiteral)
			TypeStore.FieldSetActions.GetOrAdd(@this.FieldHandle, handle => @this.SetFieldValueActionExpression().Compile())(instance, value);
	}

	public static Expression<Action<object?, object?>> SetFieldValueActionExpression(this FieldInfo @this)
	{
		@this.IsInitOnly.AssertFalse();
		@this.IsLiteral.AssertFalse();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression value = nameof(value).ToParameterExpression<object>();

		var field = !@this.IsStatic ? instance.Convert(@this.DeclaringType!).Field(@this) : @this.ToExpression(null);
		return field.Assign(value.Convert(@this.FieldType)).Lambda<Action<object?, object?>>([instance, value]);
	}

	public static LambdaExpression SetFieldValueLambdaExpression(this FieldInfo @this)
	{
		@this.IsInitOnly.AssertFalse();
		@this.IsLiteral.AssertFalse();

		return !@this.IsStatic
			? LambdaFactory.CreateAction([@this.DeclaringType!, @this.FieldType], parameters => parameters[0].Field(@this).Assign(parameters[1]))
			: LambdaFactory.CreateAction([@this.FieldType], parameters => @this.ToExpression(null).Assign(parameters[0]));
	}

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<paramref name="instance"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static field access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToExpression(this FieldInfo @this, Expression? instance)
		=> Expression.Field(instance, @this);
}
