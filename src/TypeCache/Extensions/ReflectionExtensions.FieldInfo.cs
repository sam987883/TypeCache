// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
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
			: TypeStore.FieldGetInvokes.GetOrAdd(@this.FieldHandle, handle => @this.GetFieldValueFuncLambdaExpression().Compile())(instance);

	public static Expression<Func<object?, object?>> GetFieldValueFuncLambdaExpression(this FieldInfo @this)
	{
		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		var field = !@this.IsStatic ? instance.Convert(@this.DeclaringType!).Field(@this) : @this.ToStaticFieldExpression();
		return field.As<object>().Lambda<Func<object?, object?>>(instance);
	}

	public static LambdaExpression GetFieldValueLambdaExpression(this FieldInfo @this)
		=> !@this.IsStatic
			? LambdaFactory.Create(new[] { @this.DeclaringType! }, parameters => parameters[0].Field(@this))
			: @this.ToStaticFieldExpression().Lambda();

	/// <inheritdoc cref="FieldInfo.SetValue(object, object)"/>
	/// <remarks>
	/// <b>The code to set the field is built once and used subsequently.<br/>
	/// This is much faster than late binding.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static void SetFieldValue(this FieldInfo @this, object? instance, object? value)
	{
		if (!@this.IsInitOnly && !@this.IsLiteral)
			TypeStore.FieldSetInvokes.GetOrAdd(@this.FieldHandle, handle => @this.SetFieldValueActionLambdaExpression().Compile())(instance, value);
	}

	public static Expression<Action<object?, object?>> SetFieldValueActionLambdaExpression(this FieldInfo @this)
	{
		@this.IsInitOnly.AssertFalse();
		@this.IsLiteral.AssertFalse();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression value = nameof(value).ToParameterExpression<object>();

		var field = !@this.IsStatic ? instance.Convert(@this.DeclaringType!).Field(@this) : @this.ToStaticFieldExpression();
		return field.Assign(value.Convert(@this.FieldType)).Lambda<Action<object?, object?>>(instance, value);
	}

	public static LambdaExpression SetFieldValueLambdaExpression(this FieldInfo @this)
	{
		@this.IsInitOnly.AssertFalse();
		@this.IsLiteral.AssertFalse();

		return !@this.IsStatic
			? LambdaFactory.CreateAction(new[] { @this.DeclaringType!, @this.FieldType }, parameters => parameters[0].Field(@this).Assign(parameters[1]))
			: LambdaFactory.CreateAction(new[] { @this.FieldType }, parameters => @this.ToStaticFieldExpression().Assign(parameters[0]));
	}

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticFieldExpression(this FieldInfo @this)
		=> Expression.Field(null, @this);
}
