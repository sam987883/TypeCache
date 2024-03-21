// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	public static Expression<Func<object?[]?, object?>> ToFuncExpression(this ConstructorInfo @this)
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object[]>();
		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select((parameterInfo, i) => arguments.Array()[i].Convert(parameterInfo.ParameterType))
			.ToArray();

		return @this.ToExpression(parameters).As<object>().Lambda<Func<object?[]?, object?>>(new[] { arguments });
	}

	public static LambdaExpression ToLambdaExpression(this ConstructorInfo @this)
	{
		var parameters = @this.GetParameters()
			.OrderBy(parameterInfo => parameterInfo.Position)
			.Select(parameterInfo => parameterInfo!.ToExpression())
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
}
