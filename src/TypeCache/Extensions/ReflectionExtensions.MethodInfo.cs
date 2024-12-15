// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Delegate? GetDelegate(this MethodInfo @this)
		=> TypeStore.Delegates[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For non-static methods that return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Action<object, object?[]?>? GetArrayAction(this MethodInfo @this)
		=> TypeStore.MethodArrayActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For non-static methods that return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<object, object?[]?, object?>? GetArrayFunc(this MethodInfo @this)
		=> TypeStore.MethodArrayFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For non-static methods that return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Action<object, ITuple?>? GetTupleAction(this MethodInfo @this)
		=> TypeStore.MethodTupleActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For non-static methods that return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<object, ITuple?, object?>? GetTupleFunc(this MethodInfo @this)
		=> TypeStore.MethodTupleFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For static methods that do not return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Action<object?[]?>? GetStaticArrayAction(this MethodInfo @this)
		=> TypeStore.StaticMethodArrayActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For static methods that return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<object?[]?, object?>? GetStaticArrayFunc(this MethodInfo @this)
		=> TypeStore.StaticMethodArrayFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For static methods that do not return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Action<ITuple?>? GetStaticTupleAction(this MethodInfo @this)
		=> TypeStore.StaticMethodTupleActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <summary>
	/// For static methods that return a value.
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<ITuple?, object?>? GetStaticTupleFunc(this MethodInfo @this)
		=> TypeStore.StaticMethodTupleFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <remarks>
	/// <c>@<paramref name="this"/>.ReturnType.IsAny([<see langword="typeof"/>(Task), <see langword="typeof"/>(ValueTask), <see langword="typeof"/>(void)]);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasNoReturnValue(this MethodInfo @this)
		=> @this.ReturnType.IsAny([typeof(Task), typeof(ValueTask), typeof(void)]);

	/// <summary>
	/// For non-static methods that do not return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void InvokeAction(this MethodInfo @this, object instance, object?[]? arguments)
	{
		var action = TypeStore.MethodArrayActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		action.ThrowIfNull();
		action.Invoke(instance, arguments);
	}

	/// <summary>
	/// For non-static methods that do not return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void InvokeAction(this MethodInfo @this, object instance, ITuple? arguments)
	{
		var action = TypeStore.MethodTupleActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		action.ThrowIfNull();
		action.Invoke(instance, arguments);
	}

	/// <summary>
	/// For non-static methods that return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static object? InvokeFunc(this MethodInfo @this, object instance, object?[]? arguments)
	{
		var func = TypeStore.MethodArrayFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		func.ThrowIfNull();
		return func.Invoke(instance, arguments);
	}

	/// <summary>
	/// For non-static methods that return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static object? InvokeFunc(this MethodInfo @this, object instance, ITuple? arguments)
	{
		var func = TypeStore.MethodTupleFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		func.ThrowIfNull();
		return func.Invoke(instance, arguments);
	}

	/// <summary>
	/// For static methods that do not return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void InvokeStaticAction(this MethodInfo @this, object?[]? arguments)
	{
		var action = TypeStore.StaticMethodArrayActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		action.ThrowIfNull();
		action.Invoke(arguments);
	}

	/// <summary>
	/// For static methods that do not return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void InvokeStaticAction(this MethodInfo @this, ITuple? arguments)
	{
		var action = TypeStore.StaticMethodTupleActions[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		action.ThrowIfNull();
		action.Invoke(arguments);
	}

	/// <summary>
	/// For static methods that return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static object? InvokeStaticFunc(this MethodInfo @this, object?[]? arguments)
	{
		var func = TypeStore.StaticMethodArrayFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		func.ThrowIfNull();
		return func.Invoke(arguments);
	}

	/// <summary>
	/// For static methods that return a value.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static object? InvokeStaticFunc(this MethodInfo @this, ITuple? arguments)
	{
		var func = TypeStore.StaticMethodTupleFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		func.ThrowIfNull();
		return func.Invoke(arguments);
	}

	/// <remarks>
	/// <c>=&gt; ((<see cref="MethodBase"/>)@<paramref name="this"/>).IsInvokable() &amp;&amp; @<paramref name="this"/>.ReturnType.IsInvokable();</c>
	/// </remarks>
	[DebuggerHidden]
	internal static bool IsInvokable(this MethodInfo @this)
		=> ((MethodBase)@this).IsInvokable() && @this.ReturnType.IsInvokable();

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

	/// <inheritdoc cref="MethodInfo.MakeGenericMethod(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.MakeGenericMethod(<see langword="typeof"/>(<typeparamref name="T1"/>),
	/// <see langword="typeof"/>(<typeparamref name="T2"/>),
	/// <see langword="typeof"/>(<typeparamref name="T3"/>),
	/// <see langword="typeof"/>(<typeparamref name="T4"/>),
	/// <see langword="typeof"/>(<typeparamref name="T5"/>),
	/// <see langword="typeof"/>(<typeparamref name="T6"/>),
	/// <see langword="typeof"/>(<typeparamref name="T7"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodInfo MakeGenericMethod<T1, T2, T3, T4, T5, T6, T7>(this MethodInfo @this)
		=> @this.MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Action<object, object?[]?>> ToArrayActionExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfTrue();
		EqualityComparer<Type>.Default.ThrowIfNotEqual(@this.ReturnType, typeof(void));
		@this.ReturnType.ThrowIfNotEqual(typeof(void));

		ParameterExpression instance = nameof(instance).ParameterExpression<object>();
		ParameterExpression arguments = nameof(arguments).ParameterExpression<object?[]?>();
		var call = @this.CreateArrayCall(instance, arguments);

		return call.Lambda<Action<object, object?[]?>>([instance, arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Func<object, object?[]?, object?>> ToArrayFuncExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfTrue();
		@this.ReturnType.ThrowIfEqual(typeof(void));

		ParameterExpression instance = nameof(instance).ParameterExpression<object>();
		ParameterExpression arguments = nameof(arguments).ParameterExpression<object?[]?>();
		var call = @this.CreateArrayCall(instance, arguments);

		return call.Cast<object>().Lambda<Func<object, object?[]?, object?>>([instance, arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static LambdaExpression ToDelegateExpression(this MethodInfo @this)
	{
		if (!@this.IsStatic)
			@this.DeclaringType.ThrowIfNull();

		ParameterExpression instance = nameof(instance).ParameterExpression(@this.DeclaringType!);
		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => parameterInfo!.ToExpression())
			.ToArray();

		return !@this.IsStatic
			? instance.Call(@this, parameters).Lambda([instance, ..parameters])
			: @this.ToExpression(null, parameters).Lambda(parameters);
	}

	/// <inheritdoc cref="Expression.Call(MethodInfo, IEnumerable{Expression}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToExpression(this MethodInfo @this, Expression? instance, IEnumerable<Expression> arguments)
		=> Expression.Call(instance, @this, arguments);

	/// <inheritdoc cref="Expression.Call(MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="instance"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static method call.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToExpression(this MethodInfo @this, Expression? instance, Expression[]? arguments = null)
		=> Expression.Call(instance, @this, arguments);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Action<object?[]?>> ToStaticArrayActionExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfFalse();
		@this.ReturnType.ThrowIfNotEqual(typeof(void));

		ParameterExpression arguments = nameof(arguments).ParameterExpression<object?[]?>();
		var call = @this.CreateStaticArrayCall(arguments);

		return call.Lambda<Action<object?[]?>>([arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Func<object?[]?, object?>> ToStaticArrayFuncExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfFalse();
		@this.ReturnType.ThrowIfEqual(typeof(void));

		ParameterExpression arguments = nameof(arguments).ParameterExpression<object?[]?>();
		var call = @this.CreateStaticArrayCall(arguments);

		return call.Cast<object>().Lambda<Func<object?[]?, object?>>([arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Action<ITuple?>> ToStaticTupleActionExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfFalse();
		@this.ReturnType.ThrowIfNotEqual(typeof(void));

		ParameterExpression arguments = nameof(arguments).ParameterExpression<ITuple?>();
		var call = @this.CreateStaticTupleCall(arguments);

		return call.Lambda<Action<ITuple?>>([arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Func<ITuple?, object?>> ToStaticTupleFuncExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfFalse();
		@this.ReturnType.ThrowIfEqual(typeof(void));

		ParameterExpression arguments = nameof(arguments).ParameterExpression<ITuple?>();
		var call = @this.CreateStaticTupleCall(arguments);

		return call.Cast<object>().Lambda<Func<ITuple?, object?>>([arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Action<object, ITuple?>> ToTupleActionExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfTrue();
		@this.ReturnType.ThrowIfNotEqual(typeof(void));

		ParameterExpression instance = nameof(instance).ParameterExpression<object>();
		ParameterExpression arguments = nameof(arguments).ParameterExpression<ITuple?>();
		var call = @this.CreateTupleCall(instance, arguments);

		return call.Lambda<Action<object, ITuple?>>([instance, arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Expression<Func<object, ITuple?, object?>> ToTupleFuncExpression(this MethodInfo @this)
	{
		@this.IsStatic.ThrowIfTrue();
		@this.ReturnType.ThrowIfEqual(typeof(void));

		ParameterExpression instance = nameof(instance).ParameterExpression<object>();
		ParameterExpression arguments = nameof(arguments).ParameterExpression<ITuple?>();
		var call = @this.CreateTupleCall(instance, arguments);

		return call.Cast<object>().Lambda<Func<object, ITuple?, object?>>([instance, arguments]);
	}

	private static Expression CreateStaticArrayCall(this MethodInfo @this, ParameterExpression arguments)
	{
		var parameters = @this
			.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => arguments.Array(parameterInfo.Position).Convert(parameterInfo.ParameterType));

		return @this.ToExpression(null, parameters);
	}

	private static Expression CreateArrayCall(this MethodInfo @this, ParameterExpression instance, ParameterExpression arguments)
	{
		@this.DeclaringType.ThrowIfNull();

		var parameters = @this
			.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => arguments.Array(parameterInfo.Position).Convert(parameterInfo.ParameterType));

		return @this.ToExpression(instance.Cast(@this.DeclaringType), parameters);
	}

	private static Expression CreateStaticTupleCall(this MethodInfo @this, ParameterExpression arguments)
	{
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Length == 0)
			return @this.ToExpression(null);

		parameterInfos = @this.GetParameters().OrderBy(parameterInfo => parameterInfo.Position).ToArray();
		var valueTupleType = GetValueTupleType(parameterInfos);
		var valueTupleFields = GetValueTupleFields(arguments.Cast(valueTupleType), parameterInfos);
		var parameters = parameterInfos.Select(parameterInfo =>
			arguments.Property(Item, [parameterInfo.Position.ConstantExpression()]).Convert(parameterInfo.ParameterType));

		return arguments.Is(valueTupleType).IIf(@this.ToExpression(null, valueTupleFields), @this.ToExpression(null, parameters));
	}

	private static Expression CreateTupleCall(this MethodInfo @this, ParameterExpression instance, ParameterExpression arguments)
	{
		@this.DeclaringType.ThrowIfNull();

		var typedInstance = instance.Cast(@this.DeclaringType);
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Length == 0)
			return typedInstance.Call(@this);

		parameterInfos = @this.GetParameters().OrderBy(parameterInfo => parameterInfo.Position).ToArray();
		var valueTupleType = GetValueTupleType(parameterInfos);
		var valueTupleFields = GetValueTupleFields(arguments.Cast(valueTupleType), parameterInfos);
		var parameters = parameterInfos.Select(parameterInfo =>
			arguments.Property(Item, [parameterInfo.Position.ConstantExpression()]).Convert(parameterInfo.ParameterType));

		return arguments.Is(valueTupleType).IIf(typedInstance.Call(@this, valueTupleFields), typedInstance.Call(@this, parameters));
	}

	private static IEnumerable<Expression> GetValueTupleFields(Expression tupleExpression, ParameterInfo[] parameterInfos)
	{
		var i = 0;
		foreach (var parameterInfo in parameterInfos)
		{
			if (++i == 8)
			{
				tupleExpression = tupleExpression.Field(Rest);
				i = 1;
			}

			yield return tupleExpression.Field(Item + i);
		}
	}

	private static Type GetValueTupleType(ParameterInfo[] parameterInfos)
	{
		var parameterTypeChunks = parameterInfos
			.Select(parameterInfo => parameterInfo.ParameterType)
			.Chunk(7)
			.ToArray();
		var valueTupleTypeStack = new Stack<Type>(parameterTypeChunks.Select((types, i) => getValueTupleGenericType((i < types.Length - 1) ? 8 : types.Length)));
		var parameterTypeStack = new Stack<Type[]>(parameterTypeChunks);
		var valueTupleType = valueTupleTypeStack.Pop().MakeGenericType(parameterTypeStack.Pop());
		while (valueTupleTypeStack.Count > 0)
		{
			valueTupleType = valueTupleTypeStack.Pop().MakeGenericType([..parameterTypeStack.Pop(), valueTupleType]);
		}

		return valueTupleType;

		static Type getValueTupleGenericType(int arity)
			=> arity switch
			{
				1 => typeof(ValueTuple<>),
				2 => typeof(ValueTuple<,>),
				3 => typeof(ValueTuple<,,>),
				4 => typeof(ValueTuple<,,,>),
				5 => typeof(ValueTuple<,,,,>),
				6 => typeof(ValueTuple<,,,,,>),
				7 => typeof(ValueTuple<,,,,,,>),
				_ => typeof(ValueTuple<,,,,,,,>),
			};
	}
}
