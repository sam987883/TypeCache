// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <remarks>
	/// The code to get the field value is built once and used subsequently.<br/>
	/// This is much faster than late binding.<br/>
	/// In the case of a constant, <c><see cref="FieldInfo.GetRawConstantValue"/></c> is used instead.
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? GetStaticValue(this FieldInfo @this)
		=> TypeStore.StaticFieldGetFuncs[@this.FieldHandle]();

	/// <remarks>
	/// The code to get the static field value is built once and used subsequently.<br/>
	/// This is much faster than late binding.
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<object?> GetStaticValueFunc(this FieldInfo @this)
		=> TypeStore.StaticFieldGetFuncs[@this.FieldHandle];

	/// <remarks>
	/// The code to get the field value is built once and used subsequently.<br/>
	/// This is much faster than late binding.<br/>
	/// In the case of a constant, <c><see cref="FieldInfo.GetRawConstantValue"/></c> is used instead.
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? GetValueEx(this FieldInfo @this, object instance)
		=> TypeStore.FieldGetFuncs[@this.FieldHandle](instance);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<object, object?> GetValueFunc(this FieldInfo @this)
		=> TypeStore.FieldGetFuncs[@this.FieldHandle];

	/// <remarks>
	/// The code to set the field is built once and used subsequently.<br/>
	/// This is much faster than late binding.
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetStaticValue(this FieldInfo @this, object? value)
		=> TypeStore.StaticFieldSetActions[@this.FieldHandle](value);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Action<object?> SetStaticValueAction(this FieldInfo @this)
		=> TypeStore.StaticFieldSetActions[@this.FieldHandle];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Action<object, object?> SetValueAction(this FieldInfo @this)
		=> TypeStore.FieldSetActions[@this.FieldHandle];

	/// <remarks>
	/// The code to set the field is built once and used subsequently.<br/>
	/// This is much faster than late binding.
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static void SetValueEx(this FieldInfo @this, object instance, object? value)
		=> TypeStore.FieldSetActions[@this.FieldHandle](instance, value);

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<paramref name="instance"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static field access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToExpression(this FieldInfo @this, Expression? instance)
		=> Expression.Field(instance, @this);

	public static Expression<Action<object, object?>> ToActionExpression(this FieldInfo @this)
	{
		@this.IsInitOnly.ThrowIfTrue();
		@this.IsLiteral.ThrowIfTrue();
		@this.IsStatic.ThrowIfTrue();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression value = nameof(value).ToParameterExpression<object>();
		return instance
			.Convert(@this.DeclaringType!)
			.Field(@this)
			.Assign(value.Convert(@this.FieldType))
			.Lambda<Action<object, object?>>([instance, value]);
	}

	public static Expression<Func<object, object?>> ToFuncExpression(this FieldInfo @this)
	{
		@this.IsLiteral.ThrowIfTrue();
		@this.IsStatic.ThrowIfTrue();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		return instance
			.Cast(@this.DeclaringType!)
			.Field(@this)
			.As<object>()
			.Lambda<Func<object, object?>>([instance]);
	}

	public static Expression<Action<object?>> ToStaticActionExpression(this FieldInfo @this)
	{
		@this.IsInitOnly.ThrowIfTrue();
		@this.IsLiteral.ThrowIfTrue();
		@this.IsStatic.ThrowIfFalse();

		ParameterExpression value = nameof(value).ToParameterExpression<object>();
		return @this.ToExpression(null).Assign(value.Convert(@this.FieldType)).Lambda<Action<object?>>([value]);
	}

	public static Expression<Func<object?>> ToStaticFuncExpression(this FieldInfo @this)
	{
		if (@this.IsLiteral)
			return @this.GetRawConstantValue().ConstantExpression().Cast<object>().Lambda<Func<object?>>();

		@this.IsStatic.ThrowIfFalse();

		return @this.ToExpression(null).Cast<object>().Lambda<Func<object?>>();
	}
}
