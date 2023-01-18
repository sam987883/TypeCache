// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	public static Expression<Func<object?[]?, object?>> ToInvokeLambdaExpression(this ConstructorInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object[]>();
		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select((parameterInfo, i) => arguments.Array()[i].Convert(parameterInfo.ParameterType))
			.ToArray();

		return @this.ToNewExpression(parameters).As<object>().Lambda<Func<object?[]?, object?>>(arguments);
	}

	public static LambdaExpression ToLambdaExpression(this ConstructorInfo @this)
	{
		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => parameterInfo!.ToParameterExpression())
			.ToArray();

		return @this.ToNewExpression(parameters).Lambda(parameters);
	}

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToNewExpression(this ConstructorInfo @this, IEnumerable<Expression> parameters)
		=> Expression.New(@this, parameters);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToNewExpression(this ConstructorInfo @this, params Expression[]? parameters)
		=> Expression.New(@this, parameters);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, IEnumerable{MemberInfo})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToNewExpression(this ConstructorInfo @this, IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
		=> Expression.New(@this, parameters, memberInfos);

	/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, MemberInfo[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToNewExpression(this ConstructorInfo @this, IEnumerable<Expression> parameters, params MemberInfo[]? memberInfos)
		=> Expression.New(@this, parameters, memberInfos);
}
