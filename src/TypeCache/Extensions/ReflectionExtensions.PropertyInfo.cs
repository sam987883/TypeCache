// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.GetValue(object?)"/></c> or <c><see cref="PropertyInfo.GetValue(object?, object?[]?)"/></c> for added performance improvement.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? GetStaticValue(this PropertyInfo @this, ITuple? index = null)
		=> @this.GetStaticValueFunc()(index);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Func<ITuple?, object?> GetStaticValueFunc(this PropertyInfo @this)
	{
		@this.DeclaringType.AssertNotNull();
		@this.GetMethod.AssertNotNull();

		return TypeStore.StaticPropertyFuncs[(@this.DeclaringType.TypeHandle, @this.Name)];
	}

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Delegate GetValueDelegate(this PropertyInfo @this)
	{
		@this.DeclaringType.AssertNotNull();
		@this.GetMethod.AssertNotNull();

		return TypeStore.Delegates[(@this.DeclaringType.TypeHandle, @this.GetMethod.MethodHandle)];
	}

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.GetValue(object?)"/></c> or <c><see cref="PropertyInfo.GetValue(object?, object?[]?)"/></c> for added performance improvement.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? GetValueEx(this PropertyInfo @this, object instance, ITuple? index = null)
		=> @this.GetValueFunc()(instance, index);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Func<object, ITuple?, object?> GetValueFunc(this PropertyInfo @this)
	{
		@this.DeclaringType.AssertNotNull();
		@this.GetMethod.AssertNotNull();

		return TypeStore.PropertyFuncs[(@this.DeclaringType.TypeHandle, @this.Name)];
	}

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object, object[])"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetStaticValue(this PropertyInfo @this, object? value)
		=> @this.SetStaticValueAction()(null, value);

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object, object[])"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetStaticValue(this PropertyInfo @this, ITuple index, object? value)
		=> @this.SetStaticValueAction()(index, value);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Action<ITuple?, object?> SetStaticValueAction(this PropertyInfo @this)
	{
		@this.DeclaringType.AssertNotNull();
		@this.SetMethod.AssertNotNull();

		return TypeStore.StaticPropertyActions[(@this.DeclaringType.TypeHandle, @this.Name)];
	}

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object)"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetValueEx(this PropertyInfo @this, object instance, object? value)
		=> @this.SetValueAction()(instance, null, value);

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object, object[])"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static void SetValueWithIndex(this PropertyInfo @this, object instance, ITuple index, object? value)
		=> @this.SetValueAction()(instance, index, value);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Action<object, ITuple?, object?> SetValueAction(this PropertyInfo @this)
	{
		@this.DeclaringType.AssertNotNull();
		@this.SetMethod.AssertNotNull();

		return TypeStore.PropertyActions[(@this.DeclaringType.TypeHandle, @this.Name)];
	}

	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Delegate SetValueDelegate(this PropertyInfo @this)
	{
		@this.SetMethod.AssertNotNull();
		@this.SetMethod.DeclaringType.AssertNotNull();

		return TypeStore.Delegates[(@this.SetMethod.DeclaringType.TypeHandle, @this.SetMethod.MethodHandle)];
	}

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToExpression(this PropertyInfo @this, Expression? instance)
		=> Expression.Property(instance, @this);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression ToExpression(this PropertyInfo @this, Expression? instance, Expression[] indexes)
		=> Expression.Property(instance, @this, indexes);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Action<object, ITuple?, object?>> ToPropertyActionExpression(this PropertyInfo @this)
	{
		@this.AssertNotNull();
		@this.DeclaringType.AssertNotNull();
		@this.SetMethod.AssertNotNull();
		@this.SetMethod.IsStatic.AssertFalse();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression value = nameof(instance).ToParameterExpression<object?>();
		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty("Item", INSTANCE_BINDING_FLAGS)!;
		var parameters = @this.GetIndexParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select((parameterInfo, i) => (Expression)index.Property(itemProperty, [i.ToConstantExpression()]).Convert(parameterInfo.ParameterType))
			.ToArray();
		var call = instance.Cast(@this.DeclaringType).Property(@this, parameters).Assign(value.Convert(@this.PropertyType));

		return call.Lambda<Action<object, ITuple?, object?>>([instance, index, value]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Func<object, ITuple?, object?>> ToPropertyFuncExpression(this PropertyInfo @this)
	{
		@this.AssertNotNull();
		@this.DeclaringType.AssertNotNull();
		@this.GetMethod.AssertNotNull();
		@this.GetMethod.IsStatic.AssertFalse();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty("Item", INSTANCE_BINDING_FLAGS)!;
		var parameters = @this.GetIndexParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select((parameterInfo, i) => (Expression)index.Property(itemProperty, [i.ToConstantExpression()]).Convert(parameterInfo.ParameterType))
			.ToArray();

		return @this
			.ToExpression(instance.Cast(@this.DeclaringType), parameters)
			.Cast<object>()
			.Lambda<Func<object, ITuple?, object?>>([instance, index]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Action<ITuple?, object?>> ToStaticPropertyActionExpression(this PropertyInfo @this)
	{
		@this.AssertNotNull();
		@this.DeclaringType.AssertNotNull();
		@this.SetMethod.AssertNotNull();
		@this.SetMethod.IsStatic.AssertTrue();

		ParameterExpression value = nameof(value).ToParameterExpression<object?>();
		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty("Item", INSTANCE_BINDING_FLAGS)!;
		var parameters = @this.GetIndexParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select((parameterInfo, i) => (Expression)index.Property(itemProperty, [i.ToConstantExpression()]).Convert(parameterInfo.ParameterType))
			.ToArray();

		return @this
			.ToExpression(null, parameters)
			.Lambda<Action<ITuple?, object?>>([index, value]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Func<ITuple?, object?>> ToStaticPropertyFuncExpression(this PropertyInfo @this)
	{
		@this.AssertNotNull();
		@this.DeclaringType.AssertNotNull();
		@this.GetMethod.AssertNotNull();
		@this.GetMethod.IsStatic.AssertTrue();

		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty("Item", INSTANCE_BINDING_FLAGS)!;
		var parameters = @this.GetIndexParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select((parameterInfo, i) => (Expression)index.Property(itemProperty, [i.ToConstantExpression()]).Convert(parameterInfo.ParameterType))
			.ToArray();

		return @this
			.ToExpression(null, parameters)
			.Cast<object>()
			.Lambda<Func<ITuple?, object?>>([index]);
	}
}
