// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <remarks>
	/// <c>@<paramref name="this"/>.ReturnType.IsAny(<see langword="typeof"/>(Task), <see langword="typeof"/>(ValueTask), <see langword="typeof"/>(void));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasNoReturnValue(this MethodInfo @this)
		=> @this.ReturnType.IsAny(typeof(Task), typeof(ValueTask), typeof(void));

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

	public static Expression<Func<object?[]?, object?>> ToInvokeLambdaExpression(this MethodInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object[]>();
		var parameterInfos = @this.GetParameters().OrderBy(parameterInfo => parameterInfo.Position);
		var parameters = @this.IsStatic
			? parameterInfos.Select((parameterInfo, i) => arguments.Array()[i].Convert(parameterInfo.ParameterType))
			: parameterInfos.Select((parameterInfo, i) => arguments.Array()[i + 1].Convert(parameterInfo.ParameterType));
		var call = @this.IsStatic
			? @this.ToStaticMethodCallExpression(parameters)
			: arguments.Array()[0].Cast(@this.DeclaringType!).Call(@this, parameters);

		return @this.ReturnType != typeof(void)
			? call.As<object>().Lambda<Func<object?[]?, object?>>(arguments)
			: call.Block(Expression.Constant(null)).Lambda<Func<object?[]?, object?>>(arguments);
	}

	public static LambdaExpression ToLambdaExpression(this MethodInfo @this)
	{
		ParameterExpression instance = nameof(instance).ToParameterExpression(@this.DeclaringType!);
		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => parameterInfo!.ToExpression())
			.ToArray();

		return !@this.IsStatic
			? instance.Call(@this, parameters).Lambda(parameters.Prepend(instance))
			: @this.ToStaticMethodCallExpression(parameters).Lambda(parameters);
	}

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this MethodInfo @this)
		=> Expression.Call(@this);

	/// <inheritdoc cref="Expression.Call(MethodInfo, IEnumerable{Expression}?)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this MethodInfo @this, IEnumerable<Expression> arguments)
		=> Expression.Call(@this, arguments);

	/// <inheritdoc cref="Expression.Call(MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression ToStaticMethodCallExpression(this MethodInfo @this, params Expression[]? arguments)
		=> Expression.Call(@this, arguments);
}
