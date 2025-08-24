// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;

namespace TypeCache.Extensions;

public static partial class ExpressionExtensions
{
	/// <inheritdoc cref="Expression.New(ConstructorInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.New(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static NewExpression ToExpression(this ConstructorInfo @this)
		=> Expression.New(@this);

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

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<paramref name="instance"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static field access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToExpression(this FieldInfo @this, Expression? instance)
		=> Expression.Field(instance, @this);

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

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ParameterExpression ToExpression(this ParameterInfo @this)
		=> Expression.Parameter(@this.ParameterType, @this.Name);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToExpression(this PropertyInfo @this, Expression? instance)
		=> Expression.Property(instance, @this);

	/// <inheritdoc cref="Expression.Property(Expression?, PropertyInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression ToExpression(this PropertyInfo @this, Expression? instance, IEnumerable<Expression> indexes)
		=> Expression.Property(instance, @this, indexes);

	/// <inheritdoc cref="Expression.Property(Expression?, PropertyInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression ToExpression(this PropertyInfo @this, Expression? instance, Expression[] indexes)
		=> Expression.Property(instance, @this, indexes);

	/// <inheritdoc cref="Expression.Default(Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Default(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DefaultExpression ToDefaultExpression(this Type @this)
		=> Expression.Default(@this);

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(<see langword="typeof"/>(<typeparamref name="T"/>), @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ParameterExpression ToParameterExpression<T>(this string @this)
		=> Expression.Parameter(typeof(T), @this);

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(<paramref name="type"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ParameterExpression ToParameterExpression(this string @this, Type type)
		=> Expression.Parameter(type, @this);

	/// <inheritdoc cref="Expression.Field(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticFieldExpression(this Type @this, string name)
		=> Expression.Field(null, @this, name);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticPropertyExpression(this Type @this, string name)
		=> Expression.Property(null, @this, name);
}
