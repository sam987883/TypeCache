// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Delegate GetDelegate(this ConstructorInfo @this)
		=> TypeStore.Delegates[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Func<object?[]?, object?>? GetFunc(this ConstructorInfo @this)
		=> TypeStore.ConstructorArrayFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];

	/// <exception cref="ArgumentNullException"/>
	public static object InvokeFunc(this ConstructorInfo @this, object?[]? arguments)
	{
		var func = TypeStore.ConstructorArrayFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		func.AssertNotNull();
		return func.Invoke(arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	public static object InvokeFunc(this ConstructorInfo @this, ITuple? arguments)
	{
		var func = TypeStore.ConstructorTupleFuncs[(@this.DeclaringType!.TypeHandle, @this.MethodHandle)];
		func.AssertNotNull();
		return func.Invoke(arguments);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static LambdaExpression ToDelegateExpression(this ConstructorInfo @this)
	{
		@this.DeclaringType.AssertNotNull();

		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => parameterInfo.ToExpression())
			.ToArray();

		return @this.ToExpression(parameters).Lambda(parameters);
	}

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToExpression(this ConstructorInfo @this, IEnumerable<Expression> parameters)
		=> Expression.New(@this, parameters);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToExpression(this ConstructorInfo @this, Expression[]? parameters = null)
		=> Expression.New(@this, parameters);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, IEnumerable{MemberInfo})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToExpression(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
		=> Expression.New(@this, parameters, memberInfos);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, MemberInfo[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToExpression(this ConstructorInfo @this, IEnumerable<Expression> parameters, MemberInfo[]? memberInfos = null)
		=> Expression.New(@this, parameters, memberInfos);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static Expression<Func<object?[]?, object>> ToArrayFuncExpression(this ConstructorInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object?[]?>();
		var call = @this.CreateArrayCall(arguments);

		return call.As<object>().Lambda<Func<object?[]?, object>>([arguments]);
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public static Expression<Func<ITuple?, object>> ToTupleFuncExpression(this ConstructorInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<ITuple?>();
		var call = @this.CreateTupleCall(arguments);

		return call.Cast<object>().Lambda<Func<ITuple?, object>>([arguments]);
	}

	private static Expression CreateArrayCall(this ConstructorInfo @this, ParameterExpression arguments)
	{
		@this.DeclaringType.AssertNotNull();

		var parameters = @this
			.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => arguments.Array(parameterInfo.Position).Convert(parameterInfo.ParameterType));

		return @this.ToExpression(parameters);
	}

	private static Expression CreateTupleCall(this ConstructorInfo @this, ParameterExpression arguments)
	{
		var parameterInfos = @this.GetParameters().OrderBy(parameterInfo => parameterInfo.Position).ToArray();
		var valueTupleType = GetValueTupleType(parameterInfos);
		var valueTupleFields = GetValueTupleFields(arguments.Cast(valueTupleType), parameterInfos);
		var parameters = parameterInfos.Select(parameterInfo =>
			arguments.Property("Item", [parameterInfo.Position.ToConstantExpression()]).Convert(parameterInfo.ParameterType));

		return arguments.Is(valueTupleType).IIf(@this.ToExpression(valueTupleFields), @this.ToExpression(parameters));
	}
}
