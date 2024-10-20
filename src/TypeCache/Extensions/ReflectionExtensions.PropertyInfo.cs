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
	public static object? GetStaticValue(this PropertyInfo @this, ITuple? indexes = null)
		=> @this.GetStaticValueFunc()(indexes);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Func<ITuple?, object?> GetStaticValueFunc(this PropertyInfo @this)
	{
		@this.DeclaringType.ThrowIfNull();
		@this.GetMethod.ThrowIfNull();

		return TypeStore.StaticMethodTupleFuncs[(@this.DeclaringType.TypeHandle, @this.GetMethod.MethodHandle)];
	}

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Delegate GetValueDelegate(this PropertyInfo @this)
	{
		@this.DeclaringType.ThrowIfNull();
		@this.GetMethod.ThrowIfNull();

		return TypeStore.Delegates[(@this.DeclaringType.TypeHandle, @this.GetMethod.MethodHandle)];
	}

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.GetValue(object?)"/></c> or <c><see cref="PropertyInfo.GetValue(object?, object?[]?)"/></c> for added performance improvement.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static object? GetValueEx(this PropertyInfo @this, object instance, ITuple? indexes = null)
		=> @this.GetValueFunc()(instance, indexes);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Func<object, ITuple?, object?> GetValueFunc(this PropertyInfo @this)
	{
		@this.DeclaringType.ThrowIfNull();
		@this.GetMethod.ThrowIfNull();

		return TypeStore.MethodTupleFuncs[(@this.DeclaringType.TypeHandle, @this.GetMethod.MethodHandle)];
	}

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object, object[])"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetStaticValue(this PropertyInfo @this, object? value)
		=> @this.SetStaticValueAction()(ValueTuple.Create(value));

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object, object[])"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <param name="indexesAndValue">The last field of the tuple should be the value to set the property to./param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetStaticValue(this PropertyInfo @this, ITuple indexesAndValue)
		=> @this.SetStaticValueAction()(indexesAndValue);

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Action<ITuple?> SetStaticValueAction(this PropertyInfo @this)
	{
		@this.DeclaringType.ThrowIfNull();
		@this.SetMethod.ThrowIfNull();

		return TypeStore.StaticMethodTupleActions[(@this.DeclaringType.TypeHandle, @this.SetMethod.MethodHandle)];
	}

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object)"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static void SetValueEx(this PropertyInfo @this, object instance, object? value)
		=> @this.SetValueAction()(instance, ValueTuple.Create(value));

	/// <summary>
	/// Call this instead of <c><see cref="PropertyInfo.SetValue(object, object, object[])"/></c> for added performance improvement.<br/>
	/// Values are automatically converted when possible: ie. "123" ---> 123.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <param name="indexesAndValue">The last field of the tuple should be the value to set the property to./param>
	public static void SetValueEx(this PropertyInfo @this, object instance, ITuple indexesAndValue)
	{
		var indexParameters = @this.GetIndexParameters();
		indexParameters.ThrowIfNull();
		indexParameters.ThrowIfEmpty();

		@this.SetValueAction()(instance, indexesAndValue);
	}

	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Action<object, ITuple?> SetValueAction(this PropertyInfo @this)
	{
		@this.DeclaringType.ThrowIfNull();
		@this.SetMethod.ThrowIfNull();

		return TypeStore.MethodTupleActions[(@this.DeclaringType.TypeHandle, @this.SetMethod.MethodHandle)];
	}

	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Delegate SetValueDelegate(this PropertyInfo @this)
	{
		@this.SetMethod.ThrowIfNull();
		@this.SetMethod.DeclaringType.ThrowIfNull();

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
		@this.ThrowIfNull();
		@this.DeclaringType.ThrowIfNull();
		@this.SetMethod.ThrowIfNull();
		@this.SetMethod.IsStatic.ThrowIfTrue();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression value = nameof(instance).ToParameterExpression<object?>();
		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty(Item, INSTANCE_BINDING_FLAGS)!;
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
		@this.ThrowIfNull();
		@this.DeclaringType.ThrowIfNull();
		@this.GetMethod.ThrowIfNull();
		@this.GetMethod.IsStatic.ThrowIfTrue();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty(Item, INSTANCE_BINDING_FLAGS)!;
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
		@this.ThrowIfNull();
		@this.DeclaringType.ThrowIfNull();
		@this.SetMethod.ThrowIfNull();
		@this.SetMethod.IsStatic.ThrowIfFalse();

		ParameterExpression value = nameof(value).ToParameterExpression<object?>();
		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty(Item, INSTANCE_BINDING_FLAGS)!;
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
		@this.ThrowIfNull();
		@this.DeclaringType.ThrowIfNull();
		@this.GetMethod.ThrowIfNull();
		@this.GetMethod.IsStatic.ThrowIfFalse();

		ParameterExpression index = nameof(index).ToParameterExpression<ITuple?>();
		var itemProperty = typeof(ITuple).GetProperty(Item, INSTANCE_BINDING_FLAGS)!;
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
